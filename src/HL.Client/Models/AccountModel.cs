using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Client.Models
{
    /// <summary>
    /// Defines an account model.
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the stock value.
        /// </summary>
        public double StockValue { get; set; }

        /// <summary>
        /// Gets or sets the cash value.
        /// </summary>
        public double CashValue { get; set; }

        /// <summary>
        /// Gets or sets the total value.
        /// </summary>
        public double TotalValue { get; set; }

        /// <summary>
        /// Gets or sets the available balance.
        /// </summary>
        public double Available { get; set; }
    }
}
