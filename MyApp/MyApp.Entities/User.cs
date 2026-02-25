using System;
using System.Collections.Generic;
using System.Linq;

using namasdev.Core.Entity;

namespace MyApp.Entities
{
    public partial class User : EntityCreatedModifiedDeleted<string>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string FullNameInverted { get; set; }

        public virtual List<AspNetRole> Roles { get; set; }

        public bool HasRole(string roleName)
        {
            return Roles != null
                && Roles.Any(r => string.Equals(r.Name, roleName, StringComparison.CurrentCultureIgnoreCase));
        }

        public override string ToString()
        {
            return FullNameInverted;
        }
    }
}
