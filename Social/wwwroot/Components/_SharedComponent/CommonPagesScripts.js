function ResetPageForm() {
    $("#PageForm").trigger("reset");
    $("#ID").val(0);
    $("#EntityId").val(0);  
    clearValidation("#PageForm");
    $("#PageForm input[type='checkbox']").change();
    $("#PageModal input[type='time']").val("");
   // $('#PageModal select option:eq(3)').prop('selected', 'selected');    

    $("[asp-for='categorieId']").innerHTML = "";
    
}

function GetPageFormObj(ID) {

    $.get(`/${CurrentPageUrlInfo.Area}/${CurrentPageUrlInfo.Controller}/GetObj?ID=${ID}`, (res) => {
        populateJsonObj_ToForm($("#PageForm").first(), res);
        $("#PageModal").modal("show");

    }).fail((xhr, textStatus, errorThrown) => {
        ShowNotification("error", "");
    });
}
function DisplayDetails(ID) {
    GetPageFormObj(ID);
}
function OpenPageModal(ID) {
    
    if (ID == 0 || ID == "" || ID == null) {
        ResetPageForm();
        $("#PageModalAction").text((getToken("Add")));
        $("#PageModal").modal("show");
    }
    else {
        $("#PageModalAction").text((getToken("Edit")));
        ResetPageForm();
        GetPageFormObj(ID);
    }
}
$("#cnclButtn").on("click", function () {
    $(document).trigger("PageFormSubmit_Finished");
});
$("#PageForm").on("submit", (e) => {

    e.preventDefault();
    clearValidation("#PageForm");
    let Form = $(e.currentTarget);
    let ID = $(Form).find("[name='ID']").val() ?? $(Form).find("[name='Id']").val();
    let Action = (ID == 0 || ID == "" || ID == null) ? "/Create" : "/Edit";
    let Controller = "/" + CurrentPageUrlInfo.Controller;
    let Area = (CurrentPageUrlInfo.Area == null || CurrentPageUrlInfo.Area == undefined || CurrentPageUrlInfo.Area == "") ? "" : "/" + CurrentPageUrlInfo.Area;
    let Url = Area + Controller + Action;

    if ($(Form).valid()) {
        if ($(Form).find("input[type='file']").length == 0) {
            let DataToSent = $(Form).serialize();
            $.post(Url, DataToSent, (res) => {
                
                if (res.Status == true) {
                    ShowNotification("success", res.Message);
                    RefreshPageDatatable();
                    ResetPageForm();
                    if (Action != "/Create") {
                    }
                    $(Form).parents(".modal").modal("hide");
                    
                }
                else {
                    AssignModelResponseErrorsToControllers(res.ModelErrors, Form);
                    ShowNotification("warning", res.Message);                    
                }                
            }).fail((xhr, textStatus, errorThrown) => {
                ShowNotification("error",errorThrown);

            });
        }
        else {
            let DataToSent = new FormData($(Form).get(0));
            $(Form).find(".filepond--root").each((i, elment) => {
                let filepondivID = "#" + $(elment).attr("id");
                let InputFileDefaultName = $(elment).find("input[name][type='hidden']").attr("name") ?? "Attachment";
                DataToSent = GetFilePondFilesInFormData(filepondivID, DataToSent, InputFileDefaultName)
            });


            $.ajax({
                "url": Url,
                "method": "Post",
                "datatype": "json",
                processData: false,
                contentType: false,
                //"Content-Type": "application/json; charset=utf8",
                data: DataToSent,
                success: function (res) {

                    if (res.Status == true) {
                        $(document).trigger("PageFormSubmit_Finished");

                        ShowNotification("success", res.Message);
                        RefreshPageDatatable();
                        ResetPageForm();
                        $(Form).parents(".modal").modal("hide");
                        //if (Action != "/Create") {
                        //}
                    }
                    else {
                        AssignModelResponseErrorsToControllers(res.ModelErrors, Form);
                        var messag = res.Message + " " + (res.ModelErrors["lang"] == undefined ? "" : res.ModelErrors["lang"]);
                        ShowNotification("warning", messag);
                    }

                },
                error: function (res, tt, ds) {
                    ShowNotification("error", res.Message);

                },

            }).done(function (data) {              
            });
        }
    }
    else {

    }
});
$(document).on("RemovePageObject", function (event, ID, callBack) {
    $.post(`/${CurrentPageUrlInfo.Area}/${CurrentPageUrlInfo.Controller}/RemoveObj?ID=${ID}`, (res) => {
        if (res.Status == true) {
            ShowNotification("success", res.Message);
            RefreshPageDatatable();

        }
        else {
            ShowNotification("error", res.Message);
        }

    }).fail((xhr, textStatus, errorThrown) => {
        ShowNotification("error", "");

    });


});
$(document).ajaxSend(function (event, jqxhr, settings) {
    var el = $(event.target.activeElement);
    if ($(el).prop('nodeName') != undefined) {
        var elementType = $(el).prop('nodeName').toLowerCase();
        switch (elementType) {
            case "input":
                break;
            case "button":
                let indicator_progress = $(el).find("span.indicator-progress");
                let indicator_label = $(el).find("span.indicator-label");
                if (indicator_progress.length > 0) {
                    $(indicator_progress).addClass("d-inline");
                    $(indicator_progress).removeClass("d-none");
                    $(indicator_label).addClass("d-none");
                    $(indicator_label).removeClass("d-inline");
                    $(el).prop('disabled', true);
                    settings.selector = $(el);
                }
                break;
            default:
        }
    }
});
function CreateSelect2DropDown(Selectselector, parentselector = ".modal-body") {
    $(Selectselector).each((i, e) => {
        //let parent = $("#PageModal");
        let parent = $(e).parents(parentselector);
        //let parent = $(e).parents("form");
        $(e).select2({
            dropdownParent: parent,
            dir: CurrentPageUrlInfo.dir,
        }).on('select2:open', (e) => {

            //document.querySelector('#PageModal .select2-search__field').focus();
        });
    })
}

$(document).ajaxComplete(function (event, xhr, settings) {
    if (settings.selector !== undefined) {
        let el = $(settings.selector);
        let indicator_progress = $(el).find("span.indicator-progress");
        let indicator_label = $(el).find("span.indicator-label");
        if (indicator_progress.length > 0) {
            $(indicator_progress).removeClass("d-inline");
            $(indicator_progress).addClass("d-none");
            $(indicator_label).removeClass("d-none");
            $(indicator_label).addClass("d-inline");
            $(el).prop('disabled', false);
        }
    }
});