using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
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
        private readonly ICrudApi<HistoricoViewModel, int> historico; 
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<UsuariosViewModel, int> login;
        private readonly ICrudApi<UbicacionesViewModel, int> ubicaciones;
        private readonly ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado;


        [BindProperty]
        public string UbicacionProd { get; set; }
        [BindProperty]

        public CotizacionesAprobadasViewModel[] CotizacionesAprobadas { get; set; }

        [BindProperty]
        public HistoricoViewModel Historico { get; set; }

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

        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public UbicacionesViewModel[] Ubicaciones { get; set; }
        [BindProperty]
        public HistoricoDetalladoViewModel HistoricoDetallado { get; set; }
        public EditarModel(ICrudApi<DetReparacionViewModel, int> service, ICrudApi<LlamadasViewModel, int> serviceL, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods,
           ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<EncReparacionViewModel, int> serviceE, ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion, ICrudApi<BodegasViewModel, int> serviceBodegas, ICrudApi<BitacoraMovimientosViewModel, int> bt, ICrudApi<DiagnosticosViewModel, int> serviceD
            ,ICrudApi<ErroresViewModel, int> serviceError, ICrudApi<StatusViewModel, int> status, ICrudApi<ControlProductosViewModel, int> control, ICrudApi<CotizacionesAprobadasViewModel, int> cotizaciones, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<ProductosPadresViewModel, int> prodsPadre, ICrudApi<HistoricoViewModel, int> historico,
           ICrudApi<ActividadesViewModel, int> actividades, ICrudApi<UsuariosViewModel, int> login, ICrudApi<UbicacionesViewModel, int> ubicaciones, ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado)
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
            this.historico = historico;
            this.actividades = actividades;
            this.login = login;
            this.ubicaciones = ubicaciones;
            this.historicoDetallado = historicoDetallado;
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
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "49").FirstOrDefault()))
                {
                    foreach (var item in Input)
                    {
                        Input[i].Stock = InputHijos.Where(a => a.id == item.idProducto).FirstOrDefault().Stock;
                        InputHijos = InputHijos.Where(a => a.id != item.idProducto).ToArray();
                        i++;
                    }
                }
                else
                {
                    foreach (var item in Input)
                    {
                        Input[i].Stock = ProductosGenerales.Where(a => a.id == item.idProducto).FirstOrDefault() == null ? 0 : ProductosGenerales.Where(a => a.id == item.idProducto).FirstOrDefault().Stock;
                        
                        i++;
                    }
                }
                   

                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;

                ParametrosFiltros filtUbi = new ParametrosFiltros();
                filtUbi.Texto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode;
                Ubicaciones = await ubicaciones.ObtenerLista(filtUbi);
                UbicacionProd = Ubicaciones.FirstOrDefault() == null ? "" : Ubicaciones.FirstOrDefault().Ubicacion;
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

                try
                {
                    ParametrosFiltros filtHist = new ParametrosFiltros();
                    filtHist.CardCode = InputLlamada.DocEntry.ToString();
                    Historico = await historico.ObtenerListaEspecial(filtHist);
                }
                catch (Exception ex)
                {

                }
                try
                {
                    ParametrosFiltros filtHistDet = new ParametrosFiltros();
                    filtHistDet.CardCode = InputLlamada.SerieFabricante.ToString();
                    filtHistDet.CardName = InputLlamada.ItemCode.ToString();
                    HistoricoDetallado = await historicoDetallado.ObtenerListaEspecial(filtHistDet);
                }
                catch (Exception ex)
                {
                    HistoricoDetallado = new HistoricoDetalladoViewModel();
                    HistoricoDetallado.Historico = new HistoricoDetalladoViewModel.historicoDet[1];
                    HistoricoDetallado.Historico[0] = new HistoricoDetalladoViewModel.historicoDet();
                    HistoricoDetallado.Historico[0].Boleta = "";
                    HistoricoDetallado.Historico[0].Fecha = DateTime.Now;
                    HistoricoDetallado.Historico[0].Tecnico = "";
                    HistoricoDetallado.Historico[0].DocEntryEntrega = "";
                    HistoricoDetallado.Historico[0].Articulo = "";
                    HistoricoDetallado.Historico[0].Descripcion = "";
                    HistoricoDetallado.Historico[0].Garantia = 0;
                    HistoricoDetallado.Historico[0].Facturado = 0;

                }
                ParametrosFiltros filtro2 = new ParametrosFiltros();
                filtro2.Codigo1 = InputLlamada.id;
                Actividades = await actividades.ObtenerLista(filtro2);
                Usuarios = await login.ObtenerLista("");

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
        public async Task<IActionResult> OnGetProductos(string codBodega, string codListaPrecios)
        {
            try
            {



                var Productos = await service2.ObtenerLista("");

                 



                return new JsonResult(Productos);
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
        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            try
            {
                RecibidoReparacion recibido = new RecibidoReparacion();
                var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);

                byte[] compressedData = ms.ToArray();

                // Descomprimir los datos utilizando GZip
                using (var compressedStream = new MemoryStream(compressedData))
                using (var decompressedStream = new MemoryStream())
                {
                    using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }

                    // Convertir los datos descomprimidos a una cadena JSON
                    var jsonString = System.Text.Encoding.UTF8.GetString(decompressedStream.ToArray());

                    // Procesar la cadena JSON como desees
                    // Por ejemplo, puedes deserializarla a un objeto C# utilizando Newtonsoft.Json
                    recibido = Newtonsoft.Json.JsonConvert.DeserializeObject<RecibidoReparacion>(jsonString);
                }
                //RecibidoReparacion recibido = JsonConvert.DeserializeObject<RecibidoReparacion>(recibidos);

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
                    var Horas = recibido.HorasLlamada;
                    InputLlamada = await serviceL.ObtenerPorId(recibido.idLlamada);
                    InputLlamada.Status = Status;
                    InputLlamada.Horas = Horas;
                    await serviceL.Editar(InputLlamada);
                }
                catch (Exception ex)
                {
                    var obj2 = new
                    {
                        success = true,
                        mensaje = "La reparacion se ejecuto correctamente, pero la boleta dio error al actualizar " + ex.Message
                    };

                    return new JsonResult(obj2);
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

                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + error.Message
                };
                return new JsonResult(obj);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.ToString()  
                };
                return new JsonResult(obj);
            }
        }

        public async Task<IActionResult> OnPostGenerarActividad(ActividadesViewModel recibido)
        {
            try
            {


                ActividadesViewModel coleccion = new ActividadesViewModel();
                coleccion.id = 0;
                coleccion.idLlamada = recibido.idLlamada;
                coleccion.TipoActividad = recibido.TipoActividad;
                coleccion.Detalle = recibido.Detalle;
                coleccion.UsuarioCreador = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                coleccion.idLogin = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());



                await actividades.Agregar(coleccion);

                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (ApiException ex)
            {

                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + error.Message
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

        public async Task<IActionResult> OnGetEnviarActividadSAP(string idB)
        {
            try
            {

                var Actividad = new ActividadesViewModel();
                Actividad.id = Convert.ToInt32(idB);
                var objetos = await actividades.EnviarSAP(Actividad);




                return new JsonResult(objetos);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());


                return new JsonResult(error);
            }
        }
    }
}
