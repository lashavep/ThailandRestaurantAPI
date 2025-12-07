using RestaurantAPI.DTOs.AuthDTO;

namespace RestaurantAPI.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);    
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);   
        Task ForgotPasswordAsync(ForgotPasswordDTO dto);             
        Task<bool> ResetPasswordAsync(ResetPasswordDTO dto);            
    }
}
