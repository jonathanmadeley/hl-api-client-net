using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HL.Client.Entities;
using HtmlAgilityPack;

namespace HL.Client.Operations
{
    public class LinkedAccountOperations
    {
        #region Fields
        private Requestor _requestor;
        #endregion

        #region Methods
        /// <summary>
        /// Gets a list of all accounts.
        /// </summary>
        /// <returns>The list of accounts</returns>
        public async Task<ClientAccountEntity[]> ListAsync(CancellationToken cancellationToken = default)
        {
            // Make request
            var response = await _requestor.GetAsync("my-accounts");

            HtmlDocument doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

            return ListAccounts(doc);
        }

        /// <summary>
        /// Parse the document to extract the list of accounts
        /// </summary>
        /// <param name="doc">The document containing account information</param>
        /// <returns>The list of accounts</returns>
        public static ClientAccountEntity[] ListAccounts(HtmlDocument doc)
        {
            // Get list
            var list = doc.DocumentNode.Descendants("ul").SingleOrDefault(x => x.HasClass("linked-account-tabs"));
            if (list == null)
            {
                return Array.Empty<ClientAccountEntity>();
            }

            var listItems = list.Descendants("li").ToArray();

            // For each list item, convert into client account model.
            ClientAccountEntity[] accounts = new ClientAccountEntity[listItems.Count()];
            for (int i = 0; i < accounts.Length; i++)
            {
                // Get the columns in the rows
                var link = listItems[i].Descendants("a").Single();
                var uri = new Uri(link.GetAttributeValue("href", null));
                var queryString = HttpUtility.ParseQueryString(uri.Query);
                var clientNo = int.Parse(queryString.Get("client_no"));

                // Create an account
                accounts[i] = new ClientAccountEntity
                {
                    ClientNumber = clientNo,
                    Name = link.GetAttributeValue("title", "").Replace("(" + clientNo + ")", "").Trim(),
                    CurrentlySelected = listItems[i].HasClass("current-tab")
                };
            }

            return accounts;
        }

        public async Task<bool> Switch(int clientNumber)
        {
            // Make request
            var response = await _requestor.GetAsync("my-accounts/linked_accounts_control?method=switch&client_no=" + clientNumber);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

            return ListAccounts(doc).FirstOrDefault(o => o.CurrentlySelected && o.ClientNumber == clientNumber) != null;
        }
        #endregion
        #region Constructor
        public LinkedAccountOperations(Requestor requestor)
        {
            _requestor = requestor;
        }
        #endregion
    }
}
