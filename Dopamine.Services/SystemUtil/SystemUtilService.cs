using Digimezzo.Foundation.Core.Logging;
using Dopamine.Data.Entities;
using Dopamine.Data.Repositories;
using Dopamine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Services.SystemUtil
{
    public class SystemUtilService : ISystemUtilService
    {
        private readonly ISystemUtilRepository _systemRepository;
        // private readonly IEncryptionService _encryptionService; // 预留：未来注入加密服务

        public SystemUtilService(ISystemUtilRepository systemRepository)
        {
            _systemRepository = systemRepository;
        }

        /// <summary>
        /// 查询接口：获取密码列表并自动解密
        /// </summary>
        public async Task<List<PasswordEntry>> GetPasswordEntryAsync()
        {
            try
            {
                // 1. 从 Repository 获取数据库原始数据
                var entries = await _systemRepository.GetPasswordEntryAsync();

                // 2. 业务逻辑处理：对密码字段进行解密
                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry.Password))
                    {
                        // 示例：entry.Password = _encryptionService.Decrypt(entry.Password);
                        // 目前先保持原样，后续加上加密服务后在这里一处修改即可
                    }
                }

                return entries;
            }
            catch (Exception ex)
            {
                LogClient.Error("SystemService: Failed to get and decrypt passwords. Exception: {0}", ex.Message);
                return new List<PasswordEntry>();
            }
        }

        /// <summary>
        /// 添加新密码项
        /// </summary>
        public async Task<bool> AddPasswordEntryAsync(string title)
        {
            return await _systemRepository.AddPasswordEntryAsync(title);
        }

        /// <summary>
        /// 删除密码项
        /// </summary>
        public async Task<bool> RemovePasswordEntryAsync(long passwordId)
        {
            return await _systemRepository.RemovePasswordEntryAsync(passwordId);
        }

        /// <summary>
        /// 批量更新密码：在保存前先进行加密
        /// </summary>
        public async Task UpdatePasswordEntryAsync(IList<PasswordEntry> passwordEntries)
        {
            if (passwordEntries == null || passwordEntries.Count == 0) return;

            try
            {
                // 1. 业务逻辑处理：在写入数据库前，对明文密码进行加密
                foreach (var entry in passwordEntries)
                {
                    if (!string.IsNullOrEmpty(entry.Password))
                    {
                        // entry.Password = _encryptionService.Encrypt(entry.Password);
                    }
                }

                // 2. 调用 Repo 执行批量更新
                await _systemRepository.UpdatePasswordEntryAsync(passwordEntries);
            }
            catch (Exception ex)
            {
                LogClient.Error("SystemService: Failed to encrypt and update passwords. Exception: {0}", ex.Message);
            }
        }
    }
}
