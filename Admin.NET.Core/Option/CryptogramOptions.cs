﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 密码配置选项
/// </summary>
public sealed class CryptogramOptions : IConfigurableOptions
{
    /// <summary>
    /// 是否开启密码强度验证
    /// </summary>
    public bool StrongPassword { get; set; }

    /// <summary>
    /// 密码强度验证正则表达式
    /// </summary>
    public string PasswordStrengthValidation { get; set; }

    /// <summary>
    /// 密码强度验证提示
    /// </summary>
    public string PasswordStrengthValidationMsg { get; set; }

    /// <summary>
    /// 密码类型
    /// </summary>
    public string CryptoType { get; set; }

    /// <summary>
    /// 公钥
    /// </summary>
    public string PublicKey { get; set; }

    /// <summary>
    /// 私钥
    /// </summary>
    public string PrivateKey { get; set; }
}