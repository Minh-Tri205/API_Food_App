using API_Food_App.Models;
using System.Security.Cryptography;
using System.Text;

namespace API_Food_App.Services.User
{
    public class UserService
    {
        private readonly FoodAppContext context;

        public UserService(FoodAppContext context)
        {
            this.context = context;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            string hashInput = HashPassword(password);

            return hashInput == hashedPassword;
        }

        // 🔒 Khóa / mở khóa tài khoản
        public bool ToggleUserStatus(int userId)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return true;
        }

        // 🔒 Khóa tài khoản luôn (force lock)
        public bool DeactivateUser(int userId)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return true;
        }

        // 🔓 Mở tài khoản
        public bool ActivateUser(int userId)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return true;
        }

        // 🔁 Đổi role user
        public bool ChangeRole(int userId, string newRole)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return false;

            // validate role theo DB constraint
            if (newRole != "customer" && newRole != "admin")
                return false;

            user.Role = newRole;
            user.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return true;
        }
    }
}