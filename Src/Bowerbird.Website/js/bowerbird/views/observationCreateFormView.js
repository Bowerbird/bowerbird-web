
window.Bowerbird.Views.ObservationCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

    id: 'observation-create-form',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        'change input#title': '_contentChanged',
        'change input#observedOn': '_contentChanged',
        'change input#address': '_contentChanged',
        'change input#latitude': '_latLongChanged',
        'change input#longitude': '_latLongChanged',
        'change input#anonymiseLocation': '_anonymiseLocationChanged',
        'change #projects-field input:checkbox': '_projectsChanged',
        'change #category-field input:checkbox': '_categoryChanged',
        'click #media-resource-import-button': '_showImportMedia'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        'start',
        '_showImportMedia',
        '_contentChanged',
        '_latLongChanged',
        '_anonymiseLocationChanged',
        '_projectsChanged',
        '_categoryChanged',
        '_cancel',
        '_save'
        );
        this.observation = options.observation;
        this.editMediaView = new Bowerbird.Views.EditMediaView({ el: $('#media-resources-fieldset'), observation: this.observation });
        this.editMapView = new Bowerbird.Views.EditMapView({ observation: this.observation });
    },

    render: function () {
        if (app.get('prerenderedView') != 'observations/create') {
            this.$el.append(ich.ObservationCreate({ Observation: app.get('newObservation').toJSON() }));
            window.scrollTo(0, 0);
        }
        return this;
    },

    start: function () {
        this.editMediaView.render();
        this.editMapView.render();
        $('#observedOn').datepicker();

        this.categoryListSelectView = $("#category").multiSelect({
            selectAll: false,
            singleSelect: true,
            noneSelected: 'Select Category',
            oneOrMoreSelected: function (selectedOptions) {
                var $selectedHtml = $('<span />');
                _.each(selectedOptions, function (option) {
                    $selectedHtml.append('<span>' + option.text + '</span> ');
                });
                return $selectedHtml;
            }
        });

        // on the fly template... change to mustache.
        $.tmpl('<option value="${id}">${name}</option>', app.projects.toJSONViewModel()).appendTo('#projects');

        this.projectListSelectView = $("#projects").multiSelect({
            selectAll: false,
            noneSelected: 'Select Projects',
            renderOption: function (id, option) {
                var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
                if (option.selected) {
                    html += ' checked="checked"';
                }
                var project = app.projects.get(option.value);
                html += ' /><img src="/img/avatar-1.png" />' + project.get('Name') + '</label>';
                return html;
            },
            oneOrMoreSelected: function (selectedOptions) {
                var $selectedHtml = $('<span />');
                _.each(selectedOptions, function (option) {
                    $selectedHtml.append('<span>' + option.text + '</span> ');
                });
                return $selectedHtml;
            }
        });

        var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
    },

    _showImportMedia: function () {
        alert('Coming soon');
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.observation.set(data);

        if (target.attr('name') === 'address') {
            this._latLongChanged(e);
        }
    },

    _latLongChanged: function (e) {
        var oldPosition = { latitude: this.observation.get('Latitude'), longitude: this.observation.get('Longitude') };
        var newPosition = { latitude: $('#latitude').val(), longitude: $('#longitude').val() };

        this.observation.set('Latitude', newPosition.Latitude);
        this.observation.set('Longitude', newPosition.Longitude);

        // Only update pin if the location is different to avoid infinite loop
        if (newPosition.Latitude != null && newPosition.Longitude != null && (oldPosition.Latitude !== newPosition.Latitude || oldPosition.Longitude !== newPosition.Longitude)) {
            this.editMapView.changeMarkerPosition(this.observation.get('Latitude'), this.observation.get('Longitude'));
        }
    },

    _anonymiseLocationChanged: function (e) {
        var $checkbox = $(e.currentTarget);
        this.observation.set({ AnonymiseLocation: $checkbox.attr('checked') == 'checked' ? true : false });
    },

    _projectsChanged: function (e) {
        var $checkbox = $(e.currentTarget);
        if ($checkbox.attr('checked') === 'checked') {
            var proj = app.projects.get($checkbox.attr('value'));
            this.observation.projects.add(proj);
        } else {
            this.observation.projects.remove($checkbox.attr('value'));
        }
    },

    _categoryChanged: function (e) {
        var $checkbox = $(e.currentTarget);
        if ($checkbox.attr('checked') === 'checked') {
            this.observation.set('Category', $checkbox.attr('value'));
        } else {
            this.observation.set('Category', '');
        }
    },

    _cancel: function () {
        app.set('newObservation', null);
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    },

    _save: function () {
        this.observation.save();
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    }
});