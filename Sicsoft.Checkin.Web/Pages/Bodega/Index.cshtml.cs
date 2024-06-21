using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace Boletaje.Pages.Bodega
{
    public class IndexModel : PageModel
    {
    //    private readonly ICrudApi<LlamadasViewModel, int> service;
      //  private readonly ICrudApi<EncReparacionViewModel, int> serviceEnc;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> serviceMov;
        private readonly ICrudApi<TecnicosViewModel, int> serviceT;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<LlamadasViewModel, int> serviceL;


        [BindProperty]
        public BitacoraMovimientosViewModel[] Objeto { get; set; }
        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }
        [BindProperty]

        public StatusViewModel[] Status { get; set; }

        [BindProperty]
        public LlamadasViewModel[] InputLlamada { get; set; }
        public IndexModel(ICrudApi<BitacoraMovimientosViewModel, int> serviceMov,  ICrudApi<TecnicosViewModel, int> serviceT, ICrudApi<StatusViewModel, int> status, ICrudApi<LlamadasViewModel, int> serviceL)
        {
            this.serviceMov = serviceMov;
            this.serviceT = serviceT;
            this.status = status;
            this.serviceL = serviceL;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "25").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Status = await status.ObtenerLista("");
                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now.Date;

                   

                    filtro.FechaFinal = filtro.FechaInicial.AddDays(1);

                    filtro.Codigo3 = 0;
                    filtro.Codigo4 = 0;
                    filtro.Codigo4 = Status.Where(a => a.idSAP == "56").FirstOrDefault() == null ? 0 : Convert.ToInt32(Status.Where(a => a.idSAP == "56").FirstOrDefault().idSAP);
                    if (filtro.Codigo4 != 0)
                    {
                        filtro.seleccionMultiple.Add(filtro.Codigo4);
                    }
                }
                ParametrosFiltros filtro2 = new ParametrosFiltros();
                filtro2.FechaInicial = filtro.FechaInicial.AddMonths(-1);
                filtro2.FechaFinal = filtro.FechaFinal.AddMonths(1);
                InputLlamada = await serviceL.ObtenerLista(filtro2);
                
                filtro.Texto = "";
                foreach (var item in filtro.seleccionMultiple)
                {
                    filtro.Texto += item + "|";
                }
                Objeto = await serviceMov.ObtenerLista(filtro);
                Tecnicos = await serviceT.ObtenerLista("");

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnGetEnviarSAP(string idB)
        {
            try
            {

                var Llamada = new BitacoraMovimientosViewModel();
                Llamada.id = Convert.ToInt32(idB);
                Llamada.Status = "1";

                var objetos = await serviceMov.Agregar(Llamada);




                return new JsonResult(objetos);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());


                return new JsonResult(error);
            }
        }
    }
}
