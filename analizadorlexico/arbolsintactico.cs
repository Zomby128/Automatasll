// Clase que representa un nodo en el árbol de expresión
public class NodoExpresion
{
    public string Valor { get; set; } // Valor del nodo (puede ser un operador o un número).
    public NodoExpresion? Izquierda { get; set; } // Hijo izquierdo del nodo (puede ser otro nodo).
    public NodoExpresion? Derecha { get; set; } // Hijo derecho del nodo (puede ser otro nodo).

    // Constructor para inicializar un nodo con un valor específico.
    public NodoExpresion(string valor)
    {
        Valor = valor; // Inicializa el valor del nodo con el valor proporcionado.
        Izquierda = null; // Inicializa el hijo izquierdo como nulo.
        Derecha = null; // Inicializa el hijo derecho como nulo.
    }
}

// Clase que implementa un analizador sintáctico para construir un árbol de expresiones
public class AnalizadorSintactico
{
    private string[] tokens; // Arreglo donde se guardarán los tokens de la expresión.
    private int posicion; // Posición actual en el arreglo de tokens.

    // Constructor que inicializa el analizador con los tokens proporcionados.
    public AnalizadorSintactico(string[] tokens)
    {
        this.tokens = tokens; // Asigna los tokens a la variable de instancia.
        posicion = 0; // Inicializa la posición en 0.
    }

    // Método para obtener el token actual en la posición actual.
    private string ObtenerTokenActual()
    {
        // Retorna el token actual si está dentro de los límites del arreglo, de lo contrario retorna una cadena vacía.
        return posicion < tokens.Length ? tokens[posicion] : "";
    }

    // Método para avanzar a la siguiente posición en el arreglo de tokens.
    private void Avanzar()
    {
        posicion++; // Incrementa la posición para avanzar al siguiente token.
    }

    // Método principal para iniciar el análisis y devolver la raíz del árbol de expresión.
    public NodoExpresion Parsear()
    {
        return ParsearExpresion(); // Inicia el análisis llamando al método ParsearExpresion.
    }

    // Método para analizar una expresión que puede contener sumas y restas.
    private NodoExpresion ParsearExpresion()
    {
        // Analiza el primer término de la expresión.
        NodoExpresion izquierda = ParsearTermino();
        
        // Bucle para continuar analizando mientras haya operaciones de suma o resta.
        while (true)
        {
            string tokenActual = ObtenerTokenActual(); // Obtiene el token actual.
            if (tokenActual == "+" || tokenActual == "-") // Verifica si el token es un operador de suma o resta.
            {
                Avanzar(); // Avanza para consumir el operador.
                // Analiza el siguiente término a la derecha del operador.
                NodoExpresion derecha = ParsearTermino();
                // Crea un nuevo nodo que representa la operación con el operador y los dos operandos.
                NodoExpresion nuevoNodo = new NodoExpresion(tokenActual)
                {
                    Izquierda = izquierda, // Asigna el nodo izquierdo.
                    Derecha = derecha // Asigna el nodo derecho.
                };
                izquierda = nuevoNodo; // Actualiza el nodo izquierdo para el siguiente ciclo.
            }
            else
            {
                break; // Si no hay más operadores de suma o resta, se rompe el bucle.
            }
        }
        return izquierda; // Devuelve el nodo raíz de la expresión analizada.
    }

    // Método para analizar un término que puede contener multiplicaciones y divisiones.
    private NodoExpresion ParsearTermino()
    {
        // Analiza el primer factor del término.
        NodoExpresion izquierda = ParsearFactor();
        
        // Bucle para continuar analizando mientras haya operaciones de multiplicación o división.
        while (true)
        {
            string tokenActual = ObtenerTokenActual(); // Obtiene el token actual.
            if (tokenActual == "*" || tokenActual == "/") // Verifica si el token es un operador de multiplicación o división.
            {
                Avanzar(); // Avanza para consumir el operador.
                // Analiza el siguiente factor a la derecha del operador.
                NodoExpresion derecha = ParsearFactor();
                // Crea un nuevo nodo que representa la operación con el operador y los dos operandos.
                NodoExpresion nuevoNodo = new NodoExpresion(tokenActual)
                {
                    Izquierda = izquierda, // Asigna el nodo izquierdo.
                    Derecha = derecha // Asigna el nodo derecho.
                };
                izquierda = nuevoNodo; // Actualiza el nodo izquierdo para el siguiente ciclo.
            }
            else
            {
                break; // Si no hay más operadores de multiplicación o división, se rompe el bucle.
            }
        }
        return izquierda; // Devuelve el nodo raíz del término analizado.
    }

    // Método para analizar un factor, que puede ser un número o una expresión entre paréntesis.
    private NodoExpresion ParsearFactor()
    {
        string tokenActual = ObtenerTokenActual(); // Obtiene el token actual.
        if (tokenActual == "(") // Verifica si el token es un paréntesis de apertura.
        {
            Avanzar(); // Consume el '('.
            // Llama recursivamente a ParsearExpresion para analizar la expresión dentro de los paréntesis.
            NodoExpresion nodo = ParsearExpresion();
            Avanzar(); // Consume el ')'.
            return nodo; // Devuelve el nodo analizado dentro de los paréntesis.
        }
        else
        {
            Avanzar(); // Consume el token actual.
            return new NodoExpresion(tokenActual); // Crea un nuevo nodo para el token (número u operador).
        }
    }
}
