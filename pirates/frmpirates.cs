using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace pirates
{
    public partial class form : Form
    {
        string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\naikom.2\\Downloads\\dpPirates.accdb";
        OleDbCommand cmd;
        OleDbConnection conn;
        OleDbDataAdapter adapter;
        DataTable dt;


        int id;
        Boolean ifview;
        public form()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeData();
            initializecbo();
            DisableTextBox();
            cboPiratesSearch.SelectedIndex = -1;
            cboPiratesGroup.SelectedIndex = -1;
            btnSave.Enabled = false;
            BtnCancel.Enabled = false;
           
        }
         
        
        private void EnableTextBox()
        {
            txtAge.Enabled = true;
            txtAlias.Enabled = true;
            txtBounty.Enabled = true;
            cboPiratesGroup.Enabled = true;
            txtBounty.Enabled = true;
            txtName.Enabled = true;
        }

        public void DisableTextBox()
        {
            txtAlias.Enabled = false;
            txtAge.Enabled = false;
            txtBounty.Enabled = false;
            cboPiratesGroup.Enabled = false;
            txtBounty.Enabled = false;
            txtName.Enabled = false;
        }

        public void Cleartxt()
        {
            txtAlias.Clear();
            txtAge.Clear();
            txtName.Clear();
            txtBounty.Clear();
            cboPiratesGroup.SelectedIndex = -1;
        }

        public void InitializeData()
        {
            string query = "SELECT id, piratename AS ALIAS, givenname AS NAME, age AS AGE, pirategroup AS [PIRATE GROUP], bounty AS [BOUNTY (BELLY)] FROM pirates";
            
            using (conn = new OleDbConnection(ConnStr))
            {
                conn.Open();
                
                using (adapter = new OleDbDataAdapter(query,conn))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    grdview.DataSource = dt;
                    conn.Close();

                }
                grdview.Columns["age"].Visible = false;
                grdview.Columns["id"].Visible = false;
        
            }
        }

        public void initializecbo()
        {
            string query = "select distinct pirategroup from pirates";

            using (conn = new OleDbConnection(ConnStr))
            {
                conn.Open();

                using (adapter = new OleDbDataAdapter(query, conn))
                {
                    dt = new DataTable();
                    adapter.Fill(dt); 
                    conn.Close();
                    
                }
                cboPiratesSearch.DataSource = dt;
                cboPiratesGroup.DataSource = dt;

                cboPiratesGroup.DisplayMember = "pirategroup";
                cboPiratesSearch.DisplayMember = "pirategroup";

                cboPiratesGroup.ValueMember = "pirategroup";
                cboPiratesSearch.ValueMember = "pirategroup";

            }
        }


        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            EnableTextBox();
            btnNewRecord.Enabled = false;
            btnSave.Enabled = true;
            BtnCancel.Enabled = true;

            MessageBox.Show(id.ToString());
            ifview = true;

        }

        private void grdview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id = Convert.ToInt32(grdview.SelectedCells[0].Value.ToString());
            txtAlias.Text = grdview.SelectedCells[1].Value.ToString();
            txtName.Text = grdview.SelectedCells[2].Value.ToString();
            txtAge.Text = grdview.SelectedCells[3].Value.ToString();
            cboPiratesGroup.Text = grdview.SelectedCells[4].ToString();
            txtBounty.Text = grdview.SelectedCells[5].Value.ToString();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query = "SELECT id, piratename AS ALIAS, givenname AS NAME, age AS AGE, pirategroup AS [PIRATE GROUP], bounty AS [BOUNTY(BELLY)]" +
                           " FROM pirates WHERE (piratename LIKE @alias OR givenname LIKE @name) AND pirategroup LIKE @pirategroup";
      
            using(conn = new OleDbConnection(ConnStr))
            {
                conn.Open();

                using(cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@alies", "%" + txtSearch.Text + "%");
                    cmd.Parameters.AddWithValue("@name", "%" + txtSearch.Text + "%");
                    cmd.Parameters.AddWithValue("@pirategroup", cboPiratesSearch.Text);
                    cmd.ExecuteNonQuery();

                    using(adapter = new OleDbDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        conn.Close();

                        grdview.DataSource = dt;
                    }
                    

                    //if (res > 0)
                    //{
                    //    MessageBox.Show("Successfully Added", "Add Data");
                    //}
                    conn.Close();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string query = "Delete FROM pirates WHERE id =@id";

            using(conn =new OleDbConnection(ConnStr))
            {
                conn.Open();
                
                using(cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", grdview.SelectedCells[0].Value.ToString());
                    int res = cmd.ExecuteNonQuery();
                    conn.Close();

                    if(res > 0)
                    {
                        MessageBox.Show("Deleted Data Successfully");
                    }
                 
                }
                
                
            }
            dt = new DataTable();
            InitializeData();
        }

        private void btnNewRecord_Click(object sender, EventArgs e)
        {
            Cleartxt();
            EnableTextBox();
            btnSave.Enabled = true;
            BtnCancel.Enabled = true;
            btnNewRecord.Enabled = false;
            grdview.ClearSelection();

            ifview = false;

            MessageBox.Show(id.ToString());


        }

        private void InsertData()
        {
            string query = "INSERT INTO pirates (piratename, givenname, age, pirategroup ,bounty) Values (?,?,?,?,?)";

            using(conn=new OleDbConnection(ConnStr))
            {
                conn.Open();

                using(cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", txtAlias.Text);
                    cmd.Parameters.AddWithValue("?", txtName.Text);
                    cmd.Parameters.AddWithValue("?", txtAge.Text);
                    cmd.Parameters.AddWithValue("?", cboPiratesGroup.Text);
                    cmd.Parameters.AddWithValue("?", txtBounty.Text);
                    int res = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (res > 0)
                    {
                        MessageBox.Show("Data Successfully Added");
                    }

                }
            }
        }

        public void UpdateData()
        {
            string query = "UPDATE pirates SET piratename = ?, givenname = ?, age = ?, pirategroup = ? , bounty = ? WHERE [id] = ?";

            using (conn = new OleDbConnection(ConnStr))
            {
                conn.Open();

                using (cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue ("?", txtAlias.Text);
                    cmd.Parameters.AddWithValue ("?", txtName.Text);
                    cmd.Parameters.AddWithValue ("?", txtAge.Text);
                    cmd.Parameters.AddWithValue ("?", cboPiratesGroup.Text);
                    cmd.Parameters.AddWithValue ("?", txtBounty.Text);
                    cmd.Parameters.AddWithValue ("?", grdview.SelectedCells[0].Value.ToString());


                    int res = cmd.ExecuteNonQuery();
                    conn.Close();


                    if (res > 0)
                    {
                        MessageBox.Show("Data Successfully Upadated");
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
         
            if(Convert.ToInt32(txtBounty.Text) <0)
            {
                MessageBox.Show("Bounty is invalid", "INVALID INPUT",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }


            if (ifview == false)
            {
                InsertData();
                dt = new DataTable();
                InitializeData();
               
            }
            else
            {
                UpdateData(); 
                dt = new DataTable();
                InitializeData();
               
            }

            DisableTextBox();
            Cleartxt();
            btnNewRecord.Enabled = true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Cleartxt();
            DisableTextBox();
            btnSave.Enabled = false;
            BtnCancel.Enabled = false;
            btnNewRecord.Enabled = true;
        }
    }
}
