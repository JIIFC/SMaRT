const preview = document.querySelector('.preview');

function addInputToJsonFileList(metls) {
    let metlJsonArray = [];
    const parentMetlList = document.getElementById("parentMetlList").value;

    if (parentMetlList != null && parentMetlList != "") {
        metlJsonArray = JSON.parse(document.getElementById("parentMetlList").value);
    }
    for (const metl of metls) {
        metlJsonArray.push({ "metlId": metl.metlId, "metlName": metl.name, "validFrom": metl.validFrom, "validTo": metl.validTo });
    }
    listMetls(fileJsonArray, false);
}

function addMetltoJsonFileList(metlId, metlName, validFrom, validTo) {
    if (metlId != null && metlId != "") {
        let metlJsonArray = [];
        const parentMetlList = document.getElementById("parentMetlList").value;
        if (parentMetlList != null && parentMetlList != "") {
            metlJsonArray = JSON.parse(document.getElementById("parentMetlList").value);
        }

        if (!checkIfMetlIsInList(metlId, metlJsonArray)) {
            metlJsonArray.push({ "metlId": metlId, "metlName": metlName, "validFrom": validFrom, "validTo": validTo });
        } else {
            removeMetlFromList(metlId);
            if (parentMetlList != null && parentMetlList != "") {
                metlJsonArray = JSON.parse(document.getElementById("parentMetlList").value);
            }
            metlJsonArray.push({ "metlId": metlId, "metlName": metlName, "validFrom": validFrom, "validTo": validTo });
        }
        listMetls(metlJsonArray, false);
    }
}

function checkIfMetlIsInList(Id, metlJsonArray) {
    let r = false;
    $.map(metlJsonArray, function (elem) {
        if (elem.metlId == Id) 
            r = true;
    });
    return r;
}

function listMetls(jsonFiles, readOnly) {
    // remove previous list
    while (preview.firstChild) {
        preview.removeChild(preview.firstChild);
    }
    if (jsonFiles.length === 0) {
        const para = document.createElement('p');
        para.textContent = 'No Metls';
        preview.appendChild(para);
    } else {

        const table = document.createElement('table');
        table.setAttribute("id", 'file-table');
        preview.appendChild(table);
        let thead = document.createElement('thead');
        let trHead = document.createElement('tr');
        let thMetlId = document.createElement('th');
        thMetlId.textContent = `MetlId`;
        let thMetlName = document.createElement('th');
        thMetlName.textContent = `Metl Name`;
        let thValidFrom = document.createElement('th');
        thValidFrom.textContent = `Valid From`;
        let thValidTo = document.createElement('th');
        thValidTo.textContent = `Valid To`;

        trHead.appendChild(thMetlId);
        trHead.appendChild(thMetlName);
        trHead.appendChild(thValidFrom);
        trHead.appendChild(thValidTo);


        if (!readOnly) {
            let thDelete = document.createElement('th');
            trHead.appendChild(thDelete);
        }

        thead.appendChild(trHead);
        table.appendChild(thead);

        let tbody = document.createElement('tbody');

        var i = 1;
        for (const metl of jsonFiles) {

            

            let trItem = document.createElement('tr');

            let tdNum = document.createElement('td');
            tdNum.textContent = metl.metlId;
            trItem.appendChild(tdNum);

            let tdMetlName = document.createElement('td');
            tdMetlName.textContent = `${metl.metlName}`;
            trItem.appendChild(tdMetlName);

            let tdValidFrom = document.createElement('td');
            tdValidFrom.textContent = `${metl.validFrom}`;
            trItem.appendChild(tdValidFrom);

            let tdValidTo = document.createElement('td');
            tdValidTo.textContent = `${metl.validTo}`;
            trItem.appendChild(tdValidTo);


            if (!readOnly) {
                let deleteInput = document.createElement('input');
                deleteInput.type = "image";
                deleteInput.src = "/images/x-circle.svg";
                deleteInput.setAttribute("onclick", 'removeMetlFromList("' + metl.metlId + '")');

                let tdDelete = document.createElement('td');
                tdDelete.appendChild(deleteInput);
                trItem.appendChild(tdDelete);
            }
            tbody.appendChild(trItem);
        }
        table.appendChild(tbody);
    }
    console.log(JSON.stringify(jsonFiles));
    console.log(jsonFiles);
    document.getElementById("parentMetlList").value = JSON.stringify(jsonFiles);
    document.getElementById("file-table").classList.add("table");
    document.getElementById("file-table").classList.add("table-striped");
    document.getElementById("file-table").classList.add("table-bordered");
}


function removeMetlFromList(metlIdToChange) {
    let metlList = JSON.parse(document.getElementById("parentMetlList").value);
    for (var i = 0; i < metlList.length; i++) {
        if (metlList[i].metlId == metlIdToChange) {
            metlList.splice(i, 1);
            break;
        }
    }
    listMetls(metlList, false);
}
