using System.Data.Entity.ModelConfiguration;

using MyApp.Entities;

namespace MyApp.Data.Sql.Config
{
    public class ParameterConfig : EntityTypeConfiguration<Parameter>
    {
        public ParameterConfig()
        {
            ToTable(Entities.Metadata.ParameterMetadata.DB.TABLE);
            HasKey(p => p.Name);

            Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(Entities.Metadata.ParameterMetadata.Properties.Name.LENGTH_MAX);
        }
    }
}
