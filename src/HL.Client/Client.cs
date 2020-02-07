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
        private Authentication.Authentication _authentication;
        private Requestor _requestor;
        private PortfolioOperations _profileOperations;
        #endregion

        #region Authentication
        public virtual Authentication.Authentication Authentication {
            get {
                return _authentication;
            }
        }
        #endregion

        #region Operations
        public virtual PortfolioOperations PortfolioOperations {
            get {
                return _profileOperations;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Constructor
        public Client()
        {
            // Load the requestor
            _requestor = new Requestor();

            // Load the authentication
            _authentication = new Authentication.Authentication(_requestor);

            // Setup the operations 
            _profileOperations = new PortfolioOperations(_requestor);
        }
        #endregion
    }
}
