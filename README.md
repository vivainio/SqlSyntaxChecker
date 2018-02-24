# SqlSyntaxChecker

Checks Transact-SQL syntax (i.e. mssql variant of SQL)

Based on TransactSql.ScriptDOM TSQL parser.

Example output when run against test data in repo:

```
λ  dotnet run .\testdata\ok.sql .\testdata\errors.sql
OK $.\testdata\ok.sql
ERR .\testdata\errors.sql
 3 Incorrect syntax near oeutoeuoeu.
 5 Incorrect syntax near oethulroeethulrtoetu.
 ```
