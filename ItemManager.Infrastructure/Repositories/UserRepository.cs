using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbHelper _dbHelper;

        public UserRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT UserId, Username, Password, Role, CreatedDate
                              From Users 
                              WHERE Username = @Username";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserID = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    };
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occured while fetching Username.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Username.", ex);
            }
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT UserId, Username, Password, Role, CreatedDate
                              FROM Users
                              WHERE Username = @Username
                              AND Password = @Password";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserID = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    };
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occured while validating user.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while validating user.", ex);
            }
        }
    }
}
