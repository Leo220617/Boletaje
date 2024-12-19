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
using static Boletaje.Models.HistoricoViewModel;

namespace Boletaje.Pages.Llamadas
{
    public class ActividadesModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<LlamadasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;

        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<UsuariosViewModel, int> login;

        private readonly ICrudApi<EncMovimientoViewModel, int> movimientos;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;
        private readonly ICrudApi<ErroresViewModel, int> serviceError;


        [BindProperty]
        public LlamadasViewModel Input { get; set; }

        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public string Errores { get; set; }

        [BindProperty]
        public string ComentariosGenerales { get; set; }


        public ActividadesModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ActividadesViewModel, int> actividades, ICrudApi<UsuariosViewModel, int> login, ICrudApi<ClientesViewModel, int> clientes,
            ICrudApi<EncMovimientoViewModel, int> movimientos, ICrudApi<DiagnosticosViewModel, int> serviceD, ICrudApi<ErroresViewModel, int> serviceError)
        {
            this.service = service;
            this.actividades = actividades;
            this.login = login;
            this.clientes = clientes;
            this.movimientos = movimientos;
            this.serviceD = serviceD;
            this.serviceError = serviceError;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "57").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
               

                Input = await service.ObtenerPorId(id);
                 
                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = id;
                Actividades = await actividades.ObtenerLista(filtro);
                Usuarios = await login.ObtenerLista("");
                Clientes = await clientes.ObtenerListaEspecial("");

                ParametrosFiltros filtroMov = new ParametrosFiltros();
                filtroMov.FiltroEspecial = true;
                filtroMov.Texto = Input.DocEntry.ToString();
                var Movimientos = await movimientos.ObtenerLista(filtroMov); 
                var Error = await serviceError.ObtenerLista("");

                if (Movimientos.Where(a => a.TipoMovimiento == 2).OrderByDescending(a => a.id).FirstOrDefault() != null)
                {
                    var Cotizacion = Movimientos.Where(a => a.TipoMovimiento == 2).OrderByDescending(a => a.id).FirstOrDefault();
                    ComentariosGenerales = Cotizacion.Comentarios;
                    foreach (var item in Cotizacion.Detalle)
                    {
                        Errores += (Error.Where(a => a.id == item.idError).FirstOrDefault() != null ? Error.Where(a => a.id == item.idError).FirstOrDefault().Descripcion + " -> " + Error.Where(a => a.id == item.idError).FirstOrDefault().Diagnostico +" \n" : "") ;
                    }
                }
                else if(Movimientos.Where(a => a.TipoMovimiento == 3 && a.Aprobada == true).FirstOrDefault() != null)
                {
                    var Cotizacion = Movimientos.Where(a => a.TipoMovimiento == 3 && a.Aprobada == true).FirstOrDefault();
                    ComentariosGenerales = Cotizacion.Comentarios;

                    foreach (var item in Cotizacion.Detalle)
                    {
                        Errores += (Error.Where(a => a.id == item.idError).FirstOrDefault() != null ? Error.Where(a => a.id == item.idError).FirstOrDefault().Descripcion + " -> " + Error.Where(a => a.id == item.idError).FirstOrDefault().Diagnostico + " \n" : "") + " \n";
                    }
                }else if (Movimientos.Where(a => a.TipoMovimiento == 3).OrderByDescending(a => a.id).FirstOrDefault() != null)
                {
                    var Cotizacion = Movimientos.Where(a => a.TipoMovimiento == 3).OrderByDescending(a => a.id).FirstOrDefault();
                    ComentariosGenerales = Cotizacion.Comentarios;

                    foreach (var item in Cotizacion.Detalle)
                    {
                        Errores += (Error.Where(a => a.id == item.idError).FirstOrDefault() != null ? Error.Where(a => a.id == item.idError).FirstOrDefault().Descripcion + " -> " + Error.Where(a => a.id == item.idError).FirstOrDefault().Diagnostico + " \n" : "") + " \n";
                    }
                }



                    return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerar(ActividadesViewModel recibido)
        {
            try
            {


                ActividadesViewModel coleccion = new ActividadesViewModel();
                coleccion.id = 0;
                coleccion.idLlamada = recibido.idLlamada;
                coleccion.TipoActividad = recibido.TipoActividad;
                coleccion.Detalle = recibido.Detalle;
                coleccion.UsuarioCreador = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                coleccion.idLogin = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());


                await actividades.Agregar(coleccion);

                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (ApiException ex)
            {


                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Content.ToString()
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



        public async Task<IActionResult> OnGetEnviarSAP(string idB)
        {
            try
            {

                var Actividad = new ActividadesViewModel();
                Actividad.id = Convert.ToInt32(idB);
                var objetos = await actividades.EnviarSAP(Actividad);




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
