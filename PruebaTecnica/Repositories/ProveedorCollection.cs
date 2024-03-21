using MongoDB.Bson;
using MongoDB.Driver;
using PruebaTecnica.Models;

namespace PruebaTecnica.Repositories
{
    public class ProveedorCollection : IProveedorCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();
        private IMongoCollection<Proveedor> Collection;

        public ProveedorCollection()
        {
            Collection = _repository.db.GetCollection<Proveedor>("Products");

        }
        public async Task DeleteProveedor(string id)
        {
            var filter = Builders<Proveedor>.Filter.Eq(s => s.Id, new ObjectId(id));
                await Collection.DeleteOneAsync(filter);
        }

        public async Task<List<Proveedor>> GetAllProveedor()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();

        }

        public async Task<Proveedor> GetProveedorById(string id)
        {
            return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task InserProveedor(Proveedor proveedor)
        {
            await Collection.InsertOneAsync(proveedor);
        }

        public async Task UpdateProveedor(Proveedor proveedor)
        {
            var filter = Builders<Proveedor>
               .Filter.Eq(s => s.Id, proveedor.Id);

            await Collection.ReplaceOneAsync(filter, proveedor);
        }
    }
}
