/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.FooterView
// --------------------------

// The app's footer
Bowerbird.Views.FooterView = (function (app, Bowerbird, Backbone, $, _) {

    var FooterView = Backbone.Marionette.ItemView.extend({
        el: $('footer')
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function () {
        // Render the layout and get it on the screen, first
        var footerView = new FooterView();

        footerView.on('show', function () {
            Bowerbird.app.vent.trigger('footerView:rendered');
        });

        Bowerbird.app.footer.attachView(footerView);
    });

    return FooterView;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);