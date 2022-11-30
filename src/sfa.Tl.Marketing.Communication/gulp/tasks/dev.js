const { src } = require('gulp');

var gulp = require('gulp');
var concat = require('gulp-concat');
var sass = require('gulp-sass')(require('node-sass'));
var wait = require('gulp-wait');

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
        'node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.js',
        (paths.src.JS)
    ])
        .pipe(concat('all.min.js'))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('dev.js:watch', () => {
    return gulp.watch((paths.src.JS), ['dev.js']);
});


gulp.task('dev.sass', () => {
    return src(paths.src.SCSS)
        .pipe(wait(200))
        .pipe(sass({
            errLogToConsole: true,
            outputStyle: 'expanded',
            includePaths: [
                'src/scss'
            ]
        }))
        .pipe(gulp.dest(paths.dist.CSS));
}
);

gulp.task('dev.sass:watch', () => {
    gulp.watch((paths.src.SCSSWATCH), gulp.series('dev.sass'));
});

