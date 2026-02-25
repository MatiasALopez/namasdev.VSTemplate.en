namespace MyApp.Web.Portal.Metadata
{
    public class Text
    {
        private const string INVALID_OPERATION_FORMAT = "Invalid operation ({0}).";

        public const string INVALID_DATA = "Entered data is not valid.";

        public static string InvalidOperation(string operation)
        {
            return string.Format(INVALID_OPERATION_FORMAT, operation);
        }
    }
}