using System.ComponentModel.DataAnnotations.Schema;

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

        #region User Permissions
        [NotMapped]
        public bool CanManageUsers => Flags.HasFlag(PermissionFlag.ManageUsers);
        [NotMapped]
        public bool CanViewUsers => Flags.HasFlag(PermissionFlag.ViewUsers);
        [NotMapped]
        public bool CanEditUsers => Flags.HasFlag(PermissionFlag.EditUsers);
        [NotMapped]
        public bool CanDeleteUsers => Flags.HasFlag(PermissionFlag.DeleteUsers);
        #endregion

        #region Venue Permissions
        [NotMapped]
        public bool CanManageVenues => Flags.HasFlag(PermissionFlag.ManageVenues);
        [NotMapped]
        public bool CanViewVenues => Flags.HasFlag(PermissionFlag.ViewVenues);
        [NotMapped]
        public bool CanEditVenues => Flags.HasFlag(PermissionFlag.EditVenues);
        [NotMapped]
        public bool CanDeleteVenues => Flags.HasFlag(PermissionFlag.DeleteVenues);
        #endregion

        #region Question Bank Permissions
        [NotMapped]
        public bool CanManageQuestionBanks => Flags.HasFlag(PermissionFlag.ManageQuestionBanks);
        [NotMapped]
        public bool CanViewQuestionBanks => Flags.HasFlag(PermissionFlag.ViewQuestionBanks);
        [NotMapped]
        public bool CanEditQuestionBanks => Flags.HasFlag(PermissionFlag.EditQuestionBanks);
        [NotMapped]
        public bool CanDeleteQuestionBanks => Flags.HasFlag(PermissionFlag.DeleteQuestionBanks);
        #endregion

        #region Test Permissions
        [NotMapped]
        public bool CanManageTests => Flags.HasFlag(PermissionFlag.ManageTests);
        [NotMapped]
        public bool CanViewTests => Flags.HasFlag(PermissionFlag.ViewTests);
        [NotMapped]
        public bool CanEditTests => Flags.HasFlag(PermissionFlag.EditTests);
        [NotMapped]
        public bool CanDeleteTests => Flags.HasFlag(PermissionFlag.DeleteTests);
        #endregion

        #region Booking Permissions
        [NotMapped]
        public bool CanManageBookings => Flags.HasFlag(PermissionFlag.ManageBookings);
        [NotMapped]
        public bool CanViewBookings => Flags.HasFlag(PermissionFlag.ViewBookings);
        [NotMapped]
        public bool CanCreateBookings => Flags.HasFlag(PermissionFlag.CreateBookings);
        [NotMapped]
        public bool CanEditBookings => Flags.HasFlag(PermissionFlag.EditBookings);
        [NotMapped]
        public bool CanDeleteBookings => Flags.HasFlag(PermissionFlag.DeleteBookings);
        #endregion

        #region Special Permissions
        [NotMapped]
        public bool CanViewSoftDeletedData => Flags.HasFlag(PermissionFlag.ViewSoftDeletedData);
        #endregion
    }
}