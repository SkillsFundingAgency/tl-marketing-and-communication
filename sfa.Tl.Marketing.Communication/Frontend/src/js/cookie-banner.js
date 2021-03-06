(function () {
    "use strict";
    var root = this;
    if (typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

    /*
      Cookie methods
      ==============
  
      Usage:
  
        Setting a cookie:
        GOVUK.cookie('hobnob', 'tasty', { days: 30 });
  
        Reading a cookie:
        GOVUK.cookie('hobnob');
  
        Deleting a cookie:
        GOVUK.cookie('hobnob', null);
    */
    GOVUK.cookie = function (name, value, options) {
        if (typeof value !== 'undefined') {
            if (value === false || value === null) {
                return GOVUK.setCookie(name, '', { days: -1 });
            } else {
                return GOVUK.setCookie(name, value, options);
            }
        } else {
            return GOVUK.getCookie(name);
        }
    };
    GOVUK.setCookie = function (name, value, options) {
        if (typeof options === 'undefined' || options === null) {
            options = {};
        }
        var cookieString = name + "=" + value + "; path=/";
        if (options.days) {
            var date = new Date();
            date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
            cookieString = cookieString + "; expires=" + date.toGMTString();
        }
        if (document.location.protocol === 'https:') {
            cookieString = cookieString + "; Secure";
        }
        document.cookie = cookieString;
    };
    GOVUK.getCookie = function (name) {
        var nameEq = name + "=";
        var cookies = document.cookie.split(';');
        for (var i = 0, len = cookies.length; i < len; i++) {
            var cookie = cookies[i];
            while (cookie.charAt(0) === ' ') {
                cookie = cookie.substring(1, cookie.length);
            }
            if (cookie.indexOf(nameEq) === 0) {
                return decodeURIComponent(cookie.substring(nameEq.length));
            }
        }
        return null;
    };
}).call(this);
(function () {
    "use strict";
    var root = this;
    if (typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

    GOVUK.addCookieMessage = function () {
        var message = document.getElementById('global-cookie-message'),
            hasCookieMessage = (message && GOVUK.cookie('seen_cookie_message') === null);

        if (hasCookieMessage) {
            message.style.display = 'block';

            $('#global-cookie-message-dismiss').click(function (e) {
                GOVUK.cookie('AnalyticsConsent', 'true', { days: 365 });
                GOVUK.cookie('seen_cookie_message', 'yes', { days: 28 });
                message.style.display = 'none';
                e.preventDefault();
            });

            $('#global-cookie-message-moreinfo').click(function (e) {
                GOVUK.cookie('AnalyticsConsent', 'true', { days: 365 });
                GOVUK.cookie('seen_cookie_message', 'yes', { days: 28 });
                message.style.display = 'none';
            });

            $('#global-cookie-message-cookieslink').click(function (e) {
                GOVUK.cookie('AnalyticsConsent', 'true', { days: 365 });
                GOVUK.cookie('seen_cookie_message', 'yes', { days: 28 });
                message.style.display = 'none';
            });
        }
    };
}).call(this);
(function () {
    "use strict";

    // add cookie message
    if (window.GOVUK && GOVUK.addCookieMessage) {

        GOVUK.addCookieMessage();
        var firstTabStop;
        var lastTabStop;

        const elementsThatAreFocusable = $('.tl-modal a[href], area[href], select:not([disabled]), textarea:not([disabled]), button:not([disabled]), [tabindex="0"]');

        $(".tl-modal").on("keydown",
            function (e) {
                const keyTab = 9;

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
            });

        firstTabStop = elementsThatAreFocusable[0];
        lastTabStop = elementsThatAreFocusable[elementsThatAreFocusable.length - 1];

        firstTabStop.focus();
    }
}).call(this);

/* Analytics cookies */
$(document).ready(function () {

    if (GOVUK.cookie('AnalyticsConsent') === 'true') {
        $('#cbxAnalyticsConsent').prop('checked', true);
    }

    else {
        $('#cbxAnalyticsConsent').prop('checked', false);

    }

    $('#cbxAnalyticsConsent').change(function () {
        if (this.checked) {
            GOVUK.cookie('AnalyticsConsent', 'true', { days: 365 });
        }
        else {
            GOVUK.cookie('AnalyticsConsent', 'false', { days: 365 });
        }
    });

    $('#lblAnalyticsConsent').keypress(function (e) {
        var key = e.which;
        if (key == 13) {
            $(this).click();
            return false;
        }
    }); 
});





