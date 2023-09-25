using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VehicleReport
{
    public partial class vehicle_report : System.Web.UI.Page
    {
        #region Properties
        public string InputPath { get; set; }
        public string OutputPath { get; set; }

        public string CurrentDate = DateTime.Now.ToString("MMddyyyy");
        public decimal TaxRate { get; set; }
        public decimal TotalMsrp { get; set; }
        public decimal TotalListPrice { get; set; }
        public string ReportTitle { get; set; }

        #endregion

        #region Page Events
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (csvUploadInput.HasFile)
            {
                if (csvUploadInput.PostedFile.ContentType == "text/csv")
                {
                    //save uploaded .CSV file to csv-files folder
                    InputPath = Server.MapPath("~/csv-files/" + Path.GetFileName(csvUploadInput.FileName));
                    csvUploadInput.SaveAs(InputPath);

                    ReportTitle = "vehicles" + CurrentDate + ".txt";

                    decimal.TryParse(taxRateInput.Value, out decimal inputTax);
                    TaxRate = inputTax;

                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(InputPath), ReportTitle)))
                    {
                        //save initial report file to csv-files folder
                        File.Create(Path.GetDirectoryName(InputPath) + "\\" + ReportTitle).Close();
                    }

                    OutputPath = Path.Combine(Path.GetDirectoryName(InputPath), ReportTitle);
                    string[] csvLines = File.ReadAllLines(InputPath);

                    if (csvLines.Length == 0)
                        return;

                    List<Vehicle> vehicles = CreateVehicles(csvLines);
                    GenerateReport(vehicles);
                }
            }
        }

        #endregion

        #region Public Methods
        public List<Vehicle> CreateVehicles(string[] csvLines)
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            for (int i = 1; i < csvLines.Length; i++)
            {
                string[] rowData = csvLines[i].Split(',');

                Vehicle vehicle = new Vehicle()
                {
                    Year = int.Parse(rowData[0]),
                    Make = rowData[1],
                    Model = rowData[2],
                    Msrp = decimal.Parse(rowData[3]),
                };

                vehicles.Add(vehicle);
            }

            foreach (var vehicle in vehicles)
            {
                TotalMsrp += vehicle.Msrp;
                TotalListPrice += (vehicle.Msrp * TaxRate);
            }

            return vehicles;
        }
        public void GenerateReport(List<Vehicle> vehicles)
        {

            var groupByYear = vehicles.GroupBy(v => v.Year).OrderByDescending(d => d.Key);

            using (StreamWriter writer = new StreamWriter(OutputPath))
            {
                if (writer == null)
                    return;

                string outputHeader = String.Format("--- Vehicle Report --- \t\t\t\tDate: {0}", CurrentDate);

                writer.WriteLine(outputHeader);

                foreach (var vehicleYear in groupByYear)
                {
                    writer.WriteLine("\n" + vehicleYear.Key);
                    foreach (var vehicle in vehicleYear)
                    {
                        string listPrice = (vehicle.Msrp * TaxRate).ToString("F2");
                        string vehicleOutput = String.Format("\t{0} {1}\tMSRP: ${2}\tList Price: ${3}", vehicle.Make, vehicle.Model, vehicle.Msrp.ToString("F2"), listPrice);
                        writer.WriteLine(vehicleOutput);
                    }
                }

                writer.WriteLine("\n--- Grand Total ---");
                string totalOutputs = String.Format("\tMSRP: ${0}\n\tList Price: ${1}", TotalMsrp.ToString("F2"), TotalListPrice.ToString("F2"));
                writer.WriteLine(totalOutputs);

                writer.Close();
            }

            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + ReportTitle);
            String downloadPath = OutputPath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty);
            Response.TransmitFile(Server.MapPath(downloadPath));

            Response.End();
        }

        #endregion

        public class Vehicle
        {
            public int Year { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public decimal Msrp { get; set; }
        }
    }
}