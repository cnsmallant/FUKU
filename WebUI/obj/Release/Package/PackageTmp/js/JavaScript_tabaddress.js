var start = $("#start");
var closeA = $("#closeA");
var div_con = $("#div_con");
var div_con_ul = $("#div_con_ul");
var kdval = $("#kdval");
var res = $("#res");
var gwc = $("#gwc");
var str = "";
$(function () {
    div_con.hide();

});
start.click(function () {
    onprovince();
    div_con.fadeIn();
});
closeA.click(function () {
    div_con.fadeOut();
});
res.click(function () {
    onprovince();
});
function onprovince() {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Goods/GoodsProvince?id=0",
        success: function (data) {

            var ulhtml = "";
            $.each(data, function (k, v) {
                ulhtml += "<li><a href='javascript:;' name='" + v.com_area_id + "'onclick='oncity(this)' title='" + v.com_area_name + "'>" + v.com_area_name + "</a></li>";
            });
            $("#div_con_ul").html(ulhtml);
        }
    });
}
function oncity(a) {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Goods/GoodsProvince?id=" + a.name,
        success: function (data) {
            var ulhtml = "";
            $.each(data, function (k, v) {
                ulhtml += "<li><a href='javascript:;' name='" + v.com_area_id + "'onclick='oncounty(this)' title='" + v.com_area_name + "'>" + v.com_area_name + "</a></li>";
            });

            $("#div_con_ul").html(ulhtml);

        }
    });
}
function oncounty(a) {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Goods/GoodsProvince?id=" + a.name,
        success: function (data) {
            $("#div_con_ul").html("");
            var ulhtml = "";
            $.each(data, function (k, v) {
                ulhtml += "<li><a href='javascript:;' name='" + v.com_area_id + "'onclick='onval(this)' title='" + v.com_area_name + "'>" + v.com_area_name + "</a></li>";
            });
            $("#div_con_ul").html(ulhtml);
            str += a.title;
        }
    });
}
function onval(a) {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Goods/GoodsVal?id=" + a.name,
        success: function (data) {
            str += a.title;
            start.val(str);
            kdval.text(data.pro_shipment_price);
            str = "";
        }
    });
    div_con.fadeOut();
}


//NUM
var num = $("#num");
var adds = $("#add");
var less = $("#less");
var pro_skuitem_stock = $("#pro_skuitem_stock");
var i = 1;
adds.click(function () {
    i++;
    if (i >= pro_skuitem_stock.val()) {
        i = pro_skuitem_stock.val()
    }
    num.val(i);
});
less.click(function () {
    i--;
    if (i <= 0) {
        i = 1;
    }
    num.val(i);
});
var pro_skuitem_price = $("#pro_skuitem_price");
var pro_sku_id = $("#pro_sku_id");
var pro_skuitem_mprice = $("#pro_skuitem_mprice");
gwc.click(function () {
    if (start.val() == null || start.val() == "") {
        alert("请选择终点城市！");

    } else {
        if (kdval.text() != "暂无配送信息") {
            $.ajax({
                url: "/Goods/GoodsCartCreate",
                type: 'POST',
                data: JSON.stringify(
                    {
                        pro_skuitem_id: pro_sku_id.val(),
                        shop_car_num: num.val(),
                        shop_car_price: pro_skuitem_price.val(),
                    }),
                dataType: "JSON",
                success: function (data) {
                    if (data == "OK") {
                        window.location.href = "/Goods/GoodsCart";
                    }
                }
            });
        } else {
            alert("此地区本站暂不支持配送,请您静候！");
            return false;
        }
    }
});