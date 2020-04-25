using HtmlAgilityPack;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HL.Client.Authentication
{
    /// <summary>
    /// Defines the base class for authenticating.
    /// </summary>
    public abstract class BaseStage
    {
        #region Fields
        private Requestor _requestor;
        #endregion

        #region Properties
        public string Path { get; set; }

        public string ExpectedResponse { get; set; }
        public HtmlDocument Document { get; set; }
        #endregion

        #region Methods
        public async Task LoadPage()
        {
            // Make a request to get the initial page
            var response = await _requestor.GetAsync(Path).ConfigureAwait(false);

            // Load the content
            Document = new HtmlDocument();
            Document.Load(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        /// Load the verification token for this stage.
        /// </summary>
        /// <returns></returns>
        public string GetVerificationToken()
        {
            var input = Document.DocumentNode.Descendants("input").Where(i => i.Attributes["name"]?.Value == "hl_vt").SingleOrDefault();

            if (input == null)
                throw new Exception("Unable to find verification token for Stage.");

            return input.Attributes["value"].Value;
        }

        // Build the fields to send.
        public abstract Dictionary<string, string> BuildFields();

        /// <summary>
        /// Submit this stage.
        /// </summary>
        /// <returns></returns>
        public async Task SubmitAsync()
        {
            Dictionary<string, string> fields = BuildFields();
            fields.Add("hl_vt", GetVerificationToken());

            var response = await _requestor.PostAsync(Path, new FormUrlEncodedContent(fields)).ConfigureAwait(false);

            if (response.RequestMessage.RequestUri.AbsolutePath != $"/{ExpectedResponse}")
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception("Unable to submit Stage");
            }
        }

        /// <summary>
        /// Run the stage.
        /// </summary>
        /// <returns></returns>
        public async Task RunStageAsync()
        {
            await LoadPage().ConfigureAwait(false);

            await SubmitAsync().ConfigureAwait(false);
        }
        #endregion

        #region Constructor
        public BaseStage(Requestor requestor)
        {
            _requestor = requestor;
        }
        #endregion
    }
}
