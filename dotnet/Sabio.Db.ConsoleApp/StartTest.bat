SET IsWatcherDisabled=false
:: dotnet test bin\Debug\net6.0\Sabio.Db.ConsoleApp.Tests.dll -l "trx;verbosity=detailed" -s "./default.runsettings" --filter "FullyQualifiedName~Friends | FullyQualifiedName~Users | FullyQualifiedName~Addresses"
TestExecutables\Sabio.Db.ConsoleApp.TestResults.exe -a \\bin\\Debug\\net6.0 -d \\TestResults -t \\lib -f "FullyQualifiedName~Friends|FullyQualifiedName~Users|FullyQualifiedName~Addresses"
