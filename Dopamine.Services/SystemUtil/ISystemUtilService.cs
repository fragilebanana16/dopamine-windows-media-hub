using Dopamine.Data.Entities;
using Dopamine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Services.SystemUtil
{
    public interface ISystemUtilService
    {
        /// <summary>
        /// 获取解密后的密码列表
        /// </summary>
        Task<List<PasswordEntry>> GetPasswordEntryAsync();

        /// <summary>
        /// 添加新密码项
        /// </summary>
        Task<bool> AddPasswordEntryAsync(string title);

        /// <summary>
        /// 删除密码项
        /// </summary>
        Task<bool> RemovePasswordEntryAsync(long passwordId);

        /// <summary>
        /// 批量更新密码项（会自动对密码进行加密存储）
        /// </summary>
        Task UpdatePasswordEntryAsync(IList<PasswordEntry> passwordEntries);
    }
}
