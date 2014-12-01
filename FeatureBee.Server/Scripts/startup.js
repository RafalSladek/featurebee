$(function () {
    // board hub => board specific functions. 
    var boardHub = $.connection.boardHub;
    // edit panel hub => edit dialog specific functions.
    var editPanelHub = $.connection.editPanelHub;

    var request = {
        get: function(url) {
            var reposonse;
            jQuery.ajaxSetup({ async: false });
            $.get(url).done(function(data) {
                reposonse = data;
            });
            jQuery.ajaxSetup({ async: true });
            return reposonse;
        }
    };

    var conditionsController = new ConditionController([
        { type: "trafficDistribution", values : [] },
        { type: "culture", values: [] },
        { type: "browser", values: [] },
        { type: "public-ip", values: [] }
    ]);
    var featureController = new FeatureBeeController(boardHub, editPanelHub, request, conditionsController);
    var form;
    
    boardHub.client.featureCreated = function (item) {
        $.Comm('page', 'itemChanged').publish(item);
    };

    boardHub.client.featureReleasedForEveryone = function (item) {
        $.Comm('page', 'itemMoved').publish(item);
    };
    
    boardHub.client.featureReleasedWithConditions = function (item) {
        $.Comm('page', 'itemMoved').publish(item);
    };

    boardHub.client.featureRollbacked = function (item) {
        $.Comm('page', 'itemMoved').publish(item);
    };

    editPanelHub.client.linkedToTicket = function (item) {
        $.Comm('page', 'itemChanged').publish(item);
    };

    editPanelHub.client.descriptionUpdated = function (item) {
        $.Comm('page', 'itemChanged').publish(item);
    };

    editPanelHub.client.conditionsChanged = function (item) {
        $.Comm('page', 'itemChanged').publish(item);
    };


    var boot = {
        loadPrerequisite: function () {
            form = new forms();
            return this;
        },

        loadTemplates: function () {
            handleBar(new conditionTemplates(form));
            return this;
        },
   
        loadMenu: function () {
            $('[data-open="newFeature"]').click(form.openNew);
            return this;
        },
        
        loadBoard: function () {
            board();
            return this;
        }
    };

    var dataFilter = function () {

        $('[data-filter-team]').click(function() {
            if (!$(this).hasClass('disabled')) {
                $(this).addClass("disabled");
            } else {
                $(this).removeClass("disabled");
            }
            changed();
            return false;
        });

        var byTeam = function (data) {
            var enabledTeams = $('[data-filter-team]').not('.disabled').map(function () { return $(this).attr('data-filter-team').toUpperCase(); }).get();
            return jQuery.grep(data, function (value) {
                return $.inArray(value.team.toUpperCase(), enabledTeams) >= 0;
            });
        };

        var changed = function () {
            $.Comm('page', 'itemChanged').publish();
        };

        this.apply = function (data) {
            var filters = [byTeam];
            $.each(filters, function (index, value) {
                data = value(data);
            });
            return data;
        };
    };

    var filter = new dataFilter();

    var dataProvider = function() {
        var data = featureController.find.all();
        return filter.apply(data);
    };

    var board = function() {
        $('#board').boardify({
            states: "[data-state]",
            template: '[data-item]',
            source: dataProvider,
            subscribeToItemChanged: function(obj) {
                boardHub.server.moveItem(obj.data.name, obj.data.index);
            },
            subscribeToItemSelected: function(obj) {
                form.openEdit(obj.data);
            }
        });

        $('#board').boardify('subscribeFor', 'page', 'itemChanged', $.boardifySubscribers.refresh);
        $('#board').boardify('subscribeFor', 'page', 'itemMoved', $.boardifySubscribers.refresh);
    };

    var conditionTemplates = function () {
        var templates = [];

        var loadConditionTemplates = function (templatesTypeTemplateStore) {
            $('[data-template]').each(function(index, value) {
                templatesTypeTemplateStore.push({
                    type: $(value).attr('data-template'),
                    template: $(value)
                });
            });
            return templatesTypeTemplateStore;
        };

        var c = $('[data-container="condition"]').conditionify({
            conditions: loadConditionTemplates(templates)
        });

        this.render = function (name, type, element, data) {
            c.conditionify('render', name, type, element, data);
        };
    };

    var handleBar = function(templates) {
        var self = this;

        Handlebars.registerHelper('setIndex', function (value) {
            this.outerindex = Number(value);
        });

        window.Handlebars.registerHelper('select', function(value, options) {
            var $el = $('<select />').html(options.fn(this));
            $el.find('[value=' + value + ']').attr({ 'selected': 'selected' });
            return $el.html();
        });
        window.Handlebars.registerHelper('condition', function(name, type, conditions, options) {
            var $el = $(options.fn({ type: type, values: conditions }).trim());
            templates.render(name, type, conditions);
            return $el.html();
        });
        window.Handlebars.registerHelper('emptyCondition', function (name) {
            return "";
        });
        window.Handlebars.registerHelper('editExisting', function (name) {
            return name == "edit";
        });
    };

    var forms = function () {
        var self = this;

        var createForm = function (usingItem, withWidth, callback) {
            return usingItem.clone().appendTo(usingItem.parent()).formify({
                save: function (data) {
                    callback(data);
                },
                width: withWidth,
                source: featureController.edit.get
            });
        };

        var createEditForm = function (usingItem) {
            return createForm(usingItem, $(window).width() - 180,
                featureController.edit.post);
        };

        var createNewForm = function (usingItem) {
            return createForm(usingItem, $(window).width() / 2, featureController.add.post);
        };

        var formEdit = createEditForm($('[data-edit-item="edit"]'));
        var formNew = createNewForm($('[data-edit-item="new"]'));

        this.openEdit = function (data) {
            formEdit.formify('open', data);
        };

        this.openNew = function () {
            formNew.formify('open');
        };
    };

    $.connection.hub.start().done(function () {
        boot.loadPrerequisite().loadTemplates().loadMenu().loadBoard();
    });
});