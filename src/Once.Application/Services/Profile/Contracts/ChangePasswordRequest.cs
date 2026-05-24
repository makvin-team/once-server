namespace Once.Application.Services.Profile.Contracts;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
