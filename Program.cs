using System;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Linq;
using System.Collections.Generic;

namespace CheckTsql
{
    struct SqlError
    {
        public int Line { get; set; }
        public string Message { get; set; }

        public string Display() => $"{Line}\t{Message}";
    }
    class Program
    {

        static SqlError[] ParseFile(string fname)
        {
            var textReader = new StreamReader(fname);
            var parser = new TSql130Parser(true);
            var sql = parser.Parse(textReader, out var rawErrors);
            var errors = rawErrors.Select(err =>
                new SqlError
                {
                    Line = err.Line,
                    Message = err.Message
                }
            ).ToArray();
            return errors;
        }
        static void Main(string[] args)
        {
            var allFiles = new List<string>();

            foreach (var arg in args) {
                if (File.Exists(arg)) {
                    allFiles.Add(arg);
                    continue;
                }
                if (Directory.Exists(arg)) {
                    allFiles.AddRange(new DirectoryInfo(arg).GetFiles("*.sql", SearchOption.AllDirectories).Select(it => it.FullName));
                    continue;
                }
                Console.WriteLine($"Not file or directory: {arg}");
            }
            foreach (var file in allFiles) {
                if (!File.Exists(file)) {
                    Console.WriteLine($"ERR NOTEXIST: {file}");
                    continue;
                }
                var errors = ParseFile(file);
                if (!errors.Any()) {
                    Console.WriteLine($"OK {file}");

                } else {
                    Console.WriteLine($"ERR {file}");

                    foreach (var err in errors) {
                        Console.WriteLine($"    {err.Line} {err.Message}");
                    }
                }
            }
        }
    }
}
