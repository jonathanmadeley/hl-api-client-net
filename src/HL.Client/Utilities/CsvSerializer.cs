using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HL.Client.Utilities
{
    public static class CsvSerializer
    {
        /// <summary>
        /// Serialize objects to Comma Separated Value (CSV) format [1].
        /// 
        /// Rather than try to serialize arbitrarily complex types with this
        /// function, it is better, given type A, to specify a new type, A'.
        /// Have the constructor of A' accept an object of type A, then assign
        /// the relevant values to appropriately named fields or properties on
        /// the A' object.
        /// 
        /// [1] http://tools.ietf.org/html/rfc4180
        /// </summary>
        public static void Serialize<T>(TextWriter output, IEnumerable<T> objects)
        {
            var fields = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(mi => new[] { MemberTypes.Field, MemberTypes.Property }.Contains(mi.MemberType))
                .ToArray();
            output.WriteLine(QuoteRecord(fields.Select(f => f.Name)));
            foreach (var record in objects)
            {
                output.WriteLine(QuoteRecord(FormatObject(fields, record)));
            }
        }

        private static IEnumerable<string> FormatObject<T>(IEnumerable<MemberInfo> fields, T record)
        {
            foreach (var field in fields)
            {
                if (field is FieldInfo fi)
                {
                    yield return Convert.ToString(fi.GetValue(record));
                }
                else if (field is PropertyInfo pi)
                {
                    yield return Convert.ToString(pi.GetValue(record, null));
                }
                else
                {
                    throw new Exception("Unhandled case.");
                }
            }
        }

        private const string CsvSeparator = ",";

        static string QuoteRecord(IEnumerable<string> record)
        {
            return string.Join(CsvSeparator, record.Select(QuoteField).ToArray());
        }

        static string QuoteField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "\"\"";
            }

            if (field.Contains(CsvSeparator) || field.Contains("\"") || field.Contains("\r") ||
                field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
