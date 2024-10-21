using System;
using System.Collections.Generic;

public class NodoExpresion
{
    public string Valor { get; set; }
    public NodoExpresion Izquierda { get; set; }
    public NodoExpresion Derecha { get; set; }
    public List<NodoExpresion> Sentencias { get; set; } = new List<NodoExpresion>();

    public NodoExpresion(string valor)
    {
        Valor = valor;
    }

    public void Imprimir(string prefijo = "", bool esIzquierda = true)
    {
        Console.WriteLine(prefijo + (esIzquierda ? "├── " : "└── ") + Valor);

        if (Izquierda != null)
        {
            Izquierda.Imprimir(prefijo + (esIzquierda ? "│   " : "    "), true);
        }

        if (Derecha != null)
        {
            Derecha.Imprimir(prefijo + (esIzquierda ? "│   " : "    "), false);
        }

        foreach (var sentencia in Sentencias)
        {
            sentencia.Imprimir(prefijo + "    ", false);
        }
    }
}

public class Sentencias
{
    public List<NodoExpresion> ListaSentencias { get; private set; } = new List<NodoExpresion>();

    public void AgregarSentencia(NodoExpresion sentencia)
    {
        ListaSentencias.Add(sentencia);
    }
}
