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
    if (key === 13) {
        $(this).click();
        return false;
    }
}); 


// Subject accordions //
$(".tl-subjects--accordion--button").click(function () {
    $(this).parent().toggleClass('tl-subjects--accordion--expanded');
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
    if ($("#tl-search-results div").length) {
        const searchResultSize = $("#SelectedItemIndex").val();
        $(document).scrollTop($("#tl-search-results div").eq(searchResultSize).offset().top);
        $("#tl-search-results div:eq(" + searchResultSize + ") a").focus();
    }
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

$("#tl-find-button").click(function () {

    const postcode = $("#Postcode").val().trim();
      
    if (postcode === "") {
        event.stopPropagation();
        showPostcodeError("You must enter a postcode or town");
        return false;
    } else {
        $(".tl-search--form").removeClass("tl-validation--error");
    }

    return true;
});

function showPostcodeError(message) {
    $(".tl-validation--message").text(message);
    $(".tl-search--form").addClass("tl-validation--error");
    $("#tl-search-results").empty();
    $("#tl-results-summary").removeClass("tl-none");
    $("#tl-results-summary").empty();
    $("#tl-results-summary").append("<h3>0 results</h3><p> Enter a postcode to search for schools and colleges doing T Levels.</p>");
    $("#tl-next").addClass("tl-none");
}

// AUTOCOMPLETE
const $keywordsInput = $('#Postcode');
var $defaultValue = $('#Postcode').data('default-value');

if ($keywordsInput.length > 0) {
    $keywordsInput.wrap('<div id="autocomplete-container" class="tl-autocomplete-wrap"></div>');
    const container = document.querySelector('#autocomplete-container');
    $(container).empty();

    function getSuggestions(query, populateResults) {
        if (/\d/.test(query)) {
            //ignoring potential postcodes
            return;
        }
        var results = [];
        $.ajax({
            url: "/api/locations",
            type: "get",
            dataType: 'json',
            data: { searchTerm: query }
        }).done(function (data) {
            results = data.map(function (r) {
                return getLocationDisplayName(r);
            });
            populateResults(results);
        });
    }

    function getLocationDisplayName(item) {
        if (item.county) return item.name + ', ' + item.county;
        else if (item.la) return item.name + ', ' + item.la;
        return item.name;
    }

    function onConfirm() {        
    }

    accessibleAutocomplete({
        element: container,
        id: 'Postcode',
        name: 'Postcode',
        displayMenu: 'overlay',
        showNoOptionsFound: false,
        minLength: 3,
        source: getSuggestions,
        placeholder: "Enter postcode or town",
        onConfirm: onConfirm,
        defaultValue: $defaultValue,
        confirmOnBlur: false,
        autoselect: true
    });
}
