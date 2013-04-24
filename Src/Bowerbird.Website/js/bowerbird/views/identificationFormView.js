/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// IdentificationFormView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'models/identification', 'jsonp'],
function ($, _, Backbone, app, ich, Identification) {

    var IdentificationFormView = Backbone.Marionette.ItemView.extend({
        id: 'identification-form',

        template: 'IdentificationWindow',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .done-button': '_done',
            'click .taxonomic-rank li': '_browseSpeciesRank'
        },

        serializeData: function () {
            return {
                Model: {
                    CategorySelectList: this.categorySelectList,
                    Identification: this.model.toJSON()
                }
            };
        },

        initialize: function (options) {
            _.bindAll(this, '_displaySelectedId');

            this.categories = options.categories;
            this.categorySelectList = options.categorySelectList;

            this.model.on('change:Taxonomy', this._displaySelectedId);
        },

        searchField: '',

        searchCategory: '',

        onRender: function () {
            var that = this;

            $('body').css('overflow-y', 'hidden');

            $.ajax({
                url: '/species?query=' + this.model.get('Taxonomy') + '&field=allranks&pagesize=50'
            }).done(function (data) {
                var rankNames = [];
                if (that.model.get('Taxonomy') != null) {
                    rankNames = that.model.get('Taxonomy').split(':');
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

            this.$el.find('#SearchIdentification').keypress(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                }
            });

            //this._displaySelectedId();

            this.searchCategories = this.$el.find('#SearchCategories').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                noneSelected: function () {
                    var $selectedHtml = $('<span>All Categories</span>');
                    that.searchCategory = '';
                    return $selectedHtml;
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                        that.searchCategory = option.value === 'all' ? '' : option.value;
                    });

                    return $selectedHtml.children();
                }
            });

            this.searchFields = this.$el.find('#SearchFields').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                noneSelected: function () {
                    var $selectedHtml = $('<span>All Names</span>');
                    that.searchField = '';
                    return $selectedHtml;
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                        that.searchField = option.value === 'all' ? '' : option.value;
                    });

                    return $selectedHtml.children();
                }
            });

            this.$el.find('#SearchIdentification').autocomplete({
                source: function (request, response) {
                    that.$el.find('#search-identification-field .progress-indicator').show();
                    var url = '/species?query=' + request.term + '&limitcommonnames=true';
                    if (that.searchField != '') {
                        url += '&field=' + that.searchField;
                    }
                    if (that.searchCategory != '') {
                        url += '&category=' + encodeURIComponent(that.searchCategory);
                    }
                    url += '&pagesize=50';

                    $.ajax({
                        url: url
                    }).done(function (data) {
                        log('requesting search taxon', data);
                        var model = _.map(data.Model.Species.PagedListItems, function (item) {
                            return {
                                value: item.Taxonomy,
                                label: item.Name,
                                taxon: item
                            };
                        });
                        response(model);
                        that.$el.find('#search-identification-field .progress-indicator').hide();
                    });
                },
                minLength: 1,
                focus: function (event, ui) {
                    that.$el.find('#SearchIdentification').val(ui.item.taxon.Name);
                    return false;
                },
                select: function (event, ui) {
                    that.$el.find('#SearchIdentification').val(ui.item.taxon.Name);
                    that._loadSelectedId(ui.item.taxon.Taxonomy);
                    return false;
                }
            }).data('autocomplete')._renderItem = function (ul, item) {
                if (item.taxon.Category != null) {
                    var model = {
                        Model: {
                            Category: item.taxon.Category.toLowerCase(),
                            RankType: item.taxon.RankType
                        }
                    };

                    model.Model.NameHtml = '<span class="id-part">' + that._highlightText(this.term, item.taxon.Name) + '</span>';
                    model.Model.TaxonomyHtml = _.map(item.taxon.Ranks, function (x) {
                        return '<span class="id-part">' + that._highlightText(this.term, x.Name) + '</span>';
                    }, this).join(': ');
                    var allCommonNames = _.union(item.taxon.CommonGroupNames, item.taxon.CommonNames);
                    if ((that.searchField === '' || that.searchField === 'common') && allCommonNames.length > 0) {
                        model.Model.AllCommonNamesHtml = allCommonNames.map(function (x) {
                            return '<span class="id-part">' + that._highlightText(this.term, x) + '</span>';
                        }, this).join(', ');
                    }
                    if (item.taxon.Synonyms.length > 0) {
                        model.Model.SynonymsHtml = _.map(item.taxon.Synonyms, function (x) {
                            return '<span class="id-part">' + that._highlightText(this.term, x) + '</span>';
                        }, this).join(', ');
                    }

                    return $(ich.IdentificationListItem(model))
                        .data('item.autocomplete', item)
                        .appendTo(ul);
                }
                return null;
            };

            return this;
        },

        _highlightText: function (searchValue, value) {
            var html = value;

            if (value.toLowerCase().indexOf(searchValue.toLowerCase(), 0) === 0) {
                var valueToWrap = html.substr(0, searchValue.length);
                html = '<span class="found-text">' + valueToWrap + '</span>' + html.substring(searchValue.length);
            }

            return html;
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

            // Set selected identification, if it contains a category
            if (rank.Category != null) {
                this._loadSelectedId(rank.Taxonomy);
            } else {
                this.model.clearId();
            }

            this.$el.find('#TaxonomicRank' + parseInt(rank.RankPosition, 10) + ' li').removeClass('selected');
            elem.addClass('selected');

            if (rankPosition < 8) {
                this._requestTaxa('/species?query=' + rank.RankName + '&field=rank' + rankPosition + '&pagesize=50', rankPosition);
            }

            return false;
        },

        _loadSelectedId: function (taxonomy) {
            var that = this;
            $.ajax({
                url: '/species?query=' + taxonomy + '&field=taxonomy'
            }).done(function (data) {
                log('requesting selected taxon', data);

                var item = data.Model.Species.PagedListItems[0];

                var data = {
                    IsCustomIdentification: false,
                    Taxonomy: item.Taxonomy,
                    Category: item.Category,
                    Name: item.Name,
                    RankType: item.RankType,
                    AllCommonNames: item.AllCommonNames
                };

                that.model.set(data);
            });
        },

        _displaySelectedId: function () {
            if (this.model.hasTaxonomy()) {
                this.$el.find('#Identification').empty().html(ich.Identification(this.model.toJSON()));
                this.$el.find('.done-button').removeAttr('disabled');
            } else {
                this.$el.find('#Identification').empty().html('<span class="identification-none">Not Identified</span>');
                this.$el.find('.done-button').attr('disabled', 'disabled');
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

        _appendTaxaToList: function (item, $list, selected) {
            var selectedHtml = selected ? ' class="selected"' : '';
            var speciesCountHtml = '';

            if (item.RankType !== 'species' || (item.RankType === 'species' && item.SpeciesCount !== 1)) {
                var speciesCountHtml = ' <span class="species-count">(' + item.SpeciesCount + ')</span>';
            }

            $('<li' + selectedHtml + '>' + item.RankName + speciesCountHtml + '</li>').data('item.rank', item).appendTo($list);
        },

        _cancel: function () {
            $('body').css('overflow-y', 'scroll');
            this.remove();
        },

        _done: function () {
            $('body').css('overflow-y', 'scroll');
            this.trigger('identificationdone', this.model);
            this.remove();
        }
    });

    return IdentificationFormView;
});