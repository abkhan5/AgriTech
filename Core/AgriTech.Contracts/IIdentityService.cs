using System.Net;

namespace AgriTech.Contracts;

public interface IIdentityService
{
    public string GetUserIdentity();
    public string GetUserEmail();
    IPAddress GetIpAddress();
    public string GetUserName();
    public IPAddress GetLocalAddress();
}