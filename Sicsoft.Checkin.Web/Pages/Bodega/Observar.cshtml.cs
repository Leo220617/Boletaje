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
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Bodega
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<EncReparacionViewModel, int> service;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<TecnicosViewModel, int> serviceT;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> bt;
        private readonly ICrudApi<ProductosHijosViewModel, int> prodHijos;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> llamada;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<UsuariosViewModel, int> login;
        private readonly ICrudApi<FacturasAprobadasViewModel, int> fa;
        private readonly ICrudApi<SolicitudesComprasViewModel, int> sc;


        [BindProperty]

        public StatusViewModel[] Status { get; set; }
        [BindProperty]
        public BitacoraMovimientosViewModel BTS { get; set; }
        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]
        public string Tecnico { get; set; }
        [BindProperty]
        public EncReparacionViewModel Encabezado { get; set; }
        [BindProperty]
        public string Producto { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijos { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public LlamadasViewModel InputLlamada { get; set; }
        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public SolicitudesComprasViewModel[] SolicitudesCompras { get; set; }
        [BindProperty]
        public string DocNumOfertaAprobada { get; set; }

        [BindProperty]
        public BitacoraMovimientosViewModel[] BTSHistorico { get; set; }

        public ObservarModel(ICrudApi<EncReparacionViewModel, int> service, ICrudApi<ProductosViewModel, int> prods, ICrudApi<TecnicosViewModel, int> serviceT, ICrudApi<BitacoraMovimientosViewModel, int> bt, ICrudApi<ProductosHijosViewModel, int> prodHijos,
            ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> llamada, ICrudApi<StatusViewModel, int> status, ICrudApi<ActividadesViewModel, int> actividades,
            ICrudApi<UsuariosViewModel, int> login, ICrudApi<FacturasAprobadasViewModel, int> fa, ICrudApi<SolicitudesComprasViewModel, int> sc)
        {
            this.service = service;
            this.prods = prods;
            this.serviceT = serviceT;
            this.bt = bt;
            this.prodHijos = prodHijos;
            this.clientes = clientes;
            this.llamada = llamada;
            this.status = status; 
            this.actividades = actividades; 
            this.login = login;
            this.fa = fa;
            this.sc = sc;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "26").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = id;

                Tecnicos = await serviceT.ObtenerLista("");
                BTS = await bt.ObtenerPorId(id);

                Encabezado = await service.ObtenerPorId(BTS.idEncabezado);

                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;

                var ids = Encabezado.idTecnico.ToString();
                Tecnico = Tecnicos.Where(a => a.idSAP == ids).FirstOrDefault().Nombre;

                ProductosHijos = await prodHijos.ObtenerLista("");

                Clientes = await clientes.ObtenerListaEspecial("");
                var Llamada = await llamada.ObtenerPorDocEntry(Encabezado.idLlamada);
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardName;
                Status = await status.ObtenerLista("");

                InputLlamada = Llamada;

                ParametrosFiltros filtro2 = new ParametrosFiltros();
                filtro2.Codigo1 = Llamada.id;
                Actividades = await actividades.ObtenerLista(filtro2);
                Usuarios = await login.ObtenerLista("");

                ParametrosFiltros filtroOFerta = new ParametrosFiltros();
                filtroOFerta.CardCode = Llamada.DocEntry.ToString();
                var facApro = await fa.ObtenerListaEspecial(filtroOFerta);
                DocNumOfertaAprobada = facApro.Oferta.FirstOrDefault() == null ? "" : facApro.Oferta.FirstOrDefault().DocNum;

                ParametrosFiltros filtroSolicitudes = new ParametrosFiltros();
                filtroSolicitudes.Codigo1 = BTS.id;
                SolicitudesCompras = await sc.ObtenerLista(filtroSolicitudes);

                ParametrosFiltros filtroHistoricos = new ParametrosFiltros();
                filtroHistoricos.CardCode = Llamada.DocEntry.ToString();
                BTSHistorico = await bt.ObtenerLista(filtroHistoricos);



                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if(BTS.Status == "1" && BTS.Detalle.Where(a => a.CantidadEnviar > 0 || a.CantidadFaltante < 1).Count() < 1)
                {
                    throw new Exception("No se puede poner Entregado al tecnico sin enviar repuestos");
                }
                await bt.Agregar(BTS);



                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                
                ModelState.AddModelError(string.Empty, ex.Content.ToString());
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = BTS.id;

                Tecnicos = await serviceT.ObtenerLista("");
                BTS = await bt.ObtenerPorId(BTS.id);

                Encabezado = await service.ObtenerPorId(BTS.idEncabezado);

                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;

                var ids = Encabezado.idTecnico.ToString();
                Tecnico = Tecnicos.Where(a => a.idSAP == ids).FirstOrDefault().Nombre;

                ProductosHijos = await prodHijos.ObtenerLista("");

                Clientes = await clientes.ObtenerListaEspecial("");
                var Llamada = await llamada.ObtenerPorDocEntry(Encabezado.idLlamada);
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardName;

                ParametrosFiltros filtroOFerta = new ParametrosFiltros();
                filtroOFerta.CardCode = Llamada.DocEntry.ToString();
                var facApro = await fa.ObtenerListaEspecial(filtroOFerta);
                DocNumOfertaAprobada = facApro.Oferta.FirstOrDefault() == null ? "" : facApro.Oferta.FirstOrDefault().DocNum;
                return Page();
            }
            catch (Exception ex)
            {
                 
                ModelState.AddModelError(string.Empty, ex.Message);

                return Page();
            }
        }
        public async Task<IActionResult> OnPostAgregarBTS(BitacoraMovimientosViewModel recibidos)
        {
            string error = "";


            try
            {
                if (recibidos.Status == "1" && recibidos.Detalle.Where(a => a.CantidadEnviar > 0 || a.CantidadFaltante < 1).Count() < 1)
                {
                    throw new Exception("No se puede poner Entregado al tecnico sin enviar repuestos");
                }

                var resp = await bt.Agregar(recibidos);
                try
                {
                    var Status = recibidos.StatusLlamada;
                    InputLlamada = await llamada.ObtenerPorId(recibidos.idLlamada);
                    InputLlamada.Status = Status;

                    await llamada.Editar(InputLlamada);
                }
                catch (Exception ex)
                {

                }
                var resp2 = new
                {
                    success = true,
                    Exon = resp
                };
                return new JsonResult(resp2);
            }
            catch (ApiException ex)
            {
                Errores errors = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());

                var resp2 = new
                {
                    success = false,
                    Exon = errors.Message
                };
                return new JsonResult(resp2);


            }
            catch (Exception ex)
            {
                var resp2 = new
                {
                    success = false,
                    Exon = ex.Message.ToString()
                };
                return new JsonResult(resp2);
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
