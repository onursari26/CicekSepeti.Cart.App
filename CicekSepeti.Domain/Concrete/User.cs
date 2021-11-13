using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CicekSepeti.Domain.Concrete
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<IdentityUserRole<int>> Roles { get; set; }
    }
}
