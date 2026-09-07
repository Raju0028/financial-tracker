using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Text;

namespace FinancialTracker.Services;

public class GoogleSheetsClientService
{
    public SheetsService SheetsService { get; }

    public string SpreadsheetId { get; }

    public GoogleSheetsClientService(IConfiguration configuration)
    {
        var credentialsBase64 =
            configuration["GoogleSheets:CredentialsBase64"]
            ?? throw new InvalidOperationException(
                "Google Sheets credentials are not configured.");

        SpreadsheetId =
            configuration["GoogleSheets:SpreadsheetId"]
            ?? throw new InvalidOperationException(
                "Google Sheets SpreadsheetId is not configured.");

        var credentialsJson = Encoding.UTF8.GetString(
            Convert.FromBase64String(credentialsBase64));

        GoogleCredential credential =
            GoogleCredential.FromJson(credentialsJson)
            .CreateScoped(SheetsService.Scope.Spreadsheets);

        SheetsService = new SheetsService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Financial Tracker"
            });
    }
}