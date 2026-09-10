using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class InstituteService(IConfiguration config)
    {
        private readonly string connectionString = config.GetConnectionString("DefaultConnection");
        private static readonly string tableName = "Mehonim";

        public List<Institute> GetAllInstitutes()
        {
            List<Institute> institutes = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName}", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Institute institute = new()
                    {
                        InstituteId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        HospitalId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                        Active = reader.GetBoolean(3)
                    };
                    institutes.Add(institute);
                }
            }
            return institutes;
        }

        public Institute GetInstituteById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName} WHERE Mone = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Institute institute = new()
                    {
                        InstituteId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        HospitalId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                        Active = reader.GetBoolean(3)
                    };
                    return institute;
                }
            }
            return null;
        }


        public int CreateNewInstitute(Institute institute)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO {tableName} (Mahon, Hospital, Active) VALUES (@name, @hospitalId, 1);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", institute.Name);
            command.Parameters.AddWithValue("@hospitalId", (object?)institute.HospitalId ?? DBNull.Value);

            int newInstituteId = int.Parse(command.ExecuteScalar().ToString());

            return newInstituteId;
        }

        public void EditInstitute(Institute institute)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET Mahon = @name, Hospital = @hospitalId WHERE Mone = @instituteId", sqlCon);
            command.Parameters.AddWithValue("@name", institute.Name);
            command.Parameters.AddWithValue("@hospitalId", (object?)institute.HospitalId ?? DBNull.Value);
            command.Parameters.AddWithValue("@instituteId", institute.InstituteId);

            command.ExecuteNonQuery();
        }

        public void DeleteInstitute(int instituteId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"DELETE FROM {tableName} WHERE Mone = @instituteId" , sqlCon);
            command.Parameters.AddWithValue("@instituteId", instituteId);

            command.ExecuteNonQuery();
        }

        public int SetActive(int instituteId, bool setActive)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET Active = @active WHERE Mone = @instituteId", sqlCon);
            command.Parameters.AddWithValue("@active", setActive);
            command.Parameters.AddWithValue("@instituteId", instituteId);

            try
            {
                command.ExecuteNonQuery();
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
