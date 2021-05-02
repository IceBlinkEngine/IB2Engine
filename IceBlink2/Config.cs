using Newtonsoft.Json;
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

namespace IceBlink2
{
    public partial class Config : Form
    {
        public int width = 528;
        public int height = 336;
        public string mainDirectory;
        public GameView gv;

        public Config(GameView g)
        {
            InitializeComponent();
            gv = g;
            mainDirectory = Directory.GetCurrentDirectory();            
        }

        private void btn336_Click(object sender, EventArgs e)
        {
            width = 768;
            height = 480;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btn672_Click(object sender, EventArgs e)
        {
            width = 1280;
            height = 720;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btn1008_Click(object sender, EventArgs e)
        {
            width = 1920;
            height = 1080;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnFull_Click(object sender, EventArgs e)
        {
            width = -1;
            height = -1;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCustom_Click(object sender, EventArgs e)
        {
            width = (int)numWidth.Value;
            height = (int)numHeight.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }        
    }
}
