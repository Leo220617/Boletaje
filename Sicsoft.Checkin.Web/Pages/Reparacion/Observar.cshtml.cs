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


namespace Boletaje.Pages.Reparacion
{
    public class ObservarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<EncReparacionViewModel, int> service;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<LlamadasViewModel, int> llamada;

        private readonly ICrudApi<TecnicosViewModel, int> serviceT;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> bt;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;
        private readonly ICrudApi<ErroresViewModel, int> serviceError;


        [BindProperty]
        public ErroresViewModel[] Errores { get; set; }
        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }
        [BindProperty]
        public BitacoraMovimientosViewModel[] BTS { get; set; }
        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]
        public string Tecnico { get; set; }
        [BindProperty]
        public EncReparacionViewModel Encabezado { get; set; }
        [BindProperty]
        public string Producto { get; set; }

        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }
        public ObservarModel(ICrudApi<EncReparacionViewModel, int> service, ICrudApi<ProductosViewModel, int> prods, ICrudApi<TecnicosViewModel, int> serviceT, ICrudApi<BitacoraMovimientosViewModel, int> bt, ICrudApi<DiagnosticosViewModel, int> serviceD,
            ICrudApi<ErroresViewModel, int> serviceError, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<LlamadasViewModel, int> llamada
            )
        {
            this.service = service;
            this.prods = prods;
            this.serviceT = serviceT;
            this.bt = bt;
            this.serviceD = serviceD;
            this.serviceError = serviceError;
            this.clientes = clientes;
            this.llamada = llamada;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "24").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = id;

                BTS = await bt.ObtenerLista(filt);
                Encabezado = await service.ObtenerPorId(id);
                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;

                Clientes = await clientes.ObtenerListaEspecial("");
                var Llamada = await llamada.ObtenerPorDocEntry(Encabezado.idLlamada);
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardCode +  " - " + Clientes.Clientes.Where(a => a.CardCode == Llamada.CardCode).FirstOrDefault().CardName;


                Tecnicos = await serviceT.ObtenerLista("");
                var ids = Encabezado.idTecnico.ToString();
                Tecnico = Tecnicos.Where(a => a.idSAP == ids).FirstOrDefault().Nombre;
                Diagnosticos = await serviceD.ObtenerLista("");
                Errores = await serviceError.ObtenerLista("");




                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
