using PremierFlow.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Auth
{
    public interface IUserService
    {
        //READS
        Task<List<UsersDTO>> GetAllAsync();
        Task<UsersDTO?>? GetByIdAsync(String id); // Returns null if user not found

        //WRITES
        Task<String> CreateAsync(CreateUserRequest request);
        Task<bool> UpdateAsync(String id, UpdateUserRequest request);
        Task<bool> ChangePasswordAsync(String id, ChangePasswordRequest request);

        //ADMIN
        Task<bool> ResetPasswordAsync(String id, ResetPasswordRequest request);
    }
}
