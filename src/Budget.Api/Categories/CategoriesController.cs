using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Budget.Api.Categories
{
    [Route("api/budgets/{year:int}/[controller]")]
    public sealed class CategoriesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CategoriesController> _log;

        public CategoriesController(IConfiguration config, ILogger<CategoriesController> log)
        {
            _config = config;
            _log = log;
        }

        [HttpGet]
        public IEnumerable<CategorySummary> GetAll(int year)
        {
            var accountId = Int32.Parse(User.FindFirstValue("sub"));
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var categories = db.GetCollection<BsonDocument>("categories");

            var summaries = categories
                .Find(c => c["account_id"] == accountId && c["budget_id"] == year)
                .ToList()
                .Select(c => new CategorySummary
                {
                    Id = c["_id"].AsObjectId.ToString(),
                    Name = c["name"].AsString,
                    Type = Enum.Parse<CategoryType>(c["type"].AsString, true)
                });

            return summaries;
        }

        [HttpPost]
        public IActionResult Create(int year, [FromBody] CategorySummary category)
        {
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var categories = db.GetCollection<BsonDocument>("categories");

            var doc = new BsonDocument
            {
                ["account_id"] = Int32.Parse(User.FindFirstValue("sub")),
                ["budget_id"] = year,
                ["name"] = category.Name,
                ["type"] = category.Type.ToString().ToLower()
            };

            categories.InsertOne(doc);

            category.Id = doc["_id"].AsObjectId.ToString();

            return Created("", category);
        }

        [HttpPatch("/api/[controller]/{id}")]
        public void Update(string id, [FromBody] CategoryNamePatch patch)
        {
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var categories = db.GetCollection<BsonDocument>("categories");

            categories.UpdateOne(
                new BsonDocument("_id", ObjectId.Parse(id)),
                new BsonDocument("$set", new BsonDocument("name", patch.Name))
            );
        }

        [HttpDelete("/api/[controller]/{id}")]
        public void Delete(string id)
        {
            var client = new MongoClient(_config["connectionString"]);
            var db = client.GetDatabase("budgetio");
            var categories = db.GetCollection<BsonDocument>("categories");

            categories.DeleteOne(new BsonDocument
            {
                ["_id"] = ObjectId.Parse(id)
            });
        }
    }
}
