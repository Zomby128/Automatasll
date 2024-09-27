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
Define los tipos de tokens que el analizador léxico reconocerá.
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
    Error,       // Tipo de token para errores.
    DataType,    // Tipos de datos como int, float, string, bool
    Variable,    // Variables definidas por el usuario
    Power        // Token para la potencia
}

public class Token // Representa los tokens que de nuestro código fuente
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
            TokenType.DataType => "Tipo de Dato", // Descripción del dato.
            TokenType.Variable => "Variable", // Descripción para variables por el usuario.
            TokenType.Power => "Potencia", // Descripción para potencia
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

public class Lexer // Clase que realiza el análisis léxico
{
    // Palabras clave reservadas para el análisis léxico.
    private static readonly string[] keywords = { "if", "else", "while", "return", "for", "int", "float", "string", };

    // Operadores aceptados para el análisis léxico.
    private static readonly string[] operators = { "+", "-", "*", "/", "=", "!=", "<", ">", "<=", ">=", "&&", "||", "++", "--" };

    private static readonly string[] dataTypes = { "int", "float", "string", "bool" };

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
                // Avanza mientras sea dígito o el punto decimal.
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
                else if (Array.Exists(dataTypes, dt => dt == word))
                {
                    tokens.Add(new Token(TokenType.DataType, word)); // Token de tipo de dato
                }
                else
                {
                    tokens.Add(new Token(TokenType.Identifier, word)); // Añade un identificador a la lista de tokens.
                }
                continue;
            }

            // Manejo de operadores.
            switch (code[i])
            {
                case '=':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, "==")); // Token de operador '=='
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, "=")); // Token de operador '='
                        i++;
                    }
                    break;
                case '!':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, "!=")); // Token de operador '!='
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Error, $"Error: Operador no válido '{code[i]}'")); // Error si el operador no es válido
                        i++;
                    }
                    break;
                case '<':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, "<=")); // Token de operador '<='
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, "<")); // Token de operador '<'
                        i++;
                    }
                    break;
                case '>':
                    if (i + 1 < code.Length && code[i + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.Operator, ">=")); // Token de operador '>='
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, ">")); // Token de operador '>'
                        i++;
                    }
                    break;
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                    tokens.Add(new Token(TokenType.Operator, code[i].ToString())); // Añade el operador a la lista de tokens.
                    i++;
                    break;

                // Manejo de la potencia
                case '^':
                    tokens.Add(new Token(TokenType.Power, "^")); // Añade el operador de potencia a la lista de tokens.
                    i++;
                    break;

                // Manejo de delimitadores.
                case '{':
                case '}':
                case '(':
                case ')':
                case '[':
                case ']':
                case ';':
                case ',':
                    tokens.Add(new Token(TokenType.Delimiter, code[i].ToString())); // Añade el delimitador a la lista de tokens.
                    i++;
                    break;

                // Si el carácter no se reconoce, se añade un error.
                default:
                    tokens.Add(new Token(TokenType.Error, $"Error: Carácter no reconocido '{code[i]}'")); // Error si el carácter no es reconocido.
                    i++;
                    break;
            }
        }
        return tokens; // Devuelve la lista de tokens generados.
    }
}

public class Program // Clase principal que ejecuta el analizador léxico
{
    public static void Main(string[] args) // Método principal
    {
        // Intenta leer el archivo 'codigo_fuente.txt' y ejecutar el análisis léxico.
        try
        {
            string code = File.ReadAllText("codigo_fuente.txt"); // Lee el contenido del archivo.
            Lexer lexer = new Lexer(); // Crea una instancia del analizador léxico.
            List<Token> tokens = lexer.Analyze(code); // Analiza el código fuente y obtiene los tokens.

            // Muestra los resultados en una tabla.
            Console.WriteLine($"{"Valor",-15} {"Descripción",-20} {"Tipo",-15} {"Palabra clave",-10}");
            Console.WriteLine(new string('-', 70)); // Línea de separación.
            foreach (Token token in tokens) // Recorre cada token y muestra su información.
            {
                Console.WriteLine(token);
            }
        }
        catch (Exception ex) // Captura errores en la lectura del archivo o el análisis.
        {
            Console.WriteLine($"Error: {ex.Message}"); // Muestra el mensaje de error.
        }
    }
}
