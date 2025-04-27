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

namespace test
{
    public partial class MainKasir : Form
    {
        public MainKasir()
        {
            InitializeComponent();
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin untuk Logout?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();
                LoginForm form = new LoginForm();

                try
                {
                    DateTime now = DateTime.Now;
                    SqlCommand cmd = new SqlCommand("insert into tbl_log (id_user, waktu, aktivitas) select tbl_user.id_user, @waktu, @akt from tbl_user where tbl_user.username = @uname", con);
                    cmd.Parameters.AddWithValue("@uname", LoginForm.username);
                    cmd.Parameters.AddWithValue("@waktu", now);
                    cmd.Parameters.AddWithValue("akt", "Logout");

                    cmd.ExecuteNonQuery();

                }
                catch (Exception er)
                {
                    MessageBox.Show("Error: " + er);
                }
                finally
                {
                    this.Hide();
                    form.Show();
                    con.Close();
                }
            }
        }

        private void munculItem()
        {
            Connector kon = new Connector();

            SqlConnection con = kon.getCon();

            con.Open();
            SqlCommand cmd = new SqlCommand("select kode_barang, nama_barang from tbl_barang", con);
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetString(0) + " - " + reader.GetString(1));
            }
            con.Close();
        }

        private void MainKasir_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Kode - Menu");
            munculItem();
            hargaBox.ReadOnly = true;
            totalBox.ReadOnly = true;
            reset();
            label10.Text = LoginForm.username;
            setupKeranjang();
        }

        private void reset()
        {
            comboBox1.SelectedIndex = 0;
            hargaBox.Text = "Rp. ";
            qtyBox.Text = "";
            totalBox.Text = "Rp. ";
        }

        private void comboBox1_ValueMemberChanged(object sender, EventArgs e)
        {
            string[] parts = (comboBox1.SelectedItem?.ToString() ?? "").Split(new[] {" - "}, StringSplitOptions.None);

            Connector kon = new Connector();

            SqlConnection con = kon.getCon();

            SqlCommand cmd = new SqlCommand("select harga_satuan from tbl_barang where kode_barang = '" + parts[0] + "'", con);


            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            if (reader.HasRows)
            {
                hargaBox.Text = string.Format("Rp. {0:#,##0}", reader["harga_satuan"].ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] parts = (comboBox1.SelectedItem?.ToString() ?? "").Split(new[] { " - " }, StringSplitOptions.None);

            Connector kon = new Connector();

            SqlConnection con = kon.getCon();
            con.Open();
            SqlCommand cmd = new SqlCommand("select harga_satuan from tbl_barang where kode_barang = '" + parts[0] + "'", con);


            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            if (reader.HasRows)
            {
                hargaBox.Text = string.Format("Rp. {0:#,##0}", reader["harga_satuan"]);
            }
        }

        private void qtyBox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(qtyBox.Text, out _))
            {
                long hargaSatuan = long.Parse(hargaBox.Text.Replace("Rp. ", "").Replace(".", ""));

                long qty = long.Parse(qtyBox.Text);

                long totalHarga = hargaSatuan * qty;

                totalBox.Text = string.Format("Rp. {0:#,##0}", totalHarga);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void setupKeranjang()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "No Transaksi",
                HeaderText = "No Transaksi",
                DataPropertyName = "no_transaksi",
                DisplayIndex = 0
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Kode Barang",
                HeaderText = "Kode Barang",
                DataPropertyName = "kode_barang",
                DisplayIndex = 1
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Nama Barang",
                HeaderText = "Nama Barang",
                DataPropertyName = "nama_barang",
                DisplayIndex = 2
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Harga Satuan",
                HeaderText = "Harga Satuan",
                DataPropertyName = "harga_satuan",
                DisplayIndex = 3
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Qty",
                HeaderText = "Qty",
                DataPropertyName = "qty",
                DisplayIndex = 4
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Total Harga",
                HeaderText = "Total Harga",
                DataPropertyName = "total_bayar",
                DisplayIndex = 5
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin untuk menambah barang ke keranjang?", "Konfirmasi tambah barang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if(comboBox1.SelectedIndex == 0 || teleponBox.Text == "" || namaBox.Text == "" || qtyBox.Text == "")
                {
                    MessageBox.Show("Pastikan field sudah terisi dengan benar", "Error tambah barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if(!int.TryParse(teleponBox.Text, out _))
                {
                    MessageBox.Show("format nomor telepon salah", "Error tambah barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] parts = (comboBox1.SelectedItem?.ToString() ?? "").Split(new[] { " - " }, StringSplitOptions.None);

                int count = dataGridView1.Rows.Count;
                string no_transaksi = "TR" + count.ToString("D3");
                string kode_barang = parts[0];
                string nama_barang = parts[1];
                string harga_satuan = hargaBox.Text;
                int qty = int.Parse(qtyBox.Text);
                string total_harga = totalBox.Text;

                dataGridView1.Rows.Add(no_transaksi, kode_barang, nama_barang, harga_satuan, qty, total_harga);
                dataGridView1.Refresh();

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();
                SqlCommand cmd = new SqlCommand("select * from tbl_pelanggan where nama = '"+namaBox.Text+"' or telepon = '"+teleponBox.Text+"'", con);
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                if (!reader.HasRows)
                {
                    reader.Close();

                    cmd = new SqlCommand("insert into tbl_pelanggan (nama, telepon) values (@nama, @telepon)", con);
                    cmd.Parameters.AddWithValue("@nama", namaBox.Text);
                    cmd.Parameters.AddWithValue("@telepon", teleponBox.Text);

                    cmd.ExecuteNonQuery();

                    con.Close();
                }

                reset();

            }
        }

        long totalHarga = 0;

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            totalHarga = 0;
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string total_harga = row.Cells["Total Harga"].Value.ToString();

                totalHarga += long.Parse(total_harga.Replace("Rp. ", "").Replace(".", ""));
            }

            label11.Text = "Total Harga : " + string.Format("Rp. {0:#,##0}", totalHarga);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin untuk membayar keranjang ini?", "Konfirmasi bayar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (teleponBox.Text == "" || namaBox.Text == "")
                {
                    MessageBox.Show("Pastikan field telepon dan nama sudah terisi dengan benar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(teleponBox.Text, out _))
                {
                    MessageBox.Show("format nomor telepon salah", "Error tambah barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                long jumlah_bayar = int.Parse(textBox6.Text.Replace("Rp. ", "").Replace(".", "").Replace(".", ""));
                long kembalian = jumlah_bayar - totalHarga;

                label13.Text = "Jumlah Kembalian : " + string.Format("Rp. {0:#,##0}", kembalian);
                textBox6.Text = "Rp. ";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] parts = (comboBox1.SelectedItem?.ToString() ?? "").Split(new[] { " - " }, StringSplitOptions.None);

            Connector kon = new Connector();
            SqlConnection con = kon.getCon();

            con.Open();

            SqlCommand cmd = new SqlCommand("select id_user from tbl_user where username = @username", con);
            cmd.Parameters.AddWithValue("@username", LoginForm.username);
            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            int id_user, id_pelanggan, id_barang;
            id_user = 0;
            if (reader.HasRows)
            {
                id_user = int.Parse(reader["id_user"].ToString());
                reader.Close();
            }

            cmd = new SqlCommand("select id_pelanggan from tbl_pelanggan where telepon = @tlp", con);
            cmd.Parameters.AddWithValue("@tlp", teleponBox.Text);

            reader = cmd.ExecuteReader();
            reader.Read();
            id_pelanggan = 0;
            if (reader.HasRows)
            {
                id_pelanggan = int.Parse(reader["id_pelanggan"].ToString());
                reader.Close();
            }

            
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                DateTime now = DateTime.Now;

                string no_transaksi = now.ToString("yyyyMMddHHmmss");
                string tanggal_transaksi = now.ToString("yyyy-MM-dd");
                string nama_kasir = LoginForm.username;
                long total_bayar = totalHarga;
                string kode_barang = row.Cells["Kode Barang"].Value.ToString();

                cmd = new SqlCommand("select id_barang from tbl_barang where kode_barang = @kode", con);
                cmd.Parameters.AddWithValue("@kode", kode_barang);

                reader = cmd.ExecuteReader();
                reader.Read();
                id_barang = 0;
                if (reader.HasRows)
                {
                    id_barang = int.Parse(reader["id_barang"].ToString());
                    reader.Close();
                }


                cmd = new SqlCommand("insert into tbl_transaksi (no_transaksi, tgl_transaksi, nama_kasir, total_bayar, id_user, id_pelanggan, id_barang) values (@no,@tgl, @nama, @total, @id_u, @id_p, @id_b)", con);
                cmd.Parameters.AddWithValue("@no", no_transaksi);
                cmd.Parameters.AddWithValue("@tgl", tanggal_transaksi);
                cmd.Parameters.AddWithValue("@nama", nama_kasir);
                cmd.Parameters.AddWithValue("@total", total_bayar);
                cmd.Parameters.AddWithValue("@id_u", id_user);
                cmd.Parameters.AddWithValue("@id_p", id_pelanggan);
                cmd.Parameters.AddWithValue("@id_b", id_barang);

                cmd.ExecuteNonQuery();
            }
            dataGridView1.Rows.Clear();
            namaBox.Text = "";
            teleponBox.Text = "";
            textBox6.Text = "";
            label11.Text = "Total Harga: Rp. ";
            label13.Text = "Jumlah Kembalian Rp : ";

        }

        private void teleponBox_TextChanged(object sender, EventArgs e)
        {
            if(int.TryParse(teleponBox.Text, out _))
            {

                Connector kon = new Connector();

                SqlConnection con = kon.getCon();

                con.Open();

                SqlCommand cmd = new SqlCommand("select nama from tbl_pelanggan where telepon = '"+teleponBox.Text+"'", con);
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                if (reader.HasRows)
                {
                    namaBox.Text = reader.GetString(0);
                }
            }


        }
    }
}
