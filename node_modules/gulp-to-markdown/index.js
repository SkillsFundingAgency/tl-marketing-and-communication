'use strict';
var gutil = require('gulp-util'),
    through = require('through2'),
    toMarkdown = require('to-markdown');

module.exports = function () {
  return through.obj(function (file, enc, cb) {
    var md;

    if (file.isNull()) {
      cb(null, file);
      return;
    }

    if (file.isStream()) {
      cb(new gutil.PluginError('gulp-to-markdown', 'Streaming not supported'));
      return;
    }

    try {
      md = toMarkdown(file.contents.toString());
    } catch (e) {
      cb(new gutil.PluginError('gulp-to-markdown', e, {fileName: file.path}));
      return;
    }

    file.contents = new Buffer(md);
    file.path = gutil.replaceExtension(file.path, '.md');
    cb(null, file);
  });
};
