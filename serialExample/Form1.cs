using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using caixapretaversao3;
using ZedGraph;
using System.Data.OleDb;
using ADOX;

namespace Serialexpample
{
   
    public partial class Form1 : Form
    {
        //cria alguns objetos que serao usados no programa      
        DataTable tabela_temp = new DataTable();//tabela para armazenagem temporaria dos dados
        //objetos de conexao com o banco de dados do Access.
        OleDbConnection MAconn;
        OleDbCommand MAcmd;
        //variavel para manipular os dados de data e hora
        private DateTime dtime;
        int tc = 0; // variavel que determina se uma tabela já foi aberta, quando diferente de 0.
        //vetor onde serao armazenados os bytes lidos na porta serial.
        int[] vetorout = new int[516]; 
      
        //cria uma instancia da tela de propriedades
        pp newpp = new pp();

        //create an Serial Port object
        SerialPort sp = new SerialPort("COM4");

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //Show indica os componentes explicitos ao iniciar o programa.
            //Hide indica os componentes ocultos ao iniciar o programa.
            pictureBox1.Show();
            panelhistory.Hide();
            panelnovo.Hide();
            panellabels.Hide();
            saveStatusButton.Hide();
            propertyButton.Hide();
            startCommButton.Hide();
            graficosButton.Hide();
            button1.Hide();
            endButton.Hide();
            tabelabutton.Hide();
            
        }

        private void propertyButton_Click_1(object sender, EventArgs e)
        {
            //show property dialog

            newpp.ShowDialog();
            

        }

        private void saveStatusButton_Click(object sender, EventArgs e)
        {
            panellabels.Show();
            //Exibe os valores das configuraçoes.
            //Se nenhum valor foi alterado em propriedades, salva os valores padroes.
            if (newpp.bRate == "" && newpp.sBits == "")
            {
                baudRatelLabel.Text = "BaudRate = " + sp.BaudRate.ToString();
                stopBitLabel.Text = "StopBits = " + sp.StopBits.ToString();
            }
            else
            {
                baudRatelLabel.Text = "BaudRate = " + newpp.bRate;
                stopBitLabel.Text = "StopBits = " + newpp.sBits;
            }

            parityLabel.Text = "Parity = " + sp.Parity.ToString();
            dataBitLabel.Text = "Data Bits = " + sp.DataBits.ToString();
            readTimeOutLabel.Text = "ReadTimeout = " + sp.ReadTimeout.ToString();

            propertyButton.Enabled = false;
            saveStatusButton.Enabled=false;
            startCommButton.Enabled=true;


            try
            {
                //abre porta serial
                sp.Open();
                //ajusta o timeout para 3 segundos
                sp.ReadTimeout = 3000;
                if (sp.IsOpen == true)
                { messagelabel.Text = "Porta serial aberta"; }
                
            }
            catch (System.Exception ex)
            {
                messagelabel.Text = ex.Message;
            }

        }

        private void startCommButton_Click_1(object sender, EventArgs e)
        {
            startCommButton.Enabled = false;
            button1.Show();
            graficosButton.Show();
            graficosButton.Enabled = false;
            endButton.Show();
            tabelabutton.Show();
            tabelabutton.Enabled = false;
            
            
        }

        private void graficosButton_Click(object sender, EventArgs e)
        {
            Graficos newGraficos = new Graficos(tabela_temp,maintitle.Text);
            newGraficos.ShowDialog(); 
        }

        //botao recebe (envia solicitacao ao pic, recebe os dados e converte em dados internos)
        private void button1_Click(object sender, EventArgs e)
        {
                        
            try
            {
                //Envia '1' ao Pic como solicitacao de transmissao
                sp.WriteLine("1");                
            }
            catch (System.Exception ex)
            {
                messagelabel.Text = ex.Message;
            }
            
            string errocom="";
            try
            {
                int k;//loop para leitura dos bytes na porta serial
                for (k = 2; k == 515; k++)
                {
                    vetorout[k] = sp.ReadByte();
                }
                //if (k == 516)
                MessageBox.Show("Dados recebidos com sucesso");
            }
            
            catch (System.Exception ex)
            {
                messagelabel.Text = ex.Message;
                errocom = ex.Message;
            }
            
            //Se a recepçao foi correta, o programa trata os bytes convertendo-os
            //em informaçoes de data e hora, dados do veiculo e grava tudo na tabela do Acess.
            if (1==1)//messagelabel.Text == errocom)
            {
                
                graficosButton.Enabled = true;
                tabelabutton.Enabled = true;

                //DataTable tabela_temp: tabela interna para visualizacao dos dados.
                if (tc == 0)
                {
                    //tabela_temp.Columns.Add("Ref", typeof(int));
                    tabela_temp.Columns.Add("Data", typeof(DateTime));
                    tabela_temp.Columns.Add("Horario", typeof(string));
                    tabela_temp.Columns.Add("Velocidade", typeof(double));
                    tabela_temp.Columns.Add("Volante", typeof(double));
                    tabela_temp.Columns.Add("Acelerador", typeof(double));
                    tabela_temp.Columns.Add("Freio", typeof(double));
                    tc++;
                }
                //este passo converte primeiramente os bytes referente à informacao de hora.
                //os valores estarao em formato BCD, logo criamos essa rotina para transformar em decimal.
                int[] data = new int[7];
                for (int j = 1; j < 7; j++)
                {
                    if (j == 1)//RTC byte 01: centesimos BCD para decimal
                    {
                        if (vetorout[j] < 10)
                        { data[j] = vetorout[j]; }
                        if (vetorout[j] > 15 && vetorout[j] < 26)
                        { data[j] = vetorout[j] - 6; }
                        if (vetorout[j] > 31 && vetorout[j] < 42)
                        { data[j] = vetorout[j] - 12; }
                        if (vetorout[j] > 47 && vetorout[j] < 58)
                        { data[j] = vetorout[j] - 18; }
                        if (vetorout[j] > 63 && vetorout[j] < 74)
                        { data[j] = vetorout[j] - 24; }
                        if (vetorout[j] > 79 && vetorout[j] < 90)
                        { data[j] = vetorout[j] - 30; }
                        if (vetorout[j] > 95 && vetorout[j] < 106)
                        { data[j] = vetorout[j] - 36; }
                        if (vetorout[j] > 111 && vetorout[j] < 122)
                        { data[j] = vetorout[j] - 42; }
                        if (vetorout[j] > 127 && vetorout[j] < 138)
                        { data[j] = vetorout[j] - 48; }
                        if (vetorout[j] > 143 && vetorout[j] < 154)
                        { data[j] = vetorout[j] - 54; }
                    }

                    if (j == 2)//RTC byte 02: segundos BCD para decimal.
                    {
                        if (vetorout[j] < 10)
                        { data[j] = vetorout[j]; }
                        if (vetorout[j] > 15 && vetorout[j] < 26)
                        { data[j] = vetorout[j] - 6; }
                        if (vetorout[j] > 31 && vetorout[j] < 42)
                        { data[j] = vetorout[j] - 12; }
                        if (vetorout[j] > 47 && vetorout[j] < 58)
                        { data[j] = vetorout[j] - 18; }
                        if (vetorout[j] > 63 && vetorout[j] < 74)
                        { data[j] = vetorout[j] - 24; }
                        if (vetorout[j] > 79 && vetorout[j] < 90)
                        { data[j] = vetorout[j] - 30; }
                    }

                    if (j == 3)//RTC byte 03: minutos BCD para decimal.
                    {
                        if (vetorout[j] < 10)
                        { data[j] = vetorout[j]; }
                        if (vetorout[j] > 15 && vetorout[j] < 26)
                        { data[j] = vetorout[j] - 6; }
                        if (vetorout[j] > 31 && vetorout[j] < 42)
                        { data[j] = vetorout[j] - 12; }
                        if (vetorout[j] > 47 && vetorout[j] < 58)
                        { data[j] = vetorout[j] - 18; }
                        if (vetorout[j] > 63 && vetorout[j] < 74)
                        { data[j] = vetorout[j] - 24; }
                        if (vetorout[j] > 79 && vetorout[j] < 90)
                        { data[j] = vetorout[j] - 30; }
                    }

                    if (j == 4)//RTC byte 04: horas BCD para decimal.
                    {
                        if (vetorout[j] < 10)
                        { data[j] = vetorout[j]; }
                        if (vetorout[j] > 15 && vetorout[j] < 26)
                        { data[j] = vetorout[j] - 6; }
                        if (vetorout[j] > 31 && vetorout[j] < 36)
                        { data[j] = vetorout[j] - 12; }
                    }

                    if (j == 5)//RTC byte 05: dia do mes BCD para decimal.
                    {
                        if (vetorout[j] < 10)
                        { data[j] = vetorout[j]; }
                        if (vetorout[j] > 15 && vetorout[j] < 26)
                        { data[j] = vetorout[j] - 6; }
                        if (vetorout[j] > 31 && vetorout[j] < 42)
                        { data[j] = vetorout[j] - 12; }
                        if (vetorout[j] > 47 && vetorout[j] < 50)
                        { data[j] = vetorout[j] - 18; }
                    }

                    if (j == 6)//RTC byte 06: mes BCD para decimal.
                    {

                        if (vetorout[j] < 10)
                        { data[j] = vetorout[j]; }
                        if (vetorout[j] > 15 && vetorout[j] < 19)
                        { data[j] = vetorout[j] - 6; }

                    }
                }

                //atribui os valores das variaveis referentes ao horario e data.                        

                string nome = maintitle.Text;
                int hour, minute, second, hund_second, year, month, day;
                year = 2009; month = data[6]; 
                day = data[5];
                hour = data[4];
                minute = data[3];
                second = data[2];
                hund_second = data[1];

                //faz conexao com o banco de dados no Acess: caixapreta_teste2.mdb
                
                //fecha alguma outra conexao caso esteja aberta.
                if (MAconn != null && MAconn.State == ConnectionState.Open)
                {
                    MAconn.Close();
                    MAcmd.Dispose();
                }
                //abre a nova conexao.
                try
                {
                    MAconn = new OleDbConnection();
                    MAconn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=C:\\Documents and Settings\\User\\Meus documentos\\Visual Studio 2008\\Projects\\caixapretaversao3\\caixapretaversao3\\caixapreta_teste2.MDB";
                    MAconn.Open();
                    MAcmd = MAconn.CreateCommand();

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //nesta rotina abaixo, os valores em decimais serao convertidos para formato de hora
                //centesimos: base 10, Segundos, minutos: base 60, horas: base 24, dias: base 31, meses: base 12.
                int indexdados;
                vetorout[0] = 0;
                double Accel = 0, wheel = 0, speed = 0, Brake = 0;
                int size = vetorout.Length - vetorout[0]; //int h = (size - 7) / 4;
                
                

                
                for (indexdados = 7; indexdados < (size - 2); indexdados++)
                {

                    //trata os centesimos
                   // if (hund_second >= 50)
                    //{
                   //     hund_second = hund_second - 50;
                    //}
                    //else
                    //{   //trata os segundos
                      //  hund_second = hund_second + 50;
                        if (second != 0)
                        {
                            second = second - 1;

                        }
                        else
                        {
                            second = 59;
                            if (minute != 0)
                            {   //trata os minutos
                                minute = minute - 1;

                            }
                            else
                            {   
                                minute = 59;
                                if (hour != 0)
                                {   //trata as horas
                                    hour = hour - 1;

                                }
                                else
                                {
                                    hour = 23;

                                }
                            }
                        }
                    //}


                    //converte os valores das variaveis de decimal para os correspondentes.
                    //velocidade: 0 a 100 km/h.
                    speed = ((vetorout[indexdados] * 100) / 255);
                    //volante: -90 a +90 graus
                    wheel = ((vetorout[indexdados + 1] * 180) / 255) - 90;
                    //acelerador: 0 a 100%
                    Accel = (vetorout[indexdados + 2] * 100) / 255;
                    //freio: 0 a 100%
                    Brake = (vetorout[indexdados + 3]* 100) / 255;

                    dtime = new DateTime(year, month, day, hour, minute, second);

                    string result1, result2;
                    //divide a varivel dtime em parte de horas e de datas.
                    result2 = dtime.ToShortDateString();
                    result1 = dtime.ToLongTimeString();

                    //os comandos abaixo gravam todos os dados na tabela do Acess criada no inicio.
                    tabela_temp.Rows.Add(dtime, result1, speed, wheel, Accel, Brake);

                    MAcmd.CommandText = "INSERT INTO " + nome + "(Horario, Data, Acelerador, Freio, Volante, Velocidade ) VALUES ('" + dtime + "','" + result1 + "','" + Accel + "','" + Brake + "','" + wheel + "','" + speed + "')";
                    try
                    {
                        MAcmd.ExecuteNonQuery();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    indexdados = indexdados + 3;
                    //h = h - 1;

                }
                //insert into historicos: grava o nome desta tabela na tabela historicos, para consultas futuras.
                MAcmd.CommandText = "INSERT INTO historico (Nome) VALUES ('" + maintitle.Text + "')";
                try
                {
                    MAcmd.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //fecha a conexao com o banco de dados
                MAcmd.Dispose();
                MAconn.Close();
            }
        }

        private void endButton_Click(object sender, EventArgs e)
        {
            sp.Close();
            MessageBox.Show("Comunicaçao finalizada");
            startCommButton.Hide();
            saveStatusButton.Hide();
            propertyButton.Hide();
            graficosButton.Hide();
            button1.Hide();
            endButton.Hide();
            tabelabutton.Hide();
            tabela_temp.Clear();
            maintitle.Text = "Caixa Preta";
            panellabels.Hide();
            saveStatusButton.Enabled = true;
            propertyButton.Enabled = true;
            pictureBox1.Show();
        }

        //botao abrir historico: permite consultar registros anteriores salvos no database.
        private void abrirHistoricoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (panelnovo.Visible == true)
            { panelnovo.Hide(); }

            pictureBox1.Hide();
            panelhistory.Show();
            newfilelabel.Text = "Selecione o registro desejado";
            //abre conexao com o banco de dados.
            if (MAconn != null && MAconn.State == ConnectionState.Open)
            {
                MAconn.Close();
                MAcmd.Dispose();
            }
            try
            {
                MAconn = new OleDbConnection();
                MAconn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=C:\\Documents and Settings\\User\\Meus documentos\\Visual Studio 2008\\Projects\\caixapretaversao3\\caixapretaversao3\\caixapreta_teste2.MDB";
                MAconn.Open();
                MAcmd = MAconn.CreateCommand();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MAcmd.CommandText = "SELECT Nome from historico Order by Nome";

            OleDbDataReader tabela = MAcmd.ExecuteReader();
            //le os nomes das tabelas existentes e exibe no listbox da tela.
            while (tabela.Read())
            {
                historicolist.Items.Add(tabela.GetString(0));
            }
            //fecha a conexao e o comando de leitura tabela.
            MAconn.Close();
            MAcmd.Dispose();
            tabela.Close();
            tabela.Dispose();
        }

        
        //abre outra janela para exibir a tabela de dados
        private void tabelabutton_Click(object sender, EventArgs e)
        {
            
            tabela tabela = new tabela(tabela_temp);
            tabela.Show();
        }
        //sai do aplicativo caixa preta
        private void sairToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Deseja mesmo encerrar o aplicativo?");
            sp.Close();
            Close();

        }
        //menu nova conexao: cria uma nova tabela de dados com o nome inserido.
        private void novaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (panelhistory.Visible == true)
            { panelhistory.Hide(); }

            pictureBox1.Hide();
            panelnovo.Show();
            textBox1.Clear();
                        
        }
        //botao ok da nova conexao: cria a nova tabela com o nome digitado
        private void OKnew_Click(object sender, EventArgs e)
        {
            //cria um novo banco de dados do Access.
            CatalogClass cat = new CatalogClass();
            string tmpStr,tmpStr2;
            tmpStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Documents and Settings\\User\\Meus documentos\\Visual Studio 2008\\Projects\\caixapretaversao3\\caixapretaversao3\\caixapreta_teste2.mdb;Jet OLEDB:Engine Type=5";
            cat.let_ActiveConnection(tmpStr);//conecta com o banco de dados

            Table nTable = new Table();
            tmpStr2 = textBox1.Text;
            nTable.Name = tmpStr2;
            maintitle.Text = tmpStr2;
            //cria as colunas dentro da nova tabela.
            //nTable.Columns.Append("Ref", DataTypeEnum.adInteger, 1);
            nTable.Columns.Append("Horario", DataTypeEnum.adDate, 1);
            nTable.Columns.Append("Data", DataTypeEnum.adVarWChar, 40);
            nTable.Columns.Append("Velocidade", DataTypeEnum.adInteger, 1);
            nTable.Columns.Append("Volante", DataTypeEnum.adInteger, 1);
            nTable.Columns.Append("Acelerador", DataTypeEnum.adInteger, 1);
            nTable.Columns.Append("Freio", DataTypeEnum.adInteger, 1);
            
            cat.Tables.Append(nTable);//cria a tabela conforme especificado acima
            //encerra o comando que cria a tabela.
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(nTable);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat.Tables);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat.ActiveConnection);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat);
            
            panelnovo.Hide();
            saveStatusButton.Show();
            propertyButton.Show();
            startCommButton.Show();
            startCommButton.Enabled = false;
            
        }
        //botao cancela nova conexao. fecha esta tela.
        private void cancelnew_Click(object sender, EventArgs e)
        {
            panelnovo.Hide();
            pictureBox1.Show();
        }
        //botao cancela abrir historico: fecha esta tela.
        private void historicocancel_Click(object sender, EventArgs e)
        {
            tabela_temp.Clear();
            historicolist.Items.Clear();
            panelhistory.Hide();
            pictureBox1.Show();
        }
        //botao abrir historico: abre a tela de grafico ou tabela, conforme ticado pelo usuario.
        private void abrirbutton_Click(object sender, EventArgs e)
        {
            if (MAconn != null && MAconn.State == ConnectionState.Open)
            {
                MAconn.Close();
                MAcmd.Dispose();
            }
            try
            {   //abre conexao com o banco de dados
                MAconn = new OleDbConnection();
                MAconn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=C:\\Documents and Settings\\User\\Meus documentos\\Visual Studio 2008\\Projects\\caixapretaversao3\\caixapretaversao3\\caixapreta_teste2.MDB";
                MAconn.Open();
                MAcmd = MAconn.CreateCommand();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            tabela_temp.Clear();//limpa o objeto tabela, caso ja tenha sido usado.
            string tablename = historicolist.SelectedItem.ToString();
            if (tc == 0)
            {   //cria uma nova tabela interna com as colunas necessarias.
                //tabela_temp.Columns.Add("Ref", typeof(int));
                tabela_temp.Columns.Add("Horario", typeof(DateTime));
                tabela_temp.Columns.Add("Data", typeof(string));
                tabela_temp.Columns.Add("Velocidade", typeof(double));
                tabela_temp.Columns.Add("Volante", typeof(double));
                tabela_temp.Columns.Add("Acelerador", typeof(double));
                tabela_temp.Columns.Add("Freio", typeof(double));
                
                tc++;
            }

            MAcmd.CommandText = "SELECT Horario, Data, Velocidade, Volante, Acelerador, Freio from " + tablename + " Order by Horario";
            

            OleDbDataReader tabela = MAcmd.ExecuteReader();
            //le os dados da tabela do Access e escreve na tabela interna do programa
            while (tabela.Read())
            {                                
                tabela_temp.Rows.Add(tabela.GetDateTime(0),tabela.GetString(1),tabela.GetInt32(2),tabela.GetInt32(3),tabela.GetInt32(4),tabela.GetInt32(5));
            }

            tabela.Close();
            tabela.Dispose();
            
            MAcmd.Dispose();
            MAconn.Close();
            
            if (tabcheckBox.Checked == true)
            {
                tabela tab = new tabela(tabela_temp);
                tab.Show();
                //tabelahist tab = new tabelahist(tabela_temp);
                //tab.Show();//abre tela das tabelas se esta caixa estiver ticada
            }
            if (grafcheckBox.Checked == true)
            {
                Graficos grafscreen = new Graficos(tabela_temp,tablename);
                grafscreen.Show();//abre a tela dos graficos se esta caixa estiver ticada
            }

        }
                
        //exibe o texto 'propriedades' ao passar o mouse sobre este botao
        private void propertyButton_MouseHover(object sender, EventArgs e)
        {            
            toolTipprop.Show("Propriedades",propertyButton);
        }
        //abre uma janela com informaçoes sobre a autoria da caixa preta
        private void sobreCaixapretaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sobre tela = new sobre();
            tela.Show();
        }
        //exibe o texto 'save status' ao passar o mouse sobre este botao
        private void saveStatusButton_MouseHover(object sender, EventArgs e)
        {
            toolTipsave.Show("save status", saveStatusButton);
        }
        //exibe o texto 'start com' ao passar o mouse sobre este botao
        private void startCommButton_MouseHover(object sender, EventArgs e)
        {
            toolTipcom.Show("start com", startCommButton);
        }
        //abre um arquivo txt com instruçoes de uso do programa.
        private void conteudoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "notepad";
            proc.StartInfo.Arguments = @"C:\Documents and Settings\User\Meus documentos\Visual Studio 2008\Projects\caixapretaversao3\caixapretaversao3\bin\Debug\Ajuda.txt";
            proc.Start();
        }

        //funcao de redimensionamento da subtela de grafico ajustando ao tamanho da janela.
        /*private void SetSize()
        {
            zg1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zg1.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 20);

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetSize();
        }*/
       
        
    }
}
 
