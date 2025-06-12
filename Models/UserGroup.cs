namespace VidizmoBackend.Models
{
    public class UserGroup
    {
        public int UserGroupId { get; set; } // Unique identifier for the user group
        public int GroupId { get; set; } // Foreign key to the Group this user belongs to`
        public Group Group { get; set; } // Navigation to the Group this user belongs to
        public int AddedUserId {get; set;}
        public User AddedUser {get; set;}
        public DateTime CreatedAt { get; set; } // Timestamp when the user was added to this group
        public int UserAddedByUserId { get; set; } // Foreign key to the User who added this user to the group
        public User UserAddedByUser { get; set; } // Navigation to the User who added this user to the group
    }
}