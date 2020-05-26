using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Mail;

namespace BurnSoft.Mail
{
    /// <summary>
    /// Class SimpleSmtpClient. A version quick and simple smtp client to send email notifications
    /// </summary>
    public class Send
    {
        #region "Error Handling"

        /// <summary>
        /// Main Class Name for error dumping.
        /// </summary>
        private static string ClassLocation => "BurnSoft.Mail.Send";

        /// <summary>
        /// General Error Message Format getting the ClassLocation property and appending it to the sLocation and also appended it 
        /// to the ex.message, this was done to allow you narrow down the location of were the error occurred.
        /// </summary>
        /// <param name="sLocation">Sub or Function Name</param>
        /// <param name="ex">Exception</param>
        /// <returns>string Class.SubOrFunction - Error Message</returns>
        /// <example>error = ErrorMessage("getDBVersion", ex);</example>
        /// <remarks>copy and fill in the blank - ErrorMessage("", ex);</remarks>
        private static string ErrorMessage(string sLocation, Exception ex) => $"{ClassLocation}.{sLocation} - {ex.Message}  {ex.InnerException}";
        
        #endregion

        /// <summary>
        /// This sub will send an email to a person or group of people in HTML Format
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="server">The server.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="errMsg">The error MSG.</param>
        /// <param name="useHtml">if set to <c>true</c> [use HTML].</param>
        /// <param name="port">The port.</param>
        /// <param name="useSsl">if set to <c>true</c> [use SSL].</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <param name="useAnonymousAuth">if set to <c>true</c> [use anonymous authentication].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="Exception">
        /// Missing Server to Send to!
        /// or
        /// The User Name to log in is missing!
        /// or
        /// The password for the user name is missing!
        /// or
        /// There is no one listed to send it to!
        /// </exception>
        public static bool SendMail(string to, string from, string subject, string server, string uid, string pwd, string message, string attachments, out string errMsg, bool useHtml = true, int port = 25, bool useSsl = false, string cc = "", string bcc = "", bool useAnonymousAuth = false)
        {
            bool bAns = false;
            errMsg = @"";
            try
            {
                if (server.Length == 0) throw new Exception("Missing Server to Send to!");
                if (!useAnonymousAuth)
                {
                    if (uid.Length == 0) throw new Exception("The User Name to log in is missing!");
                    if (pwd.Length == 0) throw new Exception("The password for the user name is missing!");
                }

                List<string> sendTo = to.Split(',').Where(e => e.Length > 0).ToList();
                if (sendTo.Count == 0) throw new Exception("There is no one listed to send it to!");
                List<string> sendCc = cc.Split(',').Where(e => e.Length > 0).ToList();
                List<string> sendBcc = bcc.Split(',').Where(e => e.Length > 0).ToList();
                List<string> sendAttachments = attachments.Split(',').Where(e => e.Length > 0).ToList();

                MailMessage msg = new MailMessage();

                msg.From = new MailAddress(from);

                foreach (string t in sendTo)
                {
                    msg.To.Add(t.Trim());
                }

                foreach (string t in sendCc)
                {
                    msg.CC.Add(t);
                }

                foreach (string b in sendBcc)
                {
                    msg.Bcc.Add(b);
                }

                foreach (string a in sendAttachments)
                {
                    Attachment att = new Attachment(a);
                    msg.Attachments.Add(att);
                }

                msg.Subject = subject;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.Body = message;
                msg.BodyEncoding = Encoding.UTF8;

                msg.IsBodyHtml = useHtml;


                using (SmtpClient client = new SmtpClient())
                {
                    client.Credentials = new NetworkCredential(uid, pwd);
                    client.Port = port;
                    client.Host = server;
                    client.EnableSsl = useSsl;
                    client.Send(msg);
                }
                bAns = true;
            }
            catch (Exception e)
            {
                errMsg = ErrorMessage("SendMail", e);
            }

            return bAns;
        }
        /// <summary>
        /// Sends mail using the long version function, but is simplified to send a basic html email
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="message">The message.</param>
        /// <param name="errMsg">The error MSG.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SendMail(string to, string from, string subject, string server, int port, string uid,
            string pwd, string message, out string errMsg)
        {
            return SendMail(to, from, subject, server, uid, pwd, message, "", out errMsg, true, port, (port > 25));
        }
        /// <summary>
        /// Sends mail using the long version function, but is simplified to send a basic html email with attachments
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="errMsg">The error MSG.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SendMail(string to, string from, string subject, string server, int port, string uid,
            string pwd, string message, string attachments, out string errMsg)
        {
            return SendMail(to, from, subject, server, uid, pwd, message, attachments, out errMsg, true, port, (port > 25));
        }
    }
}
