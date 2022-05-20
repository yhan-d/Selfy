using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfy.Core.Identity;

public static class Roles
{
    public static readonly string Admin = "Admin";
    public static readonly string User = "User";
    public static readonly string Operator = "Operator";
    public static readonly string Technician = "Technician";
    public static readonly string Passive = "Passive";

    public static List<string> RoleList = new List<string>()
    {
        Admin, User, Operator, Technician, Passive
    };
}


