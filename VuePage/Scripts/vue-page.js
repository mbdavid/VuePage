(function () {

    var page = document.querySelector('.vue-page');

    // if not found first page is missing <vue:App>
    if (page == null) return;

    var options = JSON.parse(page.getAttribute('data-options') || '{}');
    var loading = new Loading(0);

    // remove options
    page.removeAttribute('data-options');

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
            function ajax(request, finish) {

                var xhr = new XMLHttpRequest();
                var files = [];

                xhr.onload = function () {
                    if (xhr.status < 200 || xhr.status >= 400) {
                        _queue = [];
                        _running = false;
                        loading.stop();
                        request.vm.$el.innerHTML = xhr.responseText;
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
                        request.vm.$data[key] = value;
                    });

                    if (js) {
                        console.log('  Eval = ', response['js']);
                        setTimeout(function () {
                            new Function(js).call(request.vm);
                        })
                    }

                    finish();
                };

                // create form with all data
                var form = new FormData();

                form.append('_method', request.name);
                form.append('_params', JSON.stringify(request.params));
                form.append('_model', JSON.stringify(request.vm.$data));

                // select elements with for upload file
                if (request.files) {
                    files = request.vm.$el.querySelectorAll(request.files);
                    files.forEach(function (file) {
                        for (var i = 0; i < file.files.length; i++) {
                            form.append('_files', file.files[i]);
                            console.log('Uploading ("' + file.files[i].name + ')... ' + file.files[i].size);
                        }
                    });
                }

                console.log('Execute ("' + request.name + '") = ', request.params);

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

        // check if page already exists in body (get from url)
        var active = document.querySelector('.vue-page-active');
        var page = document.querySelector('.vue-page[data-url="' + url + '"]');

        // if navToPage came from Back button, do not push state
        if (location.href != url) {
            history.pushState(null, null, url);
        }

        if (/^#restore$/i.test(hash)) {
            // if restoring and page are in html, just toggle
            if (page != null) {
                // toogle class
                active.classList.remove('vue-page-active');
                page.classList.add('vue-page-active');

                // toogle display
                active.style.display = 'none';
                page.style.removeProperty('display');

                // do transition
                return doTransition(transition, active, page, autofocus);
            }
        }
        else {
            // if pages already exists, remove/destroy first
            if (page != null) {
                page.__vue__.$destroy();
                page.parentNode.removeChild(page);
                page = null;
            }
        }

        // avoid too many pages in memory
        var pages = document.querySelectorAll('.vue-page');

        if (pages.length > options.history) {
            var first = pages[0];
            first.__vue__.$destroy();
            first.parentNode.removeChild(first);
        }

        // create new page
        page = document.createElement('div');
        page.classList.add('vue-page');
        page.setAttribute('data-url', url);

        var xhr = new XMLHttpRequest();

        xhr.onload = function () {

            loading.stop();

            if (xhr.status < 200 || xhr.status >= 400) {
                active.innerHTML = xhr.responseText;
                return;
            }

            var response = xhr.responseText;
            var scripts = /<script\b[^>]*>([\s\S]*?)<\/script>/gm.exec(response);
            var html = response.replace(/<script\b[^>]*>([\s\S]*?)<\/script>/g, '');

            // set new page content
            page.innerHTML = html;

            // toggle class
            active.classList.remove('vue-page-active');
            page.classList.add('vue-page-active');

            // toggle display
            active.style.display = 'none';

            // insert new page after active page
            active.parentNode.insertBefore(page, active.nextSibling);

            // evaluate new vue instance
            setTimeout(function () {
                new Function(scripts[1]).call(window);
            });

            doTransition(transition, active, page, autofocus);

        };

        loading.start(null);

        xhr.open('GET', url, true);
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        xhr.send();
    }

    // implement page navigation transition
    function doTransition(transition, active, page, onEnd) {

        if (!transition) return onEnd();

        console.log('doTransition: ', transition)

        var fn = function () {
            console.log('doTransition(REMOVE): ', transition)
            document.querySelectorAll('.vue-page').forEach(function (el) {
                el.classList.remove(transition + '-out');
                el.classList.remove(transition + '-in');
            });
            onEnd();
        }

        page.removeEventListener('animationend', fn);
        page.addEventListener('animationend', fn);

        active.classList.add(transition + '-out');
        page.classList.add(transition + '-in');
    }

    // back to page
    window.addEventListener('popstate', function (e) {
        navToPage(location.href + '#restore', options.backTransition);
    });

    // Loading state machine (with delay)
    function Loading() {
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
                    handle.stop = handle.start(target);
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