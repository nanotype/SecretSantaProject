using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaProject.Module
{
	internal class MailingSystem
	{
		private static string monMail = "preve.victor@gmail.com";
		private static string monMDP = "Genetique47$";
		public static SmtpClient UseGmail()
		{
			SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
			{
				Credentials = new NetworkCredential(monMail, monMDP),
				UseDefaultCredentials = false,
				EnableSsl = true
			};
			return client;
		}

		public static bool SendMail(SmtpClient client)
		{
			client.Send(monMail, monMail, "SECRET_SANTA_PROJECT", "Ceci est mon premier test d'envois de mail sur C# !!!");
			Console.WriteLine("Sent");
			return true;
		}
	}
}
