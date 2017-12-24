using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Budget.Api.Operations
{
    [Route("api/[controller]")]
    public sealed class OperationsController : Controller
    {
        [HttpGet("/api/budgets/{year:int}/[controller]")]
        public IEnumerable<OperationSummary> GetAll(int year)
        {
            var client = new MongoClient("mongodb://192.168.255.129:27017");
            var db = client.GetDatabase("budgetio");
            var operations = db.GetCollection<BsonDocument>("operations");

            var summaries = operations
                .Find(o => o["budget_id"] == year)
                .ToList()
                .Select(o => new OperationSummary
                {
                    Id = o["_id"].AsObjectId.ToString(),
                    CategoryId = o["category_id"].AsObjectId.ToString(),
                    Month = o["month"].AsInt32,
                    Plan = o["plan"].AsDecimal,
                    Fact = o["fact"].AsDecimal
                });

            return summaries;
        }

        [HttpPost("/api/budgets/{year:int}/[controller]")]
        public IActionResult Create(int year, [FromBody] OperationSummary operation)
        {
            var client = new MongoClient("mongodb://192.168.255.129:27017");
            var db = client.GetDatabase("budgetio");
            var operations = db.GetCollection<BsonDocument>("operations");

            var doc = new BsonDocument
            {
                ["budget_id"] = year,
                ["category_id"] = ObjectId.Parse(operation.CategoryId),
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