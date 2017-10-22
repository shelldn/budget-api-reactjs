using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace Budget.Api.Categories
{
    [Route("api/budgets/{year:int}/[controller]")]
    public sealed class CategoriesController : Controller
    {
        [HttpGet]
        public IEnumerable<CategorySummary> GetAll(int year)
        {
            var client = new MongoClient("mongodb://192.168.255.129:27017");
            var db = client.GetDatabase("budgetio");
            var categories = db.GetCollection<BsonDocument>("categories");

            var summaries = categories
                .Find(c => c["budget_id"] == year)
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
            var client = new MongoClient("mongodb://192.168.255.129:27017");
            var db = client.GetDatabase("budgetio");
            var categories = db.GetCollection<BsonDocument>("categories");

            var doc = new BsonDocument
            {
                ["budget_id"] = year,
                ["name"] = category.Name,
                ["type"] = category.Type.ToString().ToLower()
            };

            categories.InsertOne(doc);

            category.Id = doc["_id"].AsObjectId.ToString();

            return Created("", category);
        }
    }
}