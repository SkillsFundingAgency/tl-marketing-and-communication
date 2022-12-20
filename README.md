[![Build Status](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_apis/build/status/S126-TL/Marketing%20%26%20Communication/tl-marketing-and-communication?repoName=SkillsFundingAgency%2Ftl-marketing-and-communication&branchName=master)](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_build/latest?definitionId=281&repoName=SkillsFundingAgency%2Ftl-marketing-and-communication&branchName=master)

# tl-marketing-and-communication

T Level Marketing and Communications website.


## Requirements 

1. node - https://nodejs.org/en/
2. npm - https://www.npmjs.com/package/npm
3. Gulp - https://gulpjs.com/


## Actions

To set up gulp and npm, `cd` to the project "root" directory then:

|Task|Description|
|----|-----------|
| `npm install` | Installs all node packages |
| `gulp` | Merges, compiles and moves all sass / js / assets. Creates all front end resources |

If the above has worked you should see files being generated in frontend.

Developers don't need to run gulp manually as it should be run by the task runner. If there are errors when running the gulp tasks, check that you have the latest version.
If the node.js version is newer than the one used in Visual Studio, the task might not run properly and you might see a warning about bindings.
To fix this, set the version that Visual Studio runs by following the following steps. See https://ryanhayes.net/synchronize-node-js-install-version-with-visual-studio/.
* Open Tools > Options
* In the dialog navigate to Projects and Solutions > Web Package Management > External Web Tools 
* Add C:\Program Files\nodejs at the top of the locations list.
* Alternatively, add the nodejs folder to Windows PATH.


## Configuration

Website configuration is in `appsettings.json` but most settings are read from Azure Storage.

Some of the configuration settings are also in `appsettings.json` but will only be used if the Azure Storage Emulator or Azurite is not running. This is done for convenience and so that developers don't always need to run Azure Storage, but it will not work for the Find page.

If you need to override any values on your local machine, add a `appsettings.Development.json` file and set `Copy to Output Directory` to `Copy if newer`, then add keys/values there.

To set up configuration in local Azure Storage on a developer machine, make sure Azure Storage Emulator or Azurite is running then add a table to local storage called `Configuration` if it doesn't already exist.
Add a new row to the table with:

- PartitionKey : `LOCAL`
- RowKey : `Sfa.Tl.Marketing.Communication_1.0`
- a property `Data` with the value below.

```
{
  "EmployerSiteSettings": {
      "SiteUrl": "<address>",
      "AboutArticle": "<partial path to article>",
      "IndustryPlacementsBenefitsArticle": "<partial path to article>",
      "SkillsArticle": "<partial path to article>",
      "TimelineArticle": "<partial path to article>"
  },
  "CourseDirectoryApiSettings": {
    "BaseUri": "<Course Directory API>",
    "ApiKey": "<API Key>"
  },
  "PostcodeRetrieverBaseUrl": "https://api.postcodes.io/",
  "CacheExpiryInSeconds": 60,
  "MergeTempProviderData": true,
  "PostcodeCacheExpiryInSeconds": 120,
  "StorageSettings":  {
    "BlobStorageConnectionString": "UseDevelopmentStorage=true;",
    "TableStorageConnectionString": "UseDevelopmentStorage=true;"
  },
"GoogleMapsApiKey": "<google key>"
}
```

- `CacheExpiryInSeconds` - Default cache time (in seconds) used for caching providers and qualifications.
- `CourseDirectoryApiSettings` 
  - `BaseUri` is the Course Directory API Uri - this can be found in the Azure DevOps library settings for the project.
  - `ApiKey` is the Course Directory API key - this can be found in Azure Key Vault for the project.
- `EmployerSiteSettings` 
  - `SiteUrl` needs to be the address of the Zendesk Employer Support site.
  - Article settings should be the url fragment which will be appended to the site url and point to the articles on Zendesk. 
- `TableStorageConnectionString` defaults to Azure Storage Emulator. If you want to use a cloud table, set the connection string here.
- `PostcodeCacheExpiryInSeconds` - the time (in seconds) to keep postcodes cached. Used to reduce repeated calls to postcodes.io.
- `PostcodeRetrieverBaseUrl` is usually `https://api.postcodes.io/`.
- `GoogleMapsApiKey` - Google key. Only required if maps are going to be used.


## Creating provider data in local storage

Data for the student search page is stored in Azure Storage Tables. 
This data is imported from the NCS Course Directory API using a scheduled function, but in a development environment sample data can be written to local storage as follows.

To load providers and qualifications from the NCS feed:

1. Run the functions project. Make sure the NCS Course Directory base uri and 
2. Using a tool such as Postman, create a POST request with address http://localhost:7071/admin/functions/CourseDirectoryScheduledImport.
3. Add a header `x-functions-key`.
4. Set the body to type `json` with an empty body `{}`.
5. Send the request.

To load town data:

1. Go to https://geoportal.statistics.gov.uk/datasets/ons::index-of-place-names-in-great-britain-july-2016-table/explore?showTable=true and download the file as csv. 
    The following columns are used, but the rest can be deleted to make the file smaller:
           placeid
           place15nm
           cty15nm
           ctry15nm
           ctyltnm
           laddescnm
           lad15nm
           descnm
           lat
           long
           popcnt
2. Make sure the functions project is running.
3. Using a tool such as Postman, create a POST request with address http://localhost:7071/api/UploadIndexOfPlaceNames.
4. Set body to form-data and add a key called file then select your csv file to attach it.
5. Press send to upload the file.


## Azure Functions

Default development configuration is in file `local.settings.json`. Configuration settings are loaded from Azure Storage in the same way as for the website.

> Note: the service version for loading the configuration table for functions uses `ServiceVersion` instead of 'Version' to avoid errors when running functions on a developer machine.


## Benchmarks

> Only applies if a benchmarking project has been added to the solution.

To run benchmarks, make sure the project is in release mode, open a terminal or console, 
navigate to the solution directory (e.g. `cd \dev\esfa\tl-marketing-and-communication\`) 
then run
```
dotnet run --project sfa.Tl.Marketing.Communication.Benchmarks\sfa.Tl.Marketing.Communication.Benchmarks.csproj -c Release
```

If you see a message that `sfa.Tl.Marketing.Communication.StaticWebAssets.xml"` is not found, try running the command again.


