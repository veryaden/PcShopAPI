using Microsoft.EntityFrameworkCore;
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

        public async Task<UserProfile>? GetUserByEmail(string email)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(x => x.Mail == email);
        }

        public async Task<UserProfile> InsertUser(UserProfile user)
        {
            await _context.UserProfiles.AddAsync(user);
            return user;
        }
        public async Task<UserProfile> GetUserById(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<UserProfile> GetByResetToken(string token)
        {
            return await _context.UserProfiles
           .FirstOrDefaultAsync(u => u.ResetPasswordToken == token);
        }
        public async Task<UserProfile> GetByEmailVerifyToken(string token)
        {
            return await _context.UserProfiles
           .FirstOrDefaultAsync(u => u.EmailVerifyToken == token); 
        }

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}

