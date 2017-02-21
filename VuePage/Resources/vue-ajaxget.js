(function () {

    // register Vue.$loading function
    var _loading = Vue.$loading;
    var _vm = null;

    // register vue plugin to server call ($navigate & $registerPageVM)
    Vue.use({
        install: function (vue) {

            // register this.$navigate function
            vue.prototype.$navigate = navigate;

            // register page view model (destroy before)
            vue.prototype.$registerPageVM = function (vm) {
                _vm = vm;
            }
        }
    });

    // navigate to page using HTTP-Request and HTML5 History
    function navigate(href) {

        var url = resolveUrl(href);

        log('$navigate: ' + url);

        // if navToPage came from Back button, do not push state
        if (location.href != url) {
            history.pushState(null, null, url);
        }

        // create new page
        var xhr = new XMLHttpRequest();

        xhr.onload = function () {

            _loading.stop();

            if (xhr.status < 200 || xhr.status >= 400) {
                document.body.innerHTML = xhr.responseText;
                return;
            }

            var response = xhr.responseText;
            var re = /(<script?\b[^>]*>([\s\S]*?)<\/script>)|(<style[^>]*>([\s\S]*?)<\/style>)/gm;
            var html = response.replace(re, '');
            var body = (/<body[^>]*>([\s\S]*)<\/body>/gm.exec(html) || [null, 'body tag not found'])[1];
            var title = (/<title[^>]*>([\s\S]*)<\/title>/gm.exec(html) || [null, ''])[1];
            var tags = re.exec(response);

            // destroy old vm
            _vm.$destroy();

            // set new page content
            document.body.innerHTML = body;
            document.title = title;

            // script/style queue to insert in order
            var queue = [];

            while (tags != null) {
                var type = /<script.*type=['"](.*?)['"]/g.exec(tags[0]);

                if (type === null || type[1] == "text/javascript") {
                    queue.push({ script: tags[2], style: tags[4] });
                }

                tags = re.exec(response);
            }

            // execute all scripts/styles in queue
            function exec() {

                if (queue.length == 0) {
                    return autofocus();
                }

                var item = queue.shift();

                if (item.style) {
                    header('style', function (t) { t.innerHTML = item.style });
                }
                else if (item.script) {
                    new Function(item.script).call(window);
                }
                exec();
            }

            // evaluate all scripts/styles instance
            setTimeout(exec, 0);
        };

        _loading.start();

        xhr.open('GET', url, true);
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        xhr.send();

    }

    // back to page using browser arrows
    window.addEventListener('popstate', function (e) {
        navigate(location.href);
    });

    // capture any link click to use "navigate" method
    // do not capture if link contains "target" attribute or is not href page
    document.addEventListener('click', function (e) {

        var el = e.target;

        while (el != null) {
            if (el.tagName === 'A') {
                var href = el.href;
                if (href.startsWith('http') && !el.target) {
                    e.stopPropagation();
                    e.preventDefault();
                    navigate(href);
                    return false;
                }
            }
            el = el.parentNode;
        }

    });

    // get full path from href
    function resolveUrl(href) {
        var a = document.createElement('a');
        a.href = href;
        return a.href;
    }

    // create new tag and append into header
    function header(tagName, fn) {
        var tag = document.createElement(tagName);
        fn(tag);
        document.querySelector('head').appendChild(tag);
    }

    // execute autofocus
    function autofocus() {
        setTimeout(function () {
            var focus = document.querySelector('[autofocus]');
            if (focus == null) return;
            try { focus.focus(); } catch (e) { }
        }, 1);
    }

    // execute console log without showing file
    function log() {
        setTimeout(console.log.bind(console, arguments[0], arguments[1] || ''));
    }

})();