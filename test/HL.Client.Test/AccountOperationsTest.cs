using HL.Client.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL.Client.Test
{
    [TestClass]
    public class AccountOperationsTest
    {
        [TestMethod]
        public void ConstructObject()
        {
            var _requestor = new Requestor();
            var AccountOperations = new AccountOperations(_requestor);
        }
    }
}
