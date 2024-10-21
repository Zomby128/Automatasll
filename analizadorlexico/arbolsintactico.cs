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
        VerificarTokenEsperado("(");
        NodoExpresion inicializacion = ParsearExpresion();
        VerificarTokenEsperado(";");
        NodoExpresion condicion = ParsearExpresion();
        VerificarTokenEsperado(";");
        NodoExpresion incremento = ParsearExpresion();
        VerificarTokenEsperado(")");
        VerificarTokenEsperado("{");

        NodoExpresion bloqueFor = new NodoExpresion("for")
        {
            Izquierda = condicion,
            Derecha = ParsearBloque()
        };

        return bloqueFor;
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
        Token siguienteToken = SiguienteToken();

        if (siguienteToken.Type == TokenType.Number || siguienteToken.Type == TokenType.Identifier)
        {
            return new NodoExpresion(siguienteToken.Value);
        }

        if (siguienteToken.Value == "(")
        {
            NodoExpresion expresion = ParsearExpresion();
            VerificarTokenEsperado(")");
            return expresion;
        }

        throw new Exception($"Error: Se esperaba un número, un identificador o una expresión entre paréntesis, pero se encontró '{siguienteToken.Value}'.");
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
