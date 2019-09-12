/// <binding BeforeBuild='Dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
require('./gulp/tasks/dev');

gulp.task('default', ['bootstrap', 'assets', 'sass', 'js', 'json' ]);


gulp.task('dev', ['bootstrap', 'assets', 'sass', 'dev.js', 'sass:watch', 'dev.js:watch', 'json']);

//gulp.task('Dev', ['bootstrap', 'assets', 'sass', 'js', 'json']);
