using BookStore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly BookStoreContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(BookStoreContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(SignInDto model)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return Conflict("Username already exists.");
            }
            //model.PasswordHash = model.PasswordHash;
            var result = new User
            {
                Username = model.Username,
                PasswordHash= model.PasswordHash,
                Role = model.Role
            };
             _context.Users.Add(result);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(Login model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.PasswordHash == model.PasswordHash);

        if (user == null)
        {
            return NotFound("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) 
            }),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
