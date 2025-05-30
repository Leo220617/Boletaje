using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using Castle.DynamicProxy.Generators;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Llamadas
{
    public class ObservarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<LlamadasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<GarantiasViewModel, int> garantias;
        private readonly ICrudApi<SucursalesViewModel, int> sucursales;
        private readonly ICrudApi<TecnicosViewModel, int> tecnicos;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;
        private readonly ICrudApi<AsuntosViewModel, int> asuntos;
        private readonly ICrudApi<EncReparacionViewModel, int> serviceE;
        private readonly ICrudApi<HistoricoViewModel, int> historico;
        private readonly ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado;

        private readonly ICrudApi<UbicacionesViewModel, int> ubicaciones;
        private readonly ICrudApi<LogModificacionesViewModel, int> logs;

        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public string Producto { get; set; }

        [BindProperty]
        public string UbicacionProd { get; set; }
        [BindProperty]
        public HistoricoViewModel Historico { get; set; }

        [BindProperty]
        public HistoricoDetalladoViewModel HistoricoDetallado { get; set; }
        [BindProperty]
        public LlamadasViewModel Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        [BindProperty]

        public GarantiasViewModel[] Garantias { get; set; }

        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }


        [BindProperty]

        public SucursalesViewModel[] Sucursales { get; set; }

        [BindProperty]

        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]

        public StatusViewModel[] Status { get; set; }
        [BindProperty]
        public AsuntosViewModel[] Asuntos { get; set; }

        [BindProperty]
        public UbicacionesViewModel[] Ubicaciones { get; set; }

        [BindProperty]
        public LogModificacionesViewModel[] Logs { get; set; }

        public ObservarModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods, ICrudApi<GarantiasViewModel, int> garantias,
            ICrudApi<SucursalesViewModel, int> sucursales, ICrudApi<TecnicosViewModel, int> tecnicos, ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<AsuntosViewModel, int> asuntos, ICrudApi<EncReparacionViewModel, int> serviceE,
            ICrudApi<HistoricoViewModel, int> historico, ICrudApi<UbicacionesViewModel, int> ubicaciones, ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado, ICrudApi<LogModificacionesViewModel, int> logs)
        {
            this.service = service;
            this.clientes = clientes;
            this.prods = prods;
            this.garantias = garantias;
            this.sucursales = sucursales;
            this.tecnicos = tecnicos;
            this.status = status;
            this.tp = tp;
            this.asuntos = asuntos;
            this.serviceE = serviceE;
            this.historico = historico;
            this.ubicaciones = ubicaciones;
            this.historicoDetallado = historicoDetallado;
            this.logs = logs;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                //if (string.IsNullOrEmpty(Roles1.Where(a => a == "16").FirstOrDefault()))
                //{
                //    return RedirectToPage("/NoPermiso");
                //}
                Asuntos = await asuntos.ObtenerLista("");

                Input = await service.ObtenerPorId(id);
                var filtro = new ParametrosFiltros();
                filtro.Codigo1 = id;
                Logs = await logs.ObtenerLista(filtro);
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault().CardName;
                ParametrosFiltros filtUbi = new ParametrosFiltros();
                filtUbi.Texto = Input.ItemCode;
                Ubicaciones = await ubicaciones.ObtenerLista(filtUbi);
                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Input.ItemCode && a.manufSN == Input.SerieFabricante).FirstOrDefault().itemName;
                UbicacionProd = (Ubicaciones.FirstOrDefault() != null ? Ubicaciones.FirstOrDefault().Ubicacion : " ");
                TP = await tp.ObtenerLista("");
                Garantias = await garantias.ObtenerLista("");
                Sucursales = await sucursales.ObtenerLista("");
                Tecnicos = await tecnicos.ObtenerLista("");
                Status = await status.ObtenerLista("");
                ParametrosFiltros filtroE = new ParametrosFiltros();
                filtroE.Codigo4 = Input.id;
                var EncReparacion = await serviceE.ObtenerLista(filtroE);
                var Enc = EncReparacion.FirstOrDefault();

                Input.AdjuntosIdentificacion = Enc.AdjuntosIdentificacion.ToList();
                Input.Adjuntos = Enc.Adjuntos.ToList();

                try
                {
                    ParametrosFiltros filtHist = new ParametrosFiltros();
                    filtHist.CardCode = Input.DocEntry.ToString();
                    Historico = await historico.ObtenerListaEspecial(filtHist);
                }
                catch (Exception ex)
                {
                    Historico = new HistoricoViewModel();
                    Historico.Historico = new HistoricoViewModel.historico[1];
                    Historico.Historico[0] = new HistoricoViewModel.historico();
                    Historico.Historico[0].Total_Revisiones = 0;
                    Historico.Historico[0].Cantidad_Revisiones = 0;

                }
                try
                {
                    ParametrosFiltros filtHistDet = new ParametrosFiltros();
                    filtHistDet.CardCode = Input.SerieFabricante.ToString();
                    filtHistDet.CardName = Input.ItemCode.ToString();
                    HistoricoDetallado = await historicoDetallado.ObtenerListaEspecial(filtHistDet);
                    if (HistoricoDetallado.Historico == null)
                    {
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

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
