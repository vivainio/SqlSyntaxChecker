using System;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Linq;

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
            foreach (var arg in args) {
                var errors = ParseFile(arg);
                if (!errors.Any()) {
                    Console.WriteLine($"OK ${arg}");

                } else {
                    Console.WriteLine($"ERR {arg}");

                    foreach (var err in errors) {
                        Console.WriteLine($" {err.Line} {err.Message}");
                    }
                }
            }
            ParseFile(args[0]);
        }
    }
}
