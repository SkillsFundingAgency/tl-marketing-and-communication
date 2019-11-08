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

        const elementsThatAreFocusable = $('.tl-modal.active a[href], area[href], select:not([disabled]), textarea:not([disabled]), button:not([disabled]), [tabindex="0"]');

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

var maps = (function () {
    function initMap() {
        $.getJSON("/js/providers.json", function (providersData) {

            const defaultResultCount = 5;
            $("#tl-next").hide();

            var dropdown = $("#tl-qualifications");
            dropdown.append($("<option></option>").attr("value", 0).text("All 2020 courses"));

            $.each(providersData.qualifications,
                function (key, entry) {
                    dropdown.append($("<option></option>").attr("value", key).text(entry));
                    if ($("#Qualification").val() === entry) {
                        dropdown.val(key);
                    }
                });

            var map = new google.maps.Map(document.getElementById("map"),
                {
                    center: { lat: 52.4774169, lng: -1.9336707 },
                    zoom: 6
                });

            const infoWindow = new google.maps.InfoWindow();

            for (let i = 0; i < providersData.providers.length; i++) {
                for (let j = 0; j < providersData.providers[i].locations.length; j++) {
                    const marker = new google.maps.Marker({
                        position: {
                            lat: providersData.providers[i].locations[j].latitude,
                            lng: providersData.providers[i].locations[j].longitude
                        },
                        map: map,
                        title: providersData.providers[i].name
                    });

                    const infoWindowContent = "<h1>" + providersData.providers[i].name + "</h1>" +
                        "<p><b>" + providersData.providers[i].locations[j].fullAddress + "</b></p>";

                    attachInfoWindow(marker, map, infoWindow, infoWindowContent);
                }
            }

            function attachInfoWindow(marker, map, infoWindow, infoWindowContent) {
                google.maps.event.addListener(marker,
                    "click", function () {
                        infoWindow.setContent(infoWindowContent);
                        infoWindow.open(map, marker);
                    });
            }

            var geocoder = new google.maps.Geocoder();

            const shouldSearch = $("#ShouldSearch").val();
            if (shouldSearch === "True") {
                $("#tl-next").click(function () {
                    const currentResultCount = parseInt($("#MaxResultCount").val());
                    $("#MaxResultCount").val(currentResultCount + defaultResultCount);
                    return search(false);
                });
                return search(true);
            }

            $("#tl-find-button").click(function () {
                $("#MaxResultCount").val(defaultResultCount);
                return search(false);
            });

            $("#tl-next").click(function () {
                const currentResultCount = parseInt($("#MaxResultCount").val());
                $("#MaxResultCount").val(currentResultCount + defaultResultCount);
                return search(false);
            });

            function search(goToSearchResults) {
                event.preventDefault();
                const postcode = document.getElementById("Postcode").value;
                const postcodeRegex = /([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})/;
                const postcodeResult = postcodeRegex.test(postcode);

                $("#tl-next").show();

                if (postcode == "") {
                    $(".tl-validation--message").text("You must enter a postcode");
                    $(".tl-search--form").addClass("tl-validation--error");
                    $("#tl-search-results").empty();
                    $("#tl-next").hide();
                }
                else if (postcodeResult == true) {
                    $(".tl-search--form").removeClass("tl-validation--error");
                    geocodeAddress(geocoder, map);

                    if (goToSearchResults) {
                        var searchResultsAnchor = $("#tl-search");
                        $("html, body").animate({ scrollTop: searchResultsAnchor.offset().top }, "slow");
                    }

                    const searchResultLastPosition = parseInt($("#MaxResultCount").val());
                    if (searchResultLastPosition > defaultResultCount)
                        $("#SearchResultLastPosition").val(searchResultLastPosition - defaultResultCount);
                }
                else {
                    $(".tl-validation--message").text("You must enter a real postcode");
                    $(".tl-search--form").addClass("tl-validation--error");
                    $("#tl-search-results").empty();
                    $("#tl-next").hide();
                }
                return false;
            }

            function geocodeAddress(geocoder, resultsMap) {
                const searchedPostcode = document.getElementById("Postcode").value;
                if (searchedPostcode === "")
                    return;

                geocoder.geocode({ 'address': searchedPostcode }, function (results, status) {
                    if (status === "OK") {
                        resultsMap.setCenter(results[0].geometry.location, 1);
                        resultsMap.setZoom(10);

                        const selectedQualification = parseInt($("#tl-qualifications").children("option:selected").val());

                        const searchedProvidersLocations = [];

                        for (let i = 0; i < providersData.providers.length; i++) {
                            for (let j = 0; j < providersData.providers[i].locations.length; j++) {

                                if (selectedQualification !== 0 &&
                                    !providersData.providers[i].locations[j].qualification2020.includes(
                                        selectedQualification)) {
                                    continue;
                                }

                                providersData.providers[i].locations[j].distanceInMiles = getDistanceInMiles(
                                    providersData.providers[i].locations[j],
                                    results[0].geometry.location);


                                providersData.providers[i].locations[j].name = providersData.providers[i].name;
                                providersData.providers[i].locations[j].website = providersData.providers[i].locations[j].website;

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

            function getDistanceInMiles(providerLocation, postcodeLocation) {

                const providerPosition = new google.maps.LatLng(providerLocation.latitude,
                    providerLocation.longitude);

                const postcodePosition = new google.maps.LatLng(postcodeLocation.lat(),
                    postcodeLocation.lng());

                const distanceInMetres = google.maps.geometry.spherical.computeDistanceBetween(postcodePosition, providerPosition);
                const distanceInMiles = distanceInMetres / 1609.344;

                return distanceInMiles.toFixed();
            }

            function showNoSearchResults() {
                const searchResults = "<div class='tl-results-box'> \
                                        <h3><span class='tl-results-box--distance'>0 results found</span></h3> \
                                    </div>";

                $("#tl-search-results").empty();
                $("#tl-search-results").append(searchResults);

                $("#tl-next").hide();
            }

            function showSearchResults(searchedProviderLocations, qualifications) {
                var searchResults = "";
                let maxResultCount = $("#MaxResultCount").val();

                if (searchedProviderLocations.length <= maxResultCount) {
                    maxResultCount = searchedProviderLocations.length;
                    $("#tl-next").hide();
                }

                for (let i = 0; i < maxResultCount; i++) {
                    let qualificationsResults = "";
                    for (let j = 0; j < searchedProviderLocations[i].qualification2020.length; j++) {
                        qualificationsResults += "<li>" + qualifications[searchedProviderLocations[i].qualification2020[j]] + "</li>";
                    }

                    searchResults += "<div class='tl-results-box'> \
                                    <h3><span class='tl-results-box--distance'>" + searchedProviderLocations[i].distanceInMiles + " miles </span>" + searchedProviderLocations[i].name + "</h3> \
                                    <p>" + searchedProviderLocations[i].fullAddress + "</p> \
                                                <p><strong>Courses starting September 2020</strong></p> \
                                                <ul class='tl-list tl-list-small'> \
                                                " + qualificationsResults + " \
                                                </ul> \
                                                <a href='" + searchedProviderLocations[i].website + "' class='tl-link tl-link--external tl-find--providersite'>Go to provider website</a> \
                                 </div> \
                                 <br/>";
                }

                $("#tl-search-results").empty();
                $("#tl-search-results").append(searchResults);

                $("#tl-search-results div:eq(" + $("#SearchResultLastPosition").val() + ") a").focus();
            }
        });
    }

    return {
        initMap: initMap
    };

})();