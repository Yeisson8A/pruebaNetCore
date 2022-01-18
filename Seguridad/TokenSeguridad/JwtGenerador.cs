using System;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using Aplicacion.Contratos;
using Dominio;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Seguridad
{
    public class JwtGenerador : IJwtGenerador
    {
        public string CrearToken(Usuario usuario, List<string> roles)
        {
            //Claims para el token
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)
            };

            //Validar que la lista de roles no sea nula
            if (roles != null)
            {
                //Recorrer listado de roles
                foreach (var rol in roles)
                {
                    //Adicionar a la lista de claims del token un claim para cada rol del usuario
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }
            }

            //Llave para encriptar el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            //Credenciales
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Descripción del token
            var tokenDescripcion = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = credenciales
            };

            //Crear token
            var tokenManejador = new JwtSecurityTokenHandler();
            var token = tokenManejador.CreateToken(tokenDescripcion);

            //Devolver token generado
            return tokenManejador.WriteToken(token);
        }
    }
}