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

namespace Calculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Calculation calculation = new Calculation();
        private bool isDot = false;
        private bool isError = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Clear()
        {
            ResultBlock.Text = "";
            Sentense.Text = "";
            isError = false;
        }

        private void ClickNumber(object sender, RoutedEventArgs e)
        {
            if (isError)
                Clear();
            ResultBlock.Text += ((Button)sender).Content.ToString();
        }

        private void ClickOperation(object sender, RoutedEventArgs e)
        {
            if (isError)
                Clear();
            int len = ResultBlock.Text.Length;
            if ((len > 0 && ResultBlock.Text[len - 1] != '(') || ((Button)sender).Content.ToString() == "-")
            {
                if (len > 0)
                {
                    if (ResultBlock.Text[len - 1] == '+' || ResultBlock.Text[len - 1] == '-' || ResultBlock.Text[len - 1] == '*' || ResultBlock.Text[len - 1] == '/')
                        ResultBlock.Text = ResultBlock.Text.Remove(len - 1);
                }
                isDot = false;
                ResultBlock.Text += ((Button)sender).Content.ToString();
            }
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            if (isError)
                Clear();
            if (ResultBlock.Text.Length != 0)
            {
                string text = ResultBlock.Text;
                int len = text.Length;
                if (text[len - 1] == '+'
                        || text[len - 1] == '-'
                        || text[len - 1] == '*'
                        || text[len - 1] == '/')
                {
                    text = text.Remove(text.Length - 1);
                }
                else if (text[len - 1] == '(')
                {
                    text = text.Remove(text.Length - 1);
                    text = text.Remove(text.Length - 1);
                }
                int end = (text.Where(c => c == '(').Count() - text.Where(c => c == ')').Count());
                for (int i = 0; i < end; i++)
                {
                    text += ')';
                }
                ResultBlock.Text = text;

                Sentense.Text = text + " =";
                ResultBlock.Text = calculation.Compute(ResultBlock.Text);
                if (!ResultBlock.Text.Contains(','))
                {
                    isDot = false;
                }
                else
                {
                    isDot = true;
                }
                try
                {
                    double.Parse(ResultBlock.Text);
                }
                catch
                {
                    isError = true;
                }
            }
        }

        private void ClickBreket(object sender, RoutedEventArgs e)
        {
            if (isError)
                Clear();
            int len = ResultBlock.Text.Length;
            if (((Button)sender).Content.ToString() == "(" 
                && (len == 0 
                    || ResultBlock.Text[len - 1] == '+' 
                    || ResultBlock.Text[len - 1] == '-' 
                    || ResultBlock.Text[len - 1] == '*'
                    || ResultBlock.Text[len - 1] == '/'))
            {
                ResultBlock.Text += ((Button)sender).Content.ToString();
                isDot = false;
            }
            else if (((Button)sender).Content.ToString() == ")"
                && ResultBlock.Text.Where(c => c == '(').Count() > ResultBlock.Text.Where(c => c == ')').Count()
                && (ResultBlock.Text[len - 1] == '0' 
                || ResultBlock.Text[len - 1] == '1'
                || ResultBlock.Text[len - 1] == '2'
                || ResultBlock.Text[len - 1] == '3'))
            {
                ResultBlock.Text += ")";
                isDot = false;
            }
        }


        private void ClickDot(object sender, RoutedEventArgs e)
        {
            if (isError)
                Clear();
            if (!isDot)
            {
                int len = ResultBlock.Text.Length;
                if (len == 0
                    || ResultBlock.Text[len - 1] == '('
                    || ResultBlock.Text[len - 1] == '+'
                    || ResultBlock.Text[len - 1] == '-'
                    || ResultBlock.Text[len - 1] == '*'
                    || ResultBlock.Text[len - 1] == '/')
                {
                    ResultBlock.Text += "0,";
                }
                else
                {
                    ResultBlock.Text += ',';
                }
                isDot = true;
            } 
        }


        private void ClickCleanAll(object sender, RoutedEventArgs e)
        {
            ResultBlock.Text = "";
            isDot = false;
        }

        private void ClickClean(object sender, RoutedEventArgs e)
        {
            if (isError)
                Clear();
            if (ResultBlock.Text.Length > 0)
            {
                if (ResultBlock.Text[ResultBlock.Text.Length - 1] == ',')
                {
                    isDot = false;
                }
                ResultBlock.Text = ResultBlock.Text.Remove(ResultBlock.Text.Length - 1);
            }
        }
    }
}
