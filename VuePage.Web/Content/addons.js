// Global overlay (if not preset in element)
Vue.$loading.register(500, function (target) {
    target.classList.add('is-loading');
    target.disabled = true;
    return function () {
        target.classList.remove('is-loading');
        target.disabled = false;
    }
});

Vue.filter('currency', function (value) {
    //return "R$ " + formatDecimal(value, 2);
    return "R$ " + formatDecimal(value);
});




function formatDecimal(str, c) {

    var parts = (str || '').toString().trim().match(/^([+-])?([\d\.]+)(,\d+)?$/);

    if (parts == null) return null;

    var c = c === undefined || c === null ? 2 : c;
    var signal = parts[1] || '+';
    var integer = parts[2].replace(/\./g, '').replace(/^0+/, '') || "0";
    var decimal = ((parts[3] || '0').replace(',', '') + '0000000000').substr(0, c);

    var j = (j = integer.length) > 3 ? j % 3 : 0
    var fi = (j ? integer.substr(0, j) + '.' : '') +
        integer.substr(j).replace(/(\d{3})(?=\d)/g, '$1.');

    return (signal == '-' ? '-' : '') +
        fi +
        (c > 0 ? ',' : '') +
        decimal;
}