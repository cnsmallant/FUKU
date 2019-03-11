var sys_admin_name = $("#sys_admin_name");
var role = $("#role");
var submit = $("#submit");
var sys_admin_role = $("#sys_admin_role");
var flg = false;

$(function () {

    if ((sys_admin_name.val() != null || sys_admin_name.val() != "")) {
        var ro = role.find("option:selected").val();
        sys_admin_role.val(ro);
        var flg = true;
    }
    if (role.find("option:selected").text() != "请选择") {
        var ro = role.find("option:selected").val();
        sys_admin_role.val(ro);
        var flg = true;
    }
   
});
sys_admin_name.blur(function () {
    if (sys_admin_name.val() == null || sys_admin_name.val() == "") {
        alert("请输入用户名称");
        flg = false;
    } else {
        flg = true;
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