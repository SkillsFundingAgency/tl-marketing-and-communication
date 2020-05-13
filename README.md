# tl-marketing-and-communication

## Requirements 

1. node - https://nodejs.org/en/
2. npm - https://www.npmjs.com/package/npm
3. Gulp - https://gulpjs.com/

## Actions
cd to "root" directory

|Task|Description|
|----|-----------|
| `npm install` | Installs all node packages |
| `gulp` | Merges, compiles and moves all sass / js / assets. Creates all front end resources |

If the above has worked you should see files being generated in frontend.

## Configuration

The Google Maps API key is stored in the site appSettings.json file. To set the value locally, you will need to add a file 'appsettings.Development.json' to the project with the content below. Add the actual key in place of `<value>`

```
{
  "GoogleMapsApiKey": "<value>"
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```

## Creating the providers data file

1. The console application *sfa.Tl.Marketing.Communication.DataLoad* can be run to regenerate the `providers.json` file. 
2. All provider data exist in "Full Provider Data 2020 - 2021 (campaign site).xlsx" spreadsheet, the latest copy exist in *sfa.Tl.Marketing.Communication.DataLoad/Provider Data* folder
3. To add a new qualification for course year a new row will be added for a provider venue to the spreadsheet
4. To regenerate the 'providers.json' from the spreadsheet, Save As the spreadsheet as .csv file. 
5. Before running the console app, update CsvFilePath and JsonOutputPath file paths in Programm.cs
Or
6. Default paths to input and output file paths are in constants in the code; if you want to set different paths add a file called `appsettings.json` with the content below. DO NOT CHECK THIS IN.

{
  "InputFilePath": "<path to file>",
  "OutputFilePath": "<path to file>"
}

7. After running the program, simply copy the `providers` section from the output file over the existing `providers` section in `\sfa.Tl.Marketing.Communication\Frontend\src\json\providers.json`.

