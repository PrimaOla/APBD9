using System.ComponentModel.DataAnnotations;

namespace UniversityTasksDbFirstApi.DTOs;

public sealed record GradeSubmissionDto(
    [Range(0, int.MaxValue)] int Score,
    string? Feedback);
