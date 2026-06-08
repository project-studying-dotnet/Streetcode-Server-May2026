namespace Streetcode.Common.Events;

public record UserRegisteredEvent(
    int UserId,
    string Email,
    string Username,
    string Name,
    string Surname);