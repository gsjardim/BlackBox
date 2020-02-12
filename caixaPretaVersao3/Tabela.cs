using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Serialexpample;

namespace caixapretaversao3
{
    public partial class tabela : Form
    {
                
        public tabela(DataTable tbl)
        {
            InitializeComponent();
            //ao abrir esta janela, recebe como parametro a variavel tabela_temp, do Form 1.
            dataGridView1.DataSource = tbl;
            
        }

        private void tabela_Load(object sender, EventArgs e)
        {
            
        }

        
    }
}
