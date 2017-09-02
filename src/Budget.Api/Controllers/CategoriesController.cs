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
                Id = 1,
                Name = "Salary",
                Type = CategoryType.Income
            };
        }
    }
}