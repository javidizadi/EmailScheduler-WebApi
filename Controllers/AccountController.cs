using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Email_Scheduler_WebApi.Configuration;
using Email_Scheduler_WebApi.Models.DTOs.Requests;
using Email_Scheduler_WebApi.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Email_Scheduler_WebApi.Controllers;

public class AccountController : Controller
{
    private readonly JwtConfigs _jwtConfigs;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(IOptionsMonitor<JwtConfigs> optionsMonitor, UserManager<IdentityUser> userManager)
    {
        _jwtConfigs = optionsMonitor.CurrentValue;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserCredentialsDto body)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var user = new IdentityUser
        {
            Email = body.Email,
            UserName = body.Email
        };

        var result = await _userManager.CreateAsync(user, body.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new UserRegistrationResponseDto()
            {
                Errors = result.Errors.Select(x => x.Description),
                Success = false
            });
        }

        return Ok(new UserRegistrationResponseDto
        {
            Token = GenerateJwt(user),
            Success = true
        });
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserCredentialsDto body)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var user = await _userManager.FindByEmailAsync(body.Email);

        if (user == null)
        {
            return NotFound(new UserLoginResponseDto
            {
                Errors = new[] { "Email or Password is Incorrect" },
                Success = false
            });
        }

        if (await _userManager.CheckPasswordAsync(user, body.Password))
        {
            return Ok(new UserLoginResponseDto
            {
                Token = GenerateJwt(user),
                Success = true
            });
        }

        return NotFound(new UserLoginResponseDto
        {
            Errors = new[] { "Email or Password is Incorrect" },
            Success = false
        });
    }

    [HttpPost]
    [Route("ChangePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto body)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue("Id");
        var currentUser = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(currentUser, body.CurrentPassword, body.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(x => x.Description).ToList());
        }

        return Ok();
    }
    private string GenerateJwt(IdentityUser user)
    {
        var securityKey = new SymmetricSecurityKey(_jwtConfigs.Secret);

        var tokenDetails = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            }),

            Expires = DateTime.Now.AddHours(6),

            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDetails);

        return tokenHandler.WriteToken(token);
    }
}