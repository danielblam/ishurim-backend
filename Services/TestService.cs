using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class TestService(IConfiguration config)
    {
        private readonly string connectionString = config.GetConnectionString("DefaultConnection");
        private static readonly string tableName = "SugeyBdikot";

        public List<Test> GetAllTests()
        {
            List<Test> tests = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName}", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Test test = new()
                    {
                        TestId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    tests.Add(test);
                }
            }
            return tests;
        }

        public Test GetTestById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName} WHERE Mone = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Test test = new()
                    {
                        TestId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    return test;
                }
            }
            return null;
        }

        public int CreateNewTest(Test test)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO {tableName} (SugBdika) VALUES (@name);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", test.Name);

            int newTestId = int.Parse(command.ExecuteScalar().ToString());

            return newTestId;
        }

        public void EditTest(Test test)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET SugBdika = @name WHERE Mone = @testId", sqlCon);
            command.Parameters.AddWithValue("@name", test.Name);
            command.Parameters.AddWithValue("@testId", test.TestId);

            command.ExecuteNonQuery();
        }

        public void DeleteTest(int testId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"DELETE FROM {tableName} WHERE Mone = @testId", sqlCon);
            command.Parameters.AddWithValue("@testId", testId);

            command.ExecuteNonQuery();
        }
    }
}
