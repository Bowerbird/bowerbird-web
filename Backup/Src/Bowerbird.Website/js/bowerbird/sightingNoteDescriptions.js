/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../libs/jquery.signalr/jquery-1.6.2-vsdoc.js" />

// Sighting Note Descriptions
// --------------------------

// Bowerbird description types
define(['underscore'], function (_) {

    var SightingNoteDescriptions = {};

    var list = [
        {
            id: 'physicaldescription',
            group: 'lookslike',
            name: 'Physical Description',
            description: 'The physical characteristics of the species in the sighting'
        },
        {
            id: 'similarspecies',
            group: 'lookslike',
            name: 'Similar Species',
            description: 'How the species sighting is similar to other species'
        },
        {
            id: 'distribution',
            group: 'wherefound',
            name: 'Distribution',
            description: 'The geographic distribution of the species in the sighting'
        },
        {
            id: 'habitat',
            group: 'wherefound',
            name: 'Habitat',
            description: 'The habitat of the species in the sighting'
        },
        {
            id: 'seasonalvariation',
            group: 'wherefound',
            name: 'Seasonal Variation',
            description: 'Any seasonal variation of the species in the sighting'
        },
        {
            id: 'conservationstatus',
            group: 'wherefound',
            name: 'Conservation Status',
            description: 'The conservation status of the species in the sighting'
        },
        {
            id: 'behaviour',
            group: 'whatitdoes',
            name: 'Behaviour',
            description: 'Any behaviour of a species in the sighting'
        },
        {
            id: 'food',
            group: 'whatitdoes',
            name: 'Food',
            description: 'The feeding chracteristics of the species in the sighting'
        },
        {
            id: 'lifecycle',
            group: 'whatitdoes',
            name: 'Life Cycle',
            description: 'The life cycle stage or breeding charatcertic of the species in the sighting'
        },
        {
            id: 'indigenouscommonnames',
            group: 'cultural',
            name: 'Indigenous Common Names',
            description: 'Any indigenous common names associated with the sighting'
        },
        {
            id: 'indigenoususage',
            group: 'cultural',
            name: 'Usage in Indigenous Culture',
            description: 'Any special usage in indigenous cultures of the species in the sighting'
        },
        {
            id: 'traditionalstories',
            group: 'cultural',
            name: 'Traditional Stories',
            description: 'Any traditional stories associated with the species in the sighting'
        },
        {
            id: 'general',
            group: 'other',
            name: 'General Details',
            description: 'Any other general details'
        }
    ];

    SightingNoteDescriptions.get = function (id) {
        return _.find(list, function (item) {
            return item.id === id;
        },
        this);
    };

    SightingNoteDescriptions.getAll = function () {
        return list;
    };

    SightingNoteDescriptions.getGroup = function (group) {
        return _.find(list, function (item) {
            return item.group === group;
        },
        this);
    };

    return SightingNoteDescriptions;

});