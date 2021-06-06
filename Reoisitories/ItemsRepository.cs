using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Reoisitories
{
    public interface IItemsRepository
    {
        Task CreateAsync(Item entity);
        Task<IReadOnlyCollection<Item>> GetAllAsync();
        Task<Item> GetAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(Item entity);
        Task CountAsync(string requestName);
    }

    public class ItemsRepository : IItemsRepository
    {
        private const string collectionName = "items";

        private const string countCollectionName = "RequestCounter";

        private readonly IMongoCollection<Item> dbCollection;

        private readonly IMongoCollection<RequestCountItem> countCollection;

        private readonly FilterDefinitionBuilder<Item> filterDefinitionBuilder = Builders<Item>.Filter;

        private readonly FilterDefinitionBuilder<RequestCountItem> countFilterDefinitionBuilder = Builders<RequestCountItem>.Filter;


        public ItemsRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var datebase = mongoClient.GetDatabase("Catalog");
            dbCollection = datebase.GetCollection<Item>(collectionName);
            countCollection = datebase.GetCollection<RequestCountItem>(countCollectionName);
        }

        public async Task CountAsync(string requestName)
        {
            var item = await countCollection.Find(item => item.RequestName == requestName).SingleOrDefaultAsync();
            if (item == null)
            {
                var toAddCountItem = new RequestCountItem()
                {
                    RequestName = requestName,
                    Count = 1
                };
                await countCollection.InsertOneAsync(toAddCountItem);
                return;
            }
            item.Count++;
            var filter = countFilterDefinitionBuilder.Eq(item => item.RequestName, requestName);
            var update = Builders<RequestCountItem>.Update.Set(p => p.Count, item.Count);
            await countCollection.UpdateOneAsync(filter, update);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbCollection.Find(filterDefinitionBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterDefinitionBuilder.Eq(item => item.Id, id);
            return await dbCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateAsync(Item entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Item entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            FilterDefinition<Item> filter = filterDefinitionBuilder.Eq(item => item.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterDefinitionBuilder.Eq(item => item.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

    }
}
