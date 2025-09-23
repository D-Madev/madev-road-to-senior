let dataTable;

$(document).ready(function () {
    try {
        const $tbl = $('#tblArticulos');

        if ($tbl.length === 0) {
            console.warn('No se encontró la tabla #tblArticulos en el DOM. DataTable no se inicializará.');
            return;
        }

        dataTable = $tbl.DataTable({
            ajax: {
                url: '/admin/articulos/GetAll',
                type: 'GET',
                dataType: 'json',
                error: function (xhr, status, error) {
                    console.error('Error en GET /admin/articulos/GetAll:', status, error, xhr.responseText);
                    toastr.error('Error cargando los datos de la tabla. Revisá la consola.');
                }
            },
            columns: [
                { data: 'id', width: '1%' },
                { data: 'urlImagen', render: (img) => `<img src="../${img}" width="120">`, width: '1%' },
                { data: 'nombre', width: '5%' },
                { data: 'descripcion', width: '5%' },
                { data: 'categoria.nombre', width: '5%' },
                { data: 'fechaCreacion', width: '5%' },
                {
                    data: 'id',
                    render: function (data) {
                        if (!data) {
                            console.warn('Render de columna: id indefinido', data);
                            return '';
                        }

                        return `
                            <div class="text-center">
                                <a href="/Admin/Articulos/Edit/${data}" class="btn btn-sm btn-warning">
                                    <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                                <button class="btn btn-sm btn-danger btn-delete" data-url="/Admin/Articulos/Delete/${data}" >
                                    <i class="far fa-trash-alt"></i> Borrar
                                </button>
                            </div>
                        `;
                    },
                    width: '10%'
                }
            ],
            initComplete: function () {
                console.log('DataTable inicializada correctamente');
            },
            drawCallback: function () {
                console.log('Tabla redraw: filas cargadas', this.api().rows().count());
            }
        });
    } catch (err) {
        console.error('Error inicializando DataTable:', err);
        toastr.error('No se pudo inicializar la tabla. Revisá la consola.');
    }

    // Delegación de eventos segura
    $(document).on('click', '.btn-delete', function (e) {
        e.preventDefault();
        const url = $(this).data('url');

        if (!url) {
            console.warn('Botón borrar sin URL definida', this);
            return;
        }

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
                try {
                    // const token = $('input[name="__RequestVerificationToken"]').val(); // si aplica
                    $.ajax({
                        url: url,
                        type: 'DELETE',
                        // headers: { 'RequestVerificationToken': token },
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
                            if (xhr.status === 405) {
                                toastr.error('Método no permitido (405). El servidor no acepta DELETE en esa ruta.');
                            } else {
                                toastr.error('Error en la petición. Revisá la consola para más detalles.');
                            }
                        }
                    });
                } catch (err) {
                    console.error('Error en la petición DELETE:', err);
                    toastr.error('Error inesperado. Revisá la consola.');
                }
            }
        }).catch((err) => {
            console.error('Error en Swal.fire:', err);
        });
    });
});
