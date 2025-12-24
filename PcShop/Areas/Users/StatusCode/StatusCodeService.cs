using Google.Apis.Auth;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.DTO;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using MailKit;
using System.Net.Mail;
using MailKit.Net.Smtp;


namespace PcShop.Areas.Users.Data
{
    public class StatusCodeService 
    {   public class ServiceResult
        {
            public bool Success { get; set; }
            public int StatusCode { get; set; } = 400;
            public string Message { get; set; } = "";

            public static ServiceResult Ok() => new() { Success = true, StatusCode = 200 };
            public static ServiceResult Fail(string message, int statusCode = 400)
                => new() { Success = false, StatusCode = statusCode, Message = message };
        }
    }
}
