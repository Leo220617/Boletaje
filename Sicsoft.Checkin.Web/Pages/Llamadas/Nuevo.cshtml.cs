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

namespace Boletaje.Pages.Llamadas
{
    public class NuevoModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<LlamadasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<GarantiasViewModel, int> garantias;
        private readonly ICrudApi<SucursalesViewModel, int> sucursales;
        private readonly ICrudApi<TecnicosViewModel, int> tecnicos;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;
        private readonly ICrudApi<AsuntosViewModel, int> asuntos;



        [BindProperty]
        public LlamadasViewModel Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        [BindProperty]

        public GarantiasViewModel[] Garantias { get; set; }

        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }


        [BindProperty]

        public SucursalesViewModel[] Sucursales { get; set; }

        [BindProperty]

        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]

        public StatusViewModel[] Status { get; set; }


        [BindProperty]
        public AdjuntosViewModel[] Adjuntos { get; set; }
        [BindProperty]
        public AdjuntosIdentificacionViewModel[] AdjuntosIdentificacion { get; set; }

        [BindProperty]
        public AsuntosViewModel[] Asuntos { get; set; }

        public NuevoModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods, ICrudApi<GarantiasViewModel, int> garantias,
            ICrudApi<SucursalesViewModel, int> sucursales, ICrudApi<TecnicosViewModel, int> tecnicos, ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<AsuntosViewModel, int> asuntos)
        {
            this.service = service;
            this.clientes = clientes;
            this.prods = prods;
            this.garantias = garantias;
            this.sucursales = sucursales;
            this.tecnicos = tecnicos;
            this.status = status;
            this.tp = tp;
            this.asuntos = asuntos;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "16").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Asuntos = await asuntos.ObtenerLista("");
                Clientes = await clientes.ObtenerListaEspecial("");
                // Productos = await prods.ObtenerListaEspecial("");
                TP = await tp.ObtenerLista("");
                Garantias = await garantias.ObtenerLista("");
                Sucursales = await sucursales.ObtenerLista("");
                Tecnicos = await tecnicos.ObtenerLista("");
                Status = await status.ObtenerLista("");
                Input = new LlamadasViewModel();
                Input.Horas = 0;
                Productos = await prods.ObtenerListaEspecial("");
                return Page();
            }
            catch (ApiException ex)
            {



                return new JsonResult(ex.Content.ToString());
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
        //Mandar Productos por cliente
        public async Task<IActionResult> OnGetProductos(string idB)
        {
            try
            {


                //var ids = Convert.ToInt32(id);
                idB = idB.Replace(" ", "");




                var objetos = await prods.ObtenerListaEspecial("");
                var objeto = objetos.Productos.ToList();
                if (idB != "0")
                {
                     objeto = objetos.Productos.Where(a => a.customer.ToString().Contains(idB.ToUpper())
               ).ToList();
                }






                return new JsonResult(objeto);
            }
            catch (ApiException ex)
            {



                return new JsonResult(ex.Content.ToString());
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message.ToString());
            }
        }


        public async Task<IActionResult> OnGetClientes(string idB)
        {
            try
            {


                //var ids = Convert.ToInt32(id);
                if (idB != "0")
                {
                    var itemCode = idB.Split("/")[0].Replace(" ", "");
                    idB = idB.Split("/")[1];
                    idB = idB.Replace(" ", "");


                    var prod = await prods.ObtenerListaEspecial("");
                    var prod1 = prod.Productos.Where(a => a.manufSN == idB && a.itemCode == itemCode).ToList();
                    var objetos = await clientes.ObtenerListaEspecial("");


                    var objeto = new List<cliente>();

                    foreach (var item in prod1)
                    {
                        objeto.Add(objetos.Clientes.Where(a => a.CardCode.ToString().Contains(item.customer)
                    ).FirstOrDefault());
                    }





                    return new JsonResult(objeto);
                }
                else
                {
                  


                    var prod = await prods.ObtenerListaEspecial("");
                    var prod1 = prod.Productos.ToList();
                    var objetos = await clientes.ObtenerListaEspecial("");


                    var objeto = new List<cliente>();

                    foreach (var item in prod1)
                    {
                        objeto.Add(objetos.Clientes.Where(a => a.CardCode.ToString().Contains(item.customer)
                    ).FirstOrDefault());
                    }





                    return new JsonResult(objeto);
                }
                 
            }
            catch (ApiException ex)
            {



                return new JsonResult(ex.Content.ToString());
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message.ToString());
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Input.TratadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                Input.CardCode = Input.CardCode.Split("/")[0].Replace(" ", "");
                var item = Input.ItemCode;
                Input.ItemCode = item.Split("/")[0].TrimEnd();
                Input.SerieFabricante = item.Split("/")[1].TrimEnd();
                await service.Agregar(Input);


                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }

            catch (Exception ex)
            {



                return new JsonResult(ex.Message.ToString());
            }
        }

        //Experimento de mandar los adjuntos por la llamada

        public async Task<IActionResult> OnPostGenerar(LlamadasViewModel recibido)
        {
            try
            {


                LlamadasViewModel coleccion = new LlamadasViewModel();
                coleccion.id = 0;
                coleccion.TipoLlamada = recibido.TipoLlamada;
                coleccion.Status = recibido.Status;
                coleccion.Asunto = recibido.Asunto;
                coleccion.TipoCaso = recibido.TipoCaso;
                coleccion.FechaSISO = null;
                coleccion.LugarReparacion = recibido.LugarReparacion;
                coleccion.SucRecibo = recibido.SucRecibo;
                coleccion.SucRetiro = recibido.SucRetiro;
                coleccion.Comentarios = recibido.Comentarios;
                coleccion.Tecnico = recibido.Tecnico;
                coleccion.Firma = recibido.Firma;
                coleccion.Horas = recibido.Horas;

                coleccion.TratadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                coleccion.CardCode = recibido.CardCode.Split("/")[0].Replace(" ", "");
                var item = recibido.ItemCode;
                coleccion.ItemCode = item.Split("/")[0].Replace(" ", "");
                coleccion.SerieFabricante = item.Split("/")[1].Replace(" ", "");
                coleccion.PersonaContacto = recibido.PersonaContacto;
                coleccion.EmailPersonaContacto = recibido.EmailPersonaContacto;
                coleccion.NumeroPersonaContacto = recibido.NumeroPersonaContacto;
                coleccion.Adjuntos = new List<AdjuntosViewModel>();
                coleccion.AdjuntosIdentificacion = new List<AdjuntosIdentificacionViewModel>();


                if (recibido.Adjuntos != null)
                {
                    var cantidad = 1;
                    foreach (var item1 in recibido.Adjuntos)
                    {
                        var adjunto = new AdjuntosViewModel();
                        adjunto.base64 = item1.base64;
                        coleccion.Adjuntos.Add(adjunto);
                        cantidad++;
                    }
                }
                if (recibido.AdjuntosIdentificacion != null)
                {
                    var cantidad = 1;
                    foreach (var item1 in recibido.AdjuntosIdentificacion)
                    {
                        var adjunto = new AdjuntosIdentificacionViewModel();
                        adjunto.base64 = item1.base64;
                        coleccion.AdjuntosIdentificacion.Add(adjunto);
                        cantidad++;
                    }
                }



                await service.Agregar(coleccion);

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


    }
}
