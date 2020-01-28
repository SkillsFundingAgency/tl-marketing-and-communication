/// <binding BeforeBuild='dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
require('./gulp/tasks/dev');

gulp.task('default', gulp.series('assets', 'sass', 'js', 'plyrjs', 'json', 'sitemap',
    (done) => {
        done();
    }));

gulp.task('dev', gulp.series('assets', 'dev.sass', 'dev.js', 'plyrjs', 'json', 'sitemap',
    (done) => {
        done();
    }));


gulp.task('devwatch', gulp.series('assets', 'dev.sass', 'dev.js', 'plyrjs', 'json', 'sitemap', 'dev.sass:watch',
    (done) => {
        done();
    }));