var sys_admin_name = $("#sys_admin_name");
var sys_admin_pwd = $("#sys_admin_pwd");
var sys_admin_pwdreg = $("#sys_admin_pwdreg");
var role = $("#role");
var submit = $("#submit");
var sys_admin_role = $("#sys_admin_role");
var flg = false;

$(function () {
    if (role.find("option:selected").text() != "请选择") {
        var ro = role.find("option:selected").val();
        sys_admin_role.val(ro);
        var flg = true;
    }
})
sys_admin_name.blur(function () {
    if (sys_admin_name.val() == null || sys_admin_name.val() == "") {
        alert("请输入用户名称");
        flg = false;
    } else {
        flg = true;
    }
});
sys_admin_pwd.blur(function () {
    if (sys_admin_pwd.val() == null || sys_admin_pwd.val() == "") {
        alert("请输入登录密码");
        flg = false;
    } else {
        flg = true;
    }
});
sys_admin_pwdreg.blur(function () {
    if (sys_admin_pwdreg.val() == null || sys_admin_pwdreg.val() == "") {
        alert("请输入确认密码");
        flg = false;
    } else {
        if (sys_admin_pwdreg.val() != sys_admin_pwd.val()) {
            alert("密码不一致");
            flg = false;
        } else {
            flg = true;
        }
    }
});
role.change(function () {
    if (role.find("option:selected").text() == "请选择") {
        alert("请选择角色！");
        flg = false;
    } else {
        var ro = role.find("option:selected").val();
        sys_admin_role.val(ro);
        flg = true;
    }
});

submit.click(function () {
    if (flg == true) {
        return true;
    } else {
        return false;
    }
});