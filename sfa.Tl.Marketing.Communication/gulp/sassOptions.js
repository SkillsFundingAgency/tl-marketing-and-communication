let sassOptions;

sassOptions = {
  errLogToConsole: false,
    outputStyle: 'compressed',
    onError: function (err) {
        return notify().write(err);
    },
  includePaths: [
      'src/scss'
  ]
};

module.exports = sassOptions;