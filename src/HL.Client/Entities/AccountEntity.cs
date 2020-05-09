using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HL.Client.Entities
{
    /// <summary>
    /// Defines an account entity.
    /// </summary>
    public class AccountEntity
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
        public decimal StockValue { get; set; }

        /// <summary>
        /// Gets or sets the cash value.
        /// </summary>
        public decimal CashValue { get; set; }

        /// <summary>
        /// Gets or sets the total value.
        /// </summary>
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Gets or sets the available balance.
        /// </summary>
        public decimal Available { get; set; }
    }
}
