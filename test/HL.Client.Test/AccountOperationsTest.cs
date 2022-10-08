using HL.Client.Operations;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL.Client.Test
{
    [TestClass]
    public class AccountOperationsTest
    {
        private const string HtmlBegin = "<html> <head></head> <body>";
        private const string HtmlEnd = "</body> </html>";

        [TestMethod]
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

            Assert.AreEqual(1, accounts.Length);
            Assert.AreEqual(999, accounts[0].Id);
            Assert.AreEqual("0", accounts[0].Name);
            Assert.AreEqual(1.0m, accounts[0].StockValue);
            Assert.AreEqual(2.0m, accounts[0].CashValue);
            Assert.AreEqual(3.0m, accounts[0].TotalValue);
            Assert.AreEqual(4.0m, accounts[0].Available);
        }

        [TestMethod]
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

            Assert.AreEqual(1, stocks.Count);
            Assert.AreEqual("999", stocks[0].Id);
            Assert.AreEqual("stock", stocks[0].Name);
            Assert.AreEqual("type", stocks[0].UnitType);
            Assert.AreEqual(2.0m, stocks[0].UnitsHeld);
            Assert.AreEqual(3.0m, stocks[0].Price);
            Assert.AreEqual(4.0m, stocks[0].Value);
            Assert.AreEqual(5.0m, stocks[0].Cost);
            Assert.AreEqual(16.0m, stocks[0].GainsLoss.Pounds);
            Assert.AreEqual(17.0m, stocks[0].GainsLoss.Percentage);
        }

        [TestMethod]
        public void ParseCashSummary()
        {
            var doc = new HtmlDocument();
            var tableBuilder = new HtmlTableBuilder { Class = "cash-generic-table" };

            tableBuilder.AddRow("£1.00");
            tableBuilder.AddRow("£2.00");
            tableBuilder.AddRow("£3.00");
            tableBuilder.AddFooter("£4.00");

            doc.Load(new StringReader(HtmlBegin + tableBuilder + HtmlEnd));

            var summary = AccountOperations.GetCashSummary(doc);

            Assert.AreEqual(1.0m, summary.CashOnCapitalAccount);
            Assert.AreEqual(2.0m, summary.IncomeLoyaltyBonus);
            Assert.AreEqual(3.0m, summary.FixedRateOffers);
            Assert.AreEqual(4.0m, summary.TotalCash);
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
