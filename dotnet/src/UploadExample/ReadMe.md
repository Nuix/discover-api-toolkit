# Overview
This Example app shows how to transfer files to a Discover environment using Aspera.

# Required reading:
https://developer.asperasoft.com/desktop/fasp-manager/index
https://developer.asperasoft.com/reference

# General workflow:
The client app must query the Discover Connect API to retrieve an Aspera bearer token.
This bearer token is used for authentication and authorization during the transfer to the Aspera server
The asperaweb_id_dsa.openssh is a wellknown ssh key published with the Aspera Connect client and must be provided to the api as well

# Details
This example contains 3 key projects:
## Common/Ringtail.API
This project contains a simple client for querying the Connect API
The Config class is used to read your configuration file located in ~\\.ringtail\config. 
ex. C:\Users\jdoe\.ringtail\config
[
    {
	"name":  "default",
	"uri":  "http://devring.rtdev.nuix.com/Ringtail.WebServices.Portal/api/",
	"token":  <the user token>,
	"apiKey":  <the api key>"
    }
]
The token and key can be retrieved within Discover by navigating to the "Portal -> User Administration -> Users" from there, select a user and go to the "API Access" tab

## FileTransfer Example/FASPClient
### FaspClient class
This class is a simple wrapper around the FaspManager api and FaspStream api.  It requires several aspera utility files to be in place to function
* Aspera redistributable
https://developer.asperasoft.com/desktop/tools/redistributable-package
this redistributable contains the executables and libraries necessary for the Aspera SDK to function and must be bundled with your application
* Aspera SDK
https://developer.asperasoft.com/desktop/fasp-manager/download
The C# sdk comes in the format of a zipped nuget package which must be downloaded and added to a nuget repository.  Once installed to the repository, a package reference can be added via nuget to the project

## UploadExample
This project is the main program which ties the example upload together.  It uses the Ringtail.API project to call the Discover Connect API and retrieve a bearer token and configuration for Aspera.  It then passes this configuration to the FASPClient class and initiates a file transfer


