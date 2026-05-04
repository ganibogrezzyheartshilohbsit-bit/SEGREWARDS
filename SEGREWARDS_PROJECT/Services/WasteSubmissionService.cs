using System;
using SEGREWARDS_PROJECT.Data.Repositories;

namespace SEGREWARDS_PROJECT.Services
{
    public sealed class WasteSubmissionService : IWasteSubmissionService
    {
        private readonly IUserRepository _users;
        private readonly IWasteTypeRepository _wasteTypes;
        private readonly IWasteSubmissionRepository _submissions;
        private readonly IAuditLogRepository _audit;

        public WasteSubmissionService(
            IUserRepository users,
            IWasteTypeRepository wasteTypes,
            IWasteSubmissionRepository submissions,
            IAuditLogRepository audit)
        {
            _users = users;
            _wasteTypes = wasteTypes;
            _submissions = submissions;
            _audit = audit;
        }

        public WasteSubmitResult Submit(WasteSubmitRequest request)
        {
            if (request == null) return WasteSubmitResult.Fail("Invalid request.");

            if (string.IsNullOrWhiteSpace(request.StudentNumber)) return WasteSubmitResult.Fail("Student number is required.");
            if (string.IsNullOrWhiteSpace(request.FullName)) return WasteSubmitResult.Fail("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.YearLevel)) return WasteSubmitResult.Fail("Year is required.");
            if (string.IsNullOrWhiteSpace(request.Course)) return WasteSubmitResult.Fail("Course is required.");
            if (string.IsNullOrWhiteSpace(request.WasteTypeCode)) return WasteSubmitResult.Fail("Select a waste type.");

            var user = _users.GetByStudentNumber(request.StudentNumber.Trim());
            if (user == null || !user.IsActive)
            {
                return WasteSubmitResult.Fail("No account exists for that student number. Please sign up first.");
            }

            var wasteType = _wasteTypes.GetByCode(request.WasteTypeCode);
            if (wasteType == null)
            {
                return WasteSubmitResult.Fail("Unknown waste type.");
            }

            if (request.ProofImageBytes == null || request.ProofImageBytes.Length == 0)
            {
                return WasteSubmitResult.Fail("Please upload proof (JPEG/PNG, max 5 MB).");
            }

            const int maxBytes = 5 * 1024 * 1024;
            if (request.ProofImageBytes.Length > maxBytes)
            {
                return WasteSubmitResult.Fail("Proof image must be 5 MB or smaller.");
            }

            var mime = (request.ProofMimeType ?? string.Empty).ToLowerInvariant();
            if (!mime.Contains("jpeg") && !mime.Contains("jpg") && !mime.Contains("png"))
            {
                return WasteSubmitResult.Fail("Proof must be JPEG or PNG.");
            }

            long submissionId;
            try
            {
                submissionId = _submissions.InsertPending(
                    user.Id,
                    wasteType.Id,
                    request.StudentNumber.Trim(),
                    request.FullName.Trim(),
                    request.YearLevel.Trim(),
                    request.Course.Trim(),
                    request.ProofImageBytes,
                    request.ProofMimeType,
                    request.ProofFileName);
            }
            catch (Exception ex)
            {
                return WasteSubmitResult.Fail("Could not save submission: " + ex.Message);
            }

            _users.UpdateProfile(user.Id, request.FullName.Trim(), request.YearLevel.Trim(), request.Course.Trim());

            _audit.Insert(
                user.Id,
                "waste_submission_created",
                "waste_submission",
                submissionId.ToString(),
                "WasteType=" + wasteType.Code + ",Status=pending");

            return WasteSubmitResult.Ok(
                submissionId,
                "Submission saved as pending. EcoPoints will be credited after staff verification.");
        }
    }
}
