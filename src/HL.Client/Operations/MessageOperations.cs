using HL.Client.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HL.Client.Operations
{
    /// <summary>
    /// Defines the message operations.
    /// </summary>
    public class MessageOperations
    {
        #region Fields
        private Requestor _requestor;
        #endregion

        #region Methods
        /// <summary>
        /// Lists the messages in your inbox.
        /// </summary>
        /// <param name="year">The eyar to search for message. Optional.</param>
        /// <param name="month">The month to search for message. Optiona.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageEntity[]> ListInboxAsync(int? year = null, int? month = null, CancellationToken cancellationToken = default)
        {
            if (year == null && month != null)
                throw new ArgumentException("You can only select a month with a valid year.", "month");

            var response = await _requestor.GetAsync($"secure_messaging/inbox?year={year ?? -1}&month={month ?? -1}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get inbox.");
            }

            HtmlDocument doc = new HtmlDocument();
            string html = Regex.Replace(await response.Content.ReadAsStringAsync().ConfigureAwait(false), @"( |\t|\r?\n)\1+", "$1");
            doc.LoadHtml(html);

            var table = doc.DocumentNode.Descendants("table").Where(x => x.HasClass("inbox-table")).SingleOrDefault();
            var body = table.SelectSingleNode("tbody");
            var rows = body.Descendants("tr").ToArray();

            // Convert into message entities
            MessageEntity[] messages = new MessageEntity[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                var columns = rows[i].Descendants("td").ToArray();

                messages[i] = new MessageEntity
                {
                    Id = int.Parse(columns[0].SelectSingleNode("a")
.Attributes.Single(x => x.Name == "href").Value.Split('/').Last()),
                    Title = columns[1].SelectSingleNode("a").InnerText.Trim(),
                    ReceivedAt = DateTime.ParseExact(columns[2].InnerText.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                };
            }

            return messages;
        }

        /// <summary>
        /// Gets a specific message.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MessageEntity> GetAsync(int id)
        {
            var response = await _requestor.GetAsync($"secure_messaging/view/message/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get message.");
            }

            HtmlDocument doc = new HtmlDocument();
            string html = Regex.Replace(await response.Content.ReadAsStringAsync().ConfigureAwait(false), @"( |\t|\r?\n)\1+", "$1");
            doc.LoadHtml(html);

            HtmlNode container = doc.DocumentNode.Descendants("div").Where(x => x.Id == "smcWindow").SingleOrDefault();
            HtmlNode header = container.SelectSingleNode("h2");
            HtmlNode[] messages = container.SelectNodes("p").ToArray();

            return new MessageEntity
            {
                Id = id,
                Title = header.SelectNodes("span").Last().InnerHtml,
                ReceivedAt = DateTime.ParseExact(header.SelectNodes("span").First().InnerText, "dd MMM yyyy", CultureInfo.InvariantCulture),
                Message = string.Join(",", messages.Select(x => $"{x.InnerText.Trim()}\n").ToArray())
            };
        }
        #endregion

        #region Constructor
        public MessageOperations(Requestor requestor)
        {
            _requestor = requestor;
        }
        #endregion
    }
}
