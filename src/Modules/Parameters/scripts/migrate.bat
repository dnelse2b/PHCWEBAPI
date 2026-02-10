@echo off
REM Script para criar migration e atualizar banco de dados no Windows

echo ======================================================
echo Parameters Module - Database Migration Script
echo ======================================================
echo.

cd Parameters.Infrastructure

echo Criando nova migration...
set /p MIGRATION_NAME="Nome da migration: "

if "%MIGRATION_NAME%"=="" (
    set MIGRATION_NAME=Migration_%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
)

dotnet ef migrations add %MIGRATION_NAME% --startup-project ../Parameters.API --context ParametersDbContext

if %errorlevel% equ 0 (
    echo.
    echo Migration criada com sucesso!
    echo.
    set /p APPLY_MIGRATION="Aplicar migration ao banco de dados? (y/n): "
    
    if /i "%APPLY_MIGRATION%"=="y" (
        echo Aplicando migration ao banco...
        dotnet ef database update --startup-project ../Parameters.API --context ParametersDbContext
        
        if %errorlevel% equ 0 (
            echo Database atualizado com sucesso!
        ) else (
            echo Erro ao aplicar migration
            exit /b 1
        )
    )
) else (
    echo Erro ao criar migration
    exit /b 1
)

echo.
echo Processo concluido!
pause
