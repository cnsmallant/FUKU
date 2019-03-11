var ary = location.href.split("&");
jQuery(".picScroll-left_itm").slide({
    titCell: ".hd_itm ul",
    mainCell: ".bd_itm ul",
    autoPage: true,
    effect: "left",
    autoPlay: false,
    scroll: 1,
    vis: 5,
    easing: "swing",
    delayTime: 500,
    pnLoop: true,
    trigger: onmouseover,
    mouseOverStop: true,
    prevCell: ".prev_itm",
    nextCell: ".next_itm"
});