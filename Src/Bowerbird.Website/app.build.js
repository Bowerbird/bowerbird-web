//({
//    appDir: "C:/Projects/bowerbird.web/Src/Bowerbird.Website/",
//    baseUrl: '/js',
//    paths: {
//        jquery: '/libs/jquery/jquery-1.7.2-min', // jQuery is now AMD compliant
//        json2: '/libs/json/json2',
//        underscore: '/libs/underscore/underscore-min',
//        backbone: '/libs/backbone/backbone-min',
//        marionette: '/libs/backbone.marionette/backbone.marionette-min',
//        text: '/libs/require/text',
//        noext: '/libs/require/noext',
//        async: '/libs/require/async',
//        goog: '/libs/require/goog',
//        propertyParser: '/libs/require/propertyparser',
//        ich: '/libs/icanhaz/icanhaz-min',
//        jqueryui: '/libs/jqueryui',
//        datepicker: '/libs/bootstrap/bootstrap-datepicker',
//        date: '/libs/date/date-min',
//        multiselect: '/libs/jquery.multiselect/jquery.multiselect',
//        loadimage: '/libs/jquery.fileupload/load-image',
//        fileupload: '/libs/jquery.fileupload/jquery.fileupload',
//        signalr: '/libs/jquery.signalr/jquery.signalr-min',
//        timeago: '/libs/jquery.timeago/jquery.timeago'
//    },
//    dir: "js-build",
//    locale: "en-us",
//    optimize: "uglify",
//    uglify: {
//        toplevel: true,
//        ascii_only: true,
//        beautify: false,
//        max_line_length: 1000
//    },
//    inlineText: true,
//    useStrict: false,
//    findNestedDependencies: true
//    //out: "bowerbird-built.js"
//})

({
    appDir: "js/",
    baseUrl: "bowerbird",
    paths: {
        jquery: '/libs/jquery/jquery-1.7.2-min', // jQuery is now AMD compliant
        json2: '/libs/json/json2',
        underscore: '/libs/underscore/underscore-min',
        backbone: '/libs/backbone/backbone-min',
        marionette: '/libs/backbone.marionette/backbone.marionette-min',
        text: '/libs/require/text',
        noext: '/libs/require/noext',
        async: '/libs/require/async',
        goog: '/libs/require/goog',
        propertyParser: '/libs/require/propertyparser',
        ich: '/libs/icanhaz/icanhaz-min',
        jqueryui: '/libs/jqueryui',
        datepicker: '/libs/bootstrap/bootstrap-datepicker',
        date: '/libs/date/date-min',
        multiselect: '/libs/jquery.multiselect/jquery.multiselect',
        loadimage: '/libs/jquery.fileupload/load-image',
        fileupload: '/libs/jquery.fileupload/jquery.fileupload',
        signalr: '/libs/jquery.signalr/jquery.signalr-min',
        timeago: '/libs/jquery.timeago/jquery.timeago'
    },
    dir: "js-build",
    locale: "en-us",
    optimize: "uglify",
    uglify: {
        toplevel: true,
        ascii_only: true,
        beautify: false,
        max_line_length: 1000
    },
    inlineText: true,
    useStrict: false,
    findNestedDependencies: true
    //out: "bowerbird-built.js"
})