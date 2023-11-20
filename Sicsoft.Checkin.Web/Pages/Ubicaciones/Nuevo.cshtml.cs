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

namespace Boletaje.Pages.Ubicaciones
{
    public class NuevoModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<UbicacionesViewModel, int> service;
        private readonly ICrudApi<ProductosViewModel, int> prods;


        [BindProperty]
        public UbicacionesViewModel Input { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        public NuevoModel(ICrudApi<UbicacionesViewModel, int> service, ICrudApi<ProductosViewModel, int> prods)
        {
            this.service = service;
            this.prods = prods; 
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "61").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Productos = await prods.ObtenerListaEspecial("");
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
