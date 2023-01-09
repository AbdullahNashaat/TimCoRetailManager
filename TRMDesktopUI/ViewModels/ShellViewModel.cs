using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {

        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user, IAPIHelper apiHelper)
        {
            _events = events;
            _salesVM = salesVM;
            _events.Subscribe(this);
            _user = user;
            _apiHelper= apiHelper;

            ActivateItem(IoC.Get<LoginViewModel>());
            _apiHelper = apiHelper;
        }
        public bool IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }
                return output;
            }

        }

        public void ExitApplication()
        {
           TryClose();
        }

        public void UserManagment()
        {
            ActivateItem(IoC.Get<UserDisplayViewModel>());
        }
        public void LogOut()
        {
            _user.ResetUserModel();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
            _apiHelper.LogOfUser();

        }
        public void Handle(LogOnEvent message)
        {
           ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        
    }
}
