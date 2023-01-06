$('.appConfigrationcontainer span[name]').keypress(function (event) {
    $(this).removeClass("text-danger");

    if (event.which == 32 || event.which == 13 || (event.which != 8 && isNaN(String.fromCharCode(event.which)))) {
        event.preventDefault(); //stop character from entering input

    }

});
function OpenEditMode(el) {
    //$(el).find("[data-bs-toggle]").tooltip('hide')
    if ($(el).find("i.bi.bi-pencil-fill").length == 0) {
        let DataToSent = $("#appConfigForm").serialize();
        let IsValidData = true;
        $(el).parents(".appConfigrationcontainer").find("span[name]").each((i, spanname) => {
            let num = parseFloat($(spanname).text());
            if (isNaN(num) || num < 0) {
                $(spanname).addClass("text-danger");
                $(spanname).focus();
                IsValidData = false;
            }
            DataToSent += `&${$(spanname).attr("name")}=${num}`;
        });
        if (IsValidData == true) {
            $.post("/admin/controlpanale/_Edit", DataToSent, (res) => {
                if (res.Status) {
                    $(el).parents(".appConfigrationcontainer").find(`span[name]`).each((ii, ell) => {

                        $(ell).siblings("input[type=hidden]").val($(ell).text())
                        $(ell).removeClass("text-danger")
                    })
                    ResetEditMode();
                    ShowNotification("success", res.Message);
                }
                else {
                    let ErrorsList = res.ModelErrors;
                    if (ErrorsList == undefined) {
                        ShowNotification("error", res.Message);
                    }
                    else {
                        $.each(ErrorsList, (key, value) => {

                            $(el).parents(".appConfigrationcontainer").find(`span[name="${key}"]`).addClass("text-danger");
                            ShowNotification("error", value);
                        })

                    }
                }
            });
        }
        else {
            ShowNotification("error", "enter valid numbers please");

        }
    }
    else {
        ResetEditMode();
        let span = `<span >
                      <i class="fas fa-check fs-5 text-success fs-6 fw-bolder">
                      </i>
                      </span>`;
        let appConfigrationcontainer = $(el).parents(".appConfigrationcontainer");
        $(appConfigrationcontainer).find("span[name]").attr("contentEditable", true);
        $($(appConfigrationcontainer).find("span[name]")[0]).focus();
        $(el).html(span);
        $(appConfigrationcontainer).find(".card-toolbar button.canclebtn").removeClass("d-none")

    }
}
function ResetEditMode() {
    $(".appConfigrationcontainer span[name]").each((i, el) => {
        let OldValue = $(el).siblings("input[type='hidden']").val()
        $(el).removeAttr("contentEditable");
        $(el).text(OldValue);

    })
  //  let span = `<span data-bs-toggle="tooltip" data-bs-placement="top" data-bs-trigger="hover" title="" data-bs-original-title="Edit" aria-label="edit">
    let span = `<span >
                      <i class="bi bi-pencil-fill fs-5 text-gray-600 text-hover-primary fs-6 fw-bolder">
                      </i>
                      </span>`;

    $(".appConfigrationcontainer .card-toolbar button").not(".canclebtn").html(span)
    $(".appConfigrationcontainer .card-toolbar button.canclebtn").addClass("d-none")
}