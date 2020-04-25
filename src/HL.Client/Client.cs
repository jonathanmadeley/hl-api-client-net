using HL.Client;
using HL.Client.Authentication;
using HL.Client.Operations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HL.Client
{
    /// <summary>
    /// Defines the base client.
    /// </summary>
    public class Client
    {
        #region Constants
        private const string CookieName = "HLWEBsession";
        #endregion

        #region Fields
        private bool _isAuthenticated = false;
        private Requestor _requestor;
        private AccountOperations _accountOperations;
        #endregion

        #region Authentication
        /// <summary>
        /// Authenticate the client.
        /// </summary>
        /// <param name="username">Your username.</param>
        /// <param name="password">Your password.</param>
        /// <param name="birthday">Your birthday.</param>
        /// <param name="securityNumbe">Your security number.</param>
        /// <returns></returns>
        public async Task AuthenticateAsync(string username, string password, DateTime birthday, string securityNumbe)
        {
            // Start stage 1
            Stage1 s1 = new Stage1(_requestor,
                                    username,
                                    birthday);

            // Run stage 1
            await s1.RunStageAsync().ConfigureAwait(false);

            // Start stage 2
            Stage2 s2 = new Stage2(_requestor,
                                    password,
                                    securityNumbe);

            // Run stage 2
            await s2.RunStageAsync().ConfigureAwait(false);

            _isAuthenticated = true;
        }
        #endregion

        #region Operations
        /// <summary>
        /// Gets the account operations.
        /// </summary>
        public virtual AccountOperations AccountOperations
        {
            get {
                return _accountOperations;
            }
        }
        #endregion

        #region Constructor
        public Client()
        {
            // Load the requestor
            _requestor = new Requestor();

            // Setup the operations 
            _accountOperations = new AccountOperations(_requestor);
        }
        #endregion
    }
}
