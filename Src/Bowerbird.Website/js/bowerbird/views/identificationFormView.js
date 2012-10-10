/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// IdentificationFormView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'jsonp'],
function ($, _, Backbone, app, ich) {

    var Identification = Backbone.Model.extend({
        defaults: {
            HasCategory: false,
            Taxonomy: ''
        },

        initialize: function (attributes) {
            if (attributes && attributes.Name) {
                var allCommonNames = _.union(attributes.CommonGroupNames, attributes.CommonNames);

                this.set('HasCategory', attributes.Category != null);
                this.set('Category', attributes.Category != null ? attributes.Category : '' != null);
                this.set('Name', attributes.Name);
                this.set('RankType', attributes.RankType);
                this.set('Taxonomy', attributes.Taxonomy);
                this.set('HasCommonNames', allCommonNames.length > 0);
                this.set('CommonNames', allCommonNames.join(', '));
            }
        }
    });

    var IdentificationFormView = Backbone.Marionette.ItemView.extend({
        id: 'identification-form',

        template: 'IdentificationForm',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .done-button': '_done',
            'click .taxonomic-rank li': '_browseSpeciesRank'
        },

        serializeData: function () {
            log(this.identification);
            return {
                Model: {
                    CategorySelectList: this.categorySelectList,
                    Identification: this.identification.toJSON()
                }
            };
        },

        initialize: function (options) {
            this.categories = options.categories;
            this.categorySelectList = options.categorySelectList;
            this.identification = new Identification(options.identification);
        },

        searchField: '',

        searchCategory: '',

        onRender: function () {
            var that = this;

            $.ajax({
                url: '/species?query=' + this.identification.get('Taxonomy') + '&field=allranks&pagesize=50'
            }).done(function (data) {
                var rankNames = that.identification.get('Taxonomy').split(':');

                for (var rank = 0; rank < data.Model.Species.length; rank++) {
                    var $list = $('<ul></ul>');

                    for (var x = 0; x < data.Model.Species[rank].PagedListItems.length; x++) {
                        var selected = '';

                        if ($.trim(rankNames[rank]) === data.Model.Species[rank].PagedListItems[x].RankName) {
                            selected = ' class="selected"';
                        }

                        $('<li' + selected + '>' + data.Model.Species[rank].PagedListItems[x].RankName + '</li>').data('item.rank', data.Model.Species[rank].PagedListItems[x]).appendTo($list);
                    }

                    that.$el.find('#TaxonomicRank' + (rank + 1)).append($list);
                }
            });

            this.$el.find('#SearchIdentification').keypress(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                }
            });


            this._displaySelectedId(this.identification.get('HasCategory'), this.identification.get('Taxonomy'));

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
                    var url = '/species?query=' + request.term;
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
                    that._displaySelectedId(true, ui.item.taxon.Taxonomy);
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

            var rank = $(e.target).data('item.rank');
            var rankPosition = parseInt(rank.RankPosition, 10) + 1;

            for (var y = rankPosition; y <= 8; y++) {
                this.$el.find('#TaxonomicRank' + y).empty();
            }

            this.$el.find('#TaxonomicRank' + rankPosition).append('<img class="progress-indicator" src="/img/loaderx.gif" alt="" /> ');
            this.$el.find('#TaxonomicRank' + rankPosition + ' .progress-indicator').show();

            // Set selected identification, if it contains a category
            this._displaySelectedId(rank.Category != null, rank.Taxonomy);

            this.$el.find('#TaxonomicRank' + parseInt(rank.RankPosition, 10) + ' li').removeClass('selected');
            $(e.target).addClass('selected');

            if (rankPosition < 8) {
                this._requestTaxa('/species?query=' + rank.RankName + '&field=rank' + rankPosition + '&pagesize=50', rankPosition);
            }

            return false;
        },

        _displaySelectedId: function (isValidId, taxonomy) {
            if (isValidId) {
                var that = this;
                $.ajax({
                    url: '/species?query=' + taxonomy + '&field=taxonomy'
                }).done(function (data) {
                    log('requesting selected taxon', data);

                    that.identification = new Identification(data.Model.Species.PagedListItems[0]);

                    var model = {
                        Model: {
                            Identification: that.identification.toJSON()
                        }
                    };

                    that.$el.find('#Identification').empty().html(ich.Identification(model));

                    that.$el.find('.done-button').removeAttr('disabled');
                });
            } else {
                this.identification = new Identification();

                var model = {
                    Model: {
                        Identification: this.identification.toJSON()
                    }
                };

                this.$el.find('#Identification').empty().html(ich.Identification(model));

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
                    $('<li>' + data.Model.Species.PagedListItems[x].RankName + '</li>').data('item.rank', data.Model.Species.PagedListItems[x]).appendTo($list);
                }

                that.$el.find('#TaxonomicRank' + rankPosition).empty().append($list);
            });
        },

        _cancel: function () {
            this.remove();
        },

        _done: function () {
            this.trigger('identificationdone', this.identification);
            this.remove();
        }
    });

    return IdentificationFormView;
});