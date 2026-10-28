using EducaReuse.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducaReuse.Api.Infrastructure.Data.Configurations;

public sealed class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("materiais");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Nome).HasMaxLength(120).IsRequired();
        builder.Property(m => m.Descricao).HasMaxLength(500);
        builder.Property(m => m.Imagem).HasColumnType("text");
        builder.Property(m => m.Categoria).HasConversion<string>().HasMaxLength(30);
        builder.Property(m => m.EstadoConservacao).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Versao).IsRowVersion();

        builder.HasOne(m => m.Doador)
            .WithMany()
            .HasForeignKey(m => m.DoadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Solicitacoes)
            .WithOne(s => s.Material)
            .HasForeignKey(s => s.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(m => m.Solicitacoes).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(m => new { m.Status, m.Categoria });
        builder.HasIndex(m => m.DoadorId);
    }
}
