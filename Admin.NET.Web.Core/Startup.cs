// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using Admin.NET.Core.Service;
using AspNetCoreRateLimit;
using DotNetCore.CAP;
using Furion;
using Furion.SpecificationDocument;
using Furion.VirtualFileServer;
using IGeekFan.AspNetCore.Knife4jUI;
using IPTools.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.V2.DependencyInjection;
using Newtonsoft.Json;
using OnceMi.AspNetCore.OSS;
using Savorboard.CAP.InMemoryMessageQueue;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using System.Net;
using System.Reflection;
using System.Net.Mail;
using Microsoft.Extensions.Primitives;
using Admin.NET.Core.DapperEx;
using Elastic.Clients.Elasticsearch.Xpack;
using System.Linq;
namespace Admin.NET.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 配置选项
        services.AddProjectOptions();
        // 电子邮件
        var emailOpt = App.GetConfig<EmailOptions>("Email", true);
        services.AddFluentEmail(emailOpt.DefaultFromEmail, emailOpt.DefaultFromName)
            .AddSmtpSender(new SmtpClient(emailOpt.Host, emailOpt.Port)
            {
                EnableSsl = emailOpt.EnableSsl,
                UseDefaultCredentials = emailOpt.UseDefaultCredentials,
                Credentials = new NetworkCredential(emailOpt.UserName, emailOpt.Password)
            });
        //注册Nacos
        var enableNacos = App.GetConfig<bool>("Nacos:Enabled", false);
        if(enableNacos)
        {
            //services.AddNacosAspNet(App.Configuration, section: "Nacos");
            services.AddNacosV2Config(App.Configuration, sectionName: "Nacos");
            //services.AddNacosV2Naming(App.Configuration, sectionName: "Nacos");
        }
        // 缓存注册
        services.AddCache();
        #region 注册Cap
        services.AddCap(option => {
            option.FailedRetryCount=5;//重试总次数
            bool enabled = App.GetConfig<bool>("EventBus:RabbitMQ:Enabled", true);
            if (enabled)
            {
                option.UseRabbitMQ(config =>
                {
                    config = App.GetConfig<RabbitMQOptions>("EventBus:RabbitMQ", true);
                    //config.HostName = rabbitConfig.HostName;
                    //config.VirtualHost = rabbitConfig.VirtualHost;
                    //config.UserName = rabbitConfig.UserName;
                    //config.Password = rabbitConfig.Password;
                    //config.Port = rabbitConfig.Port;
                });
            }
            else
            {
                option.UseInMemoryMessageQueue();
            }
            //配置消息持久化方式
            var dbOptions = App.GetConfig<DbConnectionOptions>("DbConnection", true);
            var connectionString = dbOptions.ConnectionConfigs.First(t => t.ConfigId.ToString() == SqlSugarConst.MainConfigId)?.ConnectionString;
            var storage= App.GetConfig<string>("EventBus:Storage");
            switch(storage)
            {
                case "Memory": option.UseInMemoryStorage();break;
                case "MySql": option.UseMySql(connectionString);break;
                case "Sqlite": option.UseSqlite(connectionString);break;
                case "SqlServer": option.UseSqlServer(connectionString);break;
                default: option.UseInMemoryStorage();break;
            };
            option.UseDashboard();
        }).AddSubscriberAssembly(Assembly.GetAssembly(typeof(Startup)));
        #endregion
        // 注册SqlSugar
        services.AddSqlSugar();
       
        // JWT
        services.AddJwt<JwtHandler>(enableGlobalAuthorize: true)
            // 添加 Signature 身份验证
            .AddSignatureAuthentication(options =>
            {
                options.Events = SysOpenAccessService.GetSignatureAuthenticationEventImpl();
            });
        // 允许跨域
        services.AddCorsAccessor();
        // 远程请求
        services.AddRemoteRequest();
        // 任务队列
        services.AddTaskQueue();
        // 任务调度
        services.AddSchedule(options =>
        {
            options.AddPersistence<DbJobPersistence>(); // 添加作业持久化器
        });
        // 脱敏检测
        services.AddSensitiveDetection();

        // Json序列化设置
        static void SetNewtonsoftJsonSetting(JsonSerializerSettings setting)
        {
            setting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss"; // 时间格式化
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // 忽略循环引用
            // setting.ContractResolver = new CamelCasePropertyNamesContractResolver(); // 解决动态对象属性名大写
            // setting.NullValueHandling = NullValueHandling.Ignore; // 忽略空值
            // setting.Converters.AddLongTypeConverters(); // long转string（防止js精度溢出） 超过17位开启
            // setting.MetadataPropertyHandling = MetadataPropertyHandling.Ignore; // 解决DateTimeOffset异常
            // setting.DateParseHandling = DateParseHandling.None; // 解决DateTimeOffset异常
            // setting.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }); // 解决DateTimeOffset异常
        };
        //返回格式规范化处理
        services.AddControllersWithViews()
            .AddAppLocalization()
            .AddNewtonsoftJson(options => SetNewtonsoftJsonSetting(options.SerializerSettings))
            //.AddXmlSerializerFormatters()
            //.AddXmlDataContractSerializerFormatters()
            .AddInjectWithUnifyResult<AdminResultProvider>();

        // 三方授权登录OAuth
        services.AddOAuth();

        // ElasticSearch
        services.AddElasticSearch();

        // 配置Nginx转发获取客户端真实IP
        // 注1：如果负载均衡不是在本机通过 Loopback 地址转发请求的，一定要加上options.KnownNetworks.Clear()和options.KnownProxies.Clear()
        // 注2：如果设置环境变量 ASPNETCORE_FORWARDEDHEADERS_ENABLED 为 True，则不需要下面的配置代码
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // 限流服务
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // 事件总线
        services.AddEventBus(options =>
        {
            options.UseUtcTimestamp = false;
            // 不启用事件日志
            options.LogEnabled = false;
            // 事件执行器（失败重试）
            options.AddExecutor<RetryEventHandlerExecutor>();

            #region Redis消息队列

            //// 替换事件源存储器
            //options.ReplaceStorer(serviceProvider =>
            //{
            //    var redisCache = serviceProvider.GetRequiredService<ICache>();
            //    // 创建默认内存通道事件源对象，可自定义队列路由key，如：adminnet
            //    return new RedisEventSourceStorer(redisCache, "adminnet", 3000);
            //});

            #endregion Redis消息队列

            #region RabbitMQ消息队列

            //// 创建默认内存通道事件源对象，可自定义队列路由key，如：adminnet
            //var eventBusOpt = App.GetConfig<EventBusOptions>("EventBus", true);
            //var rbmqEventSourceStorer = new RabbitMQEventSourceStore(new ConnectionFactory
            //{
            //    UserName = eventBusOpt.RabbitMQ.UserName,
            //    Password = eventBusOpt.RabbitMQ.Password,
            //    HostName = eventBusOpt.RabbitMQ.HostName,
            //    Port = eventBusOpt.RabbitMQ.Port
            //}, "adminnet", 3000);

            //// 替换默认事件总线存储器
            //options.ReplaceStorer(serviceProvider =>
            //{
            //    return rbmqEventSourceStorer;
            //});

            #endregion RabbitMQ消息队列
        });

        // 图像处理
        services.AddImageSharp();

        // OSS对象存储
        var ossOpt = App.GetConfig<OSSProviderOptions>("OSSProvider", true);
        services.AddOSSService(Enum.GetName(ossOpt.Provider), "OSSProvider");

        // 模板引擎
        services.AddViewEngine();

        // 即时通讯
        services.AddSignalR(SetNewtonsoftJsonSetting);
        //services.AddSingleton<IUserIdProvider, UserIdProvider>();

        // 系统日志
        services.AddLoggingSetup();

        // 验证码
        services.AddCaptcha();

        // 控制台logo
        services.AddConsoleLogo();

        // 将IP地址数据库文件完全加载到内存，提升查询速度（以空间换时间，内存将会增加60-70M）
        IpToolSettings.LoadInternationalDbToMemory = true;
        // 设置默认查询器China和International
        //IpToolSettings.DefalutSearcherType = IpSearcherType.China;
        IpToolSettings.DefalutSearcherType = IpSearcherType.International;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("Admin.NET", "Admin.NET");
            await next();
        });

        // 图像处理
        app.UseImageSharp();

        // 特定文件类型（文件后缀）处理
        var contentTypeProvider = FS.GetFileExtensionContentTypeProvider();
        // contentTypeProvider.Mappings[".文件后缀"] = "MIME 类型";
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = contentTypeProvider
        });

        //// 启用HTTPS
        //app.UseHttpsRedirection();

        // 启用OAuth
        app.UseOAuth();

        // 添加状态码拦截中间件
        app.UseUnifyResultStatusCodes();

        // 启用多语言，必须在 UseRouting 之前
        app.UseAppLocalization();

        // 路由注册
        app.UseRouting();

        // 启用跨域，必须在 UseRouting 和 UseAuthentication 之间注册
        app.UseCorsAccessor();

        // 启用鉴权授权
        app.UseAuthentication();
        app.UseAuthorization();

        // 限流组件（在跨域之后）
        app.UseIpRateLimiting();
        app.UseClientRateLimiting();

        // 任务调度看板
        app.UseScheduleUI();

        // 配置Swagger-Knife4UI（路由前缀一致代表独立，不同则代表共存）
        app.UseKnife4UI(options =>
        {
            options.RoutePrefix = "api";
            foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
            {
                options.SwaggerEndpoint("/" + groupInfo.RouteTemplate, groupInfo.Title);
            }
        });
        app.UseInject(string.Empty, options =>
        {
            foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
            {
                groupInfo.Description += "<br/><u><b><font color='FF0000'> 👮不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！</font></b></u>";
            }
        });
        app.UseEndpoints(endpoints =>
        {
            // 注册集线器
            endpoints.MapHubs();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}