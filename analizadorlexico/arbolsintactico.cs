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

    // Precedencia de operadores
    private int Precedencia(string operador)
    {
        return operador switch
        {
            "^" => 4,
            "*" or "/" or "%" => 3,
            "+" or "-" => 2,
            "<" or ">" or "<=" or ">=" or "==" or "!=" => 1,
            _ => 0
        };
    }

    private bool EsOperador(string valor) => Precedencia(valor) > 0;

    private Token SiguienteToken() => posicion < tokens.Count ? tokens[posicion++] : null;

    private void Retroceder()
    {
        if (posicion > 0) posicion--;
    }

    // Método principal para parsear sentencias
    public NodoExpresion ParsearSentencia()
    {
        Token tokenActual = SiguienteToken();
        if (tokenActual == null) throw new Exception("Error: Código fuente vacío.");

        return tokenActual.Type switch
        {
            TokenType.Keyword when tokenActual.Value == "while" => ParsearWhileStatement(),
            TokenType.Keyword when tokenActual.Value == "for" => ParsearForStatement(),
            TokenType.Keyword when tokenActual.Value == "if" => ParsearIfStatement(),
            TokenType.Identifier => ParsearAsignacion(tokenActual),
            _ => LanzarError($"Sentencia inesperada '{tokenActual.Value}'", tokenActual)
        };
    }

    // Parseo de sentencias 'while'
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

    // Parseo de sentencias 'for'
    private NodoExpresion ParsearForStatement()
    {
        VerificarTokenEsperado("(");

        NodoExpresion inicializacion = null;
        Token tokenInicial = SiguienteToken();
        if (tokenInicial.Type == TokenType.Identifier)
        {
            Retroceder();
            inicializacion = ParsearAsignacion(SiguienteToken());
        }
        VerificarTokenEsperado(";");

        NodoExpresion condicion = ParsearExpresion();
        VerificarTokenEsperado(";");

        NodoExpresion incremento = null;
        Token tokenIncremento = SiguienteToken();
        if (tokenIncremento.Type == TokenType.Identifier)
        {
            Retroceder();
            incremento = ParsearExpresion();
        }
        VerificarTokenEsperado(")");

        VerificarTokenEsperado("{");
        NodoExpresion bloqueFor = ParsearBloque();

        return new NodoExpresion("for")
        {
            Sentencias = new List<NodoExpresion> { inicializacion, incremento },
            Izquierda = condicion,
            Derecha = bloqueFor
        };
    }

    // Parseo de sentencias 'if'
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
            bloqueIf.Sentencias ??= new List<NodoExpresion>();
            bloqueIf.Sentencias.Add(bloqueElse);
        }
        else
        {
            Retroceder();
        }

        return bloqueIf;
    }

    // Parseo de asignación
    private NodoExpresion ParsearAsignacion(Token tokenIdentificador)
    {
        Token siguienteToken = SiguienteToken();
        if (siguienteToken?.Value != "=")
        {
            LanzarError($"Se esperaba '=' después de '{tokenIdentificador.Value}'", siguienteToken);
        }

        NodoExpresion expresion = ParsearExpresion();
        VerificarTokenEsperado(";");
        return new NodoExpresion("=")
        {
            Izquierda = new NodoExpresion(tokenIdentificador.Value),
            Derecha = expresion
        };
    }

    // Parseo de bloques de código
    private NodoExpresion ParsearBloque()
    {
        List<NodoExpresion> sentencias = new List<NodoExpresion>();
        while (true)
        {
            Token tokenActual = SiguienteToken();
            if (tokenActual != null && tokenActual.Type == TokenType.BraceClose)
            {
                break;
            }
            Retroceder();
            sentencias.Add(ParsearSentencia());
        }

        return new NodoExpresion("bloque") { Sentencias = sentencias };
    }

    // Parseo de expresiones
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

    // Parseo de términos
    private NodoExpresion ParsearTermino()
    {
        Token tokenActual = SiguienteToken();

        if (tokenActual.Type == TokenType.Number || tokenActual.Type == TokenType.Identifier)
        {
            return new NodoExpresion(tokenActual.Value);
        }

        if (tokenActual.Value == "(")
        {
            NodoExpresion expresion = ParsearExpresion();
            VerificarTokenEsperado(")");
            return expresion;
        }

        LanzarError("Se esperaba un número o identificador", tokenActual);
        return null;
    }

    // Verifica y lanza un error si no coincide el token
    private void VerificarTokenEsperado(string valorEsperado)
    {
        Token tokenActual = SiguienteToken();
        if (tokenActual == null || tokenActual.Value != valorEsperado)
        {
            LanzarError($"Se esperaba '{valorEsperado}'", tokenActual);
        }
    }

    private NodoExpresion LanzarError(string mensaje, Token token)
    {
        string ubicacion = token != null
            ? $"en la línea {token.Linea}, columna {token.Columna}"
            : "al final del archivo";
        throw new Exception($"Error de sintaxis: {mensaje} {ubicacion}.");
    }
}
