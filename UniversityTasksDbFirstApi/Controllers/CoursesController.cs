using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/courses")]
public sealed class CoursesController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public CoursesController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CourseDto>>> GetCourses(
        [FromQuery] bool activeOnly = true)
    {
        var query = _context.Courses.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(course => course.IsActive);
        }

        var courses = await query
            .Select(course => new CourseDto(
                course.CourseId,
                course.Code,
                course.Name,
                course.Credits,
                course.Assignments.Count))
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{idCourse:int}/assignments")]
    public async Task<ActionResult<IReadOnlyCollection<AssignmentDto>>> GetAssignments(
        int idCourse,
        [FromQuery] bool publishedOnly = true)
    {
        var courseExists = await _context.Courses
            .AsNoTracking()
            .AnyAsync(course => course.CourseId == idCourse);

        if (!courseExists)
        {
            return NotFound();
        }

        var query = _context.Assignments
            .AsNoTracking()
            .Where(assignment => assignment.CourseId == idCourse);

        if (publishedOnly)
        {
            query = query.Where(assignment => assignment.IsPublished);
        }

        var assignments = await query
            .Select(assignment => new AssignmentDto(
                assignment.AssignmentId,
                assignment.Title,
                assignment.DueDate,
                assignment.MaxPoints,
                assignment.IsPublished,
                assignment.Submissions.Count))
            .ToListAsync();

        return Ok(assignments);
    }
}
