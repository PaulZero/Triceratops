using MongoDB.Driver;
using Triceratops.Api.Services.DbService.Interfaces;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoTransaction : ITransaction
    {
        private readonly IClientSessionHandle _mongoSession;

        public MongoTransaction(IClientSessionHandle mongoSession)
        {
            _mongoSession = mongoSession;
            _mongoSession.StartTransaction();
        }

        public void Commit()
        {
            _mongoSession.CommitTransaction();
        }

        public void Dispose()
        {
            _mongoSession.Dispose();
        }

        public void Rollback()
        {
            _mongoSession.AbortTransaction();
        }
    }
}
