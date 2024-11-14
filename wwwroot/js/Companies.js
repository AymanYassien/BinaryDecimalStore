var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DTData').DataTable({
        ajax: {
            url: '/admin/Company/getAll',
            type: 'GET',
            dataType: 'json',
        },
        columns: [
            { data: 'name', width: '15%' },
            { data: 'state', width: '10%' },
            { data: 'city', width: '10%' },
            { data: 'address', width: '15%' },
            { data: 'phoneNumber', width: '20%' },
            {
                data: 'id',
                render: function(data) {
                    return `<div class="w-75 btn-group" role="group">
            <a href="/admin/Company/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Edit</a>
            <a onClick="Delete('/admin/Company/Delete/${data}')" class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i>Delete</a>
            </div>`;
                },
                width: "30%"
            }
        ],
        lengthMenu: [5, 10, 25, 50, 100],
        pageLength: 10
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE', // Ensure your server accepts this method
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
                error: function (xhr, status, error) {
                    // Handle errors here
                    toastr.error("An error occurred: " + error);
                }
            });
        }
    });
}
