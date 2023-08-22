using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Repository
{
    public class PessoaContext : DbContext
    {
        public PessoaContext(DbContextOptions<PessoaContext>options) : base(options) { }

        public DbSet<TabUsuario> Usuarios { get; set; }

        public DbSet<TabPessoa> Pessoas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TabUsuario>().ToTable("tabUsuario");
            modelBuilder.Entity<TabPessoa>().ToTable("tabPessoa");
        }
    }
}
