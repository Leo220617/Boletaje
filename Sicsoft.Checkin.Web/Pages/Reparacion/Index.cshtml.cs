using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace Boletaje.Pages.Reparacion
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<EncReparacionViewModel, int> service;
        private readonly ICrudApi<TecnicosViewModel, int> serviceT;
        private readonly ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion;
        private readonly ICrudApi<LlamadasViewModel, int> serviceL;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;

        private readonly ICrudApi<ProductosViewModel, int> prods;


        [BindProperty]
        public LlamadasViewModel[] InputLlamada { get; set; }
        [BindProperty]

        public StatusViewModel[] Status { get; set; }

        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }
        [BindProperty]
        public EncReparacionViewModel[] Objeto { get; set; }

        [BindProperty]
        public bool bandera { get; set; }

        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        public IndexModel(ICrudApi<EncReparacionViewModel, int> service, ICrudApi<TecnicosViewModel, int> serviceT, ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion, ICrudApi<LlamadasViewModel, int> serviceL, ICrudApi<StatusViewModel, int> status, ICrudApi<ProductosViewModel, int> prods, ICrudApi<TiposCasosViewModel, int> tp)
        {
            this.service = service;
            this.serviceT = serviceT;
            this.serviceColeccion = serviceColeccion;
            this.serviceL = serviceL;
            this.status = status;
            this.prods = prods;
            this.tp = tp;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "21").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                if (!string.IsNullOrEmpty(Roles1.Where(a => a == "22").FirstOrDefault()))
                {
                    filtro.Codigo1 = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "idTecnico").Select(s1 => s1.Value).FirstOrDefault());
                    bandera = false;
                }
                else
                {
                    bandera = true;
                }
                Status = await status.ObtenerLista("");
                TP = await tp.ObtenerLista("");
                DateTime time = new DateTime();
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "66").FirstOrDefault()))
                {
                    filtro.FiltroEspecial = true;

                }
                if (time == filtro.FechaInicial)
                {

                    if (!filtro.FiltroEspecial)
                    {
                        filtro.FechaInicial = DateTime.Now.Date.AddDays(-3);

                        filtro.FechaFinal = DateTime.Now.Date.AddDays(1);
                    }
                    else
                    {
                        //filtro.FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 20);
                        //filtro.FechaFinal = DateTime.Now.AddMonths(1);
                        //filtro.FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        //filtro.FechaFinal = DateTime.Now.AddMonths(1);
                    }

                   


                    filtro.Codigo2 = 1;

                    //filtro.FechaInicial = DateTime.Now.AddDays(-1);
                    //filtro.FechaFinal = filtro.FechaInicial;

                    //if (filtro.FiltroEspecial && filtro.seleccionMultiple.Count == 0)
                    if ( filtro.seleccionMultiple.Count == 0)
                    {
                        filtro.Codigo3 = Status.Where(a => a.idSAP == "46").FirstOrDefault() == null ? 0 : Convert.ToInt32(Status.Where(a => a.idSAP == "46").FirstOrDefault().idSAP);
                        if (filtro.Codigo3 != 0)
                        {
                            filtro.seleccionMultiple.Add(filtro.Codigo3);
                        }

                        filtro.Codigo3 = Status.Where(a => a.idSAP == "47").FirstOrDefault() == null ? 0 : Convert.ToInt32(Status.Where(a => a.idSAP == "47").FirstOrDefault().idSAP);
                        if (filtro.Codigo3 != 0)
                        {
                            filtro.seleccionMultiple.Add(filtro.Codigo3);

                        }
                        filtro.Codigo3 = Status.Where(a => a.idSAP == "45").FirstOrDefault() == null ? 0 : Convert.ToInt32(Status.Where(a => a.idSAP == "45").FirstOrDefault().idSAP);
                        if (filtro.Codigo3 != 0)
                        {
                            filtro.seleccionMultiple.Add(filtro.Codigo3);
                        }
                    }





                }
                filtro.Texto = "";
                foreach (var item in filtro.seleccionMultiple)
                {
                    filtro.Texto += item + "|";
                }
                Objeto = await service.ObtenerLista(filtro);
                var Listado = Objeto.OrderByDescending(a => a.id);
                //ParametrosFiltros filtro2 = new ParametrosFiltros();
                //filtro2.FechaInicial = Listado.LastOrDefault() != null ? Listado.LastOrDefault().FechaCreacion.AddDays(-5) : new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);
                //filtro2.FechaFinal = Listado.FirstOrDefault() != null ? Listado.FirstOrDefault().FechaCreacion.AddDays(5) : filtro.FechaFinal;
                //filtro2.CardName = filtro.Texto;
                //if (filtro.seleccionMultiple.Count > 0)
                //{
                //    foreach (var item in filtro.seleccionMultiple)
                //    {
                //        filtro2.seleccionMultiple.Add(item);
                //    }
                //}

                //InputLlamada = await serviceL.ObtenerLista(filtro2);

                Tecnicos = await serviceT.ObtenerLista("");
                Productos = await prods.ObtenerListaEspecial("");

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

        public async Task<IActionResult> OnGetEstado(int id)
        {
            try
            {
                var EncReparacion = new ColeccionRepuestosViewModel();
                EncReparacion.EncReparacion = new EncReparacionViewModel();
                EncReparacion.EncReparacion.id = id;
                EncReparacion.EncReparacion.Status = 0;

                await serviceColeccion.Editar(EncReparacion);
                return new JsonResult(true);
            }
            catch (Exception)
            {
                return new JsonResult(false);
            }
        }

    }
}
