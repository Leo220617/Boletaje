using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Movimientos
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<EncMovimientoViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;

        [BindProperty]
        public EncMovimientoViewModel Input { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        [BindProperty]
        public cliente Cliente { get; set; }


        public EditarModel(ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes)
        {
            this.service = service;
            this.clientes = clientes;

        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "35").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Input = await service.ObtenerPorId(id);
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }


        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            try
            {
                RecibidoMovimientos recibido = JsonConvert.DeserializeObject<RecibidoMovimientos>(recibidos);

                EncMovimientoViewModel coleccion = new EncMovimientoViewModel();
                 
                coleccion.Detalle = new DetMovimientoViewModel[recibido.Detalle.Length];
             
                coleccion.id = recibido.id;
                coleccion.CreadoPor = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault();
                coleccion.Comentarios = recibido.Comentarios;
                coleccion.Descuento = recibido.Descuento;
                coleccion.Impuestos = recibido.Impuestos;
                coleccion.Subtotal = recibido.Subtotal;
                coleccion.PorDescuento = recibido.PorDescuento;
                coleccion.Generar = recibido.Generar;
                coleccion.TotalComprobante = recibido.TotalComprobante;
                coleccion.Moneda = recibido.Moneda;

                short cantidad = 1;
                foreach (var item in recibido.Detalle)
                {
                    coleccion.Detalle[cantidad - 1] = new DetMovimientoViewModel();
                    coleccion.Detalle[cantidad - 1].id = item.id;

                    coleccion.Detalle[cantidad - 1].PrecioUnitario = item.PrecioUnitario;
                    coleccion.Detalle[cantidad - 1].Cantidad = item.Cantidad;
                    coleccion.Detalle[cantidad - 1].PorDescuento = item.PorDescuento;
                    coleccion.Detalle[cantidad - 1].Descuento = item.Descuento;
                    coleccion.Detalle[cantidad - 1].Impuestos = item.Impuestos;
                    coleccion.Detalle[cantidad - 1].TotalLinea = item.TotalLinea;
                    coleccion.Detalle[cantidad - 1].Garantia = item.Garantia;


                    cantidad++;
                }

                await service.Editar(coleccion);


                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message
                };
                return new JsonResult(obj);
            }
        }

    }
}
