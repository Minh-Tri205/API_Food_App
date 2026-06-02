namespace API_Food_App.Dto
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PasswordHash { get; set; }
        public string? AvatarUrl { get; set; }
        public string? DeviceToken { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public int? LoyaltyPoints { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
