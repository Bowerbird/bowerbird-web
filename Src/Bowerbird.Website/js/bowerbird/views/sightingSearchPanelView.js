/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingSearchPanelView
// ------------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'tipsy'],
function ($, _, Backbone, ich, app) {

    var SightingSearchPanelView = Backbone.Marionette.ItemView.extend({
        template: 'SightingSearchPanel',

        events: {
            'click .taxonomic-rank li': '_browseSpeciesRank',
            'change #Category': 'categoryChanged',
            'change #NeedsId': 'needsIdChanged',
            'click #search-go-button': 'queryChanged',
            'click #new-search-button': 'newSearch'
        },

        initialize: function (options) {
            this.sightingCollection = options.sightingCollection;
            this.categorySelectList = options.categorySelectList;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    CategorySelectList: this.categorySelectList,
                    FieldSelectList: this.fieldSelectList,
                    Query: {
                        Id: this.sightingCollection.projectId,
                        Page: this.sightingCollection.pageSize,
                        PageSize: this.sightingCollection.page,
                        View: this.sightingCollection.viewType,
                        Sort: this.sightingCollection.sortByType
                    }
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.loadFirstRank();
        },

        loadFirstRank: function () {
            var that = this;

            $.ajax({
                url: '/species?query=' + this.sightingCollection.taxonomy + '&field=allranks&pagesize=50'
            }).done(function (data) {
                var rankNames = [];
                if (that.sightingCollection.taxonomy !== '') {
                    rankNames = that.sightingCollection.taxonomy.split(':');
                }

                for (var rank = 0; rank < data.Model.Species.length; rank++) {
                    var $list = $('<ul></ul>');

                    for (var x = 0; x < data.Model.Species[rank].PagedListItems.length; x++) {
                        var selected = false;
                        if (rankNames.length >= rank) {
                            selected = $.trim(rankNames[rank]) === data.Model.Species[rank].PagedListItems[x].RankName;
                        }
                        that._appendTaxaToList(data.Model.Species[rank].PagedListItems[x], $list, selected);
                    }

                    that.$el.find('#TaxonomicRank' + (rank + 1)).append($list);
                }
            });
        },

        _browseSpeciesRank: function (e) {
            e.preventDefault();

            var elem = $(e.target).closest('li');

            var rank = elem.data('item.rank');

            var rankPosition = parseInt(rank.RankPosition, 10) + 1;

            for (var y = rankPosition; y <= 8; y++) {
                this.$el.find('#TaxonomicRank' + y).empty();
            }

            this.$el.find('#TaxonomicRank' + rankPosition).append('<img class="progress-indicator" src="/img/loader-small.gif" alt="" /> ');
            this.$el.find('#TaxonomicRank' + rankPosition + ' .progress-indicator').show();

            this.$el.find('#TaxonomicRank' + parseInt(rank.RankPosition, 10) + ' li').removeClass('selected');
            elem.addClass('selected');

            if (rankPosition < 8) {
                this._requestTaxa('/species?query=' + rank.RankName + '&field=rank' + rankPosition + '&pagesize=50', rankPosition);
            }

            this.sightingCollection.changeTaxonomy(rank.Taxonomy);
            Backbone.history.navigate(this.sightingCollection.searchUrl(), { trigger: false });

            return false;
        },

        _appendTaxaToList: function (item, $list, selected) {
            if (item.SightingCount > 0) {
                var selectedHtml = selected ? ' class="selected"' : '';
                var speciesCountHtml = '';

                //if (item.RankType !== 'species' || (item.RankType === 'species' && item.SightingCount !== 1)) {
                speciesCountHtml = ' <span class="species-count">(' + item.SightingCount + ')</span>';
                //}

                $('<li' + selectedHtml + '><div>' + item.RankName + speciesCountHtml + '</div></li>').data('item.rank', item).appendTo($list);
            }
        },

        _requestTaxa: function (uri, rankPosition) {
            var that = this;
            $.ajax({
                url: uri
            }).done(function (data) {
                var $list = $('<ul></ul>');

                for (var x = 0; x < data.Model.Species.PagedListItems.length; x++) {
                    that._appendTaxaToList(data.Model.Species.PagedListItems[x], $list, false);
                }

                that.$el.find('#TaxonomicRank' + rankPosition).empty().append($list);
            });
        },

        categoryChanged: function (e) {
            this.sightingCollection.changeCategory($(e.currentTarget).val());

            Backbone.history.navigate(this.sightingCollection.searchUrl(), { trigger: false });
        },

        needsIdChanged: function (e) {
            this.sightingCollection.changeNeedsId($(e.currentTarget).is(':checked'));

            Backbone.history.navigate(this.sightingCollection.searchUrl(), { trigger: false });
        },

        queryChanged: function (e) {
            e.preventDefault();
            this.sightingCollection.changeQuery(this.$el.find('#Query').val(), this.$el.find('#Field').val());

            Backbone.history.navigate(this.sightingCollection.searchUrl(), { trigger: false });
            return false;
        },

        newSearch: function (e) {
            e.preventDefault();

            this.sightingCollection.resetSearch();

            Backbone.history.navigate(this.sightingCollection.searchUrl(), { trigger: false });

            for (var y = 1; y <= 8; y++) {
                this.$el.find('#TaxonomicRank' + y).empty();
            }

            this.$el.find('#Category').val('');
            this.$el.find('#Query').val('');
            this.$el.find('#Field').val('');
            this.$el.find('#NeedsId').prop('checked', false);

            this.loadFirstRank();

            return false;
        }

    });

    return SightingSearchPanelView;

});