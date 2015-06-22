using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Antropod.MathParser
{
    /* 
     * !! Может не соответствовать коду !!
     * <number> = <digit><digit>*[.<digit>*]
     * <variable> = <letter> (<letter> | <digit>)*
     * <mulop> = "*" | "/"
     * <addop> = "+" | "-"
     * <expression> = [<addop>] <term> (<addop> <term>)*
     * <factor> = "(" <expression> ")" | <variable> | <number>
     * <term> = <factor> (<mulop> <factor>)*
     */

    public class ParserException: Exception
    {
        public ParserException(string message)
            : base(message)
        { }
    }

    public class MathParser
    {
        private string expression;
        private int position;
        private char look;
        public readonly Dictionary<string, double> variables 
            = new Dictionary<string, double>();
        public delegate double Function(double x);
        public readonly Dictionary<string, Function> functions 
            = new Dictionary<string, Function>();

        public void InitBuiltinFunctions()
        {
            functions["id"] = x => x;
            functions["log"] = Math.Log;
            functions["log10"] = Math.Log10;
            functions["abs"] = Math.Abs;
            functions["exp"] = Math.Exp;
        }

        public static int CharToInt(char c)
        {
            return Convert.ToInt32(c) - Convert.ToInt32('0');
        }

        private bool IsDigit(char c)
        { 
            return char.IsDigit(c); 
        }

        private bool IsAlpha(char c)
        {
            return char.IsLetter(c);
        }

        private static bool IsAddop(char c)
        {
            return "+-".Contains(c);
        }

        private bool IsMulop(char c)
        {
            return "*/".Contains(c);
        }

        private bool IsWhiteSpace(char c)
        {
            return Char.IsWhiteSpace(c);
        }

        private void Expected(string what)
        {
            throw new ParserException(what + " expected");
        }

        private void SkipWhiteSpace()
        {
            while (IsWhiteSpace(look))
            {
                GetChar();
            }
        }

        private double GetNum()
        {
            string result = "";
            if (!IsDigit(look)) Expected("Number");
            while (IsDigit(look)) 
            {
                result += look;
                GetChar();
            }
            if (look == '.')
            {
                result += look;
                GetChar();

                while (IsDigit(look))
                {
                    result += look;
                    GetChar();
                }
            }
            SkipWhiteSpace();
            return double.Parse(result, CultureInfo.InvariantCulture.NumberFormat);
        }

        private string GetName()
        {
            string result = "";
            if (!IsAlpha(look)) Expected("Name");
            while (IsDigit(look) || IsAlpha(look))
            {
                result += look;
                GetChar();
            }
            SkipWhiteSpace();
            return result;
        }

        private char Read()
        {
            if (position < expression.Length)
                return expression[position++];
            return '\0';
        }

        private void GetChar()
        {
            look = Read();
        }

        private void Match(char c)
        {
            if (look == c)
            {
                GetChar();
                SkipWhiteSpace();
            }
            else
                Expected(string.Format("'{0}'", c));
        }

        public double Expression()
        {
            double result = 0;
            if (IsAddop(look))
                result = 0;
            else
                result = Term();

            while (IsAddop(look))
            {
                switch (look)
                {
                    case '+': 
                        Match('+');
                        result += Term();
                        break;
                    case '-':
                        Match('-');
                        result -= Term();
                        break;
                    default:
                        throw new Exception();
                }

            }
            return result;
        }

        public double Term()
        {
            double result = Factor();
            while (IsMulop(look))
            {
                switch (look)
                {
                    case '*':
                        Match('*');
                        result *= Factor();
                        break;
                    case '/':
                        Match('/');
                        result /= Factor();
                        break;
                    default:
                        throw new Exception();
                }
            }
            return result;
        }

        public double Factor()
        {
            double result;
            if (look == '(')
            {
                Match('(');
                result = Expression();
                Match(')');
                return result;
            }
            else if (IsAlpha(look))
            {
                var name = GetName();
                if (look == '(')
                {
                    Match('(');
                    result = functions[name]( Expression() );
                    Match(')');
                }
                else
                    result = variables[name];
            }
            else
                result = GetNum();

            return result;
        }

        public MathParser(string expression)
        {
            InitBuiltinFunctions();
            this.expression = expression;
            Reset();
        }

        public void Reset()
        {
            position = 0;
            GetChar();
            SkipWhiteSpace();
        }

        public double Parse()
        {
            Reset();
            return Expression();
        }

        public static double ParseExpression(string expression)
        {
            return new MathParser(expression).Parse();
        }
    }
}
