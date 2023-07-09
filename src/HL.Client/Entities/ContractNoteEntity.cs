using System;

namespace HL.Client.Entities
{
    /// <summary>
    /// Defines the type of transaction
    /// </summary>
    public enum TransactionType
    {
        Unknown,
        Buy,
        Sell
    }

    /// <summary>
    /// Defines the content of a contract note.
    /// </summary>
    public class ContractNoteEntity
    {
        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the contract note id.
        /// </summary>
        public string ContractNoteId { get; set; }

        /// <summary>
        /// Gets or sets the product ISIN code.
        /// </summary>
        public string Isin { get; set; }

        /// <summary>
        /// Gets or sets the order time.
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the unit type.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price in it's local currency.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the currency the unit price is in.
        /// </summary>
        public string UnitCurrency { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate used for the transaction.
        /// </summary>
        public double ExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets the unit price when converted to GBP.
        /// </summary>
        public double UnitPriceGbp { get; set; }

        /// <summary>
        /// Gets or sets the commission charged for the transaction.
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        /// Gets or sets the foreign currency exchange fees charged for the transaction.
        /// </summary>
        public double FxCharge { get; set; }

        /// <summary>
        /// Gets or sets the transfer fee.
        /// </summary>
        public double TransferFee { get; set; }

        /// <summary>
        /// Gets or sets the total amount transacted in GBP exclusive of fees.
        /// </summary>
        public double TotalAmountGbpExcludingFees { get; set; }

        /// <summary>
        /// Gets or sets the total amount transacted in GBP inclusive of fees.
        /// </summary>
        public double TotalAmountGbpIncludingFees { get; set; }

        /// <summary>
        /// Gets or sets the venue where the order has been executed.
        /// </summary>
        public string Venue { get; set; }

        /// <summary>
        /// Gets or sets the order type.
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol (if available).
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the account name that conducted the transaction.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the settlement date for the transaction.
        /// </summary>
        public DateTime SettlementDate { get; set; }

        /// <summary>
        /// Gets or sets the notes of the transaction.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the name of the person transacting.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the address of the person transacting.
        /// </summary>
        public string[] ClientAddress { get; set; }
    }
}
