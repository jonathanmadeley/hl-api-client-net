namespace HL.Client.Entities
{
    /// <summary>
    /// Defines a client account.
    /// </summary>
    public class ClientAccountEntity
    {
        /// <summary>
        /// Gets or sets the client number.
        /// </summary>
        public int ClientNumber { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether it's the currently selected account.
        /// </summary>
        public bool CurrentlySelected { get; set; }
    }
}
