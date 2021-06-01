using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HL.Client;
using HL.Client.Entities;

namespace HL.Client.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string username = Environment.GetEnvironmentVariable("HL_USERNAME");
            string password = Environment.GetEnvironmentVariable("HL_PASSWORD");
            DateTime birthday = DateTime.Parse(Environment.GetEnvironmentVariable("HL_BIRTHDAY"));
            string securityCode = Environment.GetEnvironmentVariable("HL_SECURITY_CODE");

            // Load client
            Client client = new Client();

            // Authenticate client
            await client.AuthenticateAsync(username, password, birthday, securityCode);

            AccountEntity[] accounts = await client.AccountOperations.ListAsync();

            foreach (AccountEntity account in accounts)
            {
                // Account information
                Console.WriteLine($"- Account: {account.Name}");
                Console.WriteLine($"        Current Value: {account.TotalValue}");
                Console.WriteLine($"        Total Share Value: {account.StockValue}");
                Console.WriteLine($"        Cash Held on Account: {account.CashValue}");

                // Get information for each stock.
                Console.WriteLine("  Stocks & Funds");
                List<StockEntity> stocks = await client.AccountOperations.ListStocksAsync(account.Id);
                foreach(StockEntity stock in stocks)
                {
                    Console.WriteLine($"    - Stock Holding: {stock.Name}");
                    Console.WriteLine($"        Current Value: {stock.Value}");
                    Console.WriteLine($"        Bought at: {stock.Cost}");

                    if (stock.GainsLoss.Percentage > 0)
                        Console.WriteLine($"        You have made a PROFIT of {stock.GainsLoss.Pounds} to date.");
                    else if (stock.GainsLoss.Percentage == 0)
                        Console.WriteLine($"        You have not made a profit or loss to date.");
                    else
                        Console.WriteLine($"        You have made a LOSS of {stock.GainsLoss.Pounds} to date.");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();

            return;
        }
    }
}
