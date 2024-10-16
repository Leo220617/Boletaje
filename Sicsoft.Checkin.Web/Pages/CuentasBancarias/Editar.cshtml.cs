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

namespace Boletaje.Pages.CuentasBancarias
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<CuentasBancariasViewModel, int> service;

        [BindProperty]
        public CuentasBancariasViewModel Input { get; set; }

        public EditarModel(ICrudApi<CuentasBancariasViewModel, int> service)
        {
            this.service = service;


        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "77").FirstOrDefault()))
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

                Input.Estado = true;

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
