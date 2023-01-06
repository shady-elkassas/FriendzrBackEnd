var IsRtl = CurrentUserLanguage == 'ar-EG';

var allEventsStatictesInfo = function () {
    // Colors
    var initallEventStatictesInfo = function (data) {
        let Series = [
            { lable: 'total events created', serie: data.allEventsCount, color: '#35F434' },
            { lable: 'number of deleted events', serie: data.NumberOfDeletedEvents, color: '#FF004D' },
            { lable: 'users who create events', serie: data.NumberOfUseresHowCreatedEvents, color: '#4C4B4A' },
            { lable: 'number of events participants', serie: data.averageOfParticibated, color: '#28FCEB' },
            { lable: 'created events after 3 months', serie: data.eventsafter3months, color: '#815EE5' },
            { lable: 'low capacity events', serie: data.LOWcapacity, color: '#bf360c' },
            { lable: 'medium capacity events', serie: data.mediumcapacity, color: '#a1887f' },
            { lable: 'high capacity events', serie: data.highcapacity, color: '#89bf04' },

   { lable: 'number of shared events', serie: data.sharedevent, color: '#35F434' },

{ lable: 'number of Private events', serie: data.Private, color: '#1523bb' },


        ]
        let options = {
            colors: Series.map(x => x.color),
            series: Series.map(x => x.serie),
            labels: Series.map(x => x.lable + `: <span class="mySpan text-dark fw-bolder text-hover-primary fs-7">${x.serie}</span>`),
            theme: {
                monochrome: {
                    enabled: false
                }
            },
            chart: {
                height: 300,
                type: 'polarArea',
               
            },
            stroke: {
                colors: ['#fff']
            },
            fill: {
                opacity: 0.8
            },

            legend: {
                show: true,
                position: IsRtl ?'right':'left',
                containerMargin: {
                    right: 0
                }
            },
            responsive: [{
                breakpoint: 480,
                options: {
                    chart: {
                        width: 150
                    },
                    legend: {
                        position: 'bottom'
                    }
                }
            }]
        };
        let chart = new ApexCharts(document.querySelector("#allEventsStatictes"), options);
        chart.render();

    }
    return {
        init: function (UsersInfo) {
            initallEventStatictesInfo(UsersInfo);
        }
    }
}();


KTUtil.onDOMContentLoaded(function () {
    $.get("/admin/home/GetInfoAboutEvents", res => {

        allEventsStatictesInfo.init(res.EventsInfo);
    })
});