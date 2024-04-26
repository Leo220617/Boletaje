
var html = '';
var specialElementHandlers = null;
$(document).ready(function () {
    jQuery(document).ready(function ($) {
        specialElementHandlers = {
            "#editor": function (element, render) {
                return true;
            }
        }
    });



    $(document).ready(function () {
        $('#tablita').DataTable();
    });

    Recuperar();
});
var Detalle = [];
var Encabezado = [];
function Recuperar() {
    try {
        var det = JSON.parse($("#Detalle").val());
        Encabezado = JSON.parse($("#BTS").val());



        for (var i = 0; i < det.length; i++) {

            var detalle =
            {
                id: det[i].id,
                idEncabezado: det[i].idEncabezado,
                idProducto: det[i].idProducto,
                idError: det[i].idError,
                Cantidad: det[i].Cantidad,
                ItemCode: det[i].ItemCode,
                Enviar: det[i].Enviar


            }
            Detalle.push(detalle);
            onChangeRevisado(i, true);
        }




    } catch (e) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: e

        });
    }
}
function ImprimirPantalla() {
    try {
        // window.print();
        var margins = {
            top: 10,
            bottom: 10,
            left: 10,
            width: 595
        };


        html = $(".html").html();
        html2pdf(html, {
            margin: 1,
            padding: 0,
            filename: 'Movimientos.pdf',
            image: { type: 'jpeg', quality: 1 },
            html2canvas: { scale: 2, logging: true },
            jsPDF: { unit: 'in', format: 'A2', orientation: 'P' },
            class: ImprimirPantalla
        });

    } catch (e) {
        console.log(e);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Ha ocurrido un error al intentar imprimir ' + e

        })
    }
}

function onChangeRevisado(i, inicio) {
    try {



        if (inicio) {
            $("#md_checkbox_" + i).prop('checked', Detalle[i].Enviar);

        } else {
            var valorCheck = $("#md_checkbox_" + i).prop('checked');
            Detalle[i].Enviar = valorCheck;
        }




    } catch (e) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: e

        });
    }

}

function Generar() {
    try {
        var btn = document.getElementById('Guardar');


        var BTS = {
            BodegaFinal: $("#bodF").val(),
            BodegaInicial: $("#bodI").val(),
            DocEntry: Encabezado.DocEntry,
            Fecha: $("#Fecha").val(),

            Status: Encabezado.Status,
            TipoMovimiento: $("#idTraslado").val(),
            id: Encabezado.id,
            idLlamada: $("#idLlamada").val(),
            idEncabezado: Encabezado.idEncabezado,
            idTecnico: Encabezado.idTecnico,

            Detalle: Detalle
        };



        Swal.fire({
            title: '¿Desea guardar la bitacora?',
            showDenyButton: true,
            showCancelButton: false,
            confirmButtonText: `Aceptar`,
            denyButtonText: `Cancelar`,
            customClass: {
                confirmButton: 'swalBtnColor',
                denyButton: 'swalDeny'
            },
        }).then((result) => {
            if (result.isConfirmed) {
                btn.disabled = true;


                $.ajax({
                    type: 'POST',

                    url: '@Url.Page("Observar", "AgregarBTS")',
                    dataType: 'json',
                    data: { recibidos: BTS },
                    headers: {
                        RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                    },
                    success: function (json) {



                        if (json.success == true) {
                            Swal.fire({
                                title: "Ha sido generado con éxito",

                                icon: 'success',
                                showCancelButton: false,

                                confirmButtonText: 'OK',
                                customClass: {
                                    confirmButton: 'swalBtnColor',

                                },
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    //Despues de insertar, ocupariamos el id del cliente en la bd
                                    //para entonces setearlo en el array de clientes

                                    window.location.href = window.location.href.split("/Observar")[0];


                                }
                            })

                        } else {
                            btn.disabled = false;
                            Swal.fire({
                                icon: 'error',
                                title: 'Oops...',
                                text: 'Ha ocurrido un error al intentar guardar'

                            })
                        }
                    },

                    beforeSend: function (xhr) {


                    },
                    complete: function () {

                    },
                    error: function (error) {


                    }
                });
            }
        })


    } catch (e) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Ha ocurrido un error al intentar agregar ' + e

        })
    }
}
