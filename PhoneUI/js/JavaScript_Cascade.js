$(function () {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Area/Cascade?parentid=0",
        success: function (data) {
            var Province = " <option value='value'>省份</option>";
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
        url: "/Area/Cascade?parentid=" + parentid,
        success: function (data) {
            var City = " <option value='value'>城市</option>";
            var County = "<option value='value'>区县</option>";
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
        url: "/Area/Cascade?parentid=" + parentid,
        success: function (data) {
            var County = " <option value='value'>区县</option>";
            $.each(data, function (k, v) {
                County += "<option value='" + v.com_area_id + "'>" + v.com_area_name + "</option>";
            });
            $("#County").html(County);
        }
    });
})
