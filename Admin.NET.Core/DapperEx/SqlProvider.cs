// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.NET.Core.DapperEx;
public static class SqlProvider
{
    /// <summary>
    /// SqlServer 提供器程序集
    /// </summary>
    public const string SqlServer = "Microsoft.Data.SqlClient";

    /// <summary>
    /// Sqlite 提供器程序集
    /// </summary>
    public const string Sqlite = "Microsoft.Data.Sqlite";

    /// <summary>
    /// MySql 提供器程序集
    /// </summary>
    public const string MySql = "MySql.Data";

    /// <summary>
    /// PostgreSQL 提供器程序集
    /// </summary>
    public const string Npgsql = "Npgsql";

    /// <summary>
    /// Oracle 提供器程序集
    /// </summary>
    public const string Oracle = "Oracle.ManagedDataAccess";

    /// <summary>
    /// Firebird 提供器程序集
    /// </summary>
    public const string Firebird = "FirebirdSql.Data.FirebirdClient";
    /// <summary>
    /// 达梦 提供器程序集
    /// </summary>
    public const string Dm = "Dm";

    /// <summary>
    /// 数据库提供器连接对象类型集合
    /// </summary>
    internal static readonly ConcurrentDictionary<string, Type> SqlProviderDbConnectionTypeCollection;

    /// <summary>
    /// 静态构造函数
    /// </summary>
    static SqlProvider()
    {
        SqlProviderDbConnectionTypeCollection = new ConcurrentDictionary<string, Type>();
    }

    /// <summary>
    /// 获取数据库连接对象类型
    /// </summary>
    /// <param name="sqlProvider"></param>
    /// <returns></returns>
    internal static Type GetDbConnectionType(string sqlProvider)
    {
        return SqlProviderDbConnectionTypeCollection.GetOrAdd(sqlProvider, Function);

        // 本地静态方法
        static Type Function(string sqlProvider)
        {
            // 加载对应的数据库提供器程序集
            sqlProvider=sqlProvider == "Dm" ? "DmProvider" : sqlProvider;
            var databaseProviderAssembly = Assembly.Load(sqlProvider);
            sqlProvider = sqlProvider == "DmProvider" ? "Dm" : sqlProvider;
            // 获取对应数据库连接对象
            var databaseDbConnectionTypeName = sqlProvider switch
            {
                SqlServer => "Microsoft.Data.SqlClient.SqlConnection",
                Sqlite => "Microsoft.Data.Sqlite.SqliteConnection",
                MySql => "MySql.Data.MySqlClient.MySqlConnection",
                Npgsql => "Npgsql.NpgsqlConnection",
                Oracle => "Oracle.ManagedDataAccess.Client.OracleConnection",
                Firebird => "FirebirdSql.Data.FirebirdClient.FbConnection",
                Dm => "Dm.DmConnection",
                _ => null
            };

            // 加载数据库连接对象类型
            var dbConnectionType = databaseProviderAssembly.GetType(databaseDbConnectionTypeName);

            return dbConnectionType;
        }
    }
}
