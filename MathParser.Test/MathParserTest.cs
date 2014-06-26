using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Antropod.MathParser;

namespace expression_parset_test
{
    [TestClass]
    public class MathParserTest
    {
        public bool ParseFails(string expression)
        {
            try 
            {
                MathParser.ParseExpression(expression);
                return false;
            }
            catch(ParserException)
            {
                return true;
            }
        }

        [TestMethod]
        public void TestParseMutipleTimes()
        {
            var parser = new MathParser("1+2");
            Assert.AreEqual(3, parser.Parse());
            Assert.AreEqual(3, parser.Parse());
        }

        [TestMethod]
        public void TestWitespace()
        {
            Assert.AreEqual(3, MathParser.ParseExpression(" ( 1 + 2) + log(1)"));
        }


        [TestMethod]
        public void TestFloatingPoint()
        {
            Assert.AreEqual(1.0, MathParser.ParseExpression("1.0"));
            Assert.AreEqual(1.1, MathParser.ParseExpression("1.1"));
            Assert.AreEqual(123.0, MathParser.ParseExpression("123."));
            Assert.AreEqual(123.456, MathParser.ParseExpression("123.456"));
        }

        [TestMethod]
        public void TestFunctions()
        {
            Assert.AreEqual(42, MathParser.ParseExpression("id(42)"));
            Assert.AreEqual(3, MathParser.ParseExpression("id(2+1)"));
            Assert.AreEqual(49, MathParser.ParseExpression("id((1+(2*3))*(3+4))"));
            Assert.AreEqual(1, MathParser.ParseExpression("id((((1))))"));
            Assert.AreEqual(2, MathParser.ParseExpression("id(id(1+id(1)))"));
        }

        [TestMethod]
        public void TestBuiltinFunctions()
        {
            Assert.AreEqual(Math.Log(42), MathParser.ParseExpression("log(42)"));
            Assert.AreEqual(Math.Log10(42), MathParser.ParseExpression("log10(42)"));
            Assert.AreEqual(Math.Abs(-322), MathParser.ParseExpression("abs(-322)"));
            Assert.AreEqual(Math.Exp(-0.5), MathParser.ParseExpression("exp(-0.5)"));
        }

        [TestMethod]
        public void TestVariables()
        {
            var parser = new MathParser("foo123");
            parser.variables["foo123"] = 456;
            Assert.AreEqual(456, parser.Parse());
        }

        [TestMethod]
        public void TestParserExpression()
        {
            Assert.AreEqual(1, MathParser.ParseExpression("1"));
            Assert.AreEqual(1, MathParser.ParseExpression("+1"));
            Assert.AreEqual(-1, MathParser.ParseExpression("-1"));
            Assert.AreEqual(-1, MathParser.ParseExpression("0-1"));
            Assert.AreEqual(-2, MathParser.ParseExpression("0-1+2-3"));
            Assert.AreEqual(6, MathParser.ParseExpression("2*3"));
            Assert.AreEqual(2, MathParser.ParseExpression("4/2"));
            Assert.AreEqual(21, MathParser.ParseExpression("(1+2)*(3+4)"));
            Assert.AreEqual(49, MathParser.ParseExpression("(1+(2*3))*(3+4)"));
            Assert.AreEqual(123, MathParser.ParseExpression("123"));

            Assert.IsTrue(ParseFails("-"));
            Assert.IsTrue(ParseFails("-1-"));
            Assert.IsTrue(ParseFails("1+1+"));
        }

        [TestMethod]
        public void TestCharToIntMethod()
        {
            var digits = "0123456789";
            for (int x = 0; x < digits.Length; ++x )
            {
                Assert.AreEqual(x, MathParser.CharToInt(digits[x]));
            }    
        }
    }
}
