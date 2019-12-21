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

            var sheets = service.Spreadsheets.Get(spreadsheetId).Execute().Sheets;//.FirstOrDefault().Properties.Title;
            string sheetName = "Data";
            //if (sheets.Count < 2)
            //{
            //    if (sheets.FirstOrDefault().Properties.Title == sheetName)
            //    {
            //        BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            //        batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            //        var blist = new AddSheetRequest();
            //        blist.Properties = new SheetProperties();
            //        blist.Properties.Title = "Baselist";
            //        blist.Properties.Hidden = false;
            //        batchUpdateSpreadsheetRequest.Requests = new List<Request> { new Request { AddSheet = blist } };
            //        var blistADD = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
            //        blistADD.Execute();
            //    }
            //    if (sheets.FirstOrDefault().Properties.Title == "Baselist")
            //    {
            //        BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            //        batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            //        var blist = new UpdateSheetPropertiesRequest();
            //        blist.Properties = new SheetProperties();
            //        blist.Properties.Hidden = false;
            //        batchUpdateSpreadsheetRequest.Requests = new List<Request> { new Request { UpdateSheetProperties = blist } };
            //        var blistADD = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
            //        blistADD.Execute();
            //        //hide false
            //    }
            //}
            //var sheet = sheets.FirstOrDefault(x => x.Properties.Title == sheetName);
            //if (sheet != null)
            //{
            //    BatchUpdateSpreadsheetRequest req = new BatchUpdateSpreadsheetRequest();
            //    req.Requests = new List<Request>();
            //    var deletesheetreq = new DeleteSheetRequest();
            //    deletesheetreq.SheetId = sheet.Properties.SheetId;
            //    req.Requests.Add(new Request
            //    {
            //        DeleteSheet = deletesheetreq
            //    });
            //    var Remove = service.Spreadsheets.BatchUpdate(req, spreadsheetId);
            //    Remove.Execute();

            //}
            //BatchUpdateSpreadsheetRequest addreq = new BatchUpdateSpreadsheetRequest();
            //addreq.Requests = new List<Request>();
            //var addSheetRequest = new AddSheetRequest();
            //addSheetRequest.Properties = new SheetProperties();
            //addSheetRequest.Properties.Title = sheetName;
            //addreq.Requests = new List<Request> { new Request { AddSheet = addSheetRequest } };
            //var ADD = service.Spreadsheets.BatchUpdate(addreq, spreadsheetId);
            //ADD.Execute();

            string range = $"{sheetName}!A:J";

            var clearreq = new ClearValuesRequest();
            var test = service.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, range).Execute();




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
