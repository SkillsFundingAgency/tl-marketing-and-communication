$ErrorActionPreference = "Stop"

if ((Get-AzContext).Subscription.Name -ne 's126-tlevelservice-development') {
    throw 'Azure Context references incorrect subscription'
}

$scriptRoot = $PSScriptRoot
if (($PSScriptRoot).Length -eq 0) { $scriptRoot = $PWD.Path}

$location = "westeurope"
$applicationPrefix = "mcom"
$envPrefix = "s126d99"
$environmentNameAbbreviation = "xxx"
$templateFilePrefix = "marketingcommunications"
$certsToUpload = @{   
    "dev-tlevels-gov-uk" = '3799F96CD8C0AB01D444C13ED40271178051C87A'
}

$sharedResourceGroupName = $envPrefix + "-$($applicationPrefix)-shared"
$envResourceGroupName = $envPrefix + "-$($applicationPrefix)-$($environmentNameAbbreviation)"

# purge the keyvault if it's in InRemoveState due to resource group deletion
# this is up front as it's such a common reason for the script to fail
if (Get-AzKeyVault -Vaultname "$($envPrefix)$($applicationPrefix)sharedkv" -Location $location -InRemovedState) { 
    Write-Host 'Purging vault'
    Remove-AzKeyVault -VaultName "$($envPrefix)$($applicationPrefix)sharedkv" -InRemovedState -Location $location -Force
}

Get-AzResourceGroup -Name $sharedResourceGroupName -ErrorVariable notPresent -ErrorAction SilentlyContinue
if ($notPresent) {
    $tags = @{
        "Environment" = "Dev"
        "Parent Business" = "Education and Skills Funding Agency"
        "Portfolio" = "Education and Skills Funding Agency"
        "Product" = "T-Levels"
        "Service" = "ESFA T Level Service"
        "Service Line" = "Professional and Technical Education"
        "Service Offering" = "ESFA T Level Service"
    }
    New-AzResourceGroup -Name $sharedResourceGroupName -Location $location -Tag $tags
}

$sharedDeploymentParameters = @{
    Name                    = "test-{0:yyyyMMdd-HHmmss}" -f (Get-Date)
    ResourceGroupName       = $sharedResourceGroupName
    Mode                    = "Complete"
    Force                   = $true
    TemplateFile            = "$($scriptRoot)/$($templateFilePrefix)-shared.json"
    TemplateParameterObject = @{
        environmentNameAbbreviation             = "$($envPrefix)-$($applicationPrefix)"
        threatDetectionEmailAddress             = "noreply@example.com"
        appServicePlanTier                      = "Standard"
        appServicePlanSize                      = "1"
        appServicePlanInstances                 = 1
        azureWebsitesRPObjectId                 = "0b11c7a6-2868-4728-b83c-d14be9147a97"
        keyVaultReadWriteObjectIds              = @("0316d3ae-e503-4dae-9665-c999fca7cf10", "a6621090-e704-45ec-b65f-50257f9d4dcd")
        keyVaultFullAccessObjectIds             = @("b3b225a1-7c11-4698-9f15-32c345cf5bc2")  
    }
}

$sharedDeployment = New-AzResourceGroupDeployment @sharedDeploymentParameters

foreach ($key in $certsToUpload.Keys) {
    # first get a random 32 character alphanumeric key into a SecureString
    $certPassword = ConvertTo-SecureString `
                        -AsPlainText `
                        -Force `
                        -String ([System.Web.Security.Membership]::GeneratePassword(64, 0) -replace "[^a-zA-Z0-9]","").Substring(0,32)

    # save the certificate including private key into file protected with the above password
    Export-PfxCertificate `
        -Password $certPassword `
        -FilePath "$($key).pfx" `
        -Cert "cert://CurrentUser/my/$($certsToUpload[$key])"

    # import the certificate to KeyVault 
    Import-AzKeyVaultCertificate `
            -VaultName "$($envPrefix)$($applicationPrefix)sharedkv" `
            -Name $key `
            -FilePath "$($key).pfx" `
            -Password $certPassword   
    
    # and then delete the file and forget the password
    Remove-Item -Path "$($key).pfx"
    Clear-Variable -Name certPassword
}

Get-AzResourceGroup -Name $envResourceGroupName -ErrorVariable notPresent -ErrorAction SilentlyContinue
if ($notPresent) {
    $tags = @{
        "Environment" = "Dev"
        "Parent Business" = "Education and Skills Funding Agency"
        "Portfolio" = "Education and Skills Funding Agency"
        "Product" = "T-Levels"
        "Service" = "ESFA T Level Service"
        "Service Line" = "Professional and Technical Education"
        "Service Offering" = "ESFA T Level Service"
    }
    New-AzResourceGroup -Name $envResourceGroupName -Location $location -Tag $tags
}

$deploymentParameters = @{
    Name                    = "test-{0:yyyyMMdd-HHmmss}" -f (Get-Date)
    ResourceGroupName       = $envResourceGroupName
    Mode                    = "Incremental"
    TemplateFile            = "$($scriptRoot)/$($templateFilePrefix)-environment.json"
    TemplateParameterObject = @{
        environmentNameAbbreviation             = $environmentNameAbbreviation
        resourceNamePrefix                      = ("$($envPrefix)-$($applicationPrefix)-" + $environmentNameAbbreviation)
        #logAnalyticsWorkspaceFQRId              = ($sharedDeployment.Outputs.logAnalyticsWorkspaceFQRId.Value)
        sharedASPName                           = "$($envPrefix)-$($applicationPrefix)-shared-asp"
        sharedEnvResourceGroup                  = $sharedResourceGroupName
        sharedKeyVaultName                      = "$($envPrefix)$($applicationPrefix)sharedkv"
        configurationStorageConnectionString    = ($sharedDeployment.Outputs.configStorageConnectionString.Value)
        uiCustomHostname                        = "dev.tlevels.gov.uk"                     
        certificateName                         = "dev-tlevels-gov-uk"
        courseDirectoryImportTrigger            = "0 0 1 1 2" # 1st January so long as it's a Tuesday, so won't trigger anytime soon
    }
}

$envDeployment = New-AzResourceGroupDeployment @deploymentParameters -ErrorVariable errorOutput
if ($envDeployment.ProvisioningState -eq "Succeeded") {
    Write-Output "Yippee!!"
}

<# A section to allow easy cleanup of the environments, the first line is because I've been looking at migration from app insights.

# you have to remove the diagnostic settings separately as they hang around if you don't and mess things up badly
$subscriptionId = (Get-AzContext).Subscription.Id
$diagnosticResourceIds = @(
    "/subscriptions/$($subscriptionId)/resourceGroups/$($sharedResourceGroupName)/providers/Microsoft.KeyVault/vaults/$($envPrefix)$($applicationPrefix)sharedkv",
    "/subscriptions/$($subscriptionId)/resourceGroups/$($envResourceGroupName)/providers/Microsoft.Web/sites/$($envPrefix)-$($applicationPrefix)-$($environmentNameAbbreviation)-web",
    "/subscriptions/$($subscriptionId)/resourceGroups/$($envResourceGroupName)/providers/Microsoft.Web/sites/$($envPrefix)-$($applicationPrefix)-$($environmentNameAbbreviation)-func-worker"
)
foreach ($diagnosticResourceId in $diagnosticResourceIds) {
    Write-Host "Finding settings in $($diagnosticResourceId) to remove"
    foreach ($setting in (Get-AzDiagnosticSetting -ResourceId $diagnosticResourceId -ErrorAction SilentlyContinue)) {
        Write-Host "Removing $(($setting).Name)"
        Remove-AzDiagnosticSetting -ResourceId $diagnosticResourceId -Name $setting.Name
    }   
}

Remove-AzOperationalInsightsWorkspace -ResourceGroupName $sharedResourceGroupName -Name "$($sharedResourceGroupName)-log" -ForceDelete -Force -ErrorAction SilentlyContinue

Remove-AzResourceGroup -ResourceGroupName $envResourceGroupName -Force -ErrorAction SilentlyContinue
Remove-AzResourceGroup -ResourceGroupName $sharedResourceGroupName -Force -ErrorAction SilentlyContinue
#>

