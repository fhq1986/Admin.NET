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
public partial interface IDapperRepository
{
    /// <summary>
    /// 连接上下文
    /// </summary>
    IDbConnection Context { get; }

    /// <summary>
    /// 动态连接上下文
    /// </summary>
    dynamic DynamicContext { get; }

    /// <summary>
    /// 查询返回动态类型
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="buffered"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// 查询返回特定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="buffered"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// 查询返回动态类型
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// 查询返回特定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// 切换仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    IDapperRepository<TEntity> Change<TEntity>()
        where TEntity : class, new();
}

/// <summary>
/// Dapper 仓储接口定义
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public partial interface IDapperRepository<TEntity> : IDapperRepository
    where TEntity : class, new()
{
    /// <summary>
    /// 获取一条
    /// </summary>
    /// <param name="id"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    TEntity Get(object id, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 获取一条
    /// </summary>
    /// <param name="id"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<TEntity> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    IEnumerable<TEntity> GetAll(IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetAllAsync(IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 新增一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    dynamic Insert(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 新增一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<dynamic> InsertAsync(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 新增多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    void Insert(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 新增多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task InsertAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 更新一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    bool Update(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 更新一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 更新多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    void Update(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 更新多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 删除一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    bool Delete(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 删除一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 删除多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    void Delete(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null);

    /// <summary>
    /// 删除多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null);
}
