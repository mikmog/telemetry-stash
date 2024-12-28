############################################################################################################
#	Generate root certificate 
#   
#   Prerequisites:
#   - OpenSSL https://kb.firedaemon.com/support/solutions/articles/4000121705#Download-OpenSSL
#   - Azure CLI https://learn.microsoft.com/en-gb/cli/azure/install-azure-cli
#   - Azure Key Vault with secret
#       - Certificate--RootCaPassword
#
#	Usage:
#	./generate-root-cert.ps1 -env dev
#	./generate-root-cert.ps1 -env prod
#
#	Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
#
############################################################################################################
param(
	[string]$env = "dev"
)

$ErrorActionPreference = "Stop"

$root = "C:/IoTCerts/$env/rootca"
$keyVaultName = "kv-ts-$env"
$rootCaName = "rootca-$env"

Write-Output "
 ______      __                 __                ____ __              __ 
/_  __/___  / /___  __ _  ___  / /_ ____ __ __   / __// /_ ___ _ ___  / / 
 / /  / -_)/ // -_)/  ' \/ -_)/ __// __// // /  _\ \ / __// _ `/(_-< / _ \
/_/   \__//_/ \__//_/_/_/\__/ \__//_/   \_, /  /___/ \__/ \_,_//___//_//_/
                                       /___/                              
"

Write-Output "Generating root ca '$rootCaName' with secrets from '$keyVaultName'"

$rootCaPassword = Get-AzKeyVaultSecret -VaultName $keyVaultName -Name "Certificate--RootCaPassword" -AsPlainText

Write-Output "Creating directory structure $root"
New-Item -ItemType Directory -Path "$root/certs" -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path "$root/db" -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path "$root/private" -ErrorAction SilentlyContinue

"" | Out-File -encoding ascii -NoNewline "$root/db/index"
openssl rand -hex 16 | Out-File -encoding ascii "$root/db/serial"
"1001" | Out-File -encoding ascii "$root/db/crlnumber"

openssl req -new -config "$rootCaName.conf" -out "$root/$rootCaName.csr" -keyout "$root/private/$rootCaName.key" -passout "pass:$rootCaPassword"
openssl ca -batch -selfsign -config "$rootCaName.conf" -in "$root/$rootCaName.csr" -out "$root/$rootCaName.crt" -extensions ca_ext -passin "pass:$rootCaPassword"
openssl x509 -in "$root/$rootCaName.crt" -out "$root/$rootCaName.pem" -outform PEM

$fileName = "$root/$rootCaName-secrets.txt"
"[" + (Get-Date).ToUniversalTime().ToString("o") + "] Env: $env"	| Out-File -encoding ascii -Append $fileName
"[" + (Get-Date).ToUniversalTime().ToString("o") + "] Root CA password: $rootCaPassword" | Out-File -encoding ascii -Append $fileName
"" | Out-File -encoding ascii -Append $fileName

Write-Output "Done"