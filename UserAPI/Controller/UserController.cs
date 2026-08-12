using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models;

namespace UserAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("registeruser")]
    public async Task<IActionResult> Register(User user)
    {
        if (await _context.Users.AnyAsync(x => x.email == user.email))
        {
            return BadRequest("Email already registered.");
        }

        user.password = user.password;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Registration successful"
        });
    }
}