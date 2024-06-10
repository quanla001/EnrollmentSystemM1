using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnrollmentSystemM1
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Data Source=BINHQUAN\\BINHQUAN;Initial Catalog=EnrollmentSystemASM2;Integrated Security=True";
        public LoginForm()
        {
            InitializeComponent();
        }
        
        private void btnlogin_Click(object sender, EventArgs e)
        {
            // Retrieve the entered username and password
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            MessageBox.Show("Login Successfully!!", "Login Completed", MessageBoxButtons.OK, MessageBoxIcon.None);
            // Perform login and open the appropriate form
            Login(username, password);

        }
        public void Login(string username, string password)
        {
            string userRole = GetUserRoleFromDatabase(username, password);

            if (userRole != null)
            {
                OpenFormBasedOnRole(userRole);
            }
            else
            {
                MessageBox.Show("Invalid credentials. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenFormBasedOnRole(string userRole)
        {
            // switch-case in login authentication
            switch (userRole)
            {
                case "Admin":
                    OpenAdminForm();
                    break;
                case "Student":
                    OpenStudentForm();
                    break;
                case "Guest":
                    OpenGuestForm();
                    break;
                default:
                    MessageBox.Show("Unknown user role. Cannot determine the appropriate form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        public string GetUserRoleFromDatabase(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT UserType FROM Users WHERE Username = @Username AND Password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    object result = command.ExecuteScalar();

                    return result != null ? result.ToString() : null;
                }
            }
        }
        private void OpenAdminForm()
        {
            // Code to open the Admin form
            FormDasboard adminForm = new FormDasboard();
            adminForm.Show();
            this.Hide();
        }

        private void OpenStudentForm()
        {
            // Code to open the Student form
             StudentForm studentForm = new StudentForm();
             studentForm.Show();
            this.Hide();
        }

        private void OpenGuestForm()
        {
            // Code to open the Guest form
            RegisterInformation guestForm = new RegisterInformation();
            guestForm.Show();
            this.Hide();
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm register = new RegisterForm();
            register.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
