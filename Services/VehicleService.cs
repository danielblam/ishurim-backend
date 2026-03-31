using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class VehicleService
    {
        private static readonly string connectionString = new DbService().connectionString;

        public List<Vehicle> GetAllVehicles()
        {
            List<Vehicle> vehicles = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Vehicles", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Vehicle vehicle = new()
                    {
                        VehicleId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    vehicles.Add(vehicle);
                }
            }
            return vehicles;
        }

        public Vehicle GetVehicleById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Vehicles WHERE VehicleId = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Vehicle vehicle = new()
                    {
                        VehicleId = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    return vehicle;
                }
            }
            return null;
        }

        public int CreateNewVehicle(Vehicle vehicle)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO Vehicles (Name) VALUES (@name);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", vehicle.Name);

            int newVehicleId = int.Parse(command.ExecuteScalar().ToString());

            return newVehicleId;
        }

        public void EditVehicle(Vehicle vehicle)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE Vehicles SET Name = @name WHERE VehicleId = @vehicleId", sqlCon);
            command.Parameters.AddWithValue("@name", vehicle.Name);
            command.Parameters.AddWithValue("@vehicleId", vehicle.VehicleId);

            command.ExecuteNonQuery();
        }

        public void DeleteVehicle(int vehicleId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new("DELETE FROM Vehicles WHERE VehicleId = @vehicleId", sqlCon);
            command.Parameters.AddWithValue("@vehicleId", vehicleId);

            command.ExecuteNonQuery();
        }
    }
}
