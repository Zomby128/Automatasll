using System;
using System.Collections.Generic;
// Clase que representa un nodo en un árbol de expresiones
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
}

public class Sentencias
{
    public List<NodoExpresion> ListaSentencias { get; private set; } = new List<NodoExpresion>();

    public void AgregarSentencia(NodoExpresion sentencia)
    {
        ListaSentencias.Add(sentencia);
    }
}
