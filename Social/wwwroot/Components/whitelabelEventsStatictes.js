var IsRtl = CurrentUserLanguage == 'ar-EG';

var allEventsStatictesInfo = function () {
    // Colors
    var initallEventStatictesInfo = function (data) {
        let Series = [
            { lable: 'Total number of events created', serie: data.allEventsCount, color: '#35F434' },
            { lable: 'Number of existing events', serie: data.NumberOfExisteEvent, color: '#ebc034' },
            { lable: 'Number of finished events', serie: data.NumberOfFinishedEvent, color: '#282120' },
            { lable: 'Number of Private events', serie: data.NumberOfPrivateEvent, color: '#585759' },
            //{ lable: 'Number of Friendzr events', serie: data.NumberOfFriendzrEvent, color: '#24d459' },
            //{ lable: 'Number of External events', serie: data.NumberOfExternalEvent, color: '#eb6e34' },
            //{ lable: 'Number of Admin External events', serie: data.NumberOfadminExternalEvent, color: '#b6f2cd' },
            //{ lable: 'Number of WhiteLable events', serie: data.NumberOfWhiteLableEvent, color: '#afe8ed' },            
            //{ lable: 'number of deleted events', serie: data.NumberOfDeletedEvents, color: '#FF004D' },
            //{ lable: 'users who have created an event', serie: data.NumberOfUseresHowCreatedEvents, color: '#4C4B4A' },
            
            { lable: 'Percentage of events participants', serie: data.averageOfParticibated, color: '#28FCEB' },
            { lable: 'Percentage of existe events participants', serie: data.averageOfParticibatedInExisteEvent, color: '#f5a4eb' },

            //{ lable: 'created events since 3 months', serie: data.eventsafter3months, color: '#815EE5' },
            //{ lable: 'low capacity events', serie: data.LOWcapacity, color: '#bf360c' },
            //{ lable: 'medium capacity events', serie: data.mediumcapacity, color: '#a1887f' },
            //{ lable: 'high capacity events', serie: data.highcapacity, color: '#89bf04' },
            //{ lable: 'number of shared events', serie: data.sharedevent, color: '#35F434' },

            { lable: 'Total events attendance', serie: data.attendes, color: '#1fa393' },

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
            yaxis: {
                "labels": {
                    "formatter": function (val) {
                        return val.toFixed(1)
                    }
                }
            } ,
            legend: {
                show: true,
                position: IsRtl ? 'right' : 'left',
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
        let chart = new ApexCharts(document.querySelector("#whitelabelEventsStatictes"), options);
        chart?.render();

    }
    return {
        init: function (UsersInfo) {
            initallEventStatictesInfo(UsersInfo);
        }
    }
}();


KTUtil.onDOMContentLoaded(function () {
    $.get("/Whitelabel/home/GetInfoAboutEvents", res => {

        allEventsStatictesInfo.init(res.EventsInfo);
    })
});