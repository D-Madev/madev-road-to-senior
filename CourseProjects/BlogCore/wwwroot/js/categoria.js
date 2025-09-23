let dataTable;

$(document).ready(function () {
    dataTable = $('#tblCategorias').DataTable({
        ajax: {
            url: '/admin/categorias/GetAll',
            type: 'GET',
            dataType: 'json'
        },
        columns: [
            { data: 'id', width: '5%' },
            { data: 'nombre', width: '50%' },
            { data: 'orden', width: '5%' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Categorias/Edit/${data}" class="btn btn-sm btn-warning">
                                <i class="far fa-edit"></i> Editar
                            </a>
                            &nbsp;
                            <button class="btn btn-sm btn-danger btn-delete" data-url="/Admin/Categorias/Delete/${data}">
                                <i class="far fa-trash-alt"></i> Borrar
                            </button>
                        </div>
                    `;
                },
                width: '30%'
            }
        ]
    });
});

// Delegación de eventos (funciona tras redraws)
$(document).on('click', '.btn-delete', function (e) {
    e.preventDefault();
    const url = $(this).data('url');

    Swal.fire({
        title: '¿Está seguro?',
        text: '¡Este contenido no se puede recuperar!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'Sí, borrar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            // Opcional: si hay antiforgery token (ASP.NET), obtenelo y envialo en headers:
            // const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: url,
                type: 'DELETE',
                // headers: { 'RequestVerificationToken': token }, // descomentar si aplica
                success: function (data) {
                    if (data && data.success) {
                        toastr.success(data.message || 'Eliminado correctamente');
                        if (dataTable) dataTable.ajax.reload(null, false);
                    } else {
                        toastr.error((data && data.message) || 'Ocurrió un error al eliminar');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('DELETE error', status, error, xhr.responseText);
                    // Si el servidor responde 405 -> method not allowed
                    if (xhr.status === 405) {
                        toastr.error('Método no permitido (405). El servidor no acepta DELETE en esa ruta.');
                    } else {
                        toastr.error('Error en la petición. Revisá la consola para más detalles.');
                    }
                }
            });
        }
    });
});
