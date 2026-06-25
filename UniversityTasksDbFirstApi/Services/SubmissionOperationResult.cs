using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Services;

public sealed record SubmissionOperationResult(
    SubmissionResultStatus Status,
    string? Error,
    SubmissionDto? Submission)
{
    public static SubmissionOperationResult Success(SubmissionDto? submission = null)
    {
        return new SubmissionOperationResult(SubmissionResultStatus.Success, null, submission);
    }

    public static SubmissionOperationResult BadRequest(string error)
    {
        return new SubmissionOperationResult(SubmissionResultStatus.BadRequest, error, null);
    }

    public static SubmissionOperationResult NotFound(string error)
    {
        return new SubmissionOperationResult(SubmissionResultStatus.NotFound, error, null);
    }

    public static SubmissionOperationResult Conflict(string error)
    {
        return new SubmissionOperationResult(SubmissionResultStatus.Conflict, error, null);
    }
}
