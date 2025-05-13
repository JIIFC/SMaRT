function setInputValidation(field) {
    let fieldValue = $(field).val();
    if ($.trim(fieldValue) == "" || fieldValue == null) {
        $(field).addClass("input-validation-error");
    } else {
        $(field).removeClass("input-validation-error");
    }
}

function removeInputValidation(field) {
    $(field).removeClass("input-validation-error");
}