using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Movimientos
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<EncMovimientoViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> serviceLlamada;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<ProductosPadresViewModel, int> prodsPadre;
        private readonly ICrudApi<HistoricoViewModel, int> historico;




        [BindProperty]
        public LlamadasViewModel Llamada { get; set; }
        [BindProperty]
        public EncMovimientoViewModel Input { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        [BindProperty]
        public cliente Cliente { get; set; }

        [BindProperty]
        public bool RolAceptacion { get; set; }
        [BindProperty]
        public ProductosPadresViewModel ProductoPadre { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] Productos { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijos { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Imp { get; set; }

        [BindProperty]
        public ProductosHijosViewModel ManoObra { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijosInsertar { get; set; }

        [BindProperty]

        public Producto ProductoPadreLlamada { get; set; }

        [BindProperty]
        public HistoricoViewModel Historico { get; set; }

        public EditarModel(ICrudApi<EncMovimientoViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> serviceLlamada, ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<ProductosViewModel, int> prods, ICrudApi<ProductosPadresViewModel, int> prodsPadre, ICrudApi<HistoricoViewModel, int> historico)
        {
            this.service = service;
            this.clientes = clientes;
            this.serviceLlamada = serviceLlamada;
            this.service2 = service2;
            this.impuestos = impuestos;
            this.prods = prods; 
            this.prodsPadre = prodsPadre;
            this.historico = historico;

        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "35").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Imp = await impuestos.ObtenerLista("");


                RolAceptacion = Roles1.Where(a => a == "36").FirstOrDefault() != null;

                Input = await service.ObtenerPorId(id);
                Llamada = await serviceLlamada.ObtenerPorDocEntry(Convert.ToInt32(Input.NumLlamada));

                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();

                var ProductosHijos2 = new List<ProductosHijosViewModel>();
                var Produc = await service2.ObtenerLista("");
                Productos = Produc;
                foreach (var item in Input.Detalle)
                {
                    var Pr = Produc.Where(a => a.codSAP == item.ItemCode).FirstOrDefault();
                    ProductosHijos2.Add(Pr);
                }

                ManoObra = Produc.Where(a => a.Nombre.ToUpper().Contains("Mano de Obra".ToUpper())).FirstOrDefault();
                ProductosHijos2.Add(ManoObra);
                ProductosHijos = ProductosHijos2.ToArray();
                ProductosHijosInsertar = await service2.ObtenerLista("");

                foreach (var item in ProductosHijos2)
                {
                    ProductosHijosInsertar = ProductosHijosInsertar.Where(a => a.id != item.id).ToArray();
                }

                var ProductosPadres = await prodsPadre.ObtenerLista("");
                ProductoPadre = ProductosPadres.Where(a => a.codSAP == Llamada.ItemCode).FirstOrDefault();
                if(ProductoPadre == null)
                {
                    ProductoPadre = new ProductosPadresViewModel();
                    ProductoPadre.codSAP = Llamada.ItemCode;
                    ProductoPadre.Nombre = Llamada.ItemCode;
                    ProductoPadre.Precio = 0;
                }
                else
                {
                    ProductoPadre.Precio = Math.Round(ProductoPadre.Precio, 2);
                }
                try
                {
                    ParametrosFiltros filtHist = new ParametrosFiltros();
                    filtHist.CardCode = Llamada.DocEntry.ToString();
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


        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            try
            {
                RecibidoMovimientos recibido = JsonConvert.DeserializeObject<RecibidoMovimientos>(recibidos);

                EncMovimientoViewModel coleccion = new EncMovimientoViewModel();

                coleccion.Detalle = new DetMovimientoViewModel[recibido.Detalle.Length];

                coleccion.id = recibido.id;
                coleccion.CreadoPor = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault();
                coleccion.Comentarios = recibido.Comentarios;
                coleccion.Descuento = recibido.Descuento;
                coleccion.Impuestos = recibido.Impuestos;
                coleccion.Subtotal = recibido.Subtotal;
                coleccion.PorDescuento = recibido.PorDescuento;
                coleccion.Generar = recibido.Generar;
                coleccion.Regenerar = recibido.Regenerar;

                coleccion.TotalComprobante = recibido.TotalComprobante;
                coleccion.Moneda = recibido.Moneda;

                short cantidad = 1;
                foreach (var item in recibido.Detalle)
                {
                    coleccion.Detalle[cantidad - 1] = new DetMovimientoViewModel();
                    coleccion.Detalle[cantidad - 1].id = item.id;
                    coleccion.Detalle[cantidad - 1].ItemCode = item.ItemCode;
                    coleccion.Detalle[cantidad - 1].ItemName = item.ItemName;
                    coleccion.Detalle[cantidad - 1].NumLinea = item.NumLinea;


                    coleccion.Detalle[cantidad - 1].PrecioUnitario = item.PrecioUnitario;
                    coleccion.Detalle[cantidad - 1].Cantidad = item.Cantidad;
                    coleccion.Detalle[cantidad - 1].PorDescuento = item.PorDescuento;
                    coleccion.Detalle[cantidad - 1].Descuento = item.Descuento;
                    coleccion.Detalle[cantidad - 1].Impuestos = item.Impuestos;
                    coleccion.Detalle[cantidad - 1].TotalLinea = item.TotalLinea;
                    coleccion.Detalle[cantidad - 1].Garantia = item.Garantia;
                    coleccion.Detalle[cantidad - 1].Opcional = item.Opcional;
                    coleccion.Detalle[cantidad - 1].idImpuesto = item.idImpuesto;



                    cantidad++;
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

                ModelState.AddModelError(string.Empty, ex.Message);

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
                    mensaje = "Error en el exception: -> " + ex.Message
                };
                return new JsonResult(obj);
            }
        }

    }
}
