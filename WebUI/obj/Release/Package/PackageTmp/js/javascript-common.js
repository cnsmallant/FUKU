
//滚动banner图片
$(".carousel").carousel({
    interval: 2000
});

//function titleA(i) {
//    $("#" + i).popover({ html: true });
//}


//Up按钮
$(function () {
    //当滚动条的位置处于距顶部100像素以下时，跳转链接出现，否则消失
    $(function () {
        $(window).scroll(function () {
            if ($(window).scrollTop() > 0) {
                $("#totop").fadeIn(1500);
            }
            else {
                $("#totop").fadeOut(1500);
            }
        });

        //当点击跳转链接后，回到页面顶部位置

        $("#totop").click(function () {
            $('body,html').animate({ scrollTop: 0 }, 1000);
            return false;
        });
    });
});
//控制导航条
//$(window).scroll(function () {
//    // 当滚动到最底部以上100像素时， 加载新内容
//    if ($(this).scrollTop() == 0) {
//        //document.getElementById("nav-div").className = "navbar navbar-default navbar-static-top  visible-xs-block";
//        document.getElementById("pc-nav-div").className = "navbar navbar-default navbar-static-top";
//    } else {
//        //document.getElementById("nav-div").className = "navbar navbar-default navbar-fixed-top  visible-xs-block";
//        document.getElementById("pc-nav-div").className = "navbar navbar-default navbar-fixed-top";
//    }
//});

//更换body背景
$(function () {
    var myDate = new Date();
    var h = myDate.getHours();
    if (h >= 1 && h < 3) {
        document.body.style.backgroundImage = "url('/img/bgimg/01_03.jpg')";
    }
    if (h >= 3 && h < 5) {
        document.body.style.backgroundImage = "url('/img/bgimg/03_05.jpg')";
    }
    if (h >= 5 && h < 7) {
        document.body.style.backgroundImage = "url('/img/bgimg/05_07.jpg')";
    }
    if (h >= 7 && h < 9) {
        document.body.style.backgroundImage = "url('/img/bgimg/07_09.jpg')";
    }
    if (h >= 9 && h < 11) {
        document.body.style.backgroundImage = "url('/img/bgimg/09_11.jpg')";
    }
    if (h >= 11 && h < 13) {
        document.body.style.backgroundImage = "url('/img/bgimg/11_13.jpg')";
    }
    if (h >= 13 && h < 15) {
        document.body.style.backgroundImage = "url('/img/bgimg/13_15.jpg')";
    }
    if (h >= 15 && h < 17) {
        document.body.style.backgroundImage = "url('/img/bgimg/15_17.jpg')";
    }
    if (h >= 17 && h < 19) {
        document.body.style.backgroundImage = "url('/img/bgimg/17_19.jpg')";
    }
    if (h >= 19 && h < 21) {
        document.body.style.backgroundImage = "url('/img/bgimg/19_21.jpg')";
    }
    if (h >= 21 && h < 23) {
        document.body.style.backgroundImage = "url('/img/bgimg/21_23.jpg')";
    }
    if (h >= 23 || h < 1) {
        document.body.style.backgroundImage = "url('/img/bgimg/23_01.jpg')";
    }
});

