using Azure.Core;
using Ishurim.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class ApprovalService
    {
        private static readonly string connectionString = new DbService().connectionString;
        private static readonly string tableName = "Ishurim";

        public List<Approval> GetAllApprovals()
        {
            List<Approval> approvals = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();
                SqlCommand command = new($"SELECT * FROM {tableName}", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Approval approval = new()
                    {
                        ApprovalId = reader.GetInt32(0),
                        HospitalizationId = reader.IsDBNull("MisparIshpuz") ? null : reader.GetString("MisparIshpuz"),
                        Date = reader.IsDBNull("MisparIshpuz") ? null : DateOnly.FromDateTime(reader.GetFieldValue<DateTime>("Taarih")),
                        TestId = reader.GetInt32("SugBdika"),
                        TestCode = reader.IsDBNull("KodBdika") ? null : reader.GetString("KodBdika"),
                        FirstName = reader.IsDBNull("Shem") ? null : reader.GetString("Shem"),
                        LastName = reader.IsDBNull("Mishpaha") ? null : reader.GetString("Mishpaha"),
                        IdNumber = reader.GetInt32("ID"),
                        DepartmentId = reader.IsDBNull("Mahlaka") ? null : reader.GetInt32("Mahlaka"),
                        VehicleId = reader.IsDBNull("CliTahbura") ? null : reader.GetInt32("CliTahbura"),
                        ApproverId = reader.GetInt32("Measher"),
                        Clerk = reader.IsDBNull("Pakid") ? null : reader.GetString("Pakid"),
                        InstituteId = reader.GetInt32("Mahon"),
                        Note = reader.IsDBNull("Heara") ? null : reader.GetString("Heara")
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
                $"INSERT INTO {tableName} (MisparIshpuz, Taarih, SugBdika, KodBdika, Shem, Mishpaha, ID, Mahlaka, CliTahbura, Measher, Pakid, Mahon, Heara) " +
                $"VALUES (@hospitalizationId, @approvalDate, @testId, @testCode, @firstName, @lastName, @idNumber, @department, @vehicleId, @approverId, @clerk, @instituteId, @note);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@hospitalizationId", approval.HospitalizationId);
            command.Parameters.AddWithValue("@approvalDate", approval.Date?.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@testId", approval.TestId);
            command.Parameters.AddWithValue("@testCode", approval.TestCode);
            command.Parameters.AddWithValue("@firstName", approval.FirstName);
            command.Parameters.AddWithValue("@lastName", approval.LastName);
            command.Parameters.AddWithValue("@idNumber", approval.IdNumber);
            command.Parameters.AddWithValue("@department", approval.DepartmentId);
            command.Parameters.AddWithValue("@vehicleId", approval.VehicleId);
            command.Parameters.AddWithValue("@approverId", approval.ApproverId);
            command.Parameters.AddWithValue("@clerk", approval.Clerk);
            command.Parameters.AddWithValue("@instituteId", approval.InstituteId);
            command.Parameters.AddWithValue("@note", approval.Note);

            int newApprovalId = int.Parse(command.ExecuteScalar().ToString());

            return newApprovalId;
        }

        public void EditApproval(Approval approval)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"UPDATE {tableName} SET " +
                $"MisparIshpuz = @hospitalizationId, Taarih = @approvalDate, SugBdika = @testId, KodBdika = @testCode, Shem = @firstName, Mishpaha = @lastName, " +
                $"ID = @idNumber, Mahlaka = @department, CliTahbura = @vehicleId, Measher = @approverId, Pakid = @clerk, Mahon = @instituteId, Heara = @note " +
                $"WHERE MisparShover = @approvalId", sqlCon);
            command.Parameters.AddWithValue("@hospitalizationId", approval.HospitalizationId);
            command.Parameters.AddWithValue("@approvalDate", approval.Date?.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@testId", approval.TestId);
            command.Parameters.AddWithValue("@testCode", approval.TestCode);
            command.Parameters.AddWithValue("@firstName", approval.FirstName);
            command.Parameters.AddWithValue("@lastName", approval.LastName);
            command.Parameters.AddWithValue("@idNumber", approval.IdNumber);
            command.Parameters.AddWithValue("@department", approval.DepartmentId);
            command.Parameters.AddWithValue("@vehicleId", approval.VehicleId);
            command.Parameters.AddWithValue("@approverId", approval.ApproverId);
            command.Parameters.AddWithValue("@clerk", approval.Clerk);
            command.Parameters.AddWithValue("@instituteId", approval.InstituteId);
            command.Parameters.AddWithValue("@note", approval.Note);

            command.Parameters.AddWithValue("@approvalId", approval.ApprovalId);

            command.ExecuteNonQuery();
        }

        public void DeleteApproval(int approvalId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"DELETE FROM {tableName} WHERE MisparShover = @ApprovalId", sqlCon);
            command.Parameters.AddWithValue("@ApprovalId", approvalId);

            command.ExecuteNonQuery();
        }
    }
}
