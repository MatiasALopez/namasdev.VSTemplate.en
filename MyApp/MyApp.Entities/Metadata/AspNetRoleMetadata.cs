namespace MyApp.Entities.Metadata
{
    public class AspNetRoleMetadata
    {
        public const string NAME = "Role";
        public const string NAME_PLURAL = "Roles";

        public const string LABEL = "Role";
        public const string LABEL_PLURAL = "Roles";

        public class DB
        {
            public const string TABLE = "AspNetRoles";
            public const string ID = "Id";
        }

        public class Properties
        {
            public class Id
            {
                public const string LABEL = "ID";
                public const int LENGTH_MAX = 128;
            }
            public class Name
            {
                public const string LABEL = "Name";
            }
        }
    }
}
