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
using System.Drawing.Printing;

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

        private static Connector kon = new Connector();
        



        private void comboBox1_ValueMemberChanged(object sender, EventArgs e)
        {
            
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] parts = (comboBox1.SelectedItem?.ToString() ?? "").Split(new[] { "-" }, StringSplitOptions.None);
            SqlConnection con = kon.getCon();
            con.Open();

            SqlCommand cmd = new SqlCommand("select harga_satuan from tbl_barang where kode_barang = @kode", con);
            cmd.Parameters.AddWithValue("@kode", parts[0]);

            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            if (reader.HasRows)
            {
                hargaBox.Text = convert(reader["harga_satuan"]);
            }
        }

        string convert(dynamic args)
        {
            return string.Format("Rp. {0:#,##0}", args);
        }

        private void qtyBox_TextChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex != 0 && long.TryParse(qtyBox.Text, out _))
            {
                long hargaSatuan = long.Parse(hargaBox.Text.Replace("Rp. ", "").Replace(".", ""));
                long qty = long.Parse(qtyBox.Text);
                long total = hargaSatuan * qty;

                totalBox.Text = convert(total);
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
                Name = "no transaksi",
                HeaderText = "No Transaksi",
                DataPropertyName = "no_transaksi",
                DisplayIndex = 0
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "kode barang",
                HeaderText = "Kode Barang",
                DataPropertyName = "kode_barang",
                DisplayIndex = 1
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "nama Barang",
                HeaderText = "Nama Barang",
                DataPropertyName = "nama_barang",
                DisplayIndex = 2
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "harga satuan",
                HeaderText = "Harga Satuan",
                DataPropertyName = "harga_satuan",
                DisplayIndex = 3
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "qty",
                HeaderText = "Qty",
                DataPropertyName = "qty",
                DisplayIndex = 4
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "total",
                HeaderText = "Total",
                DataPropertyName = "total",
                DisplayIndex = 5
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0 || hargaBox.Text == "" || qtyBox.Text == "" || totalBox.Text == "")
            {
                MessageBox.Show("pastikan field yang di perlukan sudah di isi");
                return;
            }

            if(!int.TryParse(qtyBox.Text, out _))
            {
                MessageBox.Show("Format qty salah");
                return;
            }
            string[] parts = (comboBox1.SelectedItem?.ToString() ?? "").Split(new[] { " - " }, StringSplitOptions.None);

            int count = dataGridView1.Rows.Count;
            string no_transaksi = count.ToString("D3");
            string kode = parts[0];
            string nama = parts[1];
            string harga = hargaBox.Text;
            string qty = qtyBox.Text;
            string total = totalBox.Text;

            dataGridView1.Rows.Add(no_transaksi, kode, nama, harga, qty, total);
            dataGridView1.Refresh();
            reset();            

        }

        long totalHarga = 0;

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            totalHarga = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                totalHarga += long.Parse(row.Cells["total"].Value.ToString().Replace("Rp. ", "").Replace(".", ""));
            }
            label11.Text = "Total Harga : " + convert(totalHarga);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long bayar = long.Parse(textBox6.Text.Replace("Rp. ", ".").Replace(".", ""));
            long kembalian = bayar - totalHarga;

            label13.Text = "Jumlah Kembalian : " + convert(kembalian);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count > 1)
            {
                if(teleponBox.Text == "" || namaBox.Text == "")
                {
                    MessageBox.Show("pastikan field telepon dan nama sudah di isi");
                    return;
                }

                if(!int.TryParse(teleponBox.Text, out _))
                {
                    MessageBox.Show("format nomor telepon salah");
                    return;
                }

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();
                SqlCommand cmd = new SqlCommand("select * from tbl_pelanggan where telepon = @tlp", con);
                cmd.Parameters.AddWithValue("@tlp", teleponBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                if (!reader.HasRows)
                {
                    reader.Close();

                    cmd = new SqlCommand("insert into tbl_pelanggan (telepon, nama) values (@tlp, @nama)", con);
                    cmd.Parameters.AddWithValue("@tlp", teleponBox.Text);
                    cmd.Parameters.AddWithValue("@nama", namaBox.Text);

                    cmd.ExecuteNonQuery();
                }
                reader.Close();
                int id_user = 0, id_pelanggan = 0, id_barang = 0;

                // get id_user

                cmd = new SqlCommand("select id_user from tbl_user where username = @uname", con);
                cmd.Parameters.AddWithValue("@uname", LoginForm.username);

                reader = cmd.ExecuteReader();

                reader.Read();

                if (reader.HasRows)
                {

                    id_user = reader.GetInt32(0);
                    reader.Close();
                }

                // get id_pelanggan

                cmd = new SqlCommand("select id_pelanggan from tbl_pelanggan where telepon = @tlp", con);
                cmd.Parameters.AddWithValue("@tlp", teleponBox.Text);

                reader = cmd.ExecuteReader();

                reader.Read();

                if (reader.HasRows)
                {

                    id_pelanggan = reader.GetInt32(0);
                    reader.Close();
                }

                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    // get id_barang

                    cmd = new SqlCommand("select id_barang from tbl_barang where kode_barang = @kode", con);
                    cmd.Parameters.AddWithValue("@kode", row.Cells["kode barang"].Value.ToString());

                    reader = cmd.ExecuteReader();

                    reader.Read();

                    if (reader.HasRows)
                    {

                        id_barang = reader.GetInt32(0);
                        reader.Close();
                    }
                    string no_tr = DateTime.Now.ToString("ddMMyyyyHHmmss");
                    string tgl = DateTime.Now.ToString("yyyy-MM-dd");

                    string nama_kasir = LoginForm.username;
                    long total_bayar = long.Parse(row.Cells["total"].Value.ToString().Replace("Rp. ", "").Replace(".", ""));

                    cmd = new SqlCommand("insert into tbl_transaksi (no_transaksi, tgl_transaksi, nama_kasir, total_bayar, id_user, id_pelanggan, id_barang) values (@no, @tgl, @nama, @total, @id_u, @id_p, @id_b)", con);
                    cmd.Parameters.AddWithValue("@no", no_tr);
                    cmd.Parameters.AddWithValue("@tgl", tgl);
                    cmd.Parameters.AddWithValue("@nama", nama_kasir);
                    cmd.Parameters.AddWithValue("@total", total_bayar);
                    cmd.Parameters.AddWithValue("@id_u", id_user);
                    cmd.Parameters.AddWithValue("@id_p", id_pelanggan);
                    cmd.Parameters.AddWithValue("@id_b", id_barang);

                    cmd.ExecuteNonQuery();
                }
                dataGridView1.Rows.Clear();

                dataGridView1.Refresh();
            }
        }

        private void teleponBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
           if(dataGridView1.Rows.Count > 1)
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (s, ev) =>
                {
                    Font font = new Font("Cambria", 16);
                    Pen pen = new Pen(Brushes.Black);

                    float y = 20;
                    // header
                    ev.Graphics.DrawString("INVOICE: " + DateTime.Now.ToString("dd/MM/yyyy"),font, Brushes.Black, 20, y);
                    y += 30;
                    ev.Graphics.DrawLine(pen, 20, y, 400, y);
                    y += 40;

                    // header item

                    ev.Graphics.DrawString("Barang", font, Brushes.Black, 20, y);
                    ev.Graphics.DrawString("Qty", font, Brushes.Black, 200, y);
                    ev.Graphics.DrawString("Harga", font, Brushes.Black, 240, y);
                    y += 60;

                    foreach(DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;
                        ev.Graphics.DrawString(row.Cells["nama barang"].Value.ToString(), font, Brushes.Black, 20, y);
                        ev.Graphics.DrawString(row.Cells["qty"].Value.ToString(), font, Brushes.Black, 200, y);
                        ev.Graphics.DrawString(row.Cells["total"].Value.ToString(), font, Brushes.Black, 240, y);
                        y += 30;
                    }

                    ev.Graphics.DrawLine(pen, 20, y, 400, y);
                    ev.Graphics.DrawString("Total Harga : " + convert(totalHarga), font, Brushes.Black, 240, y);



                };

                pd.Print();
            }
            
        }
    }
}
