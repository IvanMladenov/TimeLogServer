let usersData;
let projectData;
let userForCompare;
let isUserChartActive = false;
let isCompareMode = false;

function drawUserChart() {
    isUserChartActive = true;

    const data = new google.visualization.DataTable();
    data.addColumn('string', 'User');
    data.addColumn('number', 'Total Time');

    if (isCompareMode) {
        data.addColumn('number', userForCompare.userName);
    }
    
    usersData.forEach(user => {
        const row = [user.userName, user.totalTime];
        
        if (isCompareMode) {
            row.push(userForCompare.totalTime);
        }

        data.addRow(row);
    });

    const options = {
        title: 'User Total Time with Level Line',
        seriesType: 'bars',
        bars: 'vertical',
        height: 400,
        hAxis: {
            title: 'Users'
        },
        vAxis: {
            title: 'Total Time',
            viewWindow: {
                min: 0
            }
        }
    };
    
    if (isCompareMode) {
        options.series = { 1: { type: 'line' } };
    }

    const chart = new google.visualization.ComboChart(document.getElementById('googleChartContainerAside'));
    chart.draw(data, options);
}


function drawProjectChart() {
    if (!projectData) {
        initializeProjectsChart();
        return;
    }

    isUserChartActive = false;

    const data = new google.visualization.DataTable();
    data.addColumn('string', 'Project');
    data.addColumn('number', 'Time');
    
    const rows = projectData.map(item => [item.name, item.timeWorked]);

    data.addRows(rows);

    const options = {
        title: 'Project Time Distribution',
        seriesType: 'bars',
        width: '100%',
        height: 400,
        bars: 'vertical',
        legend: { position: 'none' },
        hAxis: {
            title: 'Project'
        },
        vAxis: {
            title: 'Total Time',
            viewWindow: {
                min: 0
            }
        }
    };

    const chart = new google.visualization.ComboChart(document.getElementById('googleChartContainerAside'));
    chart.draw(data, options);
}

function compareUser(id) {
    const startDate = $('#dateFromInput').val();
    const endDate = $('#dateToInput').val();
    isCompareMode = true;
    $('#userChartRadio').prop('checked', true);

    fetchDataFromServer(
        '/Data/GetCompareUser',
        'GET',
        {
            dateFrom: startDate,
            dateTo: endDate,
            userId: id
        },
        function (data) {
            userForCompare = data;
            drawUserChart(true);
        },
        function (error) {
            console.error('Error:', error);
        }
    )
}
function initializeProjectsChart() {
    const startDate = $('#dateFromInput').val();
    const endDate = $('#dateToInput').val();

    fetchDataFromServer(
        '/Data/GetProgectsByTime',
        'GET',
        {
            dateFrom: startDate,
            dateTo: endDate
        },
        function (data) {
            projectData = data;
            drawProjectChart(false);
        },
        function (error) {
            console.error('Error:', error);
        }
    )
}
function initializeUsersChart() {
    const startDate = $('#dateFromInput').val();
    const endDate = $('#dateToInput').val();

    fetchDataFromServer(
        '/Data/GetTopUsers',
        'GET',
         {
            dateFrom: startDate,
            dateTo: endDate
        },
        function (data) {
            usersData = data;
            drawUserChart(false);
        },
        function (error) {
            console.error('Error:', error);
        }
    )
}

function fetchDataFromServer(endpoint, method, filters, successCallback, errorCallback) {
    const queryString = $.param(filters);

    // Append the query string to the endpoint if it's a GET request
    const url = method === 'GET' ? `${endpoint}?${queryString}` : endpoint;

    $.ajax({
        url: url,
        method: method,
        contentType: 'application/json',
        data: method !== 'GET' ? JSON.stringify(filters) : null,
        success: function (data) {
            successCallback(data);
        },
        error: function (xhr, textStatus, errorThrown) {
            errorCallback(`HTTP error! Status: ${xhr.status}. ${errorThrown}`);
        }
    });
}


