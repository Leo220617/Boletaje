﻿using System;
using System.Globalization;
using Boletaje.Models;
using ConectorEcommerce.Models;
using FacturaElectronica.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Sicsoft.Checkin.Web.Models;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;
 

namespace Sicsoft.Checkin.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.ExpireTimeSpan = TimeSpan.FromMinutes(90000);
               options.SlidingExpiration = true;
           });

            services.AddHttpContextAccessor();

            services.AddScoped<AuthenticatedHttpClientHandler>();

            services.AddApiServices(Configuration);

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddRazorPages()
                .AddMvcOptions(options =>
                {

                    options.Filters.Add(new AuthorizeFilter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/StatusCode", "?code={0}");

            app.UseHttpsRedirection();

            var supportedCultures = new[]
            {
                new CultureInfo("es-cr"), //es-cr

            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("es-cr"),

                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }

    public static class CheckinServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration Configuration)
        {


            services.AddRefitClient<ICrudApi<LoginDevolucion, int>>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Login/Conectar"));
            // Add additional IHttpClientBuilder chained methods as required here:
            //  .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            // .SetHandlerLifetime(TimeSpan.FromMinutes(2));

       

          

            services.AddRefitClient<ICrudApi<LoginUsuario, int>>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Login"))
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<LoginUsuarioViewModel, int>>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Login"))
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<UsuariosViewModel, int>>()
           .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Login"))
           .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<RolesViewModel, int>>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Roles"))
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();
 

         
            services.AddRefitClient<ICrudApi<SeguridadModulosViewModel, int>>()
      .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/SeguridadModulos"))
      .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();


            services.AddRefitClient<ICrudApi<SeguridadRolesModulosViewModel, int>>()
      .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/SeguridadRolesModulos"))
      .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            

            services.AddRefitClient<ICrudApi<RecibidoRoles, int>>()
       .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Compras"))
       .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();


            services.AddRefitClient<ICrudApi<BitacoraViewModel, int>>()
     .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Bitacora"))
     .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();


            services.AddRefitClient<ICrudApi<SucursalesViewModel, int>>()
     .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Sucursales"))
     .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();



            services.AddRefitClient<ICrudApi<TecnicosViewModel, int>>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Tecnicos"))
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<StatusViewModel, int>>()
  .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Status"))
  .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<GarantiasViewModel, int>>()
 .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Garantias"))
 .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<TiposCasosViewModel, int>>()
 .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/TiposCasos"))
 .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ClientesViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Clientes"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Productos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<LlamadasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/LlamadasServicio"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosPadresViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ProductosPadres"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosHijosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ProductosHijos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ColeccionProductosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ProductosPadres"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<EncReparacionViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/EncReparacion"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<DetReparacionViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/DetReparacion"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();


            services.AddRefitClient<ICrudApi<ColeccionRepuestosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/EncReparacion"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<BodegasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Bodegas"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<BitacoraMovimientosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/BitacoraMovimientos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<DiagnosticosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Diagnosticos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ErroresViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Errores"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosBoletaViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ProductosPadres/Boleta"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<BoletaViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Boletas"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ClientViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Client"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<EncMovimientoViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Movimientos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ControlProductosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ControlProductos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<CotizacionesAprobadasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/CotizacionesAprobadas"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ImpuestosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Impuestos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<AsuntosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Asuntos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<OrdenVentaViewModel, int>>()
      .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/OrdenVenta"))
      .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ClientesPOrdenesViewModel, int>>()
     .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ClientesProductos"))
     .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosCOrdenesViewModel, int>>()
     .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Productos/GetProductos"))
     .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<OfertaVentaViewModel, int>>()
      .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/OfertaVenta"))
      .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<CondicionesPagosViewModel, int>>()
     .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/CondicionesPagos"))
     .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<TiemposEntregasViewModel, int>>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/TiemposEntregas"))
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<PersonasContactoViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/PersonasContacto"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<DiasValidosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/DiasValidos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<HistoricoViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Historico"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ActividadesViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Actividades"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<UbicacionesViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Ubicaciones"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<SoportesViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Soportes"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<LogModificacionesViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/LogModificaciones"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<HistoricoDetalladoViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Historico/Detallado"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<PlazosCreditosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/PlazosCreditos"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<EncFacturasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Facturas"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<CuentasBancariasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/CuentasBancarias"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ExoneracionesViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/Exoneraciones"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosGarantiasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ProductosGarantias"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<ProductosFacturacionInicialViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/ProductosFacturacionInicial"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<LlamadasFacturasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/LlamadasFacturas"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<AprobacionesFacturasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/AprobacionesFacturas"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<FacturasAprobadasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/FacturaAprobada"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<SolicitudesComprasViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/SolicitudesCompras"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<TipoCambiosViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/TipoCambios"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddRefitClient<ICrudApi<PlanificadorDiarioViewModel, int>>()
.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{Configuration["UrlWebApi"]}/api/PlanificadorDiario"))
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            return services;
        }
    }
}
