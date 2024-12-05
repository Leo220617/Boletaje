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
    public class ReportesModel : PageModel
    {
        private readonly ICrudApi<EncFacturasViewModel, int> service;
        private readonly ICrudApi<CuentasBancariasViewModel, int> cuentasB; 
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<SucursalesViewModel, int> suc;


        [BindProperty]
        public EncFacturasViewModel[] Objeto { get; set; }

        [BindProperty]
        public CondicionesPagosViewModel[] Condiciones { get; set; }

        [BindProperty]
        public List<MetodosPagosViewModel> MP { get; set; }

        [BindProperty]
        public DateTime FechaR { get; set; }
        [BindProperty]
        public SucursalesViewModel[] Sucursales { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Sucursal { get; set; }
        [BindProperty]
        public CuentasBancariasViewModel[] CuentasBancarias { get; set; }
        public ReportesModel(ICrudApi<EncFacturasViewModel, int> service, ICrudApi<CuentasBancariasViewModel, int> cuentasB, ICrudApi<CondicionesPagosViewModel, int> conds, ICrudApi<SucursalesViewModel, int> suc)
        {
            this.service = service;
            this.cuentasB = cuentasB; 
            this.conds = conds;
            this.suc = suc;
        }

        public async Task<IActionResult> OnGetAsync(DateTime Fecha)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "79").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                CuentasBancarias = await cuentasB.ObtenerLista("");
                Condiciones = await conds.ObtenerLista("");

                FechaR = Fecha;
                DateTime time = new DateTime();

                filtro.FechaInicial = Fecha.Date;
                filtro.FechaFinal = filtro.FechaInicial.Date;

                var Suc = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Sucursal").Select(s1 => s1.Value).FirstOrDefault());
                if (Suc > 0)
                {
                    filtro.Codigo2 = Suc;
                     var  Sucursal1 = await suc.ObtenerPorId(Suc);
                    Sucursal = Sucursal1 == null ? "Sin Sucursal" : Sucursal1.Nombre;
                }
                Objeto = await service.ObtenerLista(filtro);
                MP = new List<MetodosPagosViewModel>();
                foreach(var item in Objeto)
                {
                    foreach(var item2 in item.MetodosPagos)
                    {
                        MP.Add(item2);
                    }
                }


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
