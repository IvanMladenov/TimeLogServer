function initializeUserGrid() {
    $('#userGrid').DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "lengthMenu": [10, 25, 50, 100],
        "dom": '<"top">rt<"bottom"p><"clear">',
        "ajax": {
            "url": "/Data/LoadUserData",
            "type": "POST",
            "dataType": "json",
            "contentType": 'application/json',
            "data": function (d) {

                if ($('#dateFromInput').val() && $('#dateToInput').val()) {
                    d.dateFrom = formatDate($('#dateFromInput').val());
                    d.dateTo = formatDate($('#dateToInput').val());
                }

                return JSON.stringify(d);
            },
        },
        "columns": [
            { "data": "name" },
            { "data": "surname" },
            {
                "data": null,
                "orderable": false,
                "className": "text-center",
                "render": function (data, type, row) {
                    return '<button class="btn btn-primary" onclick="compareUser(' + row.id + ')">Compare</button>';
                }
            }
        ]
    });
}
function formatDate(dateString) {
    const date = new Date(dateString);
    const formattedDate = date.toISOString().split('T')[0];
    return formattedDate;
}

