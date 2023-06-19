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

namespace Boletaje.Pages.Boletas
{

    public class EditarModel : PageModel
    {
        private readonly ICrudApi<BoletaViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> serviceClientes;
        private readonly ICrudApi<ProductosBoletaViewModel, int> serviceProductos;
        private readonly ICrudApi<ProductosViewModel, int> prods;


        [BindProperty]
        public BoletaViewModel Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosBoletaViewModel[] Productos { get; set; }

        [BindProperty]
        public Producto Producto { get; set; }
        [BindProperty]
        public string ItemCode2 { get; set; }

        public EditarModel(ICrudApi<BoletaViewModel, int> service, ICrudApi<ProductosBoletaViewModel, int> serviceProductos, ICrudApi<ClientesViewModel, int> serviceClientes, ICrudApi<ProductosViewModel, int> prods)
        {
            this.service = service;
            this.serviceClientes = serviceClientes;
            this.serviceProductos = serviceProductos;
            this.prods = prods;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "48").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = 1;
                Clientes = await serviceClientes.ObtenerListaEspecial(filtro);
                Productos = await serviceProductos.ObtenerLista("");

                var productos = await prods.ObtenerListaEspecial("");
                var Serie = id.Split("|")[1].TrimStart();
                var ItemCode = id.Split("|")[0].TrimEnd();
                Input = new BoletaViewModel();
                Input.ItemCode = ItemCode;
                ItemCode2 = ItemCode;
                Producto = productos.Productos.Where(a => a.manufSN == Serie && a.itemCode == ItemCode).FirstOrDefault();
                Producto.customer = Producto.customer + " / " + Clientes.Clientes.Where(a => a.CardCode == Producto.customer).FirstOrDefault().CardName;
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
                
                //Input = new BoletaViewModel();
                Input.NoSerie = Producto.internalSN;
                Input.NoSerieFabricante = Producto.manufSN;
                //Input.ItemCode = ItemCode2;
                Input.CardCode = Producto.customer.Split("/")[0].TrimStart().TrimEnd();

                Clientes = await serviceClientes.ObtenerListaEspecial("");
                var Existe = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();
                if (Existe == null)
                {
                    throw new Exception("Cliente es invalido");
                }

                await service.Editar(Input);
                return RedirectToPage("/Llamadas/Nuevo");
            }
            catch (ApiException ex)
            {


                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = 1;
                Clientes = await serviceClientes.ObtenerListaEspecial(filtro);
                Productos = await serviceProductos.ObtenerLista("");
                var productos = await prods.ObtenerListaEspecial("");

                Producto = productos.Productos.Where(a => a.manufSN == Producto.manufSN).FirstOrDefault();
                Producto.customer = Producto.customer + " / " + Clientes.Clientes.Where(a => a.CardCode == Producto.customer).FirstOrDefault().CardName;
                ModelState.AddModelError(string.Empty, ex.Content.ToString());

                return Page();
            }
            catch (Exception ex)
            {
                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = 1;
                Clientes = await serviceClientes.ObtenerListaEspecial(filtro);
                Productos = await serviceProductos.ObtenerLista("");

                var productos = await prods.ObtenerListaEspecial("");

                Producto = productos.Productos.Where(a => a.manufSN == Producto.manufSN).FirstOrDefault();
                Producto.customer = Producto.customer + " / " + Clientes.Clientes.Where(a => a.CardCode == Producto.customer).FirstOrDefault().CardName;
                Errores error = new Errores();
                error.Message = ex.Message;
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
