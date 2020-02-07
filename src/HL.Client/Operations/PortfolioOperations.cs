using HL.Client.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Client.Operations
{
    /// <summary>
    /// Defines the possible portfolio operations.
    /// </summary>
    public class PortfolioOperations
    {
        #region Fields
        private Requestor _requestor;
        #endregion

        #region Methods
        public async Task<AccountModel[]> GetAccountsAsync()
        {
            // Make request
            var response = await _requestor.GetAsync("my-accounts/portfolio_overview");

            HtmlDocument doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync());

            // Get table
            var table = doc.DocumentNode.Descendants("table").Where(x => x.Id == "portfolio").SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();

            // For each row, convert into account model.
            AccountModel[] accounts = new AccountModel[rows.Count()];
            for(int i = 0; i < accounts.Length; i++)
            {
                // Get the columns in the rows
                var columns = rows[i].Descendants("td").ToArray();

                // Create an account
                accounts[i] = new AccountModel()
                {
                    Id = int.Parse(columns[0].SelectSingleNode("a").Attributes.Single(x => x.Name == "href").Value.Remove(0, Constants.BaseUrl.Length).Split('/')[3]),
                    Name = columns[0].SelectSingleNode("a").InnerText.Trim('\n', '\r').Trim(),
                    StockValue = double.Parse(columns[1].SelectSingleNode("a").InnerText.Split('£')[1]),
                    CashValue = double.Parse(columns[2].InnerText.Split('£')[1]),
                    TotalValue = double.Parse(columns[3].SelectSingleNode("strong").InnerText.Split('£')[1]),
                    Available = double.Parse(columns[4].SelectSingleNode("a").InnerText.Split('£')[1])
                };
            }

            return accounts;
        }
        #endregion

        #region Constructor
        public PortfolioOperations(Requestor requestor)
        {
            _requestor = requestor;
        }
        #endregion
    }
}
