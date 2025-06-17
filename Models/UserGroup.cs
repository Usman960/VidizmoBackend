namespace VidizmoBackend.Models
{
    public class UserGroup
    {
        public int UserGroupId { get; set; } // Unique identifier for the user group
        public int GroupId { get; set; } // Foreign key to the Group this user belongs to`
        public Group Group { get; set; } // Navigation to the Group this user belongs to
        public int UserId {get; set;}
        public User User {get; set;}
        public DateTime CreatedAt { get; set; } // Timestamp when the user was added to this group
        public int AddedById { get; set; } // Foreign key to the User who added this user to the group
        public User AddedBy { get; set; } // Navigation to the User who added this user to the group
    }
}