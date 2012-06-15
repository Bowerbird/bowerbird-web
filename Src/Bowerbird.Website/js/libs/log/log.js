define(function () {

    // usage: log('inside coolFunc', this, arguments);
    // paulirish.com/2009/log-a-lightweight-wrapper-for-consolelog/
    var logger = function () {
        log.history = log.history || [];   // store logs to an array for reference
        log.history.push(arguments);
        if (this.console) console.log(Array.prototype.slice.call(arguments));
    };

    window.log = logger;

    return logger;

});