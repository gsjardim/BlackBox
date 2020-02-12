using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace caixapretaversao3
{
    public partial class Graficos : Form
    {
        DataTable dados; string nometab;
        //ao abrir a janela de graficos, recebe os parametros tabela e nome da tabela
        public Graficos(DataTable tbl2, string tablename)
        {
            InitializeComponent();
            dados = tbl2; nometab = tablename;
        }

        private void Graficos_Load(object sender, EventArgs e)
        {   //a janela ja executa automaticamente a funcao que cria o grafico.
            CreateGraph_DataSource(zg1);
            SetSize();
        }

        private void Graficos_Resize_1(object sender, EventArgs e)
        {
            SetSize();
        }
        //funcao de redimensionamento da subtela de grafico ajustando ao tamanho da janela.
        private void SetSize()
        {
            zg1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zg1.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 20);

        }
        //funcao que cria os graficos
        private void CreateGraph_DataSource(ZedGraphControl zg1)
        {
            
            //primeiro, limpa qualquer grafico que esteja no painel principal
            MasterPane master = zg1.MasterPane;
            master.PaneList.Clear();

            //Exibe o titulo do painel principal e ajusta a margem externa para 10 pontos
            master.Title.IsVisible = true;
            master.Title.Text = "Dados do Veiculo";
            master.Margin.All = 10;

            
            //Cria quatro paineis secundarios de graficos
            GraphPane pane1 = new GraphPane();
            GraphPane pane2 = new GraphPane();
            GraphPane pane3 = new GraphPane();
            GraphPane pane4 = new GraphPane();

            //adiciona os paineis secundarios ao principal
            master.Add(pane1); //velocidade
            master.Add(pane2); //volante
            master.Add(pane3); //acelerador
            master.Add(pane4); //freio

            // ajusta os titulos, eixos e titulos de cada painel secundario
            pane1.Title.IsVisible = false;
            pane1.XAxis.IsVisible = false;
            pane1.YAxis.Title.Text = "Velocidade";
            pane1.YAxis.Title.FontSpec.Size = 35;
            pane1.YAxis.Scale.FontSpec.Size = 30;
            pane1.YAxis.Scale.Max = 100;
            pane1.YAxis.Scale.Min = 0;

            pane2.Title.IsVisible = false;
            pane2.XAxis.IsVisible = false;
            pane2.YAxis.Title.Text = "Volante";
            pane2.YAxis.Title.FontSpec.Size = 35;
            pane2.YAxis.Scale.FontSpec.Size = 30;
            pane2.YAxis.Scale.Max = 90;
            pane2.YAxis.Scale.Min = -90;

            pane3.Title.IsVisible = false;
            pane3.XAxis.IsVisible = false;
            pane3.YAxis.Title.Text = "Acelerador";
            pane3.YAxis.Title.FontSpec.Size = 35;
            pane3.YAxis.Scale.FontSpec.Size = 30;
            pane3.YAxis.Scale.Max = 110;
            pane3.YAxis.Scale.Min = 0;

            pane4.Title.IsVisible = false;
            pane4.XAxis.Title.Text = "Horario";
            pane4.YAxis.Title.Text = "Freio";
            pane4.XAxis.Title.FontSpec.Size = 35;
            pane4.YAxis.Title.FontSpec.Size = 35;
            pane4.YAxis.Scale.FontSpec.Size = 30;
            pane4.XAxis.Scale.FontSpec.Size = 30;
            pane4.YAxis.Scale.Max = 100;
            pane4.YAxis.Scale.Min = 0;

            //configura os limites dos eixos dos paineis
            zg1.AxisChange();
            
            // Cria a lista de pontos de dados para comunicar com o banco de dados
            DataSourcePointList dspl1 = new DataSourcePointList();
            // cria um adaptador para acessar o banco de dados
            caixapretaTableAdapters.ferrariTableAdapter adapter = new caixapretaTableAdapters.ferrariTableAdapter();
            // Cria uma tabela e preenche-a com dados do banco de dados
            caixapreta.ferrariDataTable table = adapter.GetData();
            
            DataSourcePointList dspl2 = new DataSourcePointList();
            DataSourcePointList dspl3 = new DataSourcePointList();
            DataSourcePointList dspl4 = new DataSourcePointList();

            // Inicia o layout padrao da tela de graficos
            using (Graphics g = this.CreateGraphics())
            {
                master.SetLayout(g, PaneLayout.SquareColPreferred);
            }

            
            // oculta a escala dos eixos dos paineis 1, 2 e 3.
            // oculta as legendas, bordas e titulos dos paineis secundarios
            //pane1.XAxis.Scale.IsVisible = true;            
            pane1.Legend.IsVisible = false;
            pane1.Border.IsVisible = false;
            pane1.Title.IsVisible = false;
            // Get rid of the tics that are outside the chart rect
            pane1.XAxis.MajorTic.IsOutside = false;
            pane1.XAxis.MinorTic.IsOutside = false;
            // Mostra a grade do eixo x
            pane1.XAxis.MajorGrid.IsVisible = true;
            pane1.XAxis.MinorGrid.IsVisible = true;
            // Remove as margens
            pane1.Margin.Right = 10;
            pane1.Margin.Bottom = 60;
            
            //pane2.XAxis.Scale.IsVisible = true;            
            pane2.Legend.IsVisible = false;
            pane2.Border.IsVisible = false;
            pane2.Title.IsVisible = false;            
            pane2.XAxis.MajorTic.IsOutside = false;
            pane2.XAxis.MinorTic.IsOutside = false;            
            pane2.XAxis.MajorGrid.IsVisible = true;
            pane2.XAxis.MinorGrid.IsVisible = true;            
            pane2.Margin.Right = 10;
            pane2.Margin.Bottom = 60;
            
            pane3.Legend.IsVisible = false;
            pane3.Border.IsVisible = false;
            pane3.Title.IsVisible = false;            
            pane3.XAxis.MajorTic.IsOutside = false;
            pane3.XAxis.MinorTic.IsOutside = false;            
            pane3.XAxis.MajorGrid.IsVisible = true;
            pane3.XAxis.MinorGrid.IsVisible = true;            
            pane3.Margin.Right = 10;
            pane3.Margin.Bottom = 60;
                        
            pane4.Legend.IsVisible = false;
            pane4.Border.IsVisible = false;
            pane4.Title.IsVisible = false;
            pane4.XAxis.MajorTic.IsOutside = false;
            pane4.XAxis.MinorTic.IsOutside = false;
            pane4.XAxis.MajorGrid.IsVisible = true;
            pane4.XAxis.MinorGrid.IsVisible = true;
            pane4.Margin.Right = 10;
            
            

            // Deixa um pouco de margem no topo do primeiro painel
            pane1.Margin.Top = 20;

            // e tambem uma margem inferior no painel 4.
            // exibe a escala e titulo do eixo X somente no painel 4.
                        
            pane4.XAxis.Title.IsVisible = true;
            pane4.XAxis.Scale.IsVisible = true;
            pane4.Margin.Bottom = 10;

            pane2.YAxis.Scale.IsSkipLastLabel = true;
            pane3.YAxis.Scale.IsSkipLastLabel = true;
            pane4.YAxis.Scale.IsSkipLastLabel = true;

            //Configura as margens do lado esquerdo e direito, para que os 4 paineis tenham o mesmo tamanho.
            pane1.YAxis.MinSpace = 80;
            pane1.Y2Axis.MinSpace = 20;
            pane2.YAxis.MinSpace = 80;
            pane2.Y2Axis.MinSpace = 20;
            pane3.YAxis.MinSpace = 80;
            pane3.Y2Axis.MinSpace = 20;
            pane4.YAxis.MinSpace = 80;
            pane4.Y2Axis.MinSpace = 20;

            // Specify the table as the data source
            dspl1.DataSource = dados;
            dspl1.XDataMember = "Horario";
            dspl1.YDataMember = "Velocidade";
            dspl1.ZDataMember = null;
            
            dspl2.DataSource = dados;
            dspl2.XDataMember = "Horario";
            dspl2.YDataMember = "Volante";
            dspl2.ZDataMember = null;

            dspl3.DataSource = dados;
            dspl3.XDataMember = "Horario";
            dspl3.YDataMember = "Acelerador";
            dspl3.ZDataMember = null;

            dspl4.DataSource = dados;
            dspl4.XDataMember = "Horario";
            dspl4.YDataMember = "Freio";
            dspl4.ZDataMember = null;
            

            // eixo X será do tipo hora.
            pane1.XAxis.Type = AxisType.Date;
            pane2.XAxis.Type = AxisType.Date;
            pane3.XAxis.Type = AxisType.Date;
            pane4.XAxis.Type = AxisType.Date;

            // Cria a curva
            LineItem myCurve1 = pane1.AddCurve(nometab, dspl1, Color.Blue);
            // Faz com que a linha seja continua
            myCurve1.Line.IsVisible = true;

            LineItem myCurve2 = pane2.AddCurve(nometab, dspl2, Color.Red);
            myCurve2.Line.IsVisible = true;

            LineItem myCurve3 = pane3.AddCurve(nometab, dspl3, Color.Chocolate);
            myCurve3.Line.IsVisible = true;

            LineItem myCurve4 = pane4.AddCurve(nometab, dspl4, Color.BlueViolet);
            myCurve4.Line.IsVisible = true;

            // Exibe as coordenadas (x,y) de cada ponto
            zg1.IsShowPointValues = true;

            
            using (Graphics g = this.CreateGraphics())
            {
                // alinha os 4 graficos verticalmente
                master.SetLayout(g, PaneLayout.SingleColumn);
                master.AxisChange(g);
            }

        }


    }
}
 

