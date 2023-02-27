using Serilog;
using Social.Services.Services;
using System;
using System.Net.Mail;
using System.Threading.Tasks;


namespace Social.Sercices.Helpers
{
    public class EmailHelper
    {
        private string SenderMail = "Hello@friendzr.com";
        private string _pass = "asd@1234A";
        private readonly ILogger logger;
        private readonly IGlobalMethodsService globalMethodsService;
        readonly string BaseUrlDomain;
        public EmailHelper(IGlobalMethodsService globalMethodsService, ILogger logger)
        {
            this.logger = logger;
            this.globalMethodsService = globalMethodsService;
            BaseUrlDomain = globalMethodsService.GetBaseDomain();
        }
        public async Task SendEmailregistration(string toEmailAddress, string subject, string htmlMessage, int code, string RedirectURL, string username)
        {
            logger.Information("Send Email Configration Start");
            MailMessage m = new MailMessage();
            System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
            m.From = new MailAddress(SenderMail);
            m.To.Add(toEmailAddress);
            m.Subject = "Verify your email address";
            m.Body = "<!DOCTYPE html><html lang='en'><head>  <meta charset='UTF-8' /> <meta http-equiv='X-UA-Compatible' content='IE=edge' /> <meta name='viewport' content='width=device-width, initial-scale=1.0' /> <title></title></head><body style='margin: 3px'> <div class='container' style='  height: 110vh'><div class='body-email' style='top: 50%; left: 50%; '> <div class='title'><img style='width: 70px; border-radius: 50%;' src='https://www.friendzsocialmedia.com/assets/media/logos/favicon.ico' /><h4 style='margin: 3px'>Please verify your email address.</h4></div><h4>Hi, " + username + ".</h4><div style='font-size: 15px'>Welcome to Friendzr. To start using the app and create your profile, you need to confirm your email address.</div><div class='code' style='font-size: 15px;WIDTH: 70%;TEXT-ALIGN: center; font-weight: bold; margin: 17px'><a style=' color: white;background-color: #49b498;padding: 10px;text-decoration:none' href='" + RedirectURL + "?email=" + toEmailAddress + "&code=" + code + "'>Verify your email address</a> </div><h5 >If you did not sign up to create an account on Friendzr, please ignore this email and your email will be deleted. You can unsubscribe at anytime by clicking<a style = ' color:#49b498;padding: 1px;border-radius: 13%;' href = '" + globalMethodsService.GetBaseDomain() + "/User/UserAccount/DELETEEmail" + "?email=" + toEmailAddress + "&code=" + code + "'>here</a> </h5><footer><h5 style='margin-bottom:-20px;'>Sincerely</h5><h5>Friendzr Team</h5></footer></div> </div></body></html>";
            m.IsBodyHtml = true;
            sc.Host = "www.friendzsocialmedia.com";
            string str1 = "gmail.com";
            string str2 = SenderMail;
            if (str2.Contains(str1))
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = true;
                    sc.Send(m);
                    //Response.Write("Email Send successfully");
                }
                catch (Exception ex)
                {
                    // Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    // Response.Write("<BR><BR>If you are using gmail smtp to send email for the first time, please refer to this KB to setup your gmail account: http://www.smarterasp.net/support/kb/a1546/send-email-from-gmail-with-smtp-authentication-but-got-5_5_1-authentication-required-error.aspx?KBSearchID=137388");
                    //Response.End();
                    // throw ex;
                }
            }
            else
            {
                try
                {
                    sc.Port = 587/*25*/;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = false;
                    sc.Send(m);

                    logger.Information($"Send Email Configration Success And Reciver Email Is:{ toEmailAddress}");

                    // Response.Write("Email Send successfully");
                }
                catch (Exception ex)
                {
                    logger.Information(ex, "Send Email Configration Fail", ex.Message);

                    //  Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    // Response.End();
                    //  throw ex;
                }
            }
        }

        public async Task SendEmailchangepassword(string phone, string toEmailAddress, string subject, string httmlMessage, int code, string passwordResetLink,string userName)
        {

            MailMessage m = new MailMessage();
            System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
            m.From = new MailAddress(SenderMail);
            m.To.Add(toEmailAddress);
            m.Subject = subject;
            //m.Body = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8' />" +
            //     "<meta http-equiv='X-UA-Compatible' content='IE=edge' />" +
            //     "<meta name='viewport' content='width=device-width, initial-scale=1.0' />" +
            //     "<title>Change password</title></head><body style='margin: 3px'><div class='container' style='text-align: center; height: 110vh'><div class='body-email' style='position: absolute;top: 50%; left: 50%; transform: translate(-40%, -40%);'>" +
            //     "<div class='title'><h2 style='margin: 3px'>Friendzr</h2></div><h1>Let's Change password</h1><div style='font-size: 17px'>  We got a request to change password for your Friendzr account  .</div><div class='code'style='font-size: 20px; font-weight: bold; margin: 17px'></br> " +
            //     "<a href='" + passwordResetLink + "'>Change password</a></div></div></div></body></html>";

            m.Body = "<!DOCTYPE html><html lang='en'><head>  <meta charset='UTF-8' /> <meta http-equiv='X-UA-Compatible' content='IE=edge' /> <meta name='viewport' content='width=device-width, initial-scale=1.0' /> <title></title></head><body style='margin: 3px'> <div class='container' style='  height: 110vh'><div class='body-email' style='top: 50%; left: 50%; '> <div class='title'><img style='width: 70px; border-radius: 50%;' src='https://www.friendzsocialmedia.com/assets/media/logos/favicon.ico' /><h4 style='margin: 3px'>Reset your password.</h4></div><h4>Hi, " + userName + ".</h4><div style='font-size: 15px'>To reset your password, click the button below.</div><div class='code' style='font-size: 15px;WIDTH: 70%;TEXT-ALIGN: center; font-weight: bold; margin: 17px'><a style=' color: white;background-color: #49b498;padding: 10px' href='" + passwordResetLink +"'>Reset password</a> </div><h5 >If you did not request to change your password, please ignore this email. You can ensure your email is unsubscribed by clicking <a style = ' color:#49b498;padding: 1px;border-radius: 13%;' href = '" + globalMethodsService.GetBaseDomain() + "/User/UserAccount/DELETEEmail" + "?email=" + toEmailAddress + "&code=" + code + "'>here</a> </h5><footer><h5 style='margin-bottom:-20px;'>Sincerely</h5><h5>Friendzr Team</h5></footer></div> </div></body></html>";


            m.IsBodyHtml = true;
            sc.Host = "www.friendzsocialmedia.com";
            string str1 = "gmail.com";
            string str2 = SenderMail;
            if (str2.Contains(str1))
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = true;
                    sc.Send(m);
                    //Response.Write("Email Send successfully");
                }
                catch (Exception ex)
                {
                    // Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    // Response.Write("<BR><BR>If you are using gmail smtp to send email for the first time, please refer to this KB to setup your gmail account: http://www.smarterasp.net/support/kb/a1546/send-email-from-gmail-with-smtp-authentication-but-got-5_5_1-authentication-required-error.aspx?KBSearchID=137388");
                    //Response.End();
                    // throw ex;
                }
            }
            else
            {
                try
                {
                    sc.Port = 25;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = false;
                    sc.Send(m);
                    // Response.Write("Email Send successfully");
                }
                catch (Exception ex)
                {
                    //  Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    // Response.End();
                    //  throw ex;
                }
            }
        }



        public async Task SendEmail(string toEmailAddress, string redirectUrl, string subject, string httmlMessage , string userName)

        {
            MailMessage m = new MailMessage();
            System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
            m.From = new MailAddress(SenderMail);
            m.To.Add(toEmailAddress);
            m.Subject = subject;
            m.Body = "<!DOCTYPE html><html lang='en'><head>  <meta charset='UTF-8' /> <meta http-equiv='X-UA-Compatible' content='IE=edge' /> <meta name='viewport' content='width=device-width, initial-scale=1.0' /> <title></title></head><body style='margin: 3px'> <div class='container' style='  height: 110vh'><div class='body-email' style='top: 50%; left: 50%; '> <div class='title'><img style='width: 70px; border-radius: 50%;' src='https://www.friendzsocialmedia.com/assets/media/logos/favicon.ico' /><h4 style='margin: 3px'>Reset your password.</h4></div><h4>Hi, " + userName + ".</h4><div style='font-size: 15px'>To reset your password, click the button below.</div><div class='code' style='font-size: 15px;WIDTH: 70%;TEXT-ALIGN: center; font-weight: bold; margin: 17px'><a style=' color: white;background-color: #49b498;padding: 10px' href='" + redirectUrl + "?email=" + toEmailAddress + "'>Reset password</a> </div><h5 >If you did not request to change your password, please ignore this email. You can ensure your email is unsubscribed by clicking <a style = ' color:#49b498;padding: 1px;border-radius: 13%;' href = '" + globalMethodsService.GetBaseDomain() + "/User/UserAccount/DELETEEmail" + "?email=" + toEmailAddress +"'>here</a> </h5><footer><h5 style='margin-bottom:-20px;'>Sincerely</h5><h5>Friendzr Team</h5></footer></div> </div></body></html>";


            m.IsBodyHtml = true;
            sc.Host = "www.friendzsocialmedia.com";
            string str1 = "gmail.com";
            string str2 = SenderMail;
            if (str2.Contains(str1))
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = true;
                    sc.Send(m);
                    //Response.Write("Email Send successfully");
                }
                catch (Exception ex)
                {
                    // Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    // Response.Write("<BR><BR>If you are using gmail smtp to send email for the first time, please refer to this KB to setup your gmail account: http://www.smarterasp.net/support/kb/a1546/send-email-from-gmail-with-smtp-authentication-but-got-5_5_1-authentication-required-error.aspx?KBSearchID=137388");
                    //Response.End();
                    // throw ex;
                }
            }
            else
            {
                try
                {
                    sc.Port = 25;
                    sc.Credentials = new System.Net.NetworkCredential(SenderMail, _pass);
                    sc.EnableSsl = false;
                    sc.Send(m);
                    // Response.Write("Email Send successfully");
                }
                catch (Exception ex)
                {
                    //  Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    // Response.End();
                    //  throw ex;
                }
            }
        }
       
    }
}