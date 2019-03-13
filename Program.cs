using System;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using static System.Console;
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
        private static TSql130Parser parser = new TSql130Parser(true);



        static SqlError[] ParseString(string text)
        {

            var rd = new StringReader(text);
            parser.Parse(rd, out var rawErrors);
            return rawErrors.Select(err =>
                new SqlError
                {
                    Line = err.Line,
                    Message = err.Message
                }
            ).ToArray();

        }
        static SqlError[] ParseFile(string fname)
        {
            var textReader = new StreamReader(fname);

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

        static void ParseCsvFile(string fname)
        {
            var p = new CsvParser(new StreamReader(fname));
            var good = new List<string>();

            var badFileName = $"{fname}.bad.csv";
            var writer = new StreamWriter(File.Create(badFileName), Encoding.UTF8);

            int linenum = 0;
            while (true)
            {
                var e = p.Read();
                linenum++;
                if (e == null)
                {
                    break;
                }
                var l = e[0];
                var errors = ParseString(l);
                if (errors.Length == 0)
                {
                    good.Add(e[0]);
                }
                else
                {
                    writer.Write(linenum);
                    writer.Write(": ");

                    writer.WriteLine(l);
                }
            }
            writer.Flush();

        }
        static void Main(string[] args)
        {
            var allFiles = new List<string>();
            foreach (var arg in args) {
                if (arg.EndsWith(".csv"))
                {
                    ParseCsvFile(arg);
                    continue;

                }
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
