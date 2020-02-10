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
        public double Pounds { get; set; }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public double Percentage { get; set; }
    }
}
