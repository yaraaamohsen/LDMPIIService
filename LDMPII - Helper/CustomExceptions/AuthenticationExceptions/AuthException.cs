namespace LDMPII_Helper.CustomExceptions.AuthenticationExceptions
{
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message) { }
        public AuthException(string message, Exception ex) : base(message, ex) { }
    }
}
