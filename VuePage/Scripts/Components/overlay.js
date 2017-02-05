// overlay and disabled button
Vue.directive('overlay', {
    bind: function (el, binding) {
        el.setAttribute('data-overlay', 'true');
        binding.removeAjaxLoading = Vue.$loading(10, function (target) {
            if (target == el) {
                el.disabled = true;
            }
            return function () {
                if (target == el) {
                    el.disabled = false;
                }
            }
        })
    },
    unbind: function (el, binding) {
        binding.removeAjaxLoading();
    }
});

// Global overlay (if not preset in element)
Vue.$loading(500, function (target) {
    var show = (target == null || target.getAttribute('data-overlay') !== 'true');
    var panel = null;
    if (show) {
        panel = document.createElement('div');
        panel.className = 'overlay';
        target.appendChild(panel);
    }

    return function () {
        if (!show) return;
        target.removeChild(panel);
    }

})
