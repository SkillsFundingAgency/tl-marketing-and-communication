
var gulp = require('gulp');
var concat = require('gulp-concat');
var minify = require('gulp-minify');
var sass = require('gulp-sass');
var cleanCSS = require('gulp-clean-css');
var concatCss = require('gulp-concat-css');

const paths = require('../paths.json');
const sassOptions = require('../sassOptions.js');

gulp.task('bootstrap', () => {
    gulp.src([
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

gulp.task('dev.js:watch', function () {
    gulp.watch((paths.src.JS), ['dev.js']);
});