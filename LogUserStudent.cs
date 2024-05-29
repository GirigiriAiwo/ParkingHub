using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Data.OleDb;
using QRCoder;
using static QRCoder.PayloadGenerator;

namespace Project_Reset
{
    public partial class LogUserStudent : Form
    {
        private int userID;
        private string username;

        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\OOP2\\CLOSE 2\\CLOSE\\abapo\\LG2\\LETS GO INTEGRATE\\DB\\Database\\ProjectDatabase2.mdb";
        public bool LoginSuccess { get; private set; }
        public LogUserStudent(int UserID, string UserName)
        {
            InitializeComponent();
            LoadParkingZoneColors();
            LoadMotorParkingZoneColors();
            this.userID = UserID;
            this.username = UserName;
        }

        private void LogUserStudent_Load(object sender, EventArgs e)
        {
            loadQRcode();
        }

        
        private void loadQRcode()
        {
            userlbl.Text = Convert.ToString(username);
            lblID.Text = Convert.ToString(userID);
            // Assuming 'Register' is a form where user input QR code data, and you want to retrieve it.
            // You should make sure 'text' is properly set in the Register form.
            Register register = new Register();

            // Assuming QR is a property or field in Register form where you store the QR code data.
            string text = register.QR;
            // If 'text' is not null or empty, generate the QR code.
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(Convert.ToString(userID), QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            pictureBox1.Image = code.GetGraphic(5);
        }
        private void userlbl_Click(object sender, EventArgs e)
        {

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

        private void UpdateBtn_Click(object sender, EventArgs e)
        {


            string queryUserInfo = "UPDATE UserInfo SET FirstName = @FN, LastName = @LN, Address = @address, [Contact#] = @contact WHERE ID = @userID";
            string queryUsersTable = "UPDATE UsersTable SET [Username] = @username, [Password] = @password WHERE ID = @userID";

            // Check if all textboxes are not null or empty
            if (!string.IsNullOrEmpty(tbxFirstName.Text) &&
                !string.IsNullOrEmpty(tbxLastName.Text) &&
                !string.IsNullOrEmpty(tbxAddress.Text) &&
                !string.IsNullOrEmpty(tbxContact.Text) &&
                !string.IsNullOrEmpty(tbxUsername.Text) &&
                !string.IsNullOrEmpty(tbxPassword.Text))
            {
                using (OleDbConnection myConn = new OleDbConnection(connectionString))
                {
                    myConn.Open();

                    // Update UserInfo table
                    using (OleDbCommand cmdUserInfo = new OleDbCommand(queryUserInfo, myConn))
                    {
                        cmdUserInfo.Parameters.AddWithValue("@FN", tbxFirstName.Text);
                        cmdUserInfo.Parameters.AddWithValue("@LN", tbxLastName.Text);
                        cmdUserInfo.Parameters.AddWithValue("@address", tbxAddress.Text);
                        cmdUserInfo.Parameters.AddWithValue("@contact", tbxContact.Text);
                        cmdUserInfo.Parameters.AddWithValue("@userID", userID);
                        cmdUserInfo.ExecuteNonQuery();
                    }

                    // Update UsersTable
                    using (OleDbCommand cmdUsersTable = new OleDbCommand(queryUsersTable, myConn))
                    {
                        cmdUsersTable.Parameters.AddWithValue("@username", tbxUsername.Text);
                        cmdUsersTable.Parameters.AddWithValue("@password", tbxPassword.Text);
                        cmdUsersTable.Parameters.AddWithValue("@userID", userID);
                        cmdUsersTable.ExecuteNonQuery();
                    }

                    myConn.Close();
                }

                MessageBox.Show("Successfully Updated");
            }
            else
            {
                MessageBox.Show("Please fill in all fields before updating.");
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }

}
