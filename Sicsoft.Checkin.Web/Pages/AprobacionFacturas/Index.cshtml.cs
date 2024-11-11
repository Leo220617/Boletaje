using Boletaje.Models;
using Castle.Core.Configuration;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Boletaje.Pages.AprobacionFacturas
{
    public class IndexModel : PageModel
    {
        private readonly ICrudApi<AprobacionesFacturasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<UsuariosViewModel, int> usuarios;


        [BindProperty]
        public AprobacionesFacturasViewModel[] Objeto { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        [BindProperty]
        public ProductosViewModel Productos { get; set; }
        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<AprobacionesFacturasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods, ICrudApi<UsuariosViewModel, int> usuarios)
        {
            this.service = service;
            this.clientes = clientes;
            this.prods = prods;
            this.usuarios = usuarios;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "82").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                var time = new DateTime();
                Usuarios = await usuarios.ObtenerLista("");
                if(filtro.FechaInicial == time)
                {
                    filtro.NoFacturado = false;

                    filtro.FechaInicial = DateTime.Now.Date;

                    filtro.FechaFinal = filtro.FechaInicial.AddDays(1).AddMinutes(-1);

                }

                Clientes = await clientes.ObtenerListaEspecial("");
                Productos = await prods.ObtenerListaEspecial("");

                Objeto = await service.ObtenerLista(filtro);

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
        public async Task<IActionResult> OnGetEliminar(int id)
        {
            try
            {

                var idLogin = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());
                await service.AprobarFactura(id,idLogin);

                return new JsonResult(true);
            }
            catch (ApiException)
            {
                return new JsonResult(false);
            }
        }
    }
}
