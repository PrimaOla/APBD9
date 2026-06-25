using Microsoft.AspNetCore.Mvc;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Services;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/submissions")]
public sealed class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionDto>> CreateSubmission(CreateSubmissionDto dto)
    {
        var result = await _submissionService.CreateAsync(dto);

        if (result.Status == SubmissionResultStatus.Success && result.Submission is not null)
        {
            return Created($"/api/submissions/{result.Submission.SubmissionId}", result.Submission);
        }

        return ToActionResult(result);
    }

    [HttpPut("{idSubmission:int}/grade")]
    public async Task<ActionResult<SubmissionDto>> GradeSubmission(
        int idSubmission,
        GradeSubmissionDto dto)
    {
        var result = await _submissionService.GradeAsync(idSubmission, dto);

        if (result.Status == SubmissionResultStatus.Success && result.Submission is not null)
        {
            return Ok(result.Submission);
        }

        return ToActionResult(result);
    }

    [HttpDelete("{idSubmission:int}")]
    public async Task<IActionResult> DeleteSubmission(int idSubmission)
    {
        var result = await _submissionService.DeleteAsync(idSubmission);

        if (result.Status == SubmissionResultStatus.Success)
        {
            return NoContent();
        }

        return ToActionResult(result);
    }

    private ActionResult ToActionResult(SubmissionOperationResult result)
    {
        return result.Status switch
        {
            SubmissionResultStatus.BadRequest => Problem(result.Error, statusCode: StatusCodes.Status400BadRequest),
            SubmissionResultStatus.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
            SubmissionResultStatus.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            _ => Problem("Unexpected submission operation result.", statusCode: StatusCodes.Status400BadRequest)
        };
    }
}
