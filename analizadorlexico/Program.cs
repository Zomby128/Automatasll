//  Nombre: Adolfo Alejandro Granados Cosio
//  Matricula: 22760037
//  Trabajo: Analizador Lexico en C#
//  Profesor: Luis Armando Cárdenas Florido
using System; // Importa el espacio de nombres System, que incluye funcionalidades básicas como la entrada/salida.
using System.Collections.Generic; // Permite usar colecciones genéricas, como List y Stack, para manejar listas y pilas.
using System.IO; // Importa el espacio de nombres para operaciones de entrada/salida de archivos.
using System.Linq;

// Define los tipos de tokens que el analizador léxico reconocerá.
public enum TokenType
{
    Keyword,
    Identifier,
    Operator,
    Number,
    Delimiter,
    Comment,
    Error,
    DataType,
    Variable,
    Power
}

public class Token // Representa los tokens que de nuestro código fuente
{
    public TokenType Type { get; }
    public string Value { get; }
    public bool IsKeyword { get; }

    public Token(TokenType type, string value, bool isKeyword = false)
    {
        Type = type;
        Value = value;
        IsKeyword = isKeyword;
    }

    public string GetDescription()
    {
        return Type switch
        {
            TokenType.Keyword => "Palabra clave",
            TokenType.Identifier => "Identificador",
            TokenType.Operator => "Operador",
            TokenType.Number => "Número",
            TokenType.Delimiter => "Delimitador",
            TokenType.Comment => "Comentario",
            TokenType.Error => "Error",
            TokenType.DataType => "Tipo de Dato",
            TokenType.Variable => "Variable",
            TokenType.Power => "Potencia",
            _ => "Desconocido"
        };
    }

    public override string ToString()
    {
        string keywordFlag = IsKeyword ? "Sí" : "No";
        return $"{Value,-15} {GetDescription(),-20} {Type,-15} {keywordFlag,-10}";
    }
}

public class Lexer // Clase que realiza el análisis léxico
{
    private static readonly string[] keywords = { "if", "else", "while", "return", "for", "int", "float", "string", };
    private static readonly string[] operators = { "+", "-", "*", "/", "=", "!=", "<", ">", "<=", ">=", "&&", "||", "++", "--" };
    private static readonly string[] dataTypes = { "int", "float", "string", "bool" };
    private static readonly char[] delimiters = { '(', ')', '{', '}', ';', '[', ']', ',', '.' };

    public List<Token> Analyze(string code)
    {
        List<Token> tokens = new List<Token>();
        int i = 0;
        Stack<char> delimiterStack = new Stack<char>();

        while (i < code.Length)
        {
            if (char.IsWhiteSpace(code[i]))
            {
                i++;
                continue;
            }

            if (code[i] == '/')
            {
                if (i + 1 < code.Length && code[i + 1] == '/')
                {
                    int start = i;
                    while (i < code.Length && code[i] != '\n') i++;
                    tokens.Add(new Token(TokenType.Comment, code.Substring(start, i - start)));
                    continue;
                }
                else if (i + 1 < code.Length && code[i + 1] == '*')
                {
                    int start = i;
                    i += 2;
                    while (i + 1 < code.Length && !(code[i] == '*' && code[i + 1] == '/')) i++;

                    if (i + 1 >= code.Length)
                    {
                        tokens.Add(new Token(TokenType.Error, "Error: Comentario de bloque sin cerrar"));
                        break;
                    }

                    i += 2;
                    tokens.Add(new Token(TokenType.Comment, code.Substring(start, i - start)));
                    continue;
                }
            }

            if (char.IsDigit(code[i]))
            {
                int start = i;
                bool isDecimal = false;
                while (i < code.Length && (char.IsDigit(code[i]) || (!isDecimal && code[i] == '.')))
                {
                    if (code[i] == '.') isDecimal = true;
                    i++;
                }

                string numStr = code.Substring(start, i - start);
                tokens.Add(new Token(TokenType.Number, numStr));
                continue;
            }

            if (char.IsLetter(code[i]))
            {
                int start = i;
                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_')) i++;
                string word = code.Substring(start, i - start);
                if (Array.Exists(keywords, k => k == word))
                {
                    tokens.Add(new Token(TokenType.Keyword, word, true));
                }
                else if (Array.Exists(dataTypes, dt => dt == word))
                {
                    tokens.Add(new Token(TokenType.DataType, word));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Identifier, word));
                }
                continue;
            }

            switch (code[i])
            {
                case '=':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, "=="));
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, "="));
                        i++;
                    }
                    break;
                case '!':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, "!="));
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Error, $"Error: Operador no válido '{code[i]}'"));
                        i++;
                    }
                    break;
                case '<':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, "<="));
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, "<"));
                        i++;
                    }
                    break;
                case '>':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, ">="));
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, ">"));
                        i++;
                    }
                    break;
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                    tokens.Add(new Token(TokenType.Operator, code[i].ToString()));
                    i++;
                    break;
                case '^':
                    tokens.Add(new Token(TokenType.Power, "^"));
                    i++;
                    break;
                case '{':
                case '}':
                case '(':
                case ')':
                case '[':
                case ']':
                case ';':
                case ',':
                    tokens.Add(new Token(TokenType.Delimiter, code[i].ToString()));
                    i++;
                    break;
                default:
                    tokens.Add(new Token(TokenType.Error, $"Error: Carácter no reconocido '{code[i]}'"));
                    i++;
                    break;
            }
        }
        return tokens;
    }
}

class Program
{
public static void ImprimirArbol(NodoExpresion nodo, int nivel = 0)
{
    if (nodo != null)
    {
        // Imprime el hijo derecho (parte superior del árbol).
        ImprimirArbol(nodo.Derecha, nivel + 1);
        // Imprime el nodo actual con la indentación apropiada.
        Console.WriteLine(new string(' ', nivel * 4) + nodo.Valor);
        // Imprime el hijo izquierdo (parte inferior del árbol).
        ImprimirArbol(nodo.Izquierda, nivel + 1);
    }
}


    public static void Main(string[] args)
    {
        try
        {
            string code = File.ReadAllText("codigo_fuente.txt");
            Lexer lexer = new Lexer();
            List<Token> tokens = lexer.Analyze(code);

            Console.WriteLine("¿Qué deseas ver?");
            Console.WriteLine("1. Tabla de tokens");
            Console.WriteLine("2. Árbol sintáctico");
            Console.Write("Selecciona una opción (1 o 2): ");
            string opcion = Console.ReadLine();

            if (opcion == "1")
            {
                Console.WriteLine($"{"Valor",-15} {"Descripción",-20} {"Tipo",-15} {"Palabra clave",-10}");
                Console.WriteLine(new string('-', 70));
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token);
                }
            }
            else if (opcion == "2")
            {
                string[] tokenValues = tokens.Select(token => token.Value).ToArray();
                AnalizadorSintactico parser = new AnalizadorSintactico(tokenValues);
                NodoExpresion arbol = parser.Parsear();

                Console.WriteLine("Árbol Sintáctico:");
                ImprimirArbol(arbol);
            }
            else
            {
                Console.WriteLine("Opción no válida. Por favor, selecciona 1 o 2.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}