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
using System.Windows.Forms.DataVisualization.Charting;

namespace test
{
    public partial class LaporanPanel : Form
    {
        public LaporanPanel()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void setupDataGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { 
                Name = "No Transaksi",
                HeaderText = "No Transaksi",
                DataPropertyName = "tbl_transaksi.no_transaksi",
                DisplayIndex = 0
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Tanggal Transaksi",
                HeaderText = "Tanggal Transaksi",
                DataPropertyName = "tbl_transaksi.tgl_transaksi",
                DisplayIndex = 1
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Total Penjualan",
                HeaderText = "Total Penjualan",
                DataPropertyName = "tbl_transaksi.total_bayar",
                DisplayIndex = 2
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Nama Kasir",
                HeaderText = "Nama Kasir",
                DataPropertyName = "tbl_user.nama",
                DisplayIndex = 3
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Nama Pelanggan",
                HeaderText = "Nama Pelanggan",
                DataPropertyName = "tbl_pelanggan.nama",
                DisplayIndex = 4
            });
        }

        private void munculData()
        {
            Connector kon = new Connector();

            SqlConnection con = kon.getCon();

            con.Open();

            SqlCommand cmd = new SqlCommand(
        @"SELECT tbl_transaksi.no_transaksi AS 'No Transaksi', 
                 tbl_transaksi.tgl_transaksi AS 'Tanggal Transaksi', 
                 tbl_transaksi.total_bayar AS 'Total Penjualan', 
                 tbl_user.nama AS 'Nama Kasir', 
                 tbl_pelanggan.nama AS 'Nama Pelanggan' 
          FROM tbl_transaksi 
          JOIN tbl_user ON tbl_transaksi.id_user = tbl_user.id_user 
          JOIN tbl_pelanggan ON tbl_transaksi.id_pelanggan = tbl_pelanggan.id_pelanggan", con);


            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds, "tbl_transaksi");

            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "tbl_transaksi";
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Refresh();
        }
        private void LaporanPanel_Load(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            munculData();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.ColumnIndex == 2)
            {
                if (int.TryParse(e.Value.ToString(), out _)) {
                    e.Value = string.Format("Rp. {0:#,##0}", e.Value);
                    e.FormattingApplied = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime date1 = dateTimePicker1.Value.Date;
            DateTime date2 = dateTimePicker2.Value.Date;

            if (date2 < date1)
            {
                MessageBox.Show("tanggal ke 2 tidak boleh lebih kecil dari tanggal ke 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Connector kon = new Connector();

            SqlConnection con = kon.getCon();

            con.Open();

            SqlCommand cmd = new SqlCommand(
        @"SELECT tbl_transaksi.no_transaksi AS 'No Transaksi', 
                 tbl_transaksi.tgl_transaksi AS 'Tanggal Transaksi', 
                 tbl_transaksi.total_bayar AS 'Total Penjualan', 
                 tbl_user.nama AS 'Nama Kasir', 
                 tbl_pelanggan.nama AS 'Nama Pelanggan' 
          FROM tbl_transaksi 
          JOIN tbl_user ON tbl_transaksi.id_user = tbl_user.id_user 
          JOIN tbl_pelanggan ON tbl_transaksi.id_pelanggan = tbl_pelanggan.id_pelanggan where convert(date, tbl_transaksi.tgl_transaksi) between @tgl1 and @tgl2", con);

            cmd.Parameters.AddWithValue("@tgl1", date1);
            cmd.Parameters.AddWithValue("@tgl2", date2);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds, "tbl_transaksi");

            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "tbl_transaksi";
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime date1 = dateTimePicker1.Value.Date;
            DateTime date2 = dateTimePicker2.Value.Date;

            Connector kon = new Connector();

            SqlConnection con = kon.getCon();

            con.Open();

            SqlCommand cmd = new SqlCommand("select tgl_transaksi, total_bayar from tbl_transaksi where tgl_transaksi between @tgl1 and @tgl2", con);

            cmd.Parameters.AddWithValue("@tgl1", date1);
            cmd.Parameters.AddWithValue("@tgl2", date2);

            chart1.Series.Clear();
            Series series = chart1.Series.Add("omset");
            series.ChartType = SeriesChartType.Column;

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime tgl = reader.GetDateTime(0);
                long total = reader.GetInt64(1);

                series.Points.AddXY(tgl, total);
            }
        }
    }
}
