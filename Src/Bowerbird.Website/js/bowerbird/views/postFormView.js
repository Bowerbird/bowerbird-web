/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostFormView
// ------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog'],
function ($, _, Backbone, app, ich, moment) {

    var postTypes = [
        {
            Text: 'General news',
            Value: 'news'
        },
        {
            Text: 'Meeting',
            Value: 'meeting'
        },
        {
            Text: 'Newsletter',
            Value: 'newsletter'
        }];
    
    var PostFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form form-medium single post-form',

        template: 'PostForm',

        regions: {
            media: '#media-resources-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Subject': '_contentChanged',
            'change textarea#Message': '_contentChanged',
            'change #posttype-field input:checkbox': '_postTypeChanged'
        },

        initialize: function (options) {
            if (this.model.id) {
                this.viewEditMode = 'update';
            } else {
                this.viewEditMode = 'create';
            }
        },

        serializeData: function () {
            return {
                Model: {
                    Post: this.model.toJSON(),
                    PostTypeSelectList: _.map(postTypes, function(item) {
                        return {
                            Value: item.Value,
                            Text: item.Text,
                            Selected: item.Value === this.model.get('PostType')
                        };
                    }, this),
                    Create: this.viewEditMode === 'create',
                    Update: this.viewEditMode === 'update'
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
//            var postMediaFormView = new PostMediaFormView({ el: '#media-details', model: this.model, collection: this.model.media });
//            this.media.attachView(postMediaFormView);
//            postMediaFormView.render();

            this.postTypeListSelectView = this.$el.find('#PostType').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                noOptionsText: 'No Types',
                noneSelected: '<span class="default-option">Select Type</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            this.$el.find('#Subject, #PostType, #Message').tipsy({ trigger: 'focus', gravity: 'w', fade: true, live: true });
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _postTypeChanged: function (e) {
            var postType = '';
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                postType = $checkbox.attr('value');
            }
            this.model.set('PostType', postType);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
//            if (this.media.currentView.progressCount > 0) {
//                return;
//            }

            this.model.save();
            app.showPreviousContentView();
        }
    });

    return PostFormView;

});