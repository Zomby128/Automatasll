//  Nombre: Adolfo Alejandro Granados Cosio
//  Matricula: 22760037
//  Trabajo: Analizador Lexico, Analizador Sintactico, Arbol Sintactico y Generador de Cuádruplos en C#
//  Profesor: Luis Armando Cárdenas Florido
using System;
using System.Collections.Generic;
using System.IO;
// Asegúrate de agregar el namespace del GeneradorCuadruplos
// using TuEspacioDeNombres;

class Program
{
    static void Main(string[] args)
    {
        // Leer el contenido del archivo de código fuente
        string codigoFuente = File.ReadAllText("codigo_fuente.txt");

        // Inicializar el lexer y generar la lista de tokens
        Lexer lexer = new Lexer(codigoFuente);
        List<Token> tokens = lexer.GenerarTokens();

        // Preguntar al usuario si desea ver la tabla de tokens, el árbol sintáctico, el AST o los cuádruplos
        Console.WriteLine("Seleccione una opción:");
        Console.WriteLine("1. Mostrar tabla de tokens");
        Console.WriteLine("2. Mostrar árbol sintáctico completo");
        Console.WriteLine("3. Mostrar árbol de sintaxis abstracta (AST)");
        Console.WriteLine("4. Mostrar tabla de cuádruplos");
        string opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                MostrarTablaDeTokens(tokens);
                break;

            case "2":
                GenerarArbolSintactico(tokens);
                break;

            case "3":
                GenerarAST(tokens);
                break;

            case "4":
                GenerarCuadruplos(tokens);
                break;

            default:
                Console.WriteLine("Opción inválida. Seleccione 1, 2, 3 o 4.");
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
            Console.WriteLine("Árbol Sintáctico Completo:");
            ImprimirArbol(arbol, 0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al parsear: {e.Message}");
        }
    }

    static void GenerarAST(List<Token> tokens)
    {
        Parser parser = new Parser(tokens);
        ASTGenerator astGenerator = new ASTGenerator(); // Instanciar ASTGenerator
        try
        {
            NodoExpresion arbol = parser.ParsearSentencia();
            NodoExpresion ast = astGenerator.SimplificarAST(arbol); // Llamar a SimplificarAST en ASTGenerator
            Console.WriteLine("Árbol de Sintaxis Abstracta (AST):");
            ImprimirAST(ast);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al generar AST: {e.Message}");
        }
    }

    static void GenerarCuadruplos(List<Token> tokens)
    {
        Parser parser = new Parser(tokens);
        GeneradorCuadruplos generator = new GeneradorCuadruplos(); // Instanciar el generador de cuádruplos

        try
        {
            // Parsear el código para obtener el árbol sintáctico
            NodoExpresion arbol = parser.ParsearSentencia();
            if (arbol == null)
            {
                Console.WriteLine("Error: No se pudo generar el árbol sintáctico.");
                return;
            }

            // Generar los cuádruplos desde el árbol sintáctico
            generator.GenerarCuadruplos(arbol);

            // Mostrar los cuádruplos en una tabla
            Console.WriteLine("\nCuádruplos Generados:");
            generator.MostrarCuadruplos();
        }
        catch (Exception e)
        {
            // Capturar y mostrar errores durante la generación de cuádruplos
            Console.WriteLine($"Error al generar cuádruplos: {e.Message}");
        }
    }

    static void ImprimirArbol(NodoExpresion nodo, int nivel)
    {
        if (nodo == null)
            return;

        // Imprimir el valor del nodo
        Console.WriteLine(new string(' ', nivel * 4) + nodo.Valor);

        // Imprimir el hijo izquierdo
        if (nodo.Izquierda != null)
            ImprimirArbol(nodo.Izquierda, nivel + 1);

        // Imprimir el hijo derecho
        if (nodo.Derecha != null)
            ImprimirArbol(nodo.Derecha, nivel + 1);

        // Si hay sentencias, imprimirlas también
        if (nodo.Sentencias != null)
        {
            foreach (var sentencia in nodo.Sentencias)
                ImprimirArbol(sentencia, nivel + 1);
        }
    }

    static void ImprimirAST(NodoExpresion nodo, string prefijo = "", bool esUltimo = true)
    {
        if (nodo == null)
            return;

        // Solo imprimimos nodos que aporten información semántica
        if (nodo.Valor == "{" || nodo.Valor == "}" || nodo.Valor == "(" || nodo.Valor == ")" || nodo.Valor == ";")
            return;

        // Imprimir el nodo actual
        Console.Write(prefijo);
        Console.Write(esUltimo ? "└── " : "├── ");
        Console.WriteLine(nodo.Valor);

        // Obtener los hijos relevantes para el AST
        var hijos = new List<NodoExpresion>();
        if (nodo.Izquierda != null && nodo.Izquierda.Valor != ";") hijos.Add(nodo.Izquierda);
        if (nodo.Derecha != null && nodo.Derecha.Valor != ";") hijos.Add(nodo.Derecha);
        hijos.AddRange(nodo.Sentencias);

        // Imprimir cada hijo relevante recursivamente
        for (int i = 0; i < hijos.Count; i++)
        {
            ImprimirAST(hijos[i], prefijo + (esUltimo ? "    " : "│   "), i == hijos.Count - 1);
        }
    }
}
