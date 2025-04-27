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
using System.Globalization;

namespace test
{
    public partial class LogActivityPanel : Form
    {
        public LogActivityPanel()
        {
            InitializeComponent();
        }

        private void munculData()
        {
            Connector kon = new Connector();
            SqlConnection con = kon.getCon();

            con.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT tbl_log.id_log as ID, tbl_user.username as Username, tbl_log.waktu as WAKTU, tbl_log.aktivitas as AKTIVITAS from tbl_log join tbl_user on tbl_user.id_user = tbl_log.id_user", con);

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "aktivitas");
                setupDataGrid();
                dataGridView1.DataSource = ds;
                dataGridView1.Columns["ID"].DisplayIndex = 0;
                dataGridView1.Columns["Username"].DisplayIndex = 1;
                dataGridView1.Columns["WAKTU"].DisplayIndex = 2;
                dataGridView1.Columns["AKTIVITAS"].DisplayIndex = 3;
                dataGridView1.DataMember = "aktivitas";
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.Refresh();
            } catch (Exception er) {
                MessageBox.Show("Error: " + er);
            }
            finally
            {
                con.Close();
            }

        }

        private void setupDataGrid()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "id",
                DisplayIndex = 0
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Username",
                HeaderText = "Username",
                DataPropertyName = "username",
                DisplayIndex = 1
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "WAKTU",
                HeaderText = "WAKTU",
                DataPropertyName = "waktu",
                DisplayIndex = 2
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "AKTIVITAS",
                HeaderText = "AKTIVITAS",
                DataPropertyName = "aktivitas",
                DisplayIndex = 3
            });

        }

        private void LogActivityPanel_Load(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            var indo = new CultureInfo("id-ID");
            label2.Text = now.ToString("dddd, dd MMMM yyyy", indo);
            munculData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime tglAwal = dateTimePicker1.Value.Date;
            DateTime tglAkhir = dateTimePicker2.Value.Date;
            if(tglAkhir < tglAwal)
            {
                MessageBox.Show("Tanggal akhir tidak boleh lebih kecil dari tanggal awal", "peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Connector kon = new Connector();
            SqlConnection con = kon.getCon();

            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT tbl_log.id_log as ID, tbl_user.username as Username, tbl_log.waktu as WAKTU, tbl_log.aktivitas as AKTIVITAS from tbl_log join tbl_user on tbl_user.id_user = tbl_log.id_user where CONVERT(date, tbl_log.waktu) between @tgla and @tglb", con);
                cmd.Parameters.AddWithValue("@tgla", tglAwal);
                cmd.Parameters.AddWithValue("@tglb", tglAkhir);

                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                if (reader.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    dataGridView1.Columns["ID"].DisplayIndex = 0;
                    dataGridView1.Columns["Username"].DisplayIndex = 1;
                    dataGridView1.Columns["WAKTU"].DisplayIndex = 2;
                    dataGridView1.Columns["AKTIVITAS"].DisplayIndex = 3;
                    dataGridView1.DataSource = dt;
                }
                else
                {
                    dataGridView1.DataSource = null;
                }

                dataGridView1.Refresh();
            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + er);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
