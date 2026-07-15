using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Data.Entities
{
    public class PasswordEntry
    {
        [PrimaryKey(), AutoIncrement()]
        public long PasswordID { get; set; }

        /// <summary>
        /// 标题/名称（如: GitHub, 招商银行）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 账号/用户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码（建议存密文，展示时解密）
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 相关网址或应用包名
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 分类/类型（如: Website, App, BankCard）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 标签（多个标签可用逗号分隔，如 "Work,Important"）
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 创建时间（格式: yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// 更新时间（格式: yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public string UpdatedAt { get; set; }
    }
}
