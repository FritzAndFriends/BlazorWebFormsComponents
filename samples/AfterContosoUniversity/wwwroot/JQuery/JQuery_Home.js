$(document).ready(function () {
    $("body").addClass("active");
    $("ul a").css("color", "#25262d");
    $("#home").css("background-color", "#025d66");
    $("#home").css("color", "#e6e7ef");
    $("#welcomeHeader h1").css("color", "#e3e6ea");
    $("#welcomeHeader h1").css("font-size", "80px");
    $("#welcomeHeader h1").css("transition", "8s");  
    $("#welcomeHeader h1 #com").css("color", "#e3e6ea");
    $("#welcomeHeader h1 #com").css("transition", "8s");
   

    $(window).resize(function () { $("#welcomeHeader h1").css("transition", "0s") });
    

    $("li").hover(function () {
        $("ul").css("box-shadow", "11px 10px 50px 0px rgba(127,177,184,1)");
        $("ul").css("transition", "1s");
        $("ul a").css("color", "#e0f7f9");
        $("ul a").css("transition", "1s");
        $("ul a").css("background", "#181928");
        $("#home").css("background-color", "#e0f7f9");
        $("#home").css("color", "#181928");
       

    }, function () {
        $("ul").css("box-shadow", "none");
        $("ul").css("transition", "1s");
        $("ul a").css("color", "#25262d");
        $("ul a").css("transition", "1s");       
        $("ul a").css("background", "#d8e9ff");
        $("#home").css("background-color", "#025d66");
        $("#home").css("color", "#e6e7ef");
        $("#welcomeHeader h1").css("background-color", "#131414");
        $("#welcomeHeader h1").css("transition", "0.5s")
       
    })

})