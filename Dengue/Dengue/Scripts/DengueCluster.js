
$(document).ready(function () {

    $("#searchInput").keyup(function () {

        if ($("#searchInput").val().length != 0) {
            $.ajax({
                type: "POST",
                url: "DengueClusters/ResultPartial",
                data: { "search": $("#searchInput").val() },
                success: successCallback,
                error: errorCallback
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: "DengueClusters/ResultPartial",
                success: successCallback,
                error: errorCallback
            });
        }

    });

    function successCallback(data, status) {
        //alert("success retrieve data");
        //alert(data);
        console.log("success retrieve data");
        $('#ResultPartialContainer').html(data);
        //$('#ResultPartialContainer').load('DengueClusters/ResultPartial');
    }

    function errorCallback(data, status) {
        //alert("error retrieve data");
        console.log("error retrieve data");
    }
});




