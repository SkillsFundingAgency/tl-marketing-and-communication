const { src } = require('gulp');

var gulp = require('gulp');
var concat = require('gulp-concat');

const paths = require('../paths.json');

gulp.task('bootstrap', () => {
    return src([
        'node_modules/bootstrap/dist/css/bootstrap-grid.min.css',
        'node_modules/bootstrap/dist/js/bootstrap.min.js',
    ])
        .pipe(gulp.dest((paths.dist.default) + '/bootstrap'));
});


gulp.task('dev.js', () => {
    return gulp.src([
        'node_modules/jquery/dist/jquery.js',
        (paths.src.JS)
    ])
        .pipe(concat('all.min.js'))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('dev.js:watch', () => {
    return gulp.watch((paths.src.JS), ['dev.js']);
});