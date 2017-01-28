// default enter directive "v-default-enter"
Vue.directive('default-enter', function (el, binding) {
    if (binding.value === false) {
        el.classList.remove('vue-default-enter')
    }
    else {
        el.classList.add('vue-default-enter');
    }
});

// capture default enter
document.body.addEventListener('keydown', function (e) {

    if (e.which !== 13) return;

    var page = document.querySelector('.vue-page-active');
    var el = page.querySelector('.vue-default-enter');
    var target = e.target || { tagName: '', type: '' };

    // if focus control is a button, trigger current control button
    if (target.tagName == 'INPUT' && (target.type === 'button' || target.type === 'submit' || target.type === 'image')) return;
    if (target.tagName == 'A' || target.tagName == 'BUTTON') return;

    if (el != null) {
        e.preventDefault();
        e.stopPropagation();
        if (!el.disabled) el.click();
        return false;
    }
});

// stop submiting ASP.NET 
(function () {
    var form = document.querySelector('form');
    if (form != null) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            e.stopPropagation();
            return false;
        });
    }
})();
