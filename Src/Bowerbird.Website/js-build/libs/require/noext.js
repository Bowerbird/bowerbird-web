/*!
 * RequireJS plugin for loading files without adding the JS extension, useful for
 * JSONP services and any other kind of resource that already contain a file
 * extension or that shouldn't have one (like dynamic scripts).
 * @author Miller Medeiros
 * @version 0.3.0 (2011/10/26)
 * Released under the WTFPL <http://sam.zoy.org/wtfpl/>
 */

define([],function(){var a="noext";return{load:function(a,b,c,d){var e=b.toUrl(a).replace(/\.js$/,"");b([e],function(a){c(a)})},normalize:function(b,c){return b+=b.indexOf("?")<0?"?":"&",b+a+"=1"}}})