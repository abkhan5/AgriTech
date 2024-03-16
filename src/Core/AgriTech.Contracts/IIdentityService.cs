using System.Net;

namespace AgriTech;

public interface IIdentityService
{
    public string GetUserIdentity();
    public string GetUserEmail();
    IPAddress GetIpAddress();
    public string GetUserName();
    public IPAddress GetLocalAddress();
}