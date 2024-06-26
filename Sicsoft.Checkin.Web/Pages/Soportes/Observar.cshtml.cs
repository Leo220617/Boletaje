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
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<SoportesViewModel, int> service;

        [BindProperty]
        public SoportesViewModel Input { get; set; }
        public ObservarModel(ICrudApi<SoportesViewModel, int> service)
        {
            this.service = service;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                
                Input = await service.ObtenerPorId(id);
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
