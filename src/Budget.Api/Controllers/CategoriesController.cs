using System.Collections.Generic;
using Budget.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Budget.Api.Controllers
{
    [Route("api/budgets/{year:int}/[controller]")]
    public sealed class CategoriesController : Controller
    {
        [HttpGet]
        public IEnumerable<CategorySummary> GetAll(int year)
        {
            yield return new CategorySummary
            {
                Id = 10,
                Name = "Salary",
                Type = CategoryType.Income
            };

            yield return new CategorySummary
            {
                Id = 15,
                Name = "Cryptocurrencies",
                Type = CategoryType.Income
            };

            yield return new CategorySummary
            {
                Id = 20,
                Name = "Food",
                Type = CategoryType.Outgo
            };

            yield return new CategorySummary
            {
                Id = 30,
                Name = "Books",
                Type = CategoryType.Outgo
            };
        }
    }
}