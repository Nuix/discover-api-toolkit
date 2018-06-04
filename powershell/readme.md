


-- Examples -- 
## Run query defined on command line. No variables
Invoke-RingtailQuery -Query "{ cases { id name }}"

## Run it from a file
Invoke-RingtailQuery -Path .\queries\GetCases.txt

## Add variables. 
$vars = @{ scroll = @{ limit=4; start = 0} }
Invoke-RingtailQuery -Path .\queries\GetCaseStatsScroll.txt -Variables $vars

	NOTE: this just returns a single page of results
	NOTE: if variables aren't supplied then the $scroll parameters is 
		ignored and all results are returned. This allows you to write
		a single query that can be used to return all results, a single page or to scroll through all (see next example)

## Use a query with $scroll parameters and iterate/scroll through the data 
Invoke-RingtailQueryWithScroll -Path .\queries\GetCaseStatsScroll.txt

## Drill down to the cases results
Invoke-RingtailQuery -Path .\queries\GetCases.txt | Select -ExpandProperty data | Select -ExpandProperty cases 

## Convert results to Json
Invoke-RingtailQuery -Path .\queries\GetCaseStatsScroll.txt -Variables $vars  | ConvertTo-json -Depth 10