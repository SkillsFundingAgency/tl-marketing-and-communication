$(".tl-nav--hamburger").click(function () {
    event.stopPropagation();
    if ($("#tl-nav").hasClass("active")) {
        $("#tl-nav").removeClass("active");
        $("body").removeClass("navopen");
        $("#tl-nav--hamburger").attr("aria-expanded", "false");
    }
    else {
        $("#tl-nav").addClass("active");
        $("#tl-nav--hamburger").attr("aria-expanded", "true");
        $("body").addClass("navopen");

        $("#tl-nav")[0].addEventListener('keydown', processKeyboardEvents);
        const elementsThatAreFocusable = $("#tl-nav a[href]");

        firstTabStop = elementsThatAreFocusable[0];
        lastTabStop = elementsThatAreFocusable[elementsThatAreFocusable.length - 1];

        firstTabStop.focus();
    }
});


var firstTabStop;
var lastTabStop;

$(document).on('click', function () {
    var target = event.target
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
        $(".tl-validation--message").text("You must enter a postcode");
        $(".tl-search--form").addClass("tl-validation--error");
        return false;
    } else {
        $(".tl-search--form").removeClass("tl-validation--error");
    }

    return true;
});

var maps = (function () {
    function initMap() {

        const shouldSearch = $("#ShouldSearch").val();
        if (shouldSearch === "True") {
            $("#tl-results-summary").addClass("tl-none");
        }

        $.getJSON("/js/providers.json", function (providersData) {

            const defaultResultCount = 5;

            $("#tl-next").addClass("tl-none");

            var geocoder = new google.maps.Geocoder();

            var dropdown = $("#tl-qualifications");
            dropdown.append($("<option></option>").attr("value", 0).text("All T Level courses"));

            $.each(providersData.qualifications,
                function (key, entry) {
                    dropdown.append($("<option></option>").attr("value", key).text(entry));
                    if ($("#Qualification").val().toUpperCase() === entry.toUpperCase()) {
                        dropdown.val(key);
                    }
                });

            $("#tl-find-button").click(function () {
                $("#MaxResultCount").val(defaultResultCount);
                return search(false);
            });

            $("#tl-next").click(function () {
                const currentResultCount = parseInt($("#MaxResultCount").val());
                $("#MaxResultCount").val(currentResultCount + defaultResultCount);
                return search(false);
            });

            if (shouldSearch === "True") {
                return search(true);
            }

            function search(goToSearchResults) {
                event.preventDefault();

                const postcode = $("#Postcode").val().replace(/\s/g, "");

                $("#tl-next").addClass("tl-none");

                if (postcode === "") {
                    showPostcodeError("You must enter a postcode");
                }
                else {
                    $.ajax({
                        url: 'https://postcodes.io/postcodes/' + postcode,
                        contentType: "application/json",
                        success: function (postcodesResult) {
                            const formattedPostcode = postcodesResult.result.postcode;
                            $("#Postcode").val(formattedPostcode);
                            searchForPostcode(formattedPostcode, goToSearchResults);
                        },
                        timeout: 5000,
                        error: function (jqXhr, error) {
                            if (jqXhr.status === 404) {
                                $.ajax({
                                    url: 'https://postcodes.io/terminated_postcodes/' + postcode,
                                    contentType: "application/json",
                                    success: function(postcodesResult) {
                                        const formattedPostcode = postcodesResult.result.postcode;
                                        $("#Postcode").val(formattedPostcode);
                                        searchForPostcode(formattedPostcode, goToSearchResults);
                                    },
                                    timeout: 5000,
                                    error: function() {
                                        showPostcodeError("You must enter a real postcode");
                                    }
                                });
                            } else {
                                console.log("Other Error reading postcode: " + jqXhr.status + " - " + error);
                                showPostcodeError("You must enter a real postcode");
                            }
                        }
                    });
                }

                return false;
            }

            function searchForPostcode(postcode, goToSearchResults)
            {
                $(".tl-search--form").removeClass("tl-validation--error");

                geocodeAddress(geocoder, postcode);

                if (goToSearchResults) {
                    var searchResultsAnchor = $("#tl-search");
                    $("html, body").animate({ scrollTop: searchResultsAnchor.offset().top }, "slow");
                }

                const searchResultLastPosition = parseInt($("#MaxResultCount").val());
                if (searchResultLastPosition > defaultResultCount)
                    $("#SearchResultLastPosition").val(searchResultLastPosition - defaultResultCount);
            }

            function geocodeAddress(geocoder, searchedPostcode) {
                if (searchedPostcode === "")
                    return;

                geocoder.geocode({
                    'address': searchedPostcode,
                    componentRestrictions: { country: 'GB' }
                }, function (results, status) {
                    if (status === "OK") {

                        const selectedQualification = parseInt($("#tl-qualifications").children("option:selected").val());
                        const searchedProvidersLocations = [];

                        for (let i = 0; i < providersData.providers.length; i++) {
                            for (let j = 0; j < providersData.providers[i].locations.length; j++) {

                                if (selectedQualification !== 0 &&
                                    !providersData.providers[i].locations[j].qualification2020.includes(
                                        selectedQualification) &&
                                    !providersData.providers[i].locations[j].qualification2021.includes(
                                        selectedQualification)) {
                                    continue;
                                }

                                providersData.providers[i].locations[j].distanceInMiles = getDistanceInMiles(
                                    providersData.providers[i].locations[j],
                                    results[0].geometry.location);

                                providersData.providers[i].locations[j].providerName = providersData.providers[i].name;

                                searchedProvidersLocations.push(providersData.providers[i].locations[j]);
                            }
                        }

                        searchedProvidersLocations.sort(compare);

                        showSearchResults(searchedProvidersLocations, providersData.qualifications);
                    } else {
                        showNoSearchResults();
                    }
                });
            }

            function compare(a, b) {
                if (parseInt(a.distanceInMiles) > parseInt(b.distanceInMiles)) return 1;
                if (parseInt(b.distanceInMiles) > parseInt(a.distanceInMiles)) return -1;

                return 0;
            }

            const entityMap = {
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

            function getDistanceInMiles(providerLocation, postcodeLocation) {

                const providerPosition = new google.maps.LatLng(providerLocation.latitude,
                    providerLocation.longitude);

                const postcodePosition = new google.maps.LatLng(postcodeLocation.lat(),
                    postcodeLocation.lng());

                const distanceInMetres = google.maps.geometry.spherical.computeDistanceBetween(postcodePosition, providerPosition);
                const distanceInMiles = distanceInMetres / 1609.344;

                return distanceInMiles.toFixed();
            }

            function sortQualifications(qualifications, qualificationIds) {
                const qualificationsResults = [];
                for (let j = 0; j < qualificationIds.length; j++) {
                    qualificationsResults.push(qualifications[qualificationIds[j]]);
                }

                qualificationsResults.sort();
                return qualificationsResults;
            }

            function showPostcodeError(message) {
                $(".tl-validation--message").text(message);
                $(".tl-search--form").addClass("tl-validation--error");
                $("#tl-search-results").empty();
                $("#tl-results-summary").removeClass("tl-none");
                $("#tl-next").addClass("tl-none");
            }

            function showNoSearchResults() {
                $("#tl-search-results").empty();
                $("#tl-next").addClass("tl-none");
                $("#tl-results").removeClass("tl-none");
                $("#tl-results-summary").removeClass("tl-none");
            }

            function showSearchResults(searchedProviderLocations, qualifications) {
                var searchResults = "";
                let maxResultCount = $("#MaxResultCount").val();

                if (searchedProviderLocations.length <= maxResultCount) {
                    maxResultCount = searchedProviderLocations.length;
                }

                for (let i = 0; i < maxResultCount; i++) {

                    const sortedQualificationsResults2020 =
                        sortQualifications(qualifications, searchedProviderLocations[i].qualification2020);
                    let qualificationsResults2020 = "";
                    for (let j = 0; j < sortedQualificationsResults2020.length; j++) {
                        qualificationsResults2020 += "<li>" + sortedQualificationsResults2020[j] + "</li>";
                    }

                    const sortedQualificationsResults2021 =
                        sortQualifications(qualifications, searchedProviderLocations[i].qualification2021);
                    let qualificationsResults2021 = "";
                    for (let j = 0; j < sortedQualificationsResults2021.length; j++) {
                        qualificationsResults2021 += "<li>" + sortedQualificationsResults2021[j] + "</li>";
                    }

                    let venueName = searchedProviderLocations[i].name;
                    searchResults += "<div class='tl-results--block'>";
                    if (venueName !== "") {
                        searchResults += "<h4>" + venueName + "</h4> \
                                          <p>Part of " + searchedProviderLocations[i].providerName + "<br />";
                    } else {
                        searchResults += "<h4>" + searchedProviderLocations[i].providerName + "</h4> \
                                         <p>";
                    }

                    searchResults += searchedProviderLocations[i].town + " | " + searchedProviderLocations[i].postcode + "</p> \
                                          <span class='tl-results--block--distance'>" + searchedProviderLocations[i].distanceInMiles + " miles</span> \
                                          <hr class='tl-line-lightgrey--small'>";
                    if (qualificationsResults2020 !== "")
                        searchResults += "<h5><strong>From September 2020 onwards:</strong></h5> \
                                          <ul> \
                                            " + qualificationsResults2020 + " \
                                          </ul>";
                    if (qualificationsResults2021 !== "")
                        searchResults += "<h5><strong>From September 2021 onwards:</strong></h5> \
                                          <ul> \
                                            " + qualificationsResults2021 + " \
                                          </ul>";
                    searchResults += "<a href='" + searchedProviderLocations[i].website + "' class='tl-link-black--orange tl-results--block--link' aria-label='Visit " + escapeHtml(venueName !== "" ? venueName : searchedProviderLocations[i].providerName) + "&#8217;s website'>Visit their website</a> \
                                 </div>";
                }

                $("#tl-results-summary").addClass("tl-none");
                $("#tl-search-results").empty();
                $("#tl-search-results").append(searchResults);
                $("#tl-results").removeClass("tl-none");

                if (searchedProviderLocations.length <= maxResultCount) {
                    $("#tl-next").addClass("tl-none");
                } else {
                    $("#tl-next").removeClass("tl-none");
                }

                $("#tl-search-results div:eq(" + $("#SearchResultLastPosition").val() + ") a").focus();
            }
        });
    }

    return {
        initMap: initMap
    };

})();


