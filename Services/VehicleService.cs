using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class TestService
    {
        private static readonly string connectionString = new DbService().connectionString;

        public List<Test> GetAllTests()
        {
            List<Test> tests = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Tests", sqlCon);

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

        public int CreateNewTest(Test test)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO Tests (Name) VALUES (@name);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", test.Name);

            int newTestId = int.Parse(command.ExecuteScalar().ToString());

            return newTestId;
        }

        public void EditTest(Test test)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE Tests SET Name = @name WHERE TestId = @testId", sqlCon);
            command.Parameters.AddWithValue("@name", test.Name);
            command.Parameters.AddWithValue("@testId", test.TestId);

            command.ExecuteNonQuery();
        }

        public void DeleteTest(int testId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new("DELETE FROM Tests WHERE TestId = @testId", sqlCon);
            command.Parameters.AddWithValue("@testId", testId);

            command.ExecuteNonQuery();
        }
    }
}
