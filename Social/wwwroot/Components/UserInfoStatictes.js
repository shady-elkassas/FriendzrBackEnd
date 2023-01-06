"use strict";
// Class definition
var IsRtl = CurrentUserLanguage == 'ar-EG';

var primary = KTUtil.getCssVariableValue('--bs-primary');
var lightPrimary = KTUtil.getCssVariableValue('--bs-light-primary');
var info = KTUtil.getCssVariableValue('--bs-info');
var lightinfo = KTUtil.getCssVariableValue('--bs-light-info');
var success = KTUtil.getCssVariableValue('--bs-success');
var Danger = KTUtil.getCssVariableValue('--bs-danger');
var lightSuccess = KTUtil.getCssVariableValue('--bs-light-success');
var lightDanger = KTUtil.getCssVariableValue('--bs-light-danger');
var gray200 = KTUtil.getCssVariableValue('--bs-gray-200');
var gray500 = KTUtil.getCssVariableValue('--bs-gray-500');


var Custom_KTWidgets = function () {
    var initUseresEnableGhostModeRate = function (UsersInfo) {
        var element = document.getElementById("UseresEnableGhostModeRate");
        var height = parseInt(KTUtil.css(element, 'height'));
        if (!element) {
            return;
        }
        var options = {
            labels: ["Users with private mode enabled"],
            series: [UsersInfo.UseresEnableGhostModeRate_Rate],
            chart: {
                fontFamily: 'inherit',
                height: 300,
                type: 'radialBar',
                offsetY: 0
            },
            plotOptions: {
                radialBar: {
                    startAngle: -90,
                    endAngle: 90,

                    hollow: {
                        margin: 0,
                        size: "75%"
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: true,
                            fontSize: "10px",
                            fontWeight: "600",
                            offsetY: 15,
                            color: "#000000"
                        },
                        value: {
                            color: KTUtil.getCssVariableValue('--bs-gray-700'),
                            fontSize: "30px",
                            fontWeight: "600",
                            offsetY: -40,
                            show: true
                        }
                    },
                    track: {
                        background: KTUtil.getCssVariableValue('--bs-light-info'),
                        strokeWidth: '100%'
                    }
                }
            },
            colors: [KTUtil.getCssVariableValue('--bs-info')],
            stroke: {
                lineCap: "round",
            }
        };
        var chart = new ApexCharts(element, options);
        chart.render();
    }



    var initActiveUserRate = function (UsersInfo) {
        var element = document.getElementById("ActiveUserRate");
        var height = parseInt(KTUtil.css(element, 'height'));
        if (!element) {
            return;
        }
        var options = {
            labels: ["Active users"],
            series: [UsersInfo.ActiveUsers_Rate],
            chart: {
                fontFamily: 'inherit',
                height: 300,
                type: 'radialBar',
                offsetY: 0
            },
            plotOptions: {
                radialBar: {
                    startAngle: -90,
                    endAngle: 90,

                    hollow: {
                        margin: 0,
                        size: "75%"
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: true,
                            fontSize: "10px",
                            fontWeight: "600",
                            offsetY: 15,
                            color: "#000000"
                        },
                        value: {
                            color: KTUtil.getCssVariableValue('--bs-gray-700'),
                            fontSize: "30px",
                            fontWeight: "600",
                            offsetY: -40,
                            show: true
                        }
                    },
                    track: {
                        background: KTUtil.getCssVariableValue('--bs-light-success'),
                        strokeWidth: '100%'
                    }
                }
            },
            colors: [KTUtil.getCssVariableValue('--bs-success')],
            stroke: {
                lineCap: "round",
            }
        };

        var chart = new ApexCharts(element, options);
        chart.render();
    }

  
    var initChartsWidget1 = function (UsersInfo) {
        var element = document.getElementById("kt_charts_widget_1_chart");

        var height = parseInt(KTUtil.css(element, 'height'));
        var labelColor = KTUtil.getCssVariableValue('--bs-gray-500');
        var borderColor = KTUtil.getCssVariableValue('--bs-gray-200');
        var baseColor = KTUtil.getCssVariableValue('--bs-primary');
        var secondaryColor = KTUtil.getCssVariableValue('--bs-gray-300');

        if (!element) {
            return;
        }

        var options = {
            series: [{
                name: 'Net Profit',
                data: [44, 55, 57, 56, 61, 58, 5, 57, 56, 61, 58, 5]
            }, {
                name: 'Revenue',
                data: [76, 85, 101, 98, 87, 105, 10, 101, 98, 87, 105, 10]
            }],
            chart: {
                fontFamily: 'inherit',
                type: 'bar',
                height: height,
                toolbar: {
                    show: false
                }
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: ['30%'],
                    borderRadius: 4
                },
            },
            legend: {
                show: false
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
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                axisBorder: {
                    show: false,
                },
                axisTicks: {
                    show: false
                },
                labels: {
                    style: {
                        colors: labelColor,
                        fontSize: '12px'
                    }
                }
            },
            yaxis: {
                labels: {
                    style: {
                        colors: labelColor,
                        fontSize: '12px'
                    }
                }
            },
            fill: {
                opacity: 1
            },
            states: {
                normal: {
                    filter: {
                        type: 'none',
                        value: 0
                    }
                },
                hover: {
                    filter: {
                        type: 'none',
                        value: 0
                    }
                },
                active: {
                    allowMultipleDataPointsSelection: false,
                    filter: {
                        type: 'none',
                        value: 0
                    }
                }
            },
            tooltip: {
                style: {
                    fontSize: '12px'
                },
                y: {
                    formatter: function (val) {
                        return "$" + val + " thousands"
                    }
                }
            },
            colors: [baseColor, secondaryColor],
            grid: {
                borderColor: borderColor,
                strokeDashArray: 4,
                yaxis: {
                    lines: {
                        show: true
                    }
                }
            }
        };

        var chart = new ApexCharts(element, options);
        chart.render();
    }
    // Public methods
    return {
        init: function (UsersInfo) {
            initUseresEnableGhostModeRate(UsersInfo);
            initActiveUserRate(UsersInfo);
            initChartsWidget1(UsersInfo);
        }
    }
}();

function CreateStatictesAsRate(dataObj) {
    //ElmentID, Series, lables, dataLabelsName_Color, dataLabelsValue_Color, trackBackGroundColor, colorsArray
    var element = document.getElementById(dataObj.ElmentID);
    var height = parseInt(KTUtil.css(element, 'height'));
    if (!element) {
        return;
    }
    var options = {
        labels: dataObj.lables,
        series: dataObj.Series,
        chart: {
            fontFamily: 'inherit',
            height: 300,
            type: 'radialBar',
            offsetY: 0
        },
        plotOptions: {
            radialBar: {
                startAngle: -90,
                endAngle: 90,
                hollow: {
                    margin: 0,
                    size: "75%"
                },
                dataLabels: {
                    showOn: "always",
                    name: {
                        show: true,
                        fontSize: "10px",
                        fontWeight: "600",
                        offsetY: 15,
                        color: dataObj.dataLabelsName_Color
                    },
                    value: {
                        color: dataObj.dataLabelsValue_Color,
                        fontSize: "30px",
                        fontWeight: "600",
                        offsetY: -40,
                        show: true
                    }
                },
                track: {
                    background: dataObj.trackBackGroundColor,
                    strokeWidth: '100%'
                }
            }
        },
        colors: dataObj.colorsArray,
        stroke: {
            lineCap: "round",
        }
    };
    let chart = new ApexCharts(element, options);
    chart.render();
    return chart;
}


var KTProjectOverview = function () {
    // Colors
    var initChart = function (UsersInfo) {
        // init chart
        var element = document.getElementById("project_overview_chart");

        if (!element) {
            return;
        }

        console.log("1 ###", initChart);

        console.log("2 ###", UsersInfo);

        console.log("ConfirmedMailUsers_Count ###", UsersInfo.ConfirmedMailUsers_Count);

        console.log("Not ConfirmedMailUsers_Count ###", UsersInfo.UnConfirmedMailUsers_Count);

        var config = {
            type: 'doughnut',
            data: {
                datasets: [{
                    data: [(UsersInfo.ConfirmedMailUsers_Count), (UsersInfo.UnConfirmedMailUsers_Count)],
                    backgroundColor: ['#009EF7', '#E4E6EF']
                }],
                labels: ['Mail Confirmed', 'Mail NotConfirmed']
            },
            options: {
                chart: {
                    fontFamily: 'inherit'
                },
                cutoutPercentage: 75,
                responsive: true,
                maintainAspectRatio: false,
                cutout: '75%',
                title: {
                    display: false
                },
                animation: {
                    animateScale: true,
                    animateRotate: true
                },
                tooltips: {
                    enabled: true,
                    intersect: false,
                    mode: 'nearest',
                    bodySpacing: 5,
                    yPadding: 10,
                    xPadding: 10,
                    caretPadding: 0,
                    displayColors: false,
                    backgroundColor: '#20D489',
                    titleFontColor: '#ffffff',
                    cornerRadius: 4,
                    footerSpacing: 0,
                    titleSpacing: 0
                },

                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        };

        var ctx = element.getContext('2d');
        var myDoughnut = new Chart(ctx, config);
    }


    // Public methods
    return {
        init: function (UsersInfo) {

            initChart(UsersInfo);
            //initGraphfilteringAccordingToAge

        }
    }
}();
var allUserStatictesInfo = function () {
    // Colors
    var initallUserStatictesInfo = function (UsersInfo) {
        let Series = [
            { lable: 'registered users', serie: UsersInfo.TotalUsers_Count, color: '#35F434' },
            { lable: 'deleted accounts', serie: UsersInfo.DeletedUsers_Count, color: '#F74634' },
            //ActiveUsers_Count
            { lable: 'Active users', serie: UsersInfo.ActiveUsers_Count, color: '#18B683' },
            { lable: 'Inactive users', serie: UsersInfo.NotactiveUsers_Count, color: '#0039FF' },
            { lable: 'verified emails', serie: UsersInfo.ConfirmedMailUsers_Count, color: '#00B3FF' },
            { lable: 'unverified emails', serie: UsersInfo.UnConfirmedMailUsers_Count, color: '#5F6DDA' },
            { lable: 'Users still active after 3 months ', serie: UsersInfo.UseresStillUseAppAfter3Months_Count, color: '#E98C78' },
            //{ lable: 'activates ghost mode', serie: UsersInfo.UseresEnableGhostMode_Count, color: '#4C4B4A' },
            { lable: 'Users with private mode enabled', serie: UsersInfo.UseresEnableGhostMode_Count, color: '#4C4B4A' },
            { lable: 'Users who have disabled push notifications', serie: UsersInfo.deactivatespushnotifications_Count, color: '#F98502' },
            { lable: 'Average age of users', serie: UsersInfo.UseresAverageAge, color: '#34E4F4' },
            { lable: 'Number of friend requests', serie: UsersInfo.RequestesCount, color: '#F0ED35' },
            { lable: 'number of blocks', serie: UsersInfo.BlockRequestesCount, color: '#18B683' },
            { lable: 'completed  profile data', serie: UsersInfo.Updated, color: '#181c32' },
            { lable: 'Not completed  profile data', serie: UsersInfo.NeedUpdate, color: '#8f6b32' },
            { lable: 'Users with personal space enabled', serie: UsersInfo.personalspace, color: '#ffff00' },
            //{ lable: 'users who activate personal space', serie: UsersInfo.personalspace, color: '#fff8dd' },
            //{ lable: 'Users with private mode enabled', serie: UsersInfo.NeedUpdate, color: '#ff6b32' },
        ]
        var options = {
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
            //title: {
            //    text: 'User Statistics',
            //    fontSize: '1.275rem',
            //    fontWeight: '500',
            //},
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
        let elmentObj = document.querySelector("#allUserStatictes");
        if (elmentObj != null) {

            var chart = new ApexCharts(elmentObj, options);
        chart.render();
        }

    }
    return {
        init: function (UsersInfo) {
            initallUserStatictesInfo(UsersInfo);
        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    $.get("/admin/home/GetInfoAboutUsers", res => {
        Custom_KTWidgets.init(res.UsersInfo);
        KTProjectOverview.init(res.UsersInfo);
        //allUserStatictesInfo.init(res.UsersInfo); ////Percentage (Rate)
        allUserStatictesInfo.init(res.UsersInfo);
        console.log()
        //Age Staticts
        CreateStatictesAsRate({
            ElmentID: "UserWithLessThan18Age_Rate",
            Series: [res.UsersInfo.UserWithLessThan18Age_Rate],
            lables: [`Age Less Than 18 is :${res.UsersInfo.UserWithLessThan18Age_Count} User `],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#F5DAD3",
            colorsArray: ["#E15031"],
        })
        CreateStatictesAsRate({
            ElmentID: "UsersWith18_24Age_Rate",
            Series: [res.UsersInfo.UsersWith18_24Age_Rate],
            lables: [`Age 18-24 is: ${res.UsersInfo.UsersWith18_24Age_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#D6F3CA",
            colorsArray: ["#2A9000"],
        })
        CreateStatictesAsRate({
            ElmentID: "UsersWith25_34Age_Rate",
            Series: [res.UsersInfo.UsersWith25_34Age_Rate],
            lables: [`Age 25-34 is: ${res.UsersInfo.UsersWith25_34Age_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#AFE4F3",
            colorsArray: ["#00C5FC"],
        })
        CreateStatictesAsRate({
            ElmentID: "UsersWith35_54Age_Rate",
            Series: [res.UsersInfo.UsersWith35_54Age_Rate],
            lables: [`Age 35-54 is: ${res.UsersInfo.UsersWith35_54Age_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#E8E9E7",
            colorsArray: ["#F0FB00"],
        })
        CreateStatictesAsRate({
            ElmentID: "UsersWithMoreThan55Age_Rate",
            Series: [res.UsersInfo.UsersWithMoreThan55Age_Rate],
            lables: [`Age More Than 55 is: ${res.UsersInfo.UsersWithMoreThan55Age_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#737BBA",
            colorsArray: ["#001EFF"],
        })



        //PRivte Mode Staticts
        CreateStatictesAsRate({
            ElmentID: "AppearenceEveryOneInGhostMode_Rate",
            Series: [res.UsersInfo.AppearenceEveryOneInGhostMode_Rate],
            lables: [`Every One :${res.UsersInfo.AppearenceEveryOneInGhostMode_Count}  `],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#F5DAD3",
            colorsArray: ["#E15031"],
        })
        CreateStatictesAsRate({
            ElmentID: "AppearenceOtherGenderInGhostMode_Rate",
            Series: [res.UsersInfo.AppearenceOtherGenderInGhostMode_Rate],
            lables: [`Other Gender: ${res.UsersInfo.AppearenceOtherGenderInGhostMode_Count} `],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#D6F3CA",
            colorsArray: ["#2A9000"],
        })
        CreateStatictesAsRate({
            ElmentID: "AppearenceFemaleInGhostMode_Rate",
            Series: [res.UsersInfo.AppearenceFemaleInGhostMode_Rate],
            lables: [`Female: ${res.UsersInfo.AppearenceFemaleInGhostMode_Count} `],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#AFE4F3",
            colorsArray: ["#00C5FC"],
        })

        CreateStatictesAsRate({
            ElmentID: "AppearenceMaleInGhostMode_Rate",
            Series: [res.UsersInfo.AppearenceMaleInGhostMode_Rate],
            lables: [`Male : ${res.UsersInfo.AppearenceMaleInGhostMode_Count} `],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#E8E9E7",
            colorsArray: ["#F0FB00"],
        })


        // Users Statistics
        CreateStatictesAsRate({
            ElmentID: "MostAgeFiltirngRangeUsed",
            Series: [res.UsersInfo.MostAgeFiltirngRangeUsed_Rate],
            lables: [`Max Age Filtring  ${res.UsersInfo.MostAgeFiltirngRangeUsed} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#E8E9E7",
            colorsArray: ["#F0FB00"],
        })

        CreateStatictesAsRate({
            ElmentID: "UsersWithPersonalSpaceEnabled",
            Series: [res.UsersInfo.UsersWithPersonalSpaceEnabled_Rate],
            lables: [`Users With Personal Space Enabled From ${res.UsersInfo.Updated} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#E8E9E7",
            colorsArray: ["#66553b"],
        })

        CreateStatictesAsRate({
            ElmentID: "PushNotificationsEnabled",
            Series: [res.UsersInfo.UsersWithPushNotificationsEnabled_Rate],
            lables: [`Push Notifications Enabled From ${res.UsersInfo.Updated} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#E8E9E7",
            colorsArray: ["#f31768"],
        })

        CreateStatictesAsRate({
            ElmentID: "DeletedProfiles",
            Series: [res.UsersInfo.DeletedProfiles_Rate],
            lables: [`Deleted profiles :${res.UsersInfo.DeletedProfiles_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#E8E9E7",
            colorsArray: ["#5b5b5b"],
        })

        CreateStatictesAsRate({
            ElmentID: "VerifiedUsersRate",
            Series: [res.UsersInfo.UsersVertified_Rate],
            lables: [`Verified Users ${res.UsersInfo.ConfirmedMailUsers_Count} From ${res.UsersInfo.CurrenUsers_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#d4e6f4",
            colorsArray: ["#3e92d1"],
        })



        CreateStatictesAsRate({
            ElmentID: "UnVerifiedUsersRate",
            Series: [res.UsersInfo.UsersUnVertified_Rate],
            lables: [`UnVerified Users ${res.UsersInfo.UnConfirmedMailUsers_Count} From ${res.UsersInfo.CurrenUsers_Count} User`],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#eedbe4",
            colorsArray: ["#7f6371"],
        })
 
    })
});