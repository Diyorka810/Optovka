using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Optovka.Model;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<UserPost> UserPosts { get; set; }
    public DbSet<ApplicationUserUserPost> ApplicationUserUserPosts { get; set; }

    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserPost>().HasKey(b => b.Id);
        modelBuilder.Entity<UserPost>().Property(b => b.Id).ValueGeneratedOnAdd();
        //modelBuilder.Entity<UserPost>().HasIndex(x => x.Title).IsUnique();
        modelBuilder.Entity<UserPost>()
                .HasOne(p => p.AuthorUser)
                .WithMany(b => b.PersonalUserPosts)
                .HasForeignKey(p => p.AuthorUserId);
        //modelBuilder.Entity<UserPost>()
        //        .HasMany(c => c.ParticipatingUsers)
        //        .WithMany(p => p.ParticipatedUserPosts);

        modelBuilder.Entity<UserPost>()
        .HasMany(e => e.ParticipatingUsers)
        .WithMany(e => e.ParticipatedUserPosts)
        .UsingEntity<ApplicationUserUserPost>(x => x.Property(y => y.TakenQuantity));
    }
}
