using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Serialexpample
{
    partial class pp : Form
    {
        //variaveis para guardar valores do Baud rate e stop bits.
        private string baudR = "";
        private string stopB = "";

        //propridades para obter e fixar os valores do baud rate e stop bits.
        public string bRate
        {
            get
            {
                return baudR;
            }
            set
            {
                baudR = value;
            }
        }

        public string sBits
        {
            get
            {
                return stopB;
            }
            set
            {
                stopB = value;
            }
        }

        public pp()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            //aqui obtemos os valores digitados de Baud rate e stop bits.
            this.bRate = BaudRateComboBox.Text;
            this.sBits = stopBitComboBox.Text;
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.bRate = "";
            this.sBits = "";
            this.Close();
        }

        private void pp_Load(object sender, EventArgs e)
        {

        }
    }
}
 
