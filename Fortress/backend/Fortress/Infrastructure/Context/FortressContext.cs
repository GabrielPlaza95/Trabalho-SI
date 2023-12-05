using Fortress.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortress.Infrastructure.Context
{
    public class FortressContext : DbContext
    {
        public FortressContext(DbContextOptions<FortressContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAuth> UserAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMapper());
            modelBuilder.ApplyConfiguration(new UserAuthMapper());

            CustomTableNameConventions(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void CustomTableNameConventions(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToLower());

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.GetColumnName().ToLower());

                foreach (var key in entity.GetKeys())
                    key.SetName(key.GetName().ToLower());

                foreach (var key in entity.GetForeignKeys())
                    key.SetConstraintName(key.GetConstraintName().ToLower());

                //foreach (var index in entity.GetIndexes())
                //    index.SetName(index.GetName().ToLower());
            }
        }
    }

    public class UserMapper : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("userid");
        }
    }

    public class UserAuthMapper : IEntityTypeConfiguration<UserAuth>
    {
        public void Configure(EntityTypeBuilder<UserAuth> builder)
        {
            builder.ToTable("userauth");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("userauthid");
        }
    }
}
