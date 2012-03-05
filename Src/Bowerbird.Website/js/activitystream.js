// this is a backup of the script in members/home/index JIC

 <script type="text/javascript">
        $(function () {
            window.activityHub = $.connection.activityHub;

            window.activityHub.activityOccurred = function (data) {
                var activity = eval('(' + data + ')');
                $('#activity').append('<div><span><img src="' + activity.Avatar.UrlToImage + '" alt="' + activity.Avatar.AltTag + '"/></span><span>' + activity.Message + '</span></div>');
            };

            window.activityHub.messageReceived = function (data) {
                var message = eval('(' + data + ')');
                $('#messanger').append('<div><span><img src="' + message.Avatar.UrlToImage + '" alt="' + message.Avatar.AltTag + '"/></span><span>' + message.UserName + '</span><span>' + message.CreatedDateTime + '</span><span>' + message.Message + '</span></div>');
            };

            window.activityHub.clientConnectionCheck = function () {
                window.activityHub.pollingRefresh()
            };

            window.activityHub.connectedUsers = function (data) {
                var users = eval('(' + data + ')');
                $.each(users, function(index, value){
                    $('#connectedusers').append('<div id="' + value.UserId + '"><span><img src="' + value.Avatar.UrlToImage + '" alt="' + value.Avatar.AltTag + '"/></span><span>' + value.UserName + '</span></div>');
                });
            };

            $.connection.hub.start(function () {
                window.activityHub.clientId = $.signalR.hub.id;
                window.activityHub.registerClientUser('@(Model.UserProfile.Id)')
                    .done(function () {
                        console.log('connected with ' + window.activityHub.clientId);
                        startServerConnectionRefreshing();
                        window.activityHub.getCurrentlyConnectedUsers();
                    })
                    .fail(function (e) {
                        console.warn(e);
                    });
            });

        });

        function ChatWithUser(){
            var chatMessage = $('#message-send-to-user').val();
            var userToSendTo = $('#user-chatting-with').val();
            window.activityHub.chatToUser(userToSendTo,chatMessage)
                    .done(function () {
                        console.log('message send: ' + chatMessage + ' to: ' + userToSendTo);
                    })
                    .fail(function (e) {
                        console.warn(e);
                    });
        }

        // Setup connection refreshing every ten minutes
        var interval = 1000 * 60 * 10;
        var timerId;
        var serverRefreshConnection = function () { 
            window.activityHub.pollingRefresh();
            //window.activityHub.getCurrentlyConnectedUsers();
        };
        function startServerConnectionRefreshing(){
            timerId = setTimeout(serverRefreshConnection, interval);
        }
        function stopServerConnectionRefreshing(){
            clearTimeout(timerId);
        }
    </script>