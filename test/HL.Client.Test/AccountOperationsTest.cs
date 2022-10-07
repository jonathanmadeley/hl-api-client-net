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
    }
}
