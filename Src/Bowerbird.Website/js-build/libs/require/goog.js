/** @license
 * RequireJS plugin for loading Google Ajax API modules thru `google.load`
 * Author: Miller Medeiros
 * Version: 0.2.0 (2011/12/06)
 * Released under the MIT license
 */

define(["async","propertyParser"],function(a,b){function d(a){var d=c.exec(a),e={moduleName:d[1],version:d[2]||"1"};return e.settings=b.parseProperties(d[3]),e}var c=/^([^,]+)(?:,([^,]+))?(?:,(.+))?/;return{load:function(a,b,c,e){if(e.isBuild)c(null);else{var f=d(a),g=f.settings;g.callback=c,b(["async!"+(document.location.protocol==="https:"?"https":"http")+"://www.google.com/jsapi"],function(){google.load(f.moduleName,f.version,g)})}}}})