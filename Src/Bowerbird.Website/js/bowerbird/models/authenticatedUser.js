/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AuthenticatedUser
// -----------------

define(['jquery', 'underscore', 'backbone', 'ich', 'bootstrap-data', 'app', 'models/user', 'collections/usercollection',
        'collections/projectcollection', 'collections/organisationcollection', 'collections/userprojectcollection'],
function ($, _, Backbone, ich, bootstrapData, app, User, UserCollection, ProjectCollection, OrganisationCollection, UserProjectCollection) {

    var AuthenticatedUser = function (data) {
        this.user = new User(data.User);
        this.memberships = data.Memberships;
        this.projects = new ProjectCollection(data.Projects, { sortBy: 'a-z' });
        this.organisations = new OrganisationCollection(data.Organisations, { sortBy: 'a-z' });
        this.userProjects = new UserProjectCollection(data.UserProjects, { sortBy: 'a-z' });
        this.appRoot = data.AppRoot;

        this.hasGroupPermission = function (groupId, permissionId) {
            var membership = _.find(this.memberships, function (m) {
                return m.GroupId === groupId;
            });
            if (!membership) {
                return false;
            }
            return _.any(membership.PermissionIds, function (p) {
                return p === permissionId;
            });
        };

        this.hasGroupRole = function (groupId, roleId) {
            var membership = _.find(this.memberships, function (m) {
                return m.GroupId === groupId;
            });
            if (!membership) {
                return false;
            }
            return _.any(membership.RoleIds, function (role) {
                return role === roleId;
            });
        };

        this.defaultLicence = data.DefaultLicence;
        this.callsToAction = data.CallsToAction;
    };

    app.bind('initialize:before', function () {
        if (bootstrapData.Model.AuthenticatedUser) {
            this.authenticatedUser = new AuthenticatedUser(bootstrapData.Model.AuthenticatedUser);
            log('initialising authenticated user', this.authenticatedUser);

            var that = this;
            this.vent.on('newactivity:groupadded', function (activity) {
                var group = activity.get('GroupAdded').Group;
                if (group.GroupType === 'project') {
                    if (group.User.Id == app.authenticatedUser.user.id) {
                        that.authenticatedUser.projects.add(group);
                    }
                }
                if (group.GroupType === 'organisation') {
                    if (group.User.Id == app.authenticatedUser.user.id) {
                        that.authenticatedUser.organisations.add(group);
                    }
                }
                if (group.GroupType === 'userproject') {
                    if (group.User.Id == app.authenticatedUser.user.id) {
                        that.authenticatedUser.userProjects.add(group);
                    }
                }

            }, this);
        }
    });

    return AuthenticatedUser;
});