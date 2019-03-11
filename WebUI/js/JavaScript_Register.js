$(function () {
    Geetest();
});
var regpwd = /^[0-9a-zA-Z_]{6,18}$/;
var regphone = /^((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)$/;
var regemail = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;

var submit = $("#login_u");
var contaction = $("#contaction");//会员名
var psw = $("#psw");//密码
var rpsw = $("#rpsw");//确认密码
var eml = $("#eml");//电子邮箱
var tel = $("#tel");//手机号码
var regcode = $("#regcode");//验证码
var captcha = $("#captcha");
var err = $("#err");
var dater = $("#dater");//生日
var btn_regin = $("#btn_regin");//获取验证码按钮
var btn_regout = $("#btn_regout");//计时
var sendregcode = "";//发送的验证码
var sss = $("#sss");
var si = 60;
btn_regout.hide();

btn_regin.click(function () {
    if (tel.val() == null || tel.val() == "") {
        err.attr("class", "errshow");
        err.html("请输入手机号码！");
    } else if (!tel.val().match(regphone)) {
        err.attr("class", "errshow");
        err.html("手机号码格式不正确！");
    } else {
        settime(sss);
        $.ajax({
            url: "/SMS?phone=" + tel.val(),
            type: 'Get',
            dataType: "JSON",
            success: function (data) {
                sendregcode = data;
            }
        });
    }
});

//function getAuction() {
//    si--;
//    sss.text(si);
//    if (si == 0) {
//        btn_regin.show();
//        btn_regout.hide();
//        si == 60;
//    }
//}


function settime(val) {
    if (si == 0) {
        btn_regin.show();
        btn_regout.hide();
        val.text("提取验证码");
        si = 60;
    } else {
        btn_regin.hide();
        btn_regout.show();
        val.attr("disabled", "disabled");
        if (si < 10) {
            val.text("0" + si );
        } else {
            val.text(si);
        }
        si--;
        setTimeout(function () {
            settime(val)
        }, 1000)
    }
}
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
                err.html("请输入会员名称！");
            }
            else if (dater.val() == null || dater.val() == "") {
                err.attr("class", "errshow");
                err.html("请选择生日日期！");
            } else if (psw.val() == null || psw.val() == "") {
                err.attr("class", "errshow");
                err.html("请输入登录密码！");
            } else if (!psw.val().match(regpwd)) {
                err.attr("class", "errshow");
                err.html("密码格式不正确！");
            }
            else if (rpsw.val() == null || rpsw.val() == "") {
                err.attr("class", "errshow");
                err.html("请输入确认密码！");
            }
            else if (rpsw.val() != psw.val()) {
                err.attr("class", "errshow");
                err.html("密码不一致！");
            } else if (eml.val() == "" || eml.val() == null) {
                err.attr("class", "errshow");
                err.html("请输入电子邮箱！");
            } else if (!eml.val().match(regemail)) {
                err.attr("class", "errshow");
                err.html("电子邮箱格式不正确！");
            }
            else if (tel.val() == null || tel.val() == "") {
                err.attr("class", "errshow");
                err.html("请输入手机号码！");
            } else if (!tel.val().match(regphone)) {
                err.attr("class", "errshow");
                err.html("手机号码格式不正确！");
            }
            else if (regcode.val() == null || regcode.val() == "") {
                err.attr("class", "errshow");
                err.html("请输入手机验证码！");
            } else if (regcode.val() != sendregcode) {
                err.attr("class", "errshow");
                err.html("手机验证码错误！");
            }
            else if (geeteststr == 0) {
                err.attr("class", "errshow");
                err.html("请进行滑动验证！");
            } else {
                err.html("");
                err.attr("class", "errhide");
                $.ajax({
                    url: "/Register/RegisterCreate",
                    type: 'POST',
                    data: JSON.stringify(
                        {
                            user_basic_login: contaction.val(),
                            user_basic_pwd: psw.val(),
                            user_basic_tel: tel.val(),
                            user_detail_gender: $('input:radio:checked').val(),
                            user_detail_birthday: dater.val(),
                            user_basic_email:eml.val(),
                        }),
                    dataType: "JSON",
                    success: function (data) {
                        if (data == "UNO") {
                            err.attr("class", "errshow");
                            err.html("用户名已经存在，请重新填写！");
                        } else if (data == "PNO") {
                            err.attr("class", "errshow");
                            err.html("手机号已经存在，请重新填写！");
                        } else if (data == "NO") {
                            err.attr("class", "errshow");
                            err.html("注册失败！");
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
                            window.location.href = "/Register/RegisterSuccess";
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
