namespace WebGitNet
{
    using System.IO;
    using System.Reflection;
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;

    public static class PygmentsSyntaxHilighter
    {
        private static readonly ScriptEngine engine;
        private static readonly ScriptScope scope;
        private static readonly string hostingScript;

        static PygmentsSyntaxHilighter()
        {
            engine = Python.CreateEngine();
            hostingScript = LoadHostingScript();
            scope = GetNewScope();
        }

        public static string HilightFile(string filename, string contents)
        {
            return scope.GetVariable("hilight_file")(filename, contents);
        }

        public static string HilightText(string text)
        {
            return scope.GetVariable("hilight_text")(text);
        }

        private static string LoadHostingScript()
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebGitNet.PygmentsHosting.py"))
            {
                using (var reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static ScriptScope GetNewScope()
        {
            var scope = engine.CreateScope();
            engine.Execute(hostingScript, scope);
            return scope;
        }
    }
}