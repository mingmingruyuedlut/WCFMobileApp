using Interactive.Common;
using Interactive.Constant;
using Interactive.DBManager.Entity;
using Interactive.DBManager.Repository;
using System.Collections.Generic;
using System.Configuration;

namespace Interactive.DBManager.Service
{
    public class UsersRepService
    {
        #region Field and Property
        private UsersRepository userRepository;
        internal UsersRepository _userRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UsersRepository();
                }
                return this.userRepository;
            }
        }

        private TokensRepository tokenRepository;
        internal TokensRepository _tokenRepository
        {
            get
            {
                if (this.tokenRepository == null)
                {
                    this.tokenRepository = new TokensRepository();
                }
                return this.tokenRepository;
            }
        }

        private TokensRepService tokenService;
        internal TokensRepService _tokenService
        {
            get
            {
                if (this.tokenService == null)
                {
                    this.tokenService = new TokensRepService();
                }
                return this.tokenService;
            }
        }

        #endregion

        #region Check mail in FP

        public bool CheckMailInFP(string mail)
        {
            if (mail == ConfigurationManager.AppSettings["FPSucessMail"])
                return true;
            else
                return false;
        }

        #endregion

        #region Check mail in DB

        public bool CheckMailInDB(string mail)
        {
            return _userRepository.CheckMailInDB(mail);
        }

        #endregion

        #region Check password exist

        public bool CheckPasswordExist(string mail)
        {
            return _userRepository.CheckPasswordExist(mail);
        }


        #endregion

        #region Create password

        public void CreatePassword(string token, string password)
        {
            _userRepository.CreatePassword(token, password);
            _tokenRepository.DeleteExpiredToken(token);
        }

        #endregion

        #region Check password

        public bool CheckPassword(string mail, string password)
        {
            if (!string.IsNullOrEmpty(_userRepository.GetCorrectPassword(mail)) && Password.ValidatePassword(password, _userRepository.GetCorrectPassword(mail)))
                return true;
            else
                return false;
        }

        #endregion

        #region Check login

        public LoginEntity CheckLogin(string mail, string password)
        {
            LoginEntity _loginEntity = new LoginEntity();

            if (CheckMailInDB(mail))
            {
                _loginEntity.IsEmailExist = true;
                if (CheckPassword(mail, password))
                {
                    _loginEntity.IsPwdValid = true;
                    _loginEntity.Token = _tokenService.GetTokenWhenLogin(mail);
                }
                else
                {
                    _loginEntity.IsPwdValid = false;
                }
            }
            else
            {
                _loginEntity.IsEmailExist = false;
            }
            return _loginEntity;
        }

        #endregion

        #region Check forget password mail

        public LoginEntity CheckForgetPasswordMail(string mail)
        {
            LoginEntity _loginEntity = new LoginEntity();
            if (CheckMailInDB(mail))
            {
                _loginEntity.IsCustomerExistInMDB = true;
            }
            return _loginEntity;
        }
        #endregion

        #region Update password

        public void UpdatePassword(string token, string password)
        {
            _userRepository.UpdatePassword(token, password);
            _tokenService.DeleteExpiredToken(token);
        }

        #endregion

        #region Get userid by token

        public int GetUserIdByToken(string token)
        {
            return _userRepository.GetUserIdByToken(token);
        }

        #endregion

        public int GetUserIdByEmail(string email)
        {
            return _userRepository.GetUserIdFromEmail(email);
        }

        public void UpdateCompanyIdAndBillingId(int companyId, string billingId, string email)
        {
            _userRepository.UpdateCompanyIdAndBillingId(companyId, billingId, email);
        }

        public List<UserEntity> GetOtherUsersToShareIncident(UserEntity user, int incidentId)
        {
            return _userRepository.GetOtherUsersToShareIncident(user, incidentId);
        }

        public List<UserEntity> GetSharedUsers(UserEntity user, int incidentId)
        {
            return _userRepository.GetSharedUsers(user, incidentId);
        }

        public List<UserEntity> GetSharedUsersExceptOwner(UserEntity user, int incidentId)
        {
            return _userRepository.GetSharedUsersExceptOwner(user, incidentId);
        }

        public bool CheckUserIsIncidentOwner(UserEntity user, int incidentId)
        {
            return _userRepository.CheckUserIsIncidentOwner(user, incidentId);
        }

        public UserEntity GetUserByEmail(UserEntity user)
        {
            return _userRepository.GetUserByEmail(user);
        }
    }
}
