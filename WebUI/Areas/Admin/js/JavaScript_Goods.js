var pro_class_name = $("#pro_class_name");
var pro_class_page = $("#pro_class_page");
var pro_class_order = $("#pro_class_order");
var submit = $("#submit");
var regEN = /^.[A-Za-z]+$/;
var regNum = /^[0-9]*$/;
var flg = false;
pro_class_name.blur(function () {
    if (pro_class_name.val() == null || pro_class_name.val() == "") {
        alert("请输入类别名称");
        flg = false;
    } else {
        flg = true;
    }
});
pro_class_page.blur(function () {
    if (pro_class_page.val() == null || pro_class_page.val() == "") {
        alert("请输入页面名称");
        flg = false;
    } else {
        if (!pro_class_page.val().match(regEN)) {
            alert("必须是英文字符");
            flg = false;
        } else {
            $.ajax({
                url: "/Admin/Goods/RegPage?pagename=" + pro_class_page.val(),
                type: 'GET',
                contentType: 'application/json',
                dataType: "JSON",
                success: function (data) {
                    if (data == "NO") {
                        alert("此页面名称已经存在");
                        flg = false;
                    } else if (data == "OK") {
                        flg = true;
                    }
                }
            });
        }
    }
});
pro_class_order.blur(function () {
    if (pro_class_order.val() == null || pro_class_order.val() == "") {
        alert("请输入排序序号");
        flg = false;
    } else {
        if (!pro_class_order.val().match(regNum)) {
            alert("必须为数字");
            flg = false;
        } else {
            flg = true;
        }
    }
});


submit.click(function () {
    if (flg == true) {
        return true;
    } else {
        return false;
    }
});