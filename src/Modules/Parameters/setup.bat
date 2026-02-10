@echo off
echo ========================================
echo   Parameters Module - Setup Completo
echo ========================================
echo.

echo [1/4] Restaurando pacotes NuGet...
dotnet restore Parameters.sln
if %errorlevel% neq 0 goto error

echo.
echo [2/4] Compilando solucao...
dotnet build Parameters.sln --configuration Release
if %errorlevel% neq 0 goto error

echo.
echo [3/4] Criando migration inicial...
cd Parameters.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Parameters.API --context ParametersDbContext
if %errorlevel% neq 0 (
    echo WARNING: Migration ja existe ou erro ao criar. Continuando...
)
cd ..

echo.
echo [4/4] Aplicando migration ao banco...
cd Parameters.Infrastructure
dotnet ef database update --startup-project ../Parameters.API --context ParametersDbContext
if %errorlevel% neq 0 (
    echo WARNING: Verifique a connection string em appsettings.json
    echo Voce pode executar a migration manualmente depois.
)
cd ..

echo.
echo ========================================
echo   Setup concluido com sucesso!
echo ========================================
echo.
echo Para executar a API:
echo   cd Parameters.API
echo   dotnet run
echo.
echo Swagger estara disponivel em: http://localhost:5000
echo.
pause
goto end

:error
echo.
echo ========================================
echo   ERRO durante o setup!
echo ========================================
echo.
pause
exit /b 1

:end
