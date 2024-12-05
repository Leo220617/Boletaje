using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace Boletaje.Pages.CuentasBancarias
{
    public class IndexModel : PageModel
    {
        private readonly ICrudApi<CuentasBancariasViewModel, int> service;
        private readonly ICrudApi<SucursalesViewModel, int> suc;

        [BindProperty]
        public CuentasBancariasViewModel[] Objeto { get; set; }
        [BindProperty]
        public SucursalesViewModel[] Sucursales { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }
        public IndexModel(ICrudApi<CuentasBancariasViewModel, int> service, ICrudApi<SucursalesViewModel, int> suc)
        {
            this.service = service;
            this.suc = suc;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "76").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Objeto = await service.ObtenerLista(filtro);
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
    }
}
