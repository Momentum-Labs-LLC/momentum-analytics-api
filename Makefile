TIMESTAMP = $(shell date -u +"%Y%m%d.%H%M%S") # $(Get-Date -Format "yyyyMMddHHmmss") 

clean: 
	dotnet clean
	rm -rm tests\coverage
	rm -rf tests\coverage

restore:
	dotnet restore

build:
	dotnet build

test: build
	dotnet test

test-coverage:
	dotnet test --color:"XPlat Code Coverage" --results-directory "tests\coverage"
	reportgenerator -reports:"tests\coverage\*\coverage.cobertura.xml" -targetdir:"tests\report" -reporttypes:Html
	open tests\report\index.html

test-timestamp:
	echo ${TIMESTAMP}

pack:
	dotnet pack --no-restore -o -nuget --version-suffix=${TIMESTAMP}
