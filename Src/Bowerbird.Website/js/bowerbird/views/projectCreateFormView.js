
window.Bowerbird.Views.ProjectCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        'change input#name': '_contentChanged',
        'change input#description': '_contentChanged',
        'change input#website': '_contentChanged'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        'start',
        '_cancel',
        '_contentChanged',
        '_save'
//        ,
//        '_initMediaUploader',
//        '_onUploadAdd',
//        '_onSubmitUpload',
//        '_onUploadDone'
        );
        this.appView = options.appView;
        this.project = options.project;
        //this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#avatar-uploader'), project: this.project });
        this.editMediaView = new Bowerbird.Views.EditMediaView({ el: $('#media-resources-fieldset'), observation: this.observation });

    },

    render: function () {
        var projectTemplate = ich.projectcreate({ project: app.get('newProject').toJSON() }).appendTo(this.$el);
 //       this.$el.append(projectTemplate);
  //      var avatarUploader = ich.avataruploader();
//        this.$el.find('#avatar-uploader').append(avatarUploader);
 //       this._initMediaUploader();
        return this;
    },

    start: function () {
        //var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
    },

    _cancel: function () {
        app.set('newProject', null);
        this.remove();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.project.set(data);
    },

    _save: function () {
        //alert('Coming soon');
        this.project.save();
        //this.remove();
        //app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    }
//    ,

//    _initMediaUploader: function () {
//        log('projectCreateFormView._initMediaUploader');

//        $('#fileupload').fileupload({
//            dataType: 'json',
//            paramName: 'file',
//            url: '/members/mediaresource/avatarupload',
//            add: this._onUploadAdd,
//            submit: this._onSubmitUpload,
//            done: this._onUploadDone,
//            limitMultiFileUploads: 1
//        });

//        log('finish editAvatarView._initMediaUploader');
//    },

//    _onUploadAdd: function (e, data) {
//        log('projectCreateFormView._onUploadAdd');
//        alert('added');
//        data.submit();
//    },

//    _onSubmitUpload: function (e, data) {
//        log('projectCreateFormView._onSubmitUpload');

//        data.formData = { originalFileName: data.files[0].name };
//    },

//    _onUploadDone: function (e, data) {
//        log('projectCreateFormView._onUploadDone');

//        var uploadedAvatar = new 
//        {
//            id: data.id,
//            url: data.mediumImageUrl,
//            altTag: ''
//        };

//        this.project.set('avatar', uploadedAvatar);
//    }
});