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

If the above has worked you should see files being generated in frontend.If the above has worked you should see files being generated in frontend.

## Configuration

The Google Maps API key is stored in the site appSettings.json file. To set the value locally, you will need to add a file 'appsettings.Development.json' to the project with the content below. Add the actual key in place of `<value>`

```
{
  "Configuration": {
    "GoogleMapsApiKey": "<value>"
  }
}
```

