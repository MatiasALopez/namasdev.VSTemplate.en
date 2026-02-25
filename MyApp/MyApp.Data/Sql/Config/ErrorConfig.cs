using System.Data.Entity.ModelConfiguration;

using MyApp.Entities;

namespace MyApp.Data.Sql.Config
{
    public class ErrorConfig : EntityTypeConfiguration<Error>
    {
        public ErrorConfig()
        {
            ToTable(Entities.Metadata.ErrorMetadata.DB.TABLE);
            HasKey(p => p.Id);

            Property(p => p.Id)
                .HasColumnName(Entities.Metadata.ErrorMetadata.DB.ID);

            Property(e => e.Message)
                .IsRequired();

            Property(e => e.StackTrace)
                .IsRequired();

            Property(e => e.Source)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.ErrorMetadata.Properties.Source.LENGTH_MAX);

            Property(e => e.UserId)
                .HasMaxLength(128);
        }
    }
}
