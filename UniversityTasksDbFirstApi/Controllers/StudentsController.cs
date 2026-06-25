using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/students")]
public sealed class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public StudentsController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idStudent:int}/dashboard")]
    public async Task<ActionResult<StudentDashboardDto>> GetDashboard(int idStudent)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course)
            .Include(student => student.Submissions)
            .ThenInclude(submission => submission.Assignment)
            .ThenInclude(assignment => assignment.Course)
            .FirstOrDefaultAsync(student => student.StudentId == idStudent);

        if (student is null)
        {
            return NotFound();
        }

        var dto = new StudentDashboardDto(
            student.StudentId,
            student.IndexNumber,
            student.FullName,
            student.IsActive,
            student.Enrollments
                .Select(enrollment => new StudentEnrollmentDto(
                    enrollment.CourseId,
                    enrollment.Course.Code,
                    enrollment.Course.Name,
                    enrollment.EnrolledAt,
                    enrollment.Status))
                .ToList(),
            student.Submissions
                .Select(submission => new StudentSubmissionDto(
                    submission.SubmissionId,
                    submission.AssignmentId,
                    submission.Assignment.Title,
                    submission.Assignment.Course.Code,
                    submission.RepositoryUrl,
                    submission.SubmittedAt,
                    submission.Status,
                    submission.Score,
                    submission.Feedback))
                .ToList());

        return Ok(dto);
    }
}
