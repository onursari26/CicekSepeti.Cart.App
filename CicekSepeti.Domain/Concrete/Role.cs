using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CicekSepeti.Domain.Concrete
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<IdentityUserRole<int>> Users { get; set; }
    }
}
