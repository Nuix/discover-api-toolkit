## -- configuration -- 

function Get-RingtailConfigPath {
    $path = "$home\.ringtail\config"
    $path
}

## Note: This should not be exported
function Set-RingtailConfig {
    param(
        [object[]]$InputObject
    )
    $path = Get-RingtailConfigPath
    if( $InputObject.Count -eq 0 ) {
        Clear-Content -Path $path 
    } else {
        $InputObject | ConvertTo-Json | Set-Content -Path $path
    }
}

function Get-RingtailConfig {
    param(
        [string]$Profile='',
        [switch]$List
    )

    if( [string]::IsNullOrEmpty($Profile) ) {
        $Profile = "default"
    }

    $path = Get-RingtailConfigPath
    $config = @()
    
    if( Test-Path $path -PathType Leaf ) {
      
      $config = Get-Content $path | Out-String  | ConvertFrom-Json
    }

    ForEach($i in $config) {
        if( $List -or $Profile -ieq $i.name ) {
            $i
        }
    }
}

function Add-RingtailConfig {
    param(
        [string]$Name = '',
        [Parameter(Mandatory)]
        [string]$Token,
        [Parameter(Mandatory)]
        [string]$ApiKey,
        [Parameter(Mandatory)]
        [string]$Uri
    )
        if( [string]::IsNullOrEmpty($Profile) ) {
        $Profile = "default"
    }

    # Get the complete configuration 
    [Array]$config = Get-RingtailConfig -List
    
    #Create the new entry
    $newConfig = [PSCustomObject]@{name=$Name; token=$Token; apiKey=$ApiKey; uri=$Uri}
    $config += $newConfig

    # Save it back out
    $path = Get-RingtailConfigPath
    Set-RingtailConfig -InputObject $config
}

function Remove-RingtailConfig {
    param (
        [string]$Profile,
        [switch]$All
    )

    $config = @()
    if( -not $All ) {
        if( [string]::IsNullOrEmpty($Profile) ) {
            $Profile = "default"
        }

        # Get the complete configuration 
        [Array]$oldConfig = Get-RingtailConfig -List

        ForEach($i in $oldConfig) {
            if(  $Profile -ine $i.name ) {
                $config += $i
            }
        }
    }

    # Save it back out
    Set-RingtailConfig -InputObject $config

    $config
}



## --- Query functions. --- As
function Invoke-RingtailQueryWithScroll {
    param(
        [string] $Profile = '',
        [int] $PageSize = 10,
        [string]$Query,
        [string]$Path,
        $Variables
    )

    if($Path) {
        $Query = Get-Content -Path $Path | Out-String
    }

    if( !$Query) { throw "Must provide Query or Path" }

    # scroll through the query. Combine with the passed-in $Variables
    $vars = @{}
    if($Variables) { $vars = $Variables }
    $scroll = @{}
    $scroll.limit = $pageSize
    $scroll.start = 0
    $vars.scroll = $scroll

    do {
        $data = Invoke-RingtailQuery -Query $query -Variables $vars

        # are we there yet? (Note: This makes the function specific
        # to 'cases' queries. Need to find a generic approach.) TODO
        if($data.data.cases.Count -eq 0) { break}

        # return the data and get more
        $data
        $vars.scroll.start += 1
    } while ( $true )
}

function Invoke-RingtailQuery {
    param(
        [string]$Profile = '',
        [string]$Query,
        [string]$Path,
        $Variables   # object. E.g @{ scroll 

    )
    if($Path) {
        $Query = Get-Content -Path $Path | Out-String
    }

    if( !$Query) { throw "Must provide Query or Path" }

    $config = Get-RingtailConfig -Profile $Profile
    $token = $config.token
    $key = $config.apiKey

    $headers = @{}
    $headers.Add("Authorization", "Bearer $token")
    $headers.Add("ApiKey", $key)


    ## Execute the query
    $body = @{query=$Query; variables=$Variables; operationName=$null} | ConvertTo-Json -Compress

    $response = Invoke-WebRequest $uri -Method POST -Headers $headers -Body $body -ContentType "application/json"

    # convert from Json and return the object
    $data = ConvertFrom-Json  -InputObject $response.Content
    $data
}

