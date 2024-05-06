// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core.DapperEx;
using DapperExtensions;
using DapperExtensions.Predicate;
using DocumentFormat.OpenXml.Wordprocessing;
using Nacos.V2;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统参数配置服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 440)]
public class SysConfigService : IDynamicApiController, IScoped
{
    private readonly SqlSugarRepository<SysConfig> _sysConfigRep;
    private readonly SysCacheService _sysCacheService;
    private readonly IConfiguration _configuration;
    private readonly IDapperRepository _dapperRepository;
    private readonly IOptionsSnapshot<EmailOptions> _emailOptions;
    public SysConfigService(SqlSugarRepository<SysConfig> sysConfigRep,
        SysCacheService sysCacheService, IConfiguration configuration, IDapperRepository dapperRepository, IOptionsSnapshot<EmailOptions> emailOptions)
    {
        _sysConfigRep = sysConfigRep;
        _sysCacheService = sysCacheService;
        _configuration = configuration;
        _dapperRepository = dapperRepository;
        _emailOptions = emailOptions;
    }

    /// <summary>
    /// 获取参数配置分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取参数配置分页列表")]
    public async Task<SqlSugarPagedList<SysConfig>> Page(PageConfigInput input)
    {
        return await _sysConfigRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code?.Trim()), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupCode?.Trim()), u => u.GroupCode.Equals(input.GroupCode))
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取参数配置列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取参数配置列表")]
    public async Task<List<SysConfig>> GetList()
    {
        return await _sysConfigRep.GetListAsync();
    }

    /// <summary>
    /// 增加参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加参数配置")]
    public async Task AddConfig(AddConfigInput input)
    {
        var isExist = await _sysConfigRep.IsAnyAsync(u => u.Name == input.Name || u.Code == input.Code);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);

        await _sysConfigRep.InsertAsync(input.Adapt<SysConfig>());
    }

    /// <summary>
    /// 更新参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新参数配置")]
    public async Task UpdateConfig(UpdateConfigInput input)
    {
        var isExist = await _sysConfigRep.IsAnyAsync(u => (u.Name == input.Name || u.Code == input.Code) && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);

        var config = input.Adapt<SysConfig>();
        await _sysConfigRep.AsUpdateable(config).IgnoreColumns(true).ExecuteCommandAsync();

        _sysCacheService.Remove(config.Code);
    }

    /// <summary>
    /// 删除参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除参数配置")]
    public async Task DeleteConfig(DeleteConfigInput input)
    {
        var config = await _sysConfigRep.GetFirstAsync(u => u.Id == input.Id);
        if (config.SysFlag == YesNoEnum.Y) // 禁止删除系统参数
            throw Oops.Oh(ErrorCodeEnum.D9001);

        await _sysConfigRep.DeleteAsync(config);

        _sysCacheService.Remove(config.Code);
    }

    /// <summary>
    /// 批量删除参数配置 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BatchDelete"), HttpPost]
    [DisplayName("批量删除参数配置")]
    public async Task BatchDeleteConfig(List<long> ids)
    {
        foreach (var id in ids)
        {
            var config = await _sysConfigRep.GetFirstAsync(u => u.Id == id);
            if (config.SysFlag == YesNoEnum.Y) // 禁止删除系统参数
                continue;

            await _sysConfigRep.DeleteAsync(config);

            _sysCacheService.Remove(config.Code);
        }
    }

    /// <summary>
    /// 获取参数配置详情 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取参数配置详情")]
    public async Task<SysConfig> GetDetail([FromQuery] ConfigInput input)
    {
        return await _sysConfigRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取参数配置值
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<T> GetConfigValue<T>(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return default;

        var value = _sysCacheService.Get<string>(code);
        if (string.IsNullOrEmpty(value))
        {
            var config = await _sysConfigRep.GetFirstAsync(u => u.Code == code);
            value = config != null ? config.Value : default;
            _sysCacheService.Set(code, value);
        }
        if (string.IsNullOrWhiteSpace(value)) return default;
        return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// 获取分组列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取分组列表")]
    public async Task<List<string>> GetGroupList()
    {
        return await _sysConfigRep.AsQueryable().GroupBy(u => u.GroupCode).Select(u => u.GroupCode).ToListAsync();
    }

    /// <summary>
    /// 获取 Token 过期时间
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<int> GetTokenExpire()
    {
        var tokenExpireStr = await GetConfigValue<string>(CommonConst.SysTokenExpire);
        _ = int.TryParse(tokenExpireStr, out var tokenExpire);
        return tokenExpire == 0 ? 20 : tokenExpire;
    }

    /// <summary>
    /// 获取 RefreshToken 过期时间
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<int> GetRefreshTokenExpire()
    {
        var refreshTokenExpireStr = await GetConfigValue<string>(CommonConst.SysRefreshTokenExpire);
        _ = int.TryParse(refreshTokenExpireStr, out var refreshTokenExpire);
        return refreshTokenExpire == 0 ? 40 : refreshTokenExpire;
    }
    /// <summary>
    /// 获取Nacos配置 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取Nacos配置")]
    public async Task<string> GetNacosConfig(string dataId,string group,string name)
    {
        var val= App.Configuration[name];
        //var val2=await _configuration.GetConfig(dataId,group,3000);
        return val;
    }
    /// <summary>
    /// dapper使用达梦测试
    /// </summary>
    /// <param name="page">当前页码</param>
    /// <param name="pageSize">每页记录数</param>
    /// <returns></returns>
    [DisplayName("dapper使用达梦测试")]
    [AllowAnonymous]
    public async Task<SqlSugarPagedList<Cjxx>> TestDapper2Dm([Required][Range(1, 100)] int page = 1,[Required][Range(1,50)]int pageSize=10)
    {
        IList<ISort> sorts = new List<ISort>();
        sorts.Add(new Sort {
            PropertyName = "Ord",
            Ascending = true,
        });
        PredicateGroup pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
        pg.Predicates.Add(Predicates.Field<Cjxx>(p => p.Id, Operator.Le,1000));
        var total=await _dapperRepository.Context.CountAsync<Cjxx>(pg);
        var data=await _dapperRepository.Context.GetPageAsync<Cjxx>(pg, sorts, page, pageSize);
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0;
        UnifyContext.Fill("附加数据");
        return new SqlSugarPagedList<Cjxx>
        {
            Items = data,
            TotalPages = totalPages,
            Total = total,
            PageSize= pageSize,
            Page= page,
        };
    }
    /// <summary>
    ///集成apollo测试
    /// </summary>
    /// <param name="page">当前页码</param>
    /// <param name="pageSize">每页记录数</param>
    /// <returns></returns>
    [DisplayName("获取apollo配置")]
    [AllowAnonymous]
    public async Task<string> GetApolloConfig([Required] string name)
    {
        return App.Configuration[name];
    }
    /// <summary>
    ///选项Option测试
    /// </summary>
    /// <returns></returns>
    [DisplayName("Option")]
    [AllowAnonymous]
    public async Task<string> GetOption(string name)
    {
        return _configuration[name];
    }
}