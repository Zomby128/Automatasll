using System;
using System.Collections.Generic;

public class GeneradorCuadruplos
{
    // Clase Cuadruplo para representar las instrucciones intermedias
    public class Cuadruplo
    {
        public string Operador { get; set; }
        public string Operando1 { get; set; }
        public string Operando2 { get; set; }
        public string Resultado { get; set; }

        public Cuadruplo(string operador, string operando1, string operando2, string resultado)
        {
            Operador = operador;
            Operando1 = operando1;
            Operando2 = operando2;
            Resultado = resultado;
        }

        public override string ToString()
        {
            return $"{Operador.PadRight(8)} | {Operando1?.PadRight(9)} | {Operando2?.PadRight(9)} | {Resultado}";
        }
    }

    private List<Cuadruplo> cuadruplos;
    private int contadorTemporales;
    private int contadorEtiquetas;

    public GeneradorCuadruplos()
    {
        cuadruplos = new List<Cuadruplo>();
        contadorTemporales = 0;
        contadorEtiquetas = 0;
    }

    public List<Cuadruplo> GenerarCuadruplos(NodoExpresion nodo)
    {
        if (nodo == null) return cuadruplos;

        switch (nodo.Valor)
        {
            case "=":
                GenerarAsignacion(nodo);
                break;

            case "if":
                GenerarIfElse(nodo);
                break;

            case "while":
                GenerarWhile(nodo);
                break;

            case "for":
                GenerarFor(nodo);
                break;

            case "switch":
                GenerarSwitch(nodo);
                break;

            default:
                GenerarExpresion(nodo);  // Para operaciones aritméticas y asignaciones simples
                break;
        }

        return cuadruplos;
    }

    // Generar cuádruplo para asignación
    private void GenerarAsignacion(NodoExpresion nodo)
    {
        string operando1 = GenerarExpresion(nodo.Derecha); // Expresión en el lado derecho
        cuadruplos.Add(new Cuadruplo("=", operando1, "", nodo.Izquierda.Valor)); // Asignar a la variable de la izquierda
    }

    // Generar cuádruplos para sentencias 'if' y 'else'
    private void GenerarIfElse(NodoExpresion nodo)
    {
        string condicion = GenerarExpresion(nodo.Izquierda);  // Expresión condicional
        string etiquetaElse = GenerarEtiqueta();              // Etiqueta para el bloque 'else'
        string etiquetaFin = GenerarEtiqueta();               // Etiqueta de fin para el 'if' o 'else'

        cuadruplos.Add(new Cuadruplo("if_false", condicion, "", etiquetaElse));  // Si la condición es falsa, ir a la etiqueta 'else'

        // Procesar el bloque 'if'
        GenerarCuadruplos(nodo.Derecha);  
        cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaFin));  // Salto al final del bloque 'if'

        // Procesar el bloque 'else'
        cuadruplos.Add(new Cuadruplo($"{etiquetaElse}:", "", "", ""));
        if (nodo.Sentencias.Count > 0)  // Si existe un bloque 'else', procesarlo
        {
            GenerarCuadruplos(nodo.Sentencias[0]);
        }

        // Fin del bloque 'if'/'else'
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

    // Generar cuádruplos para un ciclo 'while'
    private void GenerarWhile(NodoExpresion nodo)
    {
        string etiquetaInicio = GenerarEtiqueta();  // Etiqueta para el inicio del ciclo
        string etiquetaFin = GenerarEtiqueta();     // Etiqueta para el fin del ciclo

        cuadruplos.Add(new Cuadruplo($"{etiquetaInicio}:", "", "", ""));
        string condicion = GenerarExpresion(nodo.Izquierda);  // Generar la condición del ciclo
        cuadruplos.Add(new Cuadruplo("if_false", condicion, "", etiquetaFin));  // Salto si la condición es falsa

        // Procesar el bloque dentro del ciclo
        GenerarCuadruplos(nodo.Derecha);

        cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaInicio));  // Salto al inicio del ciclo
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));   // Fin del ciclo
    }

    // Generar cuádruplos para un ciclo 'for'
    private void GenerarFor(NodoExpresion nodo)
    {
        NodoExpresion inicializacion = nodo.Sentencias[0];  // Inicialización del ciclo
        NodoExpresion condicion = nodo.Izquierda;           // Condición del ciclo
        NodoExpresion incremento = nodo.Sentencias[1];      // Incremento del ciclo
        string etiquetaInicio = GenerarEtiqueta();         // Etiqueta de inicio del ciclo
        string etiquetaFin = GenerarEtiqueta();            // Etiqueta de fin del ciclo

        // Procesar la inicialización
        GenerarCuadruplos(inicializacion);

        // Comenzar el ciclo
        cuadruplos.Add(new Cuadruplo($"{etiquetaInicio}:", "", "", ""));
        string condicionGenerada = GenerarExpresion(condicion);
        cuadruplos.Add(new Cuadruplo("if_false", condicionGenerada, "", etiquetaFin));  // Salto si la condición es falsa

        // Procesar el bloque dentro del ciclo
        GenerarCuadruplos(nodo.Derecha);

        // Procesar el incremento del ciclo
        GenerarCuadruplos(incremento);

        cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaInicio));  // Salto al inicio del ciclo
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));   // Fin del ciclo
    }

    // Generar cuádruplos para un 'switch'
    private void GenerarSwitch(NodoExpresion nodo)
    {
        string variable = GenerarExpresion(nodo.Izquierda);  // Expresión de la variable del 'switch'
        string etiquetaFin = GenerarEtiqueta();              // Etiqueta de fin para el 'switch'

        foreach (var sentencia in nodo.Sentencias)
        {
            if (sentencia.Valor == "case")
            {
                string etiquetaCaso = GenerarEtiqueta();  // Etiqueta para el caso
                string constante = GenerarExpresion(sentencia.Izquierda);  // Expresión dentro del case

                // Comparar la variable con el valor del case
                cuadruplos.Add(new Cuadruplo("if", $"{variable} == {constante}", "", etiquetaCaso));
                cuadruplos.Add(new Cuadruplo($"{etiquetaCaso}:", "", "", ""));

                // Generar cuádruplos para el bloque de este 'case'
                GenerarCuadruplos(sentencia.Derecha);
            }
            else if (sentencia.Valor == "default")
            {
                // Procesar la sentencia 'default'
                GenerarCuadruplos(sentencia.Derecha);
            }
        }

        // Fin del switch
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

    // Generar cuádruplos para expresiones
    private string GenerarExpresion(NodoExpresion nodo)
    {
        if (nodo == null) return null;

        // Si es una expresión simple, retorna el valor
        if (nodo.Izquierda == null && nodo.Derecha == null)
            return nodo.Valor;

        // Si no, procesamos la expresión completa
        string operando1 = GenerarExpresion(nodo.Izquierda);
        string operando2 = GenerarExpresion(nodo.Derecha);
        string temporal = GenerarTemporal();

        // Generar cuádruplo para la operación
        cuadruplos.Add(new Cuadruplo(nodo.Valor, operando1, operando2, temporal));
        return temporal;
    }

    // Generar un temporal
    private string GenerarTemporal()
    {
        return $"t{contadorTemporales++}";
    }

    // Generar una etiqueta
    private string GenerarEtiqueta()
    {
        return $"L{contadorEtiquetas++}";
    }

    // Mostrar los cuádruplos generados
    public void MostrarCuadruplos()
    {
        Console.WriteLine("Cuádruplos Generados:");
        Console.WriteLine("Operador | Operando1 | Operando2 | Resultado");
        foreach (var cuadruplo in cuadruplos)
        {
            Console.WriteLine(cuadruplo);
        }
    }
}
