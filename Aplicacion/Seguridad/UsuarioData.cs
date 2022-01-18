namespace Aplicacion.Seguridad
{
    //Información que va a devolver el servicio Login
    public class UsuarioData
    {
        public string NombreCompleto {get;set;}
        public string Token {get;set;}
        public string Email {get;set;}
        public string Username {get;set;}
        public string Imagen {get;set;}
    }
}