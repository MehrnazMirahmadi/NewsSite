function SuccessMessage(SuccessTxt) {
    Swal.fire({
        icon: 'success',
        title: 'با موفقیت انجام شد ',
        text: SuccessTxt,
    });
}
function ErrorMessage(ErrorTxt) {
    Swal.fire({
        icon: 'error',
        title: 'خطا',
        text: ErrorTxt,
    });
}
function BindGrid() {
    let action = $("#dvContent").attr("data-action");
    let controller = $("#dvContent").attr("data-controller");
    let sendingUrl = `/${controller}/${action}`;
    let sendingData = null;
    $(".waiting").css("display", "flex");
    $.get(sendingUrl, sendingData, function (recevingData) {
        $(".waiting").css("display", "none");
        $("#dvContent").html(recevingData);
    });
}
$(document).ready(function () {


    BindGrid();

});
$(document).on("click", ".btn-delete", function () {
    if (confirm("آیا مطمن هستید")) {
        let action = $(this).attr("data-action");
        let controller = $(this).attr("data-controller");
        let id = $(this).attr("data-id");
        let sendingUrl = `/${controller}/${action}`;
        let sendingData = `id=${id}`;
        $(".waiting").css("display", "flex");
        $.post(sendingUrl, sendingData, function (operationResult) {
            debugger;
            console.log(operationResult);
            if (operationResult.success.toString() == "true") {
                var trid = `#tr_${id}`;
                console.log(trid);
                $(trid).fadeOut(1000);
                BindGrid();
                $(".waiting").css("display", "none");

                SuccessMessage(operationResult.message);
            }
            else {
                $(".waiting").css("display", "none");
                ErrorMessage(operationResult.message);
            }

        });

    }

});


$(document).on("click", ".btn-update", function () {

    let action = $(this).attr("data-action");
    let controller = $(this).attr("data-controller");
    let id = $(this).attr("data-id");
    let sendingUrl = `/${controller}/${action}`;
    let sendingData = `id=${id}`;
    $.get(sendingUrl, sendingData, function (frmUpdate) {
        // $.validator.unobtrusive.parse($(frmUpdate));

        $("#dvModalContent").html(frmUpdate);


        $("#mainModal").modal("show");
        $.validator.unobtrusive.parse($("#frmUpdateNewsCategory"));
    });

});
$(document).on("click", ".saveUpdate", function () {

    let action = $(this).attr("data-action");
    let controller = $(this).attr("data-controller");
    let formid = "#" + $(this).attr("data-form-id");

    let sendingUrl = `/${controller}/${action}`;
    let sendingData = $(formid).serialize();
    $.post(sendingUrl, sendingData, function (op) {
        if (op.success.toString() == "true") {

            $("#mainModal").modal("hide");
            BindGrid();


            SuccessMessage(op.message);
        }
        else {

            ErrorMessage(op.message);
        }

    });

});

//saveUpdate
$(document).on("click", ".saveAdd", function () {

    let action = $(this).attr("data-action");
    let controller = $(this).attr("data-controller");
    let formid = "#" + $(this).attr("data-form-id");
 
    let sendingUrl = `/${controller}/${action}`;
    let sendingData = $(formid).serialize();
    $.post(sendingUrl, sendingData, function (op) {
        if (op.success.toString() == "true") {

            $("#mainModal").modal("hide");
            BindGrid();

            SuccessMessage(op.message);
        }
        else {

            ErrorMessage(op.message);
        }

    });
});

//btnAdd

$(document).on("click", ".btn-add", function () {
    console.log("Button clicked");

    let action = $(this).attr("data-action");
    let controller = $(this).attr("data-controller");

    let sendingUrl = `/${controller}/${action}`;
    console.log("Sending URL: " + sendingUrl);
    $.get(sendingUrl, null, function (frmAdd) {
        console.log("Form content received");
        $("#dvModalContent").html(frmAdd);

        $("#mainModal").modal("show");
        $.validator.unobtrusive.parse($("#frmAddNewsCategory"));
    }).fail(function () {
        console.error("Error fetching form");
    });
});


function DeleteImage(NewsID) {
    $.ajax({
        url: deleteImageUrl,
        type: 'POST',
        data: { NewsID: NewsID },
        cache: false, 
        success: function (op) {
            if (op.success) {
                alert("Success");
                RefreshNewsList(); 
            } else {
                alert("Error");
                //RefreshNewsList(); 
            }
        },
        error: function () {
            alert("Error occurred while deleting the image.");
        }
    });
}

function RefreshNewsList() {
    $.ajax({
        url: refreshNewsListUrl + "?" + new Date().getTime(),
        type: 'GET',
        cache: false,
        success: function (receivingData) {
            $("#dvList").html(receivingData); 
        },
        error: function () {
            alert("Error occurred while refreshing the list.");
        }
    });
}

