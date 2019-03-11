var chestr = $("#chestr");
var rolewname = $("#rolewname");
var submit = $("#submit");
var str = "";
submit.click(function () {

    $(":checkbox[name='PostedFruits.FruitIds']").each(function () {
        if ($(this).attr("checked")) {
            str += $(this).val() + ",";
        }
    });
    chestr.val(str);
    if (rolewname.val() == null || rolewname.val() == "") {

        alert("请输入角色名称！");
        return false;
    }
    else if (str == "") {
        alert("请选择权限名称！");
        return false;
    } else {
        return true;
    }
});