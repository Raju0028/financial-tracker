using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using FinancialTracker.Models;
using System.Text;

namespace FinancialTracker.Services;

public class MonthlyInvestmentService
{
    private readonly string _spreadsheetId;

    private readonly SheetsService _sheetsService;

    public MonthlyInvestmentService(IConfiguration configuration, GoogleSheetsClientService googleSheetsClient)
    {
        _sheetsService = googleSheetsClient.SheetsService;
        _spreadsheetId = googleSheetsClient.SpreadsheetId;
    }

    public async Task<MonthlyInvestment> GetMonthlyInvestmentAsync()
    {
        var range = "MonthlyPayment!A4:AL4";

        var request = _sheetsService.Spreadsheets.Values.Get(
            _spreadsheetId,
            range);

        var response = await request.ExecuteAsync();

        var row = response.Values?.FirstOrDefault();

        if (row == null)
        {
            return new MonthlyInvestment();
        }

        return new MonthlyInvestment
        {
            // A4
            TotalAmount = GetDecimal(row, 0),

            // B4
            GasUsed = GetDecimal(row, 1),

            // D4
            Electricity = GetDecimal(row, 3),

            // F4
            Mobile = GetDecimal(row, 5),

            // J4
            Gift = GetDecimal(row, 9),

            // L4
            AuBank = GetDecimal(row, 11),

            // N4
            CarService = GetDecimal(row, 13),

            // P4
            CarInsurance = GetDecimal(row, 15),

            // R4
            Bike = GetDecimal(row, 17),

            // T4
            Cng = GetDecimal(row, 19),

            // V4
            Wifi = GetDecimal(row, 21),

            // Z4
            CcBank = GetDecimal(row, 25),

            // AB4
            Tour = GetDecimal(row, 27),

            // AD4
            FriendsTour = GetDecimal(row, 29),

            // AF4
            HouseProduct = GetDecimal(row, 31),

            // AH4
            EmergencyAmount = GetDecimal(row, 33),

            // AL4
            IdfcBank = GetDecimal(row, 37)
        };
    }

    private static decimal GetDecimal(
        IList<object> row,
        int index)
    {
        if (row.Count <= index)
        {
            return 0;
        }

        return decimal.TryParse(
            row[index]?.ToString(),
            out var value)
            ? value
            : 0;
    }
}