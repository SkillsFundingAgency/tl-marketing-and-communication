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

var searchedProviders = [];

function initMap() {
    $.getJSON("/js/providers.json", function (providersData) {

        var dropdown = $("#tl-qualifications");
        dropdown.append($('<option></option>').attr('value', 0).text("All 2020 courses"));

        $.each(providersData.Qualifications,
            function (key, entry) {
                dropdown.append($('<option></option>').attr('value', key).text(entry));
            });

        map = new google.maps.Map(document.getElementById('map'),
            {
                center: { lat: 52.4862, lng: 1.8904 },
                zoom: 6
            });

        for (let i = 0; i < providersData.Providers.length; i++) {
            const providerPosition = {
                lat: providersData.Providers[i].location.latitude,
                lng: providersData.Providers[i].location.longitude
            };

            const marker = new google.maps.Marker({
                position: providerPosition,
                map: map,
                title: providersData.Providers[i].name
            });
        }

        var geocoder = new google.maps.Geocoder();

        document.getElementById('tl-find').addEventListener('click', function () {
            geocodeAddress(geocoder, map);
        });

        function geocodeAddress(geocoder, resultsMap) {
            const searchedPostcode = document.getElementById('Postcode').value;
            if (searchedPostcode === '')
                return;

            geocoder.geocode({ 'address': searchedPostcode }, function (results, status) {
                if (status === 'OK') {
                    resultsMap.setCenter(results[0].geometry.location, 1);
                    resultsMap.setZoom(10);

                    var selectedQualification = parseInt($("#tl-qualifications").children("option:selected").val());

                    const searchedProviders = [];

                    for (let i = 0; i < providersData.Providers.length; i++) {
                        if (selectedQualification !== 0) {
                            if (!providersData.Providers[i].location.qualification2020.includes(selectedQualification)) {
                                continue;
                            }
                        }

                        const providerPosition = new google.maps.LatLng(providersData.Providers[i].location.latitude,
                            providersData.Providers[i].location.longitude);

                        const postcodePosition = new google.maps.LatLng(results[0].geometry.location.lat(),
                            results[0].geometry.location.lng());

                        const distancedInMetres = google.maps.geometry.spherical.computeDistanceBetween(postcodePosition, providerPosition);
                        const distanceInMiles = distancedInMetres / 1609.344;

                        providersData.Providers[i].distanceInMiles = distanceInMiles.toFixed();
                        searchedProviders.push(providersData.Providers[i]);
                    }

                    searchedProviders.sort((a, b) => a.distanceInMiles - b.distanceInMiles);

                    showSearchResults(searchedProviders);
                } else {
                    showNoSearchResults();
                }
            });
        }

        function showNoSearchResults() {
            var searchResults = `<div class="tl-results-box">
                                        <h3><span class="tl-results-box--distance">0 results found</span></h3>
                                    </div>`;

            $("#tl-search-results").empty();
            $(`#tl-search-results`).append(searchResults);
        }

        function showSearchResults(searchedProviders) {
            var searchResults = "";
            for (let i = 0; i < searchedProviders.length; i++) {
                searchResults += `<div class="tl-results-box">
                                    <h3><span class="tl-results-box--distance">${searchedProviders[i].distanceInMiles} miles </span>${searchedProviders[i].name}</h3>
                                    <p>${searchedProviders[i].name}, ${searchedProviders[i].location.fullAddress}</p>
                                        <a class="text-center tl-uppercase tl-link tl-link--modal" href="#">See courses available at this site</a>
                                        <div class="tl-modal">
                                            <div class="tl-modal--content">
                                                <p>Test</p>                                    
                                            </div>
                                        </div>
                                 </div>

                                 <br/>`;
            }

            $("#tl-search-results").empty();
            $(`#tl-search-results`).append(searchResults);
        }
    });
}

$(".tl-modal--close").click(function () {
    event.preventDefault();
    $(this).closest('.tl-modal').removeClass('active');
    $("body").removeClass('modal-open');

});

$(".tl-modal--content").click(function (e) {
    e.stopPropagation();
});

$(document).on('click', function (e) {
    if ($(e.target).is(".tl-link--modal")) {
        event.preventDefault();
        $(e.target).next(".tl-modal").addClass('active');
        $("body").addClass('modal-open');
        event.stopImmediatePropagation()
    }
    else {
        if ($("body").hasClass("modal-open")) {
            e.preventDefault();
            $('.tl-modal').removeClass('active');
            $("body").removeClass('modal-open');
        }
    }
});
