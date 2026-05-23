using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Once.Infrastructure.Authentication;

public interface IHttpContextAccessorService
{
    bool IsRoleAdmin();
    bool InRole(string role);
    long GetId();
    string GetRoles();
    string GetUserIp();
    string GetInnOrPinfl();
    string GetUrlPath();
    string GetLang();
    int? GetBankId();
    string? GetBankBranchCode();

    long? GetOrganizationId();
}

public class HttpContextAccessorService(
    IHttpContextAccessor accessor) : IHttpContextAccessorService
{
    public bool IsRoleAdmin()
    {
        return accessor.HttpContext?.User.IsInRole("SysSuperadmin") ?? false;
    }

    public bool InRole(string name)
    {
        return accessor.HttpContext?.User.IsInRole(name) ?? false;
    }

    public long GetId()
    {
        var r = accessor.HttpContext?.User?.FindFirst(CustomClaims.Id);
        return r == null ? -1 : long.Parse(r.Value);
    }

    public string GetRoles()
    {
        var role = accessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
        return role?.Value ?? "";
    }

    public string GetUserIp()
    {
        //https://10xers.medium.com/how-to-get-the-clients-ip-address-in-net-core-when-behind-an-nginx-reverse-proxy-a128bf2a8450

        var clientIp = accessor.HttpContext?.Request.Headers["X-Real-IP"].ToString();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        return clientIp ?? string.Empty;
    }

    public string GetInnOrPinfl()
    {
        var r = accessor.HttpContext?.User?.FindFirst(CustomClaims.InnOrPinfl);
        return r == null ? "" : r.Value;
    }

    public string GetUrlPath()
    {
        return accessor.HttpContext?.Request.Path ?? string.Empty;
    }

    public string GetLang()
    {
        var r = accessor.HttpContext?.User?.FindFirst(CustomClaims.Lang);
        return r == null ? "" : r.Value;
    }

    public int? GetBankId()
    {
        var r = accessor.HttpContext?.User?.FindFirst(CustomClaims.BankId);
        return int.TryParse(r?.Value, out var bankId) ? bankId : null;
    }

    public string? GetBankBranchCode()
    {
        var r = accessor.HttpContext?.User?.FindFirst(CustomClaims.BankBranchCode);
        return r?.Value;
    }

    public long? GetOrganizationId()
    {
        var r = accessor.HttpContext?.User?.FindFirst(CustomClaims.OrganizationId);
        return r == null ? null : long.Parse(r.Value);
    }
}
