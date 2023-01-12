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

        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                SelectedUserName = value.Email;
                UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                LoadRoles();
                NotifyOfPropertyChange(() => SelectedUser);
            }

        }
        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set 
            {
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
            
            }
        }

        private string _selectedAvailableRole;

        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);

            }
        }



        private string _selectedUserName;

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set 
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        private BindingList<string> _userRoles = new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _userRoles; }
            set 
            {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<string> _availableRoles = new BindingList<string>();

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
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
                
                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to ineract with the Sales Form");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                TryCloseAsync();

            }
        }
        private async Task LoadUsers()
        {
            var usertList = await _userEndPoint.GetAll();
            Users = new BindingList<UserModel>(usertList);
        }
        private async Task LoadRoles()
        {
            var roles = await _userEndPoint.GetAllRoles();

            foreach (var role in roles)
            {
                if(UserRoles.IndexOf(role.Value) < 0)
                {
                    AvailableRoles.Add(role.Value);
                }

            }
        }
        public async void AddSelectedRole()
        {
            await _userEndPoint.AddUserRole(SelectedUser.Id, SelectedAvailableRole);

            UserRoles.Add(SelectedAvailableRole);
            AvailableRoles.Remove(SelectedAvailableRole);

        }
        public async void RemoveSelectedRole()
        {
            await _userEndPoint.RemoveUserRole(SelectedUser.Id, SelectedUserRole);
            AvailableRoles.Add(SelectedUserRole);
            UserRoles.Remove(SelectedUserRole);

        }


    }
}
