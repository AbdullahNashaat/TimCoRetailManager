using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles ="Cashier")]
        [HttpPost]
        public void post(SaleModel sale)
        {
            SaleData data= new SaleData();
            string uesrId = RequestContext.Principal.Identity.GetUserId();

            data.Savesale(sale, uesrId);
          
        }
        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            SaleData data= new SaleData();
            return data.GetSaleReport();
        }      
    }
}
