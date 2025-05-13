function getCheckboxesOfType(type) {
    const checkboxes = document.getElementsByClassName(type + "-checkbox");
    var selected = [];
    for (let i = 0; i < checkboxes.length; i++) {
        if (checkboxes.item(i).checked) {
            var id = checkboxes.item(i).id.substr(type.length);
            selected.push(id);
        }
    }
    return selected;
}

function getForcePackageElementIds(type) {
    const forcePackageElements = document.getElementById('packageForceElements').getElementsByTagName('li');
    var selected = [];
    for (let i = 0; i < forcePackageElements.length; i++) {
        var itemId = forcePackageElements[i].id;
        if (itemId.includes(type)) {
            selected.push(itemId.substr(type.length));
        }
    }
    return selected;
}

function setMissingValueErrors() {
    if ($('#ForcePackagePurpose').val() == "") {
        $('#ForcePackagePurposeValidation').css("display", "inline");
    } else {
        $('#ForcePackagePurposeValidation').css("display", "none");
    }
    if ($('#ForcePackageName').val() == "") {
        $('#ForcePackageNameValidation').css("display", "inline");
    } else {
        $('#ForcePackageNameValidation').css("display", "none");
    }
}