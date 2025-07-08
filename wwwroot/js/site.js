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


// Scroll to top button functionality
let scrollToTopButton = document.getElementById("scrollToTopButton");
window.onscroll = function () {
    scrollFunction();
}

// Show the button when the user scrolls down 20px from the top of the document
function scrollFunction() {
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20)
    {
        scrollToTopButton.style.display = "block";
    }
    else {
        scrollToTopButton.style.display = "none";
    }
}

// Add click event listener to the scroll to top button
scrollToTopButton.addEventListener("click", scrollToTop);

// Function to scroll to the top of the page
function scrollToTop() {
    $('html, body').animate({ scrollTop: 0 }, 'fast');
}