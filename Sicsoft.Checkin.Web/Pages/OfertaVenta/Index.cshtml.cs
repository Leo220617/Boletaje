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

namespace Boletaje.Pages.OfertaVenta
{
    public class IndexModel : PageModel
    {
        private readonly ICrudApi<OfertaVentaViewModel, int> service;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<ClientesPOrdenesViewModel, int> clientes;

        [BindProperty]
        public OfertaVentaViewModel[] Objeto { get; set; }
        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        [BindProperty]
        public ClientesPOrdenesViewModel Clientes { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<OfertaVentaViewModel, int> service, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<ClientesPOrdenesViewModel, int> clientes)
        {
            this.service = service;
            this.impuestos = impuestos;
            this.clientes = clientes;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "45").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                DateTime time = new DateTime();


                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now;

                    filtro.FechaInicial = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;




                }

                Objeto = await service.ObtenerLista(filtro);
                Clientes = await clientes.ObtenerListaEspecial("");
                Impuestos = await impuestos.ObtenerLista("");

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerar(int id)
        {


            try
            {
                var Orden = new OfertaVentaViewModel();

                Orden.id = id;


                var resp = await service.Agregar(Orden);
                if (resp.ProcesadaSAP)
                {
                    return new JsonResult(true);

                }
                else
                {
                    return new JsonResult(false);

                }
            }
            catch (ApiException ex)
            {

                ModelState.AddModelError(string.Empty, ex.Content.ToString());
                return new JsonResult(ex.Content.ToString());
                //return new JsonResult(false);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                return new JsonResult(ex.Message);
            }
        }
    }
}
