using LDMPII_DSL.ServicesInterfaces;

namespace LDMPII_DSL.Services
{
    public class ServiceManager : IserviceManager
    {
        public IAuthService authService => throw new NotImplementedException();

        public IPdfService pdfService => throw new NotImplementedException();
    }
}
