using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Mtn.Library.Extensions;

namespace Mtn.Library.Utils
{
    /// <summary>
    /// Class for attachments and images
    /// </summary>
    public class MtnFile
    {
        /// <summary>
        /// 
        /// </summary>
        public bool LoadFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MemoryStream MemoryStreamInfo { get { return Data.Length > 0 ?new MemoryStream(Data):null; } }
    }

    /// <summary>
    /// Class to send EMail
    /// </summary>
    public static class Email
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="mailBody"></param>
        /// <param name="from"></param>
        /// <param name="replyTo"></param>
        /// <param name="attachs"></param>
        /// <param name="isHtml"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="useCredentials"></param>
        /// <param name="userName"></param>
        /// <param name="passwd"></param>
        /// <param name="enableSsl"></param>
        /// <returns></returns>
        public static bool Send(string to, string subject, string mailBody, string from = null, string replyTo = null, IEnumerable<MtnFile> attachs = null, bool isHtml = true, string host = null, int port = -1, bool useCredentials = true, string userName = null, string passwd = null, bool? enableSsl = null)
        {
            var ret = false;

            try
            {
                if (host.IsNullOrWhiteSpaceMtn())
                    host = Configuration.Config.GetString("Mtn.Library.Email.SmtpHost");
                if (port <= 0)
                    port = Configuration.Config.GetNullableInt32("Mtn.Library.Email.SmtpPort")??587;
                
                using (var client = new System.Net.Mail.SmtpClient(host, port))
                {
                    // Create a network credential with your SMTP user name and password.
                    if (useCredentials)
                    {
                        if (userName == null)
                            userName = Configuration.Config.GetString("Mtn.Library.Email.SmtpUser");
                        
                        if (passwd == null)
                            passwd = Configuration.Config.GetString("Mtn.Library.Email.SmtpPassword");

                        client.Credentials = new System.Net.NetworkCredential(userName, passwd);
                    }
                    else
                    {
                        client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    }

                    // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                    // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                    enableSsl = enableSsl.HasValue
                        ? enableSsl
                        : Configuration.Config.GetNullableBoolean("Mtn.Library.Email.EnableSsl");

                    client.EnableSsl = enableSsl ?? true;

                    var mailMsg = new System.Net.Mail.MailMessage();

                    from = from.IsNullOrWhiteSpaceMtn()?Configuration.Config.GetString("Mtn.Library.Email.DefaultFrom"):from;

                    var mailAddress = @from.IndexOf(':') > 0 ? new System.Net.Mail.MailAddress(@from.Split(':')[0], @from.Split(':')[1]) : new System.Net.Mail.MailAddress(@from);

                    mailMsg.From = mailAddress;

                    replyTo = replyTo.IsNullOrWhiteSpaceMtn() ? Configuration.Config.GetString("Mtn.Library.Email.DefaultReplyTo") : replyTo;

                    if(!replyTo.IsNullOrWhiteSpaceMtn())
                        mailMsg.ReplyToList.Add(replyTo);
			        
                    mailMsg.To.Add(to);

                    mailMsg.Subject = subject;
                    mailMsg.Body = mailBody;
                    mailMsg.IsBodyHtml = isHtml;
                    
                    if(attachs != null) 
                    {
                        foreach (var att in attachs)
                        {
                            mailMsg.Attachments.Add(att.LoadFile
                                ? new System.Net.Mail.Attachment(att.FilePath, att.MediaType)
                                : new System.Net.Mail.Attachment(att.MemoryStreamInfo, att.FileName, att.MediaType));
                        }
                    }

                    // Send the email.
                    
                   client.Send(mailMsg);
                   
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }

            return ret;
        }
    }
}