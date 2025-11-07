using Microsoft.EntityFrameworkCore;
using SGCM.Models;

namespace SGCM.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Consulta> Consultas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CPF).IsUnique();
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CPF).IsRequired().HasMaxLength(14);
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CRM).IsUnique();
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CRM).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Consulta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Paciente).WithMany().HasForeignKey("PacienteId");
                entity.HasOne(e => e.Medico).WithMany().HasForeignKey("MedicoId");
            });
        }
    }
}