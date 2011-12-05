$(document).ready(function () {
    ObservationIndex.init();
    ObservationIndex.setupObservationDetail();
});

var ObservationIndex = {

    init: function () {

    },

    setupObservationDetail: function(){
        $('.observation-detail-block:nth-child(2n+2)').addClass('background-color-light');
    }

};

<script id="observationTemplate" type="text/x-jQuery-tmpl">
    <div class="observation-detail-block">
        <div class="observation-field">

        </div>
        <div class="observation-field">
        
        </div>
    </div>
</script>