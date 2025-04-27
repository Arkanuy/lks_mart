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
                @"SELECT tbl_transaksi.no_transaksi as 'No Transaksi', 
                         tbl_transaksi.tgl_transaksi as 'Tanggal Transaksi', 
                         tbl_transaksi.total_bayar as 'Total Penjualan', 
                         tbl_transaksi.nama_kasir as 'Nama Kasir',
                         tbl_pelanggan.nama as 'Nama Pelanggan' from tbl_transaksi
                 JOIN tbl_pelanggan on tbl_pelanggan.id_pelanggan = tbl_transaksi.id_pelanggan"
            , con);

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
            DateTime tgl1 = dateTimePicker1.Value.Date;
            DateTime tgl2 = dateTimePicker2.Value.Date;

            if(tgl2 < tgl1)
            {
                MessageBox.Show("Tgl 2 tidak boleh lebih kecil dari tanggal 1");
                return;
            }

            Connector kon = new Connector();

            SqlConnection con = kon.getCon();
            con.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT tbl_transaksi.no_transaksi as 'No Transaksi', 
                         tbl_transaksi.tgl_transaksi as 'Tanggal Transaksi', 
                         tbl_transaksi.total_bayar as 'Total Penjualan', 
                         tbl_transaksi.nama_kasir as 'Nama Kasir',
                         tbl_pelanggan.nama as 'Nama Pelanggan' from tbl_transaksi
                 JOIN tbl_pelanggan on tbl_pelanggan.id_pelanggan = tbl_transaksi.id_pelanggan where tbl_transaksi.tgl_transaksi between @tgl1 and @tgl2"
            , con);

            cmd.Parameters.AddWithValue("@tgl1", tgl1);
            cmd.Parameters.AddWithValue("@tgl2", tgl2);


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
            DateTime tgl1 = dateTimePicker1.Value.Date;
            DateTime tgl2 = dateTimePicker2.Value.Date;

            if (tgl2 < tgl1)
            {
                MessageBox.Show("Tgl 2 tidak boleh lebih kecil dari tanggal 1");
                return;
            }

            Connector kon = new Connector();

            SqlConnection con = kon.getCon();
            con.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT tgl_transaksi, total_bayar from tbl_transaksi where tgl_transaksi between @tgl1 and @tgl2"
            , con);

            cmd.Parameters.AddWithValue("@tgl1", tgl1);
            cmd.Parameters.AddWithValue("@tgl2", tgl2);


            chart1.Series.Clear();
            Series series = chart1.Series.Add("Omset");
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
