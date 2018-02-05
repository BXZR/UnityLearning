using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<mm> m = new List<mm>();

            m.Add(new mm("a", "b", "v"));
            m.Add(new mm("b", "b6", "vw"));
            m.Add(new mm("b", "b4", "v2"));
            m.Add(new mm("n", "rb", "vhg"));
            this.dataGridView1.DataSource = new BindingList<mm>(m);
            //this.dataGridView1.Rows[0].Selected = false;
            this.dataGridView1.Rows[1].DefaultCellStyle.BackColor = Color.Wheat;
            this.dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.Red;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {

                int number = this.dataGridView1.Rows[i].Cells.Count;
                for (int j = 0; j < number; j++)
                {
                    if (this.dataGridView1.Rows[i].Cells[j].Value.ToString() == "v2")
                        this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Magenta;
                }
            }
        }
    }


    public class mm
    {
        public mm(string a,string b,string c)
        { 
             this.aa = a;
            this.bb = b;
            this .cc =c;
        }
        string aa = "d";
        public string AAA { get { return aa; } set { aa = value; } } 
        string bb = "f";
        public string BBB{ get { return bb; } set { bb = value; } } 
        string cc = "f";
        public string CCC { get { return cc; } set { cc = value; } } 

         
    }
}


