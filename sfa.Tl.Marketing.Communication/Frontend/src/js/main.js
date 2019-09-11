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

                    const searchedProviders = [];

                    for (let i = 0; i < providersData.Providers.length; i++) {
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
                                    <p class="text-center tl-uppercase"><a class="tl-link tl-link--modal" href="#">See courses available at this site</a></p>
                                 </div>
                                <div class="tl-modal">
                                    <div class="tl-modal--content">
                                        <p>Test</p>                                    
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

$(".tl-link--modal").click(function () {
    console.log("test");
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