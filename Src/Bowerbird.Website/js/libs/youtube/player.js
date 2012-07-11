/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />

<!-- This is hacked from https://developers.google.com/youtube/iframe_api_reference to provide api access to youtube movies insitu -->

define(['youtube'], 
function (youtube) 
{
    function onYouTubePlayerAPIReady(Height, Width, VideoId) {
        player = new YT.Player('player', {
            height: Height,
            width: Width,
            videoId: VideoId,
            events: {
                'onReady': onPlayerReady,
                'onStateChange': onPlayerStateChange
            }
        });
    }

    function onPlayerReady(event) {
        // this needs to be 
        //event.target.playVideo();
    }

    function onPlayerStateChange(event) {
        if (event.data == YT.PlayerState.ENDED) {
            //rewind to the start and stop
            event.target.stopVideo();
        }
    }
});