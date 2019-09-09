/// <binding BeforeBuild='Dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');

gulp.task('default', ['bootstrap', 'assets', 'sass', 'js', 'sass:watch', 'js:watch', 'json' ]);


gulp.task('Dev', ['bootstrap', 'assets', 'sass', 'js', 'json']);
