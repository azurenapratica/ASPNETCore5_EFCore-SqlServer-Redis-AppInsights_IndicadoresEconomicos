using Microsoft.EntityFrameworkCore;
using APIIndicadores.Models;

namespace APIIndicadores.Data
{
    public class IndicadoresContext : DbContext
    {
        public DbSet<Indicador> Indicadores { get; set; }
        public DbSet<BolsaValores> BolsasValores { get; set; }

        public IndicadoresContext(DbContextOptions<IndicadoresContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Indicador>()
                .HasKey(i => i.Sigla);
            modelBuilder.Entity<BolsaValores>()
                .HasKey(b => b.Sigla);
        }
    }
}