using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class DepartmentService(IConfiguration config)
    {
        private readonly string connectionString = config.GetConnectionString("DefaultConnection");
        private static readonly string tableName = "Mahlakot";

        public List<Department> GetAllDepartments()
        {
            List<Department> departments = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName}", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Department department = new()
                    {
                        DepartmentId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    departments.Add(department);
                }
            }
            return departments;
        }

        public Department GetDepartmentById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName} WHERE Mone = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Department department = new()
                    {
                        DepartmentId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    return department;
                }
            }
            return null;
        }

        public int CreateNewDepartment(Department department)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO {tableName} (Mahlaka) VALUES (@name);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", department.Name);

            int newDepartmentId = int.Parse(command.ExecuteScalar().ToString());

            return newDepartmentId;
        }

        public void EditDepartment(Department department)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET Mahlaka = @name WHERE Mone = @departmentId", sqlCon);
            command.Parameters.AddWithValue("@name", department.Name);
            command.Parameters.AddWithValue("@departmentId", department.DepartmentId);

            command.ExecuteNonQuery();
        }

        public void DeleteDepartment(int departmentId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"DELETE FROM {tableName} WHERE Mone = @departmentId" , sqlCon);
            command.Parameters.AddWithValue("@departmentId", departmentId);

            command.ExecuteNonQuery();
        }
    }
}
