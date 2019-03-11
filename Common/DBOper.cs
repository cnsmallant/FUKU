using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;


public abstract class DBOper
{
    #region ExecuteReader
    /// <summary>
    ///   针对 System.Data.Common.DbCommand.Connection 
    ///   执行 System.Data.Common.DbCommand.CommandText，
    ///   并返回System.Data.Common.DbDataReader对象。
    /// </summary>
    /// <param name="conn">表示数据库连接。</param>
    /// <param name="cmdText">获取或设置针对数据源运行的文本命令。</param>
    /// <param name="cmd">表示要对数据源执行的 SQL 语句或存储过程，即DbCommand 命令。</param>
    /// <param name="cmdType">表示指定如何解释命令字符串。</param>
    /// <param name="para">表示 DbCommand 的参数，此参数可以不传入。</param>
    /// <returns>DbDataReader</returns>
    public static DbDataReader ExecuteReader(DbConnection conn, string cmdText,
        DbCommand cmd, CommandType cmdType, params DbParameter[] para)
    {
        try
        {
            PrepareCommand(conn, cmdText, cmd, cmdType, para);
            return cmd.ExecuteReader();
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion
    #region ExecuteNonQuery
    /// <summary>
    ///  对连接对象执行 SQL 语句。
    /// 返回结果: 受影响的行数。
    /// </summary>
    /// <param name="conn">表示数据库连接。</param>
    /// <param name="cmdText">获取或设置针对数据源运行的文本命令。</param>
    /// <param name="cmd">表示要对数据源执行的 SQL 语句或存储过程，即DbCommand 命令。</param>
    /// <param name="cmdType">表示指定如何解释命令字符串。</param>
    /// <param name="para">表示 DbCommand 的参数，此参数可以不传入。</param>
    /// <returns>int</returns>
    public static int ExecuteNonQuery(DbConnection conn, string cmdText,
        DbCommand cmd, CommandType cmdType, params DbParameter[] para)
    {
        PrepareCommand(conn, cmdText, cmd, cmdType, para);
        return cmd.ExecuteNonQuery();
    }
    #endregion
    #region DataSet
    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="cmdText">获取或设置针对数据源运行的文本命令。</param>
    /// <param name="conn">表示数据库连接。</param>
    /// <param name="da">DbDataAdapter</param>
    /// <param name="ds">DataSet</param>
    /// <returns></returns>
    public static DataSet DataSetQuery(string cmdText, DbConnection conn, DbDataAdapter da, DataSet ds)
    {
        try
        {
            conn.Open();
            da.Fill(ds, "ds");
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            conn.Close();
        }
        return ds;
    }

    #endregion
    #region ExecuteScalar
    /// <summary>
    ///  执行查询，并返回查询所返回的结果集中第一行的第一列。
    ///  所有其他的列和行将被忽略。
    /// </summary>
    /// <param name="conn">表示数据库连接。</param>
    /// <param name="cmdText">获取或设置针对数据源运行的文本命令。</param>
    /// <param name="cmd">表示要对数据源执行的 SQL 语句或存储过程，即DbCommand 命令。</param>
    /// <param name="cmdType">表示指定如何解释命令字符串。</param>
    /// <param name="para">表示 DbCommand 的参数，此参数可以不传入。</param>
    /// <returns>object</returns>
    public static object ExecuteScalar(DbConnection conn, string cmdText,
      DbCommand cmd, CommandType cmdType, params DbParameter[] para)
    {
        PrepareCommand(conn, cmdText, cmd, cmdType, para);
        return cmd.ExecuteScalar();

    }
    #endregion
    #region PrepareCommand
    /// <summary>
    /// 装配DbCommand
    /// </summary>
    /// <param name="conn">表示数据库连接。</param>
    /// <param name="cmdText">获取或设置针对数据源运行的文本命令。</param>
    /// <param name="cmd">表示要对数据源执行的 SQL 语句或存储过程，即DbCommand 命令。</param>
    /// <param name="cmdType">表示指定如何解释命令字符串。</param>
    /// <param name="para">表示 System.Data.Common.DbCommand 的参数，此参数可以不传入。</param>
    private static void PrepareCommand(DbConnection conn, string cmdText,
        DbCommand cmd, CommandType cmdType, params DbParameter[] para)
    {
        try
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                cmd.Parameters.AddRange(para);
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }


    }
    #endregion
}
