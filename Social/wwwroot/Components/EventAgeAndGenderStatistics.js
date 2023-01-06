
var EventsShareGraphchart_Data = "";
var EventsViewGraphchart_Data = "";
var EventsAttendGraphchart_Data = "";


function CreateEventsSharePerMonthsGraph(alldata) {
    let isByGender = $("#EventsShareStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "EventsShareStatisticsGrapghDataByGender" : "EventsShareStatisticsGrapghDataByAge");

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
                    return val + getToken("Event")
                }
            }
        }
    };
    if (EventsShareGraphchart_Data == "") {

        EventsShareGraphchart_Data = new ApexCharts(element, options);
        EventsShareGraphchart_Data.render();
    }
    else {
        EventsShareGraphchart_Data.updateOptions(options);
    }
}

function CreateEventsViewPerMonthsGraph(alldata) {
    let isByGender = $("#EventsViewStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "EventsViewStatisticsGrapghDataByGender" : "EventsViewStatisticsGrapghDataByAge");

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
                    return val + getToken("Event")
                }
            }
        }
    };
    if (EventsViewGraphchart_Data == "") {

        EventsViewGraphchart_Data = new ApexCharts(element, options);
        EventsViewGraphchart_Data.render();
    }
    else {
        EventsViewGraphchart_Data.updateOptions(options);
    }
}

function CreateEventsAttendPerMonthsGraph(alldata) {
    let isByGender = $("#EventsAttendStatistics_AgeAndGender").val() == "SeeByGender" ? true : false;

    let element = document.getElementById(isByGender ? "EventsAttendStatisticsGrapghDataByGender" : "EventsAttendStatisticsGrapghDataByAge");

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
                    return val + getToken("Event")
                }
            }
        }
    };
    if (EventsAttendGraphchart_Data == "") {

        EventsAttendGraphchart_Data = new ApexCharts(element, options);
        EventsAttendGraphchart_Data.render();
    }
    else {
        EventsAttendGraphchart_Data.updateOptions(options);
    }
}


function EventsShareAgeAndGenderStatistics() {
    $.get("/Admin/Home/NumberOfSharedEventStatistics?year=" + $("#EventsShareStatistics_Years").val() + "&by=" + $("#EventsShareStatistics_AgeAndGender").val()
        , (res) => {
            CreateEventsSharePerMonthsGraph(res);
        });
}

function EventsViewAgeAndGenderStatistics() {
    $.get("/Admin/Home/NumberOfViewedEventStatistics?year=" + $("#EventsViewStatistics_Years").val() + "&by=" + $("#EventsViewStatistics_AgeAndGender").val()
        , (res) => {
            CreateEventsViewPerMonthsGraph(res);
        });
}

function EventsAttendAgeAndGenderStatistics() {
    $.get("/Admin/Home/NumberOfAttendedEventStatistics?year=" + $("#EventsAttendStatistics_Years").val() + "&by=" + $("#EventsAttendStatistics_AgeAndGender").val()
        , (res) => {
            CreateEventsAttendPerMonthsGraph(res);
        });
}


$(document).ready(() => {

    EventsShareAgeAndGenderStatistics();
    EventsViewAgeAndGenderStatistics();
    EventsAttendAgeAndGenderStatistics();

    $("#EventsShareStatistics_AgeAndGender").change((e) => {
        EventsShareGraphchart_Data.destroy();
        EventsShareGraphchart_Data = "";
        $("#EventsShareStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#EventsShareStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#EventsViewStatistics_AgeAndGender").change((e) => {
        EventsViewGraphchart_Data.destroy();
        EventsViewGraphchart_Data = "";
        $("#EventsViewStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#EventsViewStatisticsGrapghDataByAge").toggleClass("d-none");
    });


    $("#EventsAttendStatistics_AgeAndGender").change((e) => {
        EventsAttendGraphchart_Data.destroy();
        EventsAttendGraphchart_Data = "";
        $("#EventsAttendStatisticsGrapghDataByGender").toggleClass("d-none");
        $("#EventsAttendStatisticsGrapghDataByAge").toggleClass("d-none");
    });

})
