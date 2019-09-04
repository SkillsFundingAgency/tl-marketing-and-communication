$(".tl-nav--hamburger").click(function () {
    event.stopPropagation();
    if ($("#tl-nav").hasClass("active")) {
        $("#tl-nav").removeClass("active");
        $("body").removeClass("navopen");

    }
    else {
        $("#tl-nav").addClass("active");
        $("body").addClass("navopen");

    }
});