using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api.ProductEndPoint;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndPoint _productEndPoint;
        public SalesViewModel(IProductEndPoint ProductEndPoint)
        {
            _productEndPoint= ProductEndPoint;
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

        private BindingList<ProductModel> _cart;

        public BindingList<ProductModel> Cart
        {
            get { return _cart; }
            set 
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }



        private int _itemQuantity;
       

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

       
        public string SubTotal
        {
            get 
            {
                // to be reblaced with calculation
                return "0.00$"; 
            }
           
        }
        public string Tax
        {
            get
            {
                // to be reblaced with calculation
                return "0.00$";
            }

        }
        public string Total
        {
            get
            {
                // to be reblaced with calculation
                return "0.00$";
            }

        }



        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                // Make sure an item is selected
                // Make sure Quantity is not 0
                return output;
            }
        }


        public void AddToCart()
        {
            
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

        }
        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                // Make sure somthing in cart
                
                return output;
            }
        }


        public void CheckOut()
        {

        }

    }

}
