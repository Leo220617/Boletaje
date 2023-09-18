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
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<UsuariosViewModel, int> login;

        [BindProperty]
        public LlamadasViewModel Input { get; set; }

        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }


        public ActividadesModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ActividadesViewModel, int> actividades, ICrudApi<UsuariosViewModel, int> login)
        {
            this.service = service;
            this.actividades = actividades;
            this.login = login;
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
