using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace InversionGloblalWeb.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<UsuariosViewModel, int> service;
        private readonly ICrudApi<RolesViewModel, int> roles;
        private readonly ICrudApi<SucursalesViewModel, int> suc;


        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Objeto { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Objeto2 { get; set; }

        [BindProperty]
        public RolesViewModel[] Roles { get; set; }
        [BindProperty]
        public SucursalesViewModel[] Sucursales { get; set; }

        public IndexModel(ICrudApi<UsuariosViewModel, int> service, ICrudApi<RolesViewModel, int> roles, ICrudApi<SucursalesViewModel, int> suc)
        {
            this.service = service;
            this.roles = roles;
            this.suc = suc; 
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // filtro.Codigo1 = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                Objeto = await service.ObtenerLista(filtro);
                Objeto2 = Objeto;
                Roles = await roles.ObtenerLista("");
                Sucursales = await suc.ObtenerLista("");
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
                 

                await service.EliminarUsuario(id);

                return new JsonResult(true);
            }
            catch (ApiException)
            {
                return new JsonResult(false);
            }
        }
    }
}
