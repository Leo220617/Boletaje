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
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> serviceLlamada;
        private readonly ICrudApi<ErroresViewModel, int> serviceErrores;
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<UsuariosViewModel, int> login; 
        private readonly ICrudApi<ProductosPadresViewModel, int> prodsPadre;

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
        public ObservarModel(ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> serviceLlamada, ICrudApi<ErroresViewModel, int> serviceErrores, ICrudApi<ActividadesViewModel, int> actividades, ICrudApi<UsuariosViewModel, int> login, ICrudApi<ProductosPadresViewModel, int> prodsPadre)
        {
            this.service = service;
            this.clientes = clientes;
            this.serviceLlamada = serviceLlamada;
            this.serviceErrores = serviceErrores;
            this.actividades = actividades;
            this.login = login;
            this.prodsPadre = prodsPadre;
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
