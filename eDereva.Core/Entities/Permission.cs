namespace eDereva.Core.Entities
{
    public class Permission
    {
        public int Id { get; set; }

        public bool CanViewSoftDeletedUsers { get; set; }

        // other permissions
    }
}
