using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using HL.Client.Entities;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

namespace HL.Client.Utilities
{
    /// <summary>
    /// Defines the element type found in the PDF.
    /// </summary>
    internal enum ElementType
    {
        Unknown,
        ClientName,
        AddressLine1,
        AddressLine2,
        AddressLine3,
        AddressLine4,
        AddressLine5,
        AddressLine6,
        AccountName,
        SettlementDate,
        ContractNoteId,
        OrderDate,
        OrderTime,
        PriceDetailType1,
        PriceDetailValue1,
        PriceDetailType2,
        PriceDetailValue2,
        PriceDetailType3,
        PriceDetailValue3,
        FeeType1,
        FeeValue1,
        FeeType2,
        FeeValue2,
        FeeType3,
        FeeValue3,
        FeeType4,
        FeeValue4,
        OrderDetailNote1,
        OrderDetailNote2,
        OrderDetailNote3,
        OrderDetailNote4,
        Isin,
        UnitName,
        UnitType,
        UnitPrice,
        NoteLine1,
        NoteLine2,
        StockCode,
        TransactionType,
        Quantity,
        TotalAmountIncludingFees,
        TotalAmountExcludingFees,
    }

    /// <summary>
    /// A helper class for parsing contract note PDF files.
    /// </summary>
    public class ContractNoteParser
    {
        private static readonly Dictionary<(double, double), ElementType> CoordinatesToElementTypes =
            new Dictionary<(double, double), ElementType>
            {
                { (62.36, 687.4), ElementType.ClientName },
                { (62.36, 677.48), ElementType.AddressLine1 },
                { (62.36, 667.56), ElementType.AddressLine2 },
                { (62.36, 657.64), ElementType.AddressLine3 },
                { (62.36, 647.72), ElementType.AddressLine4 },
                { (62.36, 637.80), ElementType.AddressLine5 },
                { (62.36, 627.88), ElementType.AddressLine6 },
                { (34.02, 158.03), ElementType.AccountName },
                { (374.17, 158.03), ElementType.SettlementDate },
                { (433.7, 582.52), ElementType.ContractNoteId },
                { (56.69, 582.52), ElementType.OrderDate },
                { (249.45, 582.52), ElementType.OrderTime },
                { (493.94, 290.55), ElementType.FeeValue1 },
                { (493.94, 280.63), ElementType.FeeValue2 },
                { (493.94, 270.71), ElementType.FeeValue3 },
                { (493.94, 260.79), ElementType.FeeValue4 },
                { (119.06, 290.55), ElementType.FeeType1 },
                { (119.06, 280.63), ElementType.FeeType2 },
                { (119.06, 270.71), ElementType.FeeType3 },
                { (119.06, 260.79), ElementType.FeeType4 },
                { (119.06, 323.15), ElementType.OrderDetailNote1 },
                { (119.06, 333.07), ElementType.OrderDetailNote2 },
                { (119.06, 342.99), ElementType.OrderDetailNote3 },
                { (119.06, 352.91), ElementType.OrderDetailNote4 },
                { (119.06, 454.96), ElementType.Isin },
                { (119.06, 445.04), ElementType.UnitName },
                { (119.06, 435.12), ElementType.UnitType },
                { (119.06, 398.27), ElementType.PriceDetailType1 },
                { (119.06, 388.35), ElementType.PriceDetailType2 },
                { (119.06, 378.43), ElementType.PriceDetailType3 },
                { (388.35, 398.27), ElementType.PriceDetailValue1 },
                { (388.35, 388.35), ElementType.PriceDetailValue2 },
                { (388.35, 378.43), ElementType.PriceDetailValue3 },
                { (31.18, 123.44), ElementType.NoteLine1 },
                { (31.18, 109.27), ElementType.NoteLine2 },
            };

        private static readonly HashSet<(double, double)> Ignored = new HashSet<(double, double)>
        {
            (249.45, 593.86),
            (347.81, 435.83),
        };

        private static ElementType GetElementTypeFromCoordinates(double x, double y)
        {
            if (y == 454.82)
            {
                return ElementType.StockCode;
            }

            if (x > 250 && x < 280 && y == 538.58)
            {
                return ElementType.TransactionType;
            }

            if (x < 100 && y == 435.83)
            {
                return ElementType.Quantity;
            }

            if (x > 400 && y == 158.03)
            {
                return ElementType.TotalAmountIncludingFees;
            }

            if (x > 450 && x < 500 && (y == 371.34 || y == 435.83))
            {
                return ElementType.TotalAmountExcludingFees;
            }

            if (x > 360 && x < 390 && y == 435.83)
            {
                return ElementType.UnitPrice;
            }

            if (CoordinatesToElementTypes.TryGetValue((x, y), out ElementType elementType))
            {
                return elementType;
            }

            return ElementType.Unknown;
        }

        private static Dictionary<ElementType, string> GetTokensForElements(byte[] data)
        {
            Dictionary<ElementType, string> output = new Dictionary<ElementType, string>();
            using (var stream = new MemoryStream(data))
            {
                var doc = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly);
                foreach (var page in doc.Pages)
                {
                    foreach (PdfContent pageContent in page.Contents)
                    {
                        pageContent.Compressed = false;
                        byte[] dataBytes = pageContent.Stream.Value;
                        var parser = new CParser(dataBytes);
                        double x = double.NaN;
                        double y = double.NaN;
                        string lineContent = null;
                        foreach (var t in parser.ReadContent())
                        {
                            if (t is COperator op)
                            {
                                if (op.OpCode.OpCodeName == OpCodeName.q)
                                {
                                    x = double.NaN;
                                    y = double.NaN;
                                    lineContent = null;
                                }

                                if (op.OpCode.OpCodeName == OpCodeName.Td && op.Operands.Count == 2 &&
                                    op.Operands[0] is CReal cx && op.Operands[1] is CReal cy)
                                {
                                    x = cx.Value;
                                    y = cy.Value;
                                }

                                if (op.OpCode.OpCodeName == OpCodeName.Tj && op.Operands[0] is CString str)
                                {
                                    lineContent = str.Value;
                                }

                                if (op.OpCode.OpCodeName == OpCodeName.Q && !string.IsNullOrWhiteSpace(lineContent) && !double.IsNaN(x) &&
                                    !double.IsNaN(y))
                                {
                                    if (Ignored.Contains((x, y)))
                                    {
                                        continue;
                                    }

                                    ElementType elementType = GetElementTypeFromCoordinates(x, y);
                                    if (elementType == ElementType.Unknown)
                                    {
                                        throw new Exception(
                                            $"Failed to identify PDF item at coordinates ({x}, {y}) = {lineContent}"
                                        );
                                    }

                                    output[elementType] = lineContent;
                                }
                            }
                        }
                    }
                }
            }

            return output;
        }

        public static ContractNoteEntity ParseContractNote(string filePath)
        {
            return ParseContractNote(File.ReadAllBytes(filePath));
        }

        public static ContractNoteEntity ParseContractNote(byte[] data)
        {
            Dictionary<ElementType, string> tokens = GetTokensForElements(data);
            var note = new ContractNoteEntity();

            if (tokens.TryGetValue(ElementType.ClientName, out string clientName))
            {
                note.ClientName = clientName;
            }

            List<string> address = new List<string>();
            for (int i = 1; i <= 6; i++)
            {
                if (!Enum.TryParse($"AddressLine{i}", out ElementType elementType))
                {
                    throw new ApplicationException($"Could not find elemet type: AddressLine{i}");
                }
                if (tokens.TryGetValue(elementType, out string addressLine) &&
                    !string.IsNullOrWhiteSpace(addressLine))
                {
                    address.Add(addressLine);
                }
            }

            note.ClientAddress = address.ToArray();

            if (tokens.TryGetValue(ElementType.AccountName, out string accountName))
            {
                note.Account = accountName;
            }

            if (tokens.TryGetValue(ElementType.SettlementDate, out string settlementDate))
            {
                note.SettlementDate = DateTime.ParseExact(settlementDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            if (tokens.TryGetValue(ElementType.TransactionType, out string transactionType))
            {
                if (transactionType.Contains("BOUGHT"))
                {
                    note.TransactionType = TransactionType.Buy;
                }
                else if (transactionType.Contains("SOLD"))
                {
                    note.TransactionType = TransactionType.Sell;
                }
                else
                {
                    throw new Exception($"Unknown transaction type: {transactionType}");
                }
            }

            if (tokens.TryGetValue(ElementType.ContractNoteId, out string contractNoteId))
            {
                note.ContractNoteId = contractNoteId;
            }

            if (tokens.TryGetValue(ElementType.OrderDate, out string orderDate))
            {
                var orderDateParsed = DateTime.ParseExact(orderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (tokens.TryGetValue(ElementType.OrderTime, out string orderTime))
                {
                    int[] parts = orderTime.Split(':').Select(int.Parse).ToArray();
                    orderDateParsed = orderDateParsed.AddHours(parts[0]).AddMinutes(parts[1]);
                }

                note.OrderTime = orderDateParsed;
            }

            if (tokens.TryGetValue(ElementType.UnitName, out string unitName))
            {
                note.UnitName = unitName;
            }

            if (tokens.TryGetValue(ElementType.UnitType, out string unitType))
            {
                note.UnitType = unitType;
            }

            if (tokens.TryGetValue(ElementType.Quantity, out string quantity))
            {
                note.Quantity = decimal.Parse(quantity);
            }

            if (tokens.TryGetValue(ElementType.Isin, out string token))
            {
                note.Isin = token;
            }

            if (tokens.TryGetValue(ElementType.NoteLine1, out string noteLine1))
            {
                note.Note = noteLine1;
            }

            if (tokens.TryGetValue(ElementType.NoteLine2, out string noteLine2))
            {
                if (!string.IsNullOrWhiteSpace(note.Note))
                {
                    note.Note += " " + noteLine2;
                }
                else
                {
                    note.Note = noteLine2;
                }
            }

            for (int i = 1; i <= 3; i++)
            {
                if (!Enum.TryParse($"PriceDetailType{i}", out ElementType priceDetailTypeElementType))
                {
                    throw new ApplicationException($"Could not find elemet type: PriceDetailType{i}");
                }

                if (!Enum.TryParse($"PriceDetailValue{i}", out ElementType priceDetailValueElementType))
                {
                    throw new ApplicationException($"Could not find elemet type: PriceDetailValue{i}");
                }

                tokens.TryGetValue(priceDetailTypeElementType, out string priceDetailType);
                tokens.TryGetValue(priceDetailValueElementType, out string priceDetailValue);

                if (priceDetailType == "Price (pence)")
                {
                    note.UnitPriceGbp = decimal.Parse(priceDetailValue) / 100;
                }
                else if (priceDetailType != null && priceDetailType.StartsWith("Price"))
                {
                    note.UnitPrice = decimal.Parse(priceDetailValue);
                    note.UnitCurrency = priceDetailType.Split(' ').Last().Replace("(", "").Replace(")", "");
                }
                else if (priceDetailType == "Exchange rate")
                {
                    note.ExchangeRate = decimal.Parse(priceDetailValue);
                }
            }

            for (int i = 1; i <= 4; i++)
            {
                if (!Enum.TryParse($"FeeType{i}", out ElementType feeTypeElementType))
                {
                    throw new ApplicationException($"Could not find elemet type: PriceDetailType{i}");
                }

                if (!Enum.TryParse($"FeeValue{i}", out ElementType feeValueElementType))
                {
                    throw new ApplicationException($"Could not find elemet type: PriceDetailValue{i}");
                }

                tokens.TryGetValue(feeTypeElementType, out string feeType);
                tokens.TryGetValue(feeValueElementType, out string feeValue);

                if (feeType == "Commission")
                {
                    note.Commission = decimal.Parse(feeValue);
                }
                else if (feeType == "FX Charge")
                {
                    note.FxCharge = decimal.Parse(feeValue);
                }
                else if (feeType == "Transfer Stamp")
                {
                    note.TransferFee = decimal.Parse(feeValue);
                }

                if (!Enum.TryParse($"OrderDetailNote{i}", out ElementType orderDetailNoteElementType))
                {
                    throw new ApplicationException($"Could not find elemet type: OrderDetailNote{i}");
                }

                if (tokens.TryGetValue(orderDetailNoteElementType, out string orderDetailsNote))
                {
                    if (orderDetailsNote.EndsWith("Order"))
                    {
                        note.OrderType = orderDetailsNote;
                    }
                    else if (orderDetailsNote.StartsWith("Venue of Execution:"))
                    {
                        note.Venue = orderDetailsNote.Replace("Venue of Execution:", "").Trim();
                    }
                }
            }

            if (tokens.TryGetValue(ElementType.StockCode, out string stockCode))
            {
                note.Symbol = stockCode.Replace("STOCK CODE:", "").Trim();
            }

            if (tokens.TryGetValue(ElementType.TotalAmountIncludingFees, out string totalAmountIncludingFees))
            {
                note.TotalAmountGbpIncludingFees = decimal.Parse(totalAmountIncludingFees);
            }

            if (tokens.TryGetValue(ElementType.TotalAmountExcludingFees, out string totalAmountExcludingFees))
            {
                note.TotalAmountGbpExcludingFees = decimal.Parse(totalAmountExcludingFees.Replace("GBP", "").Trim());
            }

            if (tokens.TryGetValue(ElementType.UnitPrice, out var unitPrice))
            {
                if (note.UnitPrice == default)
                {
                    note.UnitPrice = decimal.Parse(unitPrice);
                }

                if (note.UnitPriceGbp == default)
                {
                    note.UnitPriceGbp = decimal.Parse(unitPrice);
                }

                if (note.UnitCurrency == default)
                {
                    note.UnitCurrency = "GBP";
                }
            }


            return note;
        }
    }
}
