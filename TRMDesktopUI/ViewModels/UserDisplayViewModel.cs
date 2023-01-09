using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private  StatusInfoViewModel _status;
        private  IWindowManager _window;
        private  IUserEndPoint _userEndPoint;

        private BindingList<UserModel> _users;   

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set 
            {
                _users = value; 
                NotifyOfPropertyChange(() => Users);
            }
        }


        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window,IUserEndPoint userEndPoint)
        {
            _status = status;
            _window = window;
            _userEndPoint = userEndPoint;
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";
                TryClose();
                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to ineract with the Sales Form");
                    _window.ShowDialog(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    _window.ShowDialog(_status, null, settings);
                }

            }
        }
        private async Task LoadUsers()
        {
            var usertList = await _userEndPoint.GetAll();
            Users = new BindingList<UserModel>(usertList);
        }
            
    }
}
