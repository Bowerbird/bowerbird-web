/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingNoteSubFormView
// -----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'models/identification', 'views/sightingdetailsview', 'views/identificationformview', 'sightingnotedescriptions', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog', 'tipsy', 'tagging'],
function ($, _, Backbone, app, ich, Identification, SightingDetailsView, IdentificationFormView, sightingNoteDescriptions, moment) {

    var SightingNoteSubFormView = Backbone.Marionette.ItemView.extend({

        template: 'SightingNoteSubForm',

        events: {
            'click .sub-menu.add-description-type-button': 'showDescriptionTypes',
            'click .add-description-type-button li a': 'addDescription',
            'click #add-identification-button': '_showIdentificationForm',
            'change .description-fields textarea': '_descriptionChanged',
            'change #Tags': '_onTagsChanged'
        },

        initialize: function (options) {
            _.bindAll(this, 'showDescriptionTypes', 'addDescription', '_onIdentificationDone', '_onTagsChanged', 'showIdentification');

            this.categorySelectList = options.categorySelectList;
            this.categories = options.categories;
            this.descriptionTypesSelectList = options.descriptionTypesSelectList;
            this.sighting = options.sighting;

            this.model.on('change:Taxonomy', this.showIdentification);
        },

        serializeData: function () {
            return {
                Model: {
                    SightingNote: this.model.toJSON()
                    //Sighting: this.sighting
                    //                    CategorySelectList: this.categorySelectList,
                    //                    ProjectsSelectList: this.projectsSelectList
                }
            };
        },

        onShow: function () {
            this._showDetails();
            return this;
        },

//        onRender: function () {
//            this._showDetails();
//            return this;
//        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.$el.find("#Tags").tagit({
                animate: false,
                singleField: true,
                caseSensitive: false,
                onTagAdded: this._onTagsChanged,
                onTagRemoved: this._onTagsChanged
            });

            var that = this;
            this.$el.find('.tagit input').focus(function () {
                that.$el.find('.tagit').tipsy('show');
                that.$el.find('.tagit').addClass('field-focus');
            });
            this.$el.find('.tagit input').blur(function () {
                that.$el.find('.tagit').tipsy('hide');
                that.$el.find('.tagit').removeClass('field-focus');
            });

            this.$el.find('.add-description-type-button li a').tipsy({ gravity: 'w', live: true });
            this.$el.find('.tagit').attr('title', 'Add tags to the sighting. For multi-word tags, enclose the words in double quotes.').tipsy({ gravity: 'w', trigger: 'manual' });

            this.$el.find('.description-fields textarea').tipsy({ trigger: 'focus', gravity: 'w' });

            _.each(this.model.get('Descriptions'), function (item) {
                log(item);
                this.$el.find('li a[data-descriptionid="' + item.Key + '"]').parent().empty();
            }, this);

            // TODO: Filter out already added description fields from list button
        },

        _showIdentificationForm: function (e) {
            e.preventDefault();
            //            if (this.model.get('Category') !== '') {
            //                var that = this;
            //                $.ajax({
            //                    url: '/species?query=' + _.find(that.categories, function (item) { return item.Name === this.model.get('Category'); }, that).Taxonomy + '&field=taxonomy'
            //                }).done(function (data) {
            //                    that._renderIdentificationForm(data.Model.Species.PagedListItems[0]);
            //                });
            //            } else {
            this._renderIdentificationForm(this.model.identification || new Identification());
            //}
        },

        _renderIdentificationForm: function (identification) {
            $('body').append('<div id="modal-dialog"></div>');
            this.identificationFormView = new IdentificationFormView({ el: $('#modal-dialog'), categories: this.categories, categorySelectList: this.categorySelectList, model: identification });
            this.identificationFormView.on('identificationdone', this._onIdentificationDone, this);

            //$('#Category').multiSelectOptionsHide();

            this.identificationFormView.render();
        },

        _onIdentificationDone: function (identification) {
            log('identification done', identification);

            this.model.setIdentification(identification);

            //            this.identification = identification;

            //            this.$el.find('#Identification').html(ich.Identification(identification.toJSON()));

            //            this.model.set('Taxonomy', identification.get('Taxonomy'));
        },

        showIdentification: function () {
            if (this.model.identification) {
                this.$el.find('#Identification').html(ich.Identification(this.model.identification.toJSON()));
            } else {
                this.$el.find('#Identification').html('<span class="identification-none">Not Identified</span>');
            }
        },

        showDescriptionTypes: function (e) {
            e.preventDefault();
            $('.sub-menu').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        addDescription: function (e) {
            e.preventDefault();
            var descriptionId = $(e.currentTarget).attr('data-descriptionid');

            var descriptionType = sightingNoteDescriptions.get(descriptionId);

            $(e.currentTarget).remove();

            this.$el.find('.description-fields').append(ich.SightingNoteDescription({ Key: descriptionType.id, Value: '', Label: descriptionType.name, Description: descriptionType.description }));
            this.$el.find('#description-' + descriptionType.id).tipsy({ trigger: 'focus', gravity: 'w' });
            $('.sub-menu').removeClass('active');

            this.$el.find('.add-description-type-button li a').tipsy.revalidate();

            // TODO: Remove group when there are not more options in it

            this.model.addDescription(descriptionType.id);

            e.stopPropagation();
        },

        _onTagsChanged: function (e, tag) {
            //            log('tags changed', e.target.value, tag);
            //            this.model.set('Tags', e.target.value);
            this.model.set('Tags', $(e.target).val());
        },

        _descriptionChanged: function (e) {
            log('desc', $(e.currentTarget).val());

            var elem = $(e.currentTarget);
            var id = elem.attr('data-descriptionid');
            this.model.setDescription(id, elem.val());
        }
    });

    return SightingNoteSubFormView;

});