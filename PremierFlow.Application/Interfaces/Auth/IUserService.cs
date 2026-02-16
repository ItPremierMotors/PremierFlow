using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Auth
{
    public interface IUserService
    {
        //READS
        Task<ApiResponse<List<UsersDTO>>> GetAllAsync();
        Task<ApiResponse<UsersDTO?>>? GetByIdAsync(String id); // Returns null if user not found
        Task<ApiResponse<List<UsersDTO>>> GetByDepartmentAsync(string departamento);

        //WRITES
        Task<ApiResponse<String>> CreateAsync(CreateUserRequest request);
        Task<ApiResponse<bool>> UpdateAsync(String id, UpdateUserRequest request);
        Task<ApiResponse<bool>> ChangePasswordAsync(String id, ChangePasswordRequest request);

        //ADMIN
        Task<ApiResponse<bool>> ResetPasswordAsync(String id, ResetPasswordRequest request);
    }
}
