using VidizmoBackend.DTOs;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthService(IUserRepository userRepo, JwtTokenGenerator tokenGenerator)
        {
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetUserByEmailAsync(dto.Email);
            if (user == null) return null;

            if (!VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                return null;

            var (token, exp) = _tokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = exp
            };
        }

        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return storedHash.SequenceEqual(computedHash);
        }
    }
}
