var useremail = $("#useremail");
var userpwd = $("#userpwd");
var regpwds = $("#regpwd");
var Checkbox = $("#Checkbox1");
var regemail = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
var regpwd = /^[0-9a-zA-Z_]{6,18}$/;
$("#submit").click(function () {
    if (useremail.val() == "" || useremail.val() == null) {
        alert("请输入电子邮箱！");
        return false;
    } else if (!useremail.val().match(regemail)) {
        alert("您输入的电子邮箱格式不正确！正确格式：useremali@ziye.ren");
        return false;
    } else if (userpwd.val() == "" || userpwd.val() == null) {
        alert("请输入登录密码！");
        return false;
    } else if (!userpwd.val().match(regpwd)) {
        alert("您输入的登录密码格式不对！密码必须是6-18位之间的数字、字母和下划线");
    } else if (userpwd.val() != regpwds.val()) {
        alert("您输入的密码不一致！");
        return false;
    } else if (Checkbox1.checked == false) {
        alert("请您阅读并同意子夜人《注册条款》！");
        return false;
    }
});
