$("#DummyForceElement_ElementName").change(function () {
    calculateOverallStatus();
    let elementName = $("#DummyForceElement_ElementName").val();
    if (elementName != "" && elementName != null) {
        removeInputValidation("#DummyForceElement_ElementName");
    }
});
$("#DummyForceElement_ElementId").change(function () {
    calculateOverallStatus;
    let elementId = $("#DummyForceElement_ElementId").val();
    if (elementId != "" && elementId != null) {
        removeInputValidation("#DummyForceElement_ElementId");
    }
});

$("#PersonnelStatusId").change(function () {
    calculateOverallStatus();
    getStatus("PersonnelStatusId", "PersStatusDisplay");
});
$("#EquipmentStatusId").change(function () {
    calculateOverallStatus();
    getStatus("EquipmentStatusId", "EquipStatusDisplay");
});
$("#TrainingStatusId").change(function () {
    calculateOverallStatus();
    getStatus("TrainingStatusId", "TrainStatusDisplay");
});
$("#SustainmentStatusId").change(function () {
    calculateOverallStatus();
    getStatus("SustainmentStatusId", "SustStatusDisplay");
});

$(document).ready(function () {
    getStatus("PersonnelStatusId", "PersStatusDisplay");
    getStatus("EquipmentStatusId", "EquipStatusDisplay");
    getStatus("TrainingStatusId", "TrainStatusDisplay");
    getStatus("SustainmentStatusId", "SustStatusDisplay");
    calculateOverallStatus();
});

function submit(formId) {
    let elementName = $("#DummyForceElement_ElementName").val();
    let elementId = $("#DummyForceElement_ElementId").val();

    if (elementName != "" && elementName != null && elementId != "" && elementId != null) {
        document.getElementById(formId).submit();
    }

    if (elementName == "" || elementName == null) {
        setInputValidation("#DummyForceElement_ElementName");
    }
    if (elementId == "" || elementId == null) {
        setInputValidation("#DummyForceElement_ElementId");
    }
}

function getStatus(field, display) {
    $.ajax({
        type: 'GET',
        url: '/DataCards/GetPetsOverallColourById/' + document.getElementById(field).value + '/',
        success: function (response) {
            if (response == "") {
                response = "White";
            }
            document.getElementById(display).style.backgroundColor = response;
        }
    });
}

function calculateOverallStatus() {
    var formData = new FormData();

    formData.append("PersonnelStatus", $("#PersonnelStatusId").val());
    formData.append("EquipmentStatus", $("#EquipmentStatusId").val());
    formData.append("TrainingStatus", $("#TrainingStatusId").val());
    formData.append("SustainmentStatus", $("#SustainmentStatusId").val());

    $.ajax({
        type: 'POST',
        url: '/DataCards/CalculateOverallStatus/',
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (response) {
            var petsStatus = JSON.parse(response.responseMessage);
            var srStatusDisplay = document.getElementById("SrStatusDisplay");
            var srStatusId = document.getElementById("SrStatusId");

            srStatusId.setAttribute('Value', petsStatus.srStatusID);
            srStatusDisplay.style.backgroundColor = petsStatus.srStatusColour;
        },
        error: function (data) {
            alert("Broken");
        }
    });
}