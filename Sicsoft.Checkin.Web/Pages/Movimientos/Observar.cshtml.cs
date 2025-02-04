using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sicsoft.Checkin.Web.Servicios;

namespace Boletaje.Pages.Movimientos
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<EncMovimientoViewModel, int> service;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> serviceLlamada;
        private readonly ICrudApi<ErroresViewModel, int> serviceErrores;
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<UsuariosViewModel, int> login; 
        private readonly ICrudApi<ProductosPadresViewModel, int> prodsPadre;
        private readonly ICrudApi<UbicacionesViewModel, int> ubicaciones;
        private readonly ICrudApi<HistoricoViewModel, int> historico;
        private readonly ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;
        private readonly ICrudApi<TecnicosViewModel, int> tecnicos;
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<GarantiasViewModel, int> garan;



        [BindProperty]
        public LlamadasViewModel Llamada { get; set; }
        [BindProperty]
        public EncMovimientoViewModel Input { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        [BindProperty]
        public cliente Cliente { get; set; }

        [BindProperty]
        public ErroresViewModel[] Errores { get; set; }
        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }
        [BindProperty]
        public ProductosPadresViewModel ProductoPadre { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] Productos { get; set; }

        [BindProperty]
        public UbicacionesViewModel[] Ubicaciones { get; set; }

        [BindProperty]
        public string UbicacionProd { get; set; }
        [BindProperty]
        public HistoricoViewModel Historico { get; set; }

        [BindProperty]
        public HistoricoDetalladoViewModel HistoricoDetallado { get; set; }
    

        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }

        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]
        public CondicionesPagosViewModel[] Condiciones { get; set; }
        [BindProperty]
        public GarantiasViewModel[] Garantias { get; set; }
        public ObservarModel(ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> serviceLlamada, ICrudApi<ErroresViewModel, int> serviceErrores, ICrudApi<ActividadesViewModel, int> actividades, ICrudApi<UsuariosViewModel, int> login, 
            ICrudApi<ProductosPadresViewModel, int> prodsPadre, ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<UbicacionesViewModel, int> ubicaciones, 
            ICrudApi<HistoricoViewModel, int> historico, ICrudApi<HistoricoDetalladoViewModel, int> historicoDetallado, ICrudApi<DiagnosticosViewModel, int> serviceD, ICrudApi<TecnicosViewModel, int> tecnicos
            , ICrudApi<CondicionesPagosViewModel, int> conds, ICrudApi<GarantiasViewModel, int> garan)
        {
            this.service = service;
            this.clientes = clientes;
            this.serviceLlamada = serviceLlamada;
            this.serviceErrores = serviceErrores;
            this.actividades = actividades;
            this.login = login;
            this.prodsPadre = prodsPadre;
            this.service2 = service2;
            this.ubicaciones = ubicaciones;
            this.historico = historico;
            this.historicoDetallado = historicoDetallado;
            this.serviceD = serviceD;
            this.tecnicos = tecnicos;
            this.conds = conds;
            this.garan = garan;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                

                Input = await service.ObtenerPorId(id);
                Llamada = await serviceLlamada.ObtenerPorDocEntry(Convert.ToInt32(Input.NumLlamada));
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();
                Errores = await serviceErrores.ObtenerLista("");
                Diagnosticos = await serviceD.ObtenerLista("");
                Garantias = await garan.ObtenerLista("");
                ParametrosFiltros filtro2 = new ParametrosFiltros();
                filtro2.Codigo1 = Llamada.id;
                Actividades = await actividades.ObtenerLista(filtro2);
                Usuarios = await login.ObtenerLista("");
                var ProductosPadres = await prodsPadre.ObtenerLista("");
                ProductoPadre = ProductosPadres.Where(a => a.codSAP == Llamada.ItemCode).FirstOrDefault();
                if (ProductoPadre == null)
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
                Condiciones = await conds.ObtenerLista("");
                Tecnicos = await tecnicos.ObtenerLista("");
                var tec = Llamada.Tecnico.ToString();
                Tecnicos = Tecnicos != null ? Tecnicos.Where(a => a.idSAP == tec).ToArray() : Tecnicos;

                Productos = await service2.ObtenerLista("");
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
