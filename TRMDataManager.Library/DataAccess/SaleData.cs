using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        //public List<ProductModel> GetProducts()
        //{
        //    SqlDataAccess sql = new SqlDataAccess();

        //    var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "TRMData");
        //    return output;
        //}
        public void Savesale(SaleModel saleInfo, string cashierId) 
        {
            // TODO: make it SOLID
            
            // start fill sailDetails
            List<SaleDetailDBModel> details= new List<SaleDetailDBModel>();
            ProductData products= new ProductData();
            decimal taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails) 
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId= item.ProductId,
                    Quantity= item.Quantity

                };
                //get product details

                var productInfo = products.GetProductById(detail.ProductId);
                if(productInfo == null) 
                {
                    throw new Exception($"The product Id of { detail.ProductId} could not be found in the database");
                }

                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if(productInfo.IsTaxable)
                {
                    detail.Tax = productInfo.RetailPrice * taxRate;
                }
                

                details.Add(detail);

            }

            // create a sale
            SaleDBModel sale= new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId= cashierId
            };

            // calculate PurchasePrice, Tax and put them in sale
            sale.Total = sale.SubTotal + sale.Tax;

            

            
            using (SqlDataAccess sql = new SqlDataAccess())
            {
                try
                {
                    sql.StartTransaction("TRMData");

                    // save the sale to DB
                    sql.SaveDataInTransaction<SaleDBModel>("dbo.spSale_Insert", sale);

                    // get sale id
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Loohup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                    //finish fill sailDetails
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        //save them to DB
                        sql.SaveDataInTransaction<SaleDetailDBModel>("dbo.spSaleDetail_Insert", item);
                    }
                    sql.CommitTransaction();
                  
                }
                catch 
                {
                    sql.RollbackTransaction();
                    throw;
                }

            }
           

            

        }
    }
}
