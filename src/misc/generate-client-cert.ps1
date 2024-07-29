############################################################################################################
#	Generate device certificate and upload to Azure Key Vault
#   
#   Prerequisites:
#   - A root CA certificate
#   - OpenSSL https://kb.firedaemon.com/support/solutions/articles/4000121705#Download-OpenSSL
#   - Azure CLI 
#		Install-Module -Name Az -Force -AllowClobber -Scope CurrentUser
#   - Azure Key Vault with secrets
#       - Client--CertificatePassword
#       - Certificate--RootCaPassword
#
#	Usage:
#	./generate-client-cert.ps1 -deviceId Client-Dev1 -env dev
#	./generate-client-cert.ps1 -deviceId Client-Prod1 -env prod
#
#	Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
#
############################################################################################################
param(
	[string]$env = "dev",
	[string]$deviceId
)

$ErrorActionPreference = "Stop"

function Get-Password([int] $length) {
	return -join ((65..90) + (97..122) + (48..57) + ('!','@','#','$','%','^','&','*','?',';','+','|','[',']','{','}','-','_','£',':','$','<','>','/','§','½') | Get-Random -Count $length | ForEach-Object {[char]$_})
}

$root = "C:/IoTCerts/$env"
$deviceRoot = "$root/devices/$deviceId"
$keyVaultName = "kv-ts-$env"

Write-Output "
 ______      __                 __                ____ __              __ 
/_  __/___  / /___  __ _  ___  / /_ ____ __ __   / __// /_ ___ _ ___  / / 
 / /  / -_)/ // -_)/  ' \/ -_)/ __// __// // /  _\ \ / __// _ `/(_-< / _ \
/_/   \__//_/ \__//_/_/_/\__/ \__//_/   \_, /  /___/ \__/ \_,_//___//_//_/
                                       /___/                              
"

Write-Output "Generating '$deviceId' certificate with secrets from '$keyVaultName'"

$privateKeyPassword = Get-AzKeyVaultSecret -VaultName $keyVaultName -Name "Client--CertificatePassword" -AsPlainText
$pkcs12Password = Get-Password(20)
$rootCaPassword = Get-AzKeyVaultSecret -VaultName $keyVaultName -Name "Certificate--RootCaPassword" -AsPlainText

Write-Output "Creating device directory $deviceRoot"
New-Item -ItemType Directory -Path "$deviceRoot" -ErrorAction SilentlyContinue

# Create device certificate
openssl rand -hex 16 | Out-File -encoding ascii "$root/rootca/db/serial"
openssl genpkey -out "$deviceRoot/$deviceId.key" -algorithm RSA -pkeyopt rsa_keygen_bits:2048
openssl req -new -key "$deviceRoot/$deviceId.key" -out "$deviceRoot/$deviceId.csr" -subj "/CN=$deviceId" -passin pass:$privateKeyPassword
openssl ca -batch -config rootca-$env.conf -in "$deviceRoot/$deviceId.csr" -out "$deviceRoot/$deviceId.crt" -extensions client_ext -passin pass:$rootCaPassword
openssl x509 -in "$deviceRoot/$deviceId.crt" -out "$deviceRoot/$deviceId.pem" -outform PEM
openssl pkcs12 -export -in "$deviceRoot/$deviceId.crt" -inkey "$deviceRoot/$deviceId.key" -out "$deviceRoot/$deviceId.pfx" -passout pass:$pkcs12Password

# Merge the device certificate and private key into a single file to make it compatible with Azure Key Vault
Get-Content "$deviceRoot/$deviceId.pem", "$deviceRoot/$deviceId.key" | Set-Content "$deviceRoot/$deviceId.key.pem" -encoding ascii

Write-Output "Import into Azure Key Vault"
$Password = ConvertTo-SecureString -String "$privateKeyPassword" -AsPlainText -Force
Import-AzKeyVaultCertificate -VaultName "$keyVaultName" -Name "$deviceId" -FilePath "$deviceRoot/$deviceId.key.pem" -Password $Password

$fileName = "$deviceRoot/$deviceId-secrets.txt"
"[" + (Get-Date).ToUniversalTime().ToString("o") + "] DeviceId: $deviceId" | Out-File -encoding ascii -Append $fileName
"[" + (Get-Date).ToUniversalTime().ToString("o") + "] PrivateKeyPassword: $privateKeyPassword" | Out-File -encoding ascii -Append $fileName
"[" + (Get-Date).ToUniversalTime().ToString("o") + "] Pkcs12Password: $pkcs12Password" | Out-File -encoding ascii -Append $fileName
"" | Out-File -encoding ascii -Append $fileName

Write-Output "Done"