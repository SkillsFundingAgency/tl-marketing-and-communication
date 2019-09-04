var gulp = require('gulp');

require('./gulp/tasks/default');

gulp.task('default', ['bootstrap', 'assets', 'sass', 'js', 'html', 'html:watch', 'sass:watch']);


