using Boletaje.Models;
using Castle.Core.Configuration;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Boletaje.Pages.Impuestos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ImpuestosViewModel, int> service;

        [BindProperty]
        public ImpuestosViewModel[] Objeto { get; set; }


        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros Filtro { get; set; }

        public IndexModel(ICrudApi<ImpuestosViewModel, int> service)
        {
            this.service = service;

        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "37").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Objeto = await service.ObtenerLista(Filtro);

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
