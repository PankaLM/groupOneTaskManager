'use strict';
/*global require, module*/
/*jshint -W097*/ // Use the function form of "use strict".
var path = require('path');
var exec = require('child_process').exec;

var _ = require('lodash');
var stream = require('stream');
var streamqueue = require('streamqueue');
var uniqueStream = require('unique-stream');
var gulp = require('gulp');
var plugins = require('gulp-load-plugins')();
var es = require('event-stream');

function bundleTemplates(templates, file) {
  return templates
        .pipe(plugins.html2js({
          outputModuleName: 'taskManager'
        }))
    .pipe(plugins.concat(file));
}

function inject(stream, tag) {
  return plugins.inject(stream, {
    starttag: '<!-- inject:' + tag + ':{{ext}} -->',
    transform: function (filepath, file) {
      var relativePath = path.relative(file.base, file.path).replace(/\\/g, '/');

      switch (path.extname(relativePath).slice(1)) {
        case 'css':
          return '<link rel="stylesheet" href="' + relativePath + '">';
        case 'js':
          return '<script src="' + relativePath + '"></script>';
      }
    },
    removeTags: true
  });
}

var relocateAssets = function (assets, cssDir) {
  return es.merge.apply(null, _.map(_.keys(assets), function (key) {
    return gulp.src(assets[key])
      .pipe(plugins.rename(function (path) {
        path.dirname = cssDir + key;
      }));
  }));
};

function queue() {
  var streams = _.isArray(arguments[0]) ? arguments[0] : _.toArray(arguments);

  streams = _.map(streams, function (s) {
    return s.pipe(stream.PassThrough({ objectMode: true }));
  });

  return streamqueue.apply(null, [{ objectMode: true }].concat(streams));
}

function uniqueQueue() {
  var streams = _.isArray(arguments[0]) ? arguments[0] : _.toArray(arguments);

  return queue(streams).pipe(uniqueStream(function (file) {
    return path.relative(file.base, file.path);
  }));
}

function buildTask(config, opts) {
  return function (callback) {
    exec('git rev-parse HEAD', function (error, stdout) {
      var revision = stdout.trim().substr(0, 6);

      var appJs =
        gulp.src(config.app)
        .pipe(plugins.preprocess({ context: opts.bundleTemplates ? {} : { DEBUG: true } }))
        .pipe(plugins['if'](opts.minifyJs, plugins.uglify({ mangle: false })))
        .pipe(plugins['if'](opts.bundleJs, plugins.concat(config.jsDir + 'app.js')))
        .pipe(plugins['if'](opts.bundleJs, plugins.rev()));

      var libJs =
        gulp.src(config.lib)
        .pipe(plugins['if'](opts.minifyJs, plugins.uglify({ mangle: false })))
        .pipe(plugins['if'](opts.bundleJs, plugins.concat(config.jsDir + 'lib.js')))
        .pipe(plugins['if'](opts.bundleJs, plugins.rev()));
      var css =
        gulp.src(config.css)
        .pipe(plugins.preprocess({ context: opts.bundleTemplates ? {} : { DEBUG: true } }))
        .pipe(plugins['if'](opts.minifyCss, plugins.minifyCss()))
        .pipe(plugins['if'](opts.bundleCss, plugins.concat(config.cssDir + 'styles.css')))
        .pipe(plugins['if'](opts.bundleJs, plugins.rev()));

      var templates = opts.bundleTemplates ?
        bundleTemplates(gulp.src(config.templates), config.templatesDir + 'templates.js')
          .pipe(plugins.rev()) :
        gulp.src(config.templates);

      var assets = relocateAssets(config.assets, config.cssDir);

      return uniqueQueue(
        appJs,
        libJs,
        css,
        templates,
        assets,
        gulp.src('index.html')
          .pipe(plugins.template({ fullVersion: config.version + '.0#' + revision }))
          .pipe(inject(appJs, 'app.js'))
          .pipe(inject(libJs, 'lib.js'))
          .pipe(inject(css, 'styles.css'))
          .pipe(plugins['if'](opts.bundleTemplates, inject(templates, 'templates.js')))
      )
      .pipe(plugins.size({ gzip: false }))
      .pipe(plugins.size({ gzip: true }))
      .pipe(gulp.dest(config.outputDir))
      .on('end', callback);
    });
  };
}

var tasks = {
  clean: function (config) {
    return function () {
      return gulp
        .src(config.outputDir, { read: false })
        .pipe(plugins.clean({ force: true }));
    };
  },
  lint: function (config) {
    return function () {
      return gulp.src(config.app)
        .pipe(plugins.jshint())
        .pipe(plugins.jshint.reporter('jshint-stylish'))
        .on('error', plugins.util.log);
    };
  },
  debug: function (config) {
    return buildTask(config, {});
  },
  bundled: function (config) {
    return buildTask(config, {
      bundleJs: true,
      bundleCss: true,
      bundleTemplates: true,
      relocateAssets: true
    });
  },
  release: function (config) {
    return buildTask(config, {
      minifyJs: true,
      bundleJs: true,
      minifyCss: true,
      bundleCss: true,
      bundleTemplates: true,
      relocateAssets: true
    });
  }
};

module.exports = tasks;