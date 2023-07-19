using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace Boletaje.Pages.OfertaVenta
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<OfertaVentaViewModel, int> service;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<ClientesPOrdenesViewModel, int> clientes;
        private readonly ICrudApi<ProductosCOrdenesViewModel, int> prod;
        private readonly ICrudApi<CondicionesPagosViewModel, int> conds;
        private readonly ICrudApi<GarantiasViewModel, int> garan;
        private readonly ICrudApi<TiemposEntregasViewModel, int> tiemp;
        private readonly ICrudApi<DiasValidosViewModel, int> dvalid;

        [BindProperty]
        public string Bodega { get; set; }
        [BindProperty]
        public OfertaVentaViewModel Orden { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        [BindProperty]
        public ClientesPOrdenesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosCOrdenesViewModel Productos { get; set; }

        [BindProperty]
        public CondicionesPagosViewModel[] Condiciones { get; set; }

        [BindProperty]
        public GarantiasViewModel[] Garantias { get; set; }

        [BindProperty]
        public TiemposEntregasViewModel[] Tiempos { get; set; }
        [BindProperty]
        public DiasValidosViewModel[] DiasValidos { get; set; }
        public ObservarModel(ICrudApi<OfertaVentaViewModel, int> service, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<ClientesPOrdenesViewModel, int> clientes, ICrudApi<ProductosCOrdenesViewModel, int> prod,ICrudApi<CondicionesPagosViewModel, int> conds,
            ICrudApi<GarantiasViewModel, int> garan, ICrudApi<TiemposEntregasViewModel, int> tiemp, ICrudApi<DiasValidosViewModel, int> dvalid)
        {
            this.service = service;
            this.impuestos = impuestos;
            this.clientes = clientes;
            this.prod = prod;
            this.conds = conds;
            this.garan = garan;
            this.tiemp = tiemp;
            this.dvalid = dvalid;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "45").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Bodega = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Bodega").Select(s1 => s1.Value).FirstOrDefault();
                Condiciones = await conds.ObtenerLista("");
                Garantias = await garan.ObtenerLista("");
                Tiempos = await tiemp.ObtenerLista("");
                Clientes = await clientes.ObtenerListaEspecial("");
                Impuestos = await impuestos.ObtenerLista("");
                DiasValidos = await dvalid.ObtenerLista("");


                Orden = await service.ObtenerPorId(id);
                Orden.Cliente = Orden.CardCode + " - " + (Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault().CardName);


                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Texto = Orden.Detalle.FirstOrDefault().Bodega;
                filtro.ListPrice = Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault().ListNum.ToString();
                Productos = await prod.ObtenerListaEspecial(filtro);
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
