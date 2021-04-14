**To Run tests you must:**
1. place your Firebase key json file in folder `FireSource.Test\Resources`.
1. update the path in `<Content Include="Resources\dummydemo-11dd3-firebase-adminsdk-vp6c5-28b7f0fa93.json">` of `FireSource.Test.csproj`
1. update the `FireStoreFixture.cs` of `FireSource.Test` project with:
   * correct path to Firebase key json file in constant `FIREBASE_CREDENTIALS_PATH`
   * project id in constant `FIREBASE_PROJECT_ID`

Run tests with `dotnet test --logger "console;verbosity=detailed"` to see `Console` output.

List tests with `dotnet test -t`

Select tests to run with `dotnet test --filter NameOfTheClassTest`


Run coverage with:
```
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:Project.Tests\TestResults\66e8839d-6844-4b8a-8067-dc9c32abed5d\coverage.cobertura.xml  -targetdir:coverage
coverage\index.htm
```"# ave-2021-firemapper-i41n-g02-ave" 
