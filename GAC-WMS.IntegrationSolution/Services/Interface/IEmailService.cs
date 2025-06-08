namespace GAC_WMS.IntegrationSolution.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

}
