namespace VidizmoBackend.Models
{
    public class Organization
    {
        public int OrganizationId { get; set; } // Unique identifier for the organization
        public string Name { get; set; } // Name of the organization
        public string Description { get; set; } // Description of the organization
        public DateTime CreatedAt { get; set; } // Timestamp when the organization was created

        public int CreatedByUserId { get; set; } // FK to User
        public User CreatedByUser { get; set; } // Navigation to User

        public ICollection<UserOgGpRole> UserOgGpRoles { get; set; } // Collection of user roles in this org
        public ICollection<Video> Videos { get; set; } // Collection of videos associated with this org
        public ICollection<Group> Groups { get; set; } // Collection of groups associated with this org
        public ICollection<ScopedToken> ScopedTokens { get; set; }
        public ICollection<User> Users { get; set; } // Collection of users associated with this org
    }
}
