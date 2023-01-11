using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SaleController(IConfiguration config)
        {
            _config = config;
        }
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void post(SaleModel sale)
        {
            SaleData data = new SaleData(_config);
            string uesrId = User.FindFirstValue(ClaimTypes.NameIdentifier);// old way - RequestContext.Principal.Identity.GetUserId();

            data.Savesale(sale, uesrId);

        }
        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            SaleData data = new SaleData(_config);
            return data.GetSaleReport();
        }
    }
}
