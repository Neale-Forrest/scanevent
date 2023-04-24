using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace ScanEvent
{
    class Program
    {
        private const string LIMIT = "100";

        static void Main(string[] args)
        {
            //NOTE: we could use args to a) increase the limit value and b) to pass in a custom event id if we need to work on a specific event
            while (true)
            {
                // Load the last processed event ID from storage
                //NOTE: this will be done in a proper DB with better fault checking.  For now we will use a normal file
                int lastEventId = LoadLastEventId();
                if (lastEventId == -1)
                {
                    //NOTE: this will need better error management
                    LogError("Could not find the last event id.");
                }

                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost/v1/scans/"); //This can be better enhanced using a auth token

                // Send the API request and get the response
                var response = client.GetAsync($"scanevents?FromEventId={lastEventId}&Limit={LIMIT}").Result;

                // Check if the request was successful
                //NOTE: This could be better managed with other HTTP response codes and could be better managed.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body as JSON
                    var json = response.Content.ReadAsStringAsync().Result;
                    List<ScanEvent> scanEvents = JsonConvert.DeserializeObject<List<ScanEvent>>(json);

                    // Process each scan event
                    foreach (var scanEvent in scanEvents)
                    {
                        // Record the event ID as the last processed ID
                        lastEventId = scanEvent.EventId;
                        SaveLastEventId(lastEventId);

                        //NOTE: this should be update to a proper db save
                        RecordLastScanEvent(scanEvent);
                    }
                }
                else
                {
                    // Log the error
                    var error = response.Content.ReadAsStringAsync().Result;
                    LogError($"Error fetching scan events: {error}");
                }
                Thread.Sleep(1000);//NOTE: this is in no way production code.  this is a way to create a repeatable process for the POC
            }
        }

        private static int LoadLastEventId()
        {
            using (var reader = new StreamReader("lastid.db"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    return Convert.ToInt32(line);
                }
            }
            return -1;
        }

        private static void LogError(string error)
        {
            using (StreamWriter writer = new StreamWriter("error.db"))
            {
                writer.WriteLine(error);
            }
        }

        private static void RecordLastScanEvent(ScanEvent scanEvent)
        {
            using (var writer = new StreamWriter($"{scanEvent.ParcelId}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) //NOTE: this is using a csv lib but this should be recorded to a database.
            {
                csv.WriteRecord(new { scanEvent.EventId, scanEvent.ParcelId, scanEvent.Type, scanEvent.CreatedDateTimeUtc, scanEvent.StatusCode, scanEvent.User.RunId });
            }
        }

        private static void SaveLastEventId(int lastEventId)
        {
            using (StreamWriter writer = new StreamWriter("lastid.db"))//NOTE we can have much better error exception handeling here.
            {
                writer.WriteLine(lastEventId);
            }
        }
    }
}
