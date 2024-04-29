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

namespace Boletaje.Pages.Bodega
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<EncReparacionViewModel, int> service;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<TecnicosViewModel, int> serviceT;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> bt;
        private readonly ICrudApi<ProductosHijosViewModel, int> prodHijos;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> llamada;


        [BindProperty]
        public BitacoraMovimientosViewModel BTS { get; set; }
        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]
        public string Tecnico { get; set; }
        [BindProperty]
        public EncReparacionViewModel Encabezado { get; set; }
        [BindProperty]
        public string Producto { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] ProductosHijos { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }
        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public string Cliente { get; set; }
        public ObservarModel(ICrudApi<EncReparacionViewModel, int> service, ICrudApi<ProductosViewModel, int> prods, ICrudApi<TecnicosViewModel, int> serviceT, ICrudApi<BitacoraMovimientosViewModel, int> bt, ICrudApi<ProductosHijosViewModel, int> prodHijos,
            ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> llamada)
        {
            this.service = service;
            this.prods = prods;
            this.serviceT = serviceT;
            this.bt = bt;
            this.prodHijos = prodHijos;
            this.clientes = clientes;
            this.llamada = llamada;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "26").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = id;

                Tecnicos = await serviceT.ObtenerLista("");
                BTS = await bt.ObtenerPorId(id);

                Encabezado = await service.ObtenerPorId(BTS.idEncabezado);

                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;

                var ids = Encabezado.idTecnico.ToString();
                Tecnico = Tecnicos.Where(a => a.idSAP == ids).FirstOrDefault().Nombre;

                ProductosHijos = await prodHijos.ObtenerLista("");

                Clientes = await clientes.ObtenerListaEspecial("");
                var Llamada = await llamada.ObtenerPorDocEntry(Encabezado.idLlamada);
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardName;



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
                await bt.Agregar(BTS);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                
                ModelState.AddModelError(string.Empty, ex.Content.ToString());
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = BTS.id;

                Tecnicos = await serviceT.ObtenerLista("");
                BTS = await bt.ObtenerPorId(BTS.id);

                Encabezado = await service.ObtenerPorId(BTS.idEncabezado);

                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;

                var ids = Encabezado.idTecnico.ToString();
                Tecnico = Tecnicos.Where(a => a.idSAP == ids).FirstOrDefault().Nombre;

                ProductosHijos = await prodHijos.ObtenerLista("");

                Clientes = await clientes.ObtenerListaEspecial("");
                var Llamada = await llamada.ObtenerPorDocEntry(Encabezado.idLlamada);
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardName;

                return Page();
            }
            catch (Exception ex)
            {
                 
                ModelState.AddModelError(string.Empty, ex.Message);

                return Page();
            }
        }
        public async Task<IActionResult> OnPostAgregarBTS(BitacoraMovimientosViewModel recibidos)
        {
            string error = "";


            try
            {


                var resp = await bt.Agregar(recibidos);

                var resp2 = new
                {
                    success = true,
                    Exon = resp
                };
                return new JsonResult(resp2);
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Content.ToString());

                var resp2 = new
                {
                    success = false,
                    Exon = ex.Content.ToString()
                };
                return new JsonResult(resp2);


            }
            catch (Exception ex)
            {
                var resp2 = new
                {
                    success = false,
                    Exon = ex.Message.ToString()
                };
                return new JsonResult(resp2);
            }
        }

    }
}
