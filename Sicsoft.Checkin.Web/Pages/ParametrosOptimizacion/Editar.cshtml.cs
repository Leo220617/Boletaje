using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sicsoft.Checkin.Web.Servicios;
using static Boletaje.Models.HistoricoViewModel;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using Newtonsoft.Json;
using Refit;

namespace Boletaje.Pages.ParametrosOptimizacionSemaforo
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<ParametrosOptimizacionSemaforos, int> service;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;

        [BindProperty]
        public ParametrosOptimizacionSemaforos Parametros { get; set; }

        [BindProperty]
        public StatusViewModel[] Status { get; set; }

        [BindProperty]
        public TiposCasosViewModel[] TiposCasos { get; set; }



        public EditarModel(ICrudApi<ParametrosOptimizacionSemaforos, int> service, ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp)
        {
            this.service = service; 
            this.status = status;
            this.tp = tp;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "88").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Status = await status.ObtenerLista("");
                TiposCasos = await tp.ObtenerLista("");
                var Param = await service.ObtenerLista("");
                if (Param != null)
                {
                    Parametros = Param.FirstOrDefault();
                }
                else
                {
                    throw new Exception("No se encontro el parametro");
                }

               

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
                

                await service.Editar(Parametros);
                return RedirectToPage("./Editar");
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
