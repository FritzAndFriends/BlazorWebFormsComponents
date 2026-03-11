$(document).ready(function () {
    $("body").addClass("active");
    $("ul a").css("color", "#25262d");
    $("#about").css("background-color", "#025d66");
    $("#about").css("color", "#e6e7ef");

    /*$(".grid").css("opacity", "1");
    $(".grid").css("transition", "15s");*/

    $(".grid").hover(function () {
        $(".grid").css("transition", "5s");     
        $(".grid").css("opacity", "1");
        $("#header h1").css("transition", "1.5s");
        $("#header h1").css("opacity", "0.001");

    }, function () {
        $(".grid").css("transition", "2s");
        $(".grid").css("opacity", "0.15");
        $("#header h1").css("transition", "5s");
        $("#header h1").css("opacity", "1");
    })


    $("li").hover(function () {
        $("ul").css("box-shadow", "11px 10px 50px 0px rgba(127,177,184,1)");
        $("ul").css("transition", "1s");
        $("ul a").css("color", "#e0f7f9");
        $("ul a").css("transition", "1s");
        $("ul a").css("background", "#181928");
        $("#about").css("background-color", "#e0f7f9");
        $("#about").css("color", "#181928");

    }, function () {
        $("ul").css("box-shadow", "none");
        $("ul").css("transition", "1s");
        $("ul a").css("color", "#25262d");
        $("ul a").css("transition", "1s");
        $("ul a").css("background", "#d8e9ff");
        $("#about").css("background-color", "#025d66");
        $("#about").css("color", "#e6e7ef");

    })

})