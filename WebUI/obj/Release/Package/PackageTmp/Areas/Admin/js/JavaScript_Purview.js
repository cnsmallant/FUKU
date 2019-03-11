var submit = $("#submit");
var purviewname = $("#purviewname");
var purviewpage = $("#purviewpage");
var flg = false;
purviewname.blur(function () {

    if (purviewname.val() == null || purviewname.val() == "") {
        alert("请输入权限名称！");
        flg = false;
    } else {
        flg = true;
    }
});
purviewpage.blur(function () {
    if (purviewpage.val() == null || purviewpage.val() == "") {
        alert("请输入页面名称！");
        flg = false;
    } else {
        flg = true;
    }
});
submit.click(function () {
    if (flg == true) {
        return true;
    } else if (flg == false) {
        alert("请添加信息！");
        return false;
    }
});