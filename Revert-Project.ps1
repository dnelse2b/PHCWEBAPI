# =========================================
# Script de Reversão: PHCAPI → SGOFAPI
# =========================================
# Use este script para reverter a renomeação se necessário

param(
    [string]$OldName = "PHCAPI",
    [string]$NewName = "SGOFAPI"
)

Write-Host "=========================================" -ForegroundColor Yellow
Write-Host "  ⚠️  REVERSÃO DE RENOMEAÇÃO" -ForegroundColor Yellow
Write-Host "  $OldName → $NewName" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Yellow
Write-Host ""

$confirmation = Read-Host "Deseja realmente reverter? (S/N)"

if ($confirmation -ne 'S' -and $confirmation -ne 's') {
    Write-Host "❌ Operação cancelada." -ForegroundColor Red
    exit
}

# Executar o script principal com os nomes invertidos
.\Rename-Project.ps1 -OldName $OldName -NewName $NewName -CreateBackup $true

Write-Host ""
Write-Host "✅ Reversão concluída!" -ForegroundColor Green
