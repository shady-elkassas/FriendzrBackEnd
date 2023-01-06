function DomInDatatable() {
    return "<'row'" +
        "<'col-sm-6 d-flex align-items-center justify-conten-start'l>" +
        "<'col-sm-6 d-flex align-items-center justify-content-end'f>" +
        ">" +

        "<'table-responsive'tr>" +

        "<'row'" +
        "<'col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start'i>" +
        "<'col-sm-12 col-md-7 d-flex align-items-center justify-content-center justify-content-md-end'p>" +
        ">";
}
function CreateCommonDatatableAjax(DatatableConfigration, initCompletefunc = "", UseTableCounter = true, UseDefaultAction = true, drawCallbackfunc = "") {
    if (UseTableCounter == true) {
        DatatableConfigration.Columns=    [{
            "data": "ID",

            "render": function (d, t, f, m) {
                return m.row + 1
            }
        }].concat(DatatableConfigration.Columns);
      
    }
    if (UseDefaultAction == true) {
        DatatableConfigration.Columns.push({

            //"data": "ID",
            "className": "text-center",
            "render": function (data, ypet, full, meta) {
                let ID = full.ID;
                data = ID ?? full.EntityId;
                let Edit = `<a href="javascript:OpenPageModal('${data}')" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1">
                                    <span data-bs-toggle="tooltip" data-bs-placement="top" data-bs-trigger="hover" title="${getToken("Edit")}"><i class="bi bi-pencil-fill fs-5 "></i></span>
                                </a>`
                let Remove = `  <a href="javascript:OpenSweetAlertConfirmModal('${data}','RemovePageObject');" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1" >
                                    <span data-bs-toggle="tooltip" data-bs-placement="top" data-bs-trigger="hover" title="${getToken("Delete")}"><i class="bi bi-trash fs-5 text-danger"></i></span>
                                </a>`
                //let Details = `<a href="javascript:OpenPageModal('${data}')" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1">
                //                    <span data-bs-toggle="tooltip" data-bs-placement="top" data-bs-trigger="hover" title="${getToken("Detailss")}"><i class="bi bi-exclamation-lg fs-3"></i></span>
                //                </a>`
                return Edit + Remove ;
            },
        }
        )

    }
    if (!$.fn.DataTable.isDataTable(DatatableConfigration.TableID)) {
        $(DatatableConfigration.TableID).DataTable({
            //scrollY: 300,
            //scrollCollapse: true,
            searching: true,
            //dom: 'Bfrtip',		// col visibility
            //buttons: [
            //    GetDatatablePrintConfig(DatatableConfigration.TableID), GetDatatableExcelConfig(DatatableConfigration.TableID), GetDatatablePdfConfig(DatatableConfigration.TableID)// col visibility
            //],
            //lengthChange: true,
            dom: 'Blfrtip',
            //dom: 'Blfrtip',
            "dom": DomInDatatable(),


            "processing": true,
            "ajax": {
                "url": DatatableConfigration.Url /*`/Admission/EmployeeAllowance/GetEmployeeAllowances?EmployeeID=${$('input[name="EmployeeVM_STEP1.ID"]').val()}`*/,
                "type": "Get",
                "datatype": "json",
            },
            "lengthMenu": [[10, 50, 100, -1], [10, 50, 100, getToken("All")]],
            'language': {
                'url': DataTableLanguageUrl(),
            },

            "columns": DatatableConfigration.Columns,
            "drawCallback": function (settings) {
                if (typeof (drawCallbackfunc) == "function") {
                    drawCallbackfunc(settings);
                }
            },
            "initComplete": function (settings, json) {
                if (typeof (initCompletefunc) == "function") {
                    initCompletefunc(settings, json);
                }
                //FillAllowancesDropDownlist("#AllowancesList", json.AvilableAllowanceType);
                //Update NewChangingInsuranceValue
                //$('[name="EmployeeVM_STEP4.ChangingInsurance"]').val(json.NewChangingInsuranceValue);
            }
        });
    }
    else {
        new $.fn.dataTable.Api(DatatableConfigration.TableID).ajax.url(DatatableConfigration.Url).load((callback) => {
            //FillAllowancesDropDownlist("#AllowancesList", callback.AvilableAllowanceType)
            //$('[name="EmployeeVM_STEP4.ChangingInsurance"]').val(callback.NewChangingInsuranceValue);

        }, (rest) => {
        });
    }
}
function CustomSettingForDatatablePDF_Salary(doc, TableSelector) {
    let Language = getCookieValue("Usre_Culture");
    Language = (Language == undefined || Language.toLowerCase().indexOf("ar") >= 0) ? "ar" : "en";
    if (Language == "ar") {
        //We Revers Column Order 
        $.each(doc.content[1].table.body, (i, e) => {
            e.reverse();
            $.each(e, (ii, ee) => {
                if (isArabic(ee.text)) {
                    ee.text = ee.text.split(' ').reverse().join(' ');
                }
            })
        });
        doc.styles.tableBodyEven.alignment = "right";
        doc.styles.tableBodyOdd.alignment = "right";
        doc.content[0].alignment = 'right';
        doc.content[1].alignment = 'right';

    }

    else {
        doc.styles.tableBodyEven.alignment = "left";
        doc.styles.tableBodyOdd.alignment = "left";
        doc.content[0].alignment = 'left';
        doc.content[1].alignment = 'left';

    }

    var tblBody = doc.content[1].table.body;
    // ***
    //This section creates a grid border layout
    // ***
    doc.content[1].layout = {
        hLineWidth: function (i, node) {
            return (i === 0 || i === node.table.body.length) ? 2 : 1;
        },
        vLineWidth: function (i, node) {
            return (i === 0 || i === node.table.widths.length) ? 2 : 1;
        },

        layout: {
            paddingLeft: (i, node) => 0,
            paddingRight: (i, node) => 0,
            paddingTop: (i, node) => 10,
            paddingBottom: (i, node) => 10
        },

    };
    //Wirdth 

    var colCount = new Array();
    $(TableSelector).find('tbody tr:first-child td:visible:not(.select-checkbox):not(.HideFromExport)').each(function () {

        if ($(this).attr('colspan')) {
            for (var i = 1; i <= $(this).attr('colspan'); $i++) {
                colCount.push('auto');
                //colCount.push('*');
            }
        }
        else {
            colCount.push('auto');
            //colCount.push('*');
        }

    });

    colCount[($(TableSelector).find('tbody tr:first-child td:visible:not(.select-checkbox):not(.HideFromExport)').length - 1)] = "*";//Change Employee Name Witds
    doc.content[1].table.widths = colCount;
    doc.info = {
        title: 'awesome Document testttt',
        author: 'john doe testtt',
        subject: 'subject of document testtt',
        keywords: 'keywords for document testttt',
    };
    doc.watermark = { text: 'Pioneer-Solutions ', angle: 70, opacity: 0.3, bold: true, italics: true, fontSize: 40 };
    doc.defaultStyle =
    {
        font: 'ArabicFont',
        fontSize: 11,
        bold: false

    };
    doc.pageMargins = [10, 10, 10, 10];

}

function Create_DatatablePDF_Salary(Selector) {
    var DatatableInstance = new $.fn.dataTable.Api(Selector)
    DatatableInstance.button('.buttons-pdf').trigger();
}
function GetDatatablePdfConfig_Salary(TableSelector) {
    return {
        extend: 'pdfHtml5',
        text: 'PDF',
        customize: function (doc) {
            CustomSettingForDatatablePDF_Salary(doc, TableSelector);
        },
        titleAttr: 'Generate PDF',
        className: 'btn-outline-danger btn-sm d-none',

        exportOptions: {
            columns: ':visible:not(.select-checkbox):not(.HideFromExport)',
            orientation: 'landscape',
            pageSize: 'LEGAL'
        },
        orientation: 'landscape',
        pageSize: 'LEGAL'



    }
}
function SearchInDatatable(DataTableID, Text) {
    Text.replace('#', '');
    DatatableObj = new $.fn.dataTable.Api("#" + DataTableID);
    DatatableObj.search(Text).draw();

}
function SelectAllInDatatable(bool, TableSelector) {
    var DatatableInstance = new $.fn.dataTable.Api(TableSelector)

    if ($.fn.DataTable.isDataTable(TableSelector)) {
        if (bool) {
            DatatableInstance.rows().select();
        } else {
            DatatableInstance.rows().deselect();
        }
    }
}
function DataTableLanguageUrl() {
    return CurrentUserLanguage === 'ar-EG' ? '/Components/_SharedComponent/JsonData/DatatableLanguage_Ar.json' : '/Components/_SharedComponent/JsonData/DatatableLanguage_En.json';
}

function CustomSettingForDatatablePDF(doc, TableSelector) {

    let Language = getCookieValue("Usre_Culture");
    Language = (Language == undefined || Language.toLowerCase().indexOf("ar") >= 0) ? "ar" : "en";

    if (Language == "ar") {
        //We Revers Column Order 
        $.each(doc.content[1].table.body, (i, e) => {
            e.reverse();
            $.each(e, (ii, ee) => {
                if (isArabic(ee.text)) {

                    ee.text = ee.text.split(' ').reverse().join(' ');
                }
            })
        });
        doc.styles.tableBodyEven.alignment = "right";
        doc.styles.tableBodyOdd.alignment = "right";
        doc.content[0].alignment = 'right';
        doc.content[1].alignment = 'right';
    }

    else {
        doc.styles.tableBodyEven.alignment = "left";
        doc.styles.tableBodyOdd.alignment = "left";
        doc.content[0].alignment = 'left';
        doc.content[1].alignment = 'left';

    }

    var tblBody = doc.content[1].table.body;
    // ***
    //This section creates a grid border layout
    // ***
    doc.content[1].layout = {
        hLineWidth: function (i, node) {
            return (i === 0 || i === node.table.body.length) ? 2 : 1;
        },
        vLineWidth: function (i, node) {
            return (i === 0 || i === node.table.widths.length) ? 2 : 1;
        },
        hLineColor: function (i, node) {
            return (i === 0 || i === node.table.body.length) ? 'black' : 'gray';
        },
        vLineColor: function (i, node) {
            return (i === 0 || i === node.table.widths.length) ? 'black' : 'gray';
        }
    };
    //Wirdth 
    var colCount = new Array();
    $(TableSelector).find('tbody tr:first-child td:visible:not(.select-checkbox):not(.HideFromExport)').each(function () {

        if ($(this).attr('colspan')) {
            for (var i = 1; i <= $(this).attr('colspan'); $i++) {
                //colCount.push('auto');
                colCount.push('*');

            }
        }
        else {
            //colCount.push('auto');
            colCount.push('*');
        }

    });
    doc.content[1].table.widths = colCount;
    doc.info = {
        title: 'awesome Document testttt',
        author: 'john doe testtt',
        subject: 'subject of document testtt',
        keywords: 'keywords for document testttt',
    };
    doc.watermark = { text: 'Pioneer-Solutions ', angle: 70, opacity: 0.3, bold: true, italics: true, fontSize: 40 };
    doc.defaultStyle =
    {
        font: 'ArabicFont',
        fontSize: 12,
        bold: true

    };

}
function CustomSettingForDatatableExcel(doc, TableSelector) {

    let Language = getCookieValue("Usre_Culture");
    Language = (Language == undefined || Language.toLowerCase().indexOf("ar") >= 0) ? "ar" : "en";

    if (Language == "ar") {
        //We Revers Column Order 
        $.each(doc.content[1].table.body, (i, e) => {
            e.reverse();
            $.each(e, (ii, ee) => {
                if (isArabic(ee.text)) {

                    ee.text = ee.text.split(' ').reverse().join(' ');
                }
            })
        });
        doc.styles.tableBodyEven.alignment = "right";
        doc.styles.tableBodyOdd.alignment = "right";
        doc.content[0].alignment = 'right';
        doc.content[1].alignment = 'right';
        //This section loops thru each row in table looking for where either
        //the second or third cell is empty.
        //If both cells empty changes rows background color to '#FFF9C4'
        //if only the third cell is empty changes background color to '#FFFDE7'
        // ***
        //$('#EmployeesTable').find('tr').each(function (ix, row) {
        //    var index = ix;
        //    var rowElt = row;
        //    $(row).find('td').each(function (ind, elt) {
        //        if (tblBody[index][1].text == '' && tblBody[index][2].text == '' && tblBody[index][ind] != undefined) {
        //            delete tblBody[index][ind].style;
        //            tblBody[index][ind].fillColor = '#FFF9C4';
        //        }
        //        else {
        //            if (tblBody[index][2].text == '' && tblBody[index][ind] != undefined) {
        //                delete tblBody[index][ind].style;
        //                tblBody[index][ind].fillColor = '#FFFDE7';
        //            }
        //        }
        //    });
        //});
    }

    else {
        doc.styles.tableBodyEven.alignment = "left";
        doc.styles.tableBodyOdd.alignment = "left";
        doc.content[0].alignment = 'left';
        doc.content[1].alignment = 'left';

    }

    var tblBody = doc.content[1].table.body;
    // ***
    //This section creates a grid border layout
    // ***
    doc.content[1].layout = {
        hLineWidth: function (i, node) {
            return (i === 0 || i === node.table.body.length) ? 2 : 1;
        },
        vLineWidth: function (i, node) {
            return (i === 0 || i === node.table.widths.length) ? 2 : 1;
        },
        hLineColor: function (i, node) {
            return (i === 0 || i === node.table.body.length) ? 'black' : 'gray';
        },
        vLineColor: function (i, node) {
            return (i === 0 || i === node.table.widths.length) ? 'black' : 'gray';
        }
    };
    //Wirdth 
    var colCount = new Array();
    $(TableSelector).find('tbody tr:first-child td:visible:not(.select-checkbox):not(.HideFromExport)').each(function () {

        if ($(this).attr('colspan')) {
            for (var i = 1; i <= $(this).attr('colspan'); $i++) {
                //colCount.push('auto');
                colCount.push('*');

            }
        }
        else {
            //colCount.push('auto');
            colCount.push('*');
        }

    });
    doc.content[1].table.widths = colCount;
    doc.info = {
        title: 'awesome Document testttt',
        author: 'john doe testtt',
        subject: 'subject of document testtt',
        keywords: 'keywords for document testttt',
    };
    doc.watermark = { text: 'Pioneer-Solutions ', angle: 70, opacity: 0.3, bold: true, italics: true, fontSize: 40 };
    doc.defaultStyle =
    {
        font: 'ArabicFont',
        fontSize: 12,
        bold: true

    };

}
function Create_DatatablePDF(Selector) {
    var DatatableInstance = new $.fn.dataTable.Api(Selector)
    DatatableInstance.button('.buttons-pdf').trigger();
}
function Create_DatatableExcele(Selector) {
    
    var DatatableInstance = new $.fn.dataTable.Api(Selector)
    DatatableInstance.button('.buttons-excel').trigger();
}
function Create_DatatablePrint(Selector) {
    var DatatableInstance = new $.fn.dataTable.Api(Selector)
    DatatableInstance.button('.buttons-print').trigger();
}
function GetDatatablePdfConfig(TableSelector) {
    return {
        extend: 'pdfHtml5',
        text: 'PDF',
        customize: function (doc) {
            CustomSettingForDatatablePDF(doc, TableSelector);
        },
        titleAttr: 'Generate PDF',
        className: 'btn-outline-danger btn-sm d-none',
        exportOptions: {
            columns: ':visible:not(.select-checkbox):not(.HideFromExport)',
            orthogonal: "portrait",
        },



    }
}
function GetDatatableExcelConfig(TableSelector) {
    return {
        extend: 'excel',
        //customize: function (doc) {
        //    CustomSettingForDatatableExcel(doc, TableSelector);
        //},
        //titleAttr: 'Generate PDF',
        className: 'btn-outline-danger btn-sm d-none',
        exportOptions: {
            columns: ':visible:not(.select-checkbox):not(.HideFromExport)',
            //orthogonal: "portrait",
        },



    }
}
function GetDatatablePrintConfig(TableSelector) {
    return {
        extend: 'print',
        //customize: function (doc) {
        //    CustomSettingForDatatableExcel(doc, TableSelector);
        //},
        //titleAttr: 'Generate PDF',
        className: 'btn-outline-danger btn-sm d-none',
        exportOptions: {
            columns: ':visible:not(.select-checkbox):not(.HideFromExport)',
            //orthogonal: "portrait",
        },



    }
}
function isArabic(text) {
    var pattern = /[\u0600-\u06FF\u0750-\u077F]/;
    result = pattern.test(text);
    return result;
}