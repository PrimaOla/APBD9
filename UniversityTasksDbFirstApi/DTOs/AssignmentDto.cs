namespace UniversityTasksDbFirstApi.DTOs;

public sealed record AssignmentDto(
    int AssignmentId,
    string Title,
    DateTime DueDate,
    int MaxPoints,
    bool IsPublished,
    int SubmissionCount);
