using ContatosRegionais.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ContatosRegionais.Infra.Data.Mapping;

public class ContatoMap : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.ToTable("Contato");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasColumnName("Nome")
            .HasColumnType("varchar(100)");

        builder.OwnsOne(x => x.Email, a =>
        {
            a.Property(p => p.Endereco)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType("varchar(60)");
        });

        builder.Property(x => x.Telefone)
                        .IsRequired()
                        .HasColumnName("Telefone")
                        .HasColumnType("varchar(50)");
    }
}
