using HL.Client.Entity;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HL.Client.Operations
{
    /// <summary>
    /// Defines the possible account operations.
    /// </summary>
    public class AccountOperations
    {
        #region Fields
        private Requestor _requestor;
        #endregion

        #region Methods
        /// <summary>
        /// Gets a list of all accounts.
        /// </summary>
        /// <returns></returns>
        public async Task<AccountEntity[]> ListAsync()
        {
            // Make request
            var response = await _requestor.GetAsync("my-accounts/portfolio_overview");

            HtmlDocument doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

            // Get table
            var table = doc.DocumentNode.Descendants("table").Where(x => x.Id == "portfolio").SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();

            // For each row, convert into account model.
            AccountEntity[] accounts = new AccountEntity[rows.Count()];
            for(int i = 0; i < accounts.Length; i++)
            {
                // Get the columns in the rows
                var columns = rows[i].Descendants("td").ToArray();

                // Create an account
                accounts[i] = new AccountEntity
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


        /// <summary>
        /// Gets a list of all stocks on an account.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<StockEntity[]> ListStocksAsync(int accountId)
        {
            // Make request
            var response = await _requestor.GetAsync($"my-accounts/account_summary/account/{accountId}");

            if (!response.IsSuccessStatusCode)
            {
                // Check if 404
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ArgumentException("Unable to find account.", "accountId");

                throw new Exception($"Unable to get stock for account: {accountId}");
            }

            // Convert into a HTML doc
            HtmlDocument doc = new HtmlDocument();
            string html = Regex.Replace(await response.Content.ReadAsStringAsync().ConfigureAwait(false), @"( |\t|\r?\n)\1+", "$1");
            doc.LoadHtml(html);

            // Get the table
            var table = doc.DocumentNode.Descendants("table").Where(x => x.Id == "holdings-table").SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();

            // Convert into stock holding models
            StockEntity[] stocks = new StockEntity[rows.Length];
            for(int i = 0; i < stocks.Length; i++)
            {
                // Get the columns in the rows
                var columns = rows[i].Descendants("td").ToArray();

                stocks[i] = new StockEntity
                {
                    Id = columns[0].SelectSingleNode("a").Attributes.SingleOrDefault(x => x.Name == "href").Value.Remove(0, $"{Constants.BaseUrl}".Length).Split('/')[3],
                    Name = columns[0].SelectSingleNode("a").InnerText.Trim('\r', '\n').Trim(),
                    UnitsHeld = double.Parse(columns[2].InnerText.Trim('\r', '\n').Trim()),
                    Price = double.Parse(columns[3].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim()),
                    Value = double.Parse(columns[4].SelectSingleNode("span").SelectSingleNode("span").InnerText.Trim('\r', 'n').Trim()),
                    Cost = double.Parse(columns[5].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim()),
                    GainsLoss = new GainsLossEntity
                    {
                        Pounds = double.Parse(columns[16].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim()),
                        Percentage = double.Parse(columns[17].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim())
                    }
                };
            }

            return stocks;
        }
        #endregion

        #region Constructor
        public AccountOperations(Requestor requestor)
        {
            _requestor = requestor;
        }
        #endregion
    }
}
