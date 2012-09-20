/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// IdentificationFormView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'jsonp'],
function ($, _, Backbone, app, ich) {

    var IdentificationFormView = Backbone.Marionette.ItemView.extend({
        id: 'identification-form',

        template: 'IdentificationForm',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .add-button': '_add'
        },

        serializeData: function () {
            return {
                //Model: this.provider.getJSON()
            };
        },

        initialize: function (options) {
            //            _.bindAll(this, '_loadVideo', '_onGetYouTubeVideo', '_onGetVimeoVideo', '_onGetVideoError', '_updateVideoStatus');

            //            if (options.videoProviderName === 'youtube') {
            //                this.provider = new YouTubeVideoProvider({ onGetVideoSuccess: this._onGetYouTubeVideo, onGetVideoError: this._onGetVideoError });
            //            } else if (options.videoProviderName === 'vimeo') {
            //                this.provider = new VimeoVideoProvider({ onGetVideoSuccess: this._onGetVimeoVideo, onGetVideoError: this._onGetVideoError });
            //            }
        },

        onRender: function () {
            //            var that = this;
            //            this.$el.find('#VideoUri').on('change keyup', function (e) {
            //                that._loadVideo($(this).val());
            //            });
            //            this.$el.find('#VideoUri').on('paste', function (e) {
            //                setTimeout(function () {
            //                    that._loadVideo(that.$el.find('#VideoUri').val());
            //                }, 100);
            //            });

            $("#SearchIdentification").autocomplete({
                source: function (request, onComplete) {
                    log('stuff', request, onComplete);


                    //var deferred = new $.Deferred();

                    $.ajax({
                        url: '/species?query=' + request.term
                    }).done(function (data) {
                        log('ddddd', data);
                        //deferred.resolve(data.Model);
                        //[ { label: "Choice1", value: "value1" }, ... ]
                        onComplete([]);
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    log(ui.item ?
					"Selected: " + ui.item.value + " aka " + ui.item.id :
					"Nothing selected, input was " + this.value);
                }
            });

            return this;
        },

        _cancel: function () {
            this.remove();
        },

        _add: function () {
            //if (this.videoId !== '') {
            this.trigger('identificationdone');
            this.remove();
            //}
        }
    });

    return IdentificationFormView;
});