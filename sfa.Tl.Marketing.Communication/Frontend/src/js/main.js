$(".tl-nav--hamburger").click(function () {
    event.stopPropagation();
    if ($("#tl-nav").hasClass("active")) {
        $("#tl-nav").removeClass("active");
        $("body").removeClass("navopen");
        $("#tl-nav--hamburger").attr("aria-expanded", "false");
        $("#tl-nav--hamburger span").text("Open main navigation");

    }
    else {
        $("#tl-nav").addClass("active");
        $("#tl-nav--hamburger").attr("aria-expanded", "true");
        $("#tl-nav--hamburger span").text("Close Main navigation");
        $("body").addClass("navopen");

        $("#tl-nav")[0].addEventListener('keydown', processKeyboardEvents);
        const elementsThatAreFocusable = $("#tl-nav a");

        firstTabStop = elementsThatAreFocusable[1];
        lastTabStop = elementsThatAreFocusable[elementsThatAreFocusable.length - 1];

        firstTabStop.focus();
    }
});

$('.tl-nav--hamburger').keypress(function (e) {
    var key = e.which;
    if (key == 13) 
    {
        $(this).click();
        return false;
    }
}); 

var firstTabStop;
var lastTabStop;

$(document).on('click', function () {
    var target = event.target;
    var modalContent = $(".tl-modal--content");

    if ($(target).is(".tl-link--modal")) {
        event.preventDefault();
        $(target).next(".tl-modal").addClass('active');
        $(target).next(".tl-modal")[0].addEventListener('keydown', processKeyboardEvents);
        $("body").addClass('modal-open');
        event.stopImmediatePropagation();

        const elementsThatAreFocusable = $('.tl-modal.active a[href], area[href], select:not([disabled]), textarea:not([disabled]), [tabindex="0"]');

        firstTabStop = elementsThatAreFocusable[0];
        lastTabStop = elementsThatAreFocusable[elementsThatAreFocusable.length - 1];

        firstTabStop.focus();
    }
    else if ($("body").hasClass("modal-open") && !$(target).is(modalContent) && !modalContent.has(target).length > 0 || $(target).is(".tl-modal--close")) {
        event.preventDefault();
        closeModal();
    }
});

$(function () {
    $("#tl-search-results div:eq(" + $("#SelectedItemIndex").val() + ") a").focus();
});

function closeModal() {
    $(".tl-modal").removeClass("active");
    $("body").removeClass("modal-open");
}

function processKeyboardEvents(e) {
    const keyTab = 9;
    const keyEscape = 27;

    if (e.keyCode === keyTab) {

        if (e.shiftKey) {
            if (document.activeElement === firstTabStop) {
                e.preventDefault();
                lastTabStop.focus();
            }
        } else {
            if (document.activeElement === lastTabStop) {
                e.preventDefault();
                firstTabStop.focus();
            }
        }
    }

    if (e.keyCode === keyEscape) {
        closeModal();
    }
}

var entityMap = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': '&quot;',
    "'": '&#39;',
    '’': '&#8217',
    "/": '&#x2F;'
};

function escapeHtml(string) {
    return String(string).replace(/[&<>"'\/]/g, function (s) {
        return entityMap[s];
    });
}

$("#tl-find-button").click(function () {

    clearSearchInfo();
    const postcode = $("#Postcode").val().trim();
      
    if (postcode === "") {
        event.stopPropagation();
        $(".tl-validation--message").text("You must enter a postcode");
        $(".tl-search--form").addClass("tl-validation--error");
        return false;
    } else {
        $(".tl-search--form").removeClass("tl-validation--error");
    }

    return true;
});

$("#tl-nav--bar-student--find").click(function () {
    clearSearchInfo();
});

function persistSearchInfo() {
    const qualification = $("#tl-qualifications").children("option:selected").text();
    const postcode = $("#Postcode").val();

    GOVUK.cookie('postcode', postcode, { days: 1 });
    GOVUK.cookie('qualification', qualification, { days: 1 });
}

function loadSearchInfo() {

    const shouldSearch = $("#ShouldSearch").val();

    if (shouldSearch === "False") {

        const postcodev = GOVUK.cookie('postcode');
        const qualificationv = GOVUK.cookie('qualification');
        if (postcodev) {
            $("#Postcode").val(postcodev);
            $("#Qualification").val(qualificationv);
            $("#ShouldSearch").val("True");
        }
    }
}

function clearSearchInfo() {
    GOVUK.cookie('postcode', null);
    GOVUK.cookie('qualification', null);
}

function removeSearchStringFromFindUrl() {
    var studentsFindUrl = window.location.origin + window.location.pathname;
    window.history.replaceState({}, "students find", studentsFindUrl);
}


