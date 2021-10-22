using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AISBOT
{
    public partial class Setting_Num : Form
    {
        public Setting_Num()
        {
            InitializeComponent();
        }

        private void nsTheme1_Click(object sender, EventArgs e)
        {

        }

        private void Setting_Num_Load(object sender, EventArgs e)
        {
            if (File.Exists("Numsetting.txt"))
            {
                richTextBox1.Text = File.ReadAllText("Numsetting.txt");
            }
        }

        private void nsButton1_Click(object sender, EventArgs e)
        {
            File.WriteAllText("Numsetting.txt", richTextBox1.Text);
            MessageBox.Show("บันทึกเรียบร้อยแล้ว", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
