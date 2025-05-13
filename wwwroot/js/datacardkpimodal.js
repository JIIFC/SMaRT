function showModal(dataCardId, type) {
    var options = { keyboard: true };
    $.ajax({
        type: "POST",
        url: '/DataCards/DatacardKpiModal',
        data: {
            datacardId: dataCardId,
            type: type
        },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            $('#kpiModalContent').html(data);
            $('#kpiModal').modal(options);
            $('#kpiModal').modal('show');
        }
    });
}
function Save(modelId, type) {
    let OverallStatusBelowId = $('#OverallStatusBelowId').val();
    let OverallStatusAboveId = $('#OverallStatusAboveId').val();
    let AlertAnyChanges;
    let AlertOnSubmit;
    let AlertWhenIncomplete;
    let invalidFields = false;

    if (type == "Readiness") {
        if ($('#belowSwitch').is(':checked') && OverallStatusBelowId == "") {
            $('#OverallStatusBelowId').addClass("input-validation-error");
            invalidFields = true;
        }
        if ($('#aboveSwitch').is(':checked') && OverallStatusAboveId == "") {
            $('#OverallStatusAboveId').addClass("input-validation-error");
            invalidFields = true;
        }
        // Gets value from hidden input
        AlertAnyChanges = $('#AlertAnyChanges').val().toLowerCase();
        AlertOnSubmit = $('#AlertOnSubmit').val().toLowerCase();
        AlertWhenIncomplete = $('#AlertWhenIncomplete').val().toLowerCase();
    } else if (type == "Change") {
        // Gets values from toggles
        AlertAnyChanges = $('#AlertAnyChanges').is(':checked') + "";
        AlertOnSubmit = $('#AlertOnSubmit').is(':checked') + "";
        AlertWhenIncomplete = $('#AlertWhenIncomplete').is(':checked') + "";
    }

    if (invalidFields) {
        return;
    }

    if (OverallStatusBelowId == "" && OverallStatusAboveId == "" && AlertAnyChanges == "false" && AlertOnSubmit == "false" && AlertWhenIncomplete == "false") {
        if (modelId != 0) {
            // Delete
            $.ajax({
                type: 'POST',
                url: "/DataCards/DeleteDatacardKpi",
                data: {
                    datacardKpiId: modelId
                },
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                }
            });
        }
    } else {
        if (modelId == 0) {
            // Create
            $.ajax({
                type: 'POST',
                url: "/DataCards/CreateDatacardKpi",
                data: {
                    UserId: $('#UserId').val(),
                    DatacardId: $('#DatacardId').val(),
                    OverallStatusBelowId: OverallStatusBelowId,
                    OverallStatusAboveId: OverallStatusAboveId,
                    AlertAnyChanges: AlertAnyChanges,
                    AlertOnSubmit: AlertOnSubmit,
                    AlertWhenIncomplete: AlertWhenIncomplete
                },
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                }
            });
        } else {
            // Edit
            $.ajax({
                type: 'POST',
                url: "/DataCards/EditDatacardKpi",
                data: {
                    DatacardKpiId: modelId,
                    OverallStatusBelowId: OverallStatusBelowId,
                    OverallStatusAboveId: OverallStatusAboveId,
                    AlertAnyChanges: AlertAnyChanges,
                    AlertOnSubmit: AlertOnSubmit,
                    AlertWhenIncomplete: AlertWhenIncomplete
                },
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                }
            });
        }
    }
    $('#kpiModal').modal('hide');
}

function selectedOverallStatusFunction(elementId) {
    var colorId = document.getElementById(elementId).value;
    if (colorId != "") {
        $('#' + elementId).removeClass("input-validation-error");
    }
    if (colorId != 0) {
        $.ajax({
            type: 'GET',
            url: '/DataCards/GetPetsOverallColourById/' + colorId + '/',
            success: function (response) {
                if (response == "") {
                    response = "White";
                }
                document.getElementById(elementId).style.color = response;
            },
            error: function (data) {
                console.log("GetPetsOverallColourById failed");
            }
        });
    }
}