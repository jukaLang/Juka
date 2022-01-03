﻿using JukaCompiler.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Interpreter
{
    internal class JukaFunction : Callable
    {
        private Stmt.Function declaration;
        private JukaEnvironment closure;
        private bool isInitializer;

        internal JukaFunction(Stmt.Function declaration, JukaEnvironment closure, bool isInitializer)
        {
            this.isInitializer = isInitializer;
            this.declaration = declaration;
            this.closure = closure;
        }

        internal JukaFunction Bind(JukaInstance instance)
        {
            JukaEnvironment env = new();
            env.Define("this", instance);

            return new JukaFunction(declaration, env, isInitializer);
        }

        public int Arity()
        {
            return declaration.Params.Count;
        }

        public object Call(JukaInterpreter interpreter, List<object> arguments)
        {
            JukaEnvironment environment = new(closure);

            for (int i = 0; i < declaration.Params.Count; i++)
            {
                string name = declaration.Params[i].parameterName.ToString();
                object value = arguments[i];
                environment.Define(name, value);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch(Exception ex)
            {
                if (isInitializer)
                {
                    return closure.GetAt(0, "this");
                }

                return ex;
            }

            if (isInitializer)
            {
                return closure.GetAt(0, "this");
            }

            return null;
        }
    }
}