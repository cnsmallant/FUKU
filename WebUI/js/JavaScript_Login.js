$(function () {
    Geetest();
})


var submit = $("#login_u");
var contaction = $("#contaction");
var psw = $("#psw");
var geeteststr = "";
var captcha = $("#captcha");
var err = $("#err");
var and = $("#and");


submit.click(function () {
    var geetest_challenge = $(".geetest_challenge");
    var geetest_validate = $(".geetest_validate");
    var geetest_seccode = $(".geetest_challenge");
    $.ajax({
        url: "/Geetest/CheckGeeTestResult",
        type: 'POST',
        data: JSON.stringify(
            {
                geetest_challenge: geetest_challenge.val(),
                geetest_validate: geetest_validate.val(),
                geetest_seccode: geetest_seccode.val()
            }),
        dataType: "JSON",
        success: function (data) {
            geeteststr = data;
            if (contaction.val() == null || contaction.val() == "") {
                err.attr("class", "errshow");
                err.html("请输入登录账号！");
            }
            else if (psw.val() == null || psw.val() == "") {

                err.attr("class", "errshow");
                err.html("请输入登录密码！");

            } else if (geeteststr == 0) {
              
                err.attr("class", "errshow");
                err.html("请进行滑动验证！");
            } else {
                err.attr("class", "errhide");
                err.html("");
                $.ajax({
                    url: "/Login/UserLogin",
                    type: 'POST',
                    data: JSON.stringify(
                        {
                            user_basic_login: contaction.val(),
                            user_basic_pwd: psw.val(),
                        }),
                    dataType: "JSON",
                    success: function (data) {
                        if (data == "NO") {
                            alert("登录失败");
                            captcha.empty();
                            var handler = function (captchaObj) {
                                // 将验证码加到id为captcha的元素里
                                captchaObj.appendTo("#captcha");
                            };
                            $.ajax({
                                // 获取id，challenge，success（是否启用failback）
                                url: "/Geetest",
                                type: "get",
                                dataType: "json", // 使用jsonp格式
                                success: function (data) {
                                    // 使用initGeetest接口
                                    // 参数1：配置参数，与创建Geetest实例时接受的参数一致
                                    // 参数2：回调，回调的第一个参数验证码对象，之后可以使用它做appendTo之类的事件
                                    initGeetest({
                                        gt: data.gt,
                                        challenge: data.challenge,
                                        product: "float", // 产品形式
                                        offline: !data.success
                                    }, handler);
                                }
                            });
                        } else {
                            window.location.href = "/Personal/Index";
                        }
                    }
                });
            }
        }
    });
});


function Geetest() {
    var handler = function (captchaObj) {
        // 将验证码加到id为captcha的元素里
        captchaObj.appendTo("#captcha");
    };
    $.ajax({
        // 获取id，challenge，success（是否启用failback）
        url: "/Geetest",
        type: "get",
        dataType: "json", // 使用jsonp格式
        success: function (data) {
            // 使用initGeetest接口
            // 参数1：配置参数，与创建Geetest实例时接受的参数一致
            // 参数2：回调，回调的第一个参数验证码对象，之后可以使用它做appendTo之类的事件
            initGeetest({
                gt: data.gt,
                challenge: data.challenge,
                product: "float", // 产品形式
                offline: !data.success
            }, handler);
        }
    });
}
