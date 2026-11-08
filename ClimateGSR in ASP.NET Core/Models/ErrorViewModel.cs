namespace ClimateGSR_in_ASP.NET_Core.Models;

/// <summary>
/// Provides request-identification details for the application's error page.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}