using System;
using System.Threading.Tasks;
using HL.Client;

namespace HL.Client.Example
{
    class Program
    {
        static async Task MainAsync(string[] args)
        {
            string username = Environment.GetEnvironmentVariable("HL_USERNAME");
            string password = Environment.GetEnvironmentVariable("HL_PASSWORD");
            DateTime birthday = DateTime.Parse(Environment.GetEnvironmentVariable("HL_BIRTHDAY"));
            string securityCode = Environment.GetEnvironmentVariable("HL_SECURITY_CODE");

            // Load client
            Client client = new Client();

            // Start
            await client.Authentication.StartAuthentication(username, password, birthday, securityCode);

            // Client is now authenticated.

            var accounts = await client.AccountOperations.ListAsync();

            //var stocks = await client.AccountOperations.ListStocksAsync(accounts[0].Id);

            var transactions = await client.AccountOperations.ListTransactions(accounts[0].Id);

            return;
        }

        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
