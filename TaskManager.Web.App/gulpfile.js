var gulp = require('gulp')
var tasks = require('./tasks')
var config = require('./config.json');

gulp.task('clean'          ,                    tasks.clean             (config));
gulp.task('lint'           ,                    tasks.lint              (config));
gulp.task('debug'          , ['lint', 'clean'], tasks.debug             (config));
gulp.task('bundled'        , ['lint', 'clean'], tasks.bundled           (config));
gulp.task('release'        , ['lint', 'clean'], tasks.release           (config));

gulp.task('default', ['debug']);