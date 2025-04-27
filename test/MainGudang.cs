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
    public partial class MainGudang : Form
    {
        public MainGudang()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
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

        private void munculData()
        {
            Connector kon = new Connector();

            SqlConnection con = kon.getCon();

            con.Open();

            SqlCommand cmd = new SqlCommand("select * from tbl_barang", con);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds, "tbl_barang");

            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "tbl_barang";
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Refresh();

            con.Close();
            
        }

        private void setupDatagrid()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
               Name = "ID Barang",
               HeaderText = "ID Barang",
               DataPropertyName = "id_barang",
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
                Name = "Expired Date",
                HeaderText = "Expired Date",
                DataPropertyName = "expired_date",
                DisplayIndex = 3
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Qty",
                HeaderText = "Qty",
                DataPropertyName = "jumlah_barang",
                DisplayIndex = 4
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Satuan",
                HeaderText = "Satuan",
                DataPropertyName = "satuan",
                DisplayIndex = 5
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Harga Satuan",
                HeaderText = "Harga Satuan",
                DataPropertyName = "harga_satuan",
                DisplayIndex = 6
            });
        }

        private void resetInput()
        {
            kodeBox.Text = "";
            jumlahBox.Text = "";
            namaBox.Text = "";
            satuanBox.Text = "";
            hargaBox.Text = "";
        }

        private void MainGudang_Load(object sender, EventArgs e)
        {
            setupDatagrid();
            munculData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Yakin untuk menambah barang ?", "Konfirmasi tambah barang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if(kodeBox.Text == "" || jumlahBox.Text == "" || namaBox.Text == "" || satuanBox.Text == "" || hargaBox.Text == "")
                {
                    MessageBox.Show("Semua field harus di isi!", "Error tambah barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if(!int.TryParse(jumlahBox.Text, out _))
                {
                    MessageBox.Show("format jumlah barang salah", "Error tambah barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();
                DateTime exp = dateTimePicker1.Value;

                string formatExp = exp.ToString("yyyy-MM-dd");
                con.Open();

                SqlCommand cmd = new SqlCommand("insert into tbl_barang (kode_barang, nama_barang, expired_date, jumlah_barang, satuan, harga_satuan) values (@kode, @nama, @exp, @qty, @satuan, @harga)", con);

                
                cmd.Parameters.AddWithValue("kode", kodeBox.Text);
                cmd.Parameters.AddWithValue("nama", namaBox.Text);
                cmd.Parameters.AddWithValue("exp", formatExp);
                cmd.Parameters.AddWithValue("qty", long.Parse(jumlahBox.Text));
                cmd.Parameters.AddWithValue("satuan", satuanBox.Text);
                cmd.Parameters.AddWithValue("harga", long.Parse(hargaBox.Text));

                cmd.ExecuteNonQuery();

                munculData();
                resetInput();

                MessageBox.Show("Berhasil tambah data!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                con.Close();
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                kodeBox.Text = row.Cells[1].Value.ToString();
                namaBox.Text = row.Cells[2].Value.ToString();
                jumlahBox.Text = row.Cells[4].Value.ToString();
                satuanBox.Text = row.Cells[5].Value.ToString();
                hargaBox.Text = row.Cells[6].Value.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin untuk mengedit barang ? dibutuhkan kode barang", "Konfirmasi edit barang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (kodeBox.Text == "" || jumlahBox.Text == "" || namaBox.Text == "" || satuanBox.Text == "" || hargaBox.Text == "")
                {
                    MessageBox.Show("Semua field harus di isi!", "Error edit barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(jumlahBox.Text, out _))
                {
                    MessageBox.Show("format jumlah barang salah", "Error edit barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();
                DateTime exp = dateTimePicker1.Value;

                string formatExp = exp.ToString("yyyy-MM-dd");

                SqlCommand cmd = new SqlCommand("select * from tbl_barang where kode_barang = @kode", con);
                cmd.Parameters.AddWithValue("@kode", kodeBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                if (reader.HasRows)
                {
                    reader.Close();

                    cmd = new SqlCommand("update tbl_barang set nama_barang = @nama, expired_date = @exp, jumlah_barang = @qty, satuan = @satuan, harga_satuan = @harga where kode_barang = @kode", con);
                    
                    cmd.Parameters.AddWithValue("kode", kodeBox.Text);
                    cmd.Parameters.AddWithValue("nama", namaBox.Text);
                    cmd.Parameters.AddWithValue("exp", formatExp);
                    cmd.Parameters.AddWithValue("qty", long.Parse(jumlahBox.Text));
                    cmd.Parameters.AddWithValue("satuan", satuanBox.Text);
                    cmd.Parameters.AddWithValue("harga", long.Parse(hargaBox.Text));

                    cmd.ExecuteNonQuery();

                    munculData();
                    resetInput();

                    MessageBox.Show("Berhasil edit data!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    con.Close();
                }
                else
                {
                    MessageBox.Show("tidak ada data", "Error edit barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin untuk menghapus barang ? dibutuhkan kode barang", "Konfirmasi hapus barang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (kodeBox.Text == "")
                {
                    MessageBox.Show("Field kode barang harus di isi!", "Error hapus barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();

                SqlCommand cmd = new SqlCommand("select * from tbl_barang where kode_barang = @kode", con);
                cmd.Parameters.AddWithValue("@kode", kodeBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                if (reader.HasRows)
                {
                    reader.Close();

                    cmd = new SqlCommand("delete from tbl_barang where kode_barang = @kode", con);
                    cmd.Parameters.AddWithValue("@kode", kodeBox.Text);

                    cmd.ExecuteNonQuery();

                    munculData();
                    resetInput();

                    MessageBox.Show("Berhasil hapus data!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    con.Close();
                }
                else
                {
                    MessageBox.Show("tidak ada data", "Error hapus barang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void cariBox_TextChanged(object sender, EventArgs e)
        {
            if(cariBox.Text == "")
            {
                munculData();
                return;
            }

            Connector kon = new Connector();
            SqlConnection con = kon.getCon();

            con.Open();

            SqlCommand cmd = new SqlCommand("select * from tbl_barang where kode_barang like '"+cariBox.Text+"' or nama_barang like '"+cariBox.Text+"'", con);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds, "tbl_barang");

            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "tbl_barang";

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Refresh();
        }
    }
}
