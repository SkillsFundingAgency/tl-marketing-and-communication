/* Analytics cookies */
$(document).ready(function () {

    if (GOVUK.cookie('CovidBanner') === 'hidden') {
        $('#tl-covid-message-dismiss').text("Show");
    }

    else {
        $('#tl-covid--message').removeClass("tl-covid--banner--hidden");
        $('#tl-covid-message-dismiss').text("Hide");

    }

    $('#tl-covid-message-dismiss').click(function () {
        if ($('#tl-covid--message').hasClass('tl-covid--banner--hidden')) {
            GOVUK.cookie('CovidBanner', 'visible', { days: 365 });
            $('#tl-covid--message').removeClass("tl-covid--banner--hidden");
            $(this).text("Hide");

        }

        else {
            GOVUK.cookie('CovidBanner', 'hidden', { days: 365 });
            $('#tl-covid--message').addClass("tl-covid--banner--hidden");
            $(this).text("Show");
        }
    });

    $('#tl-covid-message-dismiss').keypress(function (e) {
        var key = e.which;
        if (key == 13) {
            $(this).click();
            return false;
        }
    }); 
});





