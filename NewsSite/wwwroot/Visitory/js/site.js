// Display success message
function SuccessMessage(SuccessTxt) {
    Swal.fire({
        icon: 'success',
        title: 'با موفقیت انجام شد',
        text: SuccessTxt,
    });
}

 Display error message
function ErrorMessage(ErrorTxt) {
    Swal.fire({
        icon: 'error',
        title: 'خطا',
        text: ErrorTxt,
    });
}

// Bind grid content dynamically
async function BindGrid() {
    try {
        let action = $("#dvContent").data("action");
        let controller = $("#dvContent").data("controller");
        let sendingUrl = `/${controller}/${action}`;

        $(".waiting").css("display", "flex");
        let response = await $.get(sendingUrl);

        $(".waiting").css("display", "none");
        $("#dvContent").html(response);
    } catch (error) {
        console.error("Error fetching grid data:", error);
        $(".waiting").css("display", "none");
    }
}

$(document).ready(function () {
    BindGrid();
});

async function postData(sendingUrl, sendingData, successCallback, errorCallback) {
    try {
        $(".waiting").css("display", "flex");
        let response = await $.post(sendingUrl, sendingData);

        $(".waiting").css("display", "none");
        if (response.success) {
            successCallback(response);
        } else {
            ErrorMessage(response.message);
        }
    } catch (error) {
        $(".waiting").css("display", "none");
        console.error("Error posting data:", error);
        errorCallback(error);
    }
}
async function getData(sendingUrl, successCallback) {
    try {
        let response = await $.get(sendingUrl);
        successCallback(response);
    } catch (error) {
        console.error("Error fetching data:", error);
    }
}

$(document).on("click", ".btn-delete", function () {
    if (confirm("آیا مطمن هستید؟")) {
        let action = $(this).data("action");
        let controller = $(this).data("controller");
        let id = $(this).data("id");
        let sendingUrl = `/${controller}/${action}`;
        let sendingData = { id: id };

        postData(sendingUrl, sendingData, function (operationResult) {
            $(`#tr_${id}`).fadeOut(1000);
            BindGrid();
            SuccessMessage(operationResult.message);
        }, function () {
            ErrorMessage("خطا در حذف داده");
        });
    }
});

// Handle add/update button click for opening modal
$(document).on("click", ".btn-add, .btn-update", function () {
    let action = $(this).data("action");
    let controller = $(this).data("controller");
    let id = $(this).data("id") || '';
    let sendingUrl = `/${controller}/${action}?id=${id}`;

    getData(sendingUrl, function (formContent) {
        $("#dvModalContent").html(formContent);
        $("#mainModal").modal("show");
        $.validator.unobtrusive.parse($("#frmAddNewsCategory"));
    });
});

// Handle form submit (Add or Update)
$(document).on("click", ".saveAdd", function (e) {
    e.preventDefault(); 

    let action = $(this).attr("data-action");
    let controller = $(this).attr("data-controller");
    let formId = "#" + $(this).attr("data-form-id");
    let form = $(formId)[0];
    let formData = new FormData(form); 
    let sendingUrl = `/${controller}/${action}`;
    alert(1);
    $.ajax({
        url: sendingUrl,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false, 
        success: function (op) {
            if (op.success) {
                SuccessMessage(op.message);
                BindGrid(); 
            } else {
                ErrorMessage(op.message);
            }
        },
        error: function () {
            ErrorMessage("An error occurred while processing your request.");
        }
    });
});

// Function to delete image
async function DeleteImage(NewsID) {
    try {
        let response = await $.post(deleteImageUrl, { NewsID: NewsID });
        if (response.success) {
            alert("Image deleted successfully.");
            RefreshNewsList();
        } else {
            alert("Error deleting the image.");
        }
    } catch (error) {
        alert("Error occurred while deleting the image.");
        console.error("Delete image error:", error);
    }
}

// Refresh news list dynamically
async function RefreshNewsList() {
    try {
        let response = await $.get(refreshNewsListUrl + "?" + new Date().getTime());
        $("#dvList").html(response);
    } catch (error) {
        alert("Error occurred while refreshing the list.");
        console.error("Refresh list error:", error);
    }
}
