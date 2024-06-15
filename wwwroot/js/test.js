var Test = function () {
    var init = function () {
        alert("init");
    }

    var doSomething = function (prop) {
        alert(prop);
    }

    return {
        init: init,
        doSomething: doSomething
    };
}();

//$(function () {
//    Test.init
//})