using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class ApproverService
    {
        private static readonly string connectionString = new DbService().connectionString;
        private static readonly string tableName = "Meashrim";

        public List<Approver> GetAllApprovers()
        {
            List<Approver> approvers = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName}", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Approver approver = new()
                    {
                        ApproverId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        FullName = reader.GetString(2),
                        Allowed = reader.GetBoolean(3)
                    };
                    approvers.Add(approver);
                }
            }
            return approvers;
        }

        public Approver GetApproverById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName} WHERE Mone = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Approver approver = new()
                    {
                        ApproverId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        FullName = reader.GetString(2)
                    };
                    return approver;
                }
            }
            return null;
        }

        public int CreateNewApprover(Approver approver)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO {tableName} (Measher, TeurMeasher, Musmah) VALUES (@name, @fullname, @allowed);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", approver.Name);
            command.Parameters.AddWithValue("@fullname", approver.FullName);
            command.Parameters.AddWithValue("@allowed", approver.Allowed);

            int newApproverId = int.Parse(command.ExecuteScalar().ToString());

            return newApproverId;
        }

        public void EditApprover(Approver approver)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET Measher = @name, TeurMeasher = @fullname, Musmah = @allowed WHERE Mone = @approverId", sqlCon);
            command.Parameters.AddWithValue("@name", approver.Name);
            command.Parameters.AddWithValue("@fullname", approver.FullName);
            command.Parameters.AddWithValue("@approverId", approver.ApproverId);
            command.Parameters.AddWithValue("@allowed", approver.Allowed);

            command.ExecuteNonQuery();
        }

        public void DeleteApprover(int approverId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"DELETE FROM {tableName} WHERE Mone = @ApproverId" , sqlCon);
            command.Parameters.AddWithValue("@ApproverId", approverId);

            command.ExecuteNonQuery();
        }
    }
}
