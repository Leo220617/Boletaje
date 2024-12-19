using Refit;
using Sicsoft.Checkin.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sicsoft.Checkin.Web.Servicios
{
     

    public interface ICrudApi<TEntity, TKey> where TEntity : class
    {
        //[Post("")]
        //Task<TEntity> Agregar<TCreate>([Body] TCreate payload) where TCreate : class;

        [Get("")]
        Task EnviarCorreo([Body] TEntity payload);

        [Post("")]
        Task<TEntity[]> AgregarLlamada([Body] TEntity llamada, TEntity[] adj);

        [Post("")]
        Task<TEntity[]> AgregarBulk([Body] TEntity[] payload);

        [Post("")]
        Task<TEntity> Agregar([Body] TEntity payload);

        [Post("/EnviarSAP")]
        Task<TEntity> EnviarSAP([Body] TEntity payload);

        [Post("")]
        Task<TEntity> CambiarClave([Body] TEntity payload);

        [Get("")]
        Task<TEntity> Login(string email, string clave);


        [Get("")]
        Task<bool> VCierre(int idLogin, string Periodo, DateTime fechaCierre, string CodMoneda);


        [Get("/Listado")]
        Task<TEntity[]> ObtenerListaCompras<TQuery>(TQuery q);

        [Get("")]
        Task<TEntity[]> ObtenerLista<TQuery>(TQuery q);

        [Get("")]
        Task<TEntity> ObtenerListaEspecial<TQuery>(TQuery q);

        [Get("")]
        Task<TEntity> ObtenerHeader<TQuery>(TQuery q);

        [Get("/Insertar")]
        Task<TEntity> InsertarAsiento(int idCierre);

        [Get("/RealizarLecturaEmail")]
        Task RealizarLecturaEmails();

        [Get("/LeerBandejaEntrada")]
        Task LecturaBandejaEntrada();

        [Get("/IngresarInventarioUno")]
        Task<TEntity> ActualizarInventario(string id, string lp);


        [Get("/IngresarClientesUno")]
        Task<TEntity> ActualizarCliente<TQuery>(TQuery q);

        [Get("/Consultar")]
        Task<TEntity> ObtenerPorId(int id);


        [Get("/GenerarAprobacion")]
        Task<TEntity> GenerarAprobacion(int id);

        [Get("/ConsultarPorDocEntry")]
        Task<TEntity> ObtenerPorDocEntry(int id);

        [Get("/Consultar")]
        Task<TEntity> ObtenerPorIdS(string id);


        [Get("/Consultar")]
        Task<TEntity[]> ObtenerDetalles(int id);

        [Get("/Estado")]
        Task<TEntity> CambiaEstado(int id, string Estado, string comentario = "", int idLoginAceptacion = 0);

        [Post("/{id}")]
        Task GenerarMovimientos(int id);

        [Post("/Debitar")]
        Task Debitar(int id, string certificado, decimal monto);

        [Post("/Actualizar")]
        Task Editar( [Body]TEntity payload);

        [Post("/Eliminar")]
        Task EliminarInversion(string id);

        [Post("/Eliminar")]
        Task Eliminar(int id);
        [Post("/Devolucion")]
        Task Devolucion(int id);

        [Post("/Eliminar")]
        Task AprobarFactura(int id, int idLoginAceptador);

        [Post("/AprobarSuperior")]
        Task AprobarSuperior(int id);

        [Post("/EliminarOferta")]
        Task EliminarOferta(int id);


        [Post("/Eliminar")]
        Task EliminarUsuario(int id);

        [Post("/Eliminar")]
        Task EliminarEjecutivo (int idEjecutivo);

        [Get("/Reenvio")]
        Task<TEntity> ReenvioFacturas(int id, string correo);
    }
}
