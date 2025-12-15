using PcShop.Areas.Users.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Data
{
    public class AuthData:IAuthData
    {
        private readonly ExamContext _context;

        public AuthData(ExamContext context)
        {
            _context = context;
        }

        public UserProfile? GetUserByEmail(string email)
        {
            return _context.UserProfiles.FirstOrDefault(x => x.Mail == email);
        }

        public UserProfile InsertUser(UserProfile user)
        {
            _context.UserProfiles.Add(user);
            return user;
        }
        public UserProfile GetUserById(int userId)
        {
            return _context.UserProfiles.FirstOrDefault(x => x.UserId == userId);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

