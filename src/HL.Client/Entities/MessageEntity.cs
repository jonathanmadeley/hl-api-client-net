using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Client.Entities
{
    /// <summary>
    /// Defines a message.
    /// </summary>
    public class MessageEntity
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data received at.
        /// </summary>
        public DateTime ReceivedAt { get; set; }
    }

    /// <summary>
    /// Defines the possible new message types.
    /// </summary>
    public static class MessageNewTypes
    {
        /// <summary>
        /// Vantage ISA, Fund and Share Account or general enquiry
        /// </summary>
        public const string GeneralEnquiry = "A0021";

        /// <summary>
        /// Pensions and Retirement
        /// </summary>
        public const string PensionsRetirement = "A0030";

        /// <summary>
        /// Corporate Action instructions / Voting instructions / AGM requests.
        /// </summary>
        public const string CorporateActoinInstructionsOrVotingInstructionsOrAGMRequests = "OCA";
    }
}
