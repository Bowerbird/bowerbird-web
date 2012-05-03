/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// FooterView
// ----------

// The app's footer
define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var FooterView = Backbone.Marionette.ItemView.extend({
        el: 'footer'
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function (options) {
        // Render the layout and get it on the screen, first
        var footerView = new FooterView();

        footerView.on('show', function () {
            Bowerbird.app.vent.trigger('footerView:rendered');
        });

        Bowerbird.app.footer.attachView(footerView);
        footerView.render();
    });

    return FooterView;

});