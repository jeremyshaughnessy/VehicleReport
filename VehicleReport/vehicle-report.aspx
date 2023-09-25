<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vehicle-report.aspx.cs" Inherits="VehicleReport.vehicle_report" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <link rel="stylesheet" type="text/css" href="styles.css" />
        <title>Vehicle Report Generator</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="container">
                <h1 class="headerText">Vehicle Report Generator</h1>
                <h7 class="subHeaderText" color="lightgray">(Report will be downloaded upon successful generation)</h7>
                <h2 class="subHeaderText">Please upload a .CSV file of vehicles</h2>
                <h7 class="subHeaderText" color="lightgray">(File must be in format of year,make,model,msrp)</h7>
                <div class ="form-group">
                    <asp:FileUpload runat="server" id="csvUploadInput" CssClass="csvUpload"/>
                </div>
                <h2 class="subHeaderText">Please input your desired tax rate</h2>
                <div class="form-group">
                    <input type="number" step="any" id="taxRateInput" placeholder="1.00" runat="server" />
                 </div>
                <asp:Button id="btnUpload" runat="server" Text="Generate Report" OnClick="btnUpload_Click" CssClass="btn"/>
            </div>
        </form>
    </body>
</html>