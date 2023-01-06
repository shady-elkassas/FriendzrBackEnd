var AppGraphchart = "";
var AppGraphchart_allData = "";
var IsRtl = CurrentUserLanguage == 'ar-EG';

function CreateAppMonthsGraph(alldata) {
    let element = document.getElementById("Kt_App_Grapgh_allData");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }
    let ListOfcolors = ['#EE0000', '#FFAB00'/*, '#FFEF00', '#808080', '#66FFB2', '#33FFFF', '#B266FF', '#006633'*/];
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
                    return val 
                }
            }
        }
    };
    if (AppGraphchart_allData == "") {

        AppGraphchart_allData = new ApexCharts(element, options);
        AppGraphchart_allData.render();
    }
    else {
        AppGraphchart_allData.updateOptions(options);
    }
}
function UpdateAppYearGraphStatictis() {
    $.get("/Admin/Home/AppStatictesPerMonth?Year=" + $("#Kt_App_Grapgh_allData_Years").val(), (res) => {
        CreateAppMonthsGraph(res);

    });
}


$(document).ready(() => {
    UpdateAppYearGraphStatictis();
})
