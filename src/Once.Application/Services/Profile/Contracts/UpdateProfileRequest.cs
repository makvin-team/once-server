namespace Once.Application.Services.Profile.Contracts;

public record UpdateProfileRequest(
    string  FirstName,
    string  LastName,
    string? PhoneNumber
);
