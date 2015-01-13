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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RegistrationWCFService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RegistrationWCFService.svc or RegistrationWCFService.svc.cs at the Solution Explorer and start debugging.
    public class RegistrationWCFService : IRegistrationWCFService
    {
        #region Field and Property
        private RegistrationService rgService;
        internal RegistrationService _rgService
        {
            get
            {
                if (this.rgService == null)
                {
                    this.rgService = new RegistrationService();
                }
                return this.rgService;
            }
        }
        #endregion

        public SetPassword CreatePassword(SetPassword password)
        {
            return _rgService.CreatePassword(password);
        }

        public CheckToken CheckToken()
        {
            return _rgService.CheckToken();
        }

        public CheckUser CheckUser(CheckUser user)
        {
            return _rgService.CheckUser(user);
        }

        public CheckUser ForgetPassword(CheckUser user)
        {
            return _rgService.ForgetPassword(user);
        }

        public SetPassword UpdatePassword(SetPassword password)
        {
            return _rgService.UpdatePassword(password);
        }

        public string GetToken(string email)
        {
            return _rgService.GetToken(email);
        }

        public UserModel GetUserDetail()
        {
            return _rgService.GetUserDetail();
        }
    }
}
