﻿using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api.ProductEndPoint;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Library.Helpers;
using System.Globalization;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndPoint _productEndPoint;
        private ISaleEndPoint _saleEndPoint;
        private IConfigHelper _configHelper;
        public SalesViewModel(IProductEndPoint ProductEndPoint,IConfigHelper configHelper, ISaleEndPoint saleEndPoint)
        {
            _configHelper= configHelper;
            _productEndPoint= ProductEndPoint;
            _saleEndPoint = saleEndPoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }
        private BindingList<ProductModel>  _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
                    
            }
        }

        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set 
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }




        private int _itemQuantity = 1 ;


        public int ItemQuantity 
        {
            get { return _itemQuantity; }
            set 
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

       
        public string SubTotal
        {
            get 
            {              
                return CalculateSubTotal().ToString("C", CultureInfo.CreateSpecificCulture("en-US")); 
            }
           
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;
            foreach (var item in Cart)
            {
                subTotal += item.Product.RetailPrice * item.QuantityInCart;
            }
            return subTotal;
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate() / 100;

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += item.Product.RetailPrice * item.QuantityInCart * taxRate;
            //    }
            //}
            return taxAmount;
        }
        public string Tax
        {
            get
            {                
                return CalculateTax().ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }

        }
        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                
                return total.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }

        }



        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                // Make sure an item is selected
                // Make sure Quantity is not 0
                if(ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity ) 
                { 
                    output = true;
                }
                return output;
            }
        }


        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null) 
            {
                existingItem.QuantityInCart += ItemQuantity;
                // HACK - There should be a better way to refresing the cart display
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else 
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }
        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                // Make sure an item is selected              
                return output;
            }
        }


        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }
        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                // Make sure somthing in cart
                if(Cart.Count > 0)
                {
                    output = true;
                }
                return output;
            }
        }


        public async Task CheckOut()
        {
            SaleModel sale= new SaleModel();

            foreach(var item in Cart) 
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                }); 
            }

            await _saleEndPoint.PostSale(sale);

        }

    }

}
