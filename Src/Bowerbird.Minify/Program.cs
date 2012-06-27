using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace Bowerbird.Minify
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = @"C:\Projects\bowerbird.web\Src\Bowerbird.Website\js\main-combined.js";
            string output = @"C:\Projects\bowerbird.web\Src\Bowerbird.Website\js\main-minified.js";

            if (args.Count() > 0)
            {
                ParseArguments(args, out input, out output);
            }

            string jsText;

            using(var jsFile = File.OpenText(input))
            {
                jsText = jsFile.ReadToEnd();
            }

            Generate(new Minifier(), jsText, output);
        }

        private static void Generate(Minifier minifier, string jsText, string outputPath)
        {
            var minifiedText = minifier.MinifyJavaScript(jsText);

            File.WriteAllText(outputPath, minifiedText);
        }

        private static void ParseArguments(string[] args, out string inputPath, out string outputPath)
        {
            inputPath = null;
            outputPath = null;

            foreach (var a in args)
            {
                if (!a.StartsWith("/"))
                {
                    continue;
                }

                KeyValuePair<string, string> arg = ParseArg(a);
                switch (arg.Key)
                {
                    case "in":
                        inputPath = arg.Value;
                        break;
                    case "out":
                        outputPath = arg.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        private static KeyValuePair<string, string> ParseArg(string arg)
        {
            arg = arg.Substring(1);
            if (arg.Contains(":"))
            {
                var splitIndex = arg.IndexOf(':');
                var key = arg.Substring(0, splitIndex).Trim();
                var value = arg.Substring(splitIndex + 1).Trim();
                return new KeyValuePair<string, string>(key, value);
            }

            return new KeyValuePair<string, string>(arg.Trim(), null);
        }
    }
}