namespace UniversityTasksDbFirstApi.DTOs;

public sealed record SubmissionDto(
    int SubmissionId,
    SubmissionStudentDto Student,
    SubmissionAssignmentDto Assignment,
    string RepositoryUrl,
    string Status,
    int? Score,
    string? Feedback);

public sealed record SubmissionStudentDto(
    int StudentId,
    string IndexNumber,
    string FullName);

public sealed record SubmissionAssignmentDto(
    int AssignmentId,
    string Title,
    int MaxPoints);
