using HL.Client.Operations;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL.Client.Test
{
    [TestClass]
    public class AccountOperationsTest
    {
        [TestMethod]
        public void ParseOneAccount()
        {
            var doc = new HtmlDocument();
            var s = @"
                <html>
                <head>
                </head>
                <body>
                <table id=""portfolio"">
                <tbody><tr>
                    <td><a href=""https://online.hl.co.uk/my-accounts/account_summary/account/999"">0</a></td> 
                    <td><a href=""https://online.hl.co.uk/my-accounts/account_summary/account/999"">£1.00</a></td> 
                    <td>£2.00</td> 
                    <td><strong>£3.00</strong></td> 
                    <td><a href=""https://online.hl.co.uk/my-accounts/account_summary/account/999"">£4.00</a></td>
                </tr></tbody>
                </table>
                </body>
                </html>";

            doc.Load(new StringReader(s));

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
            var s = @"
                <html>
                <head>
                </head>
                <body>
                <table class=""holdings-table"">
                <tbody><tr>
                    <td><a href=""https://online.hl.co.uk/my-accounts/account_summary/account/999"">0</a></td> 
                    <td><a href=""https://online.hl.co.uk/my-accounts/account_summary/account/999"">stock" + '\n' + @"type</a></td> 
                    <td>2</td> 
                    <td><span>3.00</span></td> 
                    <td><span><span>4.00</span></span></td>
                    <td><span>5.00</span></td>

                    <td></td> <td></td>
                    <td></td> <td></td>
                    <td></td> <td></td>
                    <td></td> <td></td>
                    <td></td> <td></td>

                    <td><span>16.00</span></td>
                    <td><span>17.00</span></td>
                </tr></tbody>
                </table>
                </body>
                </html>";

            doc.Load(new StringReader(s));

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
    }
}
