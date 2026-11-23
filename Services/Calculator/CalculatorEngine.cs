
using System.Globalization;

namespace LifeDeck.Services.Calculator;

public sealed class CalculatorEngine
{
    private string _expression = string.Empty;
    private int _position;

    public decimal Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new FormatException("Enter an expression.");

        if (expression.Length > 120)
            throw new FormatException("Expression is too long.");

        _expression = expression
            .Replace('×', '*')
            .Replace('÷', '/')
            .Replace('−', '-')
            .Replace(',', '.');

        _position = 0;

        decimal result = ParseExpression();

        SkipSpaces();

        if (_position != _expression.Length)
            throw new FormatException("Invalid expression.");

        return result;
    }

    // Addition and subtraction
    private decimal ParseExpression()
    {
        decimal result = ParseTerm();

        while (true)
        {
            SkipSpaces();

            if (Consume('+'))
                result += ParseTerm();
            else if (Consume('-'))
                result -= ParseTerm();
            else
                return result;
        }
    }

    // Multiplication and division
    private decimal ParseTerm()
    {
        decimal result = ParseFactor();

        while (true)
        {
            SkipSpaces();

            if (Consume('*'))
            {
                result *= ParseFactor();
            }
            else if (Consume('/'))
            {
                decimal divisor = ParseFactor();

                if (divisor == 0)
                    throw new DivideByZeroException();

                result /= divisor;
            }
            else
            {
                return result;
            }
        }
    }

    // Negative numbers and parentheses
    private decimal ParseFactor()
    {
        SkipSpaces();

        if (Consume('+'))
            return ParseFactor();

        if (Consume('-'))
            return -ParseFactor();

        if (Consume('('))
        {
            decimal result = ParseExpression();

            SkipSpaces();

            if (!Consume(')'))
                throw new FormatException("Missing closing parenthesis.");

            return result;
        }

        return ParseNumber();
    }

    // Read decimal number
    private decimal ParseNumber()
    {
        SkipSpaces();

        int start = _position;
        bool hasDigit = false;

        while (_position < _expression.Length &&
               char.IsAsciiDigit(_expression[_position]))
        {
            _position++;
            hasDigit = true;
        }

        if (Consume('.'))
        {
            while (_position < _expression.Length &&
                   char.IsAsciiDigit(_expression[_position]))
            {
                _position++;
                hasDigit = true;
            }
        }

        if (!hasDigit)
            throw new FormatException("Invalid number.");

        string text = _expression[start.._position];

        if (!decimal.TryParse(
                text,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal number))
        {
            throw new FormatException("Invalid number.");
        }

        return number;
    }

    private bool Consume(char character)
    {
        if (_position < _expression.Length &&
            _expression[_position] == character)
        {
            _position++;
            return true;
        }

        return false;
    }

    private void SkipSpaces()
    {
        while (_position < _expression.Length &&
               char.IsWhiteSpace(_expression[_position]))
        {
            _position++;
        }
    }
}
