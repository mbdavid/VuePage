Vue.component('input-date', {
    template: '<span><input type="text" v-model="text" @change="update()" :placeholder="placeholder" /> [local={{local}}|invalid={{invalid}}]</span>',
    props: ["value", "placeholder"],
    data: function () {
        return {
            text: this.parse(this.value),
            local: this.value
        }
    },
    watch: {
        text: function(v) {
            this.local = this.format(v)
            this.$emit('input', this.local);
        }
    },
    methods: {
        update: function () {
            if (this.invalid) this.text = "";
            this.$emit('change', this.local);
        },
        // yyyy-MM-dd => dd/MM/yyyy
        parse: function (v) {
            var arr = /(\d{4})-(\d{2})-(\d{2})/.exec(v || "");

            return arr ?
                (arr[3] + '/' + arr[2] + '/' + arr[1]) :
                "";
        },
        // dd/MM/yyyy => yyyy-MM-dd
        format: function (v) {
            var arr = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(v);

            return arr ?
                (arr[3] + '-' + arr[2] + '-' + arr[1]) :
                null;
        }
    },
    computed: {
        invalid: function () {
            return this.text.length > 0 && this.local == null;
        }
    }

})

Vue.filter('date', function (value, input) {
    var arr = /(\d{4})-(\d{2})-(\d{2})/.exec(value || "");
    return arr ?
        (arr[3] + '/' + arr[2] + '/' + arr[1]) :
        "";
})


Vue.filter('uppercase', function (value, input) {
    return value.toUpperCase();
})