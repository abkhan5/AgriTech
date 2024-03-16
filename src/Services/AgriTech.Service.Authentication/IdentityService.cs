using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AgriTech.Service.Authentication;

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    private readonly IHttpContextAccessor _context = context ?? throw new ArgumentNullException(nameof(context));
    public string GetUserIdentity() => _context?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    public string GetUserName() => _context?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
    public IPAddress? GetIpAddress() => _context?.HttpContext.Connection.RemoteIpAddress;
    public IPAddress? GetLocalAddress() => _context?.HttpContext.Connection.LocalIpAddress;
    public string GetUserEmail() => _context?.HttpContext.User.Claims.FirstOrDefault()?.Subject.Name;
}
