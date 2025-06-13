using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Models;

namespace VidizmoBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Portal> Portals { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<VideoTag> VideoTags { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPortalRole> UserPortalRoles { get; set; }
        public DbSet<ScopedToken> ScopedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organization>()
                .HasIndex(o => o.Name)
                .IsUnique();

            modelBuilder.Entity<Organization>()
                .HasOne(o => o.CreatedByUser)
                .WithOne(u => u.Organization)
                .HasForeignKey<Organization>(o => o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Video (UploadedBy)
            modelBuilder.Entity<Video>()
                .HasOne(v => v.UploadedByUser)
                .WithMany(u => u.Videos)
                .HasForeignKey(v => v.UploadedByUserId);
                

            // Organization - Portal
            modelBuilder.Entity<Portal>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Portals)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Portal - Group
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Portal)
                .WithMany(p => p.Groups)
                .HasForeignKey(g => g.PortalId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Group>()
                .HasOne(g => g.CreatedByUser)
                .WithMany(u => u.GroupsCreated)
                .HasForeignKey(g => g.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Group - UserGroup (many-to-many)
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => ug.UserGroupId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.AddedUser)
                .WithMany(u => u.UserGroupsAddedTo) 
                .HasForeignKey(ug => ug.AddedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.UserAddedByUser)
                .WithMany(u => u.UserGroupsAddedByMe)
                .HasForeignKey(ug => ug.UserAddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Video - VideoTag (many-to-many)
            modelBuilder.Entity<VideoTag>()
                .HasKey(vt => vt.VideoTagId);

            modelBuilder.Entity<VideoTag>()
                .HasOne(vt => vt.Video)
                .WithMany(v => v.VideoTags)
                .HasForeignKey(vt => vt.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VideoTag>()
                .HasOne(vt => vt.Tag)
                .WithMany(t => t.VideoTags)
                .HasForeignKey(vt => vt.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            // Role - RolePermission (many-to-many)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => rp.RolePermissionId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Portal - Role (UserPortalRole)
            modelBuilder.Entity<UserPortalRole>()
                .HasKey(upr => upr.UserPortalRoleId);

            modelBuilder.Entity<UserPortalRole>()
                .HasOne(upr => upr.User)
                .WithMany(u => u.UserPortalRoles)
                .HasForeignKey(upr => upr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // UserPortalRole - AssignedByUser
            modelBuilder.Entity<UserPortalRole>()
                .HasOne(upr => upr.AssignedByUser)
                .WithMany() // no back-navigation from User
                .HasForeignKey(upr => upr.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserPortalRole - RevokedByUser
            modelBuilder.Entity<UserPortalRole>()
                .HasOne(upr => upr.RevokedByUser)
                .WithMany() // no back-navigation from User
                .HasForeignKey(upr => upr.RevokedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<UserPortalRole>()
                .HasOne(upr => upr.Portal)
                .WithMany(p => p.UserPortalRoles)
                .HasForeignKey(upr => upr.PortalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPortalRole>()
                .HasOne(upr => upr.Role)
                .WithMany(r => r.UserPortalRoles)
                .HasForeignKey(upr => upr.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Token is created by a user (required)
            modelBuilder.Entity<ScopedToken>()
                .HasOne(st => st.CreatedByUser)
                .WithMany(u => u.TokensCreated) 
                .HasForeignKey(st => st.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Token is optionally assigned to a user
            modelBuilder.Entity<ScopedToken>()
                .HasOne(st => st.User)
                .WithMany(u => u.ScopedTokensReceived) 
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Token is always tied to a portal
            modelBuilder.Entity<ScopedToken>()
                .HasOne(st => st.Portal)
                .WithMany(p => p.ScopedTokens) 
                .HasForeignKey(st => st.PortalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
