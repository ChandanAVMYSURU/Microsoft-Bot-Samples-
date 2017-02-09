namespace demo.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SwaggerDemoContext : DbContext
    {
        public SwaggerDemoContext()
            : base("name=SwaggerDemoContext")
        {
        }

        public virtual DbSet<emp> emps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<emp>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<emp>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<emp>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<emp>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<emp>()
                .Property(e => e.Emp_Adress)
                .IsUnicode(false);

            modelBuilder.Entity<emp>()
                .Property(e => e.Status)
                .IsUnicode(false);
        }
    }
}
