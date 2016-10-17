$(document).ready(function () {
    var map;

    var bhLayer = [];
    bhLayer[0] = new google.maps.KmlLayer({
        url: 'https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/9f05bb14-a9df-4596-b8a2-cdf821b788c6/download'
    });
    bhLayer[1] = new google.maps.KmlLayer({
        url: 'https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/ed953998-4ee2-4a2a-bb18-3e61fc014afd/download'
    });
    bhLayer[2] = new google.maps.KmlLayer({
        url: 'https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/3c03a902-6d95-47e6-9dc1-5378ac8425c5/download'
    });
    bhLayer[3] = new google.maps.KmlLayer({
        url: 'https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/f24472c9-6703-4863-b58a-114722e418c3/download'
    });
    bhLayer[4] = new google.maps.KmlLayer({
        url: 'https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/7a7881b4-ea5e-46b1-aa4e-a25b8a0cefda/download'
    });

    var dcLayer = new google.maps.KmlLayer({
        url: 'https://data.gov.sg/dataset/e7536645-6126-4358-b959-a02b22c6c473/resource/c1d04c0e-3926-40bc-8e97-2dfbb1c51c3a/download/DENGUECLUSTER.kml'
    });

    function CenterControl(controlDiv, map) {

        // Set CSS for the control border.
        var controlUI = document.createElement('div');
        controlUI.style.backgroundColor = '#fff';
        controlUI.style.border = '2px solid #fff';
        controlUI.style.borderRadius = '3px';
        controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
        controlUI.style.cursor = 'pointer';
        controlUI.style.marginBottom = '22px';
        controlUI.style.textAlign = 'center';
        controlUI.title = 'Click to toggle between Dengue Cluster and Breeding Habitat';
        controlDiv.appendChild(controlUI);

        // Set CSS for the control interior.
        var controlText = document.createElement('div');
        controlText.style.color = 'rgb(25,25,25)';
        controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
        controlText.style.fontSize = '16px';
        controlText.style.lineHeight = '38px';
        controlText.style.paddingLeft = '5px';
        controlText.style.paddingRight = '5px';
        controlText.innerHTML = 'View Breeding Habitat';
        controlUI.appendChild(controlText);

        // Setup the click event listeners: simply set the map to Chicago.
        controlUI.addEventListener('click', function () {
            if (controlText.innerHTML == 'View Breeding Habitat') {
                controlText.innerHTML = 'View Dengue Cluster';

                dcLayer.setMap(null);
                for (var i = 0; i < bhLayer.length ; i++) {
                    bhLayer[i].setMap(map);
                }
            }
            else {
                controlText.innerHTML = 'View Breeding Habitat';

                for (var i = 0; i < bhLayer.length ; i++) {
                    bhLayer[i].setMap(null);
                }
                dcLayer.setMap(map);
            }
        });

    }

    function initialize() {
        var mapProp = {
            center: new google.maps.LatLng(1.3000, 103.8000),
            zoom: 10,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

        var centerControlDiv = document.createElement('div');
        var centerControl = new CenterControl(centerControlDiv, map);

        centerControlDiv.index = 1;
        map.controls[google.maps.ControlPosition.TOP_CENTER].push(centerControlDiv);


        dcLayer.setMap(map);
    }

    google.maps.event.addDomListener(window, 'load', initialize);

});