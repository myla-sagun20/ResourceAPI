using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VASAPI_Azure.Models;

namespace VASAPI_Azure.Services
{
    public interface ILoginUserService
    {
        LoginUser GetLoginUser(string userName, string password);

        LoginUser GetLoginUserById(int userId);
    }

    public class LoginUserService : ILoginUserService
    {
        private readonly VASContext _context;
        public LoginUserService(VASContext context)
        {
            _context = context;
        }

        public LoginUser GetLoginUser(string userName, string password)
        {
            var loginUser = _context.LoginUsers.FirstOrDefault(u => u.Username == userName && u.Password == password);

            return loginUser;
        }

        public LoginUser GetLoginUserById(int userId)
        {
            var loginUser = _context.LoginUsers.FirstOrDefault(u => u.Id == userId);

            return loginUser;
        }
    }
}
