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
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Llamadas
{
    public class EditarModel : PageModel
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
        private readonly ICrudApi<EncReparacionViewModel, int> serviceE;
        private readonly ICrudApi<HistoricoViewModel, int> historico;


        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public string Producto { get; set; }
        [BindProperty]
        public HistoricoViewModel Historico { get; set; }

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
        public AsuntosViewModel[] Asuntos { get; set; }

        public EditarModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods, ICrudApi<GarantiasViewModel, int> garantias,
            ICrudApi<SucursalesViewModel, int> sucursales, ICrudApi<TecnicosViewModel, int> tecnicos, ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<AsuntosViewModel, int> asuntos, ICrudApi<EncReparacionViewModel, int> serviceE, ICrudApi<HistoricoViewModel, int> historico)
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
            this.serviceE = serviceE;
            this.historico = historico;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "16").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Asuntos = await asuntos.ObtenerLista("");

                Input = await service.ObtenerPorId(id);
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault().CardName;
                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Input.ItemCode).FirstOrDefault().itemName;

                TP = await tp.ObtenerLista("");
                Garantias = await garantias.ObtenerLista("");
                Sucursales = await sucursales.ObtenerLista("");
                Tecnicos = await tecnicos.ObtenerLista("");
                Status = await status.ObtenerLista("");
                ParametrosFiltros filtroE = new ParametrosFiltros();
                filtroE.Codigo4 = Input.id;
                var EncReparacion = await serviceE.ObtenerLista(filtroE);
                var Enc = EncReparacion.FirstOrDefault();

                Input.AdjuntosIdentificacion = Enc.AdjuntosIdentificacion.ToList();
                Input.Adjuntos = Enc.Adjuntos.ToList();

                try
                {
                    ParametrosFiltros filtHist = new ParametrosFiltros();
                    filtHist.CardCode = Input.DocEntry.ToString();
                    Historico = await historico.ObtenerListaEspecial(filtHist);
                }
                catch (Exception ex)
                {

                }

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Input.TratadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
         
                await service.Editar(Input);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerar(LlamadasViewModel recibido)
        {
            try
            {


                LlamadasViewModel coleccion = new LlamadasViewModel();
                coleccion.id = recibido.id;
                coleccion.TipoLlamada = recibido.TipoLlamada;
                coleccion.Status = recibido.Status;
                coleccion.Asunto = recibido.Asunto;
                coleccion.TipoCaso = recibido.TipoCaso;
                coleccion.FechaSISO = recibido.FechaSISO;
                coleccion.LugarReparacion = recibido.LugarReparacion;
                coleccion.SucRecibo = recibido.SucRecibo;
                coleccion.SucRetiro = recibido.SucRetiro;
                coleccion.Comentarios = recibido.Comentarios;
                coleccion.Tecnico = recibido.Tecnico;
              //  coleccion.Firma = recibido.Firma;
                coleccion.Horas = recibido.Horas;

                coleccion.TratadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
            //    coleccion.CardCode = recibido.CardCode.Split("/")[0].Replace(" ", "");
                //var item = recibido.ItemCode;
                //coleccion.ItemCode = item.Split("/")[0].Replace(" ", "");
                //coleccion.SerieFabricante = item.Split("/")[1].Replace(" ", "");
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



                await service.Editar(coleccion);

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
