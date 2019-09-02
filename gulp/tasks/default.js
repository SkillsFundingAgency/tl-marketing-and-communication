
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


gulp.task('assets', () => {
    gulp.src(paths.src.Assets)
        .pipe(gulp.dest(paths.dist.Assets));
});

gulp.task('html', () => {
    gulp.src(paths.src.HTML)
        .pipe(gulp.dest(paths.dist.HTML));
});


gulp.task('js', () => {
    return gulp.src([
        'node_modules/jquery/dist/jquery.min.js',
        (paths.src.JS)
    ])
        .pipe(concat('all.js'))
        .pipe(minify({
            noSource: true,
            ext: {
                min: '.min.js'
            }
        }))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('sass', () => gulp
    .src(paths.src.SCSS)
    .pipe(sass(sassOptions))
    .pipe(gulp.dest(paths.dist.CSS))
);

gulp.task('html:watch', function () {
    gulp.watch((paths.src.HTML), ['html']);
});

gulp.task('sass:watch', function () {
    gulp.watch((paths.src.SCSS), ['sass']);
});
