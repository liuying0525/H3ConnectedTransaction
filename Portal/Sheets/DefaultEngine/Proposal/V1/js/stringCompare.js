function startCompare(a, b) {
    if (!a || !b) {
        return [(a || '').split(''), (b || '').split('')];
    }

    var result = highlight(a, b);
    return result;
}

function distinct(arr) {
    var obj = {};
    var result = [];
    for (i = 0; i < arr.length; i++) {
        if (!obj[arr[i]]) { //如果能查找到，证明数组元素重复了
            obj[arr[i]] = 1;
            result.push(arr[i]);
        }
    }
    return result;
};

function highlight() {
    var params = Array.prototype.slice.call(arguments);
    var result = params.map(function (e) {
        e = e.toUpperCase();
        e = e.replace(
            /[\ |\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\-|\_|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\"|\'|\,|\<|\.|\>|\/|\?]/g,
            "");
        return e.split("");
    });
    var maxLen = eval(" Math.max(" + result.map(function (e) {
        return e.length
    }).join(",") + ")");
    result.forEach(function (e) {
        if (e.length < maxLen) {
            e.length = maxLen;
        };
    });
    var index = [];
    for (var i = 0; i < result[0].length; i++) {
        if (result[0][i] === result[1][i]) {
            continue;
        } else {
            index.push(i);
        }
    };

    index.forEach(function (e) {
        result[0][e] = '<span style="color: red;">' + (result[0][e] ? result[0][e] : '') + '</span>';
        result[1][e] = '<span style="color: red;">' + (result[1][e] ? result[1][e] : '') + '</span>';
    });
    return result
}