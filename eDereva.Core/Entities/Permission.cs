namespace eDereva.Core.Entities
{
    public class Permission
    {
        public int Id { get; set; }

        // Permissions related to user management
        public bool CanManageUsers { get; set; }
        public bool CanViewUsers { get; set; }
        public bool CanEditUsers { get; set; }
        public bool CanDeleteUsers { get; set; }

        // Permissions related to venue management
        public bool CanManageVenues { get; set; }
        public bool CanViewVenues { get; set; }
        public bool CanEditVenues { get; set; }
        public bool CanDeleteVenues { get; set; }

        // Permissions related to question bank management
        public bool CanManageQuestionBanks { get; set; }
        public bool CanViewQuestionBanks { get; set; }
        public bool CanEditQuestionBanks { get; set; }
        public bool CanDeleteQuestionBanks { get; set; }

        // Permissions related to test management
        public bool CanManageTests { get; set; }
        public bool CanViewTests { get; set; }
        public bool CanEditTests { get; set; }
        public bool CanDeleteTests { get; set; }

        // Permissions related to soft deletes
        public bool CanViewSoftDeletedData { get; set; }

        // other permissions
    }
}
