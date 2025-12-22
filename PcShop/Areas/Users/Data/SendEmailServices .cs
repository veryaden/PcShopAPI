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
    public class SendEmailServices : ISendEmailService
    {
        private readonly IConfiguration _config;

        public SendEmailServices(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendAsync(string to, string subject, string html)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                "PCShop",
                _config["Mail:From"]
            ));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = html };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(
                _config["Mail:From"],
                _config["Mail:AppPassword"]
            );
            var frontendUrl = _config["FrontendUrl"];
            if (string.IsNullOrWhiteSpace(frontendUrl))
                throw new Exception("FrontendUrl 未設定");

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }



    }
}
