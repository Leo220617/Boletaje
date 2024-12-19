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

namespace Boletaje.Pages.Facturas
{
    public class IndexModel : PageModel
    {
        private readonly ICrudApi<EncFacturasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<SucursalesViewModel, int> suc;


        [BindProperty]
        public EncFacturasViewModel[] Objeto { get; set; }
        [BindProperty]
        public SucursalesViewModel[] Sucursales { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<EncFacturasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<SucursalesViewModel, int> suc)
        {
            this.service = service;
            this.clientes = clientes;
            this.suc = suc;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "75").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Sucursales = await suc.ObtenerLista("");
                DateTime time = new DateTime();
                 
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

                    

                }
                 

                if (!string.IsNullOrEmpty(filtro.CardCode))
                {
                    filtro.CardCode = filtro.CardCode.Split("/")[0];
                }
                filtro.CardName = "";
                var Suc = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Sucursal").Select(s1 => s1.Value).FirstOrDefault());
                if (Suc > 0)
                {
                    filtro.Codigo2 = Suc;
                }
                Objeto = await service.ObtenerLista(filtro);  

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
