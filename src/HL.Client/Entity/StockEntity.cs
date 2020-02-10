using System;
namespace HL.Client.Entity
{
    /// <summary>
    /// Defines a stock entity.
    /// </summary>
    public class StockEntity
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of units held.
        /// </summary>
        public double UnitsHeld { get; set; }

        /// <summary>
        /// Gets or sets the price of the unit.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the value of the holding.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the cost of the holding.
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Gets or sets the gains or losses for the holding.
        /// </summary>
        public GainsLossEntity GainsLoss { get; set; }
    }
}
