$(function () {
    Vue.validator('number', function (val) {
        return /^[+]?[0-9]+$/.test(val);
    });

    Vue.validator('space', function (val) {
        return /^\S*$/.test(val);
    });
    Vue.validator('chineseforbid', function (val) {
        return /[\u4e00-\u9fa5]/.test(val);
    });
})