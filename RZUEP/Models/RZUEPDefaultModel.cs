namespace RZUEP.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RZUEPDefaultModel : DbContext
    {
        public RZUEPDefaultModel()
            : base("name=RZUEPDefaultModel")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Grupies> Grupies { get; set; }
        public virtual DbSet<Plans> Plans { get; set; }
        public virtual DbSet<Proprowadzacies> Proprowadzacies { get; set; }
        public virtual DbSet<Prowadzacies> Prowadzacies { get; set; }
        public virtual DbSet<Prozajecias> Prozajecias { get; set; }
        public virtual DbSet<Zajecias> Zajecias { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plans>()
                .HasMany(e => e.Zajecias)
                .WithRequired(e => e.Plans)
                .HasForeignKey(e => e.planid);

            modelBuilder.Entity<Proprowadzacies>()
                .HasMany(e => e.Prozajecias)
                .WithRequired(e => e.Proprowadzacies)
                .HasForeignKey(e => e.proprowadzacyid);

            modelBuilder.Entity<Prozajecias>()
                .HasMany(e => e.Grupies)
                .WithRequired(e => e.Prozajecias)
                .HasForeignKey(e => e.prozajeciaid);

            modelBuilder.Entity<Zajecias>()
                .HasMany(e => e.Prowadzacies)
                .WithRequired(e => e.Zajecias)
                .HasForeignKey(e => e.zajeciaid);
        }
    }
}
