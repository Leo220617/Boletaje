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
using static Boletaje.Models.HistoricoViewModel;

namespace Boletaje.Pages.Facturas
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<EncFacturasViewModel, int> service;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> serviceLlamada;
        private readonly ICrudApi<UsuariosViewModel, int> login;
        private readonly ICrudApi<ProductosPadresViewModel, int> prodsPadre;
        private readonly ICrudApi<ImpuestosViewModel, int> imp;
        private readonly ICrudApi<ExoneracionesViewModel, int> exonera;



        [BindProperty]
        public LlamadasViewModel Llamada { get; set; }
        [BindProperty]
        public EncFacturasViewModel Input { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        [BindProperty]
        public cliente Cliente { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }
        [BindProperty]
        public ProductosPadresViewModel ProductoPadre { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] Productos { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        [BindProperty]
        public ExoneracionesViewModel Exoneraciones { get; set; }

        public ObservarModel(ICrudApi<EncFacturasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> serviceLlamada, ICrudApi<UsuariosViewModel, int> login,
            ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<ProductosPadresViewModel, int> prodsPadre, ICrudApi<ImpuestosViewModel, int> imp, ICrudApi<ExoneracionesViewModel, int> exonera)
        {
            this.service = service;
            this.clientes = clientes;
            this.serviceLlamada = serviceLlamada;
            this.login = login;
            this.service2 = service2;
            this.prodsPadre = prodsPadre;
            this.imp = imp;
            this.exonera = exonera;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {


                Input = await service.ObtenerPorId(id);
                Llamada = await serviceLlamada.ObtenerPorDocEntry(Convert.ToInt32(Input.NumLlamada));
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();
                Impuestos = await imp.ObtenerLista("");
                ParametrosFiltros filtroExo = new ParametrosFiltros();
                filtroExo.CardCode = Cliente.CardCode;
                Exoneraciones = await exonera.ObtenerListaEspecial(filtroExo);

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

                }

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
