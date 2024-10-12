public class NodoExpresion
{
    public string Valor { get; set; } // Valor del nodo, puede ser un operador o una variable.
    public NodoExpresion Izquierda { get; set; } // Hijo izquierdo del nodo.
    public NodoExpresion Derecha { get; set; } // Hijo derecho del nodo.

    // Constructor de la clase.
    public NodoExpresion(string valor)
    {
        Valor = valor; // Inicializa el valor del nodo.
        Izquierda = null; // Inicializa el hijo izquierdo en null.
        Derecha = null; // Inicializa el hijo derecho en null.
    }

    // Método para imprimir el árbol sintáctico de forma visual.
    public void Imprimir(int nivel = 0)
    {
        if (Derecha != null) // Imprime el subárbol derecho.
        {
            Derecha.Imprimir(nivel + 1);
        }

        Console.WriteLine(new string(' ', nivel * 4) + Valor); // Imprime el valor del nodo con indentación.

        if (Izquierda != null) // Imprime el subárbol izquierdo.
        {
            Izquierda.Imprimir(nivel + 1);
        }
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
        return posicion < tokens.Length ? tokens[posicion] : ""; // Retorna el token actual o cadena vacía si está fuera de límites.
    }

    // Método para avanzar a la siguiente posición en el arreglo de tokens.
    private void Avanzar()
    {
        posicion++; // Incrementa la posición para avanzar al siguiente token.
    }

    // Método principal para iniciar el análisis y devolver la raíz del árbol de expresión.
    public NodoExpresion Parsear()
    {
        return ParsearSentencia(); // Inicia el análisis llamando al método ParsearSentencia.
    }

    // Método para analizar una sentencia
    /*private NodoExpresion ParsearSentencia()
    {
        string tokenActual = ObtenerTokenActual();
        if (tokenActual == "if")
        {
            Avanzar(); // Consumir 'if'
            var condicion = ParsearExpresion(); // Analizar la expresión de la condición
            if (ObtenerTokenActual() != "{")
            {
                throw new Exception("Se esperaba '{' después de la condición 'if'");
            }
            Avanzar(); // Consumir '{'
            var cuerpo = ParsearCuerpo(); // Analizar el cuerpo del if
            if (ObtenerTokenActual() != "}")
            {
                throw new Exception("Se esperaba '}' después del cuerpo 'if'");
            }
            Avanzar(); // Consumir '}'

            NodoExpresion ifNodo = new NodoExpresion("if")
            {
                Izquierda = condicion, // Asignar condición al hijo izquierdo
                Derecha = cuerpo // Asignar cuerpo al hijo derecho
            };

            return ifNodo; // Retornar nodo 'if' con su cuerpo.
        }
        else
        {
            return ParsearExpresion(); // Si no es una sentencia if, se analiza como una expresión.
        }
    }*/
    private NodoExpresion ParsearSentencia()
{
    string tokenActual = ObtenerTokenActual();
    
    if (tokenActual == "if")
    {
        Avanzar(); // Consumir 'if'
        var condicion = ParsearExpresion(); // Analizar la expresión de la condición
        if (ObtenerTokenActual() != "{")
        {
            throw new Exception("Se esperaba '{' después de la condición 'if'");
        }
        Avanzar(); // Consumir '{'
        var cuerpo = ParsearCuerpo(); // Analizar el cuerpo del if
        if (ObtenerTokenActual() != "}")
        {
            throw new Exception("Se esperaba '}' después del cuerpo 'if'");
        }
        Avanzar(); // Consumir '}'

        NodoExpresion ifNodo = new NodoExpresion("if")
        {
            Izquierda = condicion, // Asignar condición al hijo izquierdo
            Derecha = cuerpo // Asignar cuerpo al hijo derecho
        };

        // Comprobar si hay un else
        if (ObtenerTokenActual() == "else")
        {
            Avanzar(); // Consumir 'else'
            if (ObtenerTokenActual() != "{")
            {
                throw new Exception("Se esperaba '{' después de 'else'");
            }
            Avanzar(); // Consumir '{'
            var cuerpoElse = ParsearCuerpo(); // Analizar el cuerpo del else
            if (ObtenerTokenActual() != "}")
            {
                throw new Exception("Se esperaba '}' después del cuerpo 'else'");
            }
            Avanzar(); // Consumir '}'

            NodoExpresion elseNodo = new NodoExpresion("else") { Derecha = cuerpoElse };
            ifNodo.Derecha = elseNodo; // Asignar el nodo else al nodo if
        }

        return ifNodo; // Retornar nodo 'if' con su cuerpo.
    }
    else if (tokenActual == "while") // Añadir soporte para while
    {
        Avanzar(); // Consumir 'while'
        var condicion = ParsearExpresion(); // Analizar la expresión de la condición
        if (ObtenerTokenActual() != "{")
        {
            throw new Exception("Se esperaba '{' después de la condición 'while'");
        }
        Avanzar(); // Consumir '{'
        var cuerpo = ParsearCuerpo(); // Analizar el cuerpo del while
        if (ObtenerTokenActual() != "}")
        {
            throw new Exception("Se esperaba '}' después del cuerpo 'while'");
        }
        Avanzar(); // Consumir '}'

        NodoExpresion whileNodo = new NodoExpresion("while")
        {
            Izquierda = condicion, // Asignar condición al hijo izquierdo
            Derecha = cuerpo // Asignar cuerpo al hijo derecho
        };

        return whileNodo; // Retornar nodo 'while' con su cuerpo.
    }
    else
    {
        return ParsearExpresion(); // Si no es una sentencia if ni while, se analiza como una expresión.
    }
}


    // Método para analizar un cuerpo que puede contener múltiples expresiones
    private NodoExpresion ParsearCuerpo()
    {
        NodoExpresion nodoCuerpo = new NodoExpresion("cuerpo");
        while (true)
        {
            string tokenActual = ObtenerTokenActual();
            if (tokenActual == "}")
            {
                break; // Salir si se llega al final del cuerpo
            }
            else
            {
                // Analiza cada sentencia en el cuerpo
                NodoExpresion sentencia = ParsearAsignacion(); // Cambiar para parsear asignaciones
                nodoCuerpo.Derecha = sentencia; // Asignar la sentencia al nodo cuerpo
            }
        }
        return nodoCuerpo; // Retornar nodo del cuerpo
    }

    // Método para analizar una asignación
    private NodoExpresion ParsearAsignacion()
    {
        string variable = ObtenerTokenActual(); // Obtiene la variable
        if (variable == null || variable.Contains("=") || variable == "{" || variable == "}") // Verifica si el token actual es una asignación
        {
            throw new Exception("Se esperaba una variable");
        }
        Avanzar(); // Consume la variable

        if (ObtenerTokenActual() != "=") // Verifica que el siguiente token sea '='
        {
            throw new Exception("Se esperaba '=' en la asignación");
        }
        Avanzar(); // Consume '='

        var valor = ParsearExpresion(); // Analiza el valor a la derecha de la asignación
        return new NodoExpresion("asignado")
        {
            Izquierda = new NodoExpresion(variable), // Nodo izquierdo para la variable
            Derecha = valor // Nodo derecho para el valor
        };
    }

    // Método para analizar una expresión que puede contener comparaciones
    private NodoExpresion ParsearExpresion()
    {
        NodoExpresion izquierda = ParsearTermino(); // Analiza el primer término de la expresión
        
        while (true)
        {
            string tokenActual = ObtenerTokenActual(); // Obtiene el token actual.
            if (tokenActual == "-" || tokenActual == "+" || tokenActual == ">" || tokenActual == "<" || tokenActual == "==" || tokenActual == "!=") // Verifica si el token es un operador de comparación.
            {
                Avanzar(); // Avanza para consumir el operador.
                NodoExpresion derecha = ParsearTermino(); // Analiza el siguiente término a la derecha del operador.
                NodoExpresion nuevoNodo = new NodoExpresion(tokenActual)
                {
                    Izquierda = izquierda, // Asigna el nodo izquierdo.
                    Derecha = derecha // Asigna el nodo derecho.
                };
                izquierda = nuevoNodo; // Actualiza el nodo izquierdo para el siguiente ciclo.
            }
            else
            {
                break; // Si no hay más operadores, se rompe el bucle.
            }
        }
        return izquierda; // Devuelve el nodo raíz de la expresión analizada.
    }

    // Método para analizar un término que puede contener multiplicaciones y divisiones.
    private NodoExpresion ParsearTermino()
    {
        NodoExpresion izquierda = ParsearFactor(); // Analiza el primer factor del término
        
        while (true)
        {
            string tokenActual = ObtenerTokenActual(); // Obtiene el token actual.
            if (tokenActual == "*" || tokenActual == "/") // Verifica si el token es un operador de multiplicación o división.
            {
                Avanzar(); // Avanza para consumir el operador.
                NodoExpresion derecha = ParsearFactor(); // Analiza el siguiente factor a la derecha del operador.
                NodoExpresion nuevoNodo = new NodoExpresion(tokenActual)
                {
                    Izquierda = izquierda, // Asigna el nodo izquierdo.
                    Derecha = derecha // Asigna el nodo derecho.
                };
                izquierda = nuevoNodo; // Actualiza el nodo izquierdo para el siguiente ciclo.
            }
            else
            {
                break; // Si no hay más operadores, se rompe el bucle.
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
            NodoExpresion nodo = ParsearExpresion(); // Llama recursivamente a ParsearExpresion para analizar la expresión dentro de los paréntesis.
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
