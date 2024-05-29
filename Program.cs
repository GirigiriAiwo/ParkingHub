using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Reset
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            Application.Run(form1);
            bool check = form1.LoginSuccess;
            bool Admincheck = form1.AdminLoginSuccess;
            if(check == true)
            {
                int userId = form1.UserID;
                string UserName = form1.UserName;
                Application.Run(new LogUserStudent(userId,UserName));
                form1.Close();
            }
            else if(Admincheck == true)
            {
                Application.Run(new Menu());
                form1.Close();
            }
       
        }
    }
}
