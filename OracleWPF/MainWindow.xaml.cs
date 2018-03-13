using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.DataAccess.Client;
using System.Data;

namespace OracleWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string connString = "";


        public MainWindow()
        {
            InitializeComponent();

            connString = BuildConnString();
        }



        #region Database

        private void LoadDatabase(string Filter)
        {
            using (var conn = new OracleConnection(connString))
            {
                dataGridOracle.DataContext = null;

                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from pedido";

                if (Filter != "")
                {
                    cmd.CommandText += $" WHERE LOWER(CLIENTE) LIKE LOWER('%{Filter}%')";
                }

                cmd.CommandType = CommandType.Text;
                

                DataSet ds = new DataSet();
                OracleDataAdapter da = new OracleDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                dataGridOracle.DataContext = ds.Tables[0];

            }
        }

        private string BuildConnString()
        {
            return "User Id=beto; password=betopass; Data Source=127.0.0.1:1521/XE;";
        }

        public bool RunSQL(string SQL, bool showMessageBox = false)
        {
            try
            {
                using (var conn = new OracleConnection(connString))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = SQL;

                    cmd.ExecuteScalar();

                    return true;
                
                }
            }
            catch (Exception ex)
            {
                if (showMessageBox) MessageBox.Show("Erro ao executar comando! - " + ex.ToString());

                return false;
            }
            
        }

        private void DeletePedido(string ID)
        {

            var result = MessageBox.Show("Confirma exclusão do pedido?", "EXCLUSÃO", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;

            RunSQL("DELETE FROM PEDIDO WHERE ID = " + ID);

            LoadDatabase(textBoxFiltro.Text);
        }

        #endregion

        #region Control Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDatabase("");
        }


        private void UpdatePedido()
        {
            var ID = dataGridOracle.SelectedValue.ToString();
            var pedWin = new PedidoWindow(ID);
            pedWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            pedWin.Closed += PedWin_Closed;
            pedWin.ShowDialog();
        }

        private void dataGridOracle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridOracle.SelectedValue == null) return;

            UpdatePedido();
        }


        private void buttonFiltro_Click(object sender, RoutedEventArgs e)
        {
            LoadDatabase(textBoxFiltro.Text);
        }

        private void buttonNovo_Click(object sender, RoutedEventArgs e)
        {
            var pedWin = new PedidoWindow("");
            pedWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            pedWin.Closed += PedWin_Closed;
            pedWin.ShowDialog();
        }

        private void PedWin_Closed(object sender, EventArgs e)
        {
            LoadDatabase(textBoxFiltro.Text);
        }

        #endregion

        private void dataGridOracle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (dataGridOracle.SelectedValue != null)
                {
                    DeletePedido(dataGridOracle.SelectedValue.ToString());
                }
            }
        }
    }
}
