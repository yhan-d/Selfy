using Microsoft.AspNetCore.Identity;

namespace Selfy.Data.Identity
{
    public class ApplicationRole : IdentityRole
    {

        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {       
        }


    }
}
