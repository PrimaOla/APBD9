namespace UniversityTasksDbFirstApi.DTOs;

public sealed record StudentDashboardDto(
    int StudentId,
    string IndexNumber,
    string FullName,
    bool IsActive,
    IReadOnlyCollection<StudentEnrollmentDto> Enrollments,
    IReadOnlyCollection<StudentSubmissionDto> Submissions);

public sealed record StudentEnrollmentDto(
    int CourseId,
    string CourseCode,
    string CourseName,
    DateOnly EnrolledAt,
    string Status);

public sealed record StudentSubmissionDto(
    int SubmissionId,
    int AssignmentId,
    string AssignmentTitle,
    string CourseCode,
    string RepositoryUrl,
    DateTime SubmittedAt,
    string Status,
    int? Score,
    string? Feedback);
