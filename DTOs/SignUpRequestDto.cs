namespace VidizmoBackend.DTOs
{
    public class SignUpRequestDto
    {
        public string Email { get; set; } // User's email address
        public string Password { get; set; } // User's password
        public string Firstname { get; set; } // User's first name
        public string Lastname { get; set; } // User's last name
    }
}