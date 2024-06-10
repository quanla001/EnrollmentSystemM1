using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Net;

namespace EnrollmentSystemM1
{
    public partial class RegisterInformation : Form
    {
        private const string connectionString = "Data Source=BINHQUAN\\BINHQUAN;Initial Catalog=EnrollmentSystemASM2;" +
                                                "Integrated Security=True";
        private string selectedImagePath = string.Empty;

        public RegisterInformation()
        {
            InitializeComponent();
            LoadComboBox();

        }

        private void RegisterInformation_Load(object sender, EventArgs e)
        {

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Validate input before proceeding
            if (ValidateInput())
            {
                // Get values from TextBoxes
                string studentName = txtStudentName.Text;
                DateTime dateOfBirth = dtpDateOfBirth.Value;
                string gender = GetSelectedGender();
                string address = txtAddress.Text;
                string major = cbMajor.Text;
                string phonenumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string avatarPath = selectedImagePath;

                // Add the item to the database
                AddItemToDatabase(studentName, dateOfBirth, gender, address, major, phonenumber, email, avatarPath);

            }
        }
        private void AddItemToDatabase(string studentName, DateTime dateOfBirth, string gender, string address, 
                                        string major, string phoneNumber, string email, string avatarPath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Students (StudentName, DateOfBirth, Gender, Address, Major, PhoneNumber, Email, Avatar) " +
                               "VALUES (@StudentName, @DateOfBirth, @Gender, @Address, @Major, @PhoneNumber, @Email, @Avatar)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@StudentName", SqlDbType.NVarChar).Value = studentName;
                    command.Parameters.AddWithValue("@DateOfBirth", SqlDbType.Date).Value = dateOfBirth;
                    command.Parameters.AddWithValue("@Gender", SqlDbType.NVarChar).Value = gender;
                    command.Parameters.AddWithValue("@Address", SqlDbType.NVarChar).Value = address;
                    command.Parameters.AddWithValue("@Major", SqlDbType.NVarChar).Value = major;
                    command.Parameters.AddWithValue("@PhoneNumber", SqlDbType.NVarChar).Value = phoneNumber;
                    command.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = email;
                    command.Parameters.AddWithValue("@Avatar", SqlDbType.NVarChar).Value = avatarPath;

                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Register successfully!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtStudentName.ReadOnly = true;
                txtAddress.ReadOnly = true;
                txtPhoneNumber.ReadOnly = true;
                txtEmail.ReadOnly = true;
                radMale.Enabled = false;
                radFemale.Enabled = false;
                dtpDateOfBirth.Enabled = false;
                cbMajor.Enabled = false;
            }
        }
        private bool ValidateInput()
        {
           
            // Check if other required fields are not empty
            if (string.IsNullOrWhiteSpace(txtStudentName.Text))
            {
                MessageBox.Show("Please enter a valid student name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!(radMale.Checked || radFemale.Checked))
            {
                MessageBox.Show("Please select a gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter a valid address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Please enter a valid PhoneNumber.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true; // If all validation checks pass
        }
        private string GetSelectedGender()
        {
            // Determine the selected gender based on the RadioButton controls
            if (radMale.Checked)
            {
                return "Male";
            }
            else if (radFemale.Checked)
            {
                return "Female";
            }
            else
            {
                return string.Empty;
            }
        }
        private void LoadComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT MajorName FROM Major";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string optionName = reader["MajorName"].ToString();
                                cbMajor.Items.Add(optionName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading ComboBox data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowserImage_Click(object sender, EventArgs e)
        {
            // Open a file dialog to select an image
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Display the selected image in the PictureBox
                    selectedImagePath = openFileDialog.FileName;
                    pictureBox1.Image = new System.Drawing.Bitmap(selectedImagePath);
                    txtAvatar.Text = selectedImagePath;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string id = txtID.Text;
            string studentname = txtStudentName.Text;
            txtStudentName.ReadOnly = false;
            txtAddress.ReadOnly = false;
            txtPhoneNumber.ReadOnly = false;
            txtEmail.ReadOnly = false;
            radMale.Enabled = true;
            radFemale.Enabled = true;
            dtpDateOfBirth.Enabled = true;
            cbMajor.Enabled = true;
            LoadDataIntoTextBox(id, studentname);

        }
        private void LoadDataIntoTextBox(string id, string studentname)
        {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id FROM Students WHERE StudentName = @StudentName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StudentName", studentname);
                        SqlDataReader da = command.ExecuteReader();
                        while (da.Read())
                        {
                            txtID.Text = da.GetValue(0).ToString();
                        }

                    }
                }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
             // Get data form TextBox
            string id = txtID.Text;
            string newStudentName = txtStudentName.Text;
            DateTime newdateOfBirth = dtpDateOfBirth.Value;
            string newgender = GetSelectedGender();
            string newAddress = txtAddress.Text;
            string newMajor = cbMajor.Text;
            string newPhoneNumber = txtPhoneNumber.Text;
            string newEmail = txtEmail.Text;
            string avatarpath = txtAvatar.Text;
            // Connect to SQL and updating the information
            UpdateStudentName(id, newStudentName, newdateOfBirth, newgender, newAddress, newMajor, newPhoneNumber, newEmail, avatarpath);
            txtStudentName.ReadOnly = true;
            txtAddress.ReadOnly = true;
            txtPhoneNumber.ReadOnly = true;
            txtEmail.ReadOnly = true;
            radMale.Enabled = false;
            radFemale.Enabled = false;
            dtpDateOfBirth.Enabled = false;
            cbMajor.Enabled = false;
        }
        private void UpdateStudentName(string id, string newStudentName, DateTime newdateOfBirth, string newgender, string newAddress, string newMajor, string newPhoneNumber, string newEmail, string avatarpath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Students SET StudentName = @NewStudentName, DateOfBirth = @NewDateOfBirth, " +
                               "Gender = @NewGender, Address = @NewAddress, Major = @NewMajor, PhoneNumber = @NewPhoneNumber, Email = @NewEmail, Avatar = @NewAvatar WHERE id = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", SqlDbType.Int).Value = id;
                        command.Parameters.AddWithValue("@NewStudentName", SqlDbType.NVarChar).Value = newStudentName;
                        command.Parameters.AddWithValue("@NewDateOfBirth", SqlDbType.Date).Value = newdateOfBirth;
                        command.Parameters.AddWithValue("@NewGender", SqlDbType.NVarChar).Value = newgender;
                        command.Parameters.AddWithValue("@NewAddress", SqlDbType.NVarChar).Value = newAddress;
                        command.Parameters.AddWithValue("@NewMajor", SqlDbType.NVarChar).Value = newMajor;
                        command.Parameters.AddWithValue("@NewPhoneNumber", SqlDbType.NVarChar).Value = newPhoneNumber;
                        command.Parameters.AddWithValue("@NewEmail", SqlDbType.NVarChar).Value = newEmail;
                        command.Parameters.AddWithValue("@NewAvatar", SqlDbType.NVarChar).Value = avatarpath;



                        command.ExecuteNonQuery();

                        MessageBox.Show("Save successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;

            }
        }

        private void cbMajor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || (e.KeyChar == '.')))
            {
                e.Handled = true;
            }
        }
    }
}
