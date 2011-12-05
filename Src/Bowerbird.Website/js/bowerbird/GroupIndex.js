$(document).ready(function () {
    GroupIndex.init();
    GroupIndex.setupGroupDetail();
});

var GroupIndex = {

    init: function () {

    },

    setupGroupDetail: function () {
        $('.group-detail-block:nth-child(2n+2)').addClass('background-color-light');
    },

};

<script id="groupTemplate" type="text/x-jQuery-tmpl">
    <div class="group-detail-block">
        <div class="group-field">

        </div>
        <div class="group-field">
        
        </div>
    </div>
</script>