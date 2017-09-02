using System.Collections.Generic;
using Budget.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Budget.Api.Controllers
{
    public sealed class OperationsController : Controller
    {
        [HttpGet("~/api/budgets/{year:int}/[controller]")]
        public IEnumerable<OperationSummary> GetAll(int year)
        {
            yield return new OperationSummary
            {
                Id = 10,
                CategoryId = 10,
                Month = 7,
                Plan = 100.0m,
                Fact = 200.0m,
            };
        }
    }
}