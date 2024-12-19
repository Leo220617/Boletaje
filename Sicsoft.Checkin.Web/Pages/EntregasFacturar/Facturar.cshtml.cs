using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO.Compression;
using System.IO;
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


namespace Boletaje.Pages.EntregasFacturar
{
    public class FacturarModel : PageModel
    {
        private readonly ICrudApi<EncFacturasViewModel, int> service;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<PlazosCreditosViewModel, int> plazos;
        private readonly ICrudApi<EncMovimientoViewModel, int> movimientos;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<CuentasBancariasViewModel, int> cuentasB;
        private readonly ICrudApi<ExoneracionesViewModel, int> exonera;



        [BindProperty]
        public EncFacturasViewModel Factura { get; set; }

        [BindProperty]
        public EncMovimientoViewModel Movimientos { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public cliente Cliente { get; set; }

        [BindProperty]
        public ExoneracionesViewModel Exoneraciones { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }
        

        [BindProperty]
        public CondicionesPagosViewModel[] Condiciones { get; set; }

        [BindProperty]
        public PlazosCreditosViewModel[] PlazosCreditos { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] Productos { get; set; }
        [BindProperty]
        public ProductosHijosViewModel ManoObra { get; set; }
        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijos { get; set; }
        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijosInsertar { get; set; }

        [BindProperty]
        public CuentasBancariasViewModel[] CuentasBancarias { get; set; }

        public FacturarModel(ICrudApi<EncFacturasViewModel, int> service, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<CondicionesPagosViewModel, int> conds, ICrudApi<PlazosCreditosViewModel, int> plazos,
            ICrudApi<EncMovimientoViewModel, int> movimientos, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<CuentasBancariasViewModel, int> cuentasB, ICrudApi<ExoneracionesViewModel, int> exonera
            )
        {
            this.service = service;
            this.impuestos = impuestos;
            this.conds = conds; 
            this.plazos = plazos;
            this.movimientos = movimientos; 
            this.clientes = clientes;
            this.service2 = service2;
            this.cuentasB = cuentasB;
            this.exonera = exonera;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "74").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Movimientos = await movimientos.ObtenerPorId(id);
                Clientes = await clientes.ObtenerListaEspecial("");
                PlazosCreditos = await plazos.ObtenerLista("");
                Condiciones = await conds.ObtenerLista("");
                Impuestos = await impuestos.ObtenerLista("");
                ParametrosFiltros filtrocuentas = new ParametrosFiltros();
                filtrocuentas.Codigo1 = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Sucursal").Select(s1 => s1.Value).FirstOrDefault());
                CuentasBancarias = await cuentasB.ObtenerLista(filtrocuentas);
                

                if (Movimientos != null)
                {
                    var ProductosHijos2 = new List<ProductosHijosViewModel>();
                    var Produc = await service2.ObtenerLista("");
                    Productos = Produc;
                    foreach (var item in Movimientos.Detalle.Where(a => a.Garantia == false).ToList())
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

                    Cliente = Clientes.Clientes.Where(a => a.CardCode == Movimientos.CardCode).FirstOrDefault(); 
                    ParametrosFiltros filtroExo = new ParametrosFiltros();
                    filtroExo.CardCode = Cliente.CardCode;
                    Exoneraciones = await exonera.ObtenerListaEspecial(filtroExo);
                    Factura = new EncFacturasViewModel();
                    Factura.CardCode = Movimientos.CardCode;
                    Factura.NombreCliente = Cliente.CardName; // Movimientos.CardName;
                    Factura.Correo = Cliente.E_Mail; //Movimientos.EmailPersonaContacto;
                    Factura.Cedula = Cliente.Cedula;
                    Factura.Fecha = DateTime.Now;
                    Factura.idEntrega = id;
                    Factura.TipoDocumento = "01";
                    Factura.Comentarios = Movimientos.Comentarios;
                    Factura.NumLlamada = Movimientos.NumLlamada;
                    Factura.Moneda = Movimientos.Moneda;
                    Factura.PorDesc = Movimientos.PorDescuento;
                    Factura.DetFactura = new DetFacturasViewModel[Movimientos.Detalle.Where(a => a.Garantia == false).Count()];
                    var i = 0;
                    foreach (var item in Movimientos.Detalle.Where(a => a.Garantia == false).ToList())
                    {
                        var Detalle = new DetFacturasViewModel();
                        Detalle.idImpuesto = item.idImpuesto;
                        Detalle.ItemCode = item.ItemCode;
                        Detalle.CodBodega = "";
                        Detalle.ListaPrecios = "";
                        Detalle.Cabys = "";
                        Detalle.Cantidad = item.Cantidad;
                        Detalle.PrecioUnitario = item.PrecioUnitario; 
                        Detalle.UnidadMedida = "";
                        Detalle.NomPro = item.ItemName;
                        Detalle.PorDescto = item.PorDescuento; 

                        Factura.DetFactura[i] = Detalle;
                        i++;
                    }

                }else
                {
                    throw new Exception("No existe ninguna entrega");
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
                RecibidosFacturas recibido = new RecibidosFacturas();
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
                    recibido = Newtonsoft.Json.JsonConvert.DeserializeObject<RecibidosFacturas>(jsonString);
                }

                //RecibidoMovimientos recibido = JsonConvert.DeserializeObject<RecibidoMovimientos>(recibidos);

                EncFacturasViewModel coleccion = new EncFacturasViewModel();

                coleccion.DetFactura = new DetFacturasViewModel[recibido.Detalle.Length];
                coleccion.MetodosPagos = new MetodosPagosViewModel[recibido.Metodos.Length];

                coleccion.idEntrega = recibido.idEntrega;
                coleccion.CreadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                coleccion.idCondicionVenta = recibido.idCondicionVenta;
                coleccion.idPlazoCredito = recibido.idPlazoCredito;
                coleccion.NumLlamada = recibido.NumLlamada;
                coleccion.TipoDocumento = recibido.TipoDocumento;
                coleccion.CardCode = recibido.CardCode;
                coleccion.Cedula = recibido.Cedula;
                coleccion.NombreCliente = recibido.NombreCliente;
                coleccion.Correo = recibido.Correo;
                coleccion.Fecha = DateTime.Now;
                coleccion.Moneda = recibido.Moneda;
                coleccion.TipoCambio = recibido.TipoCambio;
                coleccion.Comentarios = recibido.Comentarios;
                coleccion.Subtotal = recibido.Subtotal;
                coleccion.TotalImpuestos = recibido.TotalImpuestos;
                coleccion.TotalDescuento = recibido.TotalDescuento;
                coleccion.TotalCompra = recibido.TotalCompra;
                coleccion.PorDesc = recibido.PorDesc;
                coleccion.idSucursal = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Sucursal").Select(s1 => s1.Value).FirstOrDefault());

                short cantidad = 1;
                foreach (var item in recibido.Detalle)
                {
                    coleccion.DetFactura[cantidad - 1] = new DetFacturasViewModel();
                    coleccion.DetFactura[cantidad - 1].ItemCode = item.ItemCode;
                    coleccion.DetFactura[cantidad - 1].NomPro = item.NomPro;
                    coleccion.DetFactura[cantidad - 1].NumLinea = item.NumLinea;
                    coleccion.DetFactura[cantidad - 1].idImpuesto = item.idImpuesto;
                    coleccion.DetFactura[cantidad - 1].CodBodega = "";
                    coleccion.DetFactura[cantidad - 1].Cabys = "";
                    coleccion.DetFactura[cantidad - 1].ListaPrecios = "";
                    coleccion.DetFactura[cantidad - 1].Cantidad = item.Cantidad;
                    coleccion.DetFactura[cantidad - 1].UnidadMedida = "";
                    coleccion.DetFactura[cantidad - 1].PrecioUnitario = item.PrecioUnitario;
                    coleccion.DetFactura[cantidad - 1].PorDescto = item.PorDescto;
                    coleccion.DetFactura[cantidad - 1].TotalDescuento = item.TotalDescuento;
                    coleccion.DetFactura[cantidad - 1].TotalImpuestos = item.TotalImpuestos;
                    coleccion.DetFactura[cantidad - 1].TotalLinea = item.TotalLinea;
                    coleccion.DetFactura[cantidad - 1].idDocumentoExoneracion = item.idDocumentoExoneracion;


                    cantidad++;
                }
                cantidad = 1;
                foreach (var item in recibido.Metodos)
                {
                    coleccion.MetodosPagos[cantidad - 1] = new MetodosPagosViewModel();
                    coleccion.MetodosPagos[cantidad - 1].idCuentaBancaria = item.idCuentaBancaria;
                    coleccion.MetodosPagos[cantidad - 1].Metodo = item.Metodo;
                    coleccion.MetodosPagos[cantidad - 1].Moneda = item.Moneda;
                    coleccion.MetodosPagos[cantidad - 1].BIN = item.BIN;
                    coleccion.MetodosPagos[cantidad - 1].MonedaVuelto = item.MonedaVuelto;
                    coleccion.MetodosPagos[cantidad - 1].Monto = item.Monto;
                    coleccion.MetodosPagos[cantidad - 1].NumCheque = item.NumCheque;
                    coleccion.MetodosPagos[cantidad - 1].NumReferencia = item.NumReferencia;
                    coleccion.MetodosPagos[cantidad - 1].PagadoCon = item.PagadoCon;
                    cantidad++;
                }

               var Fac = await service.Agregar(coleccion);
 

                var obj = new
                {
                    success = true,
                    mensaje = "",
                    documento = Fac
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

    }
}
