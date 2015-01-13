module.exports = function(grunt) {
  'use strict';

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    banner: '/*! <%= pkg.name %> - v<%= pkg.version %> - <%= grunt.template.today("yyyy-mm-dd") %>*/\n\n',
    concat: {
      options: {
        separator: ';'
      },
      dist: {
        src: ['www/js/**.js'],
        dest: 'www/dist/app.uncompressed.js'
      }
    },
    ngtemplates: {
      app: {
        cwd: 'www',
        src: 'templates/**.html',
        dest: 'www/js/templates.js',
        options: {
          module: 'interactive',
          htmlmin: {
            collapseWhitespace: false,
            collapseBooleanAttributes: false
          }
        }
      }
    },
    uglify: {
      options: {
        banner: '<%= banner %>',
      },
      app: {
        src: ['www/dist/app.uncompressed.annotated.js'],
        dest: 'www/dist/app.compressed.js'
      }
    },
    ngAnnotate: {
      app: {
        src: 'www/dist/app.uncompressed.js',
        dest: 'www/dist/app.uncompressed.annotated.js'
      }
    },
    jshint: {
      options: {
        jshintrc: '.jshintrc',
      },
      all: ['www/js/**.js', '!www/js/templates.js']
    }
  });

  grunt.loadNpmTasks('grunt-angular-templates');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-ng-annotate');
  grunt.loadNpmTasks('grunt-contrib-jshint');

  grunt.registerTask('default', ['ngtemplates', 'concat', 'ngAnnotate', 'uglify']);
  grunt.registerTask('lint', ['jshint']);
};
