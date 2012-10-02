using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Calculator.Operations;
using Calculator.Tokenizer;

namespace Calculator.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            string formula = formulaTextBox.Text;

            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read(formula);

            ShowTokens(tokens);

            AstBuilder astBuilder = new AstBuilder();
            Operation operation = astBuilder.Build(tokens);

            ShowAbstractSyntaxTree(operation);

            IInterpreter interpreter = new BasicInterpreter();
            double result = interpreter.Execute(operation);

            resultTextBox.Text = "" + result;
        }

        private void ShowTokens(List<object> tokens)
        { 
            string result = "[ ";

            for(int i = 0; i < tokens.Count; i++)
            {
                object token = tokens[i];

                if (token.GetType() == typeof(string))
                    result += "\"" + token + "\"";
                else if (token.GetType() == typeof(char))
                    result += "'" + token + "'";
                else if (token.GetType() == typeof(double) || token.GetType() == typeof(int))
                    result += token;

                if (i < (tokens.Count - 1))
                    result += ", ";
            }

            result += " ]";

            tokensTextBox.Text = result;
        }

        private void ShowAbstractSyntaxTree(Operation operation)
        {
            astTreeView.Items.Clear();

            TreeViewItem item = CreateTreeViewItem(operation);
            astTreeView.Items.Add(item);
        }

        private TreeViewItem CreateTreeViewItem(Operation operation)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = GetLabelText(operation);

            if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;

                item.Items.Add(CreateTreeViewItem(multiplication.Argument1));
                item.Items.Add(CreateTreeViewItem(multiplication.Argument2));
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;

                item.Items.Add(CreateTreeViewItem(addition.Argument1));
                item.Items.Add(CreateTreeViewItem(addition.Argument2));
            }
            else if (operation.GetType() == typeof(Substraction))
            {
                Substraction addition = (Substraction)operation;

                item.Items.Add(CreateTreeViewItem(addition.Argument1));
                item.Items.Add(CreateTreeViewItem(addition.Argument2));
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;

                item.Items.Add(CreateTreeViewItem(division.Dividend));
                item.Items.Add(CreateTreeViewItem(division.Divisor));
            }

            return item;
        }

        private string GetLabelText(Operation operation)
        {
            Type operationType = operation.GetType();

            if (operationType.IsGenericType)
            {
                string name = operationType.Name.Substring(0, operationType.Name.LastIndexOf('`'));
                string dataType = GetDataTypeName(operationType.GetGenericArguments()[0]);
                string value = "";

                Constant<int> integerConstant = operation as Constant<int>;
                if (integerConstant != null)
                {
                    value = "(" + integerConstant.Value + ")";
                }
                else
                {
                    Constant<double> floatingPointConstant = operation as Constant<double>;
                    if (floatingPointConstant != null)
                        value = "(" + floatingPointConstant.Value + ")";
                }

                return string.Format("{0}<{1}>{2}", name, dataType, value);
            }
            else
            {
                return operationType.Name;
            }
        }

        private string GetDataTypeName(Type dataType)
        {
            switch (dataType.FullName)
            {
                case "System.Int32":
                    return "int";
                case "System.Double":
                    return "double";
                default:
                    return dataType.Name;
            }
        }
    }
}
