using Interactive.Services.Contract;
using Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LoginWCFService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select LoginWCFService.svc or LoginWCFService.svc.cs at the Solution Explorer and start debugging.
    public class LoginWCFService : ILoginWCFService
    {
        #region Field and Property
        private LoginService loginService;
        internal LoginService _loginService
        {
            get
            {
                if (this.loginService == null)
                {
                    this.loginService = new LoginService();
                }
                return this.loginService;
            }
        }
        #endregion

        public Login IsLogin(Login login)
        {
            return _loginService.IsLogin(login);
        }

        public Login IsLogout()
        {
            return _loginService.IsLogout();
        }
    }
}
