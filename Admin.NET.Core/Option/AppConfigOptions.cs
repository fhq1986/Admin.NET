// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.NET.Core.Option;
public class AppConfigOptions : IConfigurableOptions
{
    public AppType AppType { get; set; } = AppType.Controllers;

    /// <summary>
    /// Api地址，默认 http://*:8000
    /// </summary>
    public string[] Urls { get; set; }

    /// <summary>
    /// 跨域地址，默认 http://*:9000
    /// </summary>
    public string[] CorUrls { get; set; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    public string[] AssemblyNames { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    public bool Tenant { get; set; } = false;

    /// <summary>
    /// 分布式事务唯一标识
    /// </summary>
    public string DistributeKey { get; set; }

    /// <summary>
    /// Swagger文档
    /// </summary>
    public SwaggerConfig Swagger { get; set; } = new SwaggerConfig();

    /// <summary>
    /// 新版Api文档
    /// </summary>
    public ApiUIConfig ApiUI { get; set; } = new ApiUIConfig();

    /// <summary>
    /// MiniProfiler性能分析器
    /// </summary>
    public bool MiniProfiler { get; set; } = false;

    /// <summary>
    /// 限流
    /// </summary>
    public bool RateLimit { get; set; } = false;


    /// <summary>
    /// 默认密码
    /// </summary>
    public string DefaultPassword { get; set; } = "123asd";

    /// <summary>
    /// 动态Api配置
    /// </summary>
    public DynamicApiConfig DynamicApi { get; set; } = new DynamicApiConfig();

    /// <summary>
    /// 实现标准标识密码哈希
    /// </summary>
    public bool PasswordHasher { get; set; } = false;

    /// <summary>
    /// 最大请求大小
    /// </summary>
    public long MaxRequestBodySize { get; set; } = 104857600;


    /// <summary>
    /// 指定跨域访问时预检等待时间，以秒为单位，默认30分钟
    /// </summary>
    public int PreflightMaxAge { get; set; }

}
/// <summary>
/// Swagger配置
/// </summary>
public class SwaggerConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; } = false;

    /// <summary>
    /// 启用枚举架构过滤器
    /// </summary>
    public bool EnableEnumSchemaFilter { get; set; } = true;

    /// <summary>
    /// 启用接口排序文档过滤器
    /// </summary>
    public bool EnableOrderTagsDocumentFilter { get; set; } = true;

    /// <summary>
    /// 启用枚举属性名
    /// </summary>
    public bool EnableJsonStringEnumConverter { get; set; } = false;

    /// <summary>
    /// 启用SchemaId命名空间
    /// </summary>
    public bool EnableSchemaIdNamespace { get; set; } = false;

    /// <summary>
    /// 程序集列表
    /// </summary>
    public string[] AssemblyNameList { get; set; }

    private string _RoutePrefix = "swagger";
    /// <summary>
    /// 访问地址
    /// </summary>
    public string RoutePrefix { get => Regex.Replace(_RoutePrefix, "^\\/+|\\/+$", ""); set => _RoutePrefix = value; }

    /// <summary>
    /// 地址
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 项目列表
    /// </summary>
    public List<ProjectConfig> Projects { get; set; }
}

/// <summary>
///新版Api文档配置
/// </summary>
public class ApiUIConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; } = false;


    private string _RoutePrefix = "";
    /// <summary>
    /// 访问地址
    /// </summary>
    public string RoutePrefix { get => Regex.Replace(_RoutePrefix, "^\\/+|\\/+$", ""); set => _RoutePrefix = value; }

    public SwaggerFooterConfig Footer { get; set; } = new SwaggerFooterConfig();
}

/// <summary>
/// Swagger页脚配置
/// </summary>
public class SwaggerFooterConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; } = false;

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; }
}
/// <summary>
/// 项目配置
/// </summary>
public class ProjectConfig
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// 动态api配置
/// </summary>
public class DynamicApiConfig
{
    /// <summary>
    /// 结果格式化
    /// </summary>
    public bool FormatResult { get; set; } = true;
}
/// <summary>
/// 应用程序类型
/// </summary>
public enum AppType
{
    Controllers,
    ControllersWithViews,
    MVC
}