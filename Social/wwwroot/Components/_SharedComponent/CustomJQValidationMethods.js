$.validator.addMethod("dategreaterthan", function (value, element, params) {
    var otherProp = $('#' + params)
    return Date.parse(value) < Date.parse(otherProp.val());
});

$.validator.unobtrusive.adapters.add("dategreaterthan", ["datetocomparefieldname"], function (options) {
    options.rules["dategreaterthan"] =  options.params;
    options.messages["dategreaterthan"] = options.message;
});
$.validator.addMethod("timegreaterthan", function (value, element, params) {
    var otherProp = $(`[name="${params.timetocomparefieldname}"]`)
    return value == "" || otherProp.val()== ""|| Date.parse("01/01/2011 " + value) > Date.parse("01/01/2011 "+otherProp.val());
});

$.validator.unobtrusive.adapters.add("timegreaterthan", ["timetocomparefieldname"], function (options) {
    options.rules["timegreaterthan"] =  options.params;
    options.messages["timegreaterthan"] = options.message;
});


$.validator.unobtrusive.adapters.add('requiredif', ['dependentproperty', 'desiredvalue'], function (options) {
    
    options.rules['requiredif'] = options.params;
    options.messages['requiredif'] = options.message;
});

$.validator.addMethod('requiredif', function (value, element, parameters) {
    
    var desiredvalue = parameters.desiredvalue;
    desiredvalue = (desiredvalue == null ? '' : desiredvalue).toString();
    var controlType = $("input[name$='" + parameters.dependentproperty + "']").attr("type");
    var actualvalue = {}
    if (controlType == "checkbox" || controlType == "radio") {
        var control = $("input[name$='" + parameters.dependentproperty + "']:checked");
        actualvalue = control.val();
    }
    else {
        actualvalue = $(`[name="${parameters.dependentproperty}"]`).val();
    }
   
     if (actualvalue == ""||$.trim(desiredvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase()) {
        var isValid = $.validator.methods.required.call(this, value, element, parameters);
        return isValid;
    }

    return true;
    d
});

jQuery.validator.addMethod("IsDate", function (value, element) {

    if (value == "") return false;
    else if (Date.parse(value) == NaN) return false;
    else return true;

}, getToken("NotValidDate"));
jQuery.validator.addMethod("DateCanNotLessthanToday", function (value, element) {
    var DateToday = $.datepicker.formatDate('yy/mm/dd', new Date());
    var DateOfInput = $.datepicker.formatDate('yy/mm/dd', new Date(value));
    return new Date(DateOfInput) >= new Date(DateToday);


}, getToken("DateCanNotLessthanToday"));
jQuery.validator.addMethod("EndDateGreaterThanStart",
    function (value, element, params) {
      
        //Check If Params is elment or not

        var Params = $(params).length > 0 ? $(params).val() : params;

        if (!/Invalid|NaN/.test(new Date(value))) {
            return new Date(value) > new Date(Params);
        }

        return isNaN(value) && isNaN(Params)
            || (Number(value) > Number(Params));
    }, jQuery.validator.messages.EndDateGreaterThanStart);

jQuery.validator.addMethod("EndDateGreaterThanOrequalStart",
    function (value, element, params) {

        //Check If Params is elment or not

        var Params = $(params).length > 0 ? $(params).val() : params;

        if (!/Invalid|NaN/.test(new Date(value))) {
            return new Date(value) >= new Date(Params);
        }

        return isNaN(value) && isNaN(Params)
            || (Number(value) > Number(Params));
    }, jQuery.validator.messages.EndDateGreaterThanOrequalStart);


$.validator.addMethod('NumberMoreThanZero',
    function (value, element) {

        if (isNaN(parseFloat(value)) && value != "") {
            return false;
        }
        else {

        }
        return parseFloat(value) > 0;
    }, jQuery.validator.messages.NumberMoreThanZero);

$.validator.addMethod('LessThan',
    function (value, element, params) {

        if (!/Invalid|NaN/.test(new Date(value)) && isNaN(value) == true) {
            return new Date(value) < new Date($(params).val());
        }
        return isNaN(value) && isNaN($(params).val())
            || (Number(value) < Number($(params).val()));
        //return this.optional(element) || val.length >= $(element).data('min');
    }, jQuery.validator.messages.LessThan);



function clearValidation(formElement) {
    //Internal $.validator is exposed through $(form).validate()
    var validator = $(formElement).validate();
    //Iterate through named elements inside of the form, and mark them as error free
    $('[name]', formElement).each(function () {
        validator.successList.push(this);//mark as error free
        validator.showErrors();//remove error messages if present
    });
    validator.resetForm();//remove error class on name elements and clear history
    validator.reset();//remove all error and success data
}

function AssignModelResponseErrorsToControllers(ErrorsList, formElment) {
    let FristTabID = "";
    $.each(ErrorsList, (key, value) => {
        $(formElment).find(`span[data-valmsg-for="${key}"]`).html(`<span for="${key}" class="">${getToken(value)}</span>`);
        if ($(formElment).find(`span[data-valmsg-for="${key}"]`).parents('.tab-pane[role="tabpanel"]').length > 0) {
            let tabid = $(formElment).find(`span[data-valmsg-for="${key}"]`).parents('.tab-pane[role="tabpanel"]').attr("id");
            FristTabID = FristTabID == "" ? tabid : FristTabID;
            //var triggerFirstTabEl = document.querySelector(`[href="#${tabid}"]`)
            //bootstrap.Tab.getInstance(triggerFirstTabEl).show() // Select first tab
        }
    });
    //$('.tab-pane').removeClass('active').removeClass("show");
    //$('.tab-pane#' + FristTabID).addClass('active').addClass("show");
    //$('.nav-tabs a[href="#' + FristTabID + '"]').tab('show');
}