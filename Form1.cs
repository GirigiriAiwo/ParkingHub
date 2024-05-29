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
    public partial class Form1 : Form
    {

        int check = 1;
        OleDbConnection myConn;
        OleDbDataAdapter da;
        OleDbCommand cmd;
        DataSet ds;
        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\OOP2\\CLOSE 2\\CLOSE\\abapo\\LG2\\LETS GO INTEGRATE\\DB\\Database\\ProjectDatabase2.mdb";
        public bool LoginSuccess { get; private set; }
        public bool AdminLoginSuccess { get; private set; }
        public int UserID { get; private set; }
        public string UserName { get; private set; }
        public Form1()
        {
            InitializeComponent();
        }
        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                txtPassword.Focus();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                button1.PerformClick();
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void testCon_Click(object sender, EventArgs e)
        {
            myConn = new OleDbConnection(connectionString);
            ds = new DataSet();
            myConn.Open();
            System.Windows.Forms.MessageBox.Show("Connected successfully!");
            myConn.Close();
        }

        private void Login()
        {
            if (check == 1)
            {


                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Replace the connection string with your database connection string
                //string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\CIT-U 3RD YEAR\\2ND SEM\\Object Oriented Programming 2\\LETS GO INTEGRATE\\DB\\Database\\ProjectDatabase.mdb;";

                using (OleDbConnection myConn = new OleDbConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM UsersTable WHERE Username = @Username AND Password = @Password";
                    using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        myConn.Open();
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Login successful!");
                            LoginSuccess = true;

                            string query2 = "SELECT ID FROM UsersTable WHERE Username = @Username AND Password = @Password";
                            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
                            {
                                // Execute the second query to get the UserID
                                cmd2.Parameters.AddWithValue("@Username", username);
                                cmd2.Parameters.AddWithValue("@Password", password);
                                object result = cmd2.ExecuteScalar();

                                // Check if a result was found
                                if (result != null)
                                {
                                    UserID = Convert.ToInt32(result); // Assuming UserID is an integer
                                }
                            }
                            string query3 = "SELECT Username FROM UsersTable WHERE Username = @Username AND Password = @Password";
                            using (OleDbCommand cmd2 = new OleDbCommand(query3, myConn))
                            {
                                // Execute the second query to get the UserID
                                cmd2.Parameters.AddWithValue("@Username", username);
                                cmd2.Parameters.AddWithValue("@Password", password);
                                object result2 = cmd2.ExecuteScalar();

                                // Check if a result was found
                                if (result2 != null)
                                {
                                    UserName = Convert.ToString(result2); // Assuming UserID is an integer
                                }
                            }

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password. Please try again.");
                        }
                    }
                }

            }
            else if (check == 0)
            {


                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Replace the connection string with your database connection string
                //string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\CIT-U 3RD YEAR\\2ND SEM\\Object Oriented Programming 2\\LETS GO INTEGRATE\\DB\\Database\\ProjectDatabase.mdb;";

                using (myConn = new OleDbConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM UserAdmin WHERE Username = @Username AND Password = @Password";
                    using (cmd = new OleDbCommand(query, myConn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        myConn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        myConn.Close();

                        if (count > 0)
                        {
                            MessageBox.Show("Login successful!");
                            AdminLoginSuccess = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password. Please try again.");
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Register register = new Register();
            register.Show();

        }

        private void logInStudent_Click(object sender, EventArgs e)
        {
            logInAdmin.BackColor = Color.Black;
            check = 1;
            logInStudent.BackColor = Color.FromArgb(255, 128, 0);
        }

        private void logInAdmin_Click(object sender, EventArgs e)
        {
            logInStudent.BackColor = Color.Black;
            check = 0;
            logInAdmin.BackColor = Color.FromArgb(255, 128, 0);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }
    }
}
