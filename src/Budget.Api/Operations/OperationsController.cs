using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Budget.Api.Operations
{
    [Route("api/[controller]")]
    public sealed class OperationsController : Controller
    {
        [HttpPost("/api/budgets/{year:int}/[controller]")]
        public IActionResult Create(int year, [FromBody] OperationSummary operation)
        {
            var client = new MongoClient("mongodb://192.168.255.129:27017");
            var db = client.GetDatabase("budgetio");
            var operations = db.GetCollection<BsonDocument>("operations");

            var doc = new BsonDocument
            {
                ["budget_id"] = year,
                ["category_id"] = operation.CategoryId,
                ["month"] = operation.Month,
                ["plan"] = operation.Plan,
                ["fact"] = operation.Fact
            };

            operations.InsertOne(doc);

            operation.Id = doc["_id"].AsObjectId.ToString();

            return Created("", operation);
        }
    }
}