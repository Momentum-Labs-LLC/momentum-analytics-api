# momentum-pixel-api
An api for receiving behavioral data information from a client.

# Setup Private Package Repo Access
`dotnet nuget add source "https://nuget.pkg.github.com/Momentum-Labs-LLC/index.json" --name github --username ${GITHUB_USER} --password ${GITHUB_TOKEN} --store-password-in-clear-text`

# Local DynamoDb Tables
You will likely need to create a local aws profile. You don't need actually key/secret or account id values.
`aws dynamodb create-table --endpoint-url http://localhost:9876 --profile=local --cli-input-json file://local/page-views.json`
`aws dynamodb create-table --endpoint-url http://localhost:9876 --profile=local --cli-input-json file://local/pii-values.json`
`aws dynamodb create-table --endpoint-url http://localhost:9876 --profile=local --cli-input-json file://local/collected-pii.json`
`aws dynamodb create-table --endpoint-url http://localhost:9876 --profile=local --cli-input-json file://local/visits.json`

# Run Locally against api
`GITHUB_USER={USER} GITHUB_TOKEN={TOKEN} docker compose build`
`docker compose up`