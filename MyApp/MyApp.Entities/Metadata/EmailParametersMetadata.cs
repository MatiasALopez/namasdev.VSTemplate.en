namespace MyApp.Entities.Metadata
{
    public class EmailParametersMetadata
    {
        public const string NAME = "EmailParameters";
        public const string NAME_PLURAL = "EmailsParameters";

        public const string LABEL = "Email parameters";
        public const string LABEL_PLURAL = "Emails parameters";

        public class DB
        {
            public const string TABLE = "EmailsParameters";
            public const string ID = "Id";
        }

        public class Properties
        {
            public class Subject
            {
                public const string LABEL = "Subject";
                public const int LENGTH_MAX = 256;
            }

            public class Content
            {
                public const string LABEL = "Content";
            }

            public class CC
            {
                public const string LABEL = "CC";
            }
        }
    }
}
