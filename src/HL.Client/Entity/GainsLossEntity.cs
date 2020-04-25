using System;
namespace HL.Client.Entity
{
    /// <summary>
    /// Defines a gains or loss entity.
    /// </summary>
    public class GainsLossEntity
    {
        /// <summary>
        /// Get or sets the pound value.
        /// </summary>
        public decimal Pounds { get; set; }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public decimal Percentage { get; set; }
    }
}
