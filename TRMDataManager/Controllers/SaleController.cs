﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        public void post(SaleModel sale)
        {
            SaleData data= new SaleData();
            string uesrId = RequestContext.Principal.Identity.GetUserId();

            data.Savesale(sale, uesrId);
          
        }



        //// GET: User
        //[HttpGet]
        //public UserModel GetById()
        //{
        //    string uesrId = RequestContext.Principal.Identity.GetUserId();
        //    UserData data = new UserData();
        //    return data.GetUserById(uesrId).First();
        //}
    }
}