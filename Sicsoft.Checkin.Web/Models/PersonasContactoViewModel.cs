namespace Boletaje.Models
{
    public class PersonasContactoViewModel
    {
        public PersonasContacto[] PersonasContacto { get; set; }
    }
    public class PersonasContacto
    {
        public string CardCode { get; set; }
        public string NombreContacto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        
    }
}
