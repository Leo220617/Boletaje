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
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<BoletaViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> serviceClientes;
        private readonly ICrudApi<ProductosBoletaViewModel, int> serviceProductos;

        [BindProperty]
        public BoletaViewModel Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosBoletaViewModel[] Productos { get; set; }


        public NuevoModel(ICrudApi<BoletaViewModel, int> service, ICrudApi<ProductosBoletaViewModel, int> serviceProductos, ICrudApi<ClientesViewModel, int> serviceClientes)
        {
            this.service = service;
            this.serviceClientes = serviceClientes;
            this.serviceProductos = serviceProductos;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "31").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = 1;
                Clientes = await serviceClientes.ObtenerListaEspecial(filtro);
                Productos = await serviceProductos.ObtenerLista("");

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


                Input.CardCode = Input.CardCode.Split("/")[0].TrimStart().TrimEnd();
                Input.ItemCode = Input.ItemCode.Split("/")[0];



                Clientes = await serviceClientes.ObtenerListaEspecial("");
                var Existe = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault();
                if (Existe == null)
                {
                    throw new Exception("Cliente es invalido");
                }

                await service.Agregar(Input);
                return RedirectToPage("/Llamadas/Nuevo");
            }
            catch (ApiException ex)
            {
                

                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = 1;
                Clientes = await serviceClientes.ObtenerListaEspecial(filtro);
                Productos = await serviceProductos.ObtenerLista("");

                ModelState.AddModelError(string.Empty, ex.Content.ToString());

                return Page();
            }catch(Exception ex)
            {
                ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Codigo1 = 1;
                Clientes = await serviceClientes.ObtenerListaEspecial(filtro);
                Productos = await serviceProductos.ObtenerLista("");
                Errores error = new Errores();
                error.Message = ex.Message;
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
