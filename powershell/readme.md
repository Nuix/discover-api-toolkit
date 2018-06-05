# Overview
The RingtailAPI PowerShell module is used to manage tokens, apikeys and interact with the Ringtail Connect API using Powershell

	Note: This has been validated on Powershell 4 but should work under Powershell 3 as well.

# How to install
Until this module is available in PowershellGallery, you can install directly from Github
1. Clone this repository
2. Change to the `.\ringtail-api-toolkit\powershell` sub-directory 
3. Import the module: `Import-Module \\Path\to\RingtailAPI`
4. Alternatively, install the module by copying the RingtailAPI directory to one of the valid module locations outlined here: https://msdn.microsoft.com/en-us/library/dd878350(v=vs.85).aspx
5. Verify that the module is loaded correctly:
`Get-Command -Module RingtailAPI`
This should provide a list of commands provided by the module.

# Manage your tokens and keys
This module looks for a configuration file in <$home>\.ringtail\config. Although you can edit it with any text editor (it's json format), you can also use a series of methods to create and manage various *profiles*. Each *profile* maps to a combination of Ringtail portal, apikey, token, and uri to the portal API endpoint. 

All *profiles* are named. If no name is provided, the name *default* is used. 

To create a profile:
1. Get an apiKey, token and URI for the Ringtail Portal you want to interact with.
2. Run the `Add-RingtailConfig` from Powershell and provide values for Token, ApiKey and Uri.

Verify the profile exists:  `Get-RingtailConfig -List`

You can also verify that the file exists: `Get-Content $home\.ringtail\config`

## Create a profile for another portal and/or account
If you have another portal and/or user account that you wish to run under, you can create additional profiles. Just provide a name for the new profile:
`Add-RingtailConfig -Name portal2` and enter the information for that portal and user account. 

After setting up a profile, you should run a simple query to verify it works correctly. To run with a non-default profile, just supply a profile name for the `-Profile` parameter. 

To update an existing account, you should first delete the existing profile and then re-add it with the new parameters:
`Remove-RingtailConfig -Name Portal1'


# Examples 
## Run query defined on command line. No variables
`Invoke-RingtailQuery -Query "{ cases { id name }}"`

## Run it from a file
`Invoke-RingtailQuery -Path .\queries\GetCases.txt`

## Use a differnt profile
`Invoke-RingtailQuery -Path .\queries\GetCases.txt -Profile portal2`

## Add variables. 
    $vars = @{ scroll = @{ limit=4; start = 0} }
    Invoke-RingtailQuery -Path .\queries\GetCaseStatsScroll.txt -Variables $vars

	NOTE: this just returns a single page of results
	NOTE: if variables aren't supplied then the $scroll parameters is 
		ignored and all results are returned. This allows you to write
		a single query that can be used to return all results, a single page or to scroll through all (see next example)

## Use a query with $scroll parameters and iterate/scroll through the data 
`Invoke-RingtailQueryWithScroll -Path .\queries\GetCaseStatsScroll.txt`

## Drill down to the cases results
`Invoke-RingtailQuery -Path .\queries\GetCases.txt | Select -ExpandProperty data | Select -ExpandProperty cases `

## Convert results to Json
`Invoke-RingtailQuery -Path .\queries\GetCaseStatsScroll.txt -Variables $vars  | ConvertTo-json -Depth 10`