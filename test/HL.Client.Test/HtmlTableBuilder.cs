using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Client.Test
{
    internal class HtmlTableBuilder
    {
        private readonly StringWriter _writer;

        public HtmlTableBuilder()
        {
            this._writer = new StringWriter();
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
            _writer.Write(@"<tr> ");

            foreach(var cell in args)
            {
                _writer.Write(@"<td>{0}</td> ", cell);
            }

            _writer.WriteLine(@"</tr>");
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
            sw.WriteLine(this._writer);
            sw.WriteLine("</tbody> </table>");

            return sw.ToString();
        }
    }
}
