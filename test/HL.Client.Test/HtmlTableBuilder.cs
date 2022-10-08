using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Client.Test
{
    internal class HtmlTableBuilder
    {
        private readonly StringWriter _body;
        private readonly StringWriter _footer;

        public HtmlTableBuilder()
        {
            this._body = new StringWriter();
            this._footer = new StringWriter();
            this.Id = string.Empty;
            this.Class = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public HtmlTableBuilder AddRow(params string[] args)
        {
            _body.Write(@"<tr> ");

            foreach (var cell in args)
            {
                _body.Write(@"<td>{0}</td> ", cell);
            }

            _body.WriteLine(@"</tr>");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public HtmlTableBuilder AddFooter(params string[] args)
        {
            _footer.Write(@"<tr> ");

            foreach (var cell in args)
            {
                _footer.Write(@"<td>{0}</td> ", cell);
            }

            _footer.WriteLine(@"</tr>");
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sw = new StringWriter();

            sw.Write("<table");

            if (this.Id != string.Empty)
            {
                sw.Write(@" id=""{0}""", this.Id);
            }

            if (this.Class != string.Empty)
            {
                sw.Write(@" class=""{0}""", this.Class);
            }

            sw.WriteLine("> <tbody>");
            sw.WriteLine(this._body);
            sw.WriteLine("</tbody>");

            if (this._footer.ToString() != string.Empty)
            {
                sw.WriteLine("<tfoot>");
                sw.WriteLine(this._footer);
                sw.WriteLine("</tfoot>");
            }

            sw.WriteLine("</table>");

            return sw.ToString();
        }
    }
}
