namespace MyApp.Entities.Metadata
{
    public class UserMetadata
    {
        public const string NAME = "User";
        public const string NAME_PLURAL = "Users";

        public const string LABEL = "User";
        public const string LABEL_PLURAL = "Users";

        public class DB
        {
            public const string TABLE = "Users";
            public const string ID = "UserId";
        }

        public class Properties
        {
            public class UserId
            {
                public const int LENGTH_MAX = 128;
            }

            public class Email
            {
                public const string LABEL = "Email";
                public const int LENGTH_MAX = 100;
            }

            public class FirstName
            {
                public const string LABEL = "First Name";
                public const int LENGTH_MAX = 100;
            }

            public class LastName
            {
                public const string LABEL = "Last Name";
                public const int LENGTH_MAX = 100;
            }

            public class FirstNameAndLastName
            {
                public const string LABEL = "First Name / Last Name";
            }

            public class LastNameAndFirstName
            {
                public const string LABEL = "Last Name / First Name";
            }
        }

        public class Messages
        {
            public const string ADD_OK = UserMetadata.LABEL + " added successfully.";
            public const string ADD_ERROR = UserMetadata.LABEL + " add failed.";

            public const string UPDATE_OK = UserMetadata.LABEL + " updated successfully.";
            public const string UPDATE_ERROR = UserMetadata.LABEL + " update failed.";

            public const string DELETE_OK = UserMetadata.LABEL + " deleted successfully.";
            public const string DELETE_ERROR = UserMetadata.LABEL + " delete failed.";

            public const string REACTIVATE_OK = UserMetadata.LABEL + " reactivated successfully.";
            public const string REACTIVATE_ERROR = UserMetadata.LABEL + " reactivate failed.";
        }
    }
}
