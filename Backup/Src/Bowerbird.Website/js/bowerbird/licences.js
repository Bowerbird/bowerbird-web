/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../libs/jquery.signalr/jquery-1.6.2-vsdoc.js" />

// Licences
// --------

// Bowerbird licences
define(['underscore'], function (_) {

    var Licences = {};

    var list = [
        {
            Name: 'All Rights Reserved',
            Id: ' ',
            Description: 'The original creator/s of the work are recognized as the legal owner/s when it comes to fair use of the work by others. The creator holds all of the legal rights to the work unless or until they decide to grant some of those rights to others.',
            Icons: ['/img/copyright.svg']
        },
        {
            Name: 'Attribution',
            Id: 'BY',
            Description: 'This licence lets others distribute, remix and build upon a work, even commercially, as long as they credit the original creator/s (and any other nominated parties). This is the most accommodating of the licences in terms of what others can do with the work.',
            DeedUri: 'http://creativecommons.org/licenses/by/3.0/au',
            LegalCodeUri: 'http://creativecommons.org/licenses/by/3.0/au/legalcode',
            Icons: ['/img/cc.svg', '/img/by.svg']
        },
        {
            Name: 'Attribution-Share Alike',
            Id: 'BY-SA',
            Description: 'This licence lets others distribute, remix and build upon the work, even for commercial purposes, as long as they credit the original creator/s (and any other nominated parties) and license any new creations based on the work under the same terms. All new derivative works will carry the same licence, so will also allow commercial use.<br />' +
            'In other words, you agree to share your materials with others, if they will share their new works in return. This licence is often compared to the free software licences, known as ‘copyleft.’',
            DeedUri: 'http://creativecommons.org/licenses/by-sa/3.0/au',
            LegalCodeUri: 'http://creativecommons.org/licenses/by-sa/3.0/au/legalcode',
            Icons: ['/img/cc.svg', '/img/by.svg', '/img/sa.svg']
        },
        {
            Name: 'Attribution-No Derivative Works',
            Id: 'BY-ND',
            Description: 'This licence allows others to distribute the work, even for commercial purposes, as long as the work is unchanged, and the original creator/s (and any other nominated parties) are credited.',
            DeedUri: 'http://creativecommons.org/licenses/by-nd/3.0/au',
            LegalCodeUri: 'http://creativecommons.org/licenses/by-nd/3.0/au/legalcode',
            Icons: ['/img/cc.svg', '/img/by.svg', '/img/nd.svg']
        },
        {
            Name: 'Attribution-Noncommercial',
            Id: 'BY-NC',
            Description: 'This licence lets others distribute, remix and build upon the work, but only if it is for non-commercial purposes and they credit the original creator/s (and any other nominated parties). They don’t have to license their derivative works on the same terms.',
            DeedUri: 'http://creativecommons.org/licenses/by-nc/3.0/au',
            LegalCodeUri: 'http://creativecommons.org/licenses/by-nc/3.0/au/legalcode',
            Icons: ['/img/cc.svg', '/img/by.svg', '/img/nc.svg']
        },
        {
            Name: 'Attribution-Noncommercial-Share Alike',
            Id: 'BY-NC-SA',
            Description: 'This licence lets others distribute, remix and build upon the work, but only if it is for non-commercial purposes, they credit the original creator/s (and any other nominated parties) and they license their derivative works under the same terms.',
            DeedUri: 'http://creativecommons.org/licenses/by-nc-sa/3.0/au',
            LegalCodeUri: 'http://creativecommons.org/licenses/by-nc-sa/3.0/au/legalcode',
            Icons: ['/img/cc.svg', '/img/by.svg', '/img/nc.svg', '/img/sa.svg']
        },
        {
            Name: 'Attribution-Noncommercial-No Derivatives',
            Id: 'BY-NC-ND',
            Description: 'This licence is the most restrictive of the six main licences, allowing redistribution of the work in its current form only. This licence is often called the ‘free advertising’ licence because it allows others to download and share the work as long as they credit the original creator/s (and any other nominated parties), they don’t change the material in any way and they don’t use it commercially.',
            DeedUri: 'http://creativecommons.org/licenses/by-nc-nd/3.0/au',
            LegalCodeUri: 'http://creativecommons.org/licenses/by-nc-nd/3.0/au/legalcode',
            Icons: ['/img/cc.svg', '/img/by.svg', '/img/nc.svg', '/img/nd.svg']
        }
    ];

    Licences.get = function (id) {
        return _.find(list, function (item) {
            return item.Id === id;
        },
        this);
    };

    Licences.getAll = function () {
        return list;
    };

    return Licences;

});