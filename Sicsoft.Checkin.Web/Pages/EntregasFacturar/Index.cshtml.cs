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

namespace Boletaje.Pages.EntregasFacturar
{
    public class IndexModel : PageModel
    {
        
        private readonly ICrudApi<EncMovimientoViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<LlamadasViewModel, int> serviceL;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;

        [BindProperty]
        public LlamadasViewModel[] InputLlamada { get; set; }
        [BindProperty]

        public StatusViewModel[] Status { get; set; }

        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }

        [BindProperty]
        public EncMovimientoViewModel[] Objeto { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }
 
        public IndexModel(  ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<StatusViewModel, int> status, ICrudApi<LlamadasViewModel, int> serviceL, ICrudApi<TiposCasosViewModel, int> tp)
        {
            this.service = service;
            this.clientes = clientes;
            this.status = status;
            this.serviceL = serviceL;
            this.tp = tp;
        

        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "73").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
         
                DateTime time = new DateTime();
                Status = await status.ObtenerLista("");
                TP = await tp.ObtenerLista("");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "66").FirstOrDefault()))
                {
                    filtro.FiltroEspecial = true;

                }
                filtro.Codigo1 = 2;
                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now;

                    filtro.FechaInicial = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;

                    filtro.NoFacturado = true;

                }
                filtro.FiltrarFacturado = true;



                filtro.DocEntryGenerado = 1;


                if (!string.IsNullOrEmpty(filtro.CardCode))
                {
                    filtro.CardCode = filtro.CardCode.Split("/")[0];
                }
                filtro.CardName = "";
                
                

                Objeto = await service.ObtenerLista(filtro);


                //InputLlamada = await serviceL.ObtenerLista("");

                Clientes = await clientes.ObtenerListaEspecial("");

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
