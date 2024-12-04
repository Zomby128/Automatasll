using System;
using System.Collections.Generic;

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

            case "do":
                GenerarDoWhile(nodo);
                break;

            case "for":
                GenerarFor(nodo);
                break;

            case "switch":
                GenerarSwitch(nodo);
                break;

            default:
                GenerarExpresion(nodo);
                break;
        }

        return cuadruplos;
    }

    private void GenerarAsignacion(NodoExpresion nodo)
    {
        // Para asignaciones como x = 5 o x = y + z
        string operando1 = GenerarExpresion(nodo.Derecha);
        string resultado = nodo.Izquierda.Valor; // La variable que recibe el valor

        cuadruplos.Add(new Cuadruplo("=", operando1, "", resultado));
    }

    private void GenerarIfElse(NodoExpresion nodo)
    {
        string condicion = GenerarExpresion(nodo.Izquierda);
        string etiquetaElse = GenerarEtiqueta();
        string etiquetaFin = GenerarEtiqueta();

        cuadruplos.Add(new Cuadruplo("if_false", condicion, "", etiquetaElse));
        GenerarCuadruplos(nodo.Derecha);

        if (nodo.Sentencias.Count > 1)
        {
            cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaFin));
            cuadruplos.Add(new Cuadruplo($"{etiquetaElse}:", "", "", ""));
            GenerarCuadruplos(nodo.Sentencias[1]);
        }

        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

    private void GenerarWhile(NodoExpresion nodo)
    {
        string etiquetaInicio = GenerarEtiqueta();
        string etiquetaFin = GenerarEtiqueta();

        cuadruplos.Add(new Cuadruplo($"{etiquetaInicio}:", "", "", ""));
        string condicion = GenerarExpresion(nodo.Izquierda);
        cuadruplos.Add(new Cuadruplo("if_false", condicion, "", etiquetaFin));
        GenerarCuadruplos(nodo.Derecha);
        cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaInicio));
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

    private void GenerarDoWhile(NodoExpresion nodo)
    {
        string etiquetaInicio = GenerarEtiqueta();
        cuadruplos.Add(new Cuadruplo($"{etiquetaInicio}:", "", "", ""));
        GenerarCuadruplos(nodo.Derecha);

        string condicion = GenerarExpresion(nodo.Izquierda);
        cuadruplos.Add(new Cuadruplo("if", condicion, "", etiquetaInicio));
    }

    private void GenerarFor(NodoExpresion nodo)
    {
        NodoExpresion inicializacion = nodo.Sentencias[0];
        NodoExpresion condicion = nodo.Izquierda;
        NodoExpresion incremento = nodo.Sentencias[1];
        string etiquetaInicio = GenerarEtiqueta();
        string etiquetaFin = GenerarEtiqueta();

        GenerarCuadruplos(inicializacion);
        cuadruplos.Add(new Cuadruplo($"{etiquetaInicio}:", "", "", ""));
        string condicionGenerada = GenerarExpresion(condicion);
        cuadruplos.Add(new Cuadruplo("if_false", condicionGenerada, "", etiquetaFin));
        GenerarCuadruplos(nodo.Derecha);
        GenerarCuadruplos(incremento);
        cuadruplos.Add(new Cuadruplo("goto", "", "", etiquetaInicio));
        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

    private void GenerarSwitch(NodoExpresion nodo)
    {
        string variable = GenerarExpresion(nodo.Izquierda);
        string etiquetaFin = GenerarEtiqueta();

        foreach (var sentencia in nodo.Sentencias)
        {
            if (sentencia.Valor == "case")
            {
                string etiquetaCaso = GenerarEtiqueta();
                string constante = GenerarExpresion(sentencia.Izquierda);
                cuadruplos.Add(new Cuadruplo("if", $"{variable} == {constante}", "", etiquetaCaso));
                cuadruplos.Add(new Cuadruplo($"{etiquetaCaso}:", "", "", ""));
                GenerarCuadruplos(sentencia.Derecha);
            }
            else if (sentencia.Valor == "default")
            {
                GenerarCuadruplos(sentencia.Derecha);
            }
        }

        cuadruplos.Add(new Cuadruplo($"{etiquetaFin}:", "", "", ""));
    }

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

    private string GenerarTemporal()
    {
        return $"t{contadorTemporales++}";
    }

    private string GenerarEtiqueta()
    {
        return $"L{contadorEtiquetas++}";
    }

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
