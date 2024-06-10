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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace EnrollmentSystemM1
{
    public partial class FormDasboard : Form
    {
        private const string connectionString = "Data Source=BINHQUAN\\BINHQUAN;Initial Catalog=EnrollmentSystemASM2;Integrated Security=True";
        private string selectedImagePath = string.Empty;

        public FormDasboard()
        {
            InitializeComponent();

            // Load data into the DataGridView when the form is loaded
            LoadDataIntoDataGridView();

            // Wire up the Click event for the btnSearch button
            btnSearch.Click += btnSearch_Click;
            // Wire up the Click event for the btnDelete button
            LoadComboBox();
            LoadComboBoxAccount();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validate input before proceeding
            if (ValidateInput())
            {
                // Get values from TextBoxes
                string studentCode = txtStudentCode.Text;
                string studentName = txtStudentName.Text;
                DateTime dateOfBirth = dtpDateOfBirth.Value;
                string gender = GetSelectedGender();
                string address = txtAddress.Text;
                string major = cbMajor.Text;
                string phonenumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string avatarPath = selectedImagePath;

                // Add the item to the database
                AddItemToDatabase(studentCode, studentName, dateOfBirth, gender, address, major, phonenumber, email, avatarPath);

                // Refresh the data in the ListView and DataGridView
                LoadDataIntoDataGridView();

                // Clear input fields after adding
                ClearInputFields();

                MessageBox.Show("Successfully added.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.None);

            }
        }
        private void LoadDataIntoDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Students";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void AddItemToDatabase(string studentCode, string studentName, DateTime dateOfBirth, string gender, string address, 
                                        string major, string phoneNumber, string email, string avatarPath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Students (StudentCode, StudentName, DateOfBirth, Gender, Address, Major, PhoneNumber, Email, Avatar) " +
                               "VALUES (@StudentCode, @StudentName, @DateOfBirth, @Gender, @Address, @Major, @PhoneNumber, @Email, @Avatar)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentCode", SqlDbType.NVarChar).Value = studentCode;
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
            }
        }


        private void FormDasboard_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
            LoadDataAccountIntoDataGridView();
        }

        private void ClearInputFields()
        {
            // Clear the TextBox controls
            txtStudentCode.Clear();
            txtStudentName.Clear();
            // Add more TextBox controls if needed
            txtAddress.Clear();
            txtEmail.Clear();
            // Clear the DateTimePicker
            dtpDateOfBirth.Value = DateTime.Now;
            // Clear the RadioButton for Gender
            radMale.Checked = false;
            radFemale.Checked = false;

            // Clear the PictureBox and image path
            pictureBox1.Image = null;
            selectedImagePath = string.Empty;
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
        private bool ValidateInput()
        {
            // Check if Student ID is a valid integer
            if (string.IsNullOrWhiteSpace(txtStudentCode.Text))
            {
                MessageBox.Show("Invalid Student ID. Please enter a valid integer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Check if other required fields are not empty
            if (string.IsNullOrWhiteSpace(txtStudentName.Text))
            {
                MessageBox.Show("Please enter a valid student name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Check if other required fields are choosen
            if (!(radMale.Checked || radFemale.Checked))
            {
                MessageBox.Show("Please select a gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Check if other required fields are not empty
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter a valid address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Check if other required fields are not empty
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Please enter a valid PhoneNumber.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // Check if other required fields are not empty
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true; // If all validation checks pass
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Validate input before proceeding
            if (ValidateInput())
            {
                // Get values from TextBoxes
                string id = txtID.Text;
                string studentCode = txtStudentCode.Text;
                string studentName = txtStudentName.Text;

                // Example of using DateTimePicker for DateOfBirth
                DateTime dateOfBirth = dtpDateOfBirth.Value;

                string gender = GetSelectedGender();
                string address = txtAddress.Text;
                string major = cbMajor.Text;
                string phonenumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string avatarPath = txtAvatar.Text;
                // Update the item to the database
                UpdateItemToDatabase(id, studentCode, studentName, dateOfBirth, gender, address, major, phonenumber, email, avatarPath);

                LoadDataIntoDataGridView();

                ClearInputFields();

            }
            else
            {
                MessageBox.Show("Please select a row to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateItemToDatabase(string id, string studentCode, string studentName, DateTime dateOfBirth, string gender, string address, string major, 
                                            string phoneNumber, string email, string avatarPath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Students " +
                               "SET StudentCode = @StudentCode, StudentName = @StudentName, DateOfBirth = @DateOfBirth, " +
                               "Gender = @Gender, Address = @Address, Major = @Major, PhoneNumber = @PhoneNumber, Email = @Email, " +
                               "Avatar = @Avatar " +
                               "WHERE id = @id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", SqlDbType.Int).Value = id;
                    command.Parameters.AddWithValue("@StudentCode", SqlDbType.NVarChar).Value = studentCode;
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
                MessageBox.Show("Update successfully!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connection.Close();
            }
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if the clicked cell is a valid cell (not a header and within the row count)
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count && e.ColumnIndex >= 0)
            {
                txtID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                txtStudentCode.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtStudentName.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                dtpDateOfBirth.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                string gender = dataGridView1.Rows[e.RowIndex].Cells["Gender"].Value.ToString();

                if (gender == "Male")
                {
                    radMale.Checked = true;
                    radFemale.Checked = false;
                }
                else if (gender == "Female")
                {
                    radMale.Checked = false;
                    radFemale.Checked = true;
                }
                txtAddress.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                cbMajor.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
                txtPhoneNumber.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                txtEmail.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();
                txtAvatar.Text = dataGridView1.CurrentRow.Cells[9].Value.ToString();
                string imagePath = dataGridView1.Rows[e.RowIndex].Cells["Avatar"].Value.ToString();

                // Display the image in a PictureBox 
                if (!string.IsNullOrEmpty(imagePath))
                {
                    pictureBox1.Image = Image.FromFile(imagePath);
                }
                else
                {
                    // Handle the case when the image path is empty or null
                    pictureBox1.Image = null;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Clear the DataGridView
            dataGridView1.DataSource = null;

            // Call the search method with the entered search keyword
            SearchRecords(txtSearch.Text);
        }
        private void SearchRecords(string searchKeyword)
        {
            // Clear previous search results
            dataGridView1.DataSource = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Customize your query based on your search criteria
                string query = "SELECT * FROM Students WHERE StudentName LIKE @SearchKeyword OR Gender LIKE @SearchKeyword OR StudentCode LIKE @SearchKeyword OR " +
                    "Address LIKE @SearchKeyword OR Major LIKE @SearchKeyword " +
                               "OR PhoneNumber LIKE @SearchKeyword OR Email LIKE @SearchKeyword";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchKeyword", SqlDbType.NVarChar).Value = "%" + searchKeyword + "%";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Bind the results to the DataGridView
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Assuming 'id' is stored in the first column of the selected row
                    int selectedItemId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);

                    // Delete the corresponding record from the database
                    DeleteRecordFromDatabase(selectedItemId);

                    // Delete the selected row from the DataGridView
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
            }
        }
        private void DeleteRecordFromDatabase(int itemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 'Students' is the name of your table
                string query = "DELETE FROM Students WHERE id = @id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", itemId);

                    // Execute the query
                    command.ExecuteNonQuery();
                }
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
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private void LoadDataAccountIntoDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Assuming you have a table named 'Students' in the database
                string query = "SELECT * FROM Users";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView2.DataSource = dataTable;
                }
            }
        }
        private void btnAccAdd_Click(object sender, EventArgs e)
        {
            // Get values from TextBoxes
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string usertype = cbRoles.Text;

            // Add the item to the database
            AddItemToDatabaseAccount(username, password, usertype);

            // Refresh the data in the ListView and DataGridView
            LoadDataAccountIntoDataGridView();

            // Clear input fields after adding
            ClearInputFields();
        }
        private void AddItemToDatabaseAccount(string username, string password, string usertype)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Users (Username, Password, UserType) " +
                               "VALUES (@Username , @Password , @UserType)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", SqlDbType.NVarChar).Value = username;
                    command.Parameters.AddWithValue("@Password", SqlDbType.NVarChar).Value = password;
                    command.Parameters.AddWithValue("@UserType", SqlDbType.NVarChar).Value = usertype;
                    command.ExecuteNonQuery();
                }
            }
        }
        private void LoadComboBoxAccount()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Role FROM Roles " +
                                   "WHERE id < 3";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string optionName = reader["Role"].ToString();
                                cbRoles.Items.Add(optionName);
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
        private void btnAccUpdate_Click(object sender, EventArgs e)
        {
            // Get values from TextBoxes
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string usertype = cbRoles.Text;

            // Add the item to the database
            UpdateItemToDatabaseAccount(username, password, usertype);

            // Refresh the data in the ListView and DataGridView
            LoadDataAccountIntoDataGridView();

            // Clear input fields after adding
            ClearInputFields();

            MessageBox.Show("Update successfully!!.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void UpdateItemToDatabaseAccount(string username, string password, string usertype)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Users " +
                               "SET Username = @Username, Password = @Password, UserType = @UserType " +
                               "WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", SqlDbType.NVarChar).Value = username;
                    command.Parameters.AddWithValue("@Password", SqlDbType.NVarChar).Value = password;
                    command.Parameters.AddWithValue("@UserType", SqlDbType.NVarChar).Value = usertype;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            txtUsername.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            txtPassword.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            cbRoles.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();

        }

        private void btnAccDelete_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this account?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int accountid = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value);

                    // Delete the corresponding record from the database
                    DeleteAccountFromDatabase(accountid);

                    // Delete the selected row from the DataGridView
                    dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private void DeleteAccountFromDatabase(int accountid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 'Students' is the name of your table
                string query = "DELETE FROM Users WHERE UserId = @id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", accountid);

                    // Execute the query
                    command.ExecuteNonQuery();
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
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

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.PerformClick();
        }
    }
}

