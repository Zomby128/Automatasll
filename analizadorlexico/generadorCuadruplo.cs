using System;
using System.Collections.Generic;

public class GeneradorCuadruplos
{
    private List<Cuadruplo> cuadruplos;
    private int contadorTemporales;
    private int contadorEtiquetas;

    public GeneradorCuadruplos()
    {
        cuadruplos = new List<Cuadruplo>();
        contadorTemporales = 0;
        contadorEtiquetas = 0;
    }

    // Método principal: Genera cuádruplos a partir del árbol sintáctico
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

            default: 
                GenerarExpresion(nodo); // Procesar operaciones aritméticas o casos aislados
                break;
        }

        // Verificar nodos adicionales después de procesar
        if (nodo.Sentencias != null)
        {
            foreach (var sentencia in nodo.Sentencias)
                GenerarCuadruplos(sentencia);
        }

        return cuadruplos;
    }

    // Genera cuádruplos para asignaciones
    private void GenerarAsignacion(NodoExpresion nodo)
    {
        string operando1 = GenerarExpresion(nodo.Derecha);
        cuadruplos.Add(new Cuadruplo("=", operando1, "", nodo.Izquierda.Valor));
    }

private void GenerarIfElse(NodoExpresion nodo)
{
    string condicion = GenerarExpresion(nodo.Izquierda);  // Generamos la condición
    string etiquetaElse = GenerarEtiqueta();              // Etiqueta para el bloque 'else'
    string etiquetaFin = GenerarEtiqueta();               // Etiqueta de fin para el 'if'/'else'

    // Cuádruplo para la condición (si la condición es falsa, saltamos a 'else')
    cuadruplos.Add(new Cuadruplo("if_false", condicion, "", etiquetaElse));

    // Generamos cuádruplos para el bloque 'if'
    GenerarCuadruplos(nodo.Derecha);  

    // Salto al final del bloque 'if-else'
    cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaFin));  

    // Etiqueta de inicio para el bloque 'else'
    cuadruplos.Add(new Cuadruplo($"{etiquetaElse}:", "", "", ""));

    // Generamos cuádruplos para el bloque 'else'
    if (nodo.Sentencias.Count > 0) 
    {
        GenerarCuadruplos(nodo.Sentencias[0]);
    }

    // Fin del bloque 'if'/'else'
    cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
}

    // Genera cuádruplos para ciclos 'while'
    private void GenerarWhile(NodoExpresion nodo)
    {
        string etiquetaInicio = GenerarEtiqueta();
        string etiquetaFin = GenerarEtiqueta();

        cuadruplos.Add(new Cuadruplo($"{etiquetaInicio}:", "", "", ""));
        string condicion = GenerarExpresion(nodo.Izquierda);
        cuadruplos.Add(new Cuadruplo("if_false", condicion, "", etiquetaFin));

        // Generar cuádruplos para el cuerpo del ciclo
        GenerarCuadruplos(nodo.Derecha);

        cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaInicio));
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

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


    // Genera cuádruplos para expresiones
    private string GenerarExpresion(NodoExpresion nodo)
    {
        if (nodo == null) return null;

        if (nodo.Izquierda == null && nodo.Derecha == null)
            return nodo.Valor;

        string operando1 = GenerarExpresion(nodo.Izquierda);
        string operando2 = GenerarExpresion(nodo.Derecha);
        string temporal = GenerarTemporal();

        cuadruplos.Add(new Cuadruplo(nodo.Valor, operando1, operando2, temporal));
        return temporal;
    }

    // Genera un identificador temporal
    private string GenerarTemporal() => $"t{contadorTemporales++}";

    // Genera una etiqueta para los saltos
    private string GenerarEtiqueta() => $"L{contadorEtiquetas++}";

    // Imprime la tabla de cuádruplos
    public void MostrarCuadruplos()
    {
        Console.WriteLine("\nCuádruplos Generados:");
        Console.WriteLine("Operador | Operando1 | Operando2 | Resultado");
        foreach (var cuadruplo in cuadruplos)
        {
            Console.WriteLine(cuadruplo);
        }
    }

    // Genera y muestra el código intermedio simplificado
    public void GenerarCodigoIntermedio()
    {
        Console.WriteLine("\nCódigo Intermedio Simplificado:");
        foreach (var cuadruplo in cuadruplos)
        {
            if (!string.IsNullOrEmpty(cuadruplo.Resultado))
            {
                Console.WriteLine($"{cuadruplo.Resultado} = {cuadruplo.Operando1} {cuadruplo.Operador} {cuadruplo.Operando2}".Trim());
            }
        }
    }
}

// Clase para representar un cuádruplo
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

    public override string ToString() =>
        $"{Operador.PadRight(8)} | {Operando1?.PadRight(9)} | {Operando2?.PadRight(9)} | {Resultado}";
}
