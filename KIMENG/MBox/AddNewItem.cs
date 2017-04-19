using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KIMENG.MBox
{
    public partial class AddNewItem : Form
    {
        public AddNewItem()
        {
            InitializeComponent();
        }

        public AddNewItem(string item)
        {
            InitializeComponent();
            textBox1.Text = item;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox2.Text, out MainForm.Qty);
            float.TryParse(textBox3.Text, out MainForm.Dis);
            MainForm.btnState = true;
            this.Close();
        }
        

        private void AddNewItem_Shown(object sender, EventArgs e)
        {
           // textBox2.SelectAll();
        }

       
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar==(char)Keys.Back)))
            {
                e.Handled = true;
            }
            if (e.KeyChar==(char)Keys.Enter)
            {

            }
        }
    }
}