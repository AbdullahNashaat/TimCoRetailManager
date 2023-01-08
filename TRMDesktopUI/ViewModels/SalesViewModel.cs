using System;
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
using AutoMapper;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndPoint _productEndPoint;
        private ISaleEndPoint _saleEndPoint;
        private IConfigHelper _configHelper;
        private IMapper _mapper;
        public SalesViewModel(IProductEndPoint ProductEndPoint,IConfigHelper configHelper,
            ISaleEndPoint saleEndPoint, IMapper mapper)
        {
            _configHelper= configHelper;
            _productEndPoint= ProductEndPoint;
            _saleEndPoint = saleEndPoint;
            _mapper= mapper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }
        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            // TODO - clear selectedCartItem
            await LoadProducts();
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);


        }

        private BindingList<ProductDisplayModel>  _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
                    
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set 
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null) 
            {
                existingItem.QuantityInCart += ItemQuantity;
                
            }
            else 
            {
                CartItemDisplayModel item = new CartItemDisplayModel
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
                if(SelectedCartItem!= null && SelectedCartItem?.QuantityInCart>0) 
                { 
                    output = true;
                }
                            
                return output;
            }
        }


        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;
            if(SelectedCartItem.QuantityInCart> 1) 
            { 
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanAddToCart);
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
            await ResetSalesViewModel();

        }

    }

}
