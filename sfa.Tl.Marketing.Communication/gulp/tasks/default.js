
var gulp = require('gulp');
var concat = require('gulp-concat');
var minify = require('gulp-minify');
var sass = require('gulp-sass');
var cleanCSS = require('gulp-clean-css');
var concatCss = require('gulp-concat-css');
var wait = require('gulp-wait');

const paths = require('../paths.json');
const sassOptions = require('../sassOptions.js');


gulp.task('assets', () => {
    gulp.src(paths.src.Assets)
        .pipe(gulp.dest(paths.dist.Assets));
});

gulp.task('json', () => {
    gulp.src(paths.src.Json)
        .pipe(gulp.dest(paths.dist.Json));
});


gulp.task('js', () => {
    return gulp.src([
        'node_modules/jquery/dist/jquery.min.js',
        'node_modules/bootstrap/dist/js/bootstrap.min.js',
        'node_modules/plyr/dist/plyr.polyfilled.min.js',
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
    .pipe(wait(200))
    .pipe(sass(sassOptions))
    .pipe(gulp.dest(paths.dist.CSS))
);




