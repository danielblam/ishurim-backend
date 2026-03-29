using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class HospitalService
    {
        private static readonly string connectionString = new DbService().connectionString;

        public List<Hospital> GetAllHospitals()
        {
            List<Hospital> hospitals = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Hospitals", sqlCon);

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

        public int CreateNewHospital(Hospital hospital)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO Hospitals (Name) VALUES (@name);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", hospital.Name);

            int newHospitalId = int.Parse(command.ExecuteScalar().ToString());

            return newHospitalId;
        }

        public void EditHospital(Hospital hospital)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE Hospitals SET Name = @name WHERE HospitalId = @hospitalId", sqlCon);
            command.Parameters.AddWithValue("@name", hospital.Name);
            command.Parameters.AddWithValue("@hospitalId", hospital.HospitalId);

            command.ExecuteNonQuery();
        }

        public void DeleteHospital(int hospitalId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new("DELETE FROM Hospitals WHERE HospitalId = @hospitalId" , sqlCon);
            command.Parameters.AddWithValue("@hospitalId", hospitalId);

            command.ExecuteNonQuery();
        }
    }
}
