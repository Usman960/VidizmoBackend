using VidizmoBackend.DTOs;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace VidizmoBackend.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepo, JwtTokenGenerator tokenGenerator, IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<string?> AuthenticateAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetUserByEmailAsync(dto.Email);
            if (user == null) return null;
            
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            
            if (result != PasswordVerificationResult.Success)
                return null;

            var (token, _) = _tokenGenerator.GenerateToken(user);

            return token;
        }

        public async Task<bool> SignUpAsync(SignUpRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || 
            string.IsNullOrWhiteSpace(dto.Firstname) || string.IsNullOrWhiteSpace(dto.Lastname))
            {
                throw new ArgumentException("Invalid sign up data.");
            }

            var existingUser = await _userRepo.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = new User
            {
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            return await _userRepo.CreateUserAsync(user);
        }
    }
}
