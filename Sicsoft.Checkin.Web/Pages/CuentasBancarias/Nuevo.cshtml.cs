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

namespace Boletaje.Pages.CuentasBancarias
{
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<CuentasBancariasViewModel, int> service;
        private readonly ICrudApi<SucursalesViewModel, int> suc;


        [BindProperty]
        public CuentasBancariasViewModel Input { get; set; }

        [BindProperty]
        public SucursalesViewModel[] Sucursales { get; set; }

        public NuevoModel(ICrudApi<CuentasBancariasViewModel, int> service, ICrudApi<SucursalesViewModel, int> suc)
        {
            this.service = service;
            this.suc = suc;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "77").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
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
                Input.Estado = true;
                await service.Agregar(Input);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
