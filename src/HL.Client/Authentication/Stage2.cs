using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HL.Client.Authentication
{
    /// <summary>
    /// Defines stage two of the login process.
    /// </summary>
    internal class Stage2 : BaseStage
    {
        private const string _path = "my-accounts/login-step-two";
        private const string _expectedResponse = "my-accounts";

        #region Fields
        private string _password;
        private string _securityNumber;
        #endregion

        #region Methods
        public override Dictionary<string, string> BuildFields()
        {
            // Determine the inputs needed from the security number
            var container = Document.DocumentNode.Descendants("div").Where(d => d.HasClass("secure-number-container")).SingleOrDefault();

            if (container == null)
                throw new Exception("Unable to find the secure number container for Stage 2.");

            // Filter out anything that isn't either grey-box or container
            HtmlNode[] elements = container.Descendants("div").Where(d => d.HasClass("secure-number-grey-box") || d.HasClass("secure-number-container__label")).ToArray();

            if (elements.Length == 0)
                throw new Exception("Unable to get security number inputs to determine which codes are required.");

            // Determine which digits are required,
            List<int> requiredDigits = new List<int>();
            for(int i = 0; i < elements.Length; i++)
            {
                if (elements[i].HasClass("secure-number-container__label"))
                    requiredDigits.Add(i);
            }

            // Determine which digits of our security number to send.
            int[] securityNumber = new int[3];
            for (int i = 0; i < securityNumber.Length; i++)
                securityNumber[i] = int.Parse(_securityNumber.Substring(requiredDigits[i], 1));

            return new Dictionary<string, string>()
            {
                { "online-password-verification", _password },
                { "secure-number[1]", securityNumber[0].ToString() },
                { "secure-number[2]", securityNumber[1].ToString() },
                { "secure-number[3]", securityNumber[2].ToString() },
                { "submit", "Log in" }
            };
        }
        #endregion

        #region Constructor
        public Stage2(Requestor requestor, string password, string securityNumber)
            : base(requestor)
        {
            // Set the path for this stage.
            Path = _path;
            ExpectedResponse = _expectedResponse;

            _password = password;
            _securityNumber = securityNumber;
        }
        #endregion
    }
}
