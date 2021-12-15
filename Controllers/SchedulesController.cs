using System.Security.Claims;
using Email_Scheduler_WebApi.Data;
using Email_Scheduler_WebApi.Models;
using Email_Scheduler_WebApi.Models.DTOs.Requests;
using Email_Scheduler_WebApi.Models.DTOs.Responses;
using Email_Scheduler_WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Email_Scheduler_WebApi.Controllers;

[ApiController]
[Route("/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SchedulesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SchedulerManager _schedulerManager;

    public SchedulesController(ApplicationDbContext db, UserManager<IdentityUser> userManager,
        SchedulerManager schedulerManager)
    {
        _db = db;
        _userManager = userManager;
        _schedulerManager = schedulerManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetSchedules()
    {
        var userId = User.FindFirstValue("Id");

        var currentUser = await _userManager.FindByIdAsync(userId);

        var schedules = _db.EmailSchedules?
            .Where(x => x.User == currentUser)
            .Select(schedule => new ScheduleResponseDto
            {
                Id = schedule.Id,
                Text = schedule.Text,
                Title = schedule.Title,
                SendTime = schedule.SendTime,
                SendTo = schedule.SendTo,
                IsCompleted = schedule.IsCompleted
            }).ToListAsync();

        return Ok(schedules);
    }

    [HttpGet("{id:int}")]
    public ActionResult<ScheduleDto> GetSchedule(int id)
    {
        var result = _db.EmailSchedules?.FirstOrDefault(x => x.Id == id);

        if (result == null)
        {
            return NotFound();
        }

        var schedule = new ScheduleResponseDto
        {
            Id = result.Id,
            Title = result.Title,
            Text = result.Text,
            SendTo = result.SendTo,
            SendTime = result.SendTime,
            IsCompleted = result.IsCompleted
        };

        return Ok(schedule);
    }

    [HttpPost]
    public async Task<IActionResult> AddSchedule([FromBody] ScheduleDto requestBody)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (requestBody.SendTime < DateTime.Now.AddSeconds(5))
        {
            return BadRequest("Requested Time isn't usable for Scheduling");
        }

        var userId = User.FindFirstValue("Id");
        var currentUser = await _userManager.FindByIdAsync(userId);

        await _schedulerManager.Schedule(new EmailSchedule
        {
            User = currentUser,

            Title = requestBody.Subject,

            Text = requestBody.Body,

            SendTo = requestBody.SendTo,

            SendTime = requestBody.SendTime
        });

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveSchedule(int id)
    {
        if (id < 0)
        {
            return BadRequest();
        }

        var currentSchedule = await _db.EmailSchedules?.FirstOrDefaultAsync(x => x.Id == id)!;

        if (currentSchedule == null)
        {
            return NotFound();
        }

        if (currentSchedule.IsCompleted)
        {
            return BadRequest("");
        }

        await _schedulerManager.UnSchedule(currentSchedule);

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSchedule([FromBody] UpdateScheduleDto requestBody)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (requestBody.SendTime < DateTime.Now.AddSeconds(5))
        {
            return BadRequest("Requested Time isn't usable for Scheduling");
        }

        var currentSchedule = await _db.EmailSchedules?.FirstOrDefaultAsync(x => x.Id == requestBody.Id)!;

        if (currentSchedule == null)
        {
            return NotFound();
        }

        currentSchedule.Title = requestBody.Subject;
        currentSchedule.Text = requestBody.Body;
        currentSchedule.SendTo = requestBody.SendTo;
        currentSchedule.SendTime = requestBody.SendTime;

        await _schedulerManager.UpdateSchedule(currentSchedule);

        return Ok();
    }
}