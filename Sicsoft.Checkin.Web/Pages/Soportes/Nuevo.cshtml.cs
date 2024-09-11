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

namespace Boletaje.Pages.Soportes
{
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<SoportesViewModel, int> service;

        public NuevoModel(ICrudApi<SoportesViewModel, int> service)
        {
                this.service = service;
        }

        [BindProperty]
        public SoportesViewModel Soporte { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "68").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                

                return Page();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostGenerar(SoportesViewModel recibido)
        {
            try
            {


                SoportesViewModel coleccion = new SoportesViewModel();
                coleccion.id = 0;
                coleccion.NoBoleta = recibido.NoBoleta;
                coleccion.Asunto = recibido.Asunto;
                coleccion.Mensaje = recibido.Mensaje;
                coleccion.Pantalla = recibido.Pantalla;
                coleccion.Status = "A";
                coleccion.base64 = recibido.base64;
                coleccion.idUsuarioCreador = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());


                await service.Agregar(coleccion);

                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (ApiException ex)
            {

                ModelState.AddModelError(string.Empty, ex.Content.ToString());

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Content.ToString() + " -> " + ex.StackTrace.ToString()
                };
                return new JsonResult(obj);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message + " -> " + ex.StackTrace.ToString()
                };
                return new JsonResult(obj);
            }
        }
    }
}
