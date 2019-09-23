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

$(document).on('click', function () {
    var target = event.target;
    var parent = target.parentElement;
    var modalcontent = $(".tl-modal--content")

    if ($(target).is(".tl-link--modal")) {
        event.preventDefault();
        $(target).next(".tl-modal").addClass('active');
        $("body").addClass('modal-open');
        event.stopImmediatePropagation()
    }
    else if ($("body").hasClass("modal-open") && !$(target).is(modalcontent) && !modalcontent.has(target).length > 0 || $(target).is(".tl-modal--close")) {
        event.preventDefault();
        $('.tl-modal').removeClass('active');
        $("body").removeClass('modal-open');
    }

});

var maps = (function () {
    function initMap() {
        $.getJSON("/js/providers.json", function (providersData) {

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

            const shouldSearch = document.getElementById("ShouldSearch").value;
            if (shouldSearch === "True") {
                return search();
            }

            $("#tl-find-button").click(function () {
                return search();
            });

            $("#tl-next").click(function () {
                const currentResultCount = parseInt(document.getElementById("MaxResultCount").value);
                document.getElementById("MaxResultCount").value = currentResultCount + 5;
                return search();
            });

            function search() {
                event.preventDefault();
                const postcode = document.getElementById("Postcode").value;
                const postcodeRegex = /([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})/;
                const postcodeResult = postcodeRegex.test(postcode);

                if (postcode == "") {
                    $(".tl-validation--message").text("You must enter a postcode");
                    $(".tl-search--form").addClass("tl-validation--error");
                    $("#tl-search-results").empty();
                }
                else if (postcodeResult == true) {
                    $(".tl-search--form").removeClass("tl-validation--error");
                    geocodeAddress(geocoder, map);
                }
                else {
                    $(".tl-validation--message").text("You must enter a real postcode");
                    $(".tl-search--form").addClass("tl-validation--error");
                    $("#tl-search-results").empty();
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
                                providersData.providers[i].locations[j].website = providersData.providers[i].website;

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
            }

            function showSearchResults(searchedProviderLocations, qualifications) {
                var searchResults = "";
                const maxResultCount = document.getElementById("MaxResultCount").value;

                for (let i = 0; i < maxResultCount; i++) {
                    let qualificationsResults = "";
                    for (let j = 0; j < searchedProviderLocations[i].qualification2020.length; j++) {
                        qualificationsResults += "<li>" + qualifications[searchedProviderLocations[i].qualification2020[j]] + "</li>";
                    }

                    searchResults += "<div class='tl-results-box'> \
                                    <h3><span class='tl-results-box--distance'>" + searchedProviderLocations[i].distanceInMiles + " miles </span>" + searchedProviderLocations[i].name + "</h3> \
                                    <p>" + searchedProviderLocations[i].name + ", " + searchedProviderLocations[i].fullAddress + "</p> \
                                        <a class='text-center tl-uppercase tl-link tl-link--modal' href='#'>See courses available at this site</a> \
                                        <div class='tl-modal'> \
                                            <div class='tl-modal--content'> \
                                                <a href='#closemodal' class='tl-modal--close'>&times;</a> \
                                                <h2>" + searchedProviderLocations[i].name + "</h2> \
                                                <p>" + searchedProviderLocations[i].fullAddress + "</p> \
                                                <p><strong>Courses starting September 2020</strong></p> \
                                                <ul class='tl-list'> \
                                                " + qualificationsResults + " \
                                                </ul> \
                                                <a href='" + searchedProviderLocations[i].website + "' class='tl-button tl-button--orange'>Go to provider website</a> \
                                            </div> \
                                        </div> \
                                 </div> \
                                 <br/>";
                }

                $("#tl-search-results").empty();
                $("#tl-search-results").append(searchResults);
            }
        });
    }

    return {
        initMap: initMap
    };

})();