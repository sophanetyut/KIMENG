using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KIMENG.MBox
{
    public partial class AddType : Form
    {
        SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnectionString);
        SqlCommand com;
        
        public AddType()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddType_Load(object sender, EventArgs e)
        {
            LoadData();
            textBox4.Focus();
        }

        private void LoadData()
        {
            com = new SqlCommand("SELECT TID, TName FROM tbl_Type ", con);
            try
            {
                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                listBox1.Items.Clear();

                while (reader.Read())
                {
                    listboxItem item = new listboxItem();
                    item.text = reader.GetString(1);
                    item.value = reader.GetInt32(0);
                    listBox1.Items.Add(item);

                    //ListViewItem lv = new ListViewItem(reader.GetInt32(0).ToString());
                    //lv.SubItems.Add(reader.GetString(1));
                    //listView1.Items.Add(lv);
                }
                reader.Close();
            }
            catch (Exception)
            {
                label3.Text = "CAN'T LOAD DATA";
            }
            finally
            {
                con.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox4.Text!="")
            {
                com = new SqlCommand("INSERT INTO tbl_Type VALUES(@TYPE)",con);
                com.Parameters.AddWithValue("@TYPE", textBox4.Text);

                try
                {
                    con.Open();
                    com.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            LoadData();
            textBox4.Clear();
            textBox4.Focus();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem!=null)
            {
                com = new SqlCommand("DELETE FROM tbl_Type WHERE TID=@Tid", con);
                com.Parameters.AddWithValue("@Tid", (listBox1.SelectedItem as listboxItem).value);
                try
                {
                    con.Open();
                    com.ExecuteNonQuery();
                }
                catch (SqlException sq)
                {
                    if (sq.Number==547)
                    {
                        MBox.Message m = new MBox.Message("ទិន្ន័យត្រូវបានប្រើ មិនអាចលុបបានទេ");
                        m.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.Message);
                }
                finally
                {
                    con.Close();
                    LoadData();
                }
            }
            //textBox4.Text = listBox1.SelectedItem.ToString();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Return)
            {
                button3_Click(sender, e);
            }
        }

        
    }
}
