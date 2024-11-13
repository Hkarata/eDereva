using System.ComponentModel.DataAnnotations.Schema;
using eDereva.Core.Enums;

namespace eDereva.Core.Entities
{
    /// <summary>
    /// Represents a permission entity that maps flags to database records.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Gets or sets the unique identifier for the permission.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the role this permission belongs to.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role associated with this permission.
        /// </summary>
        public Role? Role { get; set; }

        /// <summary>
        /// Gets or sets the permission flags.
        /// </summary>
        public PermissionFlag Flags { get; set; }

        [NotMapped]
        public bool CanManageUsers => Flags.HasFlag(PermissionFlag.ManageUsers);
        [NotMapped]
        public bool CanViewUsers => Flags.HasFlag(PermissionFlag.ViewUsers);
        [NotMapped]
        public bool CanEditUsers => Flags.HasFlag(PermissionFlag.EditUsers);
        [NotMapped]
        public bool CanDeleteUsers => Flags.HasFlag(PermissionFlag.DeleteUsers);
        [NotMapped]
        public bool CanManageVenues => Flags.HasFlag(PermissionFlag.ManageVenues);
        [NotMapped]
        public bool CanViewVenues => Flags.HasFlag(PermissionFlag.ViewVenues);
        [NotMapped]
        public bool CanEditVenues => Flags.HasFlag(PermissionFlag.EditVenues);
        [NotMapped]
        public bool CanDeleteVenues => Flags.HasFlag(PermissionFlag.DeleteVenues);
        [NotMapped]
        public bool CanManageQuestionBanks => Flags.HasFlag(PermissionFlag.ManageQuestionBanks);
        [NotMapped]
        public bool CanViewQuestionBanks => Flags.HasFlag(PermissionFlag.ViewQuestionBanks);
        [NotMapped]
        public bool CanEditQuestionBanks => Flags.HasFlag(PermissionFlag.EditQuestionBanks);
        [NotMapped]
        public bool CanDeleteQuestionBanks => Flags.HasFlag(PermissionFlag.DeleteQuestionBanks);
        [NotMapped]
        public bool CanManageTests => Flags.HasFlag(PermissionFlag.ManageTests);
        [NotMapped]
        public bool CanViewTests => Flags.HasFlag(PermissionFlag.ViewTests);
        [NotMapped]
        public bool CanEditTests => Flags.HasFlag(PermissionFlag.EditTests);
        [NotMapped]
        public bool CanDeleteTests => Flags.HasFlag(PermissionFlag.DeleteTests);
        [NotMapped]
        public bool CanViewSoftDeletedData => Flags.HasFlag(PermissionFlag.ViewSoftDeletedData);
    }
}
