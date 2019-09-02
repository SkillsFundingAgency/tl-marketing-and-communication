var expect = require('chai').expect,
    gulpToMarkdown = require('../'),
    gutil = require('gulp-util'),
    LT = gutil.linefeed,
    File = gutil.File;

describe('gulp-to-markdown', function() {

  it('converts HTML to Markdown.', function(done) {
    var stream = gulpToMarkdown(),
        htmlSamples = [
          '<h1>Header1</h1><h2>Header2</h2><h3>Header3</h3>',
          '<em>Emphasis</em>',
          '<strong>Strong</strong>',
          '<ul><li>List1</li><li>List2</li></ul>',
          '<ol><li>List1</li><li>List2</li></ol>',
          '<a href="https://www.link.com">link</a>',
          '<img src="https://www.img.com/img.png" alt="alt" title="title">',
          'Inline <code>code</code> is <code>here</code>.',
          //'<pre>Block code is here.</pre>', // does not work
        ],
        mdSamples = [
          '# Header1' + LT + LT + '## Header2' + LT + LT + '### Header3',
          '_Emphasis_',
          '**Strong**',
          '*   List1' + LT + '*   List2',
          '1.  List1' + LT + '2.  List2',
          '[link](https://www.link.com)',
          '![alt](https://www.img.com/img.png "title")',
          'Inline `code` is `here`.',
          //'```' + LT + 'Block code is here.' + LT + '```', // does not work
        ],
        j = 0;

    stream.on('data', function(file) {
      expect(String(file.contents)).to.eq(mdSamples[j++]);
    });

    stream.once('end', function () {
      done();
    });

    for (var i = 0, il = htmlSamples.length; i < il; i++) {
      stream.write(new File({
        contents: new Buffer(htmlSamples[i]),
        cwd: './',
        base: '/test/',
        path: '/test/' + i + '.html'
      }));
    }
    stream.end();
  });

});
