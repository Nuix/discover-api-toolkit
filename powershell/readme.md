# Overview
The RingtailAPI PowerShell module is used to manage tokens, API keys, and to interact with the Discover Connect API using Windows PowerShell

	Note: This has been validated on Powershell 4 but should work under Powershell 3 as well.

# How to install
Until this module is available in PowershellGallery, you can install directly from Github
1. Clone this repository
2. Open PowerShell and change to the `.\discover-api-toolkit\powershell` sub-directory 
3. Import the module by running the following command in PowerShell: `Import-Module \\Path\to\RingtailAPI`, replacing `\\Path\to\` with the actual full path
4. Alternatively, install the module by copying the RingtailAPI directory to one of the valid module locations outlined here: https://msdn.microsoft.com/en-us/library/dd878350(v=vs.85).aspx
5. Verify that the module is loaded correctly by running the following command in PowerShell:
`Get-Command -Module RingtailAPI`
If the RingtailAPI module has been imported successfully, this will return a list of commands provided by the module.

# Manage your tokens and keys
This module looks for a configuration file in <$home>\.ringtail\config. Although you can edit the configuration file with any text editor (it's in json format), you can also use a series of methods to create and manage various *profiles*. Each *profile* maps to a combination of Discover portal, apikey, token, and uri to the portal API endpoint. 

All *profiles* are named. If no name is provided, the name *default* is used. 

To create a profile:
1. Get an API token and URI for the Discover Portal you want to interact with. A Discover system administrator may enable a user for API access and obtain the user's API token
2. In PowerShell, run  `Add-RingtailConfig`, and provide values for Token and Uri.

Run the following command to verify that the profile exists:  `Get-RingtailConfig -List`

You can also verify that the configuration file exists by running the following: `Get-Content $home\.ringtail\config`

## Create a profile for another portal and/or account
If you have another Discover portal or user account that you wish to use, you can create additional profiles. Provide a name for each additional profile:
`Add-RingtailConfig -Name portal2`, and enter the information for that portal and user account. 

After setting up a profile, you should run a simple query to verify that it works correctly. To run with a non-default profile, just supply a profile name for the `-Profile` parameter. 

To update an existing profile, first delete the existing profile and then add it again with the new parameters:
`Remove-RingtailConfig -Name Portal1'

The format of the config file is just a JSON array with the following structure:
```json
[
    {
	"name":  "default",
	"uri":  "http://localhost/Ringtail.WebServices.Portal/api/query/",
	"token":  "<big long token>",
	"apiKey":  "<api key>"
    },
    {
	"name":  "profile2",
	"uri":  "<uri for portal api>",
	"token":  "<big long token>",
	"apiKey":  "<api key>"
    }
]
```
If there is only a single entry, then the array brackets are optional.
```json
{
    "name":  "default",
    "uri":  "http://localhost/Ringtail.WebServices.Portal/api/query/",
    "token":  "<big long token>",
    "apiKey":  "<api key>"
}
```

# Examples 
## Run query defined on command line, without variables
`Invoke-RingtailQuery -Query "{ cases { id name }}"`

## Run a query from a file
`Invoke-RingtailQuery -Path .\queries\GetCases.txt`

## Use a non-default profile
`Invoke-RingtailQuery -Path .\queries\GetCases.txt -Profile portal2`

## Run a query with variables
    Invoke-RingtailQuery -Query 'query ($name:String) { cases (name:$name) { id name }}' -Variables @{name="Enron"}

	NOTE: When specifying -Query and -Variables parameters in the command line, enclose
		the query in single quotes, and form the variables as PowerShell objects.
	
	$vars = @{ scroll = @{ limit=4; start=0} }
    Invoke-RingtailQuery -Path .\queries\GetCaseStatsScroll.txt -Variables $vars

	NOTE: This just returns a single page of results.
	NOTE: If variables aren't supplied, then the $scroll parameter is 
		ignored and the maximum number of results are returned. This allows you to write
		a single query that can be used to return all results, a single page, or to scroll 
		through all (see the next example).

## Use a query with $scroll parameters and iterate/scroll through the data 
`Invoke-RingtailQueryWithScroll -Path .\queries\GetCaseStatsScroll.txt`

## Drill down to the cases results
`Invoke-RingtailQuery -Path .\queries\GetCases.txt | Select -ExpandProperty data | Select -ExpandProperty cases `

## Convert results to Json
`Invoke-RingtailQuery -Path .\queries\GetCaseStatsScroll.txt -Variables $vars  | ConvertTo-json -Depth 10`
