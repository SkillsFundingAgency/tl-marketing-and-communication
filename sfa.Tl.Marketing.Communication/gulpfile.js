/// <binding BeforeBuild='dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
require('./gulp/tasks/dev');

gulp.task('default', gulp.series('assets', 'sass', 'js', 'sitemap', 'purifycss',
    (done) => {
        done();
    }));

gulp.task('dev', gulp.series('assets', 'dev.sass', 'dev.js', 'sitemap',
    (done) => {
        done();
    }));


gulp.task('devwatch', gulp.series('assets', 'dev.sass', 'dev.js', 'sitemap', 'dev.sass:watch',
    (done) => {
        done();
    }));