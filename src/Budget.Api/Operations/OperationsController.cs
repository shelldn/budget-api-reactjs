using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace Budget.Api.Operations
{
    [Route("api/[controller]")]
    public sealed class OperationsController : Controller
    {
        private readonly IConfiguration _config;

        public OperationsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("/api/budgets/{year:int}/[controller]")]
        public IEnumerable<OperationSummary> GetAll(int year)
        {
            var accountId = Int32.Parse(User.FindFirstValue("sub"));
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var operations = db.GetCollection<BsonDocument>("operations");

            var summaries = operations
                .Find(o => o["account_id"] == accountId && o["budget_id"] == year)
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
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var operations = db.GetCollection<BsonDocument>("operations");

            var doc = new BsonDocument
            {
                ["account_id"] = Int32.Parse(User.FindFirstValue("sub")),
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

        [HttpPatch("{id}")]
        public void Update(string id, [FromBody] OperationPatch patch)
        {
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var operations = db.GetCollection<BsonDocument>("operations");

            operations.UpdateOne(
                new BsonDocument("_id", ObjectId.Parse(id)),
                new BsonDocument("$set", new BsonDocument
                {
                    ["plan"] = patch.Plan,
                    ["fact"] = patch.Fact
                })
            );
        }
    }
}
