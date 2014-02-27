/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePublicView
// --------------

// The home page view when not logged in
define(['jquery', 'underscore', 'backbone', 'app', 'carousel', 'touchswipe'], function ($, _, Backbone, app) {

    var HomePublicView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'home-public single',

        template: 'HomePublicIndex',

        events: {
            'click .home-how a': 'showItem'
        },

        initialize: function () {
            _.bindAll(this, 'showHowWhyItem');
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            var that = this;

            this.whyCarousel = $(".home-why .carousel ul").carouFredSel({
                width: "100%",
                height: '27em',
                items: {
                    visible: 1,
                    height: '27em'
                },
                scroll: {
                    fx: "crossfade"
                },
                auto: false,
                prev: {
                    button: ".home-why .carousel .previous-button",
                    key: 90,
                    onBefore: function (data) {
                        that.showHowWhyItem(data);
                    }
                },
                next: {
                    button: ".home-why .carousel .next-button",
                    key: 88,
                    onBefore: function (data) {
                        that.showHowWhyItem(data);
                    }
                },
                pagination: {
                    container: ".home-why .carousel .pagination",
                    keys: true,
                    anchorBuilder: function (nr) {
                        return '<div class="pager button"></div>';
                    }
                },
                swipe: true
            });

            this.howCarousel = $(".home-how .carousel ul").carouFredSel({
                width: '100%',
                items: {
                    width: 110,
                    visible: {
                        min: 11,
                        max: 15
                    }
                },
                auto: {
                    items: 11,
                    duration: 35000,
                    easing: "linear",
                    timeoutDuration: 0,
                    pauseOnHover: "immediate"
                }
            });

        },

        showHowWhyItem: function (data) {
            this.$el.find('.home-why .current-caption').fadeOut(function () {
                $(this).empty().html($(data.items['new']).find('.caption').clone()).fadeIn();
            });
        },

        showItem: function (e) {
            e.preventDefault();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
            return false;
        },

        beforeClose: function () {
            this.whyCarousel.stop(true);
            this.howCarousel.stop(true);
        }
    });

    return HomePublicView;

}); 