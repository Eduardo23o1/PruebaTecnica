using PruebaTecnica.Models;

namespace PruebaTecnica.Repositories
{
    public interface IProveedorCollection
    {
        Task InserProveedor(Proveedor proveedor);
        Task UpdateProveedor(Proveedor proveedor);
        Task DeleteProveedor(String id);
        Task<List<Proveedor>> GetAllProveedor();
        Task<Proveedor> GetProveedorById(String id);
    }
}
