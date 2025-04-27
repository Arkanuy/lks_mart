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
    public partial class MainAdmin : Form
    {
        public MainAdmin()
        {
            InitializeComponent();
        }

        private void loadForm(Form form)
        {
            if(MainPanel.Controls.Count > 0)
            {
                MainPanel.Controls.RemoveAt(0);
            }

            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            MainPanel.Controls.Add(form);
            MainPanel.Tag = form;
            form.Show();
        }

        private void MainAdmin_Load(object sender, EventArgs e)
        {
            loadForm(new LogActivityPanel());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Yakin untuk Logout?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

                }catch(Exception er)
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

        private void button1_Click(object sender, EventArgs e)
        {
            loadForm(new KelolaUser());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadForm(new LogActivityPanel());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadForm(new LaporanPanel());
        }
    }
}
