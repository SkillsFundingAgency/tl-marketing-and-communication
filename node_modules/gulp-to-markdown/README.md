gulp-to-markdown
================

Gulp wrapper for [to-markdown](https://github.com/domchristie/to-markdown) - an HTML to Markdown converter created by Dom Christie.
*Issues with the output should be reported on the [to-markdown issue tracker] (https://github.com/domchristie/to-markdown/issues).*

##Install
```
$ npm install --save-dev gulp-to-markdown
```

##Usage
```javascript
var gulp = require('gulp');
var toMarkdown = require('gulp-to-markdown');

gulp.task('default', function () {
    return gulp.src('./src/**/*.html')
        .pipe(toMarkdown())
        .pipe(gulp.dest('./dist'));
});
// .md files will be in ./dist
```
