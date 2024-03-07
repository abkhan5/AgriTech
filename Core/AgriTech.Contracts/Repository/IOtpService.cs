namespace AgriTech;

public interface IOtpService
{
    Task<bool> IsOtpValid(string otp, CancellationToken cancellationToken);
    Task<bool> SendOtp(UInt128 phoneNumber, CancellationToken cancellationToken);
}
