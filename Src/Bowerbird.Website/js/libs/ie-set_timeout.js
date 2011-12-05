/**
* ie-set_timeout.js
* -----------------
* Override the setTimeout function on Internet Explorer, which
* doesn't accept function parameters.
* 
* Documentation: https://developer.mozilla.org/en/DOM/window.setTimeout
*
* Copyright (c) 2010 Ernesto Mendez (der-design.com)
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
* Version 1.0.1
* - Added missing return statement, which failed to return a timeout ID
* - Changed IE detection algorithm
*
* Version 1.0.0
*  - Initial Release
* 
*/

(function () {

    var ie_version = (function () {

        var re = /MSIE (\d+)\./;

        var match = navigator.appVersion.match(re);

        if (match) { return parseInt(match[1]); }

        else { return null; }

    })()

    /* Detect if any version of Internet Explorer is used,
    * and override the window.setTimeout function with the new one, which
    * provides support for function arguments. */

    if (ie_version != null) {

        var old_setTimeout = window.setTimeout;

        window.setTimeout = function (callback, timeout) {

            var args = []; for (var i = 0; i < arguments.length; i++) { args.push(arguments[i]); } // Convert args to array

            args = args.slice(2);

            var f = function () { callback.apply(null, args); } // Makes 'this' to be mapped to DOMWindow

            return old_setTimeout(f, timeout); // Return timeout ID

        }

    }

})();