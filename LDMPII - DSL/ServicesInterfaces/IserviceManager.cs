namespace LDMPII_DSL.ServicesInterfaces
{
    public interface IserviceManager
    {
        public IAuthService authService { get; }
        public IPdfService pdfService { get; }
    }
}
