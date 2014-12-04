using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Mono.Terminal;


namespace Xorcerer.Wizard.Utilities
{
    public class PythonHelper
    {
        ScriptScope _pyScope;
        ScriptEngine _pyEngine;

        const string ScriptExtension = ".py";

        public PythonHelper(IEnumerable<Assembly> referencedAssemblies = null)
        {
            _pyEngine = Python.CreateEngine();

            ReferenceAssembly(GetType().Assembly);
            if (referencedAssemblies != null)
                foreach (var a in referencedAssemblies)
                    ReferenceAssembly(a);

            _pyScope = _pyEngine.CreateScope();
        }

        public static ScriptRuntime CreateAndSetupRuntime()
        {
            var ipy = Python.CreateRuntime();
            ipy.LoadAssembly(Assembly.Load("Camp.Barbarian"));
            ipy.LoadAssembly(Assembly.Load("Camp.Giant"));
            ipy.ExecuteFile("pyscripts/__init__.py");
            return ipy;
        }

        public void ReferenceAssembly(Assembly assembly)
        {
            _pyEngine.Runtime.LoadAssembly(assembly);
        }

        public void SetVariable(String name, object value)
        {
            _pyScope.SetVariable(name, value);
        }

        public void RunRepl()
        {
            Console.WriteLine("Interactive C# REPL, type ':h' for help, or type C# statement directly.");

            LineEditor editor = new LineEditor(GetType().Name);
            String line;
            while ((line = editor.Edit("> ", "")) != null)
            {
                if (line.Trim() == ":exit")
                    break;

                EvalCommandOrStatement(line);
            }
        }

        public void EvalCommandOrStatement(String input)
        {
            if (input.StartsWith(":"))
            {
                try
                {
                    EvalCommand(input.Substring(1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                try
                {
                    CompileSourceAndExecute(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        private void CompileSourceAndExecute(String code)
        {
            ScriptSource source = _pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile();
            compiled.Execute(_pyScope);
        }

        public void Help()
        {
            // TODO: auto generate list by an Attribute.
            Console.WriteLine("Avaliable commands:");
            Console.WriteLine("  exit(or Ctrl-D)");
            Console.WriteLine("  h, help");
            Console.WriteLine();
        }

        void EvalCommand(String str)
        {
            String[] parts = str.Trim().Split(' ');
            String cmd = parts[0];
            // String[] args = parts.Skip(1).ToArray();

            switch (cmd)
            {
                case "h":
                case "help":
                    Help();
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}

