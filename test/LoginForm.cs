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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        string passwordDefaultValue = "Min. 8 characters";
        string emailDefaultValue = "test@simple.com";
        public static string username { get; private set; }

        private void setDefault()
        {
            emailBox.Text = emailDefaultValue;
            emailBox.ForeColor = Color.Gray;

            passwordBox.Text = passwordDefaultValue;
            passwordBox.ForeColor = Color.Gray;
            passwordBox.UseSystemPasswordChar = false;

        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            setDefault();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setDefault();
        }

        private void emailBox_Enter(object sender, EventArgs e)
        {
            if(emailBox.Text == emailDefaultValue)
            {
                emailBox.Text = "";
                emailBox.ForeColor = Color.Black;
            }
        }

        private void emailBox_Leave(object sender, EventArgs e)
        {
            if (emailBox.Text == "")
            {
                emailBox.Text = emailDefaultValue;
                emailBox.ForeColor = Color.Gray;
            }
        }

        private void passwordBox_Enter(object sender, EventArgs e)
        {
            if(passwordBox.Text == passwordDefaultValue)
            {
                passwordBox.Text = "";
                passwordBox.ForeColor = Color.Black;
                passwordBox.UseSystemPasswordChar = true;
            }
        }

        private void passwordBox_Leave(object sender, EventArgs e)
        {
            if (passwordBox.Text == "")
            {
                passwordBox.Text = passwordDefaultValue;
                passwordBox.ForeColor = Color.Gray;
                passwordBox.UseSystemPasswordChar = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(emailBox.Text == "" || emailBox.Text == emailDefaultValue || passwordBox.Text == "" || passwordBox.Text == passwordDefaultValue)
            {
                MessageBox.Show("field email atau password tidak boleh kosong", "login gagal");
            }
            else
            {
                Connector kon = new Connector();
                SqlConnection Con = kon.getCon();
                username = emailBox.Text;
                Con.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tbl_user where username = @uname and password = @pass", Con);

                    cmd.Parameters.AddWithValue("@uname", emailBox.Text);
                    cmd.Parameters.AddWithValue("@pass", passwordBox.Text);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        MessageBox.Show("Login Berhasil", "Berhasil");

                        string tipeUser = reader["tipe_user"].ToString().ToLower();
                        if (tipeUser == "admin")
                        {
                            MainAdmin form = new MainAdmin();

                            Hide();
                            form.Show();
                        }else if(tipeUser == "gudang")
                        {
                            MainGudang form = new MainGudang();

                            Hide();

                            form.Show();
                        }
                        else if (tipeUser == "kasir")
                        {
                            MainKasir form = new MainKasir();

                            Hide();

                            form.Show();
                        }
                    }
                }catch (Exception er)
                {
                    MessageBox.Show("Error: " + er, "Login gagal");
                }
                finally
                {
                    Con.Close();

                    Con.Open();
                    DateTime now = DateTime.Now;
                    SqlCommand cmd = new SqlCommand("insert into tbl_log (id_user, waktu, aktivitas) select tbl_user.id_user, @waktu, @akt from tbl_user where tbl_user.username = @uname", Con);
                    cmd.Parameters.AddWithValue("@uname", username);
                    cmd.Parameters.AddWithValue("@waktu", now);
                    cmd.Parameters.AddWithValue("akt", "Login");

                    cmd.ExecuteNonQuery();
                }

            }
        }
    }
}
