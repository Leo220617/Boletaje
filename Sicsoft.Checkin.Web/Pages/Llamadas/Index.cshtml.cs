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

namespace Boletaje.Pages.Llamadas
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<LlamadasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<StatusViewModel, int> serviceStatus;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<SucursalesViewModel, int> sucursales;

        [BindProperty]

        public SucursalesViewModel[] Sucursales { get; set; }
        [BindProperty]
        public StatusViewModel[] Status { get; set; }
        [BindProperty]
        public LlamadasViewModel[] Objeto { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }
        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        public IndexModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<StatusViewModel, int> serviceStatus, ICrudApi<ProductosViewModel, int> prods, ICrudApi<SucursalesViewModel, int> sucursales)
        {
            this.service = service;
            this.clientes = clientes;
            this.serviceStatus = serviceStatus;
            this.prods = prods;
            this.sucursales = sucursales;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "15").FirstOrDefault()))
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
                Clientes = await clientes.ObtenerListaEspecial("");

                Objeto = await service.ObtenerLista(filtro);
                Status = await serviceStatus.ObtenerLista("");
                Productos = await prods.ObtenerListaEspecial("");
                Sucursales = await sucursales.ObtenerLista("");

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

        ///Reenviar el correo
        ///
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


        //Enviar a SAP

        public async Task<IActionResult> OnGetEnviarSAP(string idB)
        {
            try
            {

                var Llamada = new LlamadasViewModel();
                Llamada.id = Convert.ToInt32(idB);
                var objetos = await service.EnviarSAP(Llamada);

            


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
