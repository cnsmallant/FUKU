var com_img_title = $("#com_img_title");
var com_img_alt = $("#com_img_alt");
var com_img_src = $("#com_img_src");
var submit = $("#submit");
var flg = false;
com_img_title.blur(function () {
    if (com_img_title.val() == null || com_img_title.val() == "") {
        alert("请输入图片title");
        flg = false;
    } else {
        flg = true;
    }
});
com_img_alt.blur(function () {
    if (com_img_alt.val() == null || com_img_alt.val() == "") {
        alert("请输入图片alt");
        flg = false;
    } else {
        flg = true;
    }
});



submit.click(function () {
    if (com_img_src.val() == null || com_img_src.val() == "") {
        alert("请选择图片");
        flg = false;
    } else {
        flg = true;
    }
    if (flg == false) {
        return false;
    } else {
        return true;
    }
});