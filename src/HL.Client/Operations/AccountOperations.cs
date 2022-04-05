using HL.Client.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HL.Client.Operations
{
    /// <summary>
    /// Defines the account operations.
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
        public async Task<AccountEntity[]> ListAsync(CancellationToken cancellationToken = default)
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
            for (int i = 0; i < accounts.Length; i++)
            {
                // Get the columns in the rows
                var columns = rows[i].Descendants("td").ToArray();

                // Create an account
                accounts[i] = new AccountEntity
                {
                    Id = int.Parse(columns[0].SelectSingleNode("a").Attributes.Single(x => x.Name == "href").Value.Remove(0, Constants.BaseUrl.Length).Split('/')[3]),
                    Name = HttpUtility.HtmlDecode(columns[0].SelectSingleNode("a").InnerText.Trim('\n', '\r').Trim()),
                    StockValue = decimal.Parse(columns[1].SelectSingleNode("a").InnerText.Split('£')[1]),
                    CashValue = decimal.Parse(columns[2].InnerText.Split('£')[1]),
                    TotalValue = decimal.Parse(columns[3].SelectSingleNode("strong").InnerText.Split('£')[1]),
                    Available = decimal.Parse(columns[4].SelectSingleNode("a").InnerText.Split('£')[1])
                };
            }

            return accounts;
        }

        /// <summary>
        /// List stocks for an account.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<StockEntity>> ListStocksAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var response = await _requestor.GetAsync($"my-accounts/account_summary/account/{accountId}");

            if (!response.IsSuccessStatusCode)
            {
                // Check if 404
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ArgumentException("Unable to find account.", "accountId");

                throw new Exception($"Unable to get stock for account: {accountId}");
            }

            HtmlDocument doc = new HtmlDocument();
            string html = Regex.Replace(await response.Content.ReadAsStringAsync().ConfigureAwait(false), @"( |\t|\r?\n)\1+", "$1");
            doc.LoadHtml(html);

            List<StockEntity> stocks = new List<StockEntity>();

            var rows = doc
                .DocumentNode
                .Descendants("table")
                .Where(x => x.HasClass("holdings-table"))
                .SelectMany(table => table.SelectSingleNode("tbody").Descendants("tr"))
                .ToArray();

            // Convert into stock holding entities
            for (int i = 0; i < rows.Length; i++)
            {
                var columns = rows[i].Descendants("td").ToArray();
                var nameAndType = columns[1].InnerText.Trim('\r', '\n').Trim().Split('\n');
                stocks.Add(new StockEntity
                {
                    Id = columns[0].SelectSingleNode("a").Attributes.SingleOrDefault(x => x.Name == "href").Value.Remove(0, $"{Constants.BaseUrl}".Length).Split('/')[3],
                    Name = HttpUtility.HtmlDecode(nameAndType[0]),
                    UnitType = HttpUtility.HtmlDecode(nameAndType[nameAndType.Length-1]).Trim(),
                    UnitsHeld = decimal.Parse(columns[2].InnerText.Trim('\r', '\n').Trim()),
                    Price = decimal.Parse(columns[3].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim()),
                    Value = decimal.Parse(columns[4].SelectSingleNode("span").SelectSingleNode("span").InnerText.Trim('\r', 'n').Trim()),
                    Cost = decimal.Parse(columns[5].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim()),
                    GainsLoss = new GainsLossEntity
                    {
                        Pounds = decimal.Parse(columns[16].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim()),
                        Percentage = decimal.Parse(columns[17].SelectSingleNode("span").InnerText.Trim('\r', '\n').Trim())
                    }
                });
            }

            return stocks;
        }

        /// <summary>
        /// Gets the cash summary for an account.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <returns></returns>
        public async Task<CashSummaryEntity> GetCashSummaryAsync(int accountId)
        {
            var response = await _requestor.GetAsync($"my-accounts/cash/account/{accountId}");

            if (!response.IsSuccessStatusCode)
            {
                // Check if 404
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ArgumentException("Unable to find account.", "accountId");

                throw new Exception($"Unable to get cash summary for account: {accountId}");
            }

            HtmlDocument doc = new HtmlDocument();
            string html = Regex.Replace(await response.Content.ReadAsStringAsync().ConfigureAwait(false), @"( |\t|\r?\n)\1+", "$1");
            doc.LoadHtml(html);

            var table = doc.DocumentNode.Descendants("table").Where(x => x.HasClass("cash-generic-table")).SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();
            var footer = table.SelectSingleNode("tfoot").SelectSingleNode("tr");

            return new CashSummaryEntity
            {
                CashOnCapitalAccount = decimal.Parse(rows[0].SelectNodes("td").Last().InnerText.Trim().TrimStart('£')),
                IncomeLoyaltyBonus = decimal.Parse(rows[1].SelectNodes("td").Last().InnerText.Trim().TrimStart('£')),
                FixedRateOffers = decimal.Parse(rows[2].SelectNodes("td").Last().InnerText.Trim().TrimStart('£')),
                TotalCash = decimal.Parse(footer.SelectNodes("td").Last().InnerText.Trim().TrimStart('£')),
            };
        }

        /// <summary>
        /// List transactions for an account.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <returns></returns>
        public async Task<TransactionEntity[]> ListTransactionsAsync(int accountId, CancellationToken cancellationToken = default)
        {
            // Make request
            var response = await _requestor.GetAsync($"my-accounts/capital-transaction-history/account/{accountId}");

            if (!response.IsSuccessStatusCode)
            {
                // Check if 404
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ArgumentException("Unable to find account.", "accountId");

                throw new Exception($"Unable to get transactions for account: {accountId}");
            }

            // Convert into a HTML doc
            HtmlDocument doc = new HtmlDocument();
            string html = Regex.Replace(await response.Content.ReadAsStringAsync().ConfigureAwait(false), @"( |\t|\r?\n)\1+", "$1");
            doc.LoadHtml(html);

            // Get the table
            var table = doc.DocumentNode.Descendants("table").Where(x => x.HasClass("transaction-history-table")).SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();

            // Convert into transaction entities
            TransactionEntity[] transactions = new TransactionEntity[rows.Length];
            for (int i = 0; i < transactions.Length; i++)
            {
                // Get the columns in the rows
                var columns = rows[i].Descendants("td").ToArray();

                transactions[i] = new TransactionEntity()
                {
                    TradeDate = DateTime.Parse(columns[0].InnerText.Trim('\r', '\n')),
                    SettleDate = DateTime.Parse(columns[1].InnerText.Trim('\r', '\n')),

                    // Determine the reference
                    Reference = columns[2].ChildNodes.SingleOrDefault(c => c.Name == "a") != null ? columns[2].SelectSingleNode("a").InnerText.Trim('\r', '\n') : columns[2].InnerText.Trim('\r', '\n'),
                    ReferenceLink = columns[2].ChildNodes.SingleOrDefault(c => c.Name == "a") != null ? columns[2].SelectSingleNode("a").Attributes.SingleOrDefault(a => a.Name == "href").Value : null,

                    Description = columns[3].InnerText.Trim('\r', '\n').Trim(),
                    UnitCost = decimal.TryParse(columns[4].InnerText.Trim('\r', '\n'), out decimal unitPrice) ? unitPrice : (decimal?)null,
                    Quantity = decimal.TryParse(columns[5].InnerText.Trim('\r', '\n'), out decimal quantity) ? quantity : (decimal?)null,
                    Value = decimal.TryParse(columns[6].InnerText.Trim('\r', '\n'), out decimal value) ? value : 0
                };
            }

            return transactions;
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
