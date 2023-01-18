let canvasDataForFinishedRegistration = {};
function AgeUserStatictis(ID) {

    let result;
    $.get(`/Whitelabel/Home/AgeUserStatictis?Id=${ID}`, (res) => {
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

        
        var element = document.getElementById("UserStatictisByAge_chart");
        
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
       

        var ctx = element.getContext('2d');
        canvasDataForFinishedRegistration = new Chart(ctx, config);
    });
}