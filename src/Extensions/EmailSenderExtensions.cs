using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using workflow.Services;

namespace workflow.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string message)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email", message);
        }

        public static Task SendTemporaryCredentialsAsync(this IEmailSender emailSender, string email, string message)
        {
            return emailSender.SendEmailAsync(email, "Temporary Credentials", message);
        }
    }
}
