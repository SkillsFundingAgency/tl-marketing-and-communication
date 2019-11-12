/// <binding BeforeBuild='Dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
require('./gulp/tasks/dev');

gulp.task('default', gulp.series('assets', 'sass', 'js', 'plyrjs', 'json', 'sitemap',
    (done) => {
        done();
    }));

gulp.task('dev', gulp.series('assets', 'sass', 'dev.js', 'plyrjs', 'json', 'sitemap',
    (done) => {
        done();
    }));