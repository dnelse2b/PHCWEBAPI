using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using SGOFWS.Templates;

namespace SGOFWS.Helper;

public class EmailHelper
{
	private LogHelper _log = new LogHelper();

	private readonly StringTemplates _stringTemplates = new StringTemplates();

	public async Task sendEmailUsingTemplateAsync<T>(StringBuilder template, string fromName, string from, string to, string subject, T templateData)
	{
		try
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			IConfigurationRoot config = configuration.Build();
			string smtpserver = config.GetSection("EMAIL_DATA").GetSection("SMTP").Value;
			string port = config.GetSection("EMAIL_DATA").GetSection("PORT").Value;
			string username = config.GetSection("EMAIL_DATA").GetSection("USERNAME").Value;
			string password = config.GetSection("EMAIL_DATA").GetSection("PASSWORD").Value;
			SmtpSender sender = new SmtpSender(() => new SmtpClient(smtpserver)
			{
				UseDefaultCredentials = false,
				Port = int.Parse(port),
				EnableSsl = false,
				Credentials = new NetworkCredential(username, password)
			});
			Email.DefaultSender = sender;
			Email.DefaultRenderer = new RazorRenderer();
			IFluentEmail email = Email.From(from, fromName).To(to, "").Subject(subject)
				.UsingTemplate(template.ToString(), templateData);
			await email.SendAsync();
		}
		catch (Exception)
		{
		}
	}
}
