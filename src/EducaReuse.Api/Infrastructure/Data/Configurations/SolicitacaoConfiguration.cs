using EducaReuse.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducaReuse.Api.Infrastructure.Data.Configurations;

public sealed class SolicitacaoConfiguration : IEntityTypeConfiguration<Solicitacao>
{
    public void Configure(EntityTypeBuilder<Solicitacao> builder)
    {
        builder.ToTable("solicitacoes");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Mensagem).HasMaxLength(300);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(s => s.Beneficiario)
            .WithMany()
            .HasForeignKey(s => s.BeneficiarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.BeneficiarioId);
        builder.HasIndex(s => new { s.MaterialId, s.Status });
    }
}
