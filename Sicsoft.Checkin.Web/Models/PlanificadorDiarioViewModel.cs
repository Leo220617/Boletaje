using System;

namespace Boletaje.Models
{
    public class PlanificadorDiarioViewModel
    {
        public planificador[] Planificador { get; set; }
    }
    public class planificador
    {
        public int callID { get; set; }
        public string Clase_CLiente { get; set; }
        public string ASESOR_ASIGNADO { get; set; }
        public string Tipo_Caso { get; set; }
        public int idSAPTP { get; set; }
        public int DiasHabiles { get; set; }
        public string Status_Actual { get; set; }
        public int Dias_Status_Actual { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_de_actualizacion { get; set; }
        public string Ultimo_Status { get; set; }
        public int Dias_acumulados { get; set; }
        public decimal Monto_de_reparacion { get; set; }
        public int NumEntrega { get; set; }
        public string Serie_del_Equipo { get; set; }
        public string Codigo_Articulo { get; set; }
        public string Descripcion_Articulo { get; set; }

        public string Persona_Contacto { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public DateTime Fecha_Compra { get; set; }
        public string TECNICO { get; set; }
        public string Sucursal_Recibo { get; set; }
        public string Sucursal_Entrega { get; set; }
        public string Categoria_Cliente { get; set; }
        public DateTime FechaProximoContacto { get; set; }
    }
}
