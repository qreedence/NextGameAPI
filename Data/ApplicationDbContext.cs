using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Models;
using System.Reflection.Emit;

namespace NextGameAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        //Circles
        public DbSet<Circle> Circles { get; set; }
        public DbSet<CircleMember> CircleMembers { get; set; }
        public DbSet<CircleInvitation> CircleInvitations { get; set; }
        public DbSet<GameSuggestion> GameSuggestions { get; set; }
        public DbSet<GameVote> GameVotes { get; set; }

        //Users & Auth
        public DbSet<ExternalLoginToken> ExternalLoginTokens { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<SocialLink> SocialLinks { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        //Friendships
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        //Notifications
        public DbSet<Notification> Notifications { get; set; }

        //IGDB
        public DbSet<TwitchAccessToken> TwitchAccessTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "7dca9899-d6bf-4671-8698-4d560ee110ee",
                    Name = Constants.Roles.Admin,
                    NormalizedName = Constants.Roles.Admin.ToUpper()
                },
                new IdentityRole
                {
                    Id = "78c0caeb-c37c-4192-bc48-72f30bc7e550",
                    Name = Constants.Roles.User,
                    NormalizedName = Constants.Roles.User.ToUpper()
                });

            builder.Entity<User>()
               .HasOne(u => u.Settings)
               .WithOne(us => us.User)
               .HasForeignKey<UserSettings>(us => us.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
