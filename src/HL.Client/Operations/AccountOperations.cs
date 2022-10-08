using HL.Client.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly CultureInfo gbCulture = CultureInfo.GetCultureInfo("en-GB");
        private Requestor _requestor;
        #endregion

        #region Methods
        /// <summary>
        /// Gets a list of all accounts.
        /// </summary>
        /// <returns>The list of accounts</returns>
        public async Task<AccountEntity[]> ListAsync(CancellationToken cancellationToken = default)
        {
            // Make request
            var response = await _requestor.GetAsync("my-accounts/portfolio_overview");

            HtmlDocument doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

            return ListAccounts(doc);
        }

        /// <summary>
        /// Parse the document to extract the list of accounts
        /// </summary>
        /// <param name="doc">The document containing account information</param>
        /// <returns>The list of accounts</returns>
        public static AccountEntity[] ListAccounts(HtmlDocument doc)
        { 
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
                    StockValue = ParseCurrency(columns[1].SelectSingleNode("a").InnerText),
                    CashValue = ParseCurrency(columns[2].InnerText),
                    TotalValue = ParseCurrency(columns[3].SelectSingleNode("strong").InnerText),
                    Available = ParseCurrency(columns[4].SelectSingleNode("a").InnerText),
                };
            }

            return accounts;
        }

        /// <summary>
        /// List stocks for an account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <returns>The stocks within the specified account</returns>
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

            return ListStocks(doc);
        }

        /// <summary>
        /// Parse the document to extract the stock information
        /// </summary>
        /// <param name="doc">The document containing stock information</param>
        /// <returns>The stocks within the specified account</returns>
        public static List<StockEntity> ListStocks(HtmlDocument doc)
        {
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
                    UnitsHeld = ParseDecimal(columns[2].InnerText),
                    Price = ParseDecimal(columns[3].SelectSingleNode("span").InnerText),
                    Value = ParseDecimal(columns[4].SelectSingleNode("span").SelectSingleNode("span").InnerText),
                    Cost = ParseDecimal(columns[5].SelectSingleNode("span").InnerText),
                    GainsLoss = new GainsLossEntity
                    {
                        Pounds = ParseDecimal(columns[16].SelectSingleNode("span").InnerText),
                        Percentage = ParseDecimal(columns[17].SelectSingleNode("span").InnerText)
                    }
                });
            }

            return stocks;
        }

        /// <summary>
        /// Gets the cash summary for an account.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <returns>The cash summary</returns>
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

            return GetCashSummary(doc);
        }

        /// <summary>
        /// Parse the document to extract the cash summary
        /// </summary>
        /// <param name="doc">The document containing the summary information</param>
        /// <returns>The cash summary</returns>
        public static CashSummaryEntity GetCashSummary(HtmlDocument doc)
        {
            var table = doc.DocumentNode.Descendants("table").Where(x => x.HasClass("cash-generic-table")).SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();
            var footer = table.SelectSingleNode("tfoot").SelectSingleNode("tr");

            return new CashSummaryEntity
            {
                CashOnCapitalAccount = ParseCurrency(rows[0].SelectNodes("td").Last().InnerText),
                IncomeLoyaltyBonus = ParseCurrency(rows[1].SelectNodes("td").Last().InnerText),
                FixedRateOffers = ParseCurrency(rows[2].SelectNodes("td").Last().InnerText),
                TotalCash = ParseCurrency(footer.SelectNodes("td").Last().InnerText),
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

            return ListTransactions(doc);
        }

        /// <summary>
        /// Parse the document to extract the transactions
        /// </summary>
        /// <param name="doc">The document containing the transactions</param>
        /// <returns>The transactions</returns>
        public static TransactionEntity[] ListTransactions(HtmlDocument doc)
        {
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
                    TradeDate = ParseDateTime(columns[0].InnerText.Trim('\r', '\n')),
                    SettleDate = ParseDateTime(columns[1].InnerText.Trim('\r', '\n')),

                    // Determine the reference
                    Reference = columns[2].ChildNodes.SingleOrDefault(c => c.Name == "a") != null ? columns[2].SelectSingleNode("a").InnerText.Trim('\r', '\n') : columns[2].InnerText.Trim('\r', '\n'),
                    ReferenceLink = columns[2].ChildNodes.SingleOrDefault(c => c.Name == "a") != null ? columns[2].SelectSingleNode("a").Attributes.SingleOrDefault(a => a.Name == "href").Value : null,

                    Description = columns[3].InnerText.Trim('\r', '\n').Trim(),
                    UnitCost = ParseDecimalOrNull(columns[4].InnerText.Trim('\r', '\n')),
                    Quantity = ParseDecimalOrNull(columns[5].InnerText.Trim('\r', '\n')),
                    Value = ParseDecimalOrDefault(columns[6].InnerText.Trim('\r', '\n'), 1.0m)
                };
            }

            return transactions;
        }

        /// <summary>
        /// Parse the date and time using UK formatting
        /// </summary>
        /// <param name="str">String containing a date and possibly a time</param>
        /// <returns>The date and time</returns>
        private static DateTime ParseDateTime(string str)
        {
            return DateTime.Parse(str, gbCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>
        /// Parse a currency value using UK formatting
        /// </summary>
        /// <param name="str">String containing a currency value</param>
        /// <returns>The currency value</returns>
        private static decimal ParseCurrency(string str)
        {
            return decimal.Parse(str, NumberStyles.Currency, gbCulture);
        }

        /// <summary>
        /// Parse a number using UK formatting
        /// </summary>
        /// <param name="str">String containing a number</param>
        /// <returns>The number value</returns>
        private static decimal ParseDecimal(string str)
        {
            return decimal.Parse(str, NumberStyles.Number, gbCulture);
        }

        /// <summary>
        /// Try to parse a number using UK formatting and return null if the parsing fails
        /// </summary>
        /// <param name="str">String containing a number</param>
        /// <returns>The number value or null</returns>
        private static decimal? ParseDecimalOrNull(string str)
        {
            decimal value;

            if (decimal.TryParse(str, NumberStyles.Number, gbCulture, out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Try to parse a number using UK formatting and return a default value if the parsing fails
        /// </summary>
        /// <param name="str">String containing a number</param>
        /// <param name="defaultValue">The value if parsing fails</param>
        /// <returns>The number value</returns>
        private static decimal ParseDecimalOrDefault(string str, decimal defaultValue)
        {
            decimal value;

            if(decimal.TryParse(str, NumberStyles.Number, gbCulture, out value))
            {
                return value;
            }

            return defaultValue;
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
