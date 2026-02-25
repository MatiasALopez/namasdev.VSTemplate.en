namespace MyApp.Entities.Metadata
{
    public class ErrorMetadata
    {
        public const string NAME = "Error";
        public const string NAME_PLURAL = "Errors";

        public const string LABEL = "Error";
        public const string LABEL_PLURAL = "Errors";

        public class DB
        {
            public const string TABLE = "Errors";
            public const string ID = "Id";
        }

        public class Properties
        {
            public class Message
            {
                public const string LABEL = "Message";
            }

            public class StackTrace
            {
                public const string LABEL = "Stack Trace";
            }

            public class Source
            {
                public const string LABEL = "Source";
                public const int LENGTH_MAX = 200;
            }

            public class Arguments
            {
                public const string LABEL = "Arguments";
            }

            public class DateTime
            {
                public const string LABEL = "Date/Time";
            }

            public class UserId
            {
                public const string LABEL = "User";
                public const int LENGTH_MAX = 128;
            }
        }

        public class Messages
        {
            public const string ADD_OK = ErrorMetadata.LABEL + " added successfully.";
            public const string ADD_ERROR = ErrorMetadata.LABEL + " add failed.";

            public const string UPDATE_OK = ErrorMetadata.LABEL + " updated successfully.";
            public const string UPDATE_ERROR = ErrorMetadata.LABEL + " update failed.";

            public const string DELETE_OK = ErrorMetadata.LABEL + " deleted successfully.";
            public const string DELETE_ERROR = ErrorMetadata.LABEL + " delete failed.";
        }
    }
}
