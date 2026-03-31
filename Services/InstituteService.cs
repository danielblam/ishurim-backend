using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class InstituteService
    {
        private static readonly string connectionString = new DbService().connectionString;

        public List<Institute> GetAllInstitutes()
        {
            List<Institute> institutes = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Institutes", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Institute institute = new()
                    {
                        InstituteId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        HospitalId = reader.GetInt32(2)
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
                SqlCommand command = new($"SELECT * FROM Institutes WHERE InstituteId = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Institute institute = new()
                    {
                        InstituteId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        HospitalId = reader.GetInt32(2)
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

            SqlCommand command = new($"INSERT INTO Institutes (Name, HospitalId) VALUES (@name, @hospitalId);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", institute.Name);
            command.Parameters.AddWithValue("@hospitalId", institute.HospitalId);

            int newInstituteId = int.Parse(command.ExecuteScalar().ToString());

            return newInstituteId;
        }

        public void EditInstitute(Institute institute)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE Institutes SET Name = @name, HospitalId = @hospitalId WHERE InstituteId = @instituteId", sqlCon);
            command.Parameters.AddWithValue("@name", institute.Name);
            command.Parameters.AddWithValue("@hospitalId", institute.HospitalId);
            command.Parameters.AddWithValue("@instituteId", institute.InstituteId);

            command.ExecuteNonQuery();
        }

        public void DeleteInstitute(int instituteId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new("DELETE FROM Institutes WHERE InstituteId = @InstituteId" , sqlCon);
            command.Parameters.AddWithValue("@InstituteId", instituteId);

            command.ExecuteNonQuery();
        }
    }
}
