using System;
using System.Collections.Generic;
namespace HL.Client.Authentication
{
    /// <summary>
    /// Defines the setup for login stage 1.
    /// </summary>
    internal class Stage1 : BaseStage
    {
        private const string _path = "my-accounts/login-step-one";
        private const string _expectedResponse = "my-accounts/login-step-two";

        #region Fields
        private string _username;
        private DateTime _birthday;
        #endregion

        #region Methods
        public override Dictionary<string, string> BuildFields()
        {
            return new Dictionary<string, string>()
            {
                { "username", _username },
                { "date-of-birth",  $"{_birthday.ToString("ddMMyy")}" }
            };
        }
        #endregion

        #region Constructor
        public Stage1(Requestor requestor, string username, DateTime birthday) 
            : base(requestor)
        {
            // Set the path for this stage.
            Path = _path;
            ExpectedResponse = _expectedResponse;

            _username = username;
            _birthday = birthday;
        }
        #endregion
    }
}
