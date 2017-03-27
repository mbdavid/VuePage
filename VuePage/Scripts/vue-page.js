(function () {

    // register Vue.$loading function
    var _loading = Vue.$loading = new Loading();

    // register vue plugin to server call (vue.$update)
    Vue.use({
        install: function (vue) {

            var _queue = [];
            var _running = false;

            // in watch, test before call if field are not been updated
            vue.prototype.$updating = false;

            // request new server call
            vue.prototype.$update = function $update(name, params, files, vm) {

                var target = (event ? event.target : document.body) || document.body;

                return new Promise(function (resolve) {

                    _queue.push({
                        target: target,
                        name: name,
                        params: params,
                        files: files,
                        vm: vm,
                        resolve: resolve
                    });

                    if (!_running) nextQueue();
                });
            }

            // Execute queue
            function nextQueue() {

                if (_running === false) _running = true;

                // get first request from queue
                var request = _queue.shift();

                _loading.start(request.target);

                setTimeout(function () {

                    ajax(request, function () {

                        _loading.stop();

                        // resolve request promise
                        request.resolve(request.vm);

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
                        _loading.stop();
                        request.vm.$el.innerHTML = xhr.responseText;
                        return;
                    }

                    var response = JSON.parse(xhr.responseText);
                    var update = response['update'];
                    var js = response['js'];

                    // empty file inputs (if exists)
                    files.forEach(function (f) { f.value = ''; });

                    // server-side changes not call watch methods
                    request.vm.$updating = true;

                    Object.keys(update).forEach(function (key) {
                        var value = update[key];
                        log('>  $data["' + key + '"] = ', value);

                        // update viewmodel
                        request.vm.$data[key] = value;
                    });

                    request.vm.$nextTick(function () {
                        request.vm.$updating = false;
                    });


                    if (js) {
                        log('>  $eval = ', response['js']);
                        setTimeout(function () {
                            new Function(js).call(request.vm);
                        })
                    }

                    finish();
                };

                // create form with all data
                var form = new FormData();

                if (request.vm.$options.name) {
                    form.append('_name', request.vm.$options.name);
                }

                form.append('_method', request.name);
                form.append('_params', JSON.stringify(request.params));
                form.append('_model', JSON.stringify(request.vm.$data));

                // select elements with for upload file
                if (request.files) {
                    files = request.vm.$el.querySelectorAll(request.files);
                    files.forEach(function (file) {
                        for (var i = 0; i < file.files.length; i++) {
                            form.append('_files', file.files[i]);
                            log('$upload ("' + file.files[i].name + '")... ', file.files[i].size);
                        }
                    });
                }

                log('$update ("' + request.name + '") = ', request.params);

                xhr.open('POST', location.href, true);
                xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                xhr.send(form);
            }

            // Add css style function into vue instance
            vue.prototype.$addStyle = function (css) {
                var head = document.head || document.getElementsByTagName('head')[0];
                var style = document.createElement('style');

                style.type = 'text/css';
                if (style.styleSheet) {
                    style.styleSheet.cssText = css;
                } else {
                    style.appendChild(document.createTextNode(css));
                }

                head.appendChild(style);
            }
        }
    });

    // Load async component
    Vue.$loadComponent = function $loadComponent(name) {
        return function (resolve, reject) {
            var xhr = new XMLHttpRequest();

            xhr.onload = function () {
                if (xhr.status < 200 || xhr.status >= 400) {
                    alert('Error on load component: ' + name);
                    return;
                }

                var c = new Function(xhr.responseText);

                resolve(c());
            };
            //log('$loadComponent ("' + name + '")');

            xhr.open('GET', location.pathname + '?_name=' + name, true);
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            xhr.send();
        }
    };

    // Loading state machine (with delay)
    function Loading() {
        var _handles = [];
        // register new handle loading and returns a function thats remove this hander (un-register)
        // start function be called when need show and must return OnEnd handler function
        this.register = function (delay, start) {
            var handle = {
                delay: delay,
                start: start,
                stop: null,
                state: 'stop'
            };
            _handles.push(handle);

            // returing un-register function
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
                    handle.stop = handle.start(target || document.body);
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

    // execute console log without showing file: http://stackoverflow.com/questions/34762774/how-to-hide-source-of-log-messages-in-console
    function log() {
        setTimeout(console.log.bind(console, arguments[0], arguments[1] || ''));
    }

})();