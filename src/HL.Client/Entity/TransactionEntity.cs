using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Client.Entity
{
    /// <summary>
    /// Defines a capital transaction entity.
    /// </summary>
    public class TransactionEntity
    {
        /// <summary>
        /// Gets or sets the trade date.
        /// </summary>
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// Gets or sets the settle date.
        /// </summary>
        public DateTime SettleDate { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the reference link.
        /// </summary>
        public string ReferenceLink { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unit cost.
        /// </summary>
        public double? UnitCost { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public double? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get; set; }
    }
}
