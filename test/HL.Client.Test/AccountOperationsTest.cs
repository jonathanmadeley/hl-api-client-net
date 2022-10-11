using System.Globalization;
using HL.Client.Entities;
using HL.Client.Operations;
using HtmlAgilityPack;
using NUnit.Framework;

namespace HL.Client.Test
{
    /// <summary>
    /// Ensure the parsing of account operations is correct
    /// </summary>
    [TestFixtureSource(nameof(Cultures))]
    public class AccountOperationsTest
    {
        private const string HtmlBegin = "<html> <head></head> <body>";
        private const string HtmlEnd = "</body> </html>";

        private static readonly object[] Cultures =
        {
            CultureInfo.GetCultureInfo("en-GB"), // This the expected culture
            CultureInfo.GetCultureInfo("en-US"), // Very similar except the date format is mm/dd/yyyy
            CultureInfo.GetCultureInfo("nl-NL"), // Similar except numbers are 000.000,00
            CultureInfo.GetCultureInfo("zh-Hant-TW"), // Uses a different calendar
        };

        private readonly CultureInfo culture;

        public AccountOperationsTest(CultureInfo culture)
        {
            this.culture = culture;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            CultureInfo.CurrentCulture = culture;
        }

        [Test]
        public void ParseOneAccount()
        {
            var doc = new HtmlDocument();
            var tableBuilder = new HtmlTableBuilder { Id = "portfolio" };

            tableBuilder.AddRow(
                AccountLink(999, "0"),
                AccountLink(999, "£1.00"),
                "£2.00",
                "<strong>£3.00</strong>",
                AccountLink(999, "£4.00"));

            doc.Load(new StringReader(HtmlBegin + tableBuilder + HtmlEnd));

            var accounts = AccountOperations.ListAccounts(doc);

            var expected = new AccountEntity
            {
                Id = 999,
                Name = "0",
                StockValue = 1.0m,
                CashValue = 2.0m,
                TotalValue = 3.0m,
                Available = 4.0m
            };

            Assert.That(1, Is.EqualTo(accounts.Length), nameof(accounts.Length));
            EntitiesAssert.AreEqual(expected, accounts[0]);
        }

        [Test]
        public void ParseOneStock()
        {
            var doc = new HtmlDocument();
            var tableBuilder = new HtmlTableBuilder { Class = "holdings-table" };

            tableBuilder.AddRow(
                AccountLink(999, "0"),
                AccountLink(999, "stock" + '\n' + "type"),
                "2",
                Span("3.00"),
                Span(Span("4.00")),
                Span("5.00"),
                "", "", "", "", "",
                "", "", "", "", "",
                Span("16.00"),
                Span("17.00"));

            doc.Load(new StringReader(HtmlBegin + tableBuilder + HtmlEnd));

            var stocks = AccountOperations.ListStocks(doc);

            var expected = new StockEntity
            {
                Id = "999",
                Name = "stock",
                UnitType = "type",
                UnitsHeld = 2.0m,
                Price = 3.0m,
                Value = 4.0m,
                Cost = 5.0m,
                GainsLoss = new GainsLossEntity
                {
                    Pounds = 16.0m,
                    Percentage = 17.0m
                }
            };

            Assert.That(1, Is.EqualTo(stocks.Count), nameof(stocks.Count));
            EntitiesAssert.AreEqual(expected, stocks[0]);
        }

        [Test]
        public void ParseCashSummary()
        {
            var doc = new HtmlDocument();
            var tableBuilder = new HtmlTableBuilder { Class = "cash-generic-table" };

            tableBuilder.AddRow("£1.00");
            tableBuilder.AddRow("£2.00");
            tableBuilder.AddRow("£3.00");
            tableBuilder.AddFooter("£4.00");

            doc.Load(new StringReader(HtmlBegin + tableBuilder + HtmlEnd));

            var actual = AccountOperations.GetCashSummary(doc);

            var expected = new CashSummaryEntity
            {
                CashOnCapitalAccount = 1.0m,
                IncomeLoyaltyBonus = 2.0m,
                FixedRateOffers = 3.0m,
                TotalCash = 4.0m
            };

            EntitiesAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseEmptyTransaction()
        {
            var doc = new HtmlDocument();
            var tableBuilder = new HtmlTableBuilder { Class = "transaction-history-table" };

            tableBuilder.AddRow(
                "15/10/2020",
                "20/10/2010",
                "<a href=\"https://example.com/random-url/\">a_name</a>",
                "This is a description",
                "",
                "",
                "");

            doc.Load(new StringReader(HtmlBegin + tableBuilder + HtmlEnd));

            var transactions = AccountOperations.ListTransactions(doc);

            TransactionEntity expected = new TransactionEntity
            {
                TradeDate = new DateTime(2020, 10, 15),
                SettleDate = new DateTime(2010, 10, 20),
                Reference = "a_name",
                ReferenceLink = "https://example.com/random-url/",
                Description = "This is a description",
                UnitCost = null,
                Quantity = null,
                Value = 0.0m
            };

            Assert.That(1, Is.EqualTo(transactions.Length), nameof(transactions.Length));
            EntitiesAssert.AreEqual(expected, transactions[0]);
        }

        [Test]
        public void ParseOneTransaction()
        {
            var doc = new HtmlDocument();
            var tableBuilder = new HtmlTableBuilder { Class = "transaction-history-table" };

            tableBuilder.AddRow(
                "15/10/2020",
                "20/10/2010",
                "<a href=\"https://example.com/random-url/\">a_name</a>",
                "This is a description",
                "5.00",
                "6.00",
                "7.00");

            doc.Load(new StringReader(HtmlBegin + tableBuilder + HtmlEnd));

            var transactions = AccountOperations.ListTransactions(doc);

            TransactionEntity expected = new TransactionEntity
            {
                TradeDate = new DateTime(2020, 10, 15),
                SettleDate = new DateTime(2010, 10, 20),
                Reference = "a_name",
                ReferenceLink = "https://example.com/random-url/",
                Description = "This is a description",
                UnitCost = 5.0m,
                Quantity = 6.0m,
                Value = 7.0m
            };

            Assert.That(1, Is.EqualTo(transactions.Length), nameof(transactions.Length));
            EntitiesAssert.AreEqual(expected, transactions[0]);
        }

        /// <summary>
        /// Create a link to a specific account
        /// </summary>
        /// <param name="id">The account ID</param>
        /// <param name="name">The account name</param>
        /// <returns>The link</returns>
        private static string AccountLink(int id, string name)
        {
            return string.Format(
                "<a href=\"{0}my-accounts/account-summary/account/{1}\">{2}</a>",
                Constants.BaseUrl,
                id,
                name);
        }

        /// <summary>
        /// Wrap a string in a span tag
        /// </summary>
        /// <param name="str">The string wrap</param>
        /// <returns>The span</returns>
        private static string Span(string str)
        {
            return "<span>" + str + "</span>";
        }
    }
}
