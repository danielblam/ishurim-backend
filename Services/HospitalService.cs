using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class HospitalService(IConfiguration config)
    {
        private readonly string connectionString = config.GetConnectionString("DefaultConnection");
        private static readonly string tableName = "Hospitals";

        public List<Hospital> GetAllHospitals()
        {
            List<Hospital> hospitals = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName}", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Hospital hospital = new()
                    {
                        HospitalId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    hospitals.Add(hospital);
                }
            }
            return hospitals;
        }

        public Hospital GetHospitalById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName} WHERE Mone = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Hospital hospital = new()
                    {
                        HospitalId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    return hospital;
                }
            }
            return null;
        }

        public int CreateNewHospital(Hospital hospital)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO {tableName} (Hospital) VALUES (@name);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", hospital.Name);

            int newHospitalId = int.Parse(command.ExecuteScalar().ToString());

            return newHospitalId;
        }

        public void EditHospital(Hospital hospital)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET Hospital = @name WHERE Mone = @hospitalId", sqlCon);
            command.Parameters.AddWithValue("@name", hospital.Name);
            command.Parameters.AddWithValue("@hospitalId", hospital.HospitalId);

            command.ExecuteNonQuery();
        }

        public void DeleteHospital(int hospitalId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"DELETE FROM {tableName} WHERE Mone = @hospitalId" , sqlCon);
            command.Parameters.AddWithValue("@hospitalId", hospitalId);

            command.ExecuteNonQuery();
        }
    }
}
