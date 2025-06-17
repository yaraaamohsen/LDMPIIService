namespace LDMPII_Helper.CustomExceptions.DatabaseExceptions
{
    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException(string message) : base(message) { }
        public DatabaseOperationException(string message, Exception ex) : base(message, ex) { }
    }
}
