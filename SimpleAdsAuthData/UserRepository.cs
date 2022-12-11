using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAdsAuthData
{
    public class UserRepository
    {
        private string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }


        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public List<Ad> GetAddsByDateDesc()
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Ads ORDER BY DATE DESC";
            var result = new List<Ad>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Content = (string)reader["Content"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return result;
        }
        public List<Ad> GetAddsByDateDescByUserId(int userId)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Ads WHERE UserId = @userId ORDER BY DATE DESC";
            cmd.Parameters.AddWithValue("@userId", userId);
            var result = new List<Ad>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Content = (string)reader["Content"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return result;
        }

        public void NewAd(Ad ad)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Ads(Name, PhoneNumber, Content, Date, UserId) 
                VALUES(@name, @phoneNumber, @content, @date, @userId)";
            cmd.Parameters.AddWithValue("@name", ad.Name);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@content", ad.Content);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();

        }

        public void DeleteAd(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}


