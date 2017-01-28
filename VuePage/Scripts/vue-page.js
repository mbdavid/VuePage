(function () {

    var container = document.querySelector('.vue-container');

    if (container == null) alert('.vue-container class not found');

    var options = JSON.parse(container.getAttribute('data-options') || '{}');
    var loading = new Loading(container, 0);

    // register Vue.$loading function
    Vue.$loading = loading.register;

    // register vue plugin to server call (vue.$server)
    Vue.use({
        install: function (vue) {

            var _queue = [];
            var _running = false;

            // Request new server call
            vue.prototype.$server = function $server(name, params, files, vm) {
                var target = (event ? event.target : null) || null;
                _queue.push({ target: target, name: name, params: params, files: files, vm: vm });
                if (!_running) nextQueue();
            }

            // Execute queue
            function nextQueue() {

                if (_running === false) _running = true;

                // get first request from queue
                var request = _queue.shift();

                loading.start(request.target);

                setTimeout(function () {

                    ajax(request, function () {

                        loading.stop();

                        // if no more items in queue, stop running
                        if (_queue.length === 0) return _running = false;

                        nextQueue();
                    });

                }, 1);
            }

            // Execute ajax POST request for update model
            function ajax(options, finish) {

                var xhr = new XMLHttpRequest();
                var files = [];

                xhr.onload = function () {
                    if (xhr.status < 200 || xhr.status >= 400) {
                        _queue = [];
                        _running = false;
                        loading.stop();
                        options.vm.$el.innerHTML = xhr.responseText;
                        return;
                    }

                    var response = JSON.parse(xhr.responseText);
                    var update = response['update'];
                    var js = response['js'];

                    // empty file inputs (if exists)
                    files.forEach(function (f) { f.value = ''; });

                    Object.keys(update).forEach(function (key) {
                        var value = update[key];
                        console.log('  $data["' + key + '"] = ', value);
                        options.vm.$data[key] = value;
                    });

                    if (js) {
                        console.log('  Eval = ', response['js']);
                        setTimeout(function () {
                            new Function(js).call(options.vm);
                        })
                    }

                    finish();
                };

                // create form with all data
                var form = new FormData();

                form.append('_method', options.name);
                form.append('_params', JSON.stringify(options.params));
                form.append('_model', JSON.stringify(options.vm.$data));

                // select elements with for upload file
                if (options.files) {
                    files = options.vm.$el.querySelectorAll(options.files);
                    files.forEach(function (file) {
                        for (var i = 0; i < file.files.length; i++) {
                            form.append('_files', file.files[i]);
                            console.log('Uploading ("' + file.files[i].name + ')... ' + file.files[i].size);
                        }
                    });
                }

                console.log('Execute ("' + options.name + '") = ', options.params);

                xhr.open('POST', location.href, true);
                xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                xhr.send(form);
            }
        }
    });

    // capture any link click to use "navToPage" method
    // do not capture if link contains "target" attribute or is not href page
    document.addEventListener('click', function (e) {

        if (e.target && e.target.tagName === 'A') {
            var href = e.target.href;
            if (href.startsWith('http') && !e.target.target) {
                e.stopPropagation();
                e.preventDefault();
                navToPage(e.target.href, e.target.getAttribute('data-transition') || options.defaultTransition);
                return false;
            }
        }

    });

    // navigate to page
    window.navToPage = function (href, transition) {
        
        var url = resolveUrl(href);
        var hash = /#.*$/.test(url) ? url.match(/#.*$/)[0] : '';
        url = url.replace(/#.*$/, '');

        console.log('Navigate to page: ' + url);

        // check if page already exists in container (get from url)
        var current = document.querySelector('.vue-page-active');
        var page = document.querySelector('.vue-page[data-url="' + url + '"]');

        // if navToPage came from Back button, do not push state
        if (location.href != url) {
            history.pushState(null, null, url);
        }

        if (/^#restore$/i.test(hash)) {
            // if restoring and page are in stack in container, just toggle
            if (page != null) {
                current.classList.remove('vue-page-active');
                page.classList.add('vue-page-active');
                doTransition(transition, current, page, autofocus)
                return;
            }
        }
        else {
            // if pages already exists, remove/destroy first
            if (page != null) {
                page.__vue__.$destroy();
                container.removeChild(page);
                page = null;
            }
        }

        // avoid too many pages in memory
        var pages = container.querySelectorAll('.vue-page');

        if (pages.length > options.history) {
            pages[0].__vue__.$destroy();
            container.removeChild(pages[0]);
        }

        // create new page
        page = document.createElement('div');
        page.classList.add('vue-page');
        page.setAttribute('data-url', url);
        container.appendChild(page);

        var xhr = new XMLHttpRequest();

        xhr.onload = function () {

            loading.stop();

            if (xhr.status < 200 || xhr.status >= 400) {
                current.innerHTML = xhr.responseText;
                return;
            }

            var response = xhr.responseText;
            var scripts = /<script\b[^>]*>([\s\S]*?)<\/script>/gm.exec(response);
            var html = response.replace(/<script\b[^>]*>([\s\S]*?)<\/script>/g, '');

            // set new page content
            page.innerHTML = html;

            // toggle active page
            current.classList.remove('vue-page-active');
            page.classList.add('vue-page-active');

            // evaluate new vue instance
            setTimeout(function () {
                new Function(scripts[1]).call(window);
            });

            doTransition(transition, current, page, autofocus);

        };

        loading.start(null);

        xhr.open('GET', url, true);
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        xhr.send();
    }

    // implement page navigation transition
    function doTransition(transition, current, page, onEnd) {

        if (!transition) return onEnd();

        var fn = function () {
            current.classList.remove(transition + '-out');
            page.classList.remove(transition + '-in');
            document.querySelector('.vue-page-active').classList.remove(transition + '-in');
            onEnd();
        }

        current.removeEventListener('animationend', fn);
        current.addEventListener('animationend', fn);

        current.classList.add(transition + '-out');
        page.classList.add(transition + '-in');
    }

    // back to page
    window.addEventListener('popstate', function (e) {
        navToPage(location.href + '#restore', options.backTransition);
    });

    // Loading state machine (with delay)
    function Loading(container) {
        var _handles = [];
        // register new handle loading
        this.register = function (delay, start) {
            var handle = { delay: delay, start: start, state: 'stop', stop: null };
            _handles.push(handle);
            return function () {
                _handles.splice(_handles.indexOf(handle), 1);
            }
        }
        // start timer
        this.start = function (target) {
            _handles.forEach(function (handle) {
                if (handle.state !== 'stop') return;
                handle.state = 'waiting';
                setTimeout(function () {
                    if (handle.state !== 'waiting') return;
                    handle.stop = handle.start(target, container);
                    handle.state = 'start';
                }, handle.delay);
            });
        }
        // stop timer
        this.stop = function () {
            _handles.forEach(function (handle) {
                if (handle.state !== 'start') return handle.state = 'stop';
                handle.stop();
                handle.state = 'stop';
            });
        }
    }

    // execute autofocus
    function autofocus() {
        setTimeout(function () {
            var focus = document.querySelector('.vue-page-active [autofocus]');
            if (focus == null) return;
            try { focus.focus(); } catch (e) { }
        }, 1);
    }

    // get full path from href
    function resolveUrl(href) {
        var a = document.createElement('a');
        a.href = href;
        return a.href;
    }

})();