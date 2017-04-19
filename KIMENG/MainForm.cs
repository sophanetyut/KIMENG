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

namespace KIMENG
{
    public partial class MainForm : Form
    {
        SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnectionString);
        SqlCommand com;
        #region Form
        Button[] btn;
        public MainForm()
        {
            InitializeComponent();
            gSaleP1.EnableHeadersVisualStyles = false;
            gSaleP1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(2, 148, 159);
            gSaleP2.EnableHeadersVisualStyles = false;
            gSaleP2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(2, 148, 159);
            gSaleP3.EnableHeadersVisualStyles = false;
            gSaleP3.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(2, 148, 159);

            gProList.EnableHeadersVisualStyles = false;
            gProList.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(2, 148, 159);

            tbSearch.KeyDown += new KeyEventHandler(tbSearch_KeyDown);
            btn =new Button[]{btnHome, btnSale, btnProduct, btnAddProduct, btnReport};
        }
        
        void ChangeColor(Button buttonObj)
        {
            foreach (Button btnn in btn)
            {
                if (btnn.BackColor!=Color.Transparent)
                {
                    btnn.BackColor = Color.Transparent;
                }
            }
            buttonObj.BackColor = Color.FromArgb(5, 110, 140);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            pHome.BringToFront();
            ChangeColor(btnHome);
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            pSale.BringToFront();
            ChangeColor(btnSale);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            pProductList.BringToFront();
            ChangeColor(btnProduct);
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            pAddProduct.BringToFront();
            ChangeColor(btnAddProduct);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            pReport.BringToFront();
            ChangeColor(btnReport);
        }


        #endregion

        #region AddProduct
        //Btn Add type
        private void button6_Click(object sender, EventArgs e)
        {
            MBox.AddType A = new MBox.AddType();
            A.ShowDialog();
            LoadDataForCB();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbCodeNum.Text=="")
            {
                tbCodeNum.Focus();
                return;
            }
            if (tbProduct.Text=="")
            {
                tbProduct.Focus();
                return;
            }

            com = new SqlCommand("INSERT INTO tbl_Product VALUES(@Pcode,@Product,@Qty,@Price,@SalePrice,@type)", con);
            com.Parameters.AddWithValue("@Pcode", tbCodeNum.Text);
            com.Parameters.AddWithValue("@Product", tbProduct.Text);
            com.Parameters.AddWithValue("@Qty", tbQty.Text);
            com.Parameters.AddWithValue("@Price", tbUnitPrice.Text);
            com.Parameters.AddWithValue("@SalePrice", tbSalePrice.Text);
            com.Parameters.AddWithValue("@type", (comboBox1.SelectedItem as comboboxItem).value.ToString());
            try
            {
                con.Open();
                com.ExecuteNonQuery();
                tbCodeNum.Clear();
                tbProduct.Clear();
                tbQty.Clear();
                tbSalePrice.Clear();
                tbSearch.Clear();
                tbUnitPrice.Clear();
                tbCodeNum.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            LoadProductList();
        }

        public void LoadDataForCB()
        {
            com = new SqlCommand("SELECT TID, TName FROM tbl_Type ", con);
            try
            {
                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                comboBox1.Items.Clear();

                while (reader.Read())
                {
                    comboboxItem item = new comboboxItem();
                    item.text = reader.GetString(1);
                    item.value = reader.GetInt32(0);
                    comboBox1.Items.Add(item);
                }
                reader.Close();
                //MessageBox.Show((comboBox1.Items[1] as comboboxItem).value.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            finally
            {
                con.Close();
            }
        }

        private void tbCodeNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region ProductList

        private void gProList_DoubleClick(object sender, EventArgs e)
        {
            string Pcode = "";
            foreach (DataGridViewRow row in gProList.SelectedRows)
            {
                Pcode = row.Cells[5].Value.ToString();
            }


            com = new SqlCommand("DELETE FROM tbl_Product WHERE PCode = @Pcode", con);
            com.Parameters.AddWithValue("@Pcode", Pcode);
            try
            {
                con.Open();
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            LoadProductList();
        }

        private void tbProList_TextChanged(object sender, EventArgs e)
        {
            com = new SqlCommand(@"SELECT TOP 50 Product as 'ឈ្មោះទំនិញ', Qty as 'ចំនួន', tbl_Type.TName as 'ប្រភេទ', Price as 'តម្លៃទិញចូល', 
                                    SalePrice as 'តម្លៃលក់ចេញ', PCode as 'Code' FROM tbl_Product left join tbl_Type on tbl_Product.[Type]
                                = tbl_Type.TID WHERE PCode LIKE '%'+@Pid+'%'", con);
            com.Parameters.AddWithValue("@Pid", tbProList.Text);
            try
            {
                con.Open();
                SqlDataAdapter adt = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                adt.Fill(dt);
                gProList.DataSource = dt;
                lbProlist.Text = "Show : " + gProList.RowCount;
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadProductList()
        {
            com = new SqlCommand("SELECT TOP 50 Product as 'ឈ្មោះទំនិញ', Qty as 'ចំនួន', tbl_Type.TName as 'ប្រភេទ', Price as 'តម្លៃទិញចូល', SalePrice as 'តម្លៃលក់ចេញ', PCode as 'Code' FROM tbl_Product left join tbl_Type on tbl_Product.[Type] = tbl_Type.TID", con);
            try
            {
                con.Open();
                SqlDataAdapter adt = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                adt.Fill(dt);
                gProList.DataSource = dt;

            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            finally
            {
                con.Close();
            }
            lbProlist.Text = "Show : " + gProList.RowCount;
        }

        private void gProList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            if (null != gridView)
            {
                foreach (DataGridViewRow r in gridView.Rows)
                {
                    gridView.Rows[r.Index].HeaderCell.Value =
                                        (r.Index + 1).ToString();
                }
            }
        }

        #endregion

        #region Sale
        string Product = "", Type = "";
        float Price,total;
        public static float Dis;
        public static int Qty;
        public static bool btnState=false;
        float[] sumTotal=new float[3];
        float[] luyKok = new float[3];
        private void button2_Click(object sender, EventArgs e)
        {
            int abc;
            if (!int.TryParse(tbSearch.Text, out abc))
            {
                return;
            }
            com = new SqlCommand("SELECT Product, TName,  Price FROM tbl_Product LEFT JOIN tbl_Type ON tbl_Product.[Type]=tbl_Type.TID WHERE PCode=@Pcode", con);
            com.Parameters.AddWithValue("@Pcode", tbSearch.Text);
            try
            {
                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                if (reader.Read())
                {

                    Product = reader.GetString(0);
                    Type = reader.GetString(1);
                    float.TryParse(reader.GetSqlMoney(2).ToString(), out Price);

                    MBox.AddNewItem a = new MBox.AddNewItem(Product);
                    a.ShowDialog();

                    if (btnState)
                    {
                        if (Dis>0)
                        {
                            float totall = Qty*Price;
                            float bc = (totall * Dis) / 100;

                            total = totall-bc;
                        }else
                            total = Qty * Price;

                        if (tabControl1.SelectedTab == tabControl1.TabPages[0])
                        {
                            gSaleP1.Rows.Add(gSaleP1.RowCount + 1, Product, Qty, Type, Price, Dis, total);
                        }
                        else if (tabControl1.SelectedTab == tabControl1.TabPages[1])
                        {
                            gSaleP2.Rows.Add(gSaleP2.RowCount + 1, Product, Qty, Type, Price, Dis, total);
                        }
                        else if (tabControl1.SelectedTab == tabControl1.TabPages[2])
                        {
                            gSaleP3.Rows.Add(gSaleP3.RowCount + 1, Product, Qty, Type, Price, Dis, total);
                        }
                    }
                }


                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        
        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode ==Keys.Enter)
            {
                button2_Click(sender, e);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float a;
            float.TryParse(textBox1.Text, out a);
            //float.TryParse(label10.Text, out b);
            //label14.Text = (b - a).ToString();
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    luyKok[0] = a;
                    label14.Text = (sumTotal[0] - luyKok[0]).ToString();
                    break;
                case "tabPage2":
                    luyKok[1] = a;
                    label14.Text = (sumTotal[1] - luyKok[1]).ToString();
                    break;
                case "tabPage3":
                    luyKok[2] = a;
                    label14.Text = (sumTotal[2] - luyKok[2]).ToString();
                    break;
                default:
                    break;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    label12.Text = (sumTotal[0] / 4000).ToString();
                    label10.Text = sumTotal[0]+" រ";
                    label6.Text = "Show : " + gSaleP1.RowCount.ToString();
                    textBox1.Text = luyKok[0].ToString();
                    label14.Text = (sumTotal[0] - luyKok[0]).ToString();
                    break;
                case "tabPage2":
                    label12.Text = (sumTotal[1] / 4000).ToString();
                    label10.Text = sumTotal[1] + " រ";
                    label6.Text = "Show : " + gSaleP2.RowCount.ToString();
                    textBox1.Text = luyKok[1].ToString();
                    label14.Text = (sumTotal[1] - luyKok[1]).ToString();
                    break;
                case "tabPage3":
                    label12.Text = (sumTotal[2] / 4000).ToString();
                    label10.Text = sumTotal[2] + " រ";
                    label6.Text = "Show : " + gSaleP3.RowCount.ToString();
                    textBox1.Text = luyKok[2].ToString();
                    label14.Text = (sumTotal[2] - luyKok[2]).ToString();
                    break;
                default:
                    break;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            {
                e.Handled = true;
            }
        }


        private void gSaleP1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    sumTotal[0] = sumGrid(gSaleP1);
                    label12.Text = (sumTotal[0] / 4000).ToString();
                    label10.Text = sumTotal[0] + " រ";
                    label6.Text = "Show : " + gSaleP1.RowCount.ToString();
                    label14.Text = (sumTotal[0] - luyKok[0]) + " រ";
                    break;
                case "tabPage2":
                    sumTotal[1] = sumGrid(gSaleP2);
                    label12.Text = (sumTotal[1] / 4000).ToString();
                    label10.Text = sumTotal[1] + " រ";
                    label6.Text = "Show : " + gSaleP2.RowCount.ToString();
                    label14.Text = (sumTotal[1] - luyKok[1]) + " រ";
                    break;
                case "tabPage3":
                    sumTotal[2] = sumGrid(gSaleP3);
                    label12.Text = (sumTotal[2] / 4000).ToString();
                    label10.Text = sumTotal[2] + " រ";
                    label6.Text = "Show : " + gSaleP1.RowCount.ToString();
                    label14.Text = (sumTotal[2] - luyKok[2]) + " រ";
                    break;
                default:
                    break;
            }
        }

        private void gSaleP1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    sumTotal[0] = sumGrid(gSaleP1);
                    label12.Text = (sumTotal[0] / 4000).ToString();
                    label10.Text = sumTotal[0] + " រ";
                    label6.Text = "Show : " + gSaleP1.RowCount.ToString();
                    label14.Text = (sumTotal[0] - luyKok[0]) + " រ";
                    break;
                case "tabPage2":
                    sumTotal[1] = sumGrid(gSaleP2);
                    label12.Text = (sumTotal[1] / 4000).ToString();
                    label10.Text = sumTotal[1]+" រ";
                    label6.Text = "Show : " + gSaleP2.RowCount.ToString();
                    label14.Text = (sumTotal[1] - luyKok[1]) + " រ";
                    break;
                case "tabPage3":
                    sumTotal[2] = sumGrid(gSaleP3);
                    label12.Text = (sumTotal[2] / 4000).ToString();
                    label10.Text = sumTotal[2]+" រ";
                    label6.Text = "Show : " + gSaleP1.RowCount.ToString();
                    label14.Text = (sumTotal[2] - luyKok[2])+" រ";
                    break;
                default:
                    break;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    gSaleP1.Rows.Clear();
                    break;
                case "tabPage2":
                    gSaleP2.Rows.Clear();
                    break;
                case "tabPage3":
                    gSaleP3.Rows.Clear();
                    break;
                default:
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = "Date : " + DateTime.Now;
        }
        #endregion



        
        private float sumGrid(DataGridView dataGridView)
        {
            int sum = 0;
            for (int i = 0; i < dataGridView.Rows.Count; ++i)
            {
                int a;
                int.TryParse(dataGridView.Rows[i].Cells[6].Value.ToString(), out a);
                sum += a;
            }
            return sum;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadProductList();
            LoadDataForCB();
            timer1.Start();
        }

        
    }
}
