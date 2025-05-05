using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.IO.Compression;
using System.IO;

namespace Boletaje.Pages.PlanificadorDiario
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<PlanificadorDiarioViewModel, int> service;
        private readonly ICrudApi<LlamadasViewModel, int> serviceLl;
        private readonly ICrudApi<ActividadesViewModel, int> actividades;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;
        private readonly ICrudApi<UsuariosViewModel, int> login;

        [BindProperty]
        public PlanificadorDiarioViewModel Planificador { get; set; }
        [BindProperty]
        public LlamadasViewModel Llamada { get; set; }
        [BindProperty]
        public ActividadesViewModel[] Actividades { get; set; }

        [BindProperty]
        public StatusViewModel[] Status { get; set; }

        [BindProperty]
        public TiposCasosViewModel[] TP { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        public ObservarModel(ICrudApi<PlanificadorDiarioViewModel, int> service, ICrudApi<LlamadasViewModel, int> serviceLl, ICrudApi<ActividadesViewModel, int> actividades,
            ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<UsuariosViewModel, int> login)
        {
            this.service = service;
            this.serviceLl = serviceLl;
            this.actividades = actividades;
            this.status = status;
            this.tp = tp;
            this.login = login;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "87").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Usuarios = await login.ObtenerLista("");

                Status = await status.ObtenerLista("");
                TP = await tp.ObtenerLista("");
                Planificador = await service.ObtenerListaEspecial("");
                Planificador.Planificador = Planificador.Planificador.Where(a => a.callID == id).ToArray();

                Llamada = await serviceLl.ObtenerPorDocEntry(id);
                ParametrosFiltros filtro2 = new ParametrosFiltros();
                filtro2.Codigo1 = Llamada.id;
                Actividades = await actividades.ObtenerLista(filtro2);
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
        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            try
            {
                RecibidoMovimientos recibido = new RecibidoMovimientos();
                var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);

                byte[] compressedData = ms.ToArray();

                // Descomprimir los datos utilizando GZip
                using (var compressedStream = new MemoryStream(compressedData))
                using (var decompressedStream = new MemoryStream())
                {
                    using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }

                    // Convertir los datos descomprimidos a una cadena JSON
                    var jsonString = System.Text.Encoding.UTF8.GetString(decompressedStream.ToArray());

                    // Procesar la cadena JSON como desees
                    // Por ejemplo, puedes deserializarla a un objeto C# utilizando Newtonsoft.Json
                    recibido = Newtonsoft.Json.JsonConvert.DeserializeObject<RecibidoMovimientos>(jsonString);
                }


                var Status = recibido.StatusLlamada;
                Llamada = await serviceLl.ObtenerPorId(recibido.idLlamada);
                Llamada.Status = Status;
                Llamada.TipoCaso = recibido.TipoCaso;
                Llamada.FechaProximoContacto = recibido.Fecha;
                Llamada.BorrarFechaProximoContacto = recibido.BorrarFechaProximoContacto;
                await serviceLl.Editar(Llamada);


                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (ApiException ex)
            {

                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + error.Message
                };
                return new JsonResult(obj);
            }
            catch (Exception ex)
            {


                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message
                };
                return new JsonResult(obj);
            }
        }
        public async Task<IActionResult> OnPostGenerarActividad(ActividadesViewModel recibido)
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

                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + error.Message
                };
                return new JsonResult(obj);
            }
            catch (Exception ex)
            {



                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message + " -> " + ex.StackTrace.ToString()
                };
                return new JsonResult(obj);
            }
        }

        public async Task<IActionResult> OnGetEnviarActividadSAP(string idB)
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
            catch (Exception ex)
            {


                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message
                };
                return new JsonResult(obj);
            }
        }
    }
}
