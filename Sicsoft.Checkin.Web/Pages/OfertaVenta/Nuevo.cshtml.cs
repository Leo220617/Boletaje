using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace Boletaje.Pages.OfertaVenta
{
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<OfertaVentaViewModel, int> service;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<ClientesPOrdenesViewModel, int> clientes;
        private readonly ICrudApi<ProductosCOrdenesViewModel, int> prod;
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<GarantiasViewModel, int> garan;
        private readonly ICrudApi<TiemposEntregasViewModel, int> tiemp;
        private readonly ICrudApi<PersonasContactoViewModel, int> pContact;



        [BindProperty]
        public string Bodega { get; set; }
        [BindProperty]
        public OfertaVentaViewModel Orden { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        [BindProperty]
        public ClientesPOrdenesViewModel Clientes { get; set; }

        [BindProperty]
        public CondicionesPagosViewModel[] Condiciones { get; set; }

        [BindProperty]
        public GarantiasViewModel[] Garantias { get; set; }

        [BindProperty]
        public TiemposEntregasViewModel[] Tiempos { get; set; }

        [BindProperty]
        public PersonasContactoViewModel PContactos { get; set; }

        public NuevoModel(ICrudApi<OfertaVentaViewModel, int> service, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<ClientesPOrdenesViewModel, int> clientes, ICrudApi<ProductosCOrdenesViewModel, int> prod, ICrudApi<CondicionesPagosViewModel, int> conds,
            ICrudApi<GarantiasViewModel, int> garan, ICrudApi<TiemposEntregasViewModel, int> tiemp, ICrudApi<PersonasContactoViewModel, int> pContact)
        {
            this.service = service;
            this.impuestos = impuestos;
            this.clientes = clientes;
            this.prod = prod;
            this.conds = conds;
            this.garan = garan;
            this.tiemp = tiemp;
            this.pContact = pContact;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "46").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Bodega = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Bodega").Select(s1 => s1.Value).FirstOrDefault();

                PContactos = await pContact.ObtenerListaEspecial("");
                Clientes = await clientes.ObtenerListaEspecial("");
                Impuestos = await impuestos.ObtenerLista("");

                Condiciones = await conds.ObtenerLista("");
                Garantias = await garan.ObtenerLista("");
                Tiempos = await tiemp.ObtenerLista("");


                Orden = new OfertaVentaViewModel();
                Orden.CodVendedor = 1;
                Orden.Fecha = DateTime.Now;
                Orden.FechaVencimiento = DateTime.Now;
                Orden.FechaEntrega = DateTime.Now;

                if(id != 0)
                {
                    var Ofertas = await service.ObtenerPorId(id);
                    Orden.CardCode = Ofertas.CardCode;
                    Orden.Moneda = Ofertas.Moneda;
                    Orden.FechaEntrega = Ofertas.FechaEntrega;
                    Orden.Fecha = DateTime.Now;
                    Orden.FechaVencimiento = Ofertas.FechaVencimiento;
                    Orden.TipoDocumento = Ofertas.TipoDocumento;
                    Orden.NumAtCard = Ofertas.NumAtCard;
                    Orden.Comentarios = Ofertas.Comentarios + " | Basado en la oferta de ventas # " + id;
                    Orden.CodVendedor = Ofertas.CodVendedor;

                    Orden.idCondPago = Ofertas.idCondPago;
                    Orden.idGarantia = Ofertas.idGarantia;
                    Orden.idTiemposEntregas = Ofertas.idTiemposEntregas;
                    var Tama�o = Ofertas.Detalle.Count();
                    Orden.Detalle = new List<DetalleOferta>();

                    var i = 0;
                    foreach (var item in Ofertas.Detalle)
                    {

                        DetalleOferta det = new DetalleOferta();
                        det.ItemCode = item.ItemCode;
                        det.NumLinea = item.NumLinea;
                        det.Cantidad = item.Cantidad;
                        det.Bodega = item.Bodega;
                        det.PrecioUnitario = item.PrecioUnitario;
                        det.PorcentajeDescuento = item.PorcentajeDescuento;
                        det.Impuesto = item.Impuesto;
                        det.TaxOnly = item.TaxOnly;
                        det.Total = item.Total;
                        Orden.Detalle.Add(det);
                        i++;
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }


        public async Task<IActionResult> OnGetProductos(string codBodega, string codListaPrecios)
        {
            try
            {



                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Texto = codBodega;
                filtro.ListPrice = codListaPrecios;
                var objetos = await prod.ObtenerListaEspecial(filtro);

                var objeto = objetos.Productos.ToList();



                return new JsonResult(objeto);
            }
            catch (ApiException ex)
            {



                return new JsonResult(ex.Content.ToString());
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message.ToString());
            }
        }

        public async Task<IActionResult> OnPostGenerar(OfertaVentaViewModel recibido)
        {
            try
            {


                OfertaVentaViewModel coleccion = new OfertaVentaViewModel();
                coleccion.id = 0;
                coleccion.DocEntry = recibido.DocEntry;
                coleccion.DocNum = recibido.DocNum;
                coleccion.CardCode = recibido.CardCode.Split('/')[0].TrimEnd();
                coleccion.Moneda = recibido.Moneda;
                coleccion.Fecha = recibido.Fecha;
                coleccion.FechaVencimiento = recibido.FechaVencimiento;
                coleccion.FechaEntrega = recibido.FechaEntrega;
                coleccion.TipoDocumento = recibido.TipoDocumento;
                coleccion.NumAtCard = recibido.NumAtCard;
                coleccion.Series = recibido.Series;
                coleccion.Comentarios = recibido.Comentarios;
                coleccion.idCondPago = recibido.idCondPago;
                coleccion.idGarantia = recibido.idGarantia;
                coleccion.idTiemposEntregas = recibido.idTiemposEntregas;
                coleccion.CodVendedor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());

                coleccion.ProcesadaSAP = false;
                coleccion.PersonaContacto = recibido.PersonaContacto;
                coleccion.TelefonoContacto = recibido.TelefonoContacto;
                coleccion.CorreoContacto = recibido.CorreoContacto;
                coleccion.Detalle = new List<DetalleOferta>();
                var cantidad = 1;
                foreach (var item in recibido.Detalle)
                {
                    var detalle = new DetalleOferta();
                    detalle.id = 0;
                    detalle.idEncabezado = 0;
                    detalle.NumLinea = cantidad;
                    detalle.ItemCode = item.ItemCode;
                    detalle.ItemName = item.ItemName;
                    detalle.Bodega = item.Bodega;
                    detalle.PorcentajeDescuento = item.PorcentajeDescuento;
                    detalle.Cantidad = item.Cantidad;
                    detalle.Impuesto = item.Impuesto;
                    detalle.TaxOnly = item.TaxOnly;
                    try
                    {
                        detalle.PrecioUnitario = Convert.ToDecimal(item.PrecioUnitario1.ToString());

                    }
                    catch (Exception e)
                    {
                        detalle.PrecioUnitario = Convert.ToDecimal(item.PrecioUnitario1.ToString().Replace(".", ","));

                    }

                    var SubTotal = (detalle.Cantidad * detalle.PrecioUnitario);
                    SubTotal = SubTotal - (SubTotal * (detalle.PorcentajeDescuento / 100));
                    var Impuestos = SubTotal * Convert.ToDecimal((Convert.ToDecimal(detalle.Impuesto) / 100));
                    detalle.Total = detalle.TaxOnly ? Impuestos : (SubTotal + Impuestos);

                    coleccion.Detalle.Add(detalle);
                    cantidad++;
                }



                await service.Agregar(coleccion);

                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (ApiException ex)
            {

                ModelState.AddModelError(string.Empty, ex.Content.ToString());

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Content.ToString() + " -> " + ex.StackTrace.ToString()
                };
                return new JsonResult(obj);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message + " -> " + ex.StackTrace.ToString()
                };
                return new JsonResult(obj);
            }
        }
    }
}
