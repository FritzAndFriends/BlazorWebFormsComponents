$(document).ready(function () {
    $("body").addClass("active");
    $("ul a").css("color", "#25262d");
    $("#students").css("background-color", "#025d66");
    $("#students").css("color", "#e6e7ef");


    $("li").hover(function () {
        $("ul").css("box-shadow", "11px 10px 50px 0px rgba(127,177,184,1)");
        $("ul").css("transition", "1s");
        $("ul a").css("color", "#e0f7f9");
        $("ul a").css("transition", "1s");
        $("ul a").css("background", "#181928");
        $("#students").css("background-color", "#e0f7f9");
        $("#students").css("color", "#181928");

    }, function () {
        $("ul").css("box-shadow", "none");
        $("ul").css("transition", "1s");
        $("ul a").css("color", "#25262d");
        $("ul a").css("transition", "1s");
        $("ul a").css("background", "#d8e9ff");
        $("#students").css("background-color", "#025d66");
        $("#students").css("color", "#e6e7ef");

    })

    $("btnClear").click(function ()
    { $("txtFirstName") = "asd" });

})