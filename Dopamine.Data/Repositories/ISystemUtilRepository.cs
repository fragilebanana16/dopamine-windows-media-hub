using Dopamine.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Data.Repositories
{
    public interface ISystemUtilRepository
    {
        Task<List<PasswordEntry>> GetPasswordEntryAsync();
        Task<bool> AddPasswordEntryAsync(string path);
        Task<bool> RemovePasswordEntryAsync(long passwordID);
        Task UpdatePasswordEntryAsync(IList<PasswordEntry> passwordEntrys);
    }
}
