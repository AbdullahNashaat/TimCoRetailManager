﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api.ProductEndPoint
{
    public interface IProductEndPoint
    {
        Task<List<ProductModel>> GetAll();
    }
}