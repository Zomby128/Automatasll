//  Nombre: Adolfo Alejandro Granados Cosio
//  Matricula: 22760037
//  Trabajo: Analizador Lexico en C#
//  Profesor: Luis Armando Cárdenas Florido
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Leer el contenido del archivo de código fuente
        string codigoFuente = File.ReadAllText("codigo_fuente.txt");

        // Inicializar el lexer y generar la lista de tokens
        Lexer lexer = new Lexer(codigoFuente);
        List<Token> tokens = lexer.GenerarTokens();

        // Preguntar al usuario si desea ver la tabla de tokens o el árbol sintáctico
        Console.WriteLine("Seleccione una opción:");
        Console.WriteLine("1. Mostrar tabla de tokens");
        Console.WriteLine("2. Mostrar árbol sintáctico");
        string opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                MostrarTablaDeTokens(tokens);
                break;

            case "2":
                GenerarArbolSintactico(tokens);
                break;

            default:
                Console.WriteLine("Opción inválida. Seleccione 1 o 2.");
                break;
        }
    }

    static void MostrarTablaDeTokens(List<Token> tokens)
    {
        Console.WriteLine("Tabla de Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine($"Tipo: {token.Type}, Valor: {token.Value}");
        }
    }

    static void GenerarArbolSintactico(List<Token> tokens)
    {
        Parser parser = new Parser(tokens);
        try
        {
            NodoExpresion arbol = parser.ParsearSentencia();
            Console.WriteLine("Árbol Sintáctico:");
            ImprimirArbol(arbol, 0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al parsear: {e.Message}");
        }
    }

    static void ImprimirArbol(NodoExpresion nodo, int nivel)
{
    if (nodo == null)
        return;

    // Imprimir el valor del nodo
    Console.WriteLine(new string(' ', nivel * 4) + nodo.Valor);

    // Conectar nodos hijos si existen
    if (nodo.Izquierda != null || nodo.Derecha != null)
    {
        // Espacio para la línea de conexión
        Console.Write(new string(' ', nivel * 4));

        // Conectar el nodo izquierdo
        if (nodo.Izquierda != null)
        {
            Console.Write("|");
        }
        else
        {
            Console.Write(" ");
        }

        // Imprimir la conexión del nodo derecho
        if (nodo.Derecha != null)
        {
            Console.WriteLine(" \\");
        }
        else
        {
            Console.WriteLine("  ");
        }
    }

    // Imprimir el hijo izquierdo
    if (nodo.Izquierda != null)
    {
        ImprimirArbol(nodo.Izquierda, nivel + 1);
    }

    // Imprimir el hijo derecho
    if (nodo.Derecha != null)
    {
        Console.Write(new string(' ', nivel * 4 + 1)); // Alinear el hijo derecho
        ImprimirArbol(nodo.Derecha, nivel + 1);
    }

    // Si hay sentencias, imprimirlas también
    if (nodo.Sentencias != null)
    {
        foreach (var sentencia in nodo.Sentencias)
        {
            ImprimirArbol(sentencia, nivel + 1);
        }
    }
}

// Función de uso para la impresión
static void ImprimirArbolEstructurado(NodoExpresion nodo)
{
    ImprimirArbol(nodo, 0);
}

}
