toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toastr-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "30000",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "10000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

var NotificationType = {
    Success:1,
    Error:2,
    Warning:3,
    Info:4

}

function ShowNotification(NotificationType, Message, Title) {
    Title = Title == undefined ? getToken("Notification") : Title;
    NotificationType = NotificationType.toLowerCase();
    switch (NotificationType) {
        case "success":
            toastr.success(Message,Title);
            break;
        case "info":
            toastr.info(Message, Title);
            break;
        case "warning":
            toastr.warning(Message, Title);
            break;
        case "error":
            toastr.error(Message, Title);
            break;
        default:
            break;
    }
}