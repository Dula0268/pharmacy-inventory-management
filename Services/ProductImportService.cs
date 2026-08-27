using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public class ProductImportService : IProductImportService
    {
        private readonly IProductService _productService;

        public ProductImportService(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<int> ImportMedicinesAsync(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            var headers = BuildHeaderMap(worksheet);
            ValidateHeaders(headers, new[]
            {
                "Category", "Genaric name", "Brand Name", "Strength", "Dosage Form", "Batch No",
                "EXP", "MFD", "Packing", "No of Units", "Unit Price", "Units/Pack",
                "No of packs", "Pack price", "Total Value"
            });

            var genericNameHeader = ResolveHeader(headers, "Genaric name", "Generic name");

            var imported = 0;
            foreach (var row in worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>())
            {
                if (IsRowEmpty(row))
                    continue;

                var product = new Product
                {
                    Type = ProductType.Medicine,
                    Category = GetString(row, headers, "Category"),
                    GenericName = GetString(row, headers, genericNameHeader),
                    BrandName = GetString(row, headers, "Brand Name"),
                    Strength = GetString(row, headers, "Strength"),
                    DosageForm = GetString(row, headers, "Dosage Form"),
                    BatchNo = GetString(row, headers, "Batch No"),
                    ExpDate = GetDateOnly(row, headers, "EXP"),
                    MfdDate = GetDateOnly(row, headers, "MFD"),
                    Packing = GetString(row, headers, "Packing"),
                    NoOfUnits = GetInt(row, headers, "No of Units"),
                    UnitPrice = GetDecimal(row, headers, "Unit Price"),
                    UnitsPerPack = GetInt(row, headers, "Units/Pack"),
                    NoOfPacks = GetInt(row, headers, "No of packs"),
                    PackPrice = GetDecimal(row, headers, "Pack price"),
                    TotalValue = GetDecimal(row, headers, "Total Value")
                };

                await _productService.AddMedicineAsync(product).ConfigureAwait(false);
                imported++;
            }

            return imported;
        }

        public async Task<int> ImportGroceriesAsync(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            var headers = BuildHeaderMap(worksheet);
            ValidateHeaders(headers, new[]
            {
                "Item type", "Brand", "Speciality", "Size", "Price", "Count", "Total", "EXD", "MFD", "Out Colour", "Note"
            });

            var imported = 0;
            foreach (var row in worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>())
            {
                if (IsRowEmpty(row))
                    continue;

                var product = new Product
                {
                    Type = ProductType.Grocery,
                    ItemType = GetString(row, headers, "Item type"),
                    Brand = GetString(row, headers, "Brand"),
                    Speciality = GetString(row, headers, "Speciality"),
                    Size = GetString(row, headers, "Size"),
                    Price = GetDecimal(row, headers, "Price"),
                    Count = GetInt(row, headers, "Count"),
                    Total = GetDecimal(row, headers, "Total"),
                    ExdDate = GetDateOnly(row, headers, "EXD"),
                    MfdDate = GetDateOnly(row, headers, "MFD"),
                    OutColour = GetString(row, headers, "Out Colour"),
                    Note = GetString(row, headers, "Note")
                };

                await _productService.AddGroceryAsync(product).ConfigureAwait(false);
                imported++;
            }

            return imported;
        }

        private static Dictionary<string, int> BuildHeaderMap(IXLWorksheet worksheet)
        {
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var firstRow = worksheet.FirstRowUsed();

            if (firstRow is null)
                throw new InvalidOperationException("The worksheet is empty.");

            foreach (var cell in firstRow.CellsUsed())
            {
                var header = NormalizeHeader(cell.GetString());
                if (!string.IsNullOrWhiteSpace(header) && !headerMap.ContainsKey(header))
                    headerMap[header] = cell.Address.ColumnNumber;
            }

            return headerMap;
        }

        private static void ValidateHeaders(IReadOnlyDictionary<string, int> headerMap, IEnumerable<string> requiredHeaders)
        {
            var missing = requiredHeaders
                .Where(required => !HasHeader(headerMap, required))
                .ToArray();

            if (missing.Length > 0)
                throw new InvalidOperationException($"Missing required Excel headers: {string.Join(", ", missing)}");
        }

        private static bool HasHeader(IReadOnlyDictionary<string, int> headerMap, params string[] candidates)
            => candidates.Any(candidate => headerMap.ContainsKey(NormalizeHeader(candidate)));

        private static string ResolveHeader(IReadOnlyDictionary<string, int> headerMap, params string[] candidates)
        {
            foreach (var candidate in candidates)
            {
                var normalized = NormalizeHeader(candidate);
                if (headerMap.ContainsKey(normalized))
                    return candidate;
            }

            throw new InvalidOperationException($"Missing required Excel header: {string.Join(" or ", candidates)}");
        }

        private static bool IsRowEmpty(IXLRangeRow row)
            => row.CellsUsed().All(cell => string.IsNullOrWhiteSpace(cell.GetString()));

        private static string NormalizeHeader(string value)
            => new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

        private static string GetString(IXLRangeRow row, IReadOnlyDictionary<string, int> headers, string header)
        {
            if (!headers.TryGetValue(NormalizeHeader(header), out var column))
                return string.Empty;

            return row.Cell(column).GetString().Trim();
        }

        private static int GetInt(IXLRangeRow row, IReadOnlyDictionary<string, int> headers, string header)
        {
            var text = GetString(row, headers, header);
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                return value;

            var cell = row.Cell(headers[NormalizeHeader(header)]);
            return (int)Math.Round(cell.GetDouble());
        }

        private static decimal GetDecimal(IXLRangeRow row, IReadOnlyDictionary<string, int> headers, string header)
        {
            var text = GetString(row, headers, header);
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                return value;

            var cell = row.Cell(headers[NormalizeHeader(header)]);
            return Convert.ToDecimal(cell.GetDouble(), CultureInfo.InvariantCulture);
        }

        private static DateOnly? GetDateOnly(IXLRangeRow row, IReadOnlyDictionary<string, int> headers, string header)
        {
            if (!headers.TryGetValue(NormalizeHeader(header), out var column))
                return null;

            var cell = row.Cell(column);
            if (cell.IsEmpty())
                return null;

            if (cell.TryGetValue<DateTime>(out var dateTime))
                return DateOnly.FromDateTime(dateTime);

            var text = cell.GetString();
            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
                return DateOnly.FromDateTime(dateTime);

            if (DateOnly.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
                return dateOnly;

            return null;
        }
    }
}