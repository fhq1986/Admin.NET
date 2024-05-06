// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using DotNetCore.CAP;

namespace Admin.NET.Core.Service;

/// <summary>
/// 事件总线服务
/// </summary>
[ApiDescriptionSettings(Order = 370)]
public class EventBusService : IDynamicApiController, ITransient,ICapSubscribe
{
    
    private readonly ICapPublisher _capPublisher;

    public EventBusService(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [DisplayName("发送消息")]
    public async Task Send(string message)
    {
        await _capPublisher.PublishAsync("test.queue",message,"test.callback");
    }
    /// <summary>
    /// 消息回调
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [DisplayName("消息回调")]
    [CapSubscribe("test.callback")]
    public async Task<string> CallBack(string message)
    {
       return  await Task.FromResult($"回调:{message}");
    }
    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [DisplayName("订阅消息")]
    [CapSubscribe("test.queue")]
    public async Task<string> Receiver(string message)
    {
       Console.WriteLine($"hello:{message}");
       return  await Task.FromResult($"hello:{message}");
    }
}