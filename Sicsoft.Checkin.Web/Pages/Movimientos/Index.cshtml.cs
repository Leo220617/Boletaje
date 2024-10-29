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

namespace Boletaje.Pages.Movimientos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
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
        [BindProperty]
        public int RequiereAprobacion { get; set; }
        public IndexModel(IConfiguration configuration, ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<StatusViewModel, int> status, ICrudApi<LlamadasViewModel, int> serviceL, ICrudApi<TiposCasosViewModel, int> tp)
        {
            this.service = service;
            this.clientes = clientes;
            this.status = status;
            this.serviceL = serviceL;
            this.tp = tp;
            this.configuration = configuration;

        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "34").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                RequiereAprobacion = Convert.ToInt32(configuration["AceptarCotizaciones"].ToString());
                DateTime time = new DateTime();
                Status = await status.ObtenerLista("");
                TP = await tp.ObtenerLista("");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "66").FirstOrDefault()))
                {
                    filtro.FiltroEspecial = true;

                }

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now.AddDays(-7);
                    filtro.FechaFinal = DateTime.Now.AddDays(1);
 
                    if(filtro.FiltroEspecial && filtro.seleccionMultiple.Count == 0)
                    {
                        filtro.Codigo2 = Status.Where(a => a.idSAP == "48").FirstOrDefault() == null ? 0 : Convert.ToInt32(Status.Where(a => a.idSAP == "48").FirstOrDefault().idSAP);
                        if (filtro.Codigo2 != 0)
                        {
                            filtro.seleccionMultiple.Add(filtro.Codigo2);
                        }
                    }


                }
               
                
                   

                    
               

                if (!string.IsNullOrEmpty(filtro.CardCode))
                {
                    filtro.CardCode = filtro.CardCode.Split("/")[0];
                }
                filtro.CardName = "";
                foreach (var item in filtro.seleccionMultiple)
                {
                    filtro.CardName += item + "|";
                }
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

        public async Task<IActionResult> OnGetReenviar(int id, string correos)
        {
            try
            {

                await service.ReenvioFacturas(id, correos);
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                return new JsonResult(false);
            }
        }
        public async Task<IActionResult> OnGetAprobar(int id)
        {
            try
            {

                await service.GenerarAprobacion(id);
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                return new JsonResult(false);
            }
        }

        public async Task<IActionResult> OnGetAprobarSuperior(int id)
        {
            try
            {

                await service.AprobarSuperior(id);
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                return new JsonResult(false);
            }
        }
        public async Task<IActionResult> OnGetEliminar(int id)
        {
            try
            {


                await service.Eliminar(id);

                return new JsonResult(true);
            }
            catch (ApiException ex)
            {
                return new JsonResult(false);
            }
        }

    }


}
