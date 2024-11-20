using System;
using System.Collections.Generic;

public class Quadruple
{
    public string Operador { get; set; }
    public string Operando1 { get; set; }
    public string Operando2 { get; set; }
    public string Resultado { get; set; }

    public Quadruple(string operador, string operando1, string operando2, string resultado)
    {
        Operador = operador;
        Operando1 = operando1;
        Operando2 = operando2;
        Resultado = resultado;
    }
}

public class QuadrupleGenerator
{
    private List<Quadruple> cuadruplos;
    private int contadorTemporales;

    public QuadrupleGenerator()
    {
        cuadruplos = new List<Quadruple>();
        contadorTemporales = 0;
    }

    public List<Quadruple> GenerarCuadruplos(NodoExpresion nodo)
    {
        if (nodo == null) return cuadruplos;

        if (nodo.Valor == "=")
        {
            string resultado = nodo.Izquierda.Valor;
            string operando1 = GenerarCuadruplosRecursivo(nodo.Derecha);
            cuadruplos.Add(new Quadruple("=", operando1, null, resultado));
        }
        else
        {
            GenerarCuadruplosRecursivo(nodo);
        }

        return cuadruplos;
    }

    private string GenerarCuadruplosRecursivo(NodoExpresion nodo)
    {
        if (nodo == null) return null;

        if (nodo.Izquierda == null && nodo.Derecha == null)
            return nodo.Valor;

        string operando1 = GenerarCuadruplosRecursivo(nodo.Izquierda);
        string operando2 = GenerarCuadruplosRecursivo(nodo.Derecha);

        string temporal = GenerarTemporal();
        cuadruplos.Add(new Quadruple(nodo.Valor, operando1, operando2, temporal));

        return temporal;
    }

    private string GenerarTemporal()
    {
        return $"t{contadorTemporales++}";
    }

    public void MostrarCuadruplos()
    {
        Console.WriteLine("Cuádruplos Generados:");
        Console.WriteLine("Operador | Operando1 | Operando2 | Resultado");
        foreach (var cuadruplo in cuadruplos)
        {
            Console.WriteLine($"{cuadruplo.Operador.PadRight(8)} | {cuadruplo.Operando1.PadRight(9)} | {cuadruplo.Operando2?.PadRight(9)} | {cuadruplo.Resultado}");
        }
    }
}
