using Digimezzo.Foundation.Core.Logging;
using Dopamine.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Data.Repositories
{
    public class SystemUtilRepository : ISystemUtilRepository
    {
        private ISQLiteConnectionFactory factory;

        public SystemUtilRepository(ISQLiteConnectionFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// 异步获取所有密码项
        /// </summary>
        public async Task<List<PasswordEntry>> GetPasswordEntryAsync()
        {
            List<PasswordEntry> passwordEntries = new List<PasswordEntry>();

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            var result = conn.Query<PasswordEntry>("SELECT * FROM PasswordEntry;");
                            passwordEntries = result.ToList();
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not get PasswordEntries. Exception: {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });

            return passwordEntries;
        }

        /// <summary>
        /// 异步添加密码项 (这里根据你提供的接口参数 path 进行了实现，默认将 path 存入 Url 或 Title，并在内部处理插入)
        /// </summary>
        public async Task<bool> AddPasswordEntryAsync(string path)
        {
            bool isSuccess = false;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            // 插入一条初始数据，记录创建与更新时间
                            var sql = "INSERT INTO PasswordEntry (Title, CreatedAt, UpdatedAt) VALUES (?, datetime('now', 'localtime'), datetime('now', 'localtime'));";
                            conn.Execute(sql, path);
                            isSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not add PasswordEntry with path '{0}'. Exception: {1}", path, ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });

            return isSuccess;
        }

        /// <summary>
        /// 异步删除密码项
        /// </summary>
        public async Task<bool> RemovePasswordEntryAsync(long passwordID)
        {
            bool result = false;
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            int affectedRows = conn.Execute("DELETE FROM PasswordEntry WHERE PasswordID=?;", passwordID);
                            if (affectedRows > 0)
                            {
                                result = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error("Could not delete PasswordEntry. PasswordID: {0}, Exception: {1}", passwordID, ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });

            return result;
        }

        /// <summary>
        /// 批量异步更新密码项
        /// </summary>
        public async Task UpdatePasswordEntryAsync(IList<PasswordEntry> passwordEntrys)
        {
            if (passwordEntrys == null || passwordEntrys.Count == 0) return;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        // 使用事务来保证批量更新的性能和原子性
                        conn.Execute("BEGIN TRANSACTION;");
                        try
                        {
                            foreach (var entry in passwordEntrys)
                            {
                                var sql = "UPDATE PasswordEntry SET " +
                                          "Title = ?, Account = ?, Password = ?, Url = ?, " +
                                          "Type = ?, Tags = ?, Notes = ?, " +
                                          "UpdatedAt = datetime('now', 'localtime') " +
                                          "WHERE PasswordID = ?;";

                                conn.Execute(sql,
                                    entry.Title,
                                    entry.Account,
                                    entry.Password,
                                    entry.Url,
                                    entry.Type,
                                    entry.Tags,
                                    entry.Notes,
                                    entry.PasswordID);
                            }
                            conn.Execute("COMMIT;");
                        }
                        catch (Exception ex)
                        {
                            try { conn.Execute("ROLLBACK;"); } catch { }
                            LogClient.Error("Could not update PasswordEntries batch. Exception: {0}", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error("Could not connect to the database. Exception: {0}", ex.Message);
                }
            });
        }
    }
}
