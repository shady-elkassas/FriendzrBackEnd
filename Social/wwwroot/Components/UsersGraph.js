var UserGraphchart = "";
var UserGraphchart_allData = "";
var EventGraphchart_allData = "";
var EventInEachCategoryGraphchart = "";
var FilterInEachCategoryGrapghChart = "";
var UsersInEachInterestGraphchart = "";
var IsRtl = CurrentUserLanguage == 'ar-EG';

function CreateUserMonthsGraph2(alldata) {
    let element = document.getElementById("Kt_Users_Grapgh_allData");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }
    let ListOfcolors = ['#EE0000', '#FFAB00', '#FFEF00', '#808080', '#66FFB2', '#33FFFF', '#B266FF', '#006633'];
    let Serieslist = alldata.series;
    let Monthes = alldata.monthes.map(x => x.month);
    var options = {
        colors: IsRtl ? ListOfcolors.reverse() : ListOfcolors,
        yaxis: {
            //title: {
            //    text: ' (User Count)'
            //},
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
                //colors: ['#1A73E8', '#B32824', '#B32824', '#B32824', '#B32824', '#B32824', '#B32824', '#B32824'],
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
    if (UserGraphchart_allData == "") {

        UserGraphchart_allData = new ApexCharts(element, options);
        UserGraphchart_allData.render();
    }
    else {
        UserGraphchart_allData.updateOptions(options);
    }
}

function CreateEventMonthsGraph(alldata) {
    let element = document.getElementById("Kt_Events_Grapgh_allData");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }
    let ListOfcolors = ['#EE0000', '#FFAB00', '#FFEF00', '#808080', '#66FFB2', '#33FFFF'];
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
                //colors: ['#1A73E8', '#B32824', '#B32824', '#B32824', '#B32824', '#B32824', '#B32824', '#B32824'],
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
                    return val + getToken("Event")
                }
            }
        }
    };
    if (EventGraphchart_allData == "") {

        EventGraphchart_allData = new ApexCharts(element, options);
        EventGraphchart_allData.render();
    }
    else {
        EventGraphchart_allData.updateOptions(options);
    }
}

function CreateEventsInEachCategoryGraph(alldata) {
    let element = document.getElementById("Kt_EventsInEachCategory_Grapgh");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }

    let Serieslist = IsRtl ? alldata.series.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : alldata.series;
    let Categories = IsRtl ? alldata.categories.reverse() : alldata.categories;
    let ListOfcolors = ['#69d297'];

    var options = {
        color: ListOfcolors,
        series: Serieslist,
        chart: {
            type: 'bar',
            height: 350
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
            categories: Categories,
            rotate: 0,
            tickPlacement: 'on',

            labels: {
                rotate: -45,
            },
            //tickPlacement: 'inside',
            //labelPlacement: "inside",
            //tickPlacement: "inside", //Change it to "outside"
            //gridColor: "#ddd"

        },
        yaxis: {
            
            opposite: IsRtl
        

        },
        //fill: {
        //    type: 'gradient',
        //    gradient: {
        //        shade: 'light',
        //        type: "horizontal",
        //        shadeIntensity: 0.25,
        //        gradientToColors: undefined,
        //        inverseColors: true,
        //        opacityFrom: 0.85,
        //        opacityTo: 0.85,
        //        stops: [50, 0, 100]
        //    },
        //},
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
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + "  " + getToken("Event")
                }
            }
        }
    };

    if (EventInEachCategoryGraphchart == "") {

        EventInEachCategoryGraphchart = new ApexCharts(element, options);
        EventInEachCategoryGraphchart.render();
    }
    else {
        EventInEachCategoryGraphchart.updateOptions(options);
    }
}

function FilterInEachCategoryGrapgh(alldata) {
    let element = document.getElementById("FilterInEachCategory_Grapgh");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }

    let Serieslist = IsRtl ? alldata.series.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : alldata.series;
    let Categories = IsRtl ? alldata.categories.reverse() : alldata.categories;
    let ListOfcolors = ['#C5C5C5'];

    var options = {
        color: ListOfcolors,
        series: Serieslist,
        chart: {
            type: 'bar',
            height: 350
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
            categories: Categories,
            rotate: 0,
            tickPlacement: 'on',

            labels: {
                rotate: -45,
            },

        },
        yaxis: {

            opposite: IsRtl


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
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + "  " + getToken("User")
                }
            }
        }
    };

    if (FilterInEachCategoryGrapghChart == "") {

        FilterInEachCategoryGrapghChart = new ApexCharts(element, options);
        FilterInEachCategoryGrapghChart.render();
    }
    else {
        FilterInEachCategoryGrapghChart.updateOptions(options);
    }
}

function UsersInEachInterestGraph(alldata) {
    let element = document.getElementById("UsersInEachInterestGrapgh");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }

    let Serieslist = IsRtl ? alldata.series.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : alldata.series;
    let Interests = IsRtl ? alldata.interests.reverse() : alldata.interests;
    let ListOfcolors = ['#C5C5C5'];

    var options = {
        color: ListOfcolors,
        series: Serieslist,
        chart: {
            type: 'bar',
            height: 350
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
            categories: Interests,
            rotate: 0,
            tickPlacement: 'on',

            labels: {
                rotate: -45,
            },
           
        },
        yaxis: {

            opposite: IsRtl


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
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + "  " + getToken("User")
                }
            }
        }
    };

    if (UsersInEachInterestGraphchart == "") {

        UsersInEachInterestGraphchart = new ApexCharts(element, options);
        UsersInEachInterestGraphchart.render();
    }
    else {
        UsersInEachInterestGraphchart.updateOptions(options);
    }
}

function UpdateUsersYearGraphStatictis() {
    $.get("/Admin/Home/allUserStatictesPerMonth?Year=" + $("#Kt_Users_Grapgh_allData_Years").val(), (res) => {

        CreateUserMonthsGraph2(res);

    });
}

function UpdateEventsYearGraphStatictis() {
    $.get("/Admin/Home/EventStatictesPerMonth?Year=" + $("#Kt_Events_Grapgh_allData_Years").val(), (res) => {

        CreateEventMonthsGraph(res);

    });
}

function UpdateEventsInEachCategoryGraph() {
    $.get("/Admin/Home/EventsInEachCategory?Year=" + $("[name='Years']").val(), (res) => {

        CreateEventsInEachCategoryGraph(res);

    });
}

function UpdateFilterInEachCategoryGrapgh() {
    $.get("/Admin/Home/FilterByEventCategory", (res) => {

        FilterInEachCategoryGrapgh(res);
    });
}

function UpdateUsersInEachInterestGrapgh() {
    $.get("/Admin/Home/UsersInEachInterest", (res) => {

        UsersInEachInterestGraph(res);

    });
}

$(document).ready(() => {
    UpdateUsersYearGraphStatictis();
    UpdateEventsInEachCategoryGraph();
    UpdateFilterInEachCategoryGrapgh();
    UpdateUsersInEachInterestGrapgh();
    UpdateEventsYearGraphStatictis();
})
