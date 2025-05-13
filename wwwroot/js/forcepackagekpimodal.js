function showModal(forcePackageId) {
    var options = { keyboard: true };
    $.ajax({
        type: "POST",
        url: '/ForcePackage/ForcePackageKpiModal',
        data: {
            forcePackageId: forcePackageId
        },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            $('#kpiModalContent').html(data);
            $('#kpiModal').modal(options);
            $('#kpiModal').modal('show');
        }
    });
}