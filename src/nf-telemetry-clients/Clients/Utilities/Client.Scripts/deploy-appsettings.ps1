############################################################################################################
#	Script is used to deploy the configuration to the device. It reads the AppSettings.env.json file and
#	replaces secrets and certificates with values from Key Vault.
#
#   Prerequisites:
#   - A root CA certificate
#   - OpenSSL https://kb.firedaemon.com/support/solutions/articles/4000121705#Download-OpenSSL
#   - Azure CLI https://learn.microsoft.com/en-gb/cli/azure/install-azure-cli
#   - Azure Key Vault with secrets
#       - Client--CertificatePassword
#   - Azure Key Vault with client certificates
#       - {CERTIFICATE_NAME}
#
#	Usage:
#	./deploy-appsettings.ps1 -settingsFile AppSettings.{IDENTIFIER}.Dev.json -keyVaultName kv-ts-develop -serialPort {COM_PORT}
#	./deploy-appsettings.ps1 -settingsFile AppSettings.{IDENTIFIER}.Prod.json -keyVaultName kv-ts-prod -serialPort {COM_PORT}
#
#	E.g.
#	./deploy-appsettings.ps1 -nfClient RipTide.Nfirmware -settingsFile AppSettings.WROOM.Dev.json -keyVaultName kv-ts-develop -serialPort COM5
#	./deploy-appsettings.ps1 -nfClient AtticMonitor.NfClient -settingsFile AppSettings.WROVER.Dev.json -keyVaultName kv-ts-develop -serialPort COM8
#	./deploy-appsettings.ps1 -nfClient SmartPowerMeter.NfClient -settingsFile AppSettings.XIAOC3.Dev.json -keyVaultName kv-ts-develop -serialPort COM8
#
#	Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
#
############################################################################################################
param(
	[string]$keyVaultName = "",
	[string]$settingsFile = "",
	[string]$nfClient = "",
	[string]$serialPort = "",
	[string]$deployJsonFile = "deploy.json"
)

$ErrorActionPreference = "Stop"

Write-Output "
 ______      __                 __                ____ __              __ 
/_  __/___  / /___  __ _  ___  / /_ ____ __ __   / __// /_ ___ _ ___  / / 
 / /  / -_)/ // -_)/  ' \/ -_)/ __// __// // /  _\ \ / __// _ `/(_-< / _ \
/_/   \__//_/ \__//_/_/_/\__/ \__//_/   \_, /  /___/ \__/ \_,_//___//_//_/
                                       /___/                              
"

Write-Output "Deploying '$nfClient' '$settingsFile' with secrets from '$keyVaultName'"

$settingsFilePath = "../../$nfClient/$settingsFile"

Write-Output "Reading settings file '$settingsFilePath'"

$tempFiles = @()

# Read settings file as hash table
$hashTable = Get-Content -Path $settingsFilePath | ConvertFrom-Json -AsHashtable

# Replace <secret> from Key Vault
ForEach ($key in @($hashTable.keys)) {
	if($hashTable[$key] -eq "<secret>") {
		$secretKey = ($key -replace "\.", "--" )
		Write-Output "Reading Key Vault secret '$secretKey'"
		$secret = az keyvault secret show --name "$secretKey" --vault-name "$keyVaultName" --query "value"
		$secret = $secret.Trim('"')
		Write-Output "-----------'$secret'-------------"
		if(!$secret) {
			throw "Secret '$secretKey' not found in Key Vault"
			
		}
		$hashTable[$key] = $secret
	}
}

# Write hash table to temp settings file
$tempSettingsFile = New-TemporaryFile
$tempFiles += $tempSettingsFile
($hashTable | ConvertTo-Json) | Out-File -FilePath $tempSettingsFile.FullName

# Read deploy.json
Write-Output "Reading '$deployJsonFile'"
$content = Get-Content $deployJsonFile

# Replace '%AppSettings.json%' with temp settings file path
$content = $content -replace "%AppSettings.json%", ($tempSettingsFile.FullName -replace "\\", "\\")
$content = $content -replace "%SerialPort%", $serialPort

# Replace '%Mqtt.ClientPemCert%' and '%Mqtt.ClientPrivateKey with Key Vault certificate
if($hashTable.ContainsKey("Client.CertificateName") -and $hashTable.ContainsKey("Client.PemCert") -and $hashTable.ContainsKey("Client.PrivateKey")) {
	$certificateName = $hashTable["Client.CertificateName"]

	Write-Output "Reading client certificate '$certificateName' from Key Vault"

	$pem = az keyvault secret show --name "$certificateName" --vault-name "$keyVaultName" --query "value"
	$pem = $pem.Trim('"')

	$tempFiles += New-TemporaryFile
	$pem -match "(?s)-----BEGIN CERTIFICATE-----.+-----END CERTIFICATE-----"
	$matches[0] | Out-File -FilePath $tempFiles[-1].FullName
	$content = $content -replace "%Client.PemCert%", ($tempFiles[-1].FullName -replace "\\", "\\")

	$tempFiles += New-TemporaryFile
	$pem -match "(?s)-----BEGIN PRIVATE KEY-----.+-----END PRIVATE KEY-----"
	$matches[0] | Out-File -FilePath $tempFiles[-1].FullName
	$content = $content -replace "%Client.PrivateKey%", ($tempFiles[-1].FullName -replace "\\", "\\")
} else {
	Write-Output "No client certificate in settings file"
}

# Create temp deploy file
$tempFiles += New-TemporaryFile
$content | Set-Content $tempFiles[-1].FullName

# Deploy to device
Write-Output "Deploying to device $serialPort"
nanoff --filedeployment $tempFiles[-1].FullName

# Clean up
Write-Output "Cleaning up"
ForEach ($file in $tempFiles) {
	Remove-Item -Path $file.FullName
}
Write-Output "Done"
