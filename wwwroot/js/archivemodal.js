function showModal() {
    var options = { "backdrop": "static", keyboard: true };
    $.ajax({
        type: "GET",
        url: '/Admin/ArchiveModal',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#archiveModalContent').html(data);
            $('#archiveModal').modal(options);
            $('#archiveModal').modal('show');
        }
    });
}

function modalHidden(formName) {
    var modalArchiveReason = $('#modalArchiveReason').val();
    var reasonField = '#ArchiveReason';
    if (modalArchiveReason != "") {
        $(reasonField).val(modalArchiveReason);
        submit(formName);
    }
}

function submit(formName) {
    document.getElementById(formName).submit();
}

function getComment(commentId) {
    $.ajax({
        type: 'GET',
        url: '/Admin/GetArchiveComment',
        data: {
            commentId: commentId
        },
        success: function (data) {
            var obj = JSON.parse(data);
            $('#commentArchived').text(obj.Archived);
            $('#comments').text(obj.Comments);
            $('#commentChangeDate').text(obj.ChangeDate);
            $('#commentChangeUser').text(obj.ChangeUser);
            setCurrentComment('comment' + commentId);
        }
    });
}

function setCurrentComment(commentHtmlId) {
    document.querySelectorAll('.cursor-hover').forEach(function (element) {
        if (element.id == commentHtmlId) {
            document.getElementById(commentHtmlId).style.color = "#fff";
            document.getElementById(commentHtmlId).style.backgroundColor = "#0d6efd";
        } else {
            element.style.color = "#212529";
            element.style.backgroundColor = "#ffffff";
        }
    });
}