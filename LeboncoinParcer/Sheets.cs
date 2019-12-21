using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace LeboncoinParcer
{
    class Sheets
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Quickstart";

        public static void Export(List<Realty> list)
        {
            UserCredential credential;
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            String spreadsheetId = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json")).TableId;

            var sheets = service.Spreadsheets.Get(spreadsheetId).Execute().Sheets;
            string sheetName = "Data";
            string range = $"{sheetName}!A:J";

            if (sheets.Any(x => x.Properties.Title == sheetName))
            {
                var clearreq = new ClearValuesRequest();
                var test = service.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, range).Execute();
            }
            var valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { };
            valueRange.Values.Add(new List<object> { "Name", "Date", "Phone", "Location", "Type", "Rooms", "Surface", "Furniture", "Ges", "Energy class" });
            var rows = list.Select(x => new List<object>() { x.Name, x.Date, x.Phone, x.LocalisationTown, x.Type, x.Rooms, x.Surface, x.Furniture, x.Ges, x.EnergyClass }).ToList();
            foreach (var myvalue in rows)
                valueRange.Values.Add(myvalue);
            var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }

    }
}
