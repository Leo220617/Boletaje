using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Reparacion
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<DetReparacionViewModel, int> service;
        private readonly ICrudApi<LlamadasViewModel, int> serviceL;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<EncReparacionViewModel, int> serviceE;
        private readonly ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion;
        private readonly ICrudApi<BodegasViewModel, int> serviceBodegas;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> bt;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;
        private readonly ICrudApi<ErroresViewModel, int> serviceError;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<ControlProductosViewModel, int> control;
        private readonly ICrudApi<CotizacionesAprobadasViewModel, int> cotizaciones;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;
        private readonly ICrudApi<ProductosPadresViewModel, int> prodsPadre;


        [BindProperty]

        public CotizacionesAprobadasViewModel[] CotizacionesAprobadas { get; set; }

        [BindProperty]

        public StatusViewModel[] Status { get; set; }
        [BindProperty]
        public LlamadasViewModel InputLlamada { get; set; }

        [BindProperty]
        public ErroresViewModel[] Errores { get; set; }
        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }
        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public string Producto { get; set; }
        [BindProperty]
        public BodegasViewModel[] Bodegas { get; set; }

        [BindProperty]
        public BitacoraMovimientosViewModel[] BTS { get; set; }
        [BindProperty]
        public ProductosHijosViewModel[] InputHijos { get; set; }

        [BindProperty]
        public DetReparacionViewModel[] Input { get; set; }
        [BindProperty]
        public ProductosHijosViewModel[] ProductosGenerales { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        [BindProperty]
        public EncReparacionViewModel Encabezado { get; set; }

        [BindProperty]
        public ControlProductosViewModel[] Control { get; set; }
        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }

        [BindProperty]
        public ProductosPadresViewModel ProductoPadre { get; set; }
        [BindProperty]

        public Producto ProductoPadreLlamada { get; set; }

        public EditarModel(ICrudApi<DetReparacionViewModel, int> service, ICrudApi<LlamadasViewModel, int> serviceL, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods,
           ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<EncReparacionViewModel, int> serviceE, ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion, ICrudApi<BodegasViewModel, int> serviceBodegas, ICrudApi<BitacoraMovimientosViewModel, int> bt, ICrudApi<DiagnosticosViewModel, int> serviceD
            ,ICrudApi<ErroresViewModel, int> serviceError, ICrudApi<StatusViewModel, int> status, ICrudApi<ControlProductosViewModel, int> control, ICrudApi<CotizacionesAprobadasViewModel, int> cotizaciones, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<ProductosPadresViewModel, int> prodsPadre)
        {
            this.service = service;
            this.serviceL = serviceL;
            this.clientes = clientes;
            this.prods = prods;
            this.service2 = service2;
            this.serviceE = serviceE;
            this.serviceColeccion = serviceColeccion;
            this.serviceBodegas = serviceBodegas;
            this.bt = bt;
            this.serviceD = serviceD;
            this.serviceError = serviceError;
            this.status = status;
            this.control = control;
            this.cotizaciones = cotizaciones;
            this.tp = tp;
            this.prodsPadre = prodsPadre;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "23").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ProductosGenerales = await service2.ObtenerLista("");
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = id;
                TP = await tp.ObtenerLista("");
                Control = await control.ObtenerLista(filt);
                BTS = await bt.ObtenerLista(filt);
                Bodegas = await serviceBodegas.ObtenerLista("");
                Input = await service.ObtenerLista(filt); //Se trae todos los hijos seleccionados
                Encabezado = await serviceE.ObtenerPorId(id);

                filt.Codigo1 = 0;
                filt.Texto = Encabezado.idProductoArreglar;
                InputHijos = await service2.ObtenerLista(filt); // Se trae todos los productos hijos dispobibles
                var i = 0;
                foreach (var item in Input)
                {
                    Input[i].Stock = InputHijos.Where(a => a.id == item.idProducto).FirstOrDefault().Stock;
                    InputHijos = InputHijos.Where(a => a.id != item.idProducto).ToArray();
                    i++;
                }

                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;
                Diagnosticos = await serviceD.ObtenerLista("");
                Errores = await serviceError.ObtenerLista("");

                InputLlamada = await serviceL.ObtenerPorDocEntry(Encabezado.idLlamada);
                Status = await status.ObtenerLista("");
                ParametrosFiltros filtroCoti = new ParametrosFiltros();
                filtroCoti.Codigo1 = InputLlamada.DocEntry.Value;
                CotizacionesAprobadas = await cotizaciones.ObtenerLista(filtroCoti);
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == InputLlamada.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == InputLlamada.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == InputLlamada.CardCode).FirstOrDefault().CardName;
                var ProductosPadres = await prodsPadre.ObtenerLista("");
                ProductoPadre = ProductosPadres.Where(a => a.codSAP == InputLlamada.ItemCode).FirstOrDefault();
                if (ProductoPadre == null)
                {
                    ProductoPadre = new ProductosPadresViewModel();
                    ProductoPadre.codSAP = InputLlamada.ItemCode;
                    ProductoPadre.Nombre = InputLlamada.ItemCode;
                    ProductoPadre.Precio = 0;
                }
                else
                {
                    ProductoPadre.Precio = Math.Round(ProductoPadre.Precio, 2);
                }
                return Page();
            }
            catch (ApiException ex)
            {

                ModelState.AddModelError(string.Empty, ex.Content.ToString());

               
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
                RecibidoReparacion recibido = JsonConvert.DeserializeObject<RecibidoReparacion>(recibidos);

                ColeccionRepuestosViewModel coleccion = new ColeccionRepuestosViewModel();
                coleccion.EncReparacion = new EncReparacionViewModel();
                coleccion.DetReparacion = new DetReparacionViewModel[recibido.DetReparacion.Length];
                coleccion.Adjuntos = new AdjuntosViewModel[recibido.Adjuntos.Length];


                coleccion.EncReparacion.id = recibido.id;
                coleccion.EncReparacion.TipoReparacion = recibido.Tipo;
                coleccion.EncReparacion.Status = recibido.Status;
                coleccion.EncReparacion.Comentarios = recibido.comentarios;
                coleccion.EncReparacion.BodegaOrigen = recibido.BodegaInicial;
                coleccion.EncReparacion.BodegaFinal = recibido.BodegaFinal;
                coleccion.EncReparacion.idDiagnostico = recibido.idDiagnostico;

                short cantidad = 1;
                foreach (var item in recibido.DetReparacion)
                {
                    coleccion.DetReparacion[cantidad - 1] = new DetReparacionViewModel();
                    coleccion.DetReparacion[cantidad - 1].idEncabezado = recibido.id;
                    coleccion.DetReparacion[cantidad - 1].idProducto = item.idProducto;
                    coleccion.DetReparacion[cantidad - 1].ItemCode = item.ItemCode;
                    coleccion.DetReparacion[cantidad - 1].Cantidad = item.Cantidad;
                    coleccion.DetReparacion[cantidad - 1].idError = item.idError;
                    coleccion.DetReparacion[cantidad - 1].Opcional = item.Opcional;


                    cantidad++;
                }

                cantidad = 1;
                foreach(var item in recibido.Adjuntos)
                {
                    coleccion.Adjuntos[cantidad - 1] = new AdjuntosViewModel();
                    coleccion.Adjuntos[cantidad - 1].base64 = item.base64;
                    cantidad++;
                }


                await serviceColeccion.Agregar(coleccion);

                try
                {
                    var Status = recibido.StatusLlamada;
                    InputLlamada = await serviceL.ObtenerPorId(recibido.idLlamada);
                    InputLlamada.Status = Status;

                    await serviceL.Editar(InputLlamada);
                }
                catch (Exception ex)
                {
 
                }
             


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
                    mensaje = "Error en el exception: -> " + ex.Content.ToString()
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
