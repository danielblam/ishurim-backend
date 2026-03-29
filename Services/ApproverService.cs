using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class ApproverService
    {
        private static readonly string connectionString = new DbService().connectionString;

        public List<Approver> GetAllApprovers()
        {
            List<Approver> approvers = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Approvers", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Approver approver = new()
                    {
                        ApproverId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        FullName = reader.GetString(2)
                    };
                    approvers.Add(approver);
                }
            }
            return approvers;
        }

        public int CreateNewApprover(Approver approver)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"INSERT INTO Approvers (Name, FullName) VALUES (@name, @fullname);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@name", approver.Name);
            command.Parameters.AddWithValue("@fullname", approver.FullName);

            int newApproverId = int.Parse(command.ExecuteScalar().ToString());

            return newApproverId;
        }

        public void EditApprover(Approver approver)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE Approvers SET Name = @name, FullName = @fullname WHERE ApproverId = @approverId", sqlCon);
            command.Parameters.AddWithValue("@name", approver.Name);
            command.Parameters.AddWithValue("@fullname", approver.FullName);
            command.Parameters.AddWithValue("@approverId", approver.ApproverId);

            command.ExecuteNonQuery();
        }

        public void DeleteApprover(int approverId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new("DELETE FROM Approvers WHERE ApproverId = @ApproverId" , sqlCon);
            command.Parameters.AddWithValue("@ApproverId", approverId);

            command.ExecuteNonQuery();
        }
    }
}
