var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DTData').DataTable({
        ajax: {
            url: '/admin/user/getAll'
            //type: 'GET',
            //dataType: 'json',
        },
        columns: [
            { data: 'name', width: '15%' },
            { data: 'email', width: '10%' },
            { data: 'phoneNumber', width: '10%' },
            { data: 'company.name', width: '10%' },
            { data: 'role', width: '10%' },
            {
                data: {id:"id", lockoutEnd:"lockoutEnd"},
                render: function(data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today){
                        return `
                           <div class="text-center" >
                                   <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor: pointer; width: 100px"><i class="bi bi-lock-fill"></i>&nbsp;Lock</a>
                                   <a  href="/admin/user/manageRole?id=${data.id}" class="btn btn-danger text-white" style="cursor: pointer; width: 150px"><i class="bi bi-pencil-square"></i>&nbsp;Permission</a>
                           </div>
                    `
                    }else
                    {
                        return `
                           <div class="text-center" >
                                   
                                   <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor: pointer; width: 100px"><i class="bi bi-unlock-fill"></i>&nbsp;UnLock</a>
                                   <a  href="/admin/user/manageRole?id=${data.id}" class="btn btn-danger text-white" style="cursor: pointer; width: 150px"><i class="bi bi-pencil-square"></i>&nbsp;Permission</a>
                           </div>
                    `
                    }
                    
                    
                },
                width: "45%"
            }
        ],
        lengthMenu: [5, 10, 25, 50, 100],
        pageLength: 10
    });
}

function LockUnlock(id){
    $.ajax({
        url: '/admin/User/lockUnlock', 
        type: "POST",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data){
            if (data.success){
                toastr.success(data.message);
                dataTable.ajax.reload();
            }}
    });
}

function LockU_nlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}