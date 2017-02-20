// Global overlay (if not preset in element)
Vue.$loading.register(500, function (target) {
    target.classList.add('is-loading');
    target.disabled = true;
    return function () {
        target.classList.remove('is-loading');
        target.disabled = false;
    }
});