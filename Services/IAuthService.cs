using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(LoginRequestDto dto);
        Task<bool> SignUpAsync(SignUpRequestDto dto);
    }
}