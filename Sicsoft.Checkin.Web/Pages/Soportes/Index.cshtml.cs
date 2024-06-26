using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Boletaje.Pages.Soportes
{
    public class IndexModel : PageModel
    {
       
        private readonly ICrudApi<SoportesViewModel, int> service;
        private readonly ICrudApi<UsuariosViewModel, int> usuarios;

        [BindProperty]
        public SoportesViewModel[] Objeto { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<SoportesViewModel, int> service, ICrudApi<UsuariosViewModel, int> usuarios)
        {
            this.service = service;
            this.usuarios = usuarios;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "21").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                 
                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now.Date; 
                    filtro.FechaFinal = DateTime.Now.Date.AddDays(1); 
                   


                }
               
                Objeto = await service.ObtenerLista(filtro);
                Usuarios = await usuarios.ObtenerLista(filtro);

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
