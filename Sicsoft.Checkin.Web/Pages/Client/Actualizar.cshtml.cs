using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Boletaje.Pages.Asuntos;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
namespace Boletaje.Pages.Client
{
    public class ActualizarModel : PageModel
    {
        private readonly ICrudApi<ClientViewModel, int> service;
        private readonly ICrudApi<ClientesPOrdenesViewModel, int> clientes;

        [BindProperty]
        public ClientViewModel Input { get; set; }

        public ActualizarModel(ICrudApi<ClientViewModel, int> service, ICrudApi<ClientesPOrdenesViewModel, int> clientes)
        {
            this.service = service;
            this.clientes = clientes;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "32").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                var Clientes = await clientes.ObtenerListaEspecial("");
                var Cliente = Clientes.Clientes.Where(a => a.CardCode == id).FirstOrDefault();
                Input = new ClientViewModel();
                Input.CardCode = id;
                Input.CardName = Cliente.CardName;
               

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


                await service.Editar(Input);
                var objet = new 
                {
                    id = 0
                };
                return RedirectToPage("/OfertaVenta/Nuevo",objet);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
            catch (Exception ex)
            {
                Errores error = new Errores();
                error.Message = ex.Message;
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
