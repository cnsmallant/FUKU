var uname = $("#uname");
var un = $("#un");
var upwd = $("#upwd");
var up = $("#up");
var submit = $("#submit");
var flg = false;
uname.blur(function () {
    if (uname.val() == null || uname.val() == "") {
        un.text("请输入登录名称！");
        flg = false;
    } else {
        un.text
        flg = true;
    }
});
upwd.blur(function () {
    if (upwd.val() == null || upwd.val() == "") {
        up.text("请输入登录密码！");
        flg = false;
    } else {
        up.text("");
        flg = true;
    }
});
submit.click(function () {

    if (flg == true) {
        $.ajax({
            url: "/Admin/Login/UserLogin",
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(
                {
                    uname: uname.val(),
                    upwd: upwd.val()
                }),
            dataType: "JSON",
            success: function (data) {
                if (data == "NO") {
                    alert("登录失败");
                    return;
                } else if (data == "OK") {
                    alert(data);
                    window.location.href = "/Admin/Home/";
                }
            },
            error: function(err) {
                //alert("data error");
                alert(err.reponseText);}
    });
} else {
        alert("请输入登录信息！");
}
});