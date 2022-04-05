using System;
namespace HL.Client.Entities
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
        /// Gets or sets the unit type.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Gets or sets the number of units held.
        /// </summary>
        public decimal UnitsHeld { get; set; }

        /// <summary>
        /// Gets or sets the price of the unit.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the value of the holding.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the cost of the holding.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the gains or losses for the holding.
        /// </summary>
        public GainsLossEntity GainsLoss { get; set; }
    }
}
