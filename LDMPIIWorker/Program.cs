using LDMPII_DAL;
using LDMPII_DSL.Services;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities.Authentication;

namespace LDMPIIWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient();
                    services.Configure<TokenCredentials>(context.Configuration.GetSection("TokenCredentials"));

                    services.AddSingleton<IAuthService, AuthService>();
                    services.AddScoped<IPdfService, PdfService>();
                    services.AddScoped<IGhAttachmentService, GhAttachmentService>();
                    services.AddScoped<IGhAttachmentDAL, GhAttachmentDAL>();

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}