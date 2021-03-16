# tl-marketing-and-communication

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

Website configuration is in `appsettings.json` and the settings for different enviromnments are set by Azure DevOps release/pipeline variables.  
If you need to override values on your local machine, add a `appsettings.Development.json` file and set `Copy to Output Directory` to `Copy if newer`, then add jeys/values there.

Other API Keys and email addresses are also stored in the site `appSettings.json` file and need to be overridden in `appsettings.Development.json` as shown below.

```
{
  "EmployerContactEmailTemplateId": "<value>"
  "GovNotifyApiKey": "<api_key_value>",
  "SupportEmailInboxAddress": "<your_email>",
  "TableStorageConnectionString": "UseDevelopmentStorage=true;",
  "CacheExpiryInSeconds": time,
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```

- `GovNotifyApiKey` will come from GOV.UK Notify service settings (check with DevOps if you don't have access). 
- `EmployerContactEmailTemplateId` is the email template id created in GOV.UK Notify for the contact email. A default value is set in `appSettings.json`.
- `SupportEmailInboxAddress` can be a single email address or a semicolon-separated list of email addresses where the contact email should be sent to.
- `TableStorageConnectionString` defaults to Azure Storage Emulator. If you want to use a cloud table, set the connection string here.
- `PostcodeRetrieverBaseUrl` is usually `https://postcodes.io/` - this is set in `appSettings.json`.


## Creating provider data in local storage

Data for the student search page is stored in Azure Storage Tables. 
This data is imported from the NCS Course Directory API using a scheduled function, but in a development environment sample data can be written to local storage as follows:

1. The console application *sfa.Tl.Marketing.Communication.DataLoad* can be run to copy data into local storage. 
2. This can be run on developer machines when Azure Storage Explorer is running.
3. The program will create tables called `Qualification`, `Provider` and `Location` and copy data from json files.
4. You need to set the table connection string and paths to sample files in `appsettings.json` with the content below. DO NOT CHECK THIS IN (it is already in .gitignore). The paths below use files that are included in the project.

{
  "ProviderJsonInputFilePath": "..\\..\\..\\Provider Data\\providers.json",
  "QualificationJsonInputFilePath": "..\\..\\..\\Provider Data\\qualifications.json",
  "TableStorageConnectionString": "UseDevelopmentStorage=true;"
}


