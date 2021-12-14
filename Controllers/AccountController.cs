using System.IdentityModel.Tokens.Jwt;
using Email_Scheduler_WebApi.Configuration;
using Email_Scheduler_WebApi.Models.DTOs.Requests;
using Email_Scheduler_WebApi.Models.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                Errors = new[] {"Email or Password is Incorrect"},
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
            Errors = new[] {"Email or Password is Incorrect"},
            Success = false
        });
    }

    private string GenerateJwt(IdentityUser user)
    {
        var securityKey = new SymmetricSecurityKey(_jwtConfigs.Secret);

        var tokenDetails = new SecurityTokenDescriptor()
        {
            Claims = new Dictionary<string, object>()
            {
                {"Id", user.Id},
                {"Email", user.Email}
            },

            Expires = DateTime.Now.AddHours(6),

            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDetails);

        return tokenHandler.WriteToken(token);
    }
}