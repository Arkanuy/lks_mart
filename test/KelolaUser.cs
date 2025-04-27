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
    public partial class KelolaUser : Form
    {
        public KelolaUser()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Yakin untuk mengedit data user ?", "Konfirmasi Edit User", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if(usernameBox.Text == "" || passwordBox.Text == "")
                {
                    MessageBox.Show("Field username dan password harus di isi", "Error edit user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tbl_user where username = '"+usernameBox.Text+"' and password = '"+passwordBox.Text+"'", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        if(namaBox.Text == "" || teleponBox.Text == "" || alamatBox.Text == "")
                        {
                            MessageBox.Show("Field Nama, Telepon, dan Alamat harus di isi", "Error edit user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if(!int.TryParse(teleponBox.Text, out _))
                        {
                            MessageBox.Show("format nomor telepon salah", "Error edit user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        cmd = new SqlCommand("update tbl_user set tipe_user = @tipe, nama = @nama, telpon = @telepon, alamat = @alamat where username = @uname and password = @pass", con);

                        cmd.Parameters.AddWithValue("@tipe", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@nama", namaBox.Text);
                        cmd.Parameters.AddWithValue("@telepon", teleponBox.Text);
                        cmd.Parameters.AddWithValue("@alamat", alamatBox.Text);
                        cmd.Parameters.AddWithValue("@uname", usernameBox.Text);
                        cmd.Parameters.AddWithValue("@pass", passwordBox.Text);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("berhasil mengedit data", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("data tidak di temukan", "Error edit user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch(Exception er)
                {
                    MessageBox.Show("Error: " + er);
                }
                finally
                {
                    con.Close();
                    munculData();
                }
            }
        }

        private void munculData()
        {
            Connector kon = new Connector();
            SqlConnection con = kon.getCon();

            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_user", con);

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(ds, "tb_user");
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "tb_user";
                dataGridView1.AllowUserToAddRows = false;
            }catch(Exception er)
            {
                MessageBox.Show("error : " + er);
            }
            finally
            {
                con.Close();
                dataGridView1.Refresh();
            }
        }

        private void setupDatagrid()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "No",
                HeaderText = "No",
                DataPropertyName = "id_user",
                DisplayIndex = 0
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Tipe User",
                HeaderText = "Tipe User",
                DataPropertyName = "tipe_user",
                DisplayIndex = 1
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Nama User",
                HeaderText = "Nama User",
                DataPropertyName = "nama",
                DisplayIndex = 2
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Alamat",
                HeaderText = "Alamat",
                DataPropertyName = "alamat",
                DisplayIndex = 3
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Telepon",
                HeaderText = "Telepon",
                DataPropertyName = "telpon",
                DisplayIndex = 4
            });
        }

        private void clearInput()
        {
            comboBox1.SelectedIndex = 0;
            namaBox.Text = "";
            teleponBox.Text = "";
            alamatBox.Text = "";
            usernameBox.Text = "";
            passwordBox.Text = "";
        }

        private void KelolaUser_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            cariBox.Text = "Cari User";
            cariBox.ForeColor = Color.Gray;
            setupDatagrid();
            munculData();
        }

        private void cariBox_Enter(object sender, EventArgs e)
        {
            if(cariBox.Text == "Cari User")
            {
                cariBox.Text = "";
                cariBox.ForeColor = Color.Black;
            }
        }

        private void cariBox_Leave(object sender, EventArgs e)
        {
            if (cariBox.Text == "")
            {
                cariBox.Text = "Cari User";
                cariBox.ForeColor = Color.Gray;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("yakin untuk menambah user ?", "Konfirmasi tambah user", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if(namaBox.Text != "" || teleponBox.Text != "" || alamatBox.Text != "" || usernameBox.Text != "" || passwordBox.Text != "")
                {
                    if(int.TryParse(teleponBox.Text, out _))
                    {
                        Connector kon = new Connector();
                        SqlConnection con = kon.getCon();

                        con.Open();
                        try
                        {
                            SqlCommand cmd = new SqlCommand("insert into tbl_user (tipe_user, nama, alamat, telpon, username, password) values (@tipe, @nama, @alamat, @telepon, @username, @password)", con);

                            cmd.Parameters.AddWithValue("@tipe", comboBox1.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@nama", namaBox.Text);
                            cmd.Parameters.AddWithValue("@alamat", alamatBox.Text);
                            cmd.Parameters.AddWithValue("@telepon", teleponBox.Text);
                            cmd.Parameters.AddWithValue("@username", usernameBox.Text);
                            cmd.Parameters.AddWithValue("@password", passwordBox.Text);

                            cmd.ExecuteNonQuery();

                        }catch (Exception er)
                        {
                            MessageBox.Show("Error: " + er);
                        }
                        finally
                        {
                            con.Close(); 
                            MessageBox.Show("Berhasil tambah user", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            munculData();
                            clearInput();
                        }
                    }
                    else
                    {
                        MessageBox.Show("format nomor telepon salah", "Gagal tambah user", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Semua field harus di isi", "Gagal tambah user", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cariBox_TextChanged(object sender, EventArgs e)
        {
            if(cariBox.Text == "" || cariBox.Text == "Cari User")
            {
                munculData();
            }
            else
            {

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();
                con.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tbl_user where tipe_user like '%" + cariBox.Text + "%' or nama like '%" + cariBox.Text + "%' or alamat like '%" + cariBox.Text + "%' or telpon like '%" + cariBox.Text + "%'", con);

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(ds, "tb_user");

                    dataGridView1.DataSource = ds;
                    dataGridView1.DataMember = "tb_user";
                    dataGridView1.AllowUserToAddRows = false;

                }
                catch (Exception er)
                {
                    MessageBox.Show("Error: " + er);
                }
                finally
                {
                    dataGridView1.Refresh();
                    con.Close();
                }
                
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                comboBox1.SelectedItem = row.Cells[1].Value.ToString();
                namaBox.Text = row.Cells[2].Value.ToString();
                alamatBox.Text = row.Cells[3].Value.ToString();
                teleponBox.Text = row.Cells[4].Value.ToString();
                usernameBox.Text = row.Cells[5].Value.ToString();
                passwordBox.Text = row.Cells[6].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin untuk menghapus user ?", "Konfirmasi hapus User", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (usernameBox.Text == "" || passwordBox.Text == "")
                {
                    MessageBox.Show("Field username dan password harus di isi", "Error edit user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Connector kon = new Connector();
                SqlConnection con = kon.getCon();

                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tbl_user where username = '"+usernameBox.Text+"' and password = '"+passwordBox.Text+"'", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    if (reader.HasRows)
                    {
                        reader.Close();
                        cmd = new SqlCommand("delete from tbl_user where username = '" + usernameBox.Text + "' and password = '" + passwordBox.Text + "'", con);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Berhasil hapus user", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("data tidak di temukan", "Error Hapus user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch(Exception er)
                {
                    MessageBox.Show("error" + er);
                }
                finally
                {
                    con.Close();
                    munculData();
                }
            }
        }
    }
}
