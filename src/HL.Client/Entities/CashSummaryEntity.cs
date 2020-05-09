using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Client.Entities
{
    /// <summary>
    /// Defines the response for a cash summary on an account.
    /// </summary>
    public class CashSummaryEntity
    {
        /// <summary>
        /// Get the cash capital on an account.
        /// </summary>
        public decimal CashOnCapitalAccount { get; set; }

        /// <summary>
        /// Get the income and loyalty bonus on an account.
        /// </summary>
        public decimal IncomeLoyaltyBonus { get; set; }

        /// <summary>
        /// Gets the amount of fixed cash offers on an account.
        /// </summary>
        public decimal FixedRateOffers { get; set; }

        /// <summary>
        /// Gets the total cash on an account.
        /// </summary>
        public decimal TotalCash { get; set; }
    }
}
