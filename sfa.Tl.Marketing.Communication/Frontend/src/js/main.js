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






$(".tl-modal--close").click(function () {
    event.preventDefault();
    $(this).closest('.tl-modal').removeClass('active');
    $("body").removeClass('modal-open');

});


$(".tl-modal--content").click(function (e) {
    e.stopPropagation();
});

$(".tl-link--modal").click(function () {
    event.preventDefault();
    $(this).next('.tl-modal').addClass('active');
    $("body").addClass('modal-open');
    event.stopImmediatePropagation()
});


$(document).on('click', function () {
    if ($("body").hasClass("modal-open")) {
        event.preventDefault();
        $('.tl-modal').removeClass('active');
        $("body").removeClass('modal-open');
    }
});