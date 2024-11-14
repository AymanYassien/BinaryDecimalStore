var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    
    if (url.includes("pending"))
        loadDataTable("pending");
    else if (url.includes("inprocess"))
        loadDataTable("inprocess");
    else if (url.includes("completed"))
        loadDataTable("completed");
    else if (url.includes("approved"))
        loadDataTable("approved");
    else 
        loadDataTable("all");
    
});

function loadDataTable(status) {
    dataTable = $('#DTData').DataTable({
        ajax: {
            url: '/admin/OrderManag/getAll?status=' + status,
            type: 'GET',
            dataType: 'json',
        },
        columns: [
            { data: 'id', width: '9%' },
            { data: 'extendIdentity.name', width: '13%' },
            { data: 'phoneNumber', width: '13%' },
            { data: 'extendIdentity.email', width: '15%' },
            { data: 'orderStatus', width: '15%' },
            { data: 'orderCurrencyCode', width: '15%' },
            
            {
                data: 'id',
                render: function(data) {
                    return `<div class="w-75 btn-group" role="group">
            <a href="/admin/OrderManag/details?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
           
            </div>`;
                },
                width: "20%"
            }
        ]
      
    });
}
