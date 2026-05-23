namespace Once.Application.Services.Positions.Contracts;

public record PositionResponse(
    long     Id,
    string   Name,
    string   Code,
    DateTime CreatedAt,
    int      UsersCount
);
