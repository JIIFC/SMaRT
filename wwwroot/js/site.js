function setChicletColor(selectObj, chicletId) {
    $('#' + chicletId).css("background-color", $(selectObj).children().filter(':selected').css("background-color"));
}

function selectedOverallStatusFunction() {
    var colorId = document.getElementById('selectedOverallStatus').value;
    if (colorId != 0) {
        $.ajax({
            type: 'GET',
            url: '/DataCards/GetPetsOverallColourById/' + colorId + '/',
            success: function (response) {
                if (response == "") {
                    response = "White";
                }
                document.getElementById('selectedOverallStatus').style.color = response;
            },
            error: function (data) {
                console.log("GetPetsOverallColourById failed");
            }
        });
    }
}