using HL.Client.Authentication.Stages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HL.Client.Authentication
{
    /// <summary>
    /// Defines an authentication helper class.
    /// </summary>
    public class Authentication
    {
        #region Fields
        private Requestor _requestor;
        private string _username;
        private string _password;
        private DateTime _birthday;
        private string _securityNumber;
        private bool _isAuthenticated = false;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if the client is authenticated.
        /// </summary>
        public bool IsAuthenticated {
            get {
                return _isAuthenticated;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Start the authentication process
        /// </summary>
        public async Task StartAuthentication(string username, string password, DateTime birthday, string securityNumber)
        {
            _username = username;
            _password = password;
            _birthday = birthday;
            _securityNumber = securityNumber;

            // Start stage 1
            Stage1 s1 = new Stage1(_requestor,
                                    _username, 
                                    _birthday);

            // Run stage 1
            await s1.RunStage();

            // Start stage 2
            Stage2 s2 = new Stage2(_requestor,
                                    _password,
                                    _securityNumber);

            // Run stage 2
            await s2.RunStage();
            _isAuthenticated = true;
        }
        #endregion

        #region Constructor
        public Authentication(Requestor requestor)
        {
            _requestor = requestor;
        }
        #endregion
    }
}
