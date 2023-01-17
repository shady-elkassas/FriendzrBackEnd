var UsersInEachInterestGraphchart = "";
var IsRtl = CurrentUserLanguage == 'ar-EG';

function UsersInEachInterestGraph(alldata) {
    let element = document.getElementById("UsersInEachInterestGrapgh");
    let height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }

    let Serieslist = IsRtl ? alldata.series?.map((x) => { return { name: x.name, data: x.data.reverse() }; }) : alldata.series;
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
function UpdateUsersInEachInterestGrapgh(Id) {
    $.get(`/Whitelabel/Home/UsersInEachInterest?Id=${Id}`, (res) => {

        UsersInEachInterestGraph(res);

    });
}

$(document).ready(() => {
    debugger;
   // UpdateUsersInEachInterestGrapgh();   
})