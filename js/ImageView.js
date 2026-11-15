$(function () {
    $("#chkSelectAll").change(function () {
        if ($(this).is(":checked")) {
            $(".chkSelect").each(function () {
                $(this).prop("checked", true);
            })
        }
        else {
            $(".chkSelect").each(function () {
                $(this).prop("checked", false);
            })
        }
    });
});

$('#BulkImageDelete').click(function () {
    var Items = [];
    $(':checkbox:checked').each(function (i) {
        Items[i] = $(this).val();
    });

    if (confirm("Deleting Multiple Images take long time. Delete Selected Image Records? " + Items)) {
        var TheItemArr = JSON.stringify(Items);
        //var data = { strArray: TheItemArr }
        var data = {};
        show_ajax_loader();
        $.post('/Admin/ImageBulkDelete', data,
        function (data) {
            if (data == true) {
                alert("Deleted Successfully");
                hide_ajax_loader();
            }
            else
            {
                alert("Can't Delete. Contact Support Team.");
                hide_ajax_loader();
            }
        });
    }
})
    

$(document).ready(function () {
    $('#AddImage').click(function () {
        var div = $("#OpenDilog");
        div.load("/Admin/ImageCreate", function () {
            div.dialog({
                modal: true,
                width: 500,
                height: 500,
                title: "Add New Image",
                resizable: false
            });
        });
    });
});


function EditImage(I_ID) {
    var ph = $("#OpenDilog");
    ph.load("/Admin/ImageEdit?ImgID=" + I_ID, function () {
        ph.dialog({
            modal: true,
            width: 500,
            height: 600,
            title: "Edit Image",
            resizable: false
        });
    });
}

function DeleteImage(I_ID) {
    if (confirm("Are You Sure Delete Selected Image Record No.? " + I_ID)) {
        var data = { 'ImgID': I_ID }
        $.post('/Admin/ImageDelete', data,
        function (data) {
            if (data == true)
                window.location.href = 'ImageView';
            else
                alert("Can't Delete. Contact Support Team.");
        });
    }
}

