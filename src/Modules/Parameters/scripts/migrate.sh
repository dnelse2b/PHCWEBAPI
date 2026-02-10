#!/bin/bash

# Script para criar migration e atualizar banco de dados

echo "🚀 Parameters Module - Database Migration Script"
echo "================================================"

# Navegar para o diretório Infrastructure
cd Parameters.Infrastructure

echo ""
echo "📦 Criando nova migration..."
read -p "Nome da migration: " MIGRATION_NAME

if [ -z "$MIGRATION_NAME" ]; then
    MIGRATION_NAME="Migration_$(date +%Y%m%d_%H%M%S)"
fi

dotnet ef migrations add $MIGRATION_NAME \
    --startup-project ../Parameters.API \
    --context ParametersDbContext

if [ $? -eq 0 ]; then
    echo "✅ Migration criada com sucesso!"
    echo ""
    read -p "🔄 Aplicar migration ao banco de dados? (y/n): " APPLY_MIGRATION
    
    if [ "$APPLY_MIGRATION" = "y" ] || [ "$APPLY_MIGRATION" = "Y" ]; then
        echo "📊 Aplicando migration ao banco..."
        dotnet ef database update \
            --startup-project ../Parameters.API \
            --context ParametersDbContext
        
        if [ $? -eq 0 ]; then
            echo "✅ Database atualizado com sucesso!"
        else
            echo "❌ Erro ao aplicar migration"
            exit 1
        fi
    fi
else
    echo "❌ Erro ao criar migration"
    exit 1
fi

echo ""
echo "🎉 Processo concluído!"
