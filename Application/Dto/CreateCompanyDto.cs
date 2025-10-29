namespace Application.Dto;
public record CreateCompanyDto(string Name, string Password, string AdminDisplayName, long? AdminTelegramId);