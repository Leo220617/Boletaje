using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Boletaje.Pages.PlanificadorDiario
{
    public class IndexModel : PageModel
    {
        private readonly ICrudApi<PlanificadorDiarioViewModel, int> service;
        [BindProperty]
        public PlanificadorDiarioViewModel Planificador { get; set; }

        public IndexModel(ICrudApi<PlanificadorDiarioViewModel, int> service)
        {
            this.service = service;    
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "86").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                

                Planificador = await service.ObtenerListaEspecial("");

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message.ToString());


                return Page();
            }
        }
    }
}
