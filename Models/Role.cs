using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string? RoleNameLong { get; set; }
        public string? RoleDescription { get; set; }
        public int Order { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
