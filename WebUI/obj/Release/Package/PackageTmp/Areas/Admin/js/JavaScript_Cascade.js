$(function () {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Admin/Goods/Cascade?parentid=0",
        success: function (data) {
            var Province = " <option value='value'>请选择</option>";
            $.each(data, function (k, v) {
                Province += "<option value='" + v.com_area_id + "'>" + v.com_area_name + "</option>";
            });
            $("#Province").html(Province);
        }
    });
});
    $("#Province").change(function () {
        var parentid = $("#Province").val();
        $.ajax({
            type: "GET",
            dataType: "JSON",
            url: "/Admin/Goods/Cascade?parentid=" + parentid,
            success: function (data) {
                var City = " <option value='value'>请选择</option>";
                var County = "<option value='value'>请选择</option>";
                $.each(data, function (k, v) {
                    City += "<option value='" + v.com_area_id + "'>" + v.com_area_name + "</option>";
                });
                $("#City").html(City);
                $("#County").html(County);
            }
        });
    })
    $("#City").change(function () {
        var parentid = $("#City").val();
        $("#County").html("");
        $.ajax({
            type: "GET",
            dataType: "JSON",
            url: "/Admin/Goods/Cascade?parentid=" + parentid,
            success: function (data) {
                var County = " <option value='value'>请选择</option>";
                $.each(data, function (k, v) {
                    County += "<option value='" + v.com_area_id + "'>" + v.com_area_name + "</option>";
                });
                $("#County").html(County);
            }
        });
    })

    var pro_shipment_province = $("#pro_shipment_province");
    var pro_shipment_city = $("#pro_shipment_city");
    var pro_shipment_county = $("#pro_shipment_county");

    $("#County").change(function () {
        pro_shipment_province.val($("#Province").find("option:selected").val());
        pro_shipment_city.val($("#City").find("option:selected").val());
        pro_shipment_county.val($("#County").find("option:selected").val());
    })
    var pro_express_id = $("#pro_express_id");

    var pro_shipment_price = $("#pro_shipment_price");
    var submit = $("#submit");
    var pro_express_name = $("#pro_express_name");
    submit.click(function () {
        if (pro_express_id.find("option:selected").text() == "请选择") {
            alert("请选择公司");
            return false;
        }
        var ex = pro_express_id.find("option:selected").val();
        pro_express_name.val(ex);
        if ($("#Province").find("option:selected").text() == "请选择" || $("#City").find("option:selected").text() == "请选择" || $("#County").find("option:selected").text() == "请选择") {
            alert("请选择终点城市");
            return false;
        } 
        if (pro_shipment_price.val() == null || pro_shipment_price.val() == "") {
            alert("请输入运费价格");
            return false;
        } 
    });

