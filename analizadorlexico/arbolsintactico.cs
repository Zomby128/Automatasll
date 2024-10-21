using System;
using System.Collections.Generic;

public class Parser
{
    private List<Token> tokens;
    private int posicion;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.posicion = 0;
    }

    private int Precedencia(string operador)
    {
        switch (operador)
        {
            case "^": return 4;
            case "*": case "/": case "%": return 3;
            case "+": case "-": return 2;
            case "<": case ">": case "<=": case ">=": case "==": case "!=": return 1;
            default: return 0;
        }
    }

    private bool EsOperador(string valor)
    {
        return Precedencia(valor) > 0;
    }

    private Token SiguienteToken()
    {
        if (posicion < tokens.Count)
        {
            return tokens[posicion++];
        }
        return null;
    }

    private void Retroceder()
    {
        if (posicion > 0)
            posicion--;
    }

    public NodoExpresion ParsearSentencia()
    {
        Token tokenActual = SiguienteToken();

        // Sentencia 'return'
        if (tokenActual.Type == TokenType.Keyword && tokenActual.Value == "return")
        {
            return ParsearReturnStatement();
        }

        // Sentencia 'if'
        if (tokenActual.Type == TokenType.Keyword && tokenActual.Value == "if")
        {
            return ParsearIfStatement();
        }

        // Sentencia 'while'
        if (tokenActual.Type == TokenType.Keyword && tokenActual.Value == "while")
        {
            return ParsearWhileStatement();
        }

        // Sentencia 'for'
        if (tokenActual.Type == TokenType.Keyword && tokenActual.Value == "for")
        {
            return ParsearForStatement();
        }

        // Asignación
        if (tokenActual.Type == TokenType.Identifier)
        {
            return ParsearAsignacion(tokenActual);
        }

        throw new Exception($"Error: Sentencia inesperada '{tokenActual.Value}'");
    }

    // Método para procesar una sentencia 'return'
    private NodoExpresion ParsearReturnStatement()
    {
        NodoExpresion expresion = ParsearExpresion();
        VerificarTokenEsperado(";"); // Cada 'return' debe terminar con un ';'
        return new NodoExpresion("return")
        {
            Derecha = expresion
        };
    }

    private NodoExpresion ParsearIfStatement()
    {
        VerificarTokenEsperado("(");
        NodoExpresion condicion = ParsearExpresion();
        VerificarTokenEsperado(")");

        VerificarTokenEsperado("{");

        NodoExpresion bloqueIf = new NodoExpresion("if")
        {
            Izquierda = condicion,
            Derecha = ParsearBloque()
        };

        Token tokenSiguiente = SiguienteToken();
        if (tokenSiguiente != null && tokenSiguiente.Value == "else")
        {
            VerificarTokenEsperado("{");
            NodoExpresion bloqueElse = new NodoExpresion("else")
            {
                Derecha = ParsearBloque()
            };
            bloqueIf.Sentencias.Add(bloqueElse);
        }
        else
        {
            Retroceder();
        }

        return bloqueIf;
    }

    private NodoExpresion ParsearWhileStatement()
    {
        VerificarTokenEsperado("(");
        NodoExpresion condicion = ParsearExpresion();
        VerificarTokenEsperado(")");
        VerificarTokenEsperado("{");

        NodoExpresion bloqueWhile = new NodoExpresion("while")
        {
            Izquierda = condicion,
            Derecha = ParsearBloque()
        };

        return bloqueWhile;
    }

   private NodoExpresion ParsearForStatement()
{
    // Verifica el paréntesis de apertura '('
    VerificarTokenEsperado("(");

    // 1. Parsear la inicialización (puede ser una declaración o asignación)
    NodoExpresion inicializacion = ParsearInicializacionFor();
    VerificarTokenEsperado(";"); // Final de la inicialización

    // 2. Parsear la condición del ciclo for
    NodoExpresion condicion = ParsearExpresion();
    VerificarTokenEsperado(";"); // Final de la condición

    // 3. Parsear el incremento del ciclo for
    NodoExpresion incremento = ParsearExpresion();
    VerificarTokenEsperado(")"); // Paréntesis de cierre ')'

    // 4. Verificar que se abra el bloque de código '{'
    VerificarTokenEsperado("{");

    // 5. Parsear el bloque de código dentro del ciclo for
    NodoExpresion bloqueFor = ParsearBloque(); // El bloque de código va aquí

    // Crear el nodo del ciclo for con la estructura correcta
    NodoExpresion nodoFor = new NodoExpresion("for")
    {
        Izquierda = condicion, // La condición del ciclo va como hijo izquierdo
        Derecha = bloqueFor // El bloque de código va como hijo derecho
    };

    // Añadir la inicialización como la primera sentencia del ciclo
    nodoFor.Sentencias.Insert(0, inicializacion);

    // Añadir el incremento como la última sentencia del ciclo
    nodoFor.Sentencias.Add(incremento);

    return nodoFor;
}

// Método especializado para la inicialización del ciclo 'for'
private NodoExpresion ParsearInicializacionFor()
{
    Token tokenActual = SiguienteToken();

    // Caso de declaración (por ejemplo: int i = 0)
    if (tokenActual.Value == "int")
    {
        // Parsear la declaración completa
        NodoExpresion declaracion = new NodoExpresion("int");
        Token identificador = SiguienteToken();
        if (identificador.Type != TokenType.Identifier)
        {
            throw new Exception("Error de sintaxis: Se esperaba un identificador después de 'int'.");
        }
        declaracion.Izquierda = new NodoExpresion(identificador.Value);

        // Verificar si hay una asignación (por ejemplo: i = 0)
        Token siguiente = SiguienteToken();
        if (siguiente.Value == "=")
        {
            declaracion.Derecha = ParsearExpresion(); // Parsear la expresión después del '='
        }

        return declaracion;
    }

    // Caso de asignación o expresión simple (por ejemplo: i = 0)
    Retroceder(); // Retrocedemos para procesarlo como una expresión estándar
    return ParsearExpresion();
}





    private NodoExpresion ParsearBloque()
    {
        Sentencias sentenciasBloque = new Sentencias();

        while (true)
        {
            Token siguienteToken = SiguienteToken();
            if (siguienteToken != null && siguienteToken.Value == "}")
                break;

            Retroceder();
            NodoExpresion sentencia = ParsearSentencia();
            sentenciasBloque.AgregarSentencia(sentencia);
        }

        return new NodoExpresion("bloque")
        {
            Sentencias = sentenciasBloque.ListaSentencias
        };
    }

    private NodoExpresion ParsearAsignacion(Token tokenIdentificador)
    {
        Token siguienteToken = SiguienteToken();

        // Si el token siguiente es '=', es una asignación
        if (siguienteToken.Value == "=")
        {
            NodoExpresion nodoIdentificador = new NodoExpresion(tokenIdentificador.Value);
            NodoExpresion expresion = ParsearExpresion();
            VerificarTokenEsperado(";"); // Terminar con ';'
            return new NodoExpresion("=")
            {
                Izquierda = nodoIdentificador,
                Derecha = expresion
            };
        }

        // Si no es una asignación, retrocedemos y procesamos como una expresión regular
        Retroceder();
        return ParsearExpresion();
    }

    private NodoExpresion ParsearExpresion()
    {
        NodoExpresion izquierda = ParsearTermino();

        while (true)
        {
            Token siguienteToken = SiguienteToken();
            if (siguienteToken == null || !EsOperador(siguienteToken.Value))
            {
                Retroceder();
                break;
            }

            NodoExpresion derecha = ParsearTermino();
            izquierda = new NodoExpresion(siguienteToken.Value)
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
        }

        return izquierda;
    }

private NodoExpresion ParsearTermino()
{
    Token tokenActual = SiguienteToken();

    // Caso de un número o un identificador
    if (tokenActual.Type == TokenType.Number || tokenActual.Type == TokenType.Identifier)
    {
        NodoExpresion nodo = new NodoExpresion(tokenActual.Value);

        // Verificar si el siguiente token es '++' o '--'
        Token tokenSiguiente = SiguienteToken();
        if (tokenSiguiente != null && (tokenSiguiente.Value == "++" || tokenSiguiente.Value == "--"))
        {
            // Manejar como un operador unario
            return new NodoExpresion(tokenSiguiente.Value)
            {
                Izquierda = nodo // El operador unario actúa sobre el identificador
            };
        }

        // Si no hay '++' o '--', retroceder
        Retroceder();
        return nodo;
    }

    // Caso de una expresión entre paréntesis
    if (tokenActual.Value == "(")
    {
        NodoExpresion expresion = ParsearExpresion();
        VerificarTokenEsperado(")");
        return expresion;
    }

    throw new Exception($"Error: Se esperaba un número, un identificador o una expresión entre paréntesis, pero se encontró '{tokenActual.Value}'.");
}

    private void VerificarTokenEsperado(string valorEsperado)
    {
        Token tokenActual = SiguienteToken();
        if (tokenActual == null || tokenActual.Value != valorEsperado)
        {
            throw new Exception($"Error de sintaxis: Se esperaba '{valorEsperado}' pero se encontró '{tokenActual?.Value}'");
        }
    }
}
