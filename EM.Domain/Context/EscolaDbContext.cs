using Microsoft.EntityFrameworkCore;
using EM.Domain.Models;

namespace EM.Domain.Context
{
    public class EscolaDbContext : DbContext
    {
        public EscolaDbContext(DbContextOptions<EscolaDbContext> options) : base(options) { }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Cidade> Cidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.ToTable("TBALUNO");
                entity.HasKey(e => e.AlunoMatricula);
                entity.Property(e => e.AlunoMatricula).HasColumnName("MATRICULA");
                entity.Property(e => e.AlunoNome).HasColumnName("NOME");
                entity.Property(e => e.AlunoCPF).HasColumnName("CPF");
                entity.Property(e => e.AlunoNascimento).HasColumnName("NASCIMENTO");
                entity.Property(e => e.AlunoSexo).HasColumnName("SEXO");
                entity.Property(e => e.AlunoCidaCodigo).HasColumnName("CIDACODIGO");
            });
            modelBuilder.Entity<Cidade>(entity =>
            {
                entity.ToTable("TBCIDADE");
                entity.HasKey(e => e.CIDACODIGO);
                entity.Property(e => e.CIDACODIGO).HasColumnName("CIDACODIGO");
                entity.Property(e => e.CIDADESCRICAO).HasColumnName("CIDADESCRICAO");
                entity.Property(e => e.CIDAUF).HasColumnName("CIDAUF");
                entity.Property(e => e.CIDACODIGOIBGE).HasColumnName("CIDACODIGOIBGE");
            });
        }
    }
}