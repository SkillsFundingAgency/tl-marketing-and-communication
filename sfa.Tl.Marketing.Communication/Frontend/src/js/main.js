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




$(".tl-link--modal").click(function () {
    event.preventDefault();
    $(this).next('.tl-modal').addClass('active');
});

$(".tl-modal--close").click(function () {
    event.preventDefault();
    $(this).closest('.tl-modal').removeClass('active');
});
