using System.ComponentModel.DataAnnotations.Schema;

using namasdev.Data.Entity.Config;

using MyApp.Entities;

namespace MyApp.Data.Sql.Config
{
    public class UserConfig : EntityDeletedConfigBase<User>
    {
        public UserConfig()
        {
            ToTable(Entities.Metadata.UserMetadata.DB.TABLE);
            HasKey(p => p.Id);

            Property(p => p.Id)
                .HasColumnName(Entities.Metadata.UserMetadata.DB.ID)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.UserMetadata.Properties.UserId.LENGTH_MAX);

            Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.UserMetadata.Properties.Email.LENGTH_MAX);

            Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.UserMetadata.Properties.FirstName.LENGTH_MAX);

            Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.UserMetadata.Properties.LastName.LENGTH_MAX);

            Property(e => e.FullNameInverted)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(e => e.FullName)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            HasMany(s => s.Roles)
                .WithMany()
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("RoleId");
                    cs.ToTable("AspNetUserRoles");
                });
        }
    }
}
