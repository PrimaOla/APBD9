namespace UniversityTasksDbFirstApi.DTOs;

public sealed record CourseDto(
    int CourseId,
    string Code,
    string Name,
    int Credits,
    int AssignmentCount);
