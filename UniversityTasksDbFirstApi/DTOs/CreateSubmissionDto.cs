using System.ComponentModel.DataAnnotations;

namespace UniversityTasksDbFirstApi.DTOs;

public sealed record CreateSubmissionDto(
    int AssignmentId,
    int StudentId,
    [Required, Url] string RepositoryUrl);
