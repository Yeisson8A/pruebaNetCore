using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
        //Método para obtener lista de instructores (Procedimiento almacenado)
        Task<IEnumerable<InstructorModel>> ObtenerLista();
        //Método para obtener un instructor especifico (Procedimiento almacenado)
        Task<InstructorModel> ObtenerPorId(Guid id);
        //Método para crear un nuevo instructor (Procedimiento almacenado)
        Task<int> Crear(string nombre, string apellidos, string grado);
        //Método para actualizar un instructor existente (Procedimiento almacenado)
        Task<int> Actualizar(Guid instructorId, string nombre, string apellidos, string grado);
        //Método para eliminar un instructor existente (Procedimiento almacenado)
        Task<int> Eliminar(Guid id);
    }
}