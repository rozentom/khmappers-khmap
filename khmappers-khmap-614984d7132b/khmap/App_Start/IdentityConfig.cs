using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using khmap.Models;
using AspNet.Identity.MongoDB;
using khmap.App_Start;
using System.Net.Mail;
using System.Configuration;
using System;

namespace khmap
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationIdentityContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //return Task.FromResult(0);

            MailMessage email = new MailMessage(new MailAddress("khmap@bgu.ac.il", "(do not reply)"),
            new MailAddress(message.Destination));

            email.Subject = message.Subject;
            email.Body = message.Body;

            email.IsBodyHtml = true;

            using (var mailClient = new GmailEmailService())
            {
                //In order to use the original from email address, uncomment this line:
                //email.From = new MailAddress(mailClient.UserName, "(do not reply)");

                await mailClient.SendMailAsync(email);
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }


    //public class GmailEmailService : SmtpClient
    //{
    //    // Gmail user-name
    //    public string UserName { get; set; }

    //    public GmailEmailService() :
    //        base(ConfigurationManager.AppSettings["GmailHost"], Int32.Parse(ConfigurationManager.AppSettings["GmailPort"]))
    //    {
    //        //Get values from web.config file:
    //        this.UserName = ConfigurationManager.AppSettings["GmailUserName"];
    //        this.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["GmailSsl"]);
    //        this.UseDefaultCredentials = false;
    //        this.Credentials = new System.Net.NetworkCredential(this.UserName, ConfigurationManager.AppSettings["GmailPassword"]);
    //    }
    //}

    public class GmailEmailService : SmtpClient
    {
        // Gmail user-name
        public string UserName { get; set; }

        public GmailEmailService()
        {
            //Get values from web.config file:
            this.Host = "smtp.bgu.ac.il";
            this.UserName = ConfigurationManager.AppSettings["GmailUserName"];
            this.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["GmailSsl"]);
            this.UseDefaultCredentials = false;
            //this.Credentials = new System.Net.NetworkCredential(this.UserName, ConfigurationManager.AppSettings["GmailPassword"]);
        }
    }
}
