using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Services;

public sealed class SubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<SubmissionOperationResult> CreateAsync(CreateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) ||
            !dto.RepositoryUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return SubmissionOperationResult.BadRequest("RepositoryUrl must start with https://.");
        }

        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.StudentId == dto.StudentId);

        if (student is null)
        {
            return SubmissionOperationResult.NotFound("Student was not found.");
        }

        if (!student.IsActive)
        {
            return SubmissionOperationResult.BadRequest("Student is not active.");
        }

        var assignment = await _context.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(assignment => assignment.AssignmentId == dto.AssignmentId);

        if (assignment is null)
        {
            return SubmissionOperationResult.NotFound("Assignment was not found.");
        }

        if (!assignment.IsPublished)
        {
            return SubmissionOperationResult.BadRequest("Assignment is not published.");
        }

        var isEnrolled = await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(enrollment =>
                enrollment.StudentId == dto.StudentId &&
                enrollment.CourseId == assignment.CourseId &&
                (enrollment.Status == EnrollmentStatuses.Active ||
                 enrollment.Status == EnrollmentStatuses.Completed));

        if (!isEnrolled)
        {
            return SubmissionOperationResult.BadRequest("Student is not enrolled in assignment course.");
        }

        var alreadySubmitted = await _context.Submissions
            .AsNoTracking()
            .AnyAsync(submission =>
                submission.StudentId == dto.StudentId &&
                submission.AssignmentId == dto.AssignmentId);

        if (alreadySubmitted)
        {
            return SubmissionOperationResult.Conflict("Student already submitted this assignment.");
        }

        var now = DateTime.Now;
        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt = now,
            Status = assignment.IsOverdue(now)
                ? SubmissionStatuses.Late
                : SubmissionStatuses.Submitted
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        var created = await GetSubmissionDtoAsync(submission.SubmissionId);
        return SubmissionOperationResult.Success(created);
    }

    public async Task<SubmissionOperationResult> GradeAsync(int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _context.Submissions
            .Include(submission => submission.Assignment)
            .Include(submission => submission.Student)
            .FirstOrDefaultAsync(submission => submission.SubmissionId == submissionId);

        if (submission is null)
        {
            return SubmissionOperationResult.NotFound("Submission was not found.");
        }

        if (dto.Score > submission.Assignment.MaxPoints)
        {
            return SubmissionOperationResult.BadRequest("Score cannot be greater than assignment max points.");
        }

        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status = SubmissionStatuses.Graded;

        await _context.SaveChangesAsync();

        return SubmissionOperationResult.Success(ToDto(submission));
    }

    public async Task<SubmissionOperationResult> DeleteAsync(int submissionId)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(submission => submission.SubmissionId == submissionId);

        if (submission is null)
        {
            return SubmissionOperationResult.NotFound("Submission was not found.");
        }

        if (submission.Status == SubmissionStatuses.Graded)
        {
            return SubmissionOperationResult.BadRequest("Graded submission cannot be deleted.");
        }

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();

        return SubmissionOperationResult.Success();
    }

    private async Task<SubmissionDto?> GetSubmissionDtoAsync(int submissionId)
    {
        var submission = await _context.Submissions
            .AsNoTracking()
            .Include(submission => submission.Student)
            .Include(submission => submission.Assignment)
            .FirstOrDefaultAsync(submission => submission.SubmissionId == submissionId);

        return submission is null ? null : ToDto(submission);
    }

    private static SubmissionDto ToDto(Submission submission)
    {
        return new SubmissionDto(
            submission.SubmissionId,
            new SubmissionStudentDto(
                submission.Student.StudentId,
                submission.Student.IndexNumber,
                submission.Student.FullName),
            new SubmissionAssignmentDto(
                submission.Assignment.AssignmentId,
                submission.Assignment.Title,
                submission.Assignment.MaxPoints),
            submission.RepositoryUrl,
            submission.Status,
            submission.Score,
            submission.Feedback);
    }
}
