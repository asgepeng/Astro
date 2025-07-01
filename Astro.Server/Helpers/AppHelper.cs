using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System.Data.SqlTypes;
using System.Globalization;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Alaska.Data
{
    internal static class AppHelpers
    {
        internal static int GetUserID(HttpContext context)
        {
            var claimsPrincipal = context.User;
            string? userID = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            int.TryParse(userID, out int id);
            return id;
        }
        internal static (int userId, int roleId) ExtractPrincipal(HttpContext context)
        {
            var claimsPrincipal = context.User;
            string? userID = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            string? roleID = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            int.TryParse(userID, out int iUserID);
            int.TryParse(roleID, out int iRoleID);
            return (iUserID, iRoleID);
        }
        internal static string GetIpAddress(HttpRequest request)
        {
            // Cek header X-Forwarded-For
            if (request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                // Bisa ada beberapa IP, ambil yang pertama
                var ip = forwarded.ToString().Split(',')[0];
                return ip;
            }

            // Kalau tidak ada header, ambil langsung dari koneksi
            var ipaddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            return ipaddress is null ? "127.0.0.1" : ipaddress;
        }
        internal static string GetToken(HttpRequest request)
        {
            string? authHeader = request.Headers["Authorization"].FirstOrDefault();
            string? token = null;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }
            return token is null ? "" : token;
        }

    }

    public static class ExcelHelper
    {
        public static string GetCellValue(Cell cell, SharedStringTablePart stringTable)
        {
            if (cell == null || cell.CellValue == null)
                return "";

            string value = cell.CellValue.InnerText;

            if (cell.DataType != null && cell.DataType == CellValues.SharedString && stringTable != null)
            {
                return stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
            }

            return value;
        }

        public static string ToDecimalOrZero(string input)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result.ToString(CultureInfo.InvariantCulture)
                : "0";
        }
        public static void AddStyles(WorkbookPart workbookPart)
        {
            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();

            // Tambahkan numbering format custom
            var numberingFormats = new NumberingFormats();
            var thousandFormat = new NumberingFormat
            {
                NumberFormatId = 164, // ID >= 164 untuk format kustom
                FormatCode = StringValue.FromString("#,##0")
            };
            numberingFormats.Append(thousandFormat);

            stylesPart.Stylesheet = new Stylesheet
            {
                NumberingFormats = numberingFormats,

                Fonts = new Fonts(
                    new Font(),
                    new Font(new Bold())
                ),

                Fills = new Fills(
                    new Fill(new PatternFill { PatternType = PatternValues.None }),
                    new Fill(new PatternFill { PatternType = PatternValues.Gray125 }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = HexBinaryValue.FromString("E6E6FA") }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = HexBinaryValue.FromString("4682B4") }) { PatternType = PatternValues.Solid })
                ),

                Borders = new Borders(new Border()),

                CellFormats = new CellFormats(
                    new CellFormat(), // index 0
                    new CellFormat { FontId = 1, ApplyFont = true }, // index 1
                    new CellFormat { FillId = 2, FontId = 1, ApplyFill = true, ApplyFont = true }, // index 2
                    new CellFormat { FillId = 3, FontId = 1, ApplyFill = true, ApplyFont = true }, // index 3
                    new CellFormat { Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right }, ApplyAlignment = true }, // index 4
                    new CellFormat { Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right }, FontId = 1, ApplyAlignment = true, ApplyFont = true }, // index 5
                    new CellFormat { FillId = 2, FontId = 1, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right }, ApplyAlignment = true, ApplyFont = true, ApplyFill = true }, // index 6
                    new CellFormat { FillId = 3, FontId = 1, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right }, ApplyAlignment = true, ApplyFont = true, ApplyFill = true }, // index 7

                    // index 8 – format angka ribuan
                    new CellFormat
                    {
                        NumberFormatId = 164,
                        ApplyNumberFormat = true,
                        Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right },
                        ApplyAlignment = true
                    },
                    new CellFormat
                    {
                        FontId = 1,
                        NumberFormatId = 164,
                        ApplyFont = true,
                        ApplyNumberFormat = true,
                        Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right },
                        ApplyAlignment = true,
                        ApplyFill = true,
                        FillId = 2
                    },
                    new CellFormat
                    {
                        FontId = 1,
                        ApplyFont = true,
                        ApplyFill = true,
                        FillId = 2
                    }
                )
            };

            stylesPart.Stylesheet.Save();
        }


        public static Row CreateRow(uint index, params (string text, uint style)[] cells)
        {
            var row = new Row { RowIndex = index };
            foreach (var (text, style) in cells)
                row.Append(CreateCell(text, style));
            return row;
        }

        public static Row CreateSubtotalRow(string subtotalName, uint index, double income, double expense, double balance)
        {
            return CreateRow(index,
                            (subtotalName, 10, CellValues.String),
                            ("", 10, CellValues.String),
                            ("", 10, CellValues.String),
                            (income, 9, CellValues.Number),
                            (expense, 9, CellValues.Number),
                            (balance, 9, CellValues.Number),
                            ("", 10, CellValues.String));
        }

        private static Cell CreateCell(string value, uint styleIndex)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value ?? string.Empty),
                StyleIndex = styleIndex
            };
        }
        public static Row CreateRow(uint rowIndex, params (object? value, int styleIndex, CellValues type)[] values)
        {
            var row = new Row { RowIndex = rowIndex };

            foreach (var (value, styleIndex, type) in values)
            {
                string cellText = value switch
                {
                    null when type == CellValues.Number => "0",
                    null => string.Empty,
                    _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
                };

                var cell = new Cell
                {
                    StyleIndex = (uint)styleIndex,
                    DataType = new EnumValue<CellValues>(type),
                    CellValue = new CellValue(cellText)
                };

                row.Append(cell);
            }

            return row;
        }
        public static Columns CreateColumnWidths(params double[] widths)
        {
            var cols = new Columns();
            for (uint i = 0; i < widths.Length; i++)
            {
                cols.Append(new Column
                {
                    Min = i + 1,
                    Max = i + 1,
                    Width = widths[i],
                    CustomWidth = true
                });
            }
            return cols;
        }

    }
}
