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

namespace Boletaje.Pages.Ubicaciones
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<UbicacionesViewModel, int> service;
        private readonly ICrudApi<ProductosViewModel, int> prods;


        [BindProperty]
        public UbicacionesViewModel Input { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        public EditarModel(ICrudApi<UbicacionesViewModel, int> service, ICrudApi<ProductosViewModel, int> prods)
        {
            this.service = service;
            this.prods = prods; 
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "61").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Input = await service.ObtenerPorId(id);
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
