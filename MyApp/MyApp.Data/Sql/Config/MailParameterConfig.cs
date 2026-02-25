using System.Data.Entity.ModelConfiguration;

using MyApp.Entities;

namespace MyApp.Data.Sql.Config
{
    public class MailParameterConfig : EntityTypeConfiguration<EmailParameters>
    {
        public MailParameterConfig()
        {
            ToTable(Entities.Metadata.EmailParametersMetadata.DB.TABLE);
            HasKey(p => p.Id);

            Property(p => p.Id)
                .HasColumnName(Entities.Metadata.EmailParametersMetadata.DB.ID);

            Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.EmailParametersMetadata.Properties.Subject.LENGTH_MAX);
        }
    }
}
