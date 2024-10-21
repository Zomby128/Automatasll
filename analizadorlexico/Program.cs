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

        // Indentación para mostrar el nivel de profundidad
        Console.WriteLine(new string(' ', nivel * 2) + nodo.Valor);

        // Recursión para los nodos hijos (Izquierda y Derecha)
        if (nodo.Izquierda != null)
        {
            ImprimirArbol(nodo.Izquierda, nivel + 1);
        }

        if (nodo.Derecha != null)
        {
            ImprimirArbol(nodo.Derecha, nivel + 1);
        }

        // Si hay sentencias, imprimirlas
        if (nodo.Sentencias != null)
        {
            foreach (var sentencia in nodo.Sentencias)
            {
                ImprimirArbol(sentencia, nivel + 1);
            }
        }
    }
}
