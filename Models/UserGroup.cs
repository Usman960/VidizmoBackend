namespace VidizmoBackend.Models
{
    public class UserGroup
    {
        public int UserGroupId { get; set; } // Unique identifier for the user group
        public DateTime CreatedAt { get; set; } // Timestamp when the user was added to this group
        public int UserAddedByUserId { get; set; } // Foreign key to the User who added this user to the group
    }
}