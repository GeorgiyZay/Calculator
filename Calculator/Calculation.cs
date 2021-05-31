
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator
{
    class Calculation
    {
        private Regex number = new Regex(@"[-+]?[0-3]*\,?[0-3]+");
        private Regex operation = new Regex(@"[+\-*/]");
        private Regex innerNumber = new Regex(@"[0-3]+");
        private Regex exceptions = new Regex(@"-\(-[0-3]*\,?[0-3]+\)");


        private int GetPriority(char operation)
        {
            return (operation == '+' || operation == '-') ? 0 : 1;
        }


        private double ToDecimalNumber(string str)
        {
            string[] number = new string[2];
            if (str.Contains(','))
            {
                number = str.Split(',');
            }
            else
            {
                number[0] = str;
                number[1] = "0";
            }
            int sign;
            if (number[0].Contains('-'))
            {
                sign = -1;
                number[0] = number[0].Replace("-", "");
            }
            else
            {
                sign = 1;
            }
            double decimalNum = 0;
            int c = number[0].Length - 1;

            int tmp = 0;
            for (int i = 0; i < number[0].Length; ++i)
            {
                int.TryParse(number[0][i].ToString(), out tmp);
                decimalNum += tmp * Math.Pow(4, c);
                c--;
            }
            if (number[1].Length != 0 && number[1] != "0")
            {
                for (int i = 1; i <= number[1].Length; i++)
                {
                    int.TryParse(number[1][i - 1].ToString(), out tmp);
                    decimalNum += tmp * Math.Pow(4, -i);
                }
            }
            return (sign == -1) ? -decimalNum : decimalNum;
        }


        private string FromDecimalNumber(double num)
        {
            int sign = (num < 0) ? -1 : 1;
            num = Math.Abs(num);
            int zel = (int)Math.Truncate(num);
            double FracVal = num - zel;
            string StrInt = "";
            do
            {
                StrInt = (zel % 4).ToString() + StrInt;
                zel = zel / 4;
            } while (zel != 0);

            if (FracVal != 0)
            {
                string FracPart = "";
                int tmp;
                while (FracVal > 0 && FracPart.Length <= 5)
                {
                    FracVal = FracVal * 4;
                    tmp = (int)Math.Truncate(FracVal);
                    FracPart = FracPart + (tmp).ToString();
                    FracVal = FracVal - tmp;
                }
                StrInt = StrInt + "," + Convert.ToString(FracPart);
            }
            return (sign == -1) ? "-" + StrInt : StrInt;
        }


        private Queue<string> ParseInputString(String input)
        {
            Queue<string> answer = new Queue<string>();
            StringBuilder buffer = new StringBuilder();
            int n = input.Length;
            int i = 0;
            while (i < n)
            {
                char elem = input[i];
                if (number.IsMatch("" + elem))
                {
                    char curr = elem;
                    while ((number.IsMatch("" + curr) || curr == ','))
                    {                 
                        buffer.Append(curr);
                        i++;
                        if (i >= n)
                        {
                            break;
                        }
                        curr = input[i];
                    }

                    if (buffer[buffer.Length - 1] == ',')
                    {
                        buffer.Append('0');
                    }
                    answer.Enqueue(buffer.ToString());
                    buffer = new StringBuilder();
                }
                else
                {
                    answer.Enqueue("" + elem);
                    i++;
                }
            }
            return answer;
        }


        private Queue<string> ConvertationToPolishNotation(Queue<string> parsedString)
        {
            Queue<string> output = new Queue<string>();
            Stack<char> operators = new Stack<char>();
            while (!(parsedString.Count == 0))
            {
                String elem = parsedString.Dequeue();
                if (number.IsMatch(elem))
                {
                    output.Enqueue(elem);
                }
                else if (operation.IsMatch(elem))
                {
                    while (operators.Count > 0 && operation.IsMatch("" + operators.Peek()) && GetPriority(operators.Peek()) >= GetPriority(elem[0]))
                    {
                        output.Enqueue("" + operators.Pop());
                    }
                    operators.Push(elem[0]);
                }
                else if (elem[0] == '(')
                {
                    operators.Push(elem[0]);
                }
                else if (elem[0] == ')')
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        output.Enqueue("" + operators.Pop());
                    }
                    operators.Pop();
                }

            }
            while (operators.Count > 0)
            {
                output.Enqueue("" + operators.Pop());
            }
            return output;
        }

        private string CalculatePolishForm(Queue<string> polishNot)
        {
            Stack<string> container = new Stack<string>();
            while (polishNot.Count != 0)
            {
                string elem = polishNot.Dequeue();

                if (number.IsMatch(elem))
                {
                    container.Push(elem);
                }
                else
                {
                    if ((elem == "+" || elem == "-") && container.Count <= 1)
                    {

                        string lastNumber = container.Pop();
                        if (elem == "-" && lastNumber[0] == '-')
                        {
                            container.Push(lastNumber.Substring(1));
                        }
                        else if (elem == "-" && lastNumber[0] != '-')
                        {
                            container.Push("-" + lastNumber);
                        }
                        else
                        {
                            container.Push(lastNumber);
                        }
                    }
                    else
                    {
                        double second = ToDecimalNumber(container.Pop());
                        double first = ToDecimalNumber(container.Pop());
                        double result;
                        switch (elem)
                        {
                            case "+":
                                result = (second + first);
                                container.Push(result.ToString());
                                break;
                            case "-":
                                result = first - second;
                                container.Push(result.ToString());
                                break;
                            case "*":
                                result = first * second;
                                container.Push(result.ToString());
                                break;
                            case "/":
                                if (second == 0)
                                {
                                    throw new ArithmeticException("Ошибка, деление на 0");
                                }
                                result = first / second;
                                container.Push(result.ToString());
                                break;
                        }
                    }
                }
            }
            Console.WriteLine(container.Peek());
            double lastElem = ToDecimalNumber(container.Pop());
            int ans = (int)Math.Round(lastElem);

            return lastElem == (double)ans ? FromDecimalNumber(ans) : FromDecimalNumber(lastElem);
        }

        private string DeleteExceptions(string input)
        {
            string ans = input;
            foreach (Match exception in exceptions.Matches(input))
            {
                Match number = innerNumber.Match(exception.ToString());
                ans = input.Replace(exception.ToString(), "+" + number);
            }
            return ans;
        }

        public string Compute(string input)
        {
            if (input == null || input.Length == 0)
            {
                return "";
            }
            input = DeleteExceptions(input);
            try
            {
                return CalculatePolishForm(ConvertationToPolishNotation(ParseInputString(input)));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
    
}
