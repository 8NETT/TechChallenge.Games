using TechChallenge.Games.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TechChallenge.Games.Infrastructure.Configurations;

public class JogoConfiguration : IEntityTypeConfiguration<Jogo>
{
    public void Configure(EntityTypeBuilder<Jogo> builder)
    {
        builder.ToTable("Jogo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(x => x.DataCriacao).HasColumnType("DATETIME").IsRequired();
        builder.Property(x => x.Nome).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Descricao).HasColumnType("VARCHAR(500)");
        builder.Property(x => x.Valor).HasColumnType("DECIMAL(4,2)").IsRequired();  
        builder.Property(x => x.Desconto).HasColumnType("INT").IsRequired();
        builder.HasIndex(x => x.Nome).IsUnique();
    }
}