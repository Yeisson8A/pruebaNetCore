using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class RolLista
    {
        public class Ejecuta : IRequest<List<IdentityRole>>{}

        public class Manejador : IRequestHandler<Ejecuta, List<IdentityRole>>
        {
            private readonly CursosOnlineContext _context;

            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<List<IdentityRole>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Usar el objeto CursosOnlineContext para obtener los datos de la entidad Roles del Core Identity
                var roles = await _context.Roles.ToListAsync();
                //Devolver listado de roles obtenido
                return roles;
            }
        }
    }
}