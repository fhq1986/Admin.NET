// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.NET.Core.DapperEx;
public partial class DapperRepository : IDapperRepository
{
    /// <summary>
    /// 数据库连接对象
    /// </summary>
    private readonly IDbConnection _db;

    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="db"></param>
    public DapperRepository(
        IServiceProvider serviceProvider
        , IDbConnection db)
    {
        _serviceProvider = serviceProvider;
        _db = db;
    }

    /// <summary>
    /// 连接上下文
    /// </summary>
    public virtual IDbConnection Context
    {
        get
        {
            if (_db.State != ConnectionState.Open) _db.Open();
            return _db;
        }
    }

    /// <summary>
    /// 动态连接上下文
    /// </summary>
    public virtual dynamic DynamicContext { get; }

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
    public virtual IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return Context.Query(sql, param, transaction, buffered, commandTimeout, commandType);
    }

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
    public virtual IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return Context.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// 查询返回动态类型
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    public virtual Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return Context.QueryAsync(sql, param, transaction, commandTimeout, commandType);
    }

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
    public virtual Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return Context.QueryAsync<T>(sql: sql, param, transaction, commandTimeout, commandType);
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    public virtual int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return Context.Execute(sql, param, transaction, commandTimeout, commandType);
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    public virtual Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return Context.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }

    /// <summary>
    /// 切换仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public virtual IDapperRepository<TEntity> Change<TEntity>()
        where TEntity : class, new()
    {
        return _serviceProvider.GetService<IDapperRepository<TEntity>>();
    }
}

/// <summary>
/// Dapper 仓储实现类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public partial class DapperRepository<TEntity> : DapperRepository, IDapperRepository<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="db"></param>
    public DapperRepository(IServiceProvider serviceProvider
        , IDbConnection db) : base(serviceProvider, db)
    {
    }

    /// <summary>
    /// 获取一条
    /// </summary>
    /// <param name="id"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual TEntity Get(object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.Get<TEntity>(id, transaction, commandTimeout);
    }

    /// <summary>
    /// 获取一条
    /// </summary>
    /// <param name="id"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<TEntity> GetAsync(object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.GetAsync<TEntity>(id, transaction, commandTimeout);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual IEnumerable<TEntity> GetAll(IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.GetList<TEntity>(transaction, commandTimeout: commandTimeout);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<IEnumerable<TEntity>> GetAllAsync(IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.GetListAsync<TEntity>(transaction, commandTimeout:commandTimeout);
    }

    /// <summary>
    /// 新增一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual dynamic Insert(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.Insert(entity, transaction, commandTimeout);
    }

    /// <summary>
    /// 新增一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<dynamic> InsertAsync(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.InsertAsync(entity, transaction, commandTimeout);
    }

    /// <summary>
    /// 新增多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual void Insert(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null)
    {
         Context.Insert(entities, transaction, commandTimeout);
    }

    /// <summary>
    /// 新增多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task InsertAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.InsertAsync(entities, transaction, commandTimeout);
    }

    /// <summary>
    /// 更新一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual bool Update(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.Update(entity, transaction, commandTimeout);
    }

    /// <summary>
    /// 更新一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.UpdateAsync(entity, transaction, commandTimeout);
    }

    /// <summary>
    /// 更新多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual void Update(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null)
    {
         Context.Update(entities, transaction, commandTimeout);
    }

    /// <summary>
    /// 更新多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<bool> UpdateAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.UpdateAsync(entities, transaction, commandTimeout);
    }

    /// <summary>
    /// 删除一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual bool Delete(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.Delete(entity, transaction, commandTimeout);
    }

    /// <summary>
    /// 删除一条
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<bool> DeleteAsync(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.DeleteAsync(entity, transaction, commandTimeout);
    }

    /// <summary>
    /// 删除多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual void Delete(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null)
    {
         Context.Delete(entities, transaction, commandTimeout);
    }

    /// <summary>
    /// 删除多条
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public virtual Task<bool> DeleteAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return Context.DeleteAsync(entities, transaction, commandTimeout);
    }
}