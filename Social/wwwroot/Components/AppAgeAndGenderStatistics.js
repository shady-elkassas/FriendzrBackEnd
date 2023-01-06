let canvasDataForNumberOfConnectionRequestsAccepted = {};
let canvasDataForPercentageOfUsersWhoCreatedEvents = {};
let canvasDataForNumberOfConnectionRequestsSent = {};
let canvasDataForUsersWhoEnabledPersonalSpace = {};
let canvasDataForFinishedRegistration = {};
let canvasDataForBlockedUsers = {};
let canvasDataForActiveUsers = {};


function PercentageOfUsersWhoCreatedEventsStatictis() {

    let result;
    $.get("/Admin/Home/PercentageOfUsersWhoCreatedEventsStatictes", (res) => {
        result = res;
        $('#TotalByGender_UCE').text(result.all);
        $('#TotalByAge_UCE').text(result.all);
        $('#Male_UCE').text(result.male);
        $('#Female_UCE').text(result.female);
        $('#Other_UCE').text(result.other);
        $('#From18To25_UCE').text(result.from18To25);
        $('#From25To34_UCE').text(result.from25To34);
        $('#From35To44_UCE').text(result.from35To44);
        $('#From45To54_UCE').text(result.from45To54);
        $('#From55To64_UCE').text(result.from55To64);
        $('#From65AndMore_UCE').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("UCE_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "PercentageOfUsersWhoCreatedEventsStatictesByAge_chart" : "PercentageOfUsersWhoCreatedEventsStatictesByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForPercentageOfUsersWhoCreatedEvents = new Chart(ctx, config);
    });


}

function NumberOfConnectionRequestsAcceptedStatictis() {

    let result;
    $.get("/Admin/Home/NumberOfConnectionRequestsAcceptedStatictes", (res) => {
        result = res;
        $('#TotalByGender_NRSA').text(result.all);
        $('#TotalByAge_NRSA').text(result.all);
        $('#Male_NRSA').text(result.male);
        $('#Female_NRSA').text(result.female);
        $('#Other_NRSA').text(result.other);
        $('#From18To25_NRSA').text(result.from18To25);
        $('#From25To34_NRSA').text(result.from25To34);
        $('#From35To44_NRSA').text(result.from35To44);
        $('#From45To54_NRSA').text(result.from45To54);
        $('#From55To64_NRSA').text(result.from55To64);
        $('#From65AndMore_NRSA').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("NRSA_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "NumberOfConnectionRequestsAcceptedStatictisByAge_chart" : "NumberOfConnectionRequestsAcceptedStatictisByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForNumberOfConnectionRequestsAccepted = new Chart(ctx, config);
    });


}

function NumberOfConnectionRequestsSentStatictis() {

    let result;
    $.get("/Admin/Home/NumberOfConnectionRequestsSentStatictes", (res) => {
        result = res;
        $('#TotalByGender_NRS').text(result.all);
        $('#TotalByAge_NRS').text(result.all);
        $('#Male_NRS').text(result.male);
        $('#Female_NRS').text(result.female);
        $('#Other_NRS').text(result.other);
        $('#From18To25_NRS').text(result.from18To25);
        $('#From25To34_NRS').text(result.from25To34);
        $('#From35To44_NRS').text(result.from35To44);
        $('#From45To54_NRS').text(result.from45To54);
        $('#From55To64_NRS').text(result.from55To64);
        $('#From65AndMore_NRS').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("NRS_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "NumberOfConnectionRequestsSentStatictisByAge_chart" : "NumberOfConnectionRequestsSentStatictisByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForNumberOfConnectionRequestsSent = new Chart(ctx, config);
    });


}

function UsersWhoEnabledPersonalSpaceStatictis() {

    let result;
    $.get("/Admin/Home/UsersWhoEnabledPersonalSpaceStatictes", (res) => {
        result = res;
        $('#TotalByGender_UEPS').text(result.all);
        $('#TotalByAge_UEPS').text(result.all);
        $('#Male_UEPS').text(result.male);
        $('#Female_UEPS').text(result.female);
        $('#Other_UEPS').text(result.other);
        $('#From18To25_UEPS').text(result.from18To25);
        $('#From25To34_UEPS').text(result.from25To34);
        $('#From35To44_UEPS').text(result.from35To44);
        $('#From45To54_UEPS').text(result.from45To54);
        $('#From55To64_UEPS').text(result.from55To64);
        $('#From65AndMore_UEPS').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("UEPS_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "UsersWhoEnabledPersonalSpaceStatictisByAge_chart" : "UsersWhoEnabledPersonalSpaceStatictisByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForUsersWhoEnabledPersonalSpace = new Chart(ctx, config);
    });


}

function FinishedRegistrationStatictis() {

    let result;
    $.get("/Admin/Home/FinishedRegistrationUserStatictes", (res) => {
        result = res;
        $('#TotalByGender_FRU').text(result.all);
        $('#TotalByAge_FRU').text(result.all);
        $('#Male_FRU').text(result.male);
        $('#Female_FRU').text(result.female);
        $('#Other_FRU').text(result.other);
        $('#From18To25_FRU').text(result.from18To25);
        $('#From25To34_FRU').text(result.from25To34);
        $('#From35To44_FRU').text(result.from35To44);
        $('#From45To54_FRU').text(result.from45To54);
        $('#From55To64_FRU').text(result.from55To64);
        $('#From65AndMore_FRU').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("FRU_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "FinishedRegistrationStatictisByAge_chart" : "FinishedRegistrationStatictisByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForFinishedRegistration = new Chart(ctx, config);
    });
}

function FriendzrEventStatictis() {

    $.get("/admin/home/EventStatistics", res => {

        CreateStatictesAsRate({
            ElmentID: "FriendzrEvent_Rate",
            Series: [res.rate],
            lables: [`Friendzr Events are:${res.friendzr} from :${res.all} Event `],
            dataLabelsName_Color: "#000000",
            dataLabelsValue_Color: KTUtil.getCssVariableValue('--bs-gray-700'),
            trackBackGroundColor: "#8a9785",
            colorsArray: ["#2d4323"],
        })
        
    });
}

function GenderOfUsersStatictis() {

    let result;
    $.get("/Admin/Home/GenderOfUsersStatictes", (res) => {
        result = res;
        $('#Total_GOU').text(result.all);
        $('#Male_GOU').text(result.male);
        $('#Female_GOU').text(result.female);
        $('#Other_GOU').text(result.other);
       
        var element = document.getElementById("GenderOfUsersStatictis_chart");

        var config = {
            type: 'doughnut',
            data: {
                datasets: [{
                    data: [(result.male), (result.female), (result.other)],
                    backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                }],
                labels: ['Male', 'Female', 'Other']
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
        new Chart(ctx, config);
    });
}

function BlockedUsersStatictis() {

    let result;
    $.get("/Admin/Home/BlockedUsersStatictes", (res) => {
        result = res;
        $('#TotalByGender_BUS').text(result.all);
        $('#TotalByAge_BUS').text(result.all);
        $('#Male_BUS').text(result.male);
        $('#Female_BUS').text(result.female);
        $('#Other_BUS').text(result.other);
        $('#From18To25_BUS').text(result.from18To25);
        $('#From25To34_BUS').text(result.from25To34);
        $('#From35To44_BUS').text(result.from35To44);
        $('#From45To54_BUS').text(result.from45To54);
        $('#From55To64_BUS').text(result.from55To64);
        $('#From65AndMore_BUS').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("BUS_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "BlockedUsersStatictisByAge_chart" : "BlockedUsersStatictisByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForBlockedUsers = new Chart(ctx, config);
    });


}

function ActiveUsersStatictis() {

    let result;
    $.get("/Admin/Home/ActiveUsersStatictes", (res) => {
        result = res;
        $('#TotalByGender_ACU').text(result.all);
        $('#TotalByAge_ACU').text(result.all);
        $('#Male_ACU').text(result.male);
        $('#Female_ACU').text(result.female);
        $('#Other_ACU').text(result.other);
        $('#From18To25_ACU').text(result.from18To25);
        $('#From25To34_ACU').text(result.from25To34);
        $('#From35To44_ACU').text(result.from35To44);
        $('#From45To54_ACU').text(result.from45To54);
        $('#From55To64_ACU').text(result.from55To64);
        $('#From65AndMore_ACU').text(result.from65AndMore);

        let byAgeRadio = document.getElementById("ACU_ByAgeRadio").checked;
        var element = document.getElementById(byAgeRadio ? "ActiveUsersStatictisByAge_chart" : "ActiveUsersStatictisByGender_chart");

        if (!byAgeRadio) {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [(result.male), (result.female), (result.other)],
                        backgroundColor: ['#009EF7', '#D9214E', '#181c32']
                    }],
                    labels: ['Male', 'Female', 'Other']
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
        }
        else {
            var config = {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [result.from18To25, result.from25To34, result.from35To44, result.from45To54, result.from55To64, result.from65AndMore],
                        backgroundColor: ['#47BE7D', '#0dcaf0', '#ffc107', '#F1416C', '#E4E6EF', '#6610f2']
                    }],
                    labels: ['18-25', '25-34', '35-44', '45-55', '55-64', '65 +']
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
        }

        var ctx = element.getContext('2d');
        canvasDataForActiveUsers = new Chart(ctx, config);
    });


}


$(document).ready(() => {

    NumberOfConnectionRequestsAcceptedStatictis();
    PercentageOfUsersWhoCreatedEventsStatictis();
    NumberOfConnectionRequestsSentStatictis();
    UsersWhoEnabledPersonalSpaceStatictis();
    FinishedRegistrationStatictis();
    FriendzrEventStatictis();
    GenderOfUsersStatictis();
    BlockedUsersStatictis();
    ActiveUsersStatictis();

    $("[name='FinishedRegistrationRadio']").change((e) => {
        $("#FinishedRegistrationByGender").toggleClass("d-none");
        $("#FinishedRegistrationByAge").toggleClass("d-none");
        canvasDataForFinishedRegistration.destroy();
        FinishedRegistrationStatictis();
    });

    $("[name='UsersWhoEnabledPersonalSpaceRadio']").change((e) => {
        $("#UsersWhoEnabledPersonalSpaceByGender").toggleClass("d-none");
        $("#UsersWhoEnabledPersonalSpaceByAge").toggleClass("d-none");
        canvasDataForUsersWhoEnabledPersonalSpace.destroy();
        UsersWhoEnabledPersonalSpaceStatictis();
    });

    $("[name='NumberOfConnectionRequestsSentRadio']").change((e) => {
        $("#NumberOfConnectionRequestsSentByGender").toggleClass("d-none");
        $("#NumberOfConnectionRequestsSentByAge").toggleClass("d-none");
        canvasDataForNumberOfConnectionRequestsSent.destroy();
        NumberOfConnectionRequestsSentStatictis();
    });

    $("[name='NumberOfConnectionRequestsAcceptedRadio']").change((e) => {
        $("#NumberOfConnectionRequestsAcceptedByGender").toggleClass("d-none");
        $("#NumberOfConnectionRequestsAcceptedByAge").toggleClass("d-none");
        canvasDataForNumberOfConnectionRequestsAccepted.destroy();
        NumberOfConnectionRequestsAcceptedStatictis();
    });

    $("[name='BlockedUsersRadio']").change((e) => {
        $("#BlockedUsersByGender").toggleClass("d-none");
        $("#BlockedUsersByAge").toggleClass("d-none");
        canvasDataForBlockedUsers.destroy();
        BlockedUsersStatictis();
    });

    $("[name='ActiveUsersRadio']").change((e) => {
        $("#ActiveUsersByGender").toggleClass("d-none");
        $("#ActiveUsersByAge").toggleClass("d-none");
        canvasDataForActiveUsers.destroy();
        ActiveUsersStatictis();
    });

    $("[name='PercentageOfUsersWhoCreatedEventsRadio']").change((e) => {
        $("#PercentageOfUsersWhoCreatedEventsByGender").toggleClass("d-none");
        $("#PercentageOfUsersWhoCreatedEventsByAge").toggleClass("d-none");
        canvasDataForPercentageOfUsersWhoCreatedEvents.destroy();
        PercentageOfUsersWhoCreatedEventsStatictis();
    });

});