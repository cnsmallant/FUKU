//浮动导航
function float_nav(dom) {
    var right_nav = $(dom);
    var nav_height = right_nav.height();
    function right_nav_position(bool) {
        var window_height = $(window).height();
        var nav_top = (window_height - nav_height) / 2;
        if (bool) {
            right_nav.stop(true, false).animate({ top: nav_top + $(window).scrollTop() }, 0);
        } else {
            right_nav.stop(true, false).animate({ top: nav_top }, 0);
        }
        right_nav.show();
    }

    //if (!+'\v1' && !window.XMLHttpRequest) {
    //    $(window).bind('scroll resize', function () {
    //        if ($(window).scrollTop() > 0) {
    //            right_nav_position(true);
    //        } else {
    //            right_nav.hide();
    //        }
    //    })
    //} else {
    //$(window).bind('scroll resize', function () {
    //    if ($(window).scrollTop() > 0) {
    //        right_nav_position();
    //    } else {
    //        right_nav.hide();
    //    }
    //})
    //}
}
float_nav('#fixed_box');