//  Nombre: Adolfo Alejandro Granados Cosio
//  Matricula: 22760037
//  Trabajo: Analizador Lexico en C#
//  Profesor: Luis Armando Cárdenas Florido
// -----------------------------------------------------OBSERVACIONES A MEJORAR E IDENTIFICAR-------------------------------------------------------
// Comentarios para mejorar: Cambiar en el tipo y poner si es variable, si es float, entero, etc.
// Estudiarlo para presentar en la siguiente clase como si fuera un proyecto a una empresa.
// Estudaiar mas las librerias y la logica de la programacion para estar mas seguro al momento de hablar.
// La siguiente etapa sera una competencia entre todos, ademas de incorporar el analizador lexico para elaborar un arbol sintactico
// Arreglar el codigo para que al momento de escribir algo lo identifique como una nueva variable
// Que identifique si es un error de escritura al momento de hacer una condicion o una funcion.

using System; // Importa el espacio de nombres System, que incluye funcionalidades básicas como la entrada/salida.
using System.Collections.Generic; // Permite usar colecciones genéricas, como List y Stack, para manejar listas y pilas.
using System.IO; // Importa el espacio de nombres para operaciones de entrada/salida de archivos.
/*
//Define los tipos de tokens que el analizador léxico reconocerá.
Un token es una unidad básica de código que tiene significado, y este enum clasifica los tokens en varios tipos
*/
public enum TokenType
{
    Keyword,     // Tipo de token para palabras clave.
    Identifier,  // Tipo de token para identificadores.
    Operator,    // Tipo de token para operadores.
    Number,      // Tipo de token para números.
    Delimiter,   // Tipo de token para delimitadores.
    Comment,     // Tipo de token para comentarios.
    Error        // Tipo de token para errores.
}

public class Token //Representa los tokens que de nuestro codigo fuente
{
    public TokenType Type { get; } // Propiedad para el tipo de token.
    public string Value { get; } // Propiedad para el valor del token.
    public bool IsKeyword { get; } // Propiedad para indicar si el token es una palabra reservada.

    // Constructor para inicializar el token con tipo, valor y si es palabra clave.
    public Token(TokenType type, string value, bool isKeyword = false)
    {
        Type = type;
        Value = value;
        IsKeyword = isKeyword;
    }

    // Método para obtener una descripción en español según el tipo de token.
    public string GetDescription()
    {
        return Type switch
        {
            TokenType.Keyword => "Palabra clave", // Descripción para palabras clave.
            TokenType.Identifier => "Identificador", // Descripción para identificadores.
            TokenType.Operator => "Operador", // Descripción para operadores.
            TokenType.Number => "Número", // Descripción para números.
            TokenType.Delimiter => "Delimitador", // Descripción para delimitadores.
            TokenType.Comment => "Comentario", // Descripción para comentarios.
            TokenType.Error => "Error", // Descripción para errores.
            _ => "Desconocido" // Descripción para tipos de token desconocidos.
        };
    }

    // Método para representar el token en formato de tabla con todos los detalles.
    public override string ToString()
    {
        string keywordFlag = IsKeyword ? "Sí" : "No"; // Determina si el token es una palabra reservada.
        return $"{Value,-15} {GetDescription(),-20} {Type,-15} {keywordFlag,-10}"; // Formato para la salida en tabla.
    }
}

public class Lexer //
{
    // Palabras clave reservadas para el análisis léxico.
    private static readonly string[] keywords = { "if", "else", "while", "return", "for" };

    // Operadores aceptados para el análisis léxico.
    private static readonly string[] operators = { "+", "-", "*", "/", "=", "!=", "<", ">", "<", ">", "&&", "||", "++", "--" };

    // Delimitadores aceptados para el análisis léxico.
    private static readonly char[] delimiters = { '(', ')', '{', '}', ';', '[', ']', ',', '.' };

    // Método para analizar el código fuente y devolver una lista de tokens.
    public List<Token> Analyze(string code)
    {
        List<Token> tokens = new List<Token>(); // Lista para almacenar los tokens encontrados.
        int i = 0; // Índice para recorrer el código fuente.
        Stack<char> delimiterStack = new Stack<char>(); // Pila para gestionar los delimitadores anidados.

        // Recorre el código fuente carácter por carácter.
        while (i < code.Length)
        {
            // Ignora espacios en blanco.
            if (char.IsWhiteSpace(code[i]))
            {
                i++;
                continue;
            }

            // Manejo de comentarios (una línea y varias líneas).
            if (code[i] == '/')
            {
                // Comentario de una línea.
                if (i + 1 < code.Length && code[i + 1] == '/')
                {
                    int start = i; // Marca el inicio del comentario.
                    while (i < code.Length && code[i] != '\n') i++; // Lee hasta el final de la línea.
                    tokens.Add(new Token(TokenType.Comment, code.Substring(start, i - start))); // Añade el comentario a la lista de tokens.
                    continue;
                }
                // Comentario de varias líneas.
                else if (i + 1 < code.Length && code[i + 1] == '*')
                {
                    int start = i; // Marca el inicio del comentario.
                    i += 2; // Salta los caracteres '/*'.
                    while (i + 1 < code.Length && !(code[i] == '*' && code[i + 1] == '/')) i++; // Lee hasta el final del comentario.
                    
                    // Si no se encuentra el final del comentario, agrega un error.
                    if (i + 1 >= code.Length)
                    {
                        tokens.Add(new Token(TokenType.Error, "Error: Comentario de bloque sin cerrar"));
                        break;
                    }

                    i += 2; // Salta los caracteres '*/'.
                    tokens.Add(new Token(TokenType.Comment, code.Substring(start, i - start))); // Añade el comentario a la lista de tokens.
                    continue;
                }
            }

            // Manejo de números (enteros y decimales).
            if (char.IsDigit(code[i]))
            {
                int start = i; // Marca el inicio del número.
                bool isDecimal = false; // Bandera para indicar si el número es decimal.

                while (i < code.Length && (char.IsDigit(code[i]) || (!isDecimal && code[i] == '.')))
                {
                    if (code[i] == '.')
                        isDecimal = true; // Marca el número como decimal.
                    i++;
                }

                string numStr = code.Substring(start, i - start); // Extrae el número como una cadena.

                try
                {
                    tokens.Add(new Token(TokenType.Number, numStr)); // Añade el número a la lista de tokens.
                }
                catch (OverflowException)
                {
                    tokens.Add(new Token(TokenType.Error, $"Error: Número fuera de rango '{numStr}'")); // Maneja errores de rango.
                }
                continue;
            }

            // Identificadores o palabras clave.
            if (char.IsLetter(code[i]))
            {
                int start = i; // Marca el inicio del identificador o palabra clave.
                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_')) i++; // Lee hasta el final del identificador.
                string word = code.Substring(start, i - start); // Extrae la cadena del identificador o palabra clave.
                if (Array.Exists(keywords, k => k == word))
                {
                    tokens.Add(new Token(TokenType.Keyword, word, true)); // Añade una palabra clave a la lista de tokens.
                }
                else
                {
                    tokens.Add(new Token(TokenType.Identifier, word)); // Añade un identificador a la lista de tokens.
                }
                continue;
            }

            // Manejo de operadores.
            bool operatorFound = false; // Bandera para indicar si se encontró un operador.
            foreach (var op in operators)
            {
                if (code.Substring(i).StartsWith(op))
                {
                    tokens.Add(new Token(TokenType.Operator, op)); // Añade el operador a la lista de tokens.
                    i += op.Length; // Avanza el índice según la longitud del operador.
                    operatorFound = true;
                    break;
                }
            }

            // Manejo de operadores no válidos.
            if (!operatorFound && (code[i] == '=' || code[i] == '!' || code[i] == '<' || code[i] == '>'))
            {
                tokens.Add(new Token(TokenType.Error, $"Error: Operador no válido '{code[i]}'")); // Añade un error para operadores no válidos.
                i++;
                continue;
            }

            // Manejo de delimitadores.
            if (Array.Exists(delimiters, d => d == code[i]))
            {
                char currentDelimiter = code[i]; // Obtiene el delimitador actual.

                if (currentDelimiter == '(' || currentDelimiter == '{' || currentDelimiter == '[')
                {
                    delimiterStack.Push(currentDelimiter); // Añade el delimitador a la pila.
                }
                else if (currentDelimiter == ')' || currentDelimiter == '}' || currentDelimiter == ']')
                {
                    // Verifica si el delimitador tiene una pareja.
                    if (delimiterStack.Count == 0 || !IsMatchingPair(delimiterStack.Peek(), currentDelimiter))
                    {
                        tokens.Add(new Token(TokenType.Error, $"Error: Delimitador sin pareja '{currentDelimiter}'")); // Añade un error para delimitadores sin pareja.
                    }
                    else
                    {
                        delimiterStack.Pop(); // Elimina el delimitador de la pila.
                    }
                }

                tokens.Add(new Token(TokenType.Delimiter, currentDelimiter.ToString())); // Añade el delimitador a la lista de tokens.
                i++;
                continue;
            }

            // Manejo de caracteres no reconocidos.
            if (!char.IsLetterOrDigit(code[i]) && code[i] != '_' && !char.IsWhiteSpace(code[i]))
            {
                tokens.Add(new Token(TokenType.Error, $"Error: Carácter no reconocido '{code[i]}'")); // Añade un error para caracteres no reconocidos.
                i++;
            }
        }

        // Verifica si hay delimitadores sin cerrar al final del análisis.
        while (delimiterStack.Count > 0)
        {
            tokens.Add(new Token(TokenType.Error, $"Error: Delimitador de apertura sin cerrar '{delimiterStack.Pop()}'")); // Añade un error para delimitadores de apertura sin cerrar.
        }

        return tokens; // Devuelve la lista de tokens analizados.
    }

    // Método para verificar si un delimitador de apertura y cierre son una pareja.
    private bool IsMatchingPair(char open, char close)
    {
        return (open == '(' && close == ')') ||
               (open == '{' && close == '}') ||
               (open == '[' && close == ']');
    }
}

class Program
{
    static void Main(string[] args)
    {
        string filePath = "codigo_fuente.txt"; // Ruta del archivo de código fuente a analizar.
        if (!File.Exists(filePath))
        {
            Console.WriteLine("El archivo no existe."); // Mensaje si el archivo no existe.
            return;
        }

        string code = File.ReadAllText(filePath); // Lee el contenido del archivo de código fuente.
        Lexer lexer = new Lexer(); // Crea una instancia del analizador léxico.
        List<Token> tokens = lexer.Analyze(code); // Analiza el código fuente y obtiene los tokens.

        // Imprime la tabla con los encabezados.
        Console.WriteLine($"{"Token",-15} {"Descripción",-20} {"Tipo",-15} {"Palabra Res.",-10}");
        Console.WriteLine(new string('-', 60));

        // Imprime cada token con el formato solicitado.
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
