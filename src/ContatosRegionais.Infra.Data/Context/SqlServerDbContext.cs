using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Infra.Data.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ContatosRegionais.Infra.Data.Context;

public class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : DbContext(options)
{
    public DbSet<Contato> Contatos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contato>(new ContatoMap().Configure);
    }
}
