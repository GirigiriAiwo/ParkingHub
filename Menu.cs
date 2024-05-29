using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms;
using ZXing;
using System.Data.OleDb;
using static QRCoder.PayloadGenerator;
using System.Collections;
using GemBox.Spreadsheet;
using GemBox.Spreadsheet.WinFormsUtilities;

namespace Project_Reset
{
    public partial class Menu : Form
    {
        OleDbConnection myConn;
        OleDbDataAdapter da;
        OleDbCommand cmd;
        DataSet ds;
        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\OOP2\\CLOSE 2\\CLOSE\\abapo\\LG2\\LETS GO INTEGRATE\\DB\\Database\\ProjectDatabase2.mdb;";
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        private Dictionary<int, Color> updatedRows = new Dictionary<int, Color>();
        private Dictionary<int, Color> updatedRows2 = new Dictionary<int, Color>();
        private System.Windows.Forms.Timer dataRefreshTimer;
        int dgvVehicleindex;
        public Menu()
        {
            //dataRefreshTimer = new System.Windows.Forms.Timer();
            //dataRefreshTimer.Interval = 5000; // Refresh every 5 seconds (adjust as needed)
            //dataRefreshTimer.Tick += new EventHandler(timer3_Tick);
            //dataRefreshTimer.Start();
            InitializeComponent();
            LoadParkingZoneColors();
            LoadMotorParkingZoneColors();
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

        }
        

        private void QRscanner_Load(object sender, EventArgs e)
        {
            
        }
        private void Menu_Load(object sender, EventArgs e)
        {
            dgvUserInfo.Columns.Add("License Plate", "LicensePlate");
            //ADDUNG COLUMNS PROGRAMATICALLY EXCEEDING THE COLUMN COUNT
            dgvUserInfo.Columns.Add("Car Model", "CarModel");
            dgvUserInfo.Columns.Add("Color", "Color");
            dgvUserInfo.Columns.Add("CarType", "CarType");
            dgvUserInfo.Columns.Add("VehicleIn", "VehicleIn");
            dgvUserInfo.Columns.Add("Vehicleout", "VehicleOut");
            string[] row1 = { "", "", "", "", "","" };
            dgvUserInfo.Rows.Add(row1);
            btnLoad_Click(sender, e);
            timer2.Start();
            lbTime.Text = DateTime.Now.ToLongTimeString();
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {
                cboDevice.Items.Add(filterInfo.Name);
            }

            // cboDevice.SelectedIndex = 0;   
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_Newframe;
            captureDevice.Start();
            timer1.Start();
        }

        private void CaptureDevice_Newframe(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void QRscanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox.Image);
                if (result != null)
                {
                    txtQRCode.Text = result.ToString();
                    timer1.Stop();
                    btnStart_Click(sender, e);
                }
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            lbTime.Text = DateTime.Now.ToLongTimeString();
            timer2.Start();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVehicle.SelectedRows.Count > 0)
            {
                try
                {
                    // Get the ID of the selected row
                    string id = dgvVehicle.CurrentRow.Cells["ID"].Value.ToString();

                    // Construct the DELETE query
                    string query = "DELETE FROM UserInfo WHERE ID = @id";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        // Add parameter for the ID
                        cmd.Parameters.AddWithValue("@id", id);

                        // Open connection and execute the command
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        connection.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Row deleted successfully.");
                            btnLoad_Click(sender, e); // Reload the data after deletion
                        }
                        else
                        {
                            MessageBox.Show("No rows were deleted.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting row: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }

            btnLoad_Click(sender, e);
        }

        private void dgvVehicle_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            string type;
            if (radioButton1.Checked)
            {
                 type = radioButton1.Text;
            }
            else
            {
                type = radioButton2.Text;
            }

            dgvUserInfo.Rows.Add(txtLicensePlate.Text, txtCarModel.Text, txtColor.Text, type, lbTime.Text);
            

            // Change the color of the row cells to gree

            // Clear input fields
            txtLicensePlate.Text = "";
            txtCarModel.Text = "";
            txtColor.Text = "";
            int newRow = dgvUserInfo.Rows.Count - 1;
            dgvUserInfo.Rows[newRow].DefaultCellStyle.BackColor = Color.LightGreen;
        }
        private void RefreshDataGridView()
        {
            myConn.Open();
            ds.Clear(); // Clear the existing data
            da.Fill(ds, "HistoryQuery"); // Refill the dataset
            dgvVehicle.DataSource = ds.Tables["HistoryQuery"]; // Set the data source again
            myConn.Close();

            // Reapply the background colors to updated rows
            foreach (var kvp in updatedRows)
            {
                dgvVehicle.Rows[kvp.Key].DefaultCellStyle.BackColor = kvp.Value;
            }
            foreach (var kvp in updatedRows2)
            {
                dgvVehicle.Rows[kvp.Key].DefaultCellStyle.BackColor = kvp.Value;
            }
        }
        private void RefreshDataGridView2()
        {
            myConn.Open();
            ds.Clear(); // Clear the existing data
            da.Fill(ds, "HistoryQuery"); // Refill the dataset
            dgvVehicle.DataSource = ds.Tables["HistoryQuery"]; // Set the data source again
            myConn.Close();

            // Reapply the background colors to updated rows
            foreach (var kvp in updatedRows)
            {
                dgvVehicle.Rows[kvp.Key].DefaultCellStyle.BackColor = kvp.Value;
            }
            foreach (var kvp in updatedRows2)
            {
                dgvVehicle.Rows[kvp.Key].DefaultCellStyle.BackColor = kvp.Value;
            }

        }
        private void testCon_Click(object sender, EventArgs e)
        {
            
        }
        private void ResetDay_Click(object sender, EventArgs e)
        {
            string resetQuery1 = "UPDATE VehicleIn SET VehicleDateIn = NULL, VehicleIn = NULL";
            string resetQuery2 = "UPDATE VehicleOut SET VehicleDateOut = NULL, VehicleOut = NULL";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command1 = new OleDbCommand(resetQuery1, connection))
                using (OleDbCommand command2 = new OleDbCommand(resetQuery2, connection))
                {
                    try
                    {
                        connection.Open();
                        command1.ExecuteNonQuery();
                        command2.ExecuteNonQuery();
                        MessageBox.Show("Vehicle records reset successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error resetting vehicle records: " + ex.Message);
                    }
                }
            }
            btnLoad_Click(sender, e);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            myConn = new OleDbConnection(connectionString);
            //da = new OleDbDataAdapter("SELECT *FROM UserInfo", myConn);

           
            da = new OleDbDataAdapter("SELECT UserInfo.ID, UserInfo.StudentID, UserCar.LicensePlate, VehicleIn.VehicleIn, VehicleOut.VehicleOut, UserCar.CarType, UserInfo.ParkingZone\r\nFROM VehicleIn INNER JOIN (VehicleOut INNER JOIN (UserInfo INNER JOIN UserCar ON UserInfo.ID = UserCar.ID) ON VehicleOut.ID = UserInfo.ID) ON VehicleIn.ID = UserInfo.ID;\r\n", myConn);
            ds = new DataSet();
            myConn.Open();
            da.Fill(ds, "HistoryQuery");
            dgvVehicle.DataSource = ds.Tables["HistoryQuery"];
            myConn.Close();
            

        }

        private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            string searchedID = txtQRCode.Text.Trim(); // Get the text from txtQRCode and remove leading/trailing spaces

            // Search the database for the matching ID
            string query = "SELECT VehicleIn FROM VehicleIn WHERE ID = @id";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@id", searchedID); // Add the parameter here

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Check if VehicleIn is not DBNull (i.e., if there's already a value)
                        if (reader["VehicleIn"] != DBNull.Value)
                        {
                            // Perform VehicleOut
                            PerformVehicleOut(searchedID, connection);
                            int parkingZone = getParkingZone(searchedID);
                            ResetParkingZone(parkingZone, searchedID);
                            vehicleType(searchedID);
                            if (vehicleType(searchedID) == 1)
                            {
                                UpdateZoneStatus(parkingZone, 0);
                            }
                            else if (vehicleType(searchedID) == 0)
                            {
                                UpdateMotorZoneStatus(parkingZone, 0);
                            }
                            else
                            {
                                MessageBox.Show("Not A Vehicle");
                            }


                        }
                        else
                        {
                            // Perform VehicleIn
                            PerformVehicleIn(searchedID, connection);
                            if (vehicleType(searchedID) == 1)
                            {
                                int zoneNumber = FindAvailableParkingZone();
                                ParkingZone(zoneNumber, searchedID);
                                UpdateZoneStatus(zoneNumber, 1);
                                MessageBox.Show($"Parking zone {zoneNumber} is now occupied.");
                            }
                            else if (vehicleType(searchedID) == 0)
                            {
                                int zoneNumber = FindAvailableMotorParkingZone();
                                ParkingZone(zoneNumber, searchedID);
                                UpdateMotorZoneStatus(zoneNumber, 1);
                                MessageBox.Show($"Parking zone {zoneNumber} is now occupied.");
                            }
                            else
                            {
                                MessageBox.Show("No Parking Zone Found");
                            }
                            // If a parking zone is found
                            RefreshDataGridView();
                        }  
                    }
                    else
                    {
                        MessageBox.Show("ID not found in the database.");
                    }
                }
            }
        }
        private int vehicleType(string searchedID)
        {
            string query = "SELECT CarType FROM UserCar WHERE ID = @id";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    // Add parameter for the searched ID
                    cmd.Parameters.AddWithValue("@id", searchedID);
                    object result = cmd.ExecuteScalar();    
                    string vehicle = Convert.ToString(result);
                    connection.Close();
                    if (vehicle == "4-Wheels")
                    {
                        return 1;

                    }
                    else if(vehicle == "Motorcycle")
                    {
                        return 0;
                    }
                    else
                    {
                        return 69;

                    }
                }

            }

        }
        private int getParkingZone(string searchedID)
        {
            int parkingZone = 0;

            // Construct the SQL query to retrieve the parking zone for the searched ID
            string query = "SELECT ParkingZone FROM UserInfo WHERE ID = @id";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    // Add parameter for the searched ID
                    cmd.Parameters.AddWithValue("@id", searchedID);

                    try
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();

                        // Check if the result is not null and convert it to an integer
                        if (result != null && result != DBNull.Value)
                        {
                            parkingZone = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions
                        MessageBox.Show("Error retrieving parking zone: " + ex.Message);
                    }
                }
                connection.Close();
            }

            return parkingZone;
        }
        private void ResetParkingZone(int zoneNumber, string searchedID)
        {

            string query = "UPDATE UserInfo SET ParkingZone = NULL WHERE ID = @searchedID";

            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@searchedID", searchedID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void ParkingZone( int zoneNumber ,string searchedID)
        {
            string query = "UPDATE UserInfo SET ParkingZone = @zoneNumber WHERE ID = @searchedID";
            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Execute the query to find the first available parking zone
                    cmd.Parameters.AddWithValue("@zoneNumber", zoneNumber );
                    cmd.Parameters.AddWithValue("@id", searchedID);
                    cmd.ExecuteNonQuery();
                    foreach (DataGridViewRow row in dgvVehicle.Rows)
                    {
                        if (row.Cells["ID"].Value != null && row.Cells["ID"].Value.ToString() == searchedID)
                        {
                            row.DefaultCellStyle.BackColor = Color.Green;
                            break; // Exit loop after finding the row
                        }
                    }
                }
                
            }

        }
        private int FindAvailableParkingZone()
        {
            // Construct the SQL query to find an available parking zone
            string query = "SELECT TOP 1 ID FROM ZonesTable WHERE Availability = 0";

            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();

                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Execute the query to find the first available parking zone
                    object result = cmd.ExecuteScalar();

                    // If a parking zone is found, return its auto-numbered ID; otherwise, return -1
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        private int FindAvailableMotorParkingZone()
        {
            // Construct the SQL query to find an available parking zone
            string query = "SELECT TOP 1 ID FROM ZonesMotorTable WHERE Availability = 0";

            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();

                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Execute the query to find the first available parking zone
                    object result = cmd.ExecuteScalar();

                    // If a parking zone is found, return its auto-numbered ID; otherwise, return -1
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        private void PerformVehicleIn(string searchedID, OleDbConnection connection)
        {
            // Update the VehicleIn for the ID with the current time
            string updateQuery = "UPDATE VehicleIn SET VehicleIn = @vehicleIn, VehicleDateIn = @VehicleDate WHERE ID = @id";
            using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection))
            {
                updateCmd.Parameters.AddWithValue("@vehicleIn", DateTime.Now.ToString("HH:mm:ss"));
                updateCmd.Parameters.AddWithValue("@VehicleDate", DateTime.Now.ToString("yyyy-MM-dd"));
                updateCmd.Parameters.AddWithValue("@id", searchedID);
                updateCmd.ExecuteNonQuery();
            }

            // Change the row color to green
            foreach (DataGridViewRow row in dgvVehicle.Rows)
            {
                if (row.Cells["ID"].Value != null && row.Cells["ID"].Value.ToString() == searchedID)
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                    updatedRows[row.Index] = Color.Green;
                    break; // Exit loop after finding the row
                }
            }

            MessageBox.Show($"Vehicle In recorded for ID: {searchedID}");

            // Clear txtQRCode without triggering txtQRCode_TextChanged
            txtQRCode.TextChanged -= txtQRCode_TextChanged;
            txtQRCode.Text = "";
            txtQRCode.TextChanged += txtQRCode_TextChanged;
        }

        private void PerformVehicleOut(string searchedID, OleDbConnection connection)
        {
            // Update the VehicleOut for the ID with the current time
            string updateQuery = "UPDATE VehicleOut SET VehicleOut = @vehicleOut, VehicleDateOut = @VehicleDate WHERE ID = @id";
            using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection))
            {
                updateCmd.Parameters.AddWithValue("@vehicleOut", DateTime.Now.ToString("HH:mm:ss"));
                updateCmd.Parameters.AddWithValue("@VehicleDateOut", DateTime.Now.ToString("yyyy-MM-dd")); 
                updateCmd.Parameters.AddWithValue("@id", searchedID);
                updateCmd.ExecuteNonQuery();
            }

            // Change the row color to red
            foreach (DataGridViewRow row2 in dgvVehicle.Rows)
            {
                // Check if the cell value and searchedID are not null before comparison
                if (row2.Cells["ID"].Value != null && row2.Cells["ID"].Value.ToString() == searchedID)
                {
                    row2.DefaultCellStyle.BackColor = Color.Red;
                    updatedRows2[row2.Index] = Color.Red;
                    break; // Exit loop after finding the row
                }
            }
            //int zoneNumber = GetZoneNumberFromID(searchedID); // Assuming a method to get zone number from ID !!!!!!!!!!!!!!!!! ANI TA MAGTIWAS TOMORROW !!!!!!!!!!!!!!!!!!!!!! NOT SEARCHEDID NEED TO STORE VALUE OF PARKINGZONE ALLOCATED AND THEN CHANGE THE VALUE OF THAT PARKING ZONE WHEN VEHICLE OUT
            //UpdateZoneStatus(zoneNumber, 1);

            MessageBox.Show($"Vehicle Out recorded for ID: {searchedID}");
            txtQRCode.TextChanged -= txtQRCode_TextChanged;
            txtQRCode.Text = "";
            txtQRCode.TextChanged += txtQRCode_TextChanged;
            RefreshDataGridView2();

        }

            

        private void VehicleOut_Click(object sender, EventArgs e)
        {
            if (dgvUserInfo.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgvUserInfo.SelectedRows[0];

                // Check if VehicleIn is not null or empty
                if (selectedRow.Cells["VehicleIn"].Value != null && !string.IsNullOrEmpty(selectedRow.Cells["VehicleIn"].Value.ToString()))
                {
                    // Perform VehicleOut

                    // Update the "Vehicle Out" column with the current time
                    selectedRow.Cells["VehicleOut"].Value = DateTime.Now.ToString("HH:mm:ss");

                    // Change the row color to red
                    selectedRow.DefaultCellStyle.BackColor = Color.Red;
                    
                }
                else
                {
                    MessageBox.Show("No Vehicle In recorded for this row. Cannot perform Vehicle Out.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row.");
            }
            RefreshDataGridView();
        }

        
        private void ParkingSpot_Click(object sender, EventArgs e)
        {
            Button spotButton = (Button)sender;
            string buttonName = spotButton.Name; // Get the name of the button
            int zoneNumber = int.Parse(buttonName.Replace("Zone","")); // Extract the zone number from the button's name

            // Toggle the color of the button
            if (spotButton.BackColor == Color.Green)
            {
                UpdateZoneStatus(zoneNumber, 1); // Update the zone status to 1 (occupied)
            }
            else
            {
                spotButton.BackColor = Color.Green; // Change to green
                UpdateZoneStatus(zoneNumber, 0); // Update the zone status to 0 (available)
            }
            MessageBox.Show(Convert.ToString(zoneNumber));
            zoneNumber = Convert.ToInt32(zoneNumber);
        }
        private void MotorParkingSpot_Click(object sender, EventArgs e)
        {
            Button spotButton = (Button)sender;
            string buttonName = spotButton.Name; // Get the name of the button
            int zoneNumber = int.Parse(buttonName.Replace("MZone", "")); // Extract the zone number from the button's name

            // Toggle the color of the button
            if (spotButton.BackColor == Color.Green)
            {
                UpdateMotorZoneStatus(zoneNumber, 1); // Update the zone status to 1 (occupied)
            }
            else
            {
                spotButton.BackColor = Color.Green; // Change to green
                UpdateMotorZoneStatus(zoneNumber, 0); // Update the zone status to 0 (available)
            }
            MessageBox.Show(Convert.ToString(zoneNumber));
            zoneNumber = Convert.ToInt32(zoneNumber);
        }
        private void UpdateZoneStatus(int zoneNumber, int status)
        {
            // Construct the SQL UPDATE query

            string query = "UPDATE ZonesTable SET Availability = @status WHERE ID = @zoneNumber";
            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Add parameters to the query to prevent SQL injection
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@zoneNumber", zoneNumber);
                    MessageBox.Show($"This is the Zone {zoneNumber}");
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Check if the update was successful
                    string controlName = "Zone" + zoneNumber;
                    Control[] foundControls = this.Controls.Find(controlName, true);

                    // Check the status to determine the color
                    if (status == 0)
                    {
                        // Change the background color to green
                        if (foundControls.Length > 0 && foundControls[0] is Control zoneControl)
                        {
                            zoneControl.BackColor = Color.Green;
                        }
                        else
                        {
                            MessageBox.Show($"Control with name {controlName} not found.");
                        }
                    }
                    else if (status == 1)
                    {
                        // Change the background color to red
                        if (foundControls.Length > 0 && foundControls[0] is Control zoneControl)
                        {
                            zoneControl.BackColor = Color.Red;
                        }
                        else
                        {
                            MessageBox.Show($"Control with name {controlName} not found.");
                        }
                    }
                    else
                    {
                        // Handle invalid status value
                        MessageBox.Show($"Invalid status value: {status}");
                    }
                }
                myConn.Close();
            }
            // Assuming myConn is your OleDbConnection object

        }
        private void UpdateMotorZoneStatus(int zoneNumber, int status)
        {
            // Construct the SQL UPDATE query

            string query = "UPDATE ZonesMotorTable SET Availability = @status WHERE ID = @zoneNumber";
            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Add parameters to the query to prevent SQL injection
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@zoneNumber", zoneNumber);
                    MessageBox.Show($"This is the Zone {zoneNumber}");
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Check if the update was successful
                    string controlName = "MZone" + zoneNumber;
                    Control[] foundControls = this.Controls.Find(controlName, true);

                    // Check the status to determine the color
                    if (status == 0)
                    {
                        // Change the background color to green
                        if (foundControls.Length > 0 && foundControls[0] is Control zoneControl)
                        {
                            zoneControl.BackColor = Color.Green;
                        }
                        else
                        {
                            MessageBox.Show($"Control with name {controlName} not found.");
                        }
                    }
                    else if (status == 1)
                    {
                        // Change the background color to red
                        if (foundControls.Length > 0 && foundControls[0] is Control zoneControl)
                        {
                            zoneControl.BackColor = Color.Red;
                        }
                        else
                        {
                            MessageBox.Show($"Control with name {controlName} not found.");
                        }
                    }
                    else
                    {
                        // Handle invalid status value
                        MessageBox.Show($"Invalid status value: {status}");
                    }
                }
                myConn.Close();
            }
            // Assuming myConn is your OleDbConnection object

        }
        private void LoadParkingZoneColors()
        {
            // Loop through each parking zone button
            for (int zoneNumber = 1; zoneNumber <= 30; zoneNumber++) // Assuming you have 3 zones
            {
                // Retrieve the availability status from the database for the current zone
                int availability = GetAvailabilityForZone(zoneNumber);

                // Get the corresponding button for the current zone
                string controlName = "Zone" + zoneNumber;
                Control[] foundControls = this.Controls.Find(controlName, true);

                if (foundControls.Length > 0 && foundControls[0] is Button zoneButton)
                {
                    // Update the color of the button based on the availability status
                    zoneButton.BackColor = (availability == 0) ? Color.Green : Color.Red;
                }
                else
                {
                    MessageBox.Show($"Button with name {controlName} not found.");
                }
            }
        }
        private void LoadMotorParkingZoneColors()
        {
            // Loop through each parking zone button
            for (int zoneNumber = 1; zoneNumber <= 32; zoneNumber++) // Assuming you have 3 zones
            {
                // Retrieve the availability status from the database for the current zone
                int availability = GetMotorAvailabilityForZone(zoneNumber);

                // Get the corresponding button for the current zone
                string controlName = "MZone" + zoneNumber;
                Control[] foundControls = this.Controls.Find(controlName, true);

                if (foundControls.Length > 0 && foundControls[0] is Button zoneButton)
                {
                    // Update the color of the button based on the availability status
                    zoneButton.BackColor = (availability == 0) ? Color.Green : Color.Red;
                }
                else
                {
                    MessageBox.Show($"Button with name {controlName} not found.");
                }
            }
        }

        // Method to retrieve availability status from the database for a specific zone
        private int GetAvailabilityForZone(int zoneNumber)
        {
            int availability = 0; // Default value indicating failure to retrieve availability
        
            // Construct the SQL query to retrieve availability for the specified zone
            string query = "SELECT Availability FROM ZonesTable WHERE ID = @zoneNumber";

            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Add parameter for the zone number
                    cmd.Parameters.AddWithValue("@zoneNumber", zoneNumber);


                    myConn.Open();
                    object result = cmd.ExecuteScalar();

                    // Check if the result is not null and convert it to an integer
                    if (result != null && result != DBNull.Value)
                    {
                        availability = Convert.ToInt32(result);
                    }


                    myConn.Close();

                }
            }
            

            return availability;
        }
        private int GetMotorAvailabilityForZone(int zoneNumber)
        {
            int availability = 0; // Default value indicating failure to retrieve availability

            // Construct the SQL query to retrieve availability for the specified zone
            string query = "SELECT Availability FROM ZonesMotorTable WHERE ID = @zoneNumber";

            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Add parameter for the zone number
                    cmd.Parameters.AddWithValue("@zoneNumber", zoneNumber);


                    myConn.Open();
                    object result = cmd.ExecuteScalar();

                    // Check if the result is not null and convert it to an integer
                    if (result != null && result != DBNull.Value)
                    {
                        availability = Convert.ToInt32(result);
                    }


                    myConn.Close();

                }
            }


            return availability;
        }
        private void ResetAllParkingZones_Click(object sender, EventArgs e)
        {
            // Reset parking zones in ZonesTable
            ResetParkingZones("ZonesTable");

            // Reset parking zones in ZonesMotorTable
            ResetParkingZones("ZonesMotorTable");

            MessageBox.Show("All parking zones reset successfully.");
            LoadMotorParkingZoneColors();
            LoadParkingZoneColors();
        }

        private void ResetParkingZones(string tableName)
        {
            string query = $"UPDATE {tableName} SET Availability = 0";

            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                try
                {
                    myConn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    myConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error resetting parking zones in {tableName}: {ex.Message}");
                }
            }
        }

        private void parkingPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Zone21_Click(object sender, EventArgs e)
        {

        }

        private void Save_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter =
            "XLS (*.xls)|*.xls|" +
            "XLT (*.xlt)|*.xlt|" +
            "XLSX (*.xlsx)|*.xlsx|" +
            "XLSM (*.xlsm)|*.xlsm|" +
            "XLTX (*.xltx)|*.xltx|" +
            "XLTM (*.xltm)|*.xltm|" +
            "ODS (*.ods)|*.ods|" +
            "OTS (*.ots)|*.ots|" +
            "CSV (*.csv)|*.csv|" +
            "TSV (*.tsv)|*.tsv|" +
            "HTML (*.html)|*.html|" +
            "MHTML (.mhtml)|*.mhtml|" +
            "PDF (*.pdf)|*.pdf|" +
            "XPS (*.xps)|*.xps|" +
            "BMP (*.bmp)|*.bmp|" +
            "GIF (*.gif)|*.gif|" +
            "JPEG (*.jpg)|*.jpg|" +
            "PNG (*.png)|*.png|" +
            "TIFF (*.tif)|*.tif|" +
            "WMP (*.wdp)|*.wdp";
            saveFileDialog.FilterIndex = 3;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var workbook = new ExcelFile();
                var worksheet = workbook.Worksheets.Add("Sheet1");
                // From DataGridView to ExcelFile.
                DataGridViewConverter.ImportFromDataGridView(
                worksheet,
                this.dgvVehicle,
                new ImportFromDataGridViewOptions() { ColumnHeaders = true });
                workbook.Save(saveFileDialog.FileName);
            }
        }

        private void dgvUserInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SaveGuest_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter =
            "XLS (*.xls)|*.xls|" +
            "XLT (*.xlt)|*.xlt|" +
            "XLSX (*.xlsx)|*.xlsx|" +
            "XLSM (*.xlsm)|*.xlsm|" +
            "XLTX (*.xltx)|*.xltx|" +
            "XLTM (*.xltm)|*.xltm|" +
            "ODS (*.ods)|*.ods|" +
            "OTS (*.ots)|*.ots|" +
            "CSV (*.csv)|*.csv|" +
            "TSV (*.tsv)|*.tsv|" +
            "HTML (*.html)|*.html|" +
            "MHTML (.mhtml)|*.mhtml|" +
            "PDF (*.pdf)|*.pdf|" +
            "XPS (*.xps)|*.xps|" +
            "BMP (*.bmp)|*.bmp|" +
            "GIF (*.gif)|*.gif|" +
            "JPEG (*.jpg)|*.jpg|" +
            "PNG (*.png)|*.png|" +
            "TIFF (*.tif)|*.tif|" +
            "WMP (*.wdp)|*.wdp";
            saveFileDialog.FilterIndex = 3;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var workbook = new ExcelFile();
                var worksheet = workbook.Worksheets.Add("Sheet1");
                // From DataGridView to ExcelFile.
                DataGridViewConverter.ImportFromDataGridView(
                worksheet,
                this.dgvUserInfo,
                new ImportFromDataGridViewOptions() { ColumnHeaders = true });
                workbook.Save(saveFileDialog.FileName);
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        private void dgvVehicle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
  
            int dgvVehicleindex = e.RowIndex;
            DataGridViewRow row = dgvVehicle.Rows[dgvVehicleindex];
            string ID = row.Cells[0].Value.ToString(); // Corrected the way to get cell value

            string FirstName, LastName, Contact, Gender, Address, ParkingZone, Age, LicensePlate;
            string query = "SELECT FirstName,LastName,[Contact#],Gender,Address,ParkingZone,Age FROM UserInfo WHERE ID = @ID";
            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    // Add parameter for the zone number
                    cmd.Parameters.AddWithValue("@ID",ID );


                    myConn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FirstName = reader["FirstName"].ToString();
                            LastName = reader["LastName"].ToString();
                            Contact = reader["Contact#"].ToString();
                            Gender = reader["Gender"].ToString();
                            Address = reader["Address"].ToString();
                            ParkingZone = reader["ParkingZone"].ToString();
                            Age = reader["Age"].ToString();
                            // Assuming LicensePlate is a field in your DataGridView, otherwise, you need to fetch it from somewhere else
                            LicensePlate = row.Cells["LicensePlate"].Value.ToString();

                            // Now you have all the information, you can display it in a message box or use it as needed
                            MessageBox.Show($"First Name: {FirstName}\nLast Name: {LastName}\nContact: {Contact}\nGender: {Gender}\nAddress: {Address}\nParking Zone: {ParkingZone}\nAge: {Age}\nLicense Plate: {LicensePlate}");
                        }
                        else
                        {
                            MessageBox.Show("User not found!");
                        }
                    }
                    myConn.Close();

                }
            }
        }

        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
