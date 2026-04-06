using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class ApprovalService
    {
        private static readonly string connectionString = new DbService().connectionString;

        public List<Approval> GetAllApprovals()
        {
            List<Approval> approvals = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM Approvals", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Approval approval = new()
                    {
                        ApprovalId = reader.GetInt32(0),
                        HospitalizationId = reader.GetString(1),
                        ApprovalDate = reader.GetFieldValue<DateOnly>(2),
                        TestId = reader.GetInt32(3),
                        FirstName = reader.GetString(4),
                        LastName = reader.GetString(5),
                        IdNumber = reader.GetString(6),
                        Department = reader.GetString(7),
                        VehicleId = reader.GetInt32(8),
                        ApproverId = reader.GetInt32(9),
                        ClerkId = reader.GetInt32(10),
                        InstituteId = reader.GetInt32(11),
                        Note = reader.GetString(12)
                    };
                    approvals.Add(approval);
                }
            }
            return approvals;
        }

        public int CreateNewApproval(Approval approval)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new(
                $"INSERT INTO Approvals (HospitalizationId, ApprovalDate, TestId, FirstName, LastName, IdNumber, Department, VehicleId, ApproverId, ClerkId, InstituteId, Note) " +
                $"VALUES (@hospitalizationId, @approvalDate, @testId, @firstName, @lastName, @idNumber, @department, @vehicleId, @approverId, @clerkId, @instituteId, @note);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@hospitalizationId", approval.HospitalizationId);
            command.Parameters.AddWithValue("@approvalDate", approval.ApprovalDate);
            command.Parameters.AddWithValue("@testId", approval.TestId);
            command.Parameters.AddWithValue("@firstName", approval.FirstName);
            command.Parameters.AddWithValue("@lastName", approval.LastName);
            command.Parameters.AddWithValue("@idNumber", approval.IdNumber);
            command.Parameters.AddWithValue("@department", approval.Department);
            command.Parameters.AddWithValue("@vehicleId", approval.VehicleId);
            command.Parameters.AddWithValue("@approverId", approval.ApproverId);
            command.Parameters.AddWithValue("@clerkId", approval.ClerkId);
            command.Parameters.AddWithValue("@instituteId", approval.InstituteId);
            command.Parameters.AddWithValue("@note", approval.Note);

            int newApprovalId = int.Parse(command.ExecuteScalar().ToString());

            return newApprovalId;
        }

        public void EditApproval(Approval approval)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE Approvals SET " +
                $"HospitalizationId = @hospitalizationId, ApprovalDate = @approvalDate, TestId = @testId, FirstName = @firstName, LastName = @lastName, " +
                $"IdNumber = @idNumber, Department = @department, VehicleId = @vehicleId, ApproverId = @approverId, ClerkId = @clerkId, InstituteId = @instituteId, Note = @note " +
                $"WHERE ApprovalId = @approvalId", sqlCon);
            command.Parameters.AddWithValue("@hospitalizationId", approval.HospitalizationId);
            command.Parameters.AddWithValue("@approvalDate", approval.ApprovalDate);
            command.Parameters.AddWithValue("@testId", approval.TestId);
            command.Parameters.AddWithValue("@firstName", approval.FirstName);
            command.Parameters.AddWithValue("@lastName", approval.LastName);
            command.Parameters.AddWithValue("@idNumber", approval.IdNumber);
            command.Parameters.AddWithValue("@department", approval.Department);
            command.Parameters.AddWithValue("@vehicleId", approval.VehicleId);
            command.Parameters.AddWithValue("@approverId", approval.ApproverId);
            command.Parameters.AddWithValue("@clerkId", approval.ClerkId);
            command.Parameters.AddWithValue("@instituteId", approval.InstituteId);
            command.Parameters.AddWithValue("@note", approval.Note);

            command.Parameters.AddWithValue("@approvalId", approval.ApprovalId);

            command.ExecuteNonQuery();
        }

        public void DeleteApproval(int approvalId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new("DELETE FROM Approvals WHERE ApprovalId = @ApprovalId" , sqlCon);
            command.Parameters.AddWithValue("@ApprovalId", approvalId);

            command.ExecuteNonQuery();
        }
    }
}
