/// <binding BeforeBuild='Dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
require('./gulp/tasks/dev');

gulp.task('default', ['assets', 'sass', 'js', 'plyrjs', 'json', 'sitemap' ]);


gulp.task('dev', ['assets', 'sass', 'dev.js', 'json']);

