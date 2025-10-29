using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("telegram")]
public class TelegramController : ControllerBase
{
    private readonly IConfiguration _cfg;
    public TelegramController(IConfiguration cfg) { _cfg = cfg; }

    // Telegram WebApp: client sends initData string to backend for verification; backend returns JWT
    [HttpPost("init")]
    public IActionResult Init([FromForm] string initData)
    {
        // verify initData hash
        var botToken = _cfg["Telegram:BotToken"] ?? throw new Exception("Set BotToken");
        var parts = initData.Split('&', StringSplitOptions.RemoveEmptyEntries);
        var map = parts.Select(p => p.Split('=')).ToDictionary(a => a[0], a => Uri.UnescapeDataString(a[1]));
        if (!map.ContainsKey("hash")) return BadRequest("No hash");
        var hash = map["hash"];
        var dataCheckString = string.Join('\n', map.Where(kv => kv.Key != "hash").OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        var secret = SHA256.HashData(Encoding.UTF8.GetBytes(botToken));
        var hmac = new System.Security.Cryptography.HMACSHA256(secret);
        var computed = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString))).ToLowerInvariant();
        if (computed != hash) return Unauthorized("Invalid init data");
        // parse user info if present
        var userId = map.ContainsKey("user") ? map["user"] : null;
        // For MVP — return a short-lived dummy token; in real app create or find user and issue JWT
        var jwt = "PLACEHOLDER_CLIENT_JWT"; // generate real JWT linked to user/company
        return Ok(new { token = jwt });
    }
}