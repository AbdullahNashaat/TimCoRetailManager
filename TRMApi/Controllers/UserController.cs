using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.Models;
using TRMDataManager.Library.DataAccess;
using TRMApi.Models;
using System.Security.Claims;
using TRMApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
       
        private  ApplicationDbContext _context;
        private  UserManager<IdentityUser> _userManager;
        private  IConfiguration _config;

        public UserController(ApplicationDbContext context,UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        [HttpGet]
        public UserModel GetById()
        {
            string uesrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData data = new UserData(_config);
            return data.GetUserById(uesrId).First();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetALLUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
           
            var users = _context.Users.ToList();
            //var roles = _context.Roles.ToList();
            var userRoles = _context.UserRoles
                    .Join(_context.Roles, x => x.RoleId, xl => xl.Id, (x, xl) => new { x.UserId, x.RoleId, xl.Name })                  
                    .ToList();
            foreach (var user in users)
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(Key => Key.RoleId, val => val.Name);
                //foreach (var r in user.Roles)
                //{
                //    u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                //}
                output.Add(u);
            }
            
            return output;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetALLRoles()
        {
            
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return roles;
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddARole(UserRolePairModel pairing)
        {
            var role = _context.Roles.FirstOrDefault(p => p.Name == pairing.RoleName);

            IdentityUserRole<string> userRole = new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = pairing.UserId
            };

            _context.UserRoles.Add(userRole);
            _context.SaveChanges();

            //var user = await _context.Users.FindAsync(pairing.UserId);
            //await _userManager.AddToRoleAsync(user, pairing.RoleName);


        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            // TODO - send role Id Insted of Role Name
            
            
            var role = _context.Roles.FirstOrDefault(p => p.Name == pairing.RoleName);

            IdentityUserRole<string> userRole = new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = pairing.UserId
            };
            //var userRole =  _context.UserRoles
            //    .FirstOrDefault(x => x.UserId == pairing.UserId && x.RoleId == role.Id);
            
              _context.UserRoles.Remove(userRole);
            _context.SaveChanges();
            

            ////var user = await _context.Users.FindAsync(pairing.UserId);
            ////var result = await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
            ////return await UpdateUserAsync(user);

           

        }
    }
}
