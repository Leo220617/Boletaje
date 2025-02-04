using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO.Compression;
using System.IO;
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
        private readonly ICrudApi<LlamadasViewModel, int> serviceLlamada;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<ProductosPadresViewModel, int> prodsPadre;
        private readonly ICrudApi<HistoricoViewModel, int> historico;
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<UsuariosViewModel, int> login;
        private readonly ICrudApi<UbicacionesViewModel, int> ubicaciones;
        private readonly ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado;
        private readonly ICrudApi<DiasValidosViewModel, int> dvalid;
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<GarantiasViewModel, int> garan;
        private readonly ICrudApi<TiemposEntregasViewModel, int> tiemp;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;
        private readonly ICrudApi<ErroresViewModel, int> serviceError;
        private readonly ICrudApi<ExoneracionesViewModel, int> exonera;
        private readonly ICrudApi<ProductosGarantiasViewModel, int> prodHoras;
        private readonly ICrudApi<EncFacturasViewModel, int> facturas;
        private readonly ICrudApi<TecnicosViewModel, int> tecnicos;


        [BindProperty]
        public LlamadasViewModel Llamada { get; set; }
        [BindProperty]
        public EncMovimientoViewModel Input { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        [BindProperty]
        public cliente Cliente { get; set; }

        [BindProperty]
        public bool RolAceptacion { get; set; }
        [BindProperty]
        public ProductosPadresViewModel ProductoPadre { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] Productos { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijos { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Imp { get; set; }

        [BindProperty]
        public ProductosHijosViewModel ManoObra { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijosInsertar { get; set; }

        [BindProperty]

        public Producto ProductoPadreLlamada { get; set; }

        [BindProperty]
        public HistoricoViewModel Historico { get; set; }
        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }

        [BindProperty]

        public StatusViewModel[] Status { get; set; }

        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public UbicacionesViewModel[] Ubicaciones { get; set; }

        [BindProperty]
        public string UbicacionProd { get; set; }

        [BindProperty]
        public HistoricoDetalladoViewModel HistoricoDetallado { get; set; }

        [BindProperty]
        public CondicionesPagosViewModel[] Condiciones { get; set; }

        [BindProperty]
        public GarantiasViewModel[] Garantias { get; set; }

        [BindProperty]
        public TiemposEntregasViewModel[] Tiempos { get; set; }
        [BindProperty]
        public DiasValidosViewModel[] DiasValidos { get; set; }

        [BindProperty]
        public ErroresViewModel[] Errores { get; set; }

        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }
        [BindProperty]
        public LlamadasViewModel InputLlamada { get; set; }
        [BindProperty]
        public ExoneracionesViewModel Exoneraciones { get; set; }

        [BindProperty]
        public decimal CantidadHoras { get; set; }
        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }

        public EditarModel(ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> serviceLlamada, ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<ProductosViewModel, int> prods,
            ICrudApi<ProductosPadresViewModel, int> prodsPadre, ICrudApi<HistoricoViewModel, int> historico, ICrudApi<ActividadesViewModel, int> actividades, 
            ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<UsuariosViewModel, int> login, ICrudApi<UbicacionesViewModel, int> ubicaciones, ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado, ICrudApi<CondicionesPagosViewModel, int> conds,
            ICrudApi<GarantiasViewModel, int> garan, ICrudApi<TiemposEntregasViewModel, int> tiemp, ICrudApi<DiasValidosViewModel, int> dvalid, ICrudApi<DiagnosticosViewModel, int> serviceD
            , ICrudApi<ErroresViewModel, int> serviceError, ICrudApi<ExoneracionesViewModel, int> exonera, ICrudApi<ProductosGarantiasViewModel, int> prodHoras, ICrudApi<EncFacturasViewModel, int> facturas, ICrudApi<TecnicosViewModel, int> tecnicos)
        {
            this.service = service;
            this.clientes = clientes;
            this.serviceLlamada = serviceLlamada;
            this.service2 = service2;
            this.impuestos = impuestos;
            this.prods = prods; 
            this.prodsPadre = prodsPadre;
            this.historico = historico;
            this.actividades = actividades;
            this.status = status;
            this.tp = tp;
            this.login = login;
            this.ubicaciones = ubicaciones;
            this.historicoDetallado = historicoDetallado;
            this.conds = conds;
            this.garan = garan;
            this.tiemp = tiemp; 
            this.dvalid = dvalid;
            this.serviceError = serviceError;
            this.serviceD = serviceD;
            this.exonera = exonera;
            this.prodHoras = prodHoras;
            this.facturas = facturas;
            this.tecnicos = tecnicos;
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
                Diagnosticos = await serviceD.ObtenerLista("");
                Errores = await serviceError.ObtenerLista("");
                Status = await status.ObtenerLista("");
                TP = await tp.ObtenerLista("");
                Imp = await impuestos.ObtenerLista("");

                Condiciones = await conds.ObtenerLista("");
                Garantias = await garan.ObtenerLista("");
                Tiempos = await tiemp.ObtenerLista("");
                DiasValidos = await dvalid.ObtenerLista("");

                RolAceptacion = Roles1.Where(a => a == "36").FirstOrDefault() != null;

                Input = await service.ObtenerPorId(id);
                Llamada = await serviceLlamada.ObtenerPorDocEntry(Convert.ToInt32(Input.NumLlamada));
                Tecnicos = await tecnicos.ObtenerLista("");
                var tec = Llamada.Tecnico.ToString();
                Tecnicos = Tecnicos != null ? Tecnicos.Where(a => a.idSAP == tec).ToArray(): Tecnicos;

                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();
                ParametrosFiltros filtroExo = new ParametrosFiltros();
                filtroExo.CardCode = Cliente.CardCode;
                Exoneraciones = await exonera.ObtenerListaEspecial(filtroExo);
                var ProductosHijos2 = new List<ProductosHijosViewModel>();
                var Produc = await service2.ObtenerLista("");
                Productos = Produc;
                foreach (var item in Input.Detalle)
                {
                    var Pr = Produc.Where(a => a.codSAP == item.ItemCode).FirstOrDefault();
                    ProductosHijos2.Add(Pr);
                }

                ManoObra = Produc.Where(a => a.Nombre.ToUpper().Contains("Mano de Obra".ToUpper())).FirstOrDefault();
                ProductosHijos2.Add(ManoObra);
                ProductosHijos = ProductosHijos2.ToArray();
                ProductosHijosInsertar = await service2.ObtenerLista("");

                foreach (var item in ProductosHijos2)
                {
                    ProductosHijosInsertar = ProductosHijosInsertar.Where(a => a.id != item.id).ToArray();
                }

                var ProductosPadres = await prodsPadre.ObtenerLista("");
                ProductoPadre = ProductosPadres.Where(a => a.codSAP == Llamada.ItemCode).FirstOrDefault();
                if(ProductoPadre == null)
                {
                    ProductoPadre = new ProductosPadresViewModel();
                    ProductoPadre.codSAP = Llamada.ItemCode;
                    ProductoPadre.Nombre = Llamada.ItemCode;
                    ProductoPadre.Precio = 0;
                }
                else
                {
                    ProductoPadre.Precio = Math.Round(ProductoPadre.Precio, 2);
                    ParametrosFiltros filtUbi = new ParametrosFiltros();
                    filtUbi.Texto = ProductoPadre.codSAP;
                    Ubicaciones = await ubicaciones.ObtenerLista(filtUbi);
                    UbicacionProd = (Ubicaciones.FirstOrDefault() != null ? Ubicaciones.FirstOrDefault().Ubicacion : " ");

                }
                try
                {
                    ParametrosFiltros filtHist = new ParametrosFiltros();
                    filtHist.CardCode = Llamada.DocEntry.ToString();
                    Historico = await historico.ObtenerListaEspecial(filtHist);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    ParametrosFiltros filtHistDet = new ParametrosFiltros();
                    filtHistDet.CardCode = Llamada.SerieFabricante.ToString();
                    filtHistDet.CardName = Llamada.ItemCode.ToString();
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
                filtro2.Codigo1 = Llamada.id;
                Actividades = await actividades.ObtenerLista(filtro2);
                Usuarios = await login.ObtenerLista("");

                ParametrosFiltros filtroProdHoras = new ParametrosFiltros();
                filtroProdHoras.ItemCode = Llamada.ItemCode;
                var CantidadHoras2 = await prodHoras.ObtenerLista(filtroProdHoras);

                CantidadHoras = CantidadHoras2.FirstOrDefault() == null ? 0 : CantidadHoras2.FirstOrDefault().CantidadFinal;
                filtroProdHoras.Texto = Llamada.DocEntry.ToString();
                var Facturas = await facturas.ObtenerLista(filtroProdHoras);

                var CantidadFacturado = Facturas.Where(a => a.idEntrega == 0).FirstOrDefault() == null ? 0 : Facturas.Where(a => a.idEntrega == 0).FirstOrDefault().DetFactura.Count() == 0 ? 0 : Facturas.Where(a => a.idEntrega == 0).FirstOrDefault().DetFactura.FirstOrDefault().Cantidad;


                CantidadHoras = CantidadHoras - CantidadFacturado;

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
                RecibidoMovimientos recibido = new RecibidoMovimientos();
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
                    recibido = Newtonsoft.Json.JsonConvert.DeserializeObject<RecibidoMovimientos>(jsonString);
                }

                //RecibidoMovimientos recibido = JsonConvert.DeserializeObject<RecibidoMovimientos>(recibidos);

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
                coleccion.Regenerar = recibido.Regenerar; 
                coleccion.TotalComprobante = recibido.TotalComprobante;
                coleccion.Moneda = recibido.Moneda;
                coleccion.idCondPago = recibido.idCondPago;
                coleccion.idGarantia = recibido.idGarantia;
                coleccion.idTiemposEntregas = recibido.idTiemposEntregas;
                coleccion.idDiasValidos = recibido.idDiasValidos;
                coleccion.Redondeo = recibido.Redondeo;
                short cantidad = 1;
                foreach (var item in recibido.Detalle)
                {
                    coleccion.Detalle[cantidad - 1] = new DetMovimientoViewModel();
                    coleccion.Detalle[cantidad - 1].id = item.id;
                    coleccion.Detalle[cantidad - 1].ItemCode = item.ItemCode;
                    coleccion.Detalle[cantidad - 1].ItemName = item.ItemName;
                    coleccion.Detalle[cantidad - 1].NumLinea = item.NumLinea;


                    coleccion.Detalle[cantidad - 1].PrecioUnitario = item.PrecioUnitario;
                    coleccion.Detalle[cantidad - 1].Cantidad = item.Cantidad;
                    coleccion.Detalle[cantidad - 1].PorDescuento = 0;//item.PorDescuento;
                    coleccion.Detalle[cantidad - 1].Descuento = 0;//item.Descuento;
                    coleccion.Detalle[cantidad - 1].Impuestos = item.Impuestos;
                    coleccion.Detalle[cantidad - 1].TotalLinea = item.TotalLinea;
                    coleccion.Detalle[cantidad - 1].Garantia = item.Garantia;
                    coleccion.Detalle[cantidad - 1].Opcional = item.Opcional;
                    coleccion.Detalle[cantidad - 1].idImpuesto = item.idImpuesto;
                    coleccion.Detalle[cantidad - 1].idImpuesto = item.idImpuesto;
                    coleccion.Detalle[cantidad - 1].idDocumentoExoneracion = item.idDocumentoExoneracion;



                    if (item.TotalLinea < 1 && item.Garantia == false)
                    {
                        throw new Exception("El item con el codigo '" + item.ItemCode + " tiene el total en 0 y no es garantia");
                    }


                    cantidad++;
                }

                await service.Editar(coleccion);

                try
                {
                    var Status = recibido.StatusLlamada;
                    InputLlamada = await serviceLlamada.ObtenerPorId(recibido.idLlamada);
                    InputLlamada.Status = Status;
                    InputLlamada.TipoCaso = recibido.TipoCaso;

                    await serviceLlamada.Editar(InputLlamada);
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
 

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message
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
            catch (Exception ex)
            {


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
