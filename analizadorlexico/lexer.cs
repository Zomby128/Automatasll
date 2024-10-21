using System;
using System.Collections.Generic;

public enum TokenType
{
    Number,
    Identifier,
    Operator,
    Keyword,
    ParenOpen,
    ParenClose,
    BraceOpen,
    BraceClose,
    Semicolon,
    Unknown
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
}

public class Lexer
{
    private string codigoFuente;
    private int posicion;
    private Dictionary<string, TokenType> palabrasClave;

    // Constructor que acepta el código fuente.
    public Lexer(string codigoFuente)
    {
        this.codigoFuente = codigoFuente;
        this.posicion = 0;
        // Inicializar palabras clave como "if", "else", "while", "for", etc.
        palabrasClave = new Dictionary<string, TokenType>
        {
            {"if", TokenType.Keyword},
            {"else", TokenType.Keyword},
            {"while", TokenType.Keyword},
            {"for", TokenType.Keyword},
            {"return", TokenType.Keyword}
        };
    }

    // Método para generar tokens.
    public List<Token> GenerarTokens()
    {
        List<Token> tokens = new List<Token>();

        while (posicion < codigoFuente.Length)
        {
            char caracterActual = codigoFuente[posicion];

            if (char.IsWhiteSpace(caracterActual))
            {
                posicion++;
            }
            else if (char.IsDigit(caracterActual))
            {
                tokens.Add(GenerarTokenNumero());
            }
            else if (char.IsLetter(caracterActual))
            {
                tokens.Add(GenerarTokenIdentificador());
            }
            else
            {
                tokens.Add(GenerarTokenSimbolo());
            }
        }

        return tokens;
    }

    private Token GenerarTokenNumero()
    {
        string numero = "";
        while (posicion < codigoFuente.Length && char.IsDigit(codigoFuente[posicion]))
        {
            numero += codigoFuente[posicion];
            posicion++;
        }

        return new Token(TokenType.Number, numero);
    }

    private Token GenerarTokenIdentificador()
    {
        string identificador = "";
        while (posicion < codigoFuente.Length && char.IsLetterOrDigit(codigoFuente[posicion]))
        {
            identificador += codigoFuente[posicion];
            posicion++;
        }

        if (palabrasClave.ContainsKey(identificador))
        {
            return new Token(palabrasClave[identificador], identificador);
        }

        return new Token(TokenType.Identifier, identificador);
    }

    private Token GenerarTokenSimbolo()
    {
        char simboloActual = codigoFuente[posicion];
        posicion++; // Avanzar para consumir el símbolo

        switch (simboloActual)
        {
            case '+':
            case '-':
            case '*':
            case '/':
            case '=':
            case '<':
            case '>':
                return new Token(TokenType.Operator, simboloActual.ToString());

            case '(':
                return new Token(TokenType.ParenOpen, "(");

            case ')':
                return new Token(TokenType.ParenClose, ")");

            case '{':
                return new Token(TokenType.BraceOpen, "{");

            case '}':
                return new Token(TokenType.BraceClose, "}");

            case ';':
                return new Token(TokenType.Semicolon, ";");

            default:
                throw new Exception($"Símbolo inesperado: {simboloActual}");
        }
    }
}
