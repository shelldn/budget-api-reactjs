using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Budget.Api.Controllers
{
    public sealed class OperationsController : Controller
    {
        [HttpGet("~/api/budgets/{year:int}/[controller]")]
        public IEnumerable<int> GetAll(int year)
        {
            return new [] { 1, 2 };
        }
    }
}