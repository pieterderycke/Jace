using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jace.DemoApp
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog(string variableName)
        {
            InitializeComponent();
            questionLabel.Content = string.Format("Please provide a value for variable \"{0}\":", variableName);
        }

        public double Value 
        {
            get
            {
                double result;
                if (double.TryParse(valueTextBox.Text, out result))
                    return result;
                else
                    return 0.0;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
