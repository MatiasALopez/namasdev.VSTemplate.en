namespace MyApp.Entities.Metadata
{
    public class ParameterMetadata
    {
        public const string NAME = "Parameter";
        public const string NAME_PLURAL = "Parameters";

        public const string LABEL = "Parameter";
        public const string LABEL_PLURAL = "Parameters";

        public class DB
        {
            public const string TABLE = "Parameters";
            public const string ID = "Name";
        }

        public class Properties
        {
            public class Name
            {
                public const string LABEL = "Name";
                public const int LENGTH_MAX = 100;
            }

            public class Value
            {
                public const string LABEL = "Value";
            }
        }
    }
}
