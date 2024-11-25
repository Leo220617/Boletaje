using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
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
using Microsoft.AspNetCore.Mvc.ApiExplorer;

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
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<AprobacionesFacturasViewModel, int> aprobacion;




        [BindProperty]
        public LlamadasViewModel Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ClientesViewModel ClientesFac { get; set; }

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

        [BindProperty]
        public CondicionesPagosViewModel[] CondicionesPagos { get; set; }

        [BindProperty]
        public string EmpiezaPor { get; set; }



        public NuevoModel(IConfiguration configuration, ICrudApi<LlamadasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods, ICrudApi<GarantiasViewModel, int> garantias,
            ICrudApi<SucursalesViewModel, int> sucursales, ICrudApi<TecnicosViewModel, int> tecnicos, ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp,
            ICrudApi<AsuntosViewModel, int> asuntos, ICrudApi<CondicionesPagosViewModel, int> conds, ICrudApi<AprobacionesFacturasViewModel, int> aprobacion)
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
            this.configuration = configuration;
            this.conds = conds;
            this.aprobacion = aprobacion;
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

                EmpiezaPor = configuration["BusquedaPor"].ToString();
                CondicionesPagos = await conds.ObtenerLista("");

                Asuntos = await asuntos.ObtenerLista("");
                //
                // Productos = await prods.ObtenerListaEspecial("");
                TP = await tp.ObtenerLista("");
                Garantias = await garantias.ObtenerLista("");
                Sucursales = await sucursales.ObtenerLista("");
                Tecnicos = await tecnicos.ObtenerLista("");
                Status = await status.ObtenerLista("");
                Input = new LlamadasViewModel();
                Input.Horas = 0;
                ClientesFac = await clientes.ObtenerListaEspecial("");
                if (EmpiezaPor == "C")
                {
                    Clientes = await clientes.ObtenerListaEspecial("");
                    Productos = new ProductosViewModel();
                    Productos.Productos = new Producto[0];
                }
                else if (EmpiezaPor == "P")
                {
                    Clientes = new ClientesViewModel();
                    Clientes.Clientes = new cliente[0];

                    Productos = await prods.ObtenerListaEspecial("");

                }



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
                    idB = idB.TrimEnd().TrimStart();//.Replace(" ", "");


                    var prod = await prods.ObtenerListaEspecial("");
                    var prod1 = prod.Productos.Where(a => a.manufSN == idB && a.itemCode == itemCode).ToList(); //
                    var objetos = await clientes.ObtenerListaEspecial("");


                    var objeto = new List<cliente>();

                    foreach (var item in prod1)
                    {
                        objeto.Add(objetos.Clientes.Where(a => a.CardCode.ToString() == item.customer).FirstOrDefault());
                    }





                    return new JsonResult(objeto);
                }
                else
                {



                    var prod = await prods.ObtenerListaEspecial("");
                    var prod1 = prod.Productos.ToList();
                    var objetos = await clientes.ObtenerListaEspecial("");


                    var objeto = new List<cliente>();

                    //foreach (var item in prod1)
                    //{
                    //    objeto.Add(objetos.Clientes.Where(a => a.CardCode.ToString().Contains(item.customer)
                    //).FirstOrDefault());
                    //}

                    foreach (var item in objetos.Clientes)
                    {
                        objeto.Add(item);
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
                    recibido = Newtonsoft.Json.JsonConvert.DeserializeObject<LlamadasViewModel>(jsonString);
                }
                var item = recibido.ItemCode;
                if (recibido.Credito)
                {
                    ParametrosFiltros filtroAprobacion = new ParametrosFiltros();
                    filtroAprobacion.ItemCode = item.Split("/")[0].Replace(" ", "");
                    filtroAprobacion.FechaBusqueda = DateTime.Now.Date;
                    filtroAprobacion.CardCode = recibido.CardCode.Split("/")[0].Replace(" ", "");
                    filtroAprobacion.Texto = item.Split("/")[1].Replace(" ", "");
                    filtroAprobacion.NoFacturado = true;

                    AprobacionesFacturasViewModel apf = new AprobacionesFacturasViewModel();
                    apf.CardCode = filtroAprobacion.CardCode;
                    apf.ItemCode = filtroAprobacion.ItemCode;
                    apf.Serie = filtroAprobacion.Texto;
                    apf.Fecha = DateTime.Now.Date;
                    apf.Aprobada = true;
                    apf.idLoginAprobador = -1;
                    apf.FechaAprobacion = DateTime.Now;
                    await aprobacion.Agregar(apf);
                }

                if (recibido.SinFacturar)
                {
                    ParametrosFiltros filtroAprobacion = new ParametrosFiltros();
                    filtroAprobacion.ItemCode = item.Split("/")[0].Replace(" ", "");
                    filtroAprobacion.FechaBusqueda = DateTime.Now.Date;
                    filtroAprobacion.CardCode = recibido.CardCode.Split("/")[0].Replace(" ", "");
                    filtroAprobacion.Texto = item.Split("/")[1].Replace(" ", "");
                    filtroAprobacion.NoFacturado = true; //Si esta aprobada


                    var Aprobacion = await aprobacion.ObtenerLista(filtroAprobacion);

                    if (Aprobacion.FirstOrDefault() == null)
                    {
                        AprobacionesFacturasViewModel apf = new AprobacionesFacturasViewModel();
                        apf.CardCode = filtroAprobacion.CardCode;
                        apf.ItemCode = filtroAprobacion.ItemCode;
                        apf.Serie = filtroAprobacion.Texto;
                        apf.Fecha = DateTime.Now.Date;
                        apf.Aprobada = false;
                        apf.idLoginAprobador = 0;
                        apf.FechaAprobacion = DateTime.Now;
                        await aprobacion.Agregar(apf);

                        var obj2 = new
                        {
                            success = false,
                            mensaje = "Se creo la solicitud de aprobación, favor notificar a administrador de backoffice",
                            solicitud = true,
                            facturar = false
                        };

                        return new JsonResult(obj2);
                    }

                }
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
                coleccion.SinRepuestos = recibido.SinRepuestos;
                coleccion.SinFacturar = recibido.SinFacturar;
                coleccion.Prioridad = recibido.Prioridad;
                coleccion.TratadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                try
                {
                    coleccion.CardCode = recibido.CardCode.Split("/")[0].Replace(" ", "");
                    var Validacion = recibido.CardCode.Split("/")[1].Replace(" ", "");

                }
                catch (Exception)
                {

                    throw new Exception("Debe seleccionar el cliente de la manera correcta");
                }

                try
                {
                    Clientes = await clientes.ObtenerListaEspecial("");
                    var Existe = Clientes.Clientes.Where(a => a.CardCode == coleccion.CardCode).FirstOrDefault();
                    if (Existe == null)
                    {
                        throw new Exception("Cliente es invalido");
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception($"{ex.Message}");
                }


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
                    mensaje = "",
                    solicitud = false,
                    facturar = false
                };

                return new JsonResult(obj);

            }
            catch (ApiException ex)
            {

                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                if (error.Message.Contains("Facturar"))
                {
                    var obj = new
                    {
                        success = false,
                        mensaje = "Error en el exception: -> " + error.Message,
                        solicitud = false,
                        facturar = true
                    };
                    return new JsonResult(obj);
                }
                else
                {
                    var obj = new
                    {
                        success = false,
                        mensaje = "Error en el exception: -> " + error.Message,
                        solicitud = false,
                        facturar = false
                    };
                    return new JsonResult(obj);
                }

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message + " -> " + ex.StackTrace.ToString(),
                    solicitud = false,
                    facturar = false
                };
                return new JsonResult(obj);
            }
        }


    }
}
