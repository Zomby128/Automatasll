using System;
using System.Collections.Generic;

// Enumeración que define los tipos de tokens posibles.
public enum TokenType
{
    Number,       // Representa un número
    Identifier,   // Representa un identificador (como nombres de variables)
    Operator,     // Representa un operador (como +, -, *, etc.)
    Keyword,      // Representa una palabra clave (como if, for, etc.)
    ParenOpen,    // Paréntesis abierto '('
    ParenClose,   // Paréntesis cerrado ')'
    BraceOpen,    // Llave abierta '{'
    BraceClose,   // Llave cerrada '}'
    Semicolon,    // Punto y coma ';'
    Unknown       // Token desconocido
}

// Clase que representa un token.
public class Token
{
    public TokenType Type { get; set; } // Tipo del token (definido en la enumeración TokenType)
    public string Value { get; set; }    // Valor del token (el texto que representa)

    // Constructor que inicializa un token con su tipo y valor.
    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
}

// Clase que se encarga de analizar el código fuente y generar tokens.
public class Lexer
{
    private string codigoFuente; // Código fuente a analizar.
    private int posicion;        // Posición actual en el código fuente.
    private Dictionary<string, TokenType> palabrasClave; // Diccionario de palabras clave.

    // Constructor que acepta el código fuente y lo inicializa.
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

    // Método que genera una lista de tokens a partir del código fuente.
    public List<Token> GenerarTokens()
    {
        List<Token> tokens = new List<Token>(); // Lista para almacenar los tokens generados.

        // Recorrer el código fuente hasta el final.
        while (posicion < codigoFuente.Length)
        {
            char caracterActual = codigoFuente[posicion]; // Obtener el carácter actual.

            // Ignorar espacios en blanco.
            if (char.IsWhiteSpace(caracterActual))
            {
                posicion++;
            }
            // Si el carácter actual es un dígito, generar un token de número.
            else if (char.IsDigit(caracterActual))
            {
                tokens.Add(GenerarTokenNumero());
            }
            // Si el carácter actual es una letra, generar un token de identificador.
            else if (char.IsLetter(caracterActual))
            {
                tokens.Add(GenerarTokenIdentificador());
            }
            // Si el carácter actual no es un espacio, dígito o letra, tratarlo como un símbolo.
            else
            {
                tokens.Add(GenerarTokenSimbolo());
            }
        }

        return tokens; // Devolver la lista de tokens generados.
    }

    // Método para generar un token de número.
    private Token GenerarTokenNumero()
    {
        string numero = ""; // Cadena para almacenar el número.
        // Recorrer el código fuente mientras el carácter actual sea un dígito.
        while (posicion < codigoFuente.Length && char.IsDigit(codigoFuente[posicion]))
        {
            numero += codigoFuente[posicion]; // Agregar el dígito a la cadena.
            posicion++; // Avanzar la posición.
        }

        return new Token(TokenType.Number, numero); // Devolver el token de número generado.
    }

    // Método para generar un token de identificador.
    private Token GenerarTokenIdentificador()
    {
        string identificador = ""; // Cadena para almacenar el identificador.
        // Recorrer el código fuente mientras el carácter actual sea una letra o dígito.
        while (posicion < codigoFuente.Length && char.IsLetterOrDigit(codigoFuente[posicion]))
        {
            identificador += codigoFuente[posicion]; // Agregar el carácter a la cadena.
            posicion++; // Avanzar la posición.
        }

        // Si el identificador es una palabra clave, devolver el token correspondiente.
        if (palabrasClave.ContainsKey(identificador))
        {
            return new Token(palabrasClave[identificador], identificador);
        }

        // Devolver un token de identificador normal.
        return new Token(TokenType.Identifier, identificador);
    }

    // Método para generar un token para símbolos y operadores.
    private Token GenerarTokenSimbolo()
    {
        char simboloActual = codigoFuente[posicion]; // Obtener el símbolo actual.

        // Manejar operadores de incremento (++) y decremento (--) específicamente.
        if (posicion + 1 < codigoFuente.Length)
        {
            string dosCaracteres = codigoFuente.Substring(posicion, 2); // Obtener los siguientes dos caracteres.

            if (dosCaracteres == "++" || dosCaracteres == "--")
            {
                posicion += 2; // Avanzar dos caracteres.
                return new Token(TokenType.Operator, dosCaracteres); // Devolver el token para '++' o '--'.
            }
        }

        posicion++; // Avanzar para consumir el símbolo actual.

        // Generar un token basado en el símbolo actual.
        switch (simboloActual)
        {
            case '+':
            case '-':
            case '*':
            case '/':
            case '=':
            case '<':
            case '>':
                return new Token(TokenType.Operator, simboloActual.ToString()); // Devolver el token del operador.
            case '(':
                return new Token(TokenType.ParenOpen, "("); // Devolver el token de paréntesis abierto.
            case ')':
                return new Token(TokenType.ParenClose, ")"); // Devolver el token de paréntesis cerrado.
            case '{':
                return new Token(TokenType.BraceOpen, "{"); // Devolver el token de llave abierta.
            case '}':
                return new Token(TokenType.BraceClose, "}"); // Devolver el token de llave cerrada.
            case ';':
                return new Token(TokenType.Semicolon, ";"); // Devolver el token de punto y coma.
            default:
                // Lanzar una excepción si se encuentra un símbolo inesperado.
                throw new Exception($"Símbolo inesperado: {simboloActual}");
        }
    }
}
