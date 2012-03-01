// this is a backup of the script in members/home/index JIC

 <script type="text/javascript">
        $(function () {
            window.activityHub = $.connection.activityHub;
            window.activityHub.activityOccurred = function (data) {
                var activity = eval('(' + data + ')');
                $('#activity').append('<div><span><img src="' + activity.Avatar.UrlToImage + '" alt="' + activity.Avatar.AltTag + '"/></span><span>' + activity.Message + '</span></div>');
            };

            $.connection.hub.start(function () {
                window.activityHub.clientId = $.signalR.hub.id;
                window.activityHub.registerClientUser('@(Model.UserProfile.Id)')
                    .done(function () {
                        console.log('connected with ' + window.activityHub.clientId);
                    })
                    .fail(function (e) {
                        console.warn(e);
                    });
            });
        });
    </script>