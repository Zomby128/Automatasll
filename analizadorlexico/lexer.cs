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
    public int Linea { get; set; }      // Línea donde se encuentra el token
    public int Columna { get; set; }    // Columna donde empieza el token

    // Constructor que inicializa un token con su tipo, valor, línea y columna.
    public Token(TokenType type, string value, int linea, int columna)
    {
        Type = type;
        Value = value;
        Linea = linea;
        Columna = columna;
    }
}

// Clase que se encarga de analizar el código fuente y generar tokens.
public class Lexer
{
    private string codigoFuente; // Código fuente a analizar.
    private int posicion;        // Posición actual en el código fuente.
    private int linea;           // Línea actual del código fuente.
    private int columna;         // Columna actual del código fuente.
    private Dictionary<string, TokenType> palabrasClave; // Diccionario de palabras clave.

    // Constructor que acepta el código fuente y lo inicializa.
    public Lexer(string codigoFuente)
    {
        this.codigoFuente = codigoFuente;
        this.posicion = 0;
        this.linea = 1;
        this.columna = 1;
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

            // Ignorar espacios en blanco y manejar nuevas líneas.
            if (char.IsWhiteSpace(caracterActual))
            {
                if (caracterActual == '\n')
                {
                    linea++;
                    columna = 1;
                }
                else
                {
                    columna++;
                }
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
        int inicioColumna = columna;

        // Recorrer el código fuente mientras el carácter actual sea un dígito.
        while (posicion < codigoFuente.Length && char.IsDigit(codigoFuente[posicion]))
        {
            numero += codigoFuente[posicion]; // Agregar el dígito a la cadena.
            posicion++;
            columna++;
        }

        return new Token(TokenType.Number, numero, linea, inicioColumna); // Devolver el token de número generado.
    }

    // Método para generar un token de identificador.
    private Token GenerarTokenIdentificador()
    {
        string identificador = ""; // Cadena para almacenar el identificador.
        int inicioColumna = columna;

        // Recorrer el código fuente mientras el carácter actual sea una letra o dígito.
        while (posicion < codigoFuente.Length && char.IsLetterOrDigit(codigoFuente[posicion]))
        {
            identificador += codigoFuente[posicion]; // Agregar el carácter a la cadena.
            posicion++;
            columna++;
        }

        // Si el identificador es una palabra clave, devolver el token correspondiente.
        if (palabrasClave.ContainsKey(identificador))
        {
            return new Token(palabrasClave[identificador], identificador, linea, inicioColumna);
        }

        // Devolver un token de identificador normal.
        return new Token(TokenType.Identifier, identificador, linea, inicioColumna);
    }

    // Método para generar un token para símbolos y operadores.
    private Token GenerarTokenSimbolo()
    {
        char simboloActual = codigoFuente[posicion]; // Obtener el símbolo actual.
        int inicioColumna = columna;

        // Manejar operadores de incremento (++) y decremento (--) específicamente.
        if (posicion + 1 < codigoFuente.Length)
        {
            string dosCaracteres = codigoFuente.Substring(posicion, 2); // Obtener los siguientes dos caracteres.

            if (dosCaracteres == "++" || dosCaracteres == "--")
            {
                posicion += 2; // Avanzar dos caracteres.
                columna += 2;
                return new Token(TokenType.Operator, dosCaracteres, linea, inicioColumna); // Devolver el token para '++' o '--'.
            }
        }

        posicion++; // Avanzar para consumir el símbolo actual.
        columna++;

        // Generar un token basado en el símbolo actual.
        return simboloActual switch
        {
            '+' or '-' or '*' or '/' or '=' or '<' or '>' => new Token(TokenType.Operator, simboloActual.ToString(), linea, inicioColumna),
            '(' => new Token(TokenType.ParenOpen, "(", linea, inicioColumna),
            ')' => new Token(TokenType.ParenClose, ")", linea, inicioColumna),
            '{' => new Token(TokenType.BraceOpen, "{", linea, inicioColumna),
            '}' => new Token(TokenType.BraceClose, "}", linea, inicioColumna),
            ';' => new Token(TokenType.Semicolon, ";", linea, inicioColumna),
            _ => throw new Exception($"Símbolo inesperado: {simboloActual} en línea {linea}, columna {inicioColumna}")
        };
    }

    // Método para imprimir la lista de tokens en formato tabular.
    public void ImprimirTokens(List<Token> tokens)
    {
        Console.WriteLine("Tipo\t\tValor\t\tLínea\tColumna");
        Console.WriteLine(new string('-', 40));

        foreach (var token in tokens)
        {
            Console.WriteLine($"{token.Type}\t\t{token.Value}\t\t{token.Linea}\t{token.Columna}");
        }
    }
}
