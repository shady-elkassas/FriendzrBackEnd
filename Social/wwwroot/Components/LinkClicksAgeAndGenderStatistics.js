
let linlType = ['Share', 'Help', 'TipsAndGuidance', 'AboutUs', 'TermsAndConditions', 'PrivacyPolicy', 'SkipTutorial', 'SupportRequest', 'SortByInterestMatch']

var ShareLinkClicksGraphchart_Data = "";
var HelpLinkClicksGraphchart_Data = "";
var TipsAndGuidanceLinkClicksGraphchart_Data = "";
var AboutUsLinkClicksGraphchart_Data = "";
var TermsAndConditionsLinkClicksGraphchart_Data = "";
var PrivacyPolicyLinkClicksGraphchart_Data = "";
var SkipTutorialLinkClicksGraphchart_Data = "";
var SupportRequestLinkClicksGraphchart_Data = "";
var SortByInterestMatchGraphchart_Data = "";


function CreateShareLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#ShareLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "ShareLinkClickStatisticsGrapghDataByGender" : "ShareLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (ShareLinkClicksGraphchart_Data == "") {

        ShareLinkClicksGraphchart_Data = new ApexCharts(element, options);
        ShareLinkClicksGraphchart_Data.render();
    }
    else {
        ShareLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateHelpLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#HelpLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "HelpLinkClickStatisticsGrapghDataByGender" : "HelpLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (HelpLinkClicksGraphchart_Data == "") {

        HelpLinkClicksGraphchart_Data = new ApexCharts(element, options);
        HelpLinkClicksGraphchart_Data.render();
    }
    else {
        HelpLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateTipsAndGuidanceLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#TipsAndGuidanceLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "TipsAndGuidanceLinkClickStatisticsGrapghDataByGender" : "TipsAndGuidanceLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (TipsAndGuidanceLinkClicksGraphchart_Data == "") {

        TipsAndGuidanceLinkClicksGraphchart_Data = new ApexCharts(element, options);
        TipsAndGuidanceLinkClicksGraphchart_Data.render();
    }
    else {
        TipsAndGuidanceLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateAboutUsLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#AboutUsLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "AboutUsLinkClickStatisticsGrapghDataByGender" : "AboutUsLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (AboutUsLinkClicksGraphchart_Data == "") {

        AboutUsLinkClicksGraphchart_Data = new ApexCharts(element, options);
        AboutUsLinkClicksGraphchart_Data.render();
    }
    else {
        AboutUsLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateTermsAndConditionsLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#TermsAndConditionsLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "TermsAndConditionsLinkClickStatisticsGrapghDataByGender" : "TermsAndConditionsLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (TermsAndConditionsLinkClicksGraphchart_Data == "") {

        TermsAndConditionsLinkClicksGraphchart_Data = new ApexCharts(element, options);
        TermsAndConditionsLinkClicksGraphchart_Data.render();
    }
    else {
        TermsAndConditionsLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreatePrivacyPolicyLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#PrivacyPolicyLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "PrivacyPolicyLinkClickStatisticsGrapghDataByGender" : "PrivacyPolicyLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (PrivacyPolicyLinkClicksGraphchart_Data == "") {

        PrivacyPolicyLinkClicksGraphchart_Data = new ApexCharts(element, options);
        PrivacyPolicyLinkClicksGraphchart_Data.render();
    }
    else {
        PrivacyPolicyLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateSkipTutorialLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#SkipTutorialLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "SkipTutorialLinkClickStatisticsGrapghDataByGender" : "SkipTutorialLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (SkipTutorialLinkClicksGraphchart_Data == "") {

        SkipTutorialLinkClicksGraphchart_Data = new ApexCharts(element, options);
        SkipTutorialLinkClicksGraphchart_Data.render();
    }
    else {
        SkipTutorialLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateSupportRequestLinkClicksPerMonthsGraph(alldata) {
    let isByGender = $("#SupportRequestLinkClickStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "SupportRequestLinkClickStatisticsGrapghDataByGender" : "SupportRequestLinkClickStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (SupportRequestLinkClicksGraphchart_Data == "") {

        SupportRequestLinkClicksGraphchart_Data = new ApexCharts(element, options);
        SupportRequestLinkClicksGraphchart_Data.render();
    }
    else {
        SupportRequestLinkClicksGraphchart_Data.updateOptions(options);
    }
}

function CreateSortByInterestMatchPerMonthsGraph(alldata) {
    let isByGender = $("#SortByInterestMatchStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "SortByInterestMatchStatisticsGrapghDataByGender" : "SortByInterestMatchStatisticsGrapghDataByAge");

    let ListOfcolors = isByGender ? ['#009EF7', '#D9214E', '#181c32', '#33FFFF'] : ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            opposite: IsRtl
        },
        series: IsRtl == true ? Serieslist.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : Serieslist,
        chart: {
            type: 'bar',
            height: 350
        },

        legend: {
            show: true,
            showForSingleSeries: false,
            showForNullSeries: true,
            showForZeroSeries: true,
            position: 'bottom',
            horizontalAlign: 'center',
            floating: false,
            fontSize: '14px',
            fontFamily: 'Helvetica, Arial',
            fontWeight: 400,
            formatter: undefined,
            inverseOrder: false,
            width: undefined,
            height: undefined,
            tooltipHoverFormatter: undefined,
            customLegendItems: [],
            offsetX: 0,
            colors: ListOfcolors,
            offsetY: 0,
            labels: {
                useSeriesColors: false
            },
            markers: {
                width: 12,
                height: 12,
                strokeWidth: 0,
                radius: 12,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '55%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IsRtl ? Monthes.reverse() : Monthes,
        },
        fill: {
            opacity: 1,
            type: 'solid',
            colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + getToken("User")
                }
            }
        }
    };
    if (SortByInterestMatchGraphchart_Data == "") {

        SortByInterestMatchGraphchart_Data = new ApexCharts(element, options);
        SortByInterestMatchGraphchart_Data.render();
    }
    else {
        SortByInterestMatchGraphchart_Data.updateOptions(options);
    }
}


function ShareLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#ShareLinkClickStatistics_Years").val() + "&by=" + $("#ShareLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[0]}`, (res) => {
        CreateShareLinkClicksPerMonthsGraph(res);
    });
}

function HelpLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#HelpLinkClickStatistics_Years").val() + "&by=" + $("#HelpLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[1]}`, (res) => {
        CreateHelpLinkClicksPerMonthsGraph(res);
    });
}

function TipsAndGuidanceLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#TipsAndGuidanceLinkClickStatistics_Years").val() + "&by=" + $("#TipsAndGuidanceLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[2]}`, (res) => {
        CreateTipsAndGuidanceLinkClicksPerMonthsGraph(res);
    });
}

function AboutUsLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#AboutUsLinkClickStatistics_Years").val() + "&by=" + $("#AboutUsLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[3]}`, (res) => {
        CreateAboutUsLinkClicksPerMonthsGraph(res);
    });
}

function TermsAndConditionsLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#TermsAndConditionsLinkClickStatistics_Years").val() + "&by=" + $("#TermsAndConditionsLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[4]}`, (res) => {
        CreateTermsAndConditionsLinkClicksPerMonthsGraph(res);
    });
}

function PrivacyPolicyLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#PrivacyPolicyLinkClickStatistics_Years").val() + "&by=" + $("#PrivacyPolicyLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[5]}`, (res) => {
        CreatePrivacyPolicyLinkClicksPerMonthsGraph(res);
    });
}

function SkipTutorialLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#SkipTutorialLinkClickStatistics_Years").val() + "&by=" + $("#SkipTutorialLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[6]}`, (res) => {
        CreateSkipTutorialLinkClicksPerMonthsGraph(res);
    });
}

function SupportRequestLinkClickAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#SupportRequestLinkClickStatistics_Years").val() + "&by=" + $("#SupportRequestLinkClickStatistics_AgeAndGender").val()
        + `&linkType=${linlType[7]}`, (res) => {
        CreateSupportRequestLinkClicksPerMonthsGraph(res);
    });
}

function SortByInterestMatchAgeAndGenderStatistics() {
    $.get("/Admin/Home/LinkClickAgeAndGenderStatistics?year=" + $("#SortByInterestMatchStatistics_Years").val() + "&by=" + $("#SortByInterestMatchStatistics_AgeAndGender").val()
        + `&linkType=${linlType[8]}`, (res) => {
            CreateSortByInterestMatchPerMonthsGraph(res);
        });
}


$(document).ready(() => {

    ShareLinkClickAgeAndGenderStatistics();
    HelpLinkClickAgeAndGenderStatistics();
    TipsAndGuidanceLinkClickAgeAndGenderStatistics();
    AboutUsLinkClickAgeAndGenderStatistics();
    TermsAndConditionsLinkClickAgeAndGenderStatistics();
    PrivacyPolicyLinkClickAgeAndGenderStatistics();
    SkipTutorialLinkClickAgeAndGenderStatistics();
    SupportRequestLinkClickAgeAndGenderStatistics();
    SortByInterestMatchAgeAndGenderStatistics();

    $("#ShareLinkClickStatistics_AgeAndGender").change((e) => {
        ShareLinkClicksGraphchart_Data.destroy();
        ShareLinkClicksGraphchart_Data = "";
        $("#ShareLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#ShareLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#HelpLinkClickStatistics_AgeAndGender").change((e) => {
        HelpLinkClicksGraphchart_Data.destroy();
        HelpLinkClicksGraphchart_Data = "";
        $("#HelpLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#HelpLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#TipsAndGuidanceLinkClickStatistics_AgeAndGender").change((e) => {
        TipsAndGuidanceLinkClicksGraphchart_Data.destroy();
        TipsAndGuidanceLinkClicksGraphchart_Data = "";
        $("#TipsAndGuidanceLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#TipsAndGuidanceLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#AboutUsLinkClickStatistics_AgeAndGender").change((e) => {
        AboutUsLinkClicksGraphchart_Data.destroy();
        AboutUsLinkClicksGraphchart_Data = "";
        $("#AboutUsLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#AboutUsLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#TermsAndConditionsLinkClickStatistics_AgeAndGender").change((e) => {
        TermsAndConditionsLinkClicksGraphchart_Data.destroy();
        TermsAndConditionsLinkClicksGraphchart_Data = "";
        $("#TermsAndConditionsLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#TermsAndConditionsLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#PrivacyPolicyLinkClickStatistics_AgeAndGender").change((e) => {
        PrivacyPolicyLinkClicksGraphchart_Data.destroy();
        PrivacyPolicyLinkClicksGraphchart_Data = "";
        $("#PrivacyPolicyLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#PrivacyPolicyLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#SkipTutorialLinkClickStatistics_AgeAndGender").change((e) => {
        SkipTutorialLinkClicksGraphchart_Data.destroy();
        SkipTutorialLinkClicksGraphchart_Data = "";
        $("#SkipTutorialLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#SkipTutorialLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#SupportRequestLinkClickStatistics_AgeAndGender").change((e) => {
        SupportRequestLinkClicksGraphchart_Data.destroy();
        SupportRequestLinkClicksGraphchart_Data = "";
        $("#SupportRequestLinkClickStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#SupportRequestLinkClickStatisticsGrapghDataByAge").toggleClass("d-none");
    });

    $("#SortByInterestMatchStatistics_AgeAndGender").change((e) => {
        SortByInterestMatchGraphchart_Data.destroy();
        SortByInterestMatchGraphchart_Data = "";
        $("#SortByInterestMatchStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#SortByInterestMatchStatisticsGrapghDataByAge").toggleClass("d-none");
    });

})
