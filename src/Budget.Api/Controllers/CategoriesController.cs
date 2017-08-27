using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Budget.Api.Controllers
{
    [Route("api/[controller]")]
    public sealed class CategoriesController
    {
        [HttpGet]
        public IEnumerable<string> GetAll()
        {
            return new[] { "Salary", "Food" };
        }
    }
}