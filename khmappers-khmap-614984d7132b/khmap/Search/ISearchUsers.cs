using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2
{
    public interface ISearchUsers
    {
        IEnumerable<ApplicationUser> SearchUsersFunc(string userName);
    }
}
