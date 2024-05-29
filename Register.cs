using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Reset
{
    public partial class Register : Form
    {
        OleDbConnection myConn;
        OleDbDataAdapter da;
        OleDbCommand cmd;
        DataSet ds;
        public string QR { get; set; }
        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\OOP2\\CLOSE 2\\CLOSE\\abapo\\LG2\\LETS GO INTEGRATE\\DB\\Database\\ProjectDatabase2.mdb;";
        public Register()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Check if any of the textboxes are empty
            if (string.IsNullOrWhiteSpace(tbxUsername.Text) ||
                string.IsNullOrWhiteSpace(tbxPassword.Text) ||
                string.IsNullOrWhiteSpace(tbxStudentID.Text) ||
                string.IsNullOrWhiteSpace(tbxLastName.Text) ||
                string.IsNullOrWhiteSpace(tbxFirstName.Text) ||
                string.IsNullOrWhiteSpace(tbxAge.Text) ||
                string.IsNullOrWhiteSpace(tbxGender.Text) ||
                string.IsNullOrWhiteSpace(tbxAddress.Text) ||
                string.IsNullOrWhiteSpace(tbxContact.Text) ||
                string.IsNullOrWhiteSpace(tbxBrand.Text) ||
                string.IsNullOrWhiteSpace(tbxModel.Text) ||
                string.IsNullOrWhiteSpace(tbxColor.Text) ||
                string.IsNullOrWhiteSpace(tbxLicensePlate.Text) ||
                (!radioButton1.Checked && !radioButton2.Checked))
            {
                MessageBox.Show("Please fill in all fields.");
                return; // Exit the method without proceeding with registration
            }

            // If all fields are filled, proceed with registration
            using (OleDbConnection myConn = new OleDbConnection(connectionString))
            {
                myConn.Open();
                string currentDateTime = DateTime.Now.ToString();
                // Insert into Users table
                string query = "INSERT into UsersTable ([Username], [Password])  values (@Username, @Password)";
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@Username", tbxUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", tbxPassword.Text);
                    cmd.ExecuteNonQuery();
                }

                // Insert into UserInfo table
                string UserInfoquery = "INSERT INTO UserInfo ([StudentID],[LastName],[FirstName], [Age], [Gender], [Address], [Contact#],[VehicleID]) " +
                                       "VALUES (@StudentID, @Lname, @Fname, @Age, @Gender, @Address, @Contact,@VID)";
                using (OleDbCommand cmd = new OleDbCommand(UserInfoquery, myConn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", tbxStudentID.Text);
                    cmd.Parameters.AddWithValue("@Lname", tbxLastName.Text);
                    cmd.Parameters.AddWithValue("@Fname", tbxFirstName.Text);
                    cmd.Parameters.AddWithValue("@Age", tbxAge.Text);
                    cmd.Parameters.AddWithValue("@Gender", tbxGender.Text);
                    cmd.Parameters.AddWithValue("@Address", tbxAddress.Text);
                    cmd.Parameters.AddWithValue("@Contact", tbxContact.Text);
                    cmd.Parameters.AddWithValue("@VID", tbxStudentID.Text);
                    cmd.ExecuteNonQuery();
                }

                QR = tbxStudentID.Text;

                // Insert into UserCar table
                string UserCarquery = "INSERT INTO UserCar (CarBrand, CarModel, Color,LicensePlate,CarType) VALUES (@Brand, @Model, @Color,@LicensePlate,@CarType)";
                using (OleDbCommand cmd = new OleDbCommand(UserCarquery, myConn))
                {
                    string type;
                    cmd.Parameters.AddWithValue("@Brand", tbxBrand.Text);
                    cmd.Parameters.AddWithValue("@Model", tbxModel.Text);
                    cmd.Parameters.AddWithValue("@Color", tbxColor.Text);
                    cmd.Parameters.AddWithValue("@LicensePlate", tbxLicensePlate.Text);
                    if (radioButton1.Checked)
                        type = radioButton1.Text;
                    else
                        type = radioButton2.Text;
                    cmd.Parameters.AddWithValue("@CarType", type);
                    cmd.ExecuteNonQuery();
                }
                myConn.Close();
                myConn.Open();
                string VehicleInquery = "INSERT INTO VehicleIn (VehicleDateIn, VehicleIn) VALUES (@Date, @In)";
                using (OleDbCommand cmd = new OleDbCommand(VehicleInquery, myConn))
                {
                    cmd.Parameters.AddWithValue("@Date", currentDateTime);
                    cmd.Parameters.AddWithValue("@In", "Yes");
                    cmd.ExecuteNonQuery();

                }
                string VehicleOutquery = "INSERT INTO VehicleOut (VehicleDateOut, VehicleOut) VALUES (@Date, @Out)";
                using (OleDbCommand cmd = new OleDbCommand(VehicleOutquery, myConn))
                {
                    cmd.Parameters.AddWithValue("@Date", currentDateTime);
                    cmd.Parameters.AddWithValue("@Out", "Yes");
                    cmd.ExecuteNonQuery();
                }
                myConn.Close();
                ClearTextBoxes();
            }

            MessageBox.Show("Registration successful!");
        }

        private void ClearTextBoxes()
        {
            tbxUsername.Text = "";
            tbxPassword.Text = "";
            tbxLastName.Text = "";
            tbxFirstName.Text = "";
            tbxAge.Text = "";
            tbxGender.Text = "";
            tbxAddress.Text = "";
            tbxContact.Text = "";
            tbxBrand.Text = "";
            tbxModel.Text = "";
            tbxColor.Text = "";
            tbxLicensePlate.Text = "";
            radioButton1.Checked = false;
            radioButton2.Checked = false;
        }

        private void testCon_Click(object sender, EventArgs e)
        {
            myConn = new OleDbConnection(connectionString);
            ds = new DataSet();
            myConn.Open();
            System.Windows.Forms.MessageBox.Show("Connected successfully!");
            myConn.Close();
        }
    }
}
