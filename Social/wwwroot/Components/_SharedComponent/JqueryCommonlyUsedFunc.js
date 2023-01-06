

function buildingRowFN(options, object) {
    let rowData = "";
    rowData += "<tr>";
    Object.keys(object).forEach(function (key, index) {
        //validating exepted fields
        if (exeptDataFN(options.exeptData, key)) {
            let myRowData = "";
            //chicking check Boxes fields
            myRowData = checkBoxFieldsFN(options.checkBoxes, key, object);
            //adding default row data
            if (myRowData === "") myRowData = "<td>" + object[key] + "</td>";
            rowData += MyRowData;
        }
    });
    //appending custom buttons
    rowData += appendingCustomButtonsFN(options.customButtons);
    //appending default Buttons
    appendingDefaultButtonsFN(options.defaultButtons, options.id, object);

    rowData += "</tr>";
    return rowData;
}

function getValidationSumryErrorsStr() {
    let li = "";
    $(".field-validation-error").each((i, e) => {
        let InputLableName = $(e).siblings(".form-group").find("label.col-form-label").text();
        let ErrorMassage = $(e).text();
        li += `<li>${getToken("Filed")} _ ${InputLableName} :: ${ErrorMassage}</li>`;
    });
    return `<ul>${li}</ul>`;
}

//getting exepted records
function exeptDataFN(exeptData, key) {
    if (exeptData === undefined) return true;
    if (exeptData.indexOf(key) === -1) return true;
    return false;
}
function appendingCustomButtonsFN(customButtons) {
    if (customButtons === undefined) return "";
    var row = "";
    $(customButtons).each(function () {
        row += "<td>" + this + "</td>";
    })
    return row;
}
function appendingDefaultButtonsFN(defaultButtonsOptions, id, object) {
    if (defaultButtonsOptions.add) {
        rowData += "<td class='p-left'><button data-id='" + object[id] + "' id='btn_edit' onclick='" + defaultButtonsOptions.functions[0] + "' title='تعديل' ><i class='fa fa-edit'></i></button>";
        rowData += "<button title='حذف' data-id='" + object[id] + "' onclick='" + defaultButtonsOptions.functions[1] + "' id='btn_delete' ><i class='fa fa-times'></i></button></td>";
    }
}
function checkBoxFieldsFN(checkBoxFields, key, object, id) {
    if (checkBoxFields === undefined) return "";
    var row = "";
    let indexOfItem = checkBoxFields.indexOf(key);
    if (indexOfItem !== -1) {
        let checked = object[key] === true ? "checked" : "";
        row = "<td> <label class='kt-checkbox pr-30 kt-checkbox--bold kt-checkbox--success pt-3' >" +
            "<input  data-id='" + object[id] + "'  id='chk_" + key + "' type='checkbox' class='checkbox-inline' autocomplete='off' " + checked + " disabled>" +
            "<span></span>" +
            "</label></td>";
    }

    return row;
}


function CreateFilePondForElment(elment, multifiles = false, files = []) {
    $(elment).filepond({
        allowMultiple: multifiles,
        allowReorder: true,

        files: files
    });



}
//this FuncTion To Creat File Pondd For input Pass Elment And Pass what You Whant To Do when rremove and when Add file function
function CreateFilePondObj(Selector, allowMultiple = true, OnAddFileFunc_TwoParameters = "", OnRemoveFile_TwoParameter = "") {

    let obj = FilePond.create(document.querySelector(Selector));
    let DefaultOptions = {
        allowReorder: true,
        allowMultiple: allowMultiple,
        dropValidation: true,
        dropOnPage: true,
        labelFileTypeNotAllowed: getToken("FileNotSupportedHere"),
        fileValidateTypeDetectType: (source, type) => new Promise((resolve, reject) => {
            resolve(type);
        })
    }
    obj.setOptions(DefaultOptions);

    obj.on("addfile", (error, file) => {
        ;
        if (error) {

            obj.removeFile(file.id);
            ShowNotification("error", getToken("FileNotSupportedHere"));
            return;
        }
        else {
            $(Selector).removeClass("CustomNotValiderror")

        }
        if (typeof OnAddFileFunc_TwoParameters === "function") {
            OnAddFileFunc_TwoParameters(error, file);
        }
    });

    obj.on('removefile', (error, file) => {
        if (typeof OnRemoveFile_TwoParameter === "function") {
            OnRemoveFile_TwoParameter(error, file);
        }
    });
    return obj;
}
function populateJsonObj_ToForm(form, data, prefex = "", WithSelectChangeEventFire = true, withChosenSelectChangeEventFire = true) {

    $.each(data, function (key, value) {
        key = prefex + key;
        var elment = $(`[name="${key}"]`, form);
        if (elment.length > 0 && $(elment)[0].type == "select" && value == null) {
            $(elment).find('option:first').attr('selected', 'selected');
        }
        else if (elment.length > 0 && $(elment)[0].type == "date" && value != null) {
            //let DateFormated = $.format.date(value, 'yyyy/MM/dd');
            let jsDate = ConvertCSharpDateTojsDate(value);
            let DateFormated = $.datepicker.formatDate('yy-mm-dd', jsDate);
            $(elment).val(DateFormated);
        }
        else if (elment.length > 0 && $(elment)[0].type == "time" && value != null) {
            $(elment).removeAttr("value")

            if (Date.parse(value)) {
                //Valid date
            $(elment).val(moment(value).format("HH:mm"));
            }
            else {
                //Not a valid date
                let tt = moment(value, 'HH:mm').format("HH:mm");
                $(elment).val(tt);
               
            }
            //$(elment).val(moment(value).format("HH:mm"));
        }
        //Handel elment With Togglebootstrap
        else if ($(elment).attr("data-toggle") !== undefined && $(elment)[0].type == "checkbox") {
            $(elment).bootstrapToggle(value == true ? 'on' : 'off')
        }
        else if ($(elment).hasClass("glDatePicker")) {

            let DateFormated = $(elment).glDatePicker().val(value).glDatePicker();

            //$(elment).val(DateFormated);
        }
        else if (elment.length > 0 && $(elment)[0].type == "checkbox") {
            //check If More Than One Checkbox
            $(form).find("[type='checkbox']").prop("checked", false);

            if (Array.isArray(value)) {
                $.each(value, function (ii, value2) {
                    let CheckboxElment = $('[name=' + key + '][value=' + value2 + ']', form);
                    $(CheckboxElment).prop("checked", true);
                })
            }
            else {
                $(elment).prop("checked", value);
            }
        }
        else if (elment.length > 0 && $(elment)[0].type == "radio") {
            $(elment).filter(function () { return this.value == value }).prop("checked", true);
        }
        else if (value != null) {
            $(elment).val(value);
        }
    });
    if (withChosenSelectChangeEventFire == true) {
        $(".chosen-select").trigger("chosen:updated");
    }
    if (WithSelectChangeEventFire == true) {
        $(form).find("select").trigger("change");
    }
    $("#PageForm input[type='checkbox']").change();

    $(document).trigger("populateJsonObj_ToForm_Finished", data);
}


function IsValidEmail(Email) {
    //ValidateStudentEmailAddress

    var testEmail = /^[A-Z0-9._%+-]+@([A-Z0-9-]+\.)+[A-Z]{2,4}$/i;
    if (testEmail.test(Email))
        return true;
    else
        return false;
}
function OpenBootBoxConfirmModal(ID, Func) {
    bootbox.confirm(getToken("ConfirmDelete"), function (flag) {
        if (flag == true) {
            
            if (typeof window[Func] === "function") {

                //Func = window[Func](ID);
                Func(ID);

            }
            else if (typeof Func === "function") {
                Func(ID);
            }

        }

    });

}
function OpenSweetAlertConfirmModal(ID, Func, title = "", text = "", icon = "error") {
    text = text == "" ? getToken("AreYouShure") : text;
    title = title == "" ? getToken("alert") : title;
    new  swal({
        title: title,
        text: text,
        icon: icon,
        confirmButtonText: getToken("Confirm"),
        cancelButtonText: getToken("Cancele"),
        customClass: {
            confirmButton: "btn btn-danger",
            cancelButton: 'btn btn-primary'
        },
        buttonsStyling: false,
        showCancelButton: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete.value == true)
            {
                $(document).trigger(Func, ID, (re) => {
                });
            }
            else
            {
                //swal("Your imaginary file is safe!");
            }
        });


}
function OpenBootBoxConfirmModal_V2(ID, Func, MessageToDisplay, title = "") {
    bootbox.dialog({
        message: `<div class="hrDel-modal"><div class="del-confirm text-center"><i class="fas fa-info-circle text-danger"></i><h4 class="font-weight-bold">${title}</h4><p>${MessageToDisplay}</p></div></div>`,
        closeButton: false,
        backdrop: true,
        centerVertical: true,
        buttons: {
            cancel: {
                label: getToken("cancel"),
                className: 'btn-secondary',
                callback: function () { }
            },
            delete: {
                label: getToken("Confirmation"),
                className: 'btn-danger',
                callback: function () {
                    if (typeof window[Func] === "function") {

                        Func = window[Func](ID);
                    }
                    else if (typeof Func === "function") {
                        Func(ID)
                    }
                }
            }
        }
    });


}

//Function To Create Drop Down Cascade Url : ans Selector to pass elment will affected by result or pass function to run after request success
function DrawCascadeDropDown(url, selector = "", dataToSent = null, SuccessFunc = "", AfterDrawNewList = "") {
    $.ajax({
        url: url,
        method: "get",
        dataType: "json",
        data: dataToSent,
        success: function (res) {
            if (typeof SuccessFunc === "function") {
                SuccessFunc(res);
            }
            else if (selector != "") {
                //RemoveAll Old Option with Value
                $(selector).find("option").each((i, e) => {

                    if ($(e).val() != "" && $(e).val() != undefined) {
                        $(e).remove()
                    }
                });
                let DropDownData = Array.isArray(res) == true ? res : res.DropdownListData;

                $.each(DropDownData, function (i, e) {
                    let opt = `<option value="${e.id}">${e.name}</option>`;
                    $(selector).append(opt);
                });
                if (typeof AfterDrawNewList === "function") {
                    AfterDrawNewList(res);
                }
            }
            $(selector).change();
            $(".chosen-select").trigger("chosen:updated");


        }
    });
}
function ReplaceOptionsInDropDownlist(SelectElment, ListOfData, SelectOptionValue = "id", SelectOptiontext = "name") {
    let Options = ``;
    $(SelectElment).find("option").each((i, e) => {
        if ($(e).val() != "" && $(e).val() != undefined) {
            $(e).remove()
        }
    });
    $.each(ListOfData, (i, obj) => {
        let attributes = "";
        let OptionText = "";
        let OptionValue = "";
        //Loobthrogh Json

        $.each(obj, (key, Value) => {
            if (key.toLowerCase() == SelectOptionValue) {
                OptionValue = Value;
            }
            else if (key.toLowerCase() == SelectOptiontext) {

                OptionText = Value;
            }
            else {

                attributes += key + "=" + value;
            }
        });

        Options += `<option value=${OptionValue} ${attributes} >${OptionText}</option>`

    });
    $(SelectElment).append(Options);
    $(SelectElment).trigger("chosen:updated");

}
//Remove File From File Pond By ID
function RemoveAllFilesFromFilePond(FilePondObj) {
    $.each(FilePondObj.getFiles(), function (i, El) {
        FilePondObj.removeFile(El.id)
    });
}
// this function return form data object have files in filepond object if exist and null if no files 
function GetFilePondFilesInFormData(selector, FormDataObj, NameInFormData) {
    let Files = $(selector).filepond('getFiles');
    if (Files.length == 0) {
        return FormDataObj;
    }
    else {

        for (let Counter in Files) {

            if (!isNaN(Counter)) {
                let MyFile = Files[Counter].file;
                //this line To Convert Blob To file
                if (MyFile.name !== undefined) {
                    let Newfile = new File([MyFile], MyFile.name)
                    FormDataObj.append(NameInFormData, Newfile);
                }
                else {
                    FormDataObj.append(NameInFormData, MyFile);
                }
            }
        }
        return FormDataObj;
    }

}

//JQ Valiation




$.validator.setDefaults({ ignore: ":hidden:not(.chosen-select)" })


function ConvertCSharpDateTojsDate(DateCSharpFormat) { ///Date(1460008501597)/
    if (DateCSharpFormat.split("T").length == 2) {
        return new Date(DateCSharpFormat.split("T")[0]);
    }
    else {
        return new Date(Number(DateCSharpFormat.replace(/\D/g, '')))
    }
}

function GetDataFromFormAsFormDataObj(FormDataObj, formElement, Prefextext = "", suffix = "") {
    $(formElement).find("[name]").each((i, el) => {
        let Key = Prefextext + $(el).attr("name") + suffix;
        let Value = $(el).val();
        FormDataObj.append(Key, Value);
    });
    return FormDataObj;
}





//Datatble

function DeleteCertificatedRows(ID, Controller, RefreshTableFunc = "", Area = "Admission", Action = "DeleteCertificated", func = "") {
    bootbox.confirm(getToken("ConfirmDeleteCertficated"), function (flag) {
        if (flag == true) {
            let Url = `/${Area}/${Controller}/${Action}`;

            if (Area.indexOf("api") >= 0) {/// For Api Only ?????
                Url = Url + "?id=" + ID
            }
            $.post(Url, { ID: ID }, (res) => {
                if (func == "") {

                    if (res.Status == true) {
                        if (RefreshTableFunc == "") {
                            location.reload();
                        }
                        else if (typeof window[RefreshTableFunc] === "function") {

                            RefreshTableFunc = window[RefreshTableFunc]();
                        }
                        else if (typeof RefreshTableFunc === "function") {
                            RefreshTableFunc()
                        }

                        ShowNotification("success", getToken("SuccessfullyDeleted"))
                    }
                    else if (res.Status == false) {
                        ShowNotification("error", res.Message)

                    }
                    else {
                        ShowNotification("error", getToken("DoesNotHavepermissionForThis"))

                    }
                }
                else {

                }

            });

        }

    });

}

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

