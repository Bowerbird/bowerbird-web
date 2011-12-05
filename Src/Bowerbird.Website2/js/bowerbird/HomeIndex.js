var homeIndex = {
    init: function () {

        observationCreateDialog:'',
        // Setup partial plugins
        observationCreate.init();

        $('#observationCreateButton').button().click(function () {
            homeIndex.
            observationCreate.makeForm();
            return false;
        });
    }
};
