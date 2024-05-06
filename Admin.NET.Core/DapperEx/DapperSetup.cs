// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.NET.Core.DapperEx;
public static class DapperSetup
{
        /// <summary>
        /// 添加 Dapper 拓展
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="sqlProvider">  类型</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddDapper(this IServiceCollection services, string connectionString, string sqlProvider, Action configure = default)
    {
        // 获取数据库类型
        var dbConnectionType = SqlProvider.GetDbConnectionType(sqlProvider);

        // 创建数据库连接对象
        services.AddScoped(u =>
        {
            var dbConnection = Activator.CreateInstance(dbConnectionType, new[] { connectionString }) as IDbConnection;
            if (dbConnection.State != ConnectionState.Open) dbConnection.Open();

            return dbConnection;
        });

        // 注册非泛型仓储
        services.AddScoped<IDapperRepository, DapperRepository>();

        // 注册 Dapper 仓储
        services.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));

        // 添加 Dapper 其他初始配置
        configure?.Invoke();

        return services;
    }
}
