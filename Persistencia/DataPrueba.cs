using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Identity;

namespace Persistencia
{
    public class DataPrueba
    {
        //Método para insertar usuario de prueba en base de datos
        public static async Task InsertarData(CursosOnlineContext context, UserManager<Usuario> usuarioManager)
        {
            //Se valida si no hay ningún usuario en la base de datos
            if (!usuarioManager.Users.Any())
            {
                //Usuario a crear
                var usuario = new Usuario{
                    NombreCompleto = "Yeisson Ochoa Villa", 
                    UserName = "yeisson8a", 
                    Email = "yeisson.ochoa72@gmail.com"
                };

                //Crear usuario en la base de datos
                await usuarioManager.CreateAsync(usuario, "Password123$");
            }
        }
    }
}