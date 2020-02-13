const { src } = require('gulp');

var gulp = require('gulp'),
    concat = require('gulp-concat'),
    minify = require('gulp-minify'),
    sass = require('gulp-sass'),
    wait = require('gulp-wait'),
    watch = require('gulp-watch');

const paths = require('../paths.json');
const sassOptions = require('../sassOptions.js');

gulp.task('assets', () => {
    return src(paths.src.Assets)
        .pipe(gulp.dest(paths.dist.Assets));
});

gulp.task('json', () => {
    return src(paths.src.Json)
        .pipe(gulp.dest(paths.dist.Json));
});

gulp.task('js', () => {
    return src([
        'node_modules/jquery/dist/jquery.min.js',
        'node_modules/bootstrap/dist/js/bootstrap.min.js',
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

gulp.task('plyrjs', () => {
    return src([
        'node_modules/plyr/dist/plyr.polyfilled.min.js',
    ])
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('sass', () => {
        return src(paths.src.SCSS)
            .pipe(wait(200))
            .pipe(sass(sassOptions))
            .pipe(gulp.dest(paths.dist.CSS));
    }
);

gulp.task('sitemap', () => {
    return src(paths.src.Assets + "sitemap.xml")
        .pipe(gulp.dest(paths.dist.default));
});