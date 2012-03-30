
window.Bowerbird.Views.ObservationCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

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

    template: $.template('observationCreateFormTemplate', $('#observation-create-form-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.appView = options.appView;
        this.observation = options.observation;
        this.editMediaView = new Bowerbird.Views.EditMediaView({ el: $('#media-resources-fieldset'), observation: this.observation });
        this.editMapView = new Bowerbird.Views.EditMapView({ observation: this.observation });
    },

    render: function () {
        $.tmpl('observationCreateFormTemplate', app.get('newObservation').toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
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
                html += ' /><img src="/img/avatar-1.png" />' + project.get('name') + '</label>';
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
        var oldposition = { latitude: this.observation.get('latitude'), longitude: this.observation.get('longitude') };
        var newPosition = { latitude: $('#latitude').val(), longitude: $('#longitude').val() };

        this.observation.set('latitude', newPosition.latitude);
        this.observation.set('longitude', newPosition.longitude);

        // Only update pin if the location is different to avoid infinite loop
        if (newPosition.latitude != null && newPosition.longitude != null && (oldposition.latitude !== newPosition.latitude || oldposition.longitude !== newPosition.longitude)) {
            this.editMapView.changeMarkerPosition(this.observation.get('latitude'), this.observation.get('longitude'));
        }
    },

    _anonymiseLocationChanged: function (e) {
        var $checkbox = $(e.currentTarget);
        this.observation.set({ anonymiseLocation: $checkbox.attr('checked') == 'checked' ? true : false });
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
            this.observation.set('category', $checkbox.attr('value'));
        } else {
            this.observation.set('category', '');
        }
    },

    _cancel: function () {
        app.set('newObservation', null);
        this.remove();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    },

    _save: function () {
        this.observation.save();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: false });
        this.trigger('formClosed', this);
    }
});

