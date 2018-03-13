using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace OracleWPF
{
    /// <summary>
    /// Interaction logic for PedidoWindow.xaml
    /// </summary>
    public partial class PedidoWindow : Window
    {
        string ID = "";
        MainWindow main;
        public PedidoWindow(string id)
        {
            InitializeComponent();

            main = ((MainWindow)App.Current.MainWindow);
            ID = id;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ID != "")
            {
                LoadPedido();
            }
            else
            {
                GetNextID();
            }
        }


        private void GetNextID()
        {
            using (var conn = new OracleConnection(main.connString))
            {
                OracleCommand cmd = new OracleCommand($"SELECT MAX(ID) as NextID from PEDIDO", conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                var dr = cmd.ExecuteReader();
                dr.Read();

                var id = dr.GetOracleDecimal(dr.GetOrdinal("NEXTID"));
                IDTextBox.Text = (id + 1).ToString();
            }
        }

        private void LoadPedido()
        {
            using (var conn = new OracleConnection(main.connString))
            {
                OracleCommand cmd = new OracleCommand($"SELECT * from PEDIDO where ID = {ID}", conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                var dr = cmd.ExecuteReader();


                dr.Read();

                var id = ID;
                var Cliente = dr.GetOracleString(dr.GetOrdinal("CLIENTE"));
                var Data = dr.GetOracleDate(dr.GetOrdinal("DATA"));
                var total = dr.GetOracleDecimal(dr.GetOrdinal("TOTAL"));

                IDTextBox.Text = ID;
                CLIENTETextBox.Text = Cliente.Value;
                DATADatePicker.SelectedDate = Data.Value;
                TOTALTextBox.Text = string.Format("{0:N}", total);


            }
        }


        private bool Validar()
        {
            var erros = "";

            if (CLIENTETextBox.Text == "") erros += "Nome do Cliente Vazio!\n";
            if (TOTALTextBox.Text == "") erros += "Total Vazio!\n";
            else
            {
                decimal total = 0;
                if (!decimal.TryParse(TOTALTextBox.Text, out total)) erros += "Total não é um número válido!\n";
            }
            if (DATADatePicker.SelectedDate == null) erros += "Selecione uma data!\n";


            if (erros.Length == 0) return true;
            else
            {
                MessageBox.Show(erros);
                return false;
            }
        }


        private void SalvarPedido()
        {

            if (!Validar()) return;

            var id = IDTextBox.Text;
            var cliente = CLIENTETextBox.Text;
            var data = DATADatePicker.SelectedDate.Value.ToShortDateString();
            var total = TOTALTextBox.Text.Replace(".", "").Replace(",", ".");

            if (ID == "")
            {
                var ins = $"INSERT INTO PEDIDO (ID, CLIENTE, DATA, TOTAL) VALUES ({id}, '{cliente}', '{data}', {total})";
                main.RunSQL(ins);
            }
            else
            {
                var upd = $"UPDATE PEDIDO SET CLIENTE = '{cliente}', DATA = '{data}', TOTAL = {total} WHERE ID = {id} ";
                main.RunSQL(upd);
            }

            MessageBox.Show("Pedido Salvo!");
            Close();
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            SalvarPedido();
          
        }
    }
}
