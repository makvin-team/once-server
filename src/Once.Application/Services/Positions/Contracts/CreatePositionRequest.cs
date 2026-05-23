namespace Once.Application.Services.Positions.Contracts;

public record CreatePositionRequest(
    string Name,
    string Code
);
