using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly IConfiguration _cfg;
    public AuthController(AuthService auth, IConfiguration cfg) { _auth = auth; _cfg = cfg; }

    [HttpPost("company/create")]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        var (company, admin) = await _auth.CreateCompanyAsync(dto.Name, dto.Password, dto.AdminDisplayName, dto.AdminTelegramId);
        // create JWT for admin
        var token = CreateJwt(admin);
        return Ok(new { companyId = company.Id, token });
    }

    private string CreateJwt(Domain.Entities.User user)
    {
        var secret = _cfg["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim("userId", user.Id.ToString()),
            new Claim("companyId", user.CompanyId.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(6), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [HttpPost("company/login")]
    public async Task<IActionResult> LoginCompany([FromBody] LoginCompanyDto dto)
    {
        // Получаем Telegram user_id из initData (или временно передаём вручную)
        var telegramId = Request.Headers.ContainsKey("X-Telegram-Id")
            ? long.Parse(Request.Headers["X-Telegram-Id"])
            : (long?)null;

        var displayName = Request.Headers["X-Telegram-Name"].FirstOrDefault() ?? "User";

        var user = await _auth.LoginToCompanyAsync(dto.Name, dto.Password, telegramId, displayName);
        if (user == null) return Unauthorized("Неверное имя компании или пароль");

        var token = CreateJwt(user);
        return Ok(new { token, companyId = user.CompanyId });
    }

}