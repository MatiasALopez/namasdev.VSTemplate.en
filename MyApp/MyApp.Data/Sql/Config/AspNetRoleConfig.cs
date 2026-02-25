using System.Data.Entity.ModelConfiguration;

using MyApp.Entities;

namespace MyApp.Data.Sql.Config
{
    public class AspNetRoleConfig : EntityTypeConfiguration<AspNetRole>
    {
        public AspNetRoleConfig()
        {
            ToTable(Entities.Metadata.AspNetRoleMetadata.DB.TABLE);
            HasKey(p => p.Id);

            Property(p => p.Id)
                .HasColumnName(Entities.Metadata.AspNetRoleMetadata.DB.ID)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.AspNetRoleMetadata.Properties.Id.LENGTH_MAX);
        }
    }
}
