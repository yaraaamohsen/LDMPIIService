namespace LDMPII_DSL.ServicesInterfaces
{
    public interface IAuthService
    {
        //public string GetTokenUrl();
        Task<string?> GetTokenAsync();
    }
}
