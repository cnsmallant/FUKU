using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


    public class DBConnectionString
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        /// <param name="DBName">数据库名称 web.config配置里的name值</param>
        /// <returns></returns>
        public static string ConnectionString(string DBName)
        {
            return ConfigurationManager.ConnectionStrings[DBName].ToString();
        }
    }

