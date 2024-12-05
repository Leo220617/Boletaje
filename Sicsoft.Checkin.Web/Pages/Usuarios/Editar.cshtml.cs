using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace InversionGloblalWeb.Pages.Usuarios
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<UsuariosViewModel, int> service;
        private readonly ICrudApi<RolesViewModel, int> roles;
        private readonly ICrudApi<SucursalesViewModel, int> suc;

        [BindProperty]
        public UsuariosViewModel Input { get; set; }

        [BindProperty]
        public RolesViewModel[] Roles { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public SucursalesViewModel[] Sucursales { get; set; }
        public EditarModel(ICrudApi<UsuariosViewModel, int> service, ICrudApi<RolesViewModel, int> roles, ICrudApi<SucursalesViewModel, int> suc)
        {
            this.service = service;
            this.roles = roles;
            this.suc = suc;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Roles = await roles.ObtenerLista("");
                
                Input = await service.ObtenerPorId(id);
                Sucursales = await suc.ObtenerLista("");
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
                
                
                await service.Editar(Input);
                return Redirect("../Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                //return Redirect("/Error");
                return Page();
            }
        }
    }
}
