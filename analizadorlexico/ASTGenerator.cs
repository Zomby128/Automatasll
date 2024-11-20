using System;
using System.Collections.Generic;

public class ASTGenerator
{
    // Método para simplificar el árbol sintáctico y generar el AST
    public NodoExpresion SimplificarAST(NodoExpresion nodo)
    {
        if (nodo == null)
            return null;

        // Si el nodo actual es un punto y coma, se usa como separador en el AST
        if (nodo.Valor == ";")
        {
            // Crear un nuevo nodo raíz para el punto y coma
            NodoExpresion nodoSeparador = new NodoExpresion(";");

            // La instrucción previa al ; está a la izquierda
            nodoSeparador.Izquierda = SimplificarAST(nodo.Izquierda);

            // El próximo nodo después del ; está a la derecha
            nodoSeparador.Derecha = SimplificarAST(nodo.Derecha);

            return nodoSeparador;
        }

        // Crear un nuevo nodo que conserva solo la información semántica
        NodoExpresion astNodo = new NodoExpresion(nodo.Valor)
        {
            Sentencias = new List<NodoExpresion>() // Inicializar la lista de sentencias
        };

        // Si el nodo es una estructura de control, maneja sentencias anidadas
        if (nodo.Valor == "if" || nodo.Valor == "else" || nodo.Valor == "while" || nodo.Valor == "for")
        {
            astNodo.Izquierda = SimplificarAST(nodo.Izquierda); // Condición o inicialización
            astNodo.Derecha = SimplificarAST(nodo.Derecha);     // Bloque de código o incremento

            // Procesar las sentencias anidadas en estructuras de control
            if (nodo.Sentencias != null) // Verificar que Sentencias no sea null
            {
                foreach (var sentencia in nodo.Sentencias)
                {
                    var simplificada = SimplificarAST(sentencia);
                    if (simplificada != null)
                        astNodo.Sentencias.Add(simplificada);
                }
            }
        }
        else
        {
            // Procesar recursivamente los hijos izquierdo y derecho
            if (nodo.Izquierda != null)
                astNodo.Izquierda = SimplificarAST(nodo.Izquierda);

            if (nodo.Derecha != null)
                astNodo.Derecha = SimplificarAST(nodo.Derecha);

            // Filtrar y simplificar otras sentencias no estructurales
            if (nodo.Sentencias != null) // Verificar que Sentencias no sea null
            {
                foreach (var sentencia in nodo.Sentencias)
                {
                    var simplificada = SimplificarAST(sentencia);
                    if (simplificada != null)
                        astNodo.Sentencias.Add(simplificada);
                }
            }
        }

        return astNodo;
    }

    // Método auxiliar para depuración o pruebas
    public void PrintAST(NodoExpresion nodo, string prefijo = "", bool esUltimo = true)
    {
        if (nodo == null)
            return;

        // Imprimir el nodo actual
        Console.Write(prefijo);
        Console.Write(esUltimo ? "└── " : "├── ");
        Console.WriteLine(nodo.Valor);

        // Obtener los hijos relevantes para el AST
        var hijos = new List<NodoExpresion>();
        if (nodo.Izquierda != null) hijos.Add(nodo.Izquierda);
        if (nodo.Derecha != null) hijos.Add(nodo.Derecha);
        hijos.AddRange(nodo.Sentencias);

        // Imprimir cada hijo relevante recursivamente
        for (int i = 0; i < hijos.Count; i++)
        {
            PrintAST(hijos[i], prefijo + (esUltimo ? "    " : "│   "), i == hijos.Count - 1);
        }
    }
}