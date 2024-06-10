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

namespace EnrollmentSystemM1
{
    public partial class StudentForm : Form
    {
        private const string connectionString = "Data Source=BINHQUAN\\BINHQUAN;Initial Catalog=EnrollmentSystemASM2;Integrated Security=True";

        public StudentForm()
        {
            InitializeComponent();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }
        private void LoadDataIntoDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Assuming you have a table named 'Students' in the database
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
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if the clicked cell is a valid cell (not a header and within the row count)
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count && e.ColumnIndex >= 0)
            {
                txtStudentCode.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                txtStudentName.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtGender.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                txtAddress.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                txtMajor.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
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
                string query = "SELECT * FROM Students WHERE StudentName LIKE @SearchKeyword OR StudentCode LIKE @SearchKeyword OR Major LIKE @SearchKeyword " +
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.PerformClick();
        }
    }
}
