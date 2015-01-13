using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.Constant
{
    public static class EmailHTML
    {
        public static string ConfirmationEmailContent(string mail)
        {
            return "<p>Thank you.</p>"
                + "<p>You have successfully registered for the Interactive Customer Mobile Application which will give you the ability to place a service call to Interactive and receive updates as to ongoing status.</p>"
                + "<p>You will receive an email with instructions on creating your login details shortly.</p>";
        }

        public static string RequestSubmitEmailContent = "<p>Thank you.</p>"
            + "<p>You have successfully registered for the Interactive Customer Mobile Application which will give you the ability to place a service call to Interactive and receive updates as to ongoing status.</p>"
            + "<p>You will receive a call shortly from the Interactive Service desk to verify your access and guide you towards completion on the process.</p>";

        public static string ForgetPasswordContent(string mail)
        {
            return "<p>Please click the below link to reset your password.</p>";
        }

        public static string ConfirmationEmailSubject = "Interactive ServiceView Registration - Email Verification";
        public static string RequestSubmitEmailSubject = "Interactive ServiceView Registration - Authentication Request Submitted";
        public static string ForgetPasswordSubject = "Forget password";

    }
}
