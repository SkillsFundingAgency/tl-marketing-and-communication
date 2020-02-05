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

The console application *sfa.Tl.Marketing.Communication.DataLoad* can be run to regenerate the `providers.json` file. 

Default paths to input and output file paths are in constants in the code; if you want to set different paths add a file called `appsettings.json` with the content below. DO NOT CHECK THIS IN.

{
  "InputFilePath": "<path to file>",
  "OutputFilePath": "<path to file>"
}

After running the program, simply copy the `providers` section from the output file over the existing `providers` section in `\sfa.Tl.Marketing.Communication\Frontend\src\json\providers.json`.

