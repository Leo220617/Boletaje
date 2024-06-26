using Boletaje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sicsoft.Checkin.Web.Servicios;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using Refit;
using InversionGloblalWeb.Models;
using Newtonsoft.Json;


namespace Boletaje.Pages.Soportes
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<SoportesViewModel, int> service;

        [BindProperty]
        public SoportesViewModel Input { get; set; }
        public EditarModel(ICrudApi<SoportesViewModel, int> service)
        {
            this.service = service;    
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "69").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Input = await service.ObtenerPorId(id);
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
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

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
