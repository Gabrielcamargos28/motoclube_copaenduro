using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotoClubeCerrado.Models;

namespace MotoClubeCerrado.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Inscrito> Inscritos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário
            modelBuilder.Entity<Inscrito>()
                .HasOne(i => i.Categoria)
                .WithMany(c => c.Inscritos)
                .HasForeignKey(i => i.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscrito>()
                .HasOne(i => i.Etapa)
                .WithMany(e => e.Inscritos)
                .HasForeignKey(i => i.IdEtapa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Etapa>()
                .HasOne(e => e.Evento)
                .WithMany(ev => ev.Etapas)
                .HasForeignKey(e => e.IdEvento)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed de dados iniciais - Categorias
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Elite", Descricao = "Categoria Elite", Ativo = true },
                new Categoria { Id = 2, Nome = "Importada Pró", Descricao = "Categoria Importada Pró", Ativo = true },
                new Categoria { Id = 3, Nome = "Nacional Pró", Descricao = "Categoria Nacional Pró", Ativo = true },
                new Categoria { Id = 4, Nome = "Over 35", Descricao = "Categoria Over 35", Ativo = true },
                new Categoria { Id = 5, Nome = "Over 40", Descricao = "Categoria Over 40", Ativo = true },
                new Categoria { Id = 6, Nome = "Over 45", Descricao = "Categoria Over 45", Ativo = true },
                new Categoria { Id = 7, Nome = "Importada Estreante", Descricao = "Categoria Importada Estreante", Ativo = true },
                new Categoria { Id = 8, Nome = "Nacional Estreante", Descricao = "Categoria Nacional Estreante", Ativo = true },
                new Categoria { Id = 9, Nome = "Over 50", Descricao = "Categoria Over 50", Ativo = true },
                new Categoria { Id = 10, Nome = "Local", Descricao = "Categoria Local", Ativo = true },
                new Categoria { Id = 15, Nome = "Feminina", Descricao = "Categoria Feminina", Ativo = true }
            );

            // Seed de dados iniciais - Etapa
            modelBuilder.Entity<Etapa>().HasData(
                new Etapa
                {
                    Id = 16,
                    Nome = "6ª Etapa Copa Cerrado de Enduro",
                    DataEvento = new DateTime(2025, 12, 15),
                    Local = "Patrocínio - MG",
                    Descricao = "Sexta etapa da Copa Cerrado de Enduro",
                    Ativo = true,
                    InscricoesAbertas = true
                }
            );
        }
    }
}
