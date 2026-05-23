using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Stock741.Views
{
    public partial class StockView : UserControl
    {
        private static readonly Regex EntierNaturelRegex = new("^[0-9]+$");

        public StockView()
        {
            InitializeComponent();
        }

        private void EntierNaturelTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !EntierNaturelRegex.IsMatch(e.Text);
        }

        private void StocksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem != null)
            {
                dataGrid.Dispatcher.BeginInvoke(() =>
                {
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                    dataGrid.Focus();
                });
            }
        }
    }
}
