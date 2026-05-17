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
    }
}
