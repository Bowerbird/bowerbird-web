
/*!
 * jQuery JavaScript Library v1.7.2
 * http://jquery.com/
 *
 * Copyright 2011, John Resig
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 * Copyright 2011, The Dojo Foundation
 * Released under the MIT, BSD, and GPL Licenses.
 *
 * Date: Wed Mar 21 12:46:34 2012 -0700
 */
(function( window, undefined ) {

// Use the correct document accordingly with window argument (sandbox)
var document = window.document,
	navigator = window.navigator,
	location = window.location;
var jQuery = (function() {

// Define a local copy of jQuery
var jQuery = function( selector, context ) {
		// The jQuery object is actually just the init constructor 'enhanced'
		return new jQuery.fn.init( selector, context, rootjQuery );
	},

	// Map over jQuery in case of overwrite
	_jQuery = window.jQuery,

	// Map over the $ in case of overwrite
	_$ = window.$,

	// A central reference to the root jQuery(document)
	rootjQuery,

	// A simple way to check for HTML strings or ID strings
	// Prioritize #id over <tag> to avoid XSS via location.hash (#9521)
	quickExpr = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/,

	// Check if a string has a non-whitespace character in it
	rnotwhite = /\S/,

	// Used for trimming whitespace
	trimLeft = /^\s+/,
	trimRight = /\s+$/,

	// Match a standalone tag
	rsingleTag = /^<(\w+)\s*\/?>(?:<\/\1>)?$/,

	// JSON RegExp
	rvalidchars = /^[\],:{}\s]*$/,
	rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,
	rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,
	rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g,

	// Useragent RegExp
	rwebkit = /(webkit)[ \/]([\w.]+)/,
	ropera = /(opera)(?:.*version)?[ \/]([\w.]+)/,
	rmsie = /(msie) ([\w.]+)/,
	rmozilla = /(mozilla)(?:.*? rv:([\w.]+))?/,

	// Matches dashed string for camelizing
	rdashAlpha = /-([a-z]|[0-9])/ig,
	rmsPrefix = /^-ms-/,

	// Used by jQuery.camelCase as callback to replace()
	fcamelCase = function( all, letter ) {
		return ( letter + "" ).toUpperCase();
	},

	// Keep a UserAgent string for use with jQuery.browser
	userAgent = navigator.userAgent,

	// For matching the engine and version of the browser
	browserMatch,

	// The deferred used on DOM ready
	readyList,

	// The ready event handler
	DOMContentLoaded,

	// Save a reference to some core methods
	toString = Object.prototype.toString,
	hasOwn = Object.prototype.hasOwnProperty,
	push = Array.prototype.push,
	slice = Array.prototype.slice,
	trim = String.prototype.trim,
	indexOf = Array.prototype.indexOf,

	// [[Class]] -> type pairs
	class2type = {};

jQuery.fn = jQuery.prototype = {
	constructor: jQuery,
	init: function( selector, context, rootjQuery ) {
		var match, elem, ret, doc;

		// Handle $(""), $(null), or $(undefined)
		if ( !selector ) {
			return this;
		}

		// Handle $(DOMElement)
		if ( selector.nodeType ) {
			this.context = this[0] = selector;
			this.length = 1;
			return this;
		}

		// The body element only exists once, optimize finding it
		if ( selector === "body" && !context && document.body ) {
			this.context = document;
			this[0] = document.body;
			this.selector = selector;
			this.length = 1;
			return this;
		}

		// Handle HTML strings
		if ( typeof selector === "string" ) {
			// Are we dealing with HTML string or an ID?
			if ( selector.charAt(0) === "<" && selector.charAt( selector.length - 1 ) === ">" && selector.length >= 3 ) {
				// Assume that strings that start and end with <> are HTML and skip the regex check
				match = [ null, selector, null ];

			} else {
				match = quickExpr.exec( selector );
			}

			// Verify a match, and that no context was specified for #id
			if ( match && (match[1] || !context) ) {

				// HANDLE: $(html) -> $(array)
				if ( match[1] ) {
					context = context instanceof jQuery ? context[0] : context;
					doc = ( context ? context.ownerDocument || context : document );

					// If a single string is passed in and it's a single tag
					// just do a createElement and skip the rest
					ret = rsingleTag.exec( selector );

					if ( ret ) {
						if ( jQuery.isPlainObject( context ) ) {
							selector = [ document.createElement( ret[1] ) ];
							jQuery.fn.attr.call( selector, context, true );

						} else {
							selector = [ doc.createElement( ret[1] ) ];
						}

					} else {
						ret = jQuery.buildFragment( [ match[1] ], [ doc ] );
						selector = ( ret.cacheable ? jQuery.clone(ret.fragment) : ret.fragment ).childNodes;
					}

					return jQuery.merge( this, selector );

				// HANDLE: $("#id")
				} else {
					elem = document.getElementById( match[2] );

					// Check parentNode to catch when Blackberry 4.6 returns
					// nodes that are no longer in the document #6963
					if ( elem && elem.parentNode ) {
						// Handle the case where IE and Opera return items
						// by name instead of ID
						if ( elem.id !== match[2] ) {
							return rootjQuery.find( selector );
						}

						// Otherwise, we inject the element directly into the jQuery object
						this.length = 1;
						this[0] = elem;
					}

					this.context = document;
					this.selector = selector;
					return this;
				}

			// HANDLE: $(expr, $(...))
			} else if ( !context || context.jquery ) {
				return ( context || rootjQuery ).find( selector );

			// HANDLE: $(expr, context)
			// (which is just equivalent to: $(context).find(expr)
			} else {
				return this.constructor( context ).find( selector );
			}

		// HANDLE: $(function)
		// Shortcut for document ready
		} else if ( jQuery.isFunction( selector ) ) {
			return rootjQuery.ready( selector );
		}

		if ( selector.selector !== undefined ) {
			this.selector = selector.selector;
			this.context = selector.context;
		}

		return jQuery.makeArray( selector, this );
	},

	// Start with an empty selector
	selector: "",

	// The current version of jQuery being used
	jquery: "1.7.2",

	// The default length of a jQuery object is 0
	length: 0,

	// The number of elements contained in the matched element set
	size: function() {
		return this.length;
	},

	toArray: function() {
		return slice.call( this, 0 );
	},

	// Get the Nth element in the matched element set OR
	// Get the whole matched element set as a clean array
	get: function( num ) {
		return num == null ?

			// Return a 'clean' array
			this.toArray() :

			// Return just the object
			( num < 0 ? this[ this.length + num ] : this[ num ] );
	},

	// Take an array of elements and push it onto the stack
	// (returning the new matched element set)
	pushStack: function( elems, name, selector ) {
		// Build a new jQuery matched element set
		var ret = this.constructor();

		if ( jQuery.isArray( elems ) ) {
			push.apply( ret, elems );

		} else {
			jQuery.merge( ret, elems );
		}

		// Add the old object onto the stack (as a reference)
		ret.prevObject = this;

		ret.context = this.context;

		if ( name === "find" ) {
			ret.selector = this.selector + ( this.selector ? " " : "" ) + selector;
		} else if ( name ) {
			ret.selector = this.selector + "." + name + "(" + selector + ")";
		}

		// Return the newly-formed element set
		return ret;
	},

	// Execute a callback for every element in the matched set.
	// (You can seed the arguments with an array of args, but this is
	// only used internally.)
	each: function( callback, args ) {
		return jQuery.each( this, callback, args );
	},

	ready: function( fn ) {
		// Attach the listeners
		jQuery.bindReady();

		// Add the callback
		readyList.add( fn );

		return this;
	},

	eq: function( i ) {
		i = +i;
		return i === -1 ?
			this.slice( i ) :
			this.slice( i, i + 1 );
	},

	first: function() {
		return this.eq( 0 );
	},

	last: function() {
		return this.eq( -1 );
	},

	slice: function() {
		return this.pushStack( slice.apply( this, arguments ),
			"slice", slice.call(arguments).join(",") );
	},

	map: function( callback ) {
		return this.pushStack( jQuery.map(this, function( elem, i ) {
			return callback.call( elem, i, elem );
		}));
	},

	end: function() {
		return this.prevObject || this.constructor(null);
	},

	// For internal use only.
	// Behaves like an Array's method, not like a jQuery method.
	push: push,
	sort: [].sort,
	splice: [].splice
};

// Give the init function the jQuery prototype for later instantiation
jQuery.fn.init.prototype = jQuery.fn;

jQuery.extend = jQuery.fn.extend = function() {
	var options, name, src, copy, copyIsArray, clone,
		target = arguments[0] || {},
		i = 1,
		length = arguments.length,
		deep = false;

	// Handle a deep copy situation
	if ( typeof target === "boolean" ) {
		deep = target;
		target = arguments[1] || {};
		// skip the boolean and the target
		i = 2;
	}

	// Handle case when target is a string or something (possible in deep copy)
	if ( typeof target !== "object" && !jQuery.isFunction(target) ) {
		target = {};
	}

	// extend jQuery itself if only one argument is passed
	if ( length === i ) {
		target = this;
		--i;
	}

	for ( ; i < length; i++ ) {
		// Only deal with non-null/undefined values
		if ( (options = arguments[ i ]) != null ) {
			// Extend the base object
			for ( name in options ) {
				src = target[ name ];
				copy = options[ name ];

				// Prevent never-ending loop
				if ( target === copy ) {
					continue;
				}

				// Recurse if we're merging plain objects or arrays
				if ( deep && copy && ( jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)) ) ) {
					if ( copyIsArray ) {
						copyIsArray = false;
						clone = src && jQuery.isArray(src) ? src : [];

					} else {
						clone = src && jQuery.isPlainObject(src) ? src : {};
					}

					// Never move original objects, clone them
					target[ name ] = jQuery.extend( deep, clone, copy );

				// Don't bring in undefined values
				} else if ( copy !== undefined ) {
					target[ name ] = copy;
				}
			}
		}
	}

	// Return the modified object
	return target;
};

jQuery.extend({
	noConflict: function( deep ) {
		if ( window.$ === jQuery ) {
			window.$ = _$;
		}

		if ( deep && window.jQuery === jQuery ) {
			window.jQuery = _jQuery;
		}

		return jQuery;
	},

	// Is the DOM ready to be used? Set to true once it occurs.
	isReady: false,

	// A counter to track how many items to wait for before
	// the ready event fires. See #6781
	readyWait: 1,

	// Hold (or release) the ready event
	holdReady: function( hold ) {
		if ( hold ) {
			jQuery.readyWait++;
		} else {
			jQuery.ready( true );
		}
	},

	// Handle when the DOM is ready
	ready: function( wait ) {
		// Either a released hold or an DOMready/load event and not yet ready
		if ( (wait === true && !--jQuery.readyWait) || (wait !== true && !jQuery.isReady) ) {
			// Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
			if ( !document.body ) {
				return setTimeout( jQuery.ready, 1 );
			}

			// Remember that the DOM is ready
			jQuery.isReady = true;

			// If a normal DOM Ready event fired, decrement, and wait if need be
			if ( wait !== true && --jQuery.readyWait > 0 ) {
				return;
			}

			// If there are functions bound, to execute
			readyList.fireWith( document, [ jQuery ] );

			// Trigger any bound ready events
			if ( jQuery.fn.trigger ) {
				jQuery( document ).trigger( "ready" ).off( "ready" );
			}
		}
	},

	bindReady: function() {
		if ( readyList ) {
			return;
		}

		readyList = jQuery.Callbacks( "once memory" );

		// Catch cases where $(document).ready() is called after the
		// browser event has already occurred.
		if ( document.readyState === "complete" ) {
			// Handle it asynchronously to allow scripts the opportunity to delay ready
			return setTimeout( jQuery.ready, 1 );
		}

		// Mozilla, Opera and webkit nightlies currently support this event
		if ( document.addEventListener ) {
			// Use the handy event callback
			document.addEventListener( "DOMContentLoaded", DOMContentLoaded, false );

			// A fallback to window.onload, that will always work
			window.addEventListener( "load", jQuery.ready, false );

		// If IE event model is used
		} else if ( document.attachEvent ) {
			// ensure firing before onload,
			// maybe late but safe also for iframes
			document.attachEvent( "onreadystatechange", DOMContentLoaded );

			// A fallback to window.onload, that will always work
			window.attachEvent( "onload", jQuery.ready );

			// If IE and not a frame
			// continually check to see if the document is ready
			var toplevel = false;

			try {
				toplevel = window.frameElement == null;
			} catch(e) {}

			if ( document.documentElement.doScroll && toplevel ) {
				doScrollCheck();
			}
		}
	},

	// See test/unit/core.js for details concerning isFunction.
	// Since version 1.3, DOM methods and functions like alert
	// aren't supported. They return false on IE (#2968).
	isFunction: function( obj ) {
		return jQuery.type(obj) === "function";
	},

	isArray: Array.isArray || function( obj ) {
		return jQuery.type(obj) === "array";
	},

	isWindow: function( obj ) {
		return obj != null && obj == obj.window;
	},

	isNumeric: function( obj ) {
		return !isNaN( parseFloat(obj) ) && isFinite( obj );
	},

	type: function( obj ) {
		return obj == null ?
			String( obj ) :
			class2type[ toString.call(obj) ] || "object";
	},

	isPlainObject: function( obj ) {
		// Must be an Object.
		// Because of IE, we also have to check the presence of the constructor property.
		// Make sure that DOM nodes and window objects don't pass through, as well
		if ( !obj || jQuery.type(obj) !== "object" || obj.nodeType || jQuery.isWindow( obj ) ) {
			return false;
		}

		try {
			// Not own constructor property must be Object
			if ( obj.constructor &&
				!hasOwn.call(obj, "constructor") &&
				!hasOwn.call(obj.constructor.prototype, "isPrototypeOf") ) {
				return false;
			}
		} catch ( e ) {
			// IE8,9 Will throw exceptions on certain host objects #9897
			return false;
		}

		// Own properties are enumerated firstly, so to speed up,
		// if last one is own, then all properties are own.

		var key;
		for ( key in obj ) {}

		return key === undefined || hasOwn.call( obj, key );
	},

	isEmptyObject: function( obj ) {
		for ( var name in obj ) {
			return false;
		}
		return true;
	},

	error: function( msg ) {
		throw new Error( msg );
	},

	parseJSON: function( data ) {
		if ( typeof data !== "string" || !data ) {
			return null;
		}

		// Make sure leading/trailing whitespace is removed (IE can't handle it)
		data = jQuery.trim( data );

		// Attempt to parse using the native JSON parser first
		if ( window.JSON && window.JSON.parse ) {
			return window.JSON.parse( data );
		}

		// Make sure the incoming data is actual JSON
		// Logic borrowed from http://json.org/json2.js
		if ( rvalidchars.test( data.replace( rvalidescape, "@" )
			.replace( rvalidtokens, "]" )
			.replace( rvalidbraces, "")) ) {

			return ( new Function( "return " + data ) )();

		}
		jQuery.error( "Invalid JSON: " + data );
	},

	// Cross-browser xml parsing
	parseXML: function( data ) {
		if ( typeof data !== "string" || !data ) {
			return null;
		}
		var xml, tmp;
		try {
			if ( window.DOMParser ) { // Standard
				tmp = new DOMParser();
				xml = tmp.parseFromString( data , "text/xml" );
			} else { // IE
				xml = new ActiveXObject( "Microsoft.XMLDOM" );
				xml.async = "false";
				xml.loadXML( data );
			}
		} catch( e ) {
			xml = undefined;
		}
		if ( !xml || !xml.documentElement || xml.getElementsByTagName( "parsererror" ).length ) {
			jQuery.error( "Invalid XML: " + data );
		}
		return xml;
	},

	noop: function() {},

	// Evaluates a script in a global context
	// Workarounds based on findings by Jim Driscoll
	// http://weblogs.java.net/blog/driscoll/archive/2009/09/08/eval-javascript-global-context
	globalEval: function( data ) {
		if ( data && rnotwhite.test( data ) ) {
			// We use execScript on Internet Explorer
			// We use an anonymous function so that context is window
			// rather than jQuery in Firefox
			( window.execScript || function( data ) {
				window[ "eval" ].call( window, data );
			} )( data );
		}
	},

	// Convert dashed to camelCase; used by the css and data modules
	// Microsoft forgot to hump their vendor prefix (#9572)
	camelCase: function( string ) {
		return string.replace( rmsPrefix, "ms-" ).replace( rdashAlpha, fcamelCase );
	},

	nodeName: function( elem, name ) {
		return elem.nodeName && elem.nodeName.toUpperCase() === name.toUpperCase();
	},

	// args is for internal usage only
	each: function( object, callback, args ) {
		var name, i = 0,
			length = object.length,
			isObj = length === undefined || jQuery.isFunction( object );

		if ( args ) {
			if ( isObj ) {
				for ( name in object ) {
					if ( callback.apply( object[ name ], args ) === false ) {
						break;
					}
				}
			} else {
				for ( ; i < length; ) {
					if ( callback.apply( object[ i++ ], args ) === false ) {
						break;
					}
				}
			}

		// A special, fast, case for the most common use of each
		} else {
			if ( isObj ) {
				for ( name in object ) {
					if ( callback.call( object[ name ], name, object[ name ] ) === false ) {
						break;
					}
				}
			} else {
				for ( ; i < length; ) {
					if ( callback.call( object[ i ], i, object[ i++ ] ) === false ) {
						break;
					}
				}
			}
		}

		return object;
	},

	// Use native String.trim function wherever possible
	trim: trim ?
		function( text ) {
			return text == null ?
				"" :
				trim.call( text );
		} :

		// Otherwise use our own trimming functionality
		function( text ) {
			return text == null ?
				"" :
				text.toString().replace( trimLeft, "" ).replace( trimRight, "" );
		},

	// results is for internal usage only
	makeArray: function( array, results ) {
		var ret = results || [];

		if ( array != null ) {
			// The window, strings (and functions) also have 'length'
			// Tweaked logic slightly to handle Blackberry 4.7 RegExp issues #6930
			var type = jQuery.type( array );

			if ( array.length == null || type === "string" || type === "function" || type === "regexp" || jQuery.isWindow( array ) ) {
				push.call( ret, array );
			} else {
				jQuery.merge( ret, array );
			}
		}

		return ret;
	},

	inArray: function( elem, array, i ) {
		var len;

		if ( array ) {
			if ( indexOf ) {
				return indexOf.call( array, elem, i );
			}

			len = array.length;
			i = i ? i < 0 ? Math.max( 0, len + i ) : i : 0;

			for ( ; i < len; i++ ) {
				// Skip accessing in sparse arrays
				if ( i in array && array[ i ] === elem ) {
					return i;
				}
			}
		}

		return -1;
	},

	merge: function( first, second ) {
		var i = first.length,
			j = 0;

		if ( typeof second.length === "number" ) {
			for ( var l = second.length; j < l; j++ ) {
				first[ i++ ] = second[ j ];
			}

		} else {
			while ( second[j] !== undefined ) {
				first[ i++ ] = second[ j++ ];
			}
		}

		first.length = i;

		return first;
	},

	grep: function( elems, callback, inv ) {
		var ret = [], retVal;
		inv = !!inv;

		// Go through the array, only saving the items
		// that pass the validator function
		for ( var i = 0, length = elems.length; i < length; i++ ) {
			retVal = !!callback( elems[ i ], i );
			if ( inv !== retVal ) {
				ret.push( elems[ i ] );
			}
		}

		return ret;
	},

	// arg is for internal usage only
	map: function( elems, callback, arg ) {
		var value, key, ret = [],
			i = 0,
			length = elems.length,
			// jquery objects are treated as arrays
			isArray = elems instanceof jQuery || length !== undefined && typeof length === "number" && ( ( length > 0 && elems[ 0 ] && elems[ length -1 ] ) || length === 0 || jQuery.isArray( elems ) ) ;

		// Go through the array, translating each of the items to their
		if ( isArray ) {
			for ( ; i < length; i++ ) {
				value = callback( elems[ i ], i, arg );

				if ( value != null ) {
					ret[ ret.length ] = value;
				}
			}

		// Go through every key on the object,
		} else {
			for ( key in elems ) {
				value = callback( elems[ key ], key, arg );

				if ( value != null ) {
					ret[ ret.length ] = value;
				}
			}
		}

		// Flatten any nested arrays
		return ret.concat.apply( [], ret );
	},

	// A global GUID counter for objects
	guid: 1,

	// Bind a function to a context, optionally partially applying any
	// arguments.
	proxy: function( fn, context ) {
		if ( typeof context === "string" ) {
			var tmp = fn[ context ];
			context = fn;
			fn = tmp;
		}

		// Quick check to determine if target is callable, in the spec
		// this throws a TypeError, but we will just return undefined.
		if ( !jQuery.isFunction( fn ) ) {
			return undefined;
		}

		// Simulated bind
		var args = slice.call( arguments, 2 ),
			proxy = function() {
				return fn.apply( context, args.concat( slice.call( arguments ) ) );
			};

		// Set the guid of unique handler to the same of original handler, so it can be removed
		proxy.guid = fn.guid = fn.guid || proxy.guid || jQuery.guid++;

		return proxy;
	},

	// Mutifunctional method to get and set values to a collection
	// The value/s can optionally be executed if it's a function
	access: function( elems, fn, key, value, chainable, emptyGet, pass ) {
		var exec,
			bulk = key == null,
			i = 0,
			length = elems.length;

		// Sets many values
		if ( key && typeof key === "object" ) {
			for ( i in key ) {
				jQuery.access( elems, fn, i, key[i], 1, emptyGet, value );
			}
			chainable = 1;

		// Sets one value
		} else if ( value !== undefined ) {
			// Optionally, function values get executed if exec is true
			exec = pass === undefined && jQuery.isFunction( value );

			if ( bulk ) {
				// Bulk operations only iterate when executing function values
				if ( exec ) {
					exec = fn;
					fn = function( elem, key, value ) {
						return exec.call( jQuery( elem ), value );
					};

				// Otherwise they run against the entire set
				} else {
					fn.call( elems, value );
					fn = null;
				}
			}

			if ( fn ) {
				for (; i < length; i++ ) {
					fn( elems[i], key, exec ? value.call( elems[i], i, fn( elems[i], key ) ) : value, pass );
				}
			}

			chainable = 1;
		}

		return chainable ?
			elems :

			// Gets
			bulk ?
				fn.call( elems ) :
				length ? fn( elems[0], key ) : emptyGet;
	},

	now: function() {
		return ( new Date() ).getTime();
	},

	// Use of jQuery.browser is frowned upon.
	// More details: http://docs.jquery.com/Utilities/jQuery.browser
	uaMatch: function( ua ) {
		ua = ua.toLowerCase();

		var match = rwebkit.exec( ua ) ||
			ropera.exec( ua ) ||
			rmsie.exec( ua ) ||
			ua.indexOf("compatible") < 0 && rmozilla.exec( ua ) ||
			[];

		return { browser: match[1] || "", version: match[2] || "0" };
	},

	sub: function() {
		function jQuerySub( selector, context ) {
			return new jQuerySub.fn.init( selector, context );
		}
		jQuery.extend( true, jQuerySub, this );
		jQuerySub.superclass = this;
		jQuerySub.fn = jQuerySub.prototype = this();
		jQuerySub.fn.constructor = jQuerySub;
		jQuerySub.sub = this.sub;
		jQuerySub.fn.init = function init( selector, context ) {
			if ( context && context instanceof jQuery && !(context instanceof jQuerySub) ) {
				context = jQuerySub( context );
			}

			return jQuery.fn.init.call( this, selector, context, rootjQuerySub );
		};
		jQuerySub.fn.init.prototype = jQuerySub.fn;
		var rootjQuerySub = jQuerySub(document);
		return jQuerySub;
	},

	browser: {}
});

// Populate the class2type map
jQuery.each("Boolean Number String Function Array Date RegExp Object".split(" "), function(i, name) {
	class2type[ "[object " + name + "]" ] = name.toLowerCase();
});

browserMatch = jQuery.uaMatch( userAgent );
if ( browserMatch.browser ) {
	jQuery.browser[ browserMatch.browser ] = true;
	jQuery.browser.version = browserMatch.version;
}

// Deprecated, use jQuery.browser.webkit instead
if ( jQuery.browser.webkit ) {
	jQuery.browser.safari = true;
}

// IE doesn't match non-breaking spaces with \s
if ( rnotwhite.test( "\xA0" ) ) {
	trimLeft = /^[\s\xA0]+/;
	trimRight = /[\s\xA0]+$/;
}

// All jQuery objects should point back to these
rootjQuery = jQuery(document);

// Cleanup functions for the document ready method
if ( document.addEventListener ) {
	DOMContentLoaded = function() {
		document.removeEventListener( "DOMContentLoaded", DOMContentLoaded, false );
		jQuery.ready();
	};

} else if ( document.attachEvent ) {
	DOMContentLoaded = function() {
		// Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
		if ( document.readyState === "complete" ) {
			document.detachEvent( "onreadystatechange", DOMContentLoaded );
			jQuery.ready();
		}
	};
}

// The DOM ready check for Internet Explorer
function doScrollCheck() {
	if ( jQuery.isReady ) {
		return;
	}

	try {
		// If IE is used, use the trick by Diego Perini
		// http://javascript.nwbox.com/IEContentLoaded/
		document.documentElement.doScroll("left");
	} catch(e) {
		setTimeout( doScrollCheck, 1 );
		return;
	}

	// and execute any waiting functions
	jQuery.ready();
}

return jQuery;

})();


// String to Object flags format cache
var flagsCache = {};

// Convert String-formatted flags into Object-formatted ones and store in cache
function createFlags( flags ) {
	var object = flagsCache[ flags ] = {},
		i, length;
	flags = flags.split( /\s+/ );
	for ( i = 0, length = flags.length; i < length; i++ ) {
		object[ flags[i] ] = true;
	}
	return object;
}

/*
 * Create a callback list using the following parameters:
 *
 *	flags:	an optional list of space-separated flags that will change how
 *			the callback list behaves
 *
 * By default a callback list will act like an event callback list and can be
 * "fired" multiple times.
 *
 * Possible flags:
 *
 *	once:			will ensure the callback list can only be fired once (like a Deferred)
 *
 *	memory:			will keep track of previous values and will call any callback added
 *					after the list has been fired right away with the latest "memorized"
 *					values (like a Deferred)
 *
 *	unique:			will ensure a callback can only be added once (no duplicate in the list)
 *
 *	stopOnFalse:	interrupt callings when a callback returns false
 *
 */
jQuery.Callbacks = function( flags ) {

	// Convert flags from String-formatted to Object-formatted
	// (we check in cache first)
	flags = flags ? ( flagsCache[ flags ] || createFlags( flags ) ) : {};

	var // Actual callback list
		list = [],
		// Stack of fire calls for repeatable lists
		stack = [],
		// Last fire value (for non-forgettable lists)
		memory,
		// Flag to know if list was already fired
		fired,
		// Flag to know if list is currently firing
		firing,
		// First callback to fire (used internally by add and fireWith)
		firingStart,
		// End of the loop when firing
		firingLength,
		// Index of currently firing callback (modified by remove if needed)
		firingIndex,
		// Add one or several callbacks to the list
		add = function( args ) {
			var i,
				length,
				elem,
				type,
				actual;
			for ( i = 0, length = args.length; i < length; i++ ) {
				elem = args[ i ];
				type = jQuery.type( elem );
				if ( type === "array" ) {
					// Inspect recursively
					add( elem );
				} else if ( type === "function" ) {
					// Add if not in unique mode and callback is not in
					if ( !flags.unique || !self.has( elem ) ) {
						list.push( elem );
					}
				}
			}
		},
		// Fire callbacks
		fire = function( context, args ) {
			args = args || [];
			memory = !flags.memory || [ context, args ];
			fired = true;
			firing = true;
			firingIndex = firingStart || 0;
			firingStart = 0;
			firingLength = list.length;
			for ( ; list && firingIndex < firingLength; firingIndex++ ) {
				if ( list[ firingIndex ].apply( context, args ) === false && flags.stopOnFalse ) {
					memory = true; // Mark as halted
					break;
				}
			}
			firing = false;
			if ( list ) {
				if ( !flags.once ) {
					if ( stack && stack.length ) {
						memory = stack.shift();
						self.fireWith( memory[ 0 ], memory[ 1 ] );
					}
				} else if ( memory === true ) {
					self.disable();
				} else {
					list = [];
				}
			}
		},
		// Actual Callbacks object
		self = {
			// Add a callback or a collection of callbacks to the list
			add: function() {
				if ( list ) {
					var length = list.length;
					add( arguments );
					// Do we need to add the callbacks to the
					// current firing batch?
					if ( firing ) {
						firingLength = list.length;
					// With memory, if we're not firing then
					// we should call right away, unless previous
					// firing was halted (stopOnFalse)
					} else if ( memory && memory !== true ) {
						firingStart = length;
						fire( memory[ 0 ], memory[ 1 ] );
					}
				}
				return this;
			},
			// Remove a callback from the list
			remove: function() {
				if ( list ) {
					var args = arguments,
						argIndex = 0,
						argLength = args.length;
					for ( ; argIndex < argLength ; argIndex++ ) {
						for ( var i = 0; i < list.length; i++ ) {
							if ( args[ argIndex ] === list[ i ] ) {
								// Handle firingIndex and firingLength
								if ( firing ) {
									if ( i <= firingLength ) {
										firingLength--;
										if ( i <= firingIndex ) {
											firingIndex--;
										}
									}
								}
								// Remove the element
								list.splice( i--, 1 );
								// If we have some unicity property then
								// we only need to do this once
								if ( flags.unique ) {
									break;
								}
							}
						}
					}
				}
				return this;
			},
			// Control if a given callback is in the list
			has: function( fn ) {
				if ( list ) {
					var i = 0,
						length = list.length;
					for ( ; i < length; i++ ) {
						if ( fn === list[ i ] ) {
							return true;
						}
					}
				}
				return false;
			},
			// Remove all callbacks from the list
			empty: function() {
				list = [];
				return this;
			},
			// Have the list do nothing anymore
			disable: function() {
				list = stack = memory = undefined;
				return this;
			},
			// Is it disabled?
			disabled: function() {
				return !list;
			},
			// Lock the list in its current state
			lock: function() {
				stack = undefined;
				if ( !memory || memory === true ) {
					self.disable();
				}
				return this;
			},
			// Is it locked?
			locked: function() {
				return !stack;
			},
			// Call all callbacks with the given context and arguments
			fireWith: function( context, args ) {
				if ( stack ) {
					if ( firing ) {
						if ( !flags.once ) {
							stack.push( [ context, args ] );
						}
					} else if ( !( flags.once && memory ) ) {
						fire( context, args );
					}
				}
				return this;
			},
			// Call all the callbacks with the given arguments
			fire: function() {
				self.fireWith( this, arguments );
				return this;
			},
			// To know if the callbacks have already been called at least once
			fired: function() {
				return !!fired;
			}
		};

	return self;
};




var // Static reference to slice
	sliceDeferred = [].slice;

jQuery.extend({

	Deferred: function( func ) {
		var doneList = jQuery.Callbacks( "once memory" ),
			failList = jQuery.Callbacks( "once memory" ),
			progressList = jQuery.Callbacks( "memory" ),
			state = "pending",
			lists = {
				resolve: doneList,
				reject: failList,
				notify: progressList
			},
			promise = {
				done: doneList.add,
				fail: failList.add,
				progress: progressList.add,

				state: function() {
					return state;
				},

				// Deprecated
				isResolved: doneList.fired,
				isRejected: failList.fired,

				then: function( doneCallbacks, failCallbacks, progressCallbacks ) {
					deferred.done( doneCallbacks ).fail( failCallbacks ).progress( progressCallbacks );
					return this;
				},
				always: function() {
					deferred.done.apply( deferred, arguments ).fail.apply( deferred, arguments );
					return this;
				},
				pipe: function( fnDone, fnFail, fnProgress ) {
					return jQuery.Deferred(function( newDefer ) {
						jQuery.each( {
							done: [ fnDone, "resolve" ],
							fail: [ fnFail, "reject" ],
							progress: [ fnProgress, "notify" ]
						}, function( handler, data ) {
							var fn = data[ 0 ],
								action = data[ 1 ],
								returned;
							if ( jQuery.isFunction( fn ) ) {
								deferred[ handler ](function() {
									returned = fn.apply( this, arguments );
									if ( returned && jQuery.isFunction( returned.promise ) ) {
										returned.promise().then( newDefer.resolve, newDefer.reject, newDefer.notify );
									} else {
										newDefer[ action + "With" ]( this === deferred ? newDefer : this, [ returned ] );
									}
								});
							} else {
								deferred[ handler ]( newDefer[ action ] );
							}
						});
					}).promise();
				},
				// Get a promise for this deferred
				// If obj is provided, the promise aspect is added to the object
				promise: function( obj ) {
					if ( obj == null ) {
						obj = promise;
					} else {
						for ( var key in promise ) {
							obj[ key ] = promise[ key ];
						}
					}
					return obj;
				}
			},
			deferred = promise.promise({}),
			key;

		for ( key in lists ) {
			deferred[ key ] = lists[ key ].fire;
			deferred[ key + "With" ] = lists[ key ].fireWith;
		}

		// Handle state
		deferred.done( function() {
			state = "resolved";
		}, failList.disable, progressList.lock ).fail( function() {
			state = "rejected";
		}, doneList.disable, progressList.lock );

		// Call given func if any
		if ( func ) {
			func.call( deferred, deferred );
		}

		// All done!
		return deferred;
	},

	// Deferred helper
	when: function( firstParam ) {
		var args = sliceDeferred.call( arguments, 0 ),
			i = 0,
			length = args.length,
			pValues = new Array( length ),
			count = length,
			pCount = length,
			deferred = length <= 1 && firstParam && jQuery.isFunction( firstParam.promise ) ?
				firstParam :
				jQuery.Deferred(),
			promise = deferred.promise();
		function resolveFunc( i ) {
			return function( value ) {
				args[ i ] = arguments.length > 1 ? sliceDeferred.call( arguments, 0 ) : value;
				if ( !( --count ) ) {
					deferred.resolveWith( deferred, args );
				}
			};
		}
		function progressFunc( i ) {
			return function( value ) {
				pValues[ i ] = arguments.length > 1 ? sliceDeferred.call( arguments, 0 ) : value;
				deferred.notifyWith( promise, pValues );
			};
		}
		if ( length > 1 ) {
			for ( ; i < length; i++ ) {
				if ( args[ i ] && args[ i ].promise && jQuery.isFunction( args[ i ].promise ) ) {
					args[ i ].promise().then( resolveFunc(i), deferred.reject, progressFunc(i) );
				} else {
					--count;
				}
			}
			if ( !count ) {
				deferred.resolveWith( deferred, args );
			}
		} else if ( deferred !== firstParam ) {
			deferred.resolveWith( deferred, length ? [ firstParam ] : [] );
		}
		return promise;
	}
});




jQuery.support = (function() {

	var support,
		all,
		a,
		select,
		opt,
		input,
		fragment,
		tds,
		events,
		eventName,
		i,
		isSupported,
		div = document.createElement( "div" ),
		documentElement = document.documentElement;

	// Preliminary tests
	div.setAttribute("className", "t");
	div.innerHTML = "   <link/><table></table><a href='/a' style='top:1px;float:left;opacity:.55;'>a</a><input type='checkbox'/>";

	all = div.getElementsByTagName( "*" );
	a = div.getElementsByTagName( "a" )[ 0 ];

	// Can't get basic test support
	if ( !all || !all.length || !a ) {
		return {};
	}

	// First batch of supports tests
	select = document.createElement( "select" );
	opt = select.appendChild( document.createElement("option") );
	input = div.getElementsByTagName( "input" )[ 0 ];

	support = {
		// IE strips leading whitespace when .innerHTML is used
		leadingWhitespace: ( div.firstChild.nodeType === 3 ),

		// Make sure that tbody elements aren't automatically inserted
		// IE will insert them into empty tables
		tbody: !div.getElementsByTagName("tbody").length,

		// Make sure that link elements get serialized correctly by innerHTML
		// This requires a wrapper element in IE
		htmlSerialize: !!div.getElementsByTagName("link").length,

		// Get the style information from getAttribute
		// (IE uses .cssText instead)
		style: /top/.test( a.getAttribute("style") ),

		// Make sure that URLs aren't manipulated
		// (IE normalizes it by default)
		hrefNormalized: ( a.getAttribute("href") === "/a" ),

		// Make sure that element opacity exists
		// (IE uses filter instead)
		// Use a regex to work around a WebKit issue. See #5145
		opacity: /^0.55/.test( a.style.opacity ),

		// Verify style float existence
		// (IE uses styleFloat instead of cssFloat)
		cssFloat: !!a.style.cssFloat,

		// Make sure that if no value is specified for a checkbox
		// that it defaults to "on".
		// (WebKit defaults to "" instead)
		checkOn: ( input.value === "on" ),

		// Make sure that a selected-by-default option has a working selected property.
		// (WebKit defaults to false instead of true, IE too, if it's in an optgroup)
		optSelected: opt.selected,

		// Test setAttribute on camelCase class. If it works, we need attrFixes when doing get/setAttribute (ie6/7)
		getSetAttribute: div.className !== "t",

		// Tests for enctype support on a form(#6743)
		enctype: !!document.createElement("form").enctype,

		// Makes sure cloning an html5 element does not cause problems
		// Where outerHTML is undefined, this still works
		html5Clone: document.createElement("nav").cloneNode( true ).outerHTML !== "<:nav></:nav>",

		// Will be defined later
		submitBubbles: true,
		changeBubbles: true,
		focusinBubbles: false,
		deleteExpando: true,
		noCloneEvent: true,
		inlineBlockNeedsLayout: false,
		shrinkWrapBlocks: false,
		reliableMarginRight: true,
		pixelMargin: true
	};

	// jQuery.boxModel DEPRECATED in 1.3, use jQuery.support.boxModel instead
	jQuery.boxModel = support.boxModel = (document.compatMode === "CSS1Compat");

	// Make sure checked status is properly cloned
	input.checked = true;
	support.noCloneChecked = input.cloneNode( true ).checked;

	// Make sure that the options inside disabled selects aren't marked as disabled
	// (WebKit marks them as disabled)
	select.disabled = true;
	support.optDisabled = !opt.disabled;

	// Test to see if it's possible to delete an expando from an element
	// Fails in Internet Explorer
	try {
		delete div.test;
	} catch( e ) {
		support.deleteExpando = false;
	}

	if ( !div.addEventListener && div.attachEvent && div.fireEvent ) {
		div.attachEvent( "onclick", function() {
			// Cloning a node shouldn't copy over any
			// bound event handlers (IE does this)
			support.noCloneEvent = false;
		});
		div.cloneNode( true ).fireEvent( "onclick" );
	}

	// Check if a radio maintains its value
	// after being appended to the DOM
	input = document.createElement("input");
	input.value = "t";
	input.setAttribute("type", "radio");
	support.radioValue = input.value === "t";

	input.setAttribute("checked", "checked");

	// #11217 - WebKit loses check when the name is after the checked attribute
	input.setAttribute( "name", "t" );

	div.appendChild( input );
	fragment = document.createDocumentFragment();
	fragment.appendChild( div.lastChild );

	// WebKit doesn't clone checked state correctly in fragments
	support.checkClone = fragment.cloneNode( true ).cloneNode( true ).lastChild.checked;

	// Check if a disconnected checkbox will retain its checked
	// value of true after appended to the DOM (IE6/7)
	support.appendChecked = input.checked;

	fragment.removeChild( input );
	fragment.appendChild( div );

	// Technique from Juriy Zaytsev
	// http://perfectionkills.com/detecting-event-support-without-browser-sniffing/
	// We only care about the case where non-standard event systems
	// are used, namely in IE. Short-circuiting here helps us to
	// avoid an eval call (in setAttribute) which can cause CSP
	// to go haywire. See: https://developer.mozilla.org/en/Security/CSP
	if ( div.attachEvent ) {
		for ( i in {
			submit: 1,
			change: 1,
			focusin: 1
		}) {
			eventName = "on" + i;
			isSupported = ( eventName in div );
			if ( !isSupported ) {
				div.setAttribute( eventName, "return;" );
				isSupported = ( typeof div[ eventName ] === "function" );
			}
			support[ i + "Bubbles" ] = isSupported;
		}
	}

	fragment.removeChild( div );

	// Null elements to avoid leaks in IE
	fragment = select = opt = div = input = null;

	// Run tests that need a body at doc ready
	jQuery(function() {
		var container, outer, inner, table, td, offsetSupport,
			marginDiv, conMarginTop, style, html, positionTopLeftWidthHeight,
			paddingMarginBorderVisibility, paddingMarginBorder,
			body = document.getElementsByTagName("body")[0];

		if ( !body ) {
			// Return for frameset docs that don't have a body
			return;
		}

		conMarginTop = 1;
		paddingMarginBorder = "padding:0;margin:0;border:";
		positionTopLeftWidthHeight = "position:absolute;top:0;left:0;width:1px;height:1px;";
		paddingMarginBorderVisibility = paddingMarginBorder + "0;visibility:hidden;";
		style = "style='" + positionTopLeftWidthHeight + paddingMarginBorder + "5px solid #000;";
		html = "<div " + style + "display:block;'><div style='" + paddingMarginBorder + "0;display:block;overflow:hidden;'></div></div>" +
			"<table " + style + "' cellpadding='0' cellspacing='0'>" +
			"<tr><td></td></tr></table>";

		container = document.createElement("div");
		container.style.cssText = paddingMarginBorderVisibility + "width:0;height:0;position:static;top:0;margin-top:" + conMarginTop + "px";
		body.insertBefore( container, body.firstChild );

		// Construct the test element
		div = document.createElement("div");
		container.appendChild( div );

		// Check if table cells still have offsetWidth/Height when they are set
		// to display:none and there are still other visible table cells in a
		// table row; if so, offsetWidth/Height are not reliable for use when
		// determining if an element has been hidden directly using
		// display:none (it is still safe to use offsets if a parent element is
		// hidden; don safety goggles and see bug #4512 for more information).
		// (only IE 8 fails this test)
		div.innerHTML = "<table><tr><td style='" + paddingMarginBorder + "0;display:none'></td><td>t</td></tr></table>";
		tds = div.getElementsByTagName( "td" );
		isSupported = ( tds[ 0 ].offsetHeight === 0 );

		tds[ 0 ].style.display = "";
		tds[ 1 ].style.display = "none";

		// Check if empty table cells still have offsetWidth/Height
		// (IE <= 8 fail this test)
		support.reliableHiddenOffsets = isSupported && ( tds[ 0 ].offsetHeight === 0 );

		// Check if div with explicit width and no margin-right incorrectly
		// gets computed margin-right based on width of container. For more
		// info see bug #3333
		// Fails in WebKit before Feb 2011 nightlies
		// WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
		if ( window.getComputedStyle ) {
			div.innerHTML = "";
			marginDiv = document.createElement( "div" );
			marginDiv.style.width = "0";
			marginDiv.style.marginRight = "0";
			div.style.width = "2px";
			div.appendChild( marginDiv );
			support.reliableMarginRight =
				( parseInt( ( window.getComputedStyle( marginDiv, null ) || { marginRight: 0 } ).marginRight, 10 ) || 0 ) === 0;
		}

		if ( typeof div.style.zoom !== "undefined" ) {
			// Check if natively block-level elements act like inline-block
			// elements when setting their display to 'inline' and giving
			// them layout
			// (IE < 8 does this)
			div.innerHTML = "";
			div.style.width = div.style.padding = "1px";
			div.style.border = 0;
			div.style.overflow = "hidden";
			div.style.display = "inline";
			div.style.zoom = 1;
			support.inlineBlockNeedsLayout = ( div.offsetWidth === 3 );

			// Check if elements with layout shrink-wrap their children
			// (IE 6 does this)
			div.style.display = "block";
			div.style.overflow = "visible";
			div.innerHTML = "<div style='width:5px;'></div>";
			support.shrinkWrapBlocks = ( div.offsetWidth !== 3 );
		}

		div.style.cssText = positionTopLeftWidthHeight + paddingMarginBorderVisibility;
		div.innerHTML = html;

		outer = div.firstChild;
		inner = outer.firstChild;
		td = outer.nextSibling.firstChild.firstChild;

		offsetSupport = {
			doesNotAddBorder: ( inner.offsetTop !== 5 ),
			doesAddBorderForTableAndCells: ( td.offsetTop === 5 )
		};

		inner.style.position = "fixed";
		inner.style.top = "20px";

		// safari subtracts parent border width here which is 5px
		offsetSupport.fixedPosition = ( inner.offsetTop === 20 || inner.offsetTop === 15 );
		inner.style.position = inner.style.top = "";

		outer.style.overflow = "hidden";
		outer.style.position = "relative";

		offsetSupport.subtractsBorderForOverflowNotVisible = ( inner.offsetTop === -5 );
		offsetSupport.doesNotIncludeMarginInBodyOffset = ( body.offsetTop !== conMarginTop );

		if ( window.getComputedStyle ) {
			div.style.marginTop = "1%";
			support.pixelMargin = ( window.getComputedStyle( div, null ) || { marginTop: 0 } ).marginTop !== "1%";
		}

		if ( typeof container.style.zoom !== "undefined" ) {
			container.style.zoom = 1;
		}

		body.removeChild( container );
		marginDiv = div = container = null;

		jQuery.extend( support, offsetSupport );
	});

	return support;
})();




var rbrace = /^(?:\{.*\}|\[.*\])$/,
	rmultiDash = /([A-Z])/g;

jQuery.extend({
	cache: {},

	// Please use with caution
	uuid: 0,

	// Unique for each copy of jQuery on the page
	// Non-digits removed to match rinlinejQuery
	expando: "jQuery" + ( jQuery.fn.jquery + Math.random() ).replace( /\D/g, "" ),

	// The following elements throw uncatchable exceptions if you
	// attempt to add expando properties to them.
	noData: {
		"embed": true,
		// Ban all objects except for Flash (which handle expandos)
		"object": "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000",
		"applet": true
	},

	hasData: function( elem ) {
		elem = elem.nodeType ? jQuery.cache[ elem[jQuery.expando] ] : elem[ jQuery.expando ];
		return !!elem && !isEmptyDataObject( elem );
	},

	data: function( elem, name, data, pvt /* Internal Use Only */ ) {
		if ( !jQuery.acceptData( elem ) ) {
			return;
		}

		var privateCache, thisCache, ret,
			internalKey = jQuery.expando,
			getByName = typeof name === "string",

			// We have to handle DOM nodes and JS objects differently because IE6-7
			// can't GC object references properly across the DOM-JS boundary
			isNode = elem.nodeType,

			// Only DOM nodes need the global jQuery cache; JS object data is
			// attached directly to the object so GC can occur automatically
			cache = isNode ? jQuery.cache : elem,

			// Only defining an ID for JS objects if its cache already exists allows
			// the code to shortcut on the same path as a DOM node with no cache
			id = isNode ? elem[ internalKey ] : elem[ internalKey ] && internalKey,
			isEvents = name === "events";

		// Avoid doing any more work than we need to when trying to get data on an
		// object that has no data at all
		if ( (!id || !cache[id] || (!isEvents && !pvt && !cache[id].data)) && getByName && data === undefined ) {
			return;
		}

		if ( !id ) {
			// Only DOM nodes need a new unique ID for each element since their data
			// ends up in the global cache
			if ( isNode ) {
				elem[ internalKey ] = id = ++jQuery.uuid;
			} else {
				id = internalKey;
			}
		}

		if ( !cache[ id ] ) {
			cache[ id ] = {};

			// Avoids exposing jQuery metadata on plain JS objects when the object
			// is serialized using JSON.stringify
			if ( !isNode ) {
				cache[ id ].toJSON = jQuery.noop;
			}
		}

		// An object can be passed to jQuery.data instead of a key/value pair; this gets
		// shallow copied over onto the existing cache
		if ( typeof name === "object" || typeof name === "function" ) {
			if ( pvt ) {
				cache[ id ] = jQuery.extend( cache[ id ], name );
			} else {
				cache[ id ].data = jQuery.extend( cache[ id ].data, name );
			}
		}

		privateCache = thisCache = cache[ id ];

		// jQuery data() is stored in a separate object inside the object's internal data
		// cache in order to avoid key collisions between internal data and user-defined
		// data.
		if ( !pvt ) {
			if ( !thisCache.data ) {
				thisCache.data = {};
			}

			thisCache = thisCache.data;
		}

		if ( data !== undefined ) {
			thisCache[ jQuery.camelCase( name ) ] = data;
		}

		// Users should not attempt to inspect the internal events object using jQuery.data,
		// it is undocumented and subject to change. But does anyone listen? No.
		if ( isEvents && !thisCache[ name ] ) {
			return privateCache.events;
		}

		// Check for both converted-to-camel and non-converted data property names
		// If a data property was specified
		if ( getByName ) {

			// First Try to find as-is property data
			ret = thisCache[ name ];

			// Test for null|undefined property data
			if ( ret == null ) {

				// Try to find the camelCased property
				ret = thisCache[ jQuery.camelCase( name ) ];
			}
		} else {
			ret = thisCache;
		}

		return ret;
	},

	removeData: function( elem, name, pvt /* Internal Use Only */ ) {
		if ( !jQuery.acceptData( elem ) ) {
			return;
		}

		var thisCache, i, l,

			// Reference to internal data cache key
			internalKey = jQuery.expando,

			isNode = elem.nodeType,

			// See jQuery.data for more information
			cache = isNode ? jQuery.cache : elem,

			// See jQuery.data for more information
			id = isNode ? elem[ internalKey ] : internalKey;

		// If there is already no cache entry for this object, there is no
		// purpose in continuing
		if ( !cache[ id ] ) {
			return;
		}

		if ( name ) {

			thisCache = pvt ? cache[ id ] : cache[ id ].data;

			if ( thisCache ) {

				// Support array or space separated string names for data keys
				if ( !jQuery.isArray( name ) ) {

					// try the string as a key before any manipulation
					if ( name in thisCache ) {
						name = [ name ];
					} else {

						// split the camel cased version by spaces unless a key with the spaces exists
						name = jQuery.camelCase( name );
						if ( name in thisCache ) {
							name = [ name ];
						} else {
							name = name.split( " " );
						}
					}
				}

				for ( i = 0, l = name.length; i < l; i++ ) {
					delete thisCache[ name[i] ];
				}

				// If there is no data left in the cache, we want to continue
				// and let the cache object itself get destroyed
				if ( !( pvt ? isEmptyDataObject : jQuery.isEmptyObject )( thisCache ) ) {
					return;
				}
			}
		}

		// See jQuery.data for more information
		if ( !pvt ) {
			delete cache[ id ].data;

			// Don't destroy the parent cache unless the internal data object
			// had been the only thing left in it
			if ( !isEmptyDataObject(cache[ id ]) ) {
				return;
			}
		}

		// Browsers that fail expando deletion also refuse to delete expandos on
		// the window, but it will allow it on all other JS objects; other browsers
		// don't care
		// Ensure that `cache` is not a window object #10080
		if ( jQuery.support.deleteExpando || !cache.setInterval ) {
			delete cache[ id ];
		} else {
			cache[ id ] = null;
		}

		// We destroyed the cache and need to eliminate the expando on the node to avoid
		// false lookups in the cache for entries that no longer exist
		if ( isNode ) {
			// IE does not allow us to delete expando properties from nodes,
			// nor does it have a removeAttribute function on Document nodes;
			// we must handle all of these cases
			if ( jQuery.support.deleteExpando ) {
				delete elem[ internalKey ];
			} else if ( elem.removeAttribute ) {
				elem.removeAttribute( internalKey );
			} else {
				elem[ internalKey ] = null;
			}
		}
	},

	// For internal use only.
	_data: function( elem, name, data ) {
		return jQuery.data( elem, name, data, true );
	},

	// A method for determining if a DOM node can handle the data expando
	acceptData: function( elem ) {
		if ( elem.nodeName ) {
			var match = jQuery.noData[ elem.nodeName.toLowerCase() ];

			if ( match ) {
				return !(match === true || elem.getAttribute("classid") !== match);
			}
		}

		return true;
	}
});

jQuery.fn.extend({
	data: function( key, value ) {
		var parts, part, attr, name, l,
			elem = this[0],
			i = 0,
			data = null;

		// Gets all values
		if ( key === undefined ) {
			if ( this.length ) {
				data = jQuery.data( elem );

				if ( elem.nodeType === 1 && !jQuery._data( elem, "parsedAttrs" ) ) {
					attr = elem.attributes;
					for ( l = attr.length; i < l; i++ ) {
						name = attr[i].name;

						if ( name.indexOf( "data-" ) === 0 ) {
							name = jQuery.camelCase( name.substring(5) );

							dataAttr( elem, name, data[ name ] );
						}
					}
					jQuery._data( elem, "parsedAttrs", true );
				}
			}

			return data;
		}

		// Sets multiple values
		if ( typeof key === "object" ) {
			return this.each(function() {
				jQuery.data( this, key );
			});
		}

		parts = key.split( ".", 2 );
		parts[1] = parts[1] ? "." + parts[1] : "";
		part = parts[1] + "!";

		return jQuery.access( this, function( value ) {

			if ( value === undefined ) {
				data = this.triggerHandler( "getData" + part, [ parts[0] ] );

				// Try to fetch any internally stored data first
				if ( data === undefined && elem ) {
					data = jQuery.data( elem, key );
					data = dataAttr( elem, key, data );
				}

				return data === undefined && parts[1] ?
					this.data( parts[0] ) :
					data;
			}

			parts[1] = value;
			this.each(function() {
				var self = jQuery( this );

				self.triggerHandler( "setData" + part, parts );
				jQuery.data( this, key, value );
				self.triggerHandler( "changeData" + part, parts );
			});
		}, null, value, arguments.length > 1, null, false );
	},

	removeData: function( key ) {
		return this.each(function() {
			jQuery.removeData( this, key );
		});
	}
});

function dataAttr( elem, key, data ) {
	// If nothing was found internally, try to fetch any
	// data from the HTML5 data-* attribute
	if ( data === undefined && elem.nodeType === 1 ) {

		var name = "data-" + key.replace( rmultiDash, "-$1" ).toLowerCase();

		data = elem.getAttribute( name );

		if ( typeof data === "string" ) {
			try {
				data = data === "true" ? true :
				data === "false" ? false :
				data === "null" ? null :
				jQuery.isNumeric( data ) ? +data :
					rbrace.test( data ) ? jQuery.parseJSON( data ) :
					data;
			} catch( e ) {}

			// Make sure we set the data so it isn't changed later
			jQuery.data( elem, key, data );

		} else {
			data = undefined;
		}
	}

	return data;
}

// checks a cache object for emptiness
function isEmptyDataObject( obj ) {
	for ( var name in obj ) {

		// if the public data object is empty, the private is still empty
		if ( name === "data" && jQuery.isEmptyObject( obj[name] ) ) {
			continue;
		}
		if ( name !== "toJSON" ) {
			return false;
		}
	}

	return true;
}




function handleQueueMarkDefer( elem, type, src ) {
	var deferDataKey = type + "defer",
		queueDataKey = type + "queue",
		markDataKey = type + "mark",
		defer = jQuery._data( elem, deferDataKey );
	if ( defer &&
		( src === "queue" || !jQuery._data(elem, queueDataKey) ) &&
		( src === "mark" || !jQuery._data(elem, markDataKey) ) ) {
		// Give room for hard-coded callbacks to fire first
		// and eventually mark/queue something else on the element
		setTimeout( function() {
			if ( !jQuery._data( elem, queueDataKey ) &&
				!jQuery._data( elem, markDataKey ) ) {
				jQuery.removeData( elem, deferDataKey, true );
				defer.fire();
			}
		}, 0 );
	}
}

jQuery.extend({

	_mark: function( elem, type ) {
		if ( elem ) {
			type = ( type || "fx" ) + "mark";
			jQuery._data( elem, type, (jQuery._data( elem, type ) || 0) + 1 );
		}
	},

	_unmark: function( force, elem, type ) {
		if ( force !== true ) {
			type = elem;
			elem = force;
			force = false;
		}
		if ( elem ) {
			type = type || "fx";
			var key = type + "mark",
				count = force ? 0 : ( (jQuery._data( elem, key ) || 1) - 1 );
			if ( count ) {
				jQuery._data( elem, key, count );
			} else {
				jQuery.removeData( elem, key, true );
				handleQueueMarkDefer( elem, type, "mark" );
			}
		}
	},

	queue: function( elem, type, data ) {
		var q;
		if ( elem ) {
			type = ( type || "fx" ) + "queue";
			q = jQuery._data( elem, type );

			// Speed up dequeue by getting out quickly if this is just a lookup
			if ( data ) {
				if ( !q || jQuery.isArray(data) ) {
					q = jQuery._data( elem, type, jQuery.makeArray(data) );
				} else {
					q.push( data );
				}
			}
			return q || [];
		}
	},

	dequeue: function( elem, type ) {
		type = type || "fx";

		var queue = jQuery.queue( elem, type ),
			fn = queue.shift(),
			hooks = {};

		// If the fx queue is dequeued, always remove the progress sentinel
		if ( fn === "inprogress" ) {
			fn = queue.shift();
		}

		if ( fn ) {
			// Add a progress sentinel to prevent the fx queue from being
			// automatically dequeued
			if ( type === "fx" ) {
				queue.unshift( "inprogress" );
			}

			jQuery._data( elem, type + ".run", hooks );
			fn.call( elem, function() {
				jQuery.dequeue( elem, type );
			}, hooks );
		}

		if ( !queue.length ) {
			jQuery.removeData( elem, type + "queue " + type + ".run", true );
			handleQueueMarkDefer( elem, type, "queue" );
		}
	}
});

jQuery.fn.extend({
	queue: function( type, data ) {
		var setter = 2;

		if ( typeof type !== "string" ) {
			data = type;
			type = "fx";
			setter--;
		}

		if ( arguments.length < setter ) {
			return jQuery.queue( this[0], type );
		}

		return data === undefined ?
			this :
			this.each(function() {
				var queue = jQuery.queue( this, type, data );

				if ( type === "fx" && queue[0] !== "inprogress" ) {
					jQuery.dequeue( this, type );
				}
			});
	},
	dequeue: function( type ) {
		return this.each(function() {
			jQuery.dequeue( this, type );
		});
	},
	// Based off of the plugin by Clint Helfers, with permission.
	// http://blindsignals.com/index.php/2009/07/jquery-delay/
	delay: function( time, type ) {
		time = jQuery.fx ? jQuery.fx.speeds[ time ] || time : time;
		type = type || "fx";

		return this.queue( type, function( next, hooks ) {
			var timeout = setTimeout( next, time );
			hooks.stop = function() {
				clearTimeout( timeout );
			};
		});
	},
	clearQueue: function( type ) {
		return this.queue( type || "fx", [] );
	},
	// Get a promise resolved when queues of a certain type
	// are emptied (fx is the type by default)
	promise: function( type, object ) {
		if ( typeof type !== "string" ) {
			object = type;
			type = undefined;
		}
		type = type || "fx";
		var defer = jQuery.Deferred(),
			elements = this,
			i = elements.length,
			count = 1,
			deferDataKey = type + "defer",
			queueDataKey = type + "queue",
			markDataKey = type + "mark",
			tmp;
		function resolve() {
			if ( !( --count ) ) {
				defer.resolveWith( elements, [ elements ] );
			}
		}
		while( i-- ) {
			if (( tmp = jQuery.data( elements[ i ], deferDataKey, undefined, true ) ||
					( jQuery.data( elements[ i ], queueDataKey, undefined, true ) ||
						jQuery.data( elements[ i ], markDataKey, undefined, true ) ) &&
					jQuery.data( elements[ i ], deferDataKey, jQuery.Callbacks( "once memory" ), true ) )) {
				count++;
				tmp.add( resolve );
			}
		}
		resolve();
		return defer.promise( object );
	}
});




var rclass = /[\n\t\r]/g,
	rspace = /\s+/,
	rreturn = /\r/g,
	rtype = /^(?:button|input)$/i,
	rfocusable = /^(?:button|input|object|select|textarea)$/i,
	rclickable = /^a(?:rea)?$/i,
	rboolean = /^(?:autofocus|autoplay|async|checked|controls|defer|disabled|hidden|loop|multiple|open|readonly|required|scoped|selected)$/i,
	getSetAttribute = jQuery.support.getSetAttribute,
	nodeHook, boolHook, fixSpecified;

jQuery.fn.extend({
	attr: function( name, value ) {
		return jQuery.access( this, jQuery.attr, name, value, arguments.length > 1 );
	},

	removeAttr: function( name ) {
		return this.each(function() {
			jQuery.removeAttr( this, name );
		});
	},

	prop: function( name, value ) {
		return jQuery.access( this, jQuery.prop, name, value, arguments.length > 1 );
	},

	removeProp: function( name ) {
		name = jQuery.propFix[ name ] || name;
		return this.each(function() {
			// try/catch handles cases where IE balks (such as removing a property on window)
			try {
				this[ name ] = undefined;
				delete this[ name ];
			} catch( e ) {}
		});
	},

	addClass: function( value ) {
		var classNames, i, l, elem,
			setClass, c, cl;

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( j ) {
				jQuery( this ).addClass( value.call(this, j, this.className) );
			});
		}

		if ( value && typeof value === "string" ) {
			classNames = value.split( rspace );

			for ( i = 0, l = this.length; i < l; i++ ) {
				elem = this[ i ];

				if ( elem.nodeType === 1 ) {
					if ( !elem.className && classNames.length === 1 ) {
						elem.className = value;

					} else {
						setClass = " " + elem.className + " ";

						for ( c = 0, cl = classNames.length; c < cl; c++ ) {
							if ( !~setClass.indexOf( " " + classNames[ c ] + " " ) ) {
								setClass += classNames[ c ] + " ";
							}
						}
						elem.className = jQuery.trim( setClass );
					}
				}
			}
		}

		return this;
	},

	removeClass: function( value ) {
		var classNames, i, l, elem, className, c, cl;

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( j ) {
				jQuery( this ).removeClass( value.call(this, j, this.className) );
			});
		}

		if ( (value && typeof value === "string") || value === undefined ) {
			classNames = ( value || "" ).split( rspace );

			for ( i = 0, l = this.length; i < l; i++ ) {
				elem = this[ i ];

				if ( elem.nodeType === 1 && elem.className ) {
					if ( value ) {
						className = (" " + elem.className + " ").replace( rclass, " " );
						for ( c = 0, cl = classNames.length; c < cl; c++ ) {
							className = className.replace(" " + classNames[ c ] + " ", " ");
						}
						elem.className = jQuery.trim( className );

					} else {
						elem.className = "";
					}
				}
			}
		}

		return this;
	},

	toggleClass: function( value, stateVal ) {
		var type = typeof value,
			isBool = typeof stateVal === "boolean";

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( i ) {
				jQuery( this ).toggleClass( value.call(this, i, this.className, stateVal), stateVal );
			});
		}

		return this.each(function() {
			if ( type === "string" ) {
				// toggle individual class names
				var className,
					i = 0,
					self = jQuery( this ),
					state = stateVal,
					classNames = value.split( rspace );

				while ( (className = classNames[ i++ ]) ) {
					// check each className given, space seperated list
					state = isBool ? state : !self.hasClass( className );
					self[ state ? "addClass" : "removeClass" ]( className );
				}

			} else if ( type === "undefined" || type === "boolean" ) {
				if ( this.className ) {
					// store className if set
					jQuery._data( this, "__className__", this.className );
				}

				// toggle whole className
				this.className = this.className || value === false ? "" : jQuery._data( this, "__className__" ) || "";
			}
		});
	},

	hasClass: function( selector ) {
		var className = " " + selector + " ",
			i = 0,
			l = this.length;
		for ( ; i < l; i++ ) {
			if ( this[i].nodeType === 1 && (" " + this[i].className + " ").replace(rclass, " ").indexOf( className ) > -1 ) {
				return true;
			}
		}

		return false;
	},

	val: function( value ) {
		var hooks, ret, isFunction,
			elem = this[0];

		if ( !arguments.length ) {
			if ( elem ) {
				hooks = jQuery.valHooks[ elem.type ] || jQuery.valHooks[ elem.nodeName.toLowerCase() ];

				if ( hooks && "get" in hooks && (ret = hooks.get( elem, "value" )) !== undefined ) {
					return ret;
				}

				ret = elem.value;

				return typeof ret === "string" ?
					// handle most common string cases
					ret.replace(rreturn, "") :
					// handle cases where value is null/undef or number
					ret == null ? "" : ret;
			}

			return;
		}

		isFunction = jQuery.isFunction( value );

		return this.each(function( i ) {
			var self = jQuery(this), val;

			if ( this.nodeType !== 1 ) {
				return;
			}

			if ( isFunction ) {
				val = value.call( this, i, self.val() );
			} else {
				val = value;
			}

			// Treat null/undefined as ""; convert numbers to string
			if ( val == null ) {
				val = "";
			} else if ( typeof val === "number" ) {
				val += "";
			} else if ( jQuery.isArray( val ) ) {
				val = jQuery.map(val, function ( value ) {
					return value == null ? "" : value + "";
				});
			}

			hooks = jQuery.valHooks[ this.type ] || jQuery.valHooks[ this.nodeName.toLowerCase() ];

			// If set returns undefined, fall back to normal setting
			if ( !hooks || !("set" in hooks) || hooks.set( this, val, "value" ) === undefined ) {
				this.value = val;
			}
		});
	}
});

jQuery.extend({
	valHooks: {
		option: {
			get: function( elem ) {
				// attributes.value is undefined in Blackberry 4.7 but
				// uses .value. See #6932
				var val = elem.attributes.value;
				return !val || val.specified ? elem.value : elem.text;
			}
		},
		select: {
			get: function( elem ) {
				var value, i, max, option,
					index = elem.selectedIndex,
					values = [],
					options = elem.options,
					one = elem.type === "select-one";

				// Nothing was selected
				if ( index < 0 ) {
					return null;
				}

				// Loop through all the selected options
				i = one ? index : 0;
				max = one ? index + 1 : options.length;
				for ( ; i < max; i++ ) {
					option = options[ i ];

					// Don't return options that are disabled or in a disabled optgroup
					if ( option.selected && (jQuery.support.optDisabled ? !option.disabled : option.getAttribute("disabled") === null) &&
							(!option.parentNode.disabled || !jQuery.nodeName( option.parentNode, "optgroup" )) ) {

						// Get the specific value for the option
						value = jQuery( option ).val();

						// We don't need an array for one selects
						if ( one ) {
							return value;
						}

						// Multi-Selects return an array
						values.push( value );
					}
				}

				// Fixes Bug #2551 -- select.val() broken in IE after form.reset()
				if ( one && !values.length && options.length ) {
					return jQuery( options[ index ] ).val();
				}

				return values;
			},

			set: function( elem, value ) {
				var values = jQuery.makeArray( value );

				jQuery(elem).find("option").each(function() {
					this.selected = jQuery.inArray( jQuery(this).val(), values ) >= 0;
				});

				if ( !values.length ) {
					elem.selectedIndex = -1;
				}
				return values;
			}
		}
	},

	attrFn: {
		val: true,
		css: true,
		html: true,
		text: true,
		data: true,
		width: true,
		height: true,
		offset: true
	},

	attr: function( elem, name, value, pass ) {
		var ret, hooks, notxml,
			nType = elem.nodeType;

		// don't get/set attributes on text, comment and attribute nodes
		if ( !elem || nType === 3 || nType === 8 || nType === 2 ) {
			return;
		}

		if ( pass && name in jQuery.attrFn ) {
			return jQuery( elem )[ name ]( value );
		}

		// Fallback to prop when attributes are not supported
		if ( typeof elem.getAttribute === "undefined" ) {
			return jQuery.prop( elem, name, value );
		}

		notxml = nType !== 1 || !jQuery.isXMLDoc( elem );

		// All attributes are lowercase
		// Grab necessary hook if one is defined
		if ( notxml ) {
			name = name.toLowerCase();
			hooks = jQuery.attrHooks[ name ] || ( rboolean.test( name ) ? boolHook : nodeHook );
		}

		if ( value !== undefined ) {

			if ( value === null ) {
				jQuery.removeAttr( elem, name );
				return;

			} else if ( hooks && "set" in hooks && notxml && (ret = hooks.set( elem, value, name )) !== undefined ) {
				return ret;

			} else {
				elem.setAttribute( name, "" + value );
				return value;
			}

		} else if ( hooks && "get" in hooks && notxml && (ret = hooks.get( elem, name )) !== null ) {
			return ret;

		} else {

			ret = elem.getAttribute( name );

			// Non-existent attributes return null, we normalize to undefined
			return ret === null ?
				undefined :
				ret;
		}
	},

	removeAttr: function( elem, value ) {
		var propName, attrNames, name, l, isBool,
			i = 0;

		if ( value && elem.nodeType === 1 ) {
			attrNames = value.toLowerCase().split( rspace );
			l = attrNames.length;

			for ( ; i < l; i++ ) {
				name = attrNames[ i ];

				if ( name ) {
					propName = jQuery.propFix[ name ] || name;
					isBool = rboolean.test( name );

					// See #9699 for explanation of this approach (setting first, then removal)
					// Do not do this for boolean attributes (see #10870)
					if ( !isBool ) {
						jQuery.attr( elem, name, "" );
					}
					elem.removeAttribute( getSetAttribute ? name : propName );

					// Set corresponding property to false for boolean attributes
					if ( isBool && propName in elem ) {
						elem[ propName ] = false;
					}
				}
			}
		}
	},

	attrHooks: {
		type: {
			set: function( elem, value ) {
				// We can't allow the type property to be changed (since it causes problems in IE)
				if ( rtype.test( elem.nodeName ) && elem.parentNode ) {
					jQuery.error( "type property can't be changed" );
				} else if ( !jQuery.support.radioValue && value === "radio" && jQuery.nodeName(elem, "input") ) {
					// Setting the type on a radio button after the value resets the value in IE6-9
					// Reset value to it's default in case type is set after value
					// This is for element creation
					var val = elem.value;
					elem.setAttribute( "type", value );
					if ( val ) {
						elem.value = val;
					}
					return value;
				}
			}
		},
		// Use the value property for back compat
		// Use the nodeHook for button elements in IE6/7 (#1954)
		value: {
			get: function( elem, name ) {
				if ( nodeHook && jQuery.nodeName( elem, "button" ) ) {
					return nodeHook.get( elem, name );
				}
				return name in elem ?
					elem.value :
					null;
			},
			set: function( elem, value, name ) {
				if ( nodeHook && jQuery.nodeName( elem, "button" ) ) {
					return nodeHook.set( elem, value, name );
				}
				// Does not return so that setAttribute is also used
				elem.value = value;
			}
		}
	},

	propFix: {
		tabindex: "tabIndex",
		readonly: "readOnly",
		"for": "htmlFor",
		"class": "className",
		maxlength: "maxLength",
		cellspacing: "cellSpacing",
		cellpadding: "cellPadding",
		rowspan: "rowSpan",
		colspan: "colSpan",
		usemap: "useMap",
		frameborder: "frameBorder",
		contenteditable: "contentEditable"
	},

	prop: function( elem, name, value ) {
		var ret, hooks, notxml,
			nType = elem.nodeType;

		// don't get/set properties on text, comment and attribute nodes
		if ( !elem || nType === 3 || nType === 8 || nType === 2 ) {
			return;
		}

		notxml = nType !== 1 || !jQuery.isXMLDoc( elem );

		if ( notxml ) {
			// Fix name and attach hooks
			name = jQuery.propFix[ name ] || name;
			hooks = jQuery.propHooks[ name ];
		}

		if ( value !== undefined ) {
			if ( hooks && "set" in hooks && (ret = hooks.set( elem, value, name )) !== undefined ) {
				return ret;

			} else {
				return ( elem[ name ] = value );
			}

		} else {
			if ( hooks && "get" in hooks && (ret = hooks.get( elem, name )) !== null ) {
				return ret;

			} else {
				return elem[ name ];
			}
		}
	},

	propHooks: {
		tabIndex: {
			get: function( elem ) {
				// elem.tabIndex doesn't always return the correct value when it hasn't been explicitly set
				// http://fluidproject.org/blog/2008/01/09/getting-setting-and-removing-tabindex-values-with-javascript/
				var attributeNode = elem.getAttributeNode("tabindex");

				return attributeNode && attributeNode.specified ?
					parseInt( attributeNode.value, 10 ) :
					rfocusable.test( elem.nodeName ) || rclickable.test( elem.nodeName ) && elem.href ?
						0 :
						undefined;
			}
		}
	}
});

// Add the tabIndex propHook to attrHooks for back-compat (different case is intentional)
jQuery.attrHooks.tabindex = jQuery.propHooks.tabIndex;

// Hook for boolean attributes
boolHook = {
	get: function( elem, name ) {
		// Align boolean attributes with corresponding properties
		// Fall back to attribute presence where some booleans are not supported
		var attrNode,
			property = jQuery.prop( elem, name );
		return property === true || typeof property !== "boolean" && ( attrNode = elem.getAttributeNode(name) ) && attrNode.nodeValue !== false ?
			name.toLowerCase() :
			undefined;
	},
	set: function( elem, value, name ) {
		var propName;
		if ( value === false ) {
			// Remove boolean attributes when set to false
			jQuery.removeAttr( elem, name );
		} else {
			// value is true since we know at this point it's type boolean and not false
			// Set boolean attributes to the same name and set the DOM property
			propName = jQuery.propFix[ name ] || name;
			if ( propName in elem ) {
				// Only set the IDL specifically if it already exists on the element
				elem[ propName ] = true;
			}

			elem.setAttribute( name, name.toLowerCase() );
		}
		return name;
	}
};

// IE6/7 do not support getting/setting some attributes with get/setAttribute
if ( !getSetAttribute ) {

	fixSpecified = {
		name: true,
		id: true,
		coords: true
	};

	// Use this for any attribute in IE6/7
	// This fixes almost every IE6/7 issue
	nodeHook = jQuery.valHooks.button = {
		get: function( elem, name ) {
			var ret;
			ret = elem.getAttributeNode( name );
			return ret && ( fixSpecified[ name ] ? ret.nodeValue !== "" : ret.specified ) ?
				ret.nodeValue :
				undefined;
		},
		set: function( elem, value, name ) {
			// Set the existing or create a new attribute node
			var ret = elem.getAttributeNode( name );
			if ( !ret ) {
				ret = document.createAttribute( name );
				elem.setAttributeNode( ret );
			}
			return ( ret.nodeValue = value + "" );
		}
	};

	// Apply the nodeHook to tabindex
	jQuery.attrHooks.tabindex.set = nodeHook.set;

	// Set width and height to auto instead of 0 on empty string( Bug #8150 )
	// This is for removals
	jQuery.each([ "width", "height" ], function( i, name ) {
		jQuery.attrHooks[ name ] = jQuery.extend( jQuery.attrHooks[ name ], {
			set: function( elem, value ) {
				if ( value === "" ) {
					elem.setAttribute( name, "auto" );
					return value;
				}
			}
		});
	});

	// Set contenteditable to false on removals(#10429)
	// Setting to empty string throws an error as an invalid value
	jQuery.attrHooks.contenteditable = {
		get: nodeHook.get,
		set: function( elem, value, name ) {
			if ( value === "" ) {
				value = "false";
			}
			nodeHook.set( elem, value, name );
		}
	};
}


// Some attributes require a special call on IE
if ( !jQuery.support.hrefNormalized ) {
	jQuery.each([ "href", "src", "width", "height" ], function( i, name ) {
		jQuery.attrHooks[ name ] = jQuery.extend( jQuery.attrHooks[ name ], {
			get: function( elem ) {
				var ret = elem.getAttribute( name, 2 );
				return ret === null ? undefined : ret;
			}
		});
	});
}

if ( !jQuery.support.style ) {
	jQuery.attrHooks.style = {
		get: function( elem ) {
			// Return undefined in the case of empty string
			// Normalize to lowercase since IE uppercases css property names
			return elem.style.cssText.toLowerCase() || undefined;
		},
		set: function( elem, value ) {
			return ( elem.style.cssText = "" + value );
		}
	};
}

// Safari mis-reports the default selected property of an option
// Accessing the parent's selectedIndex property fixes it
if ( !jQuery.support.optSelected ) {
	jQuery.propHooks.selected = jQuery.extend( jQuery.propHooks.selected, {
		get: function( elem ) {
			var parent = elem.parentNode;

			if ( parent ) {
				parent.selectedIndex;

				// Make sure that it also works with optgroups, see #5701
				if ( parent.parentNode ) {
					parent.parentNode.selectedIndex;
				}
			}
			return null;
		}
	});
}

// IE6/7 call enctype encoding
if ( !jQuery.support.enctype ) {
	jQuery.propFix.enctype = "encoding";
}

// Radios and checkboxes getter/setter
if ( !jQuery.support.checkOn ) {
	jQuery.each([ "radio", "checkbox" ], function() {
		jQuery.valHooks[ this ] = {
			get: function( elem ) {
				// Handle the case where in Webkit "" is returned instead of "on" if a value isn't specified
				return elem.getAttribute("value") === null ? "on" : elem.value;
			}
		};
	});
}
jQuery.each([ "radio", "checkbox" ], function() {
	jQuery.valHooks[ this ] = jQuery.extend( jQuery.valHooks[ this ], {
		set: function( elem, value ) {
			if ( jQuery.isArray( value ) ) {
				return ( elem.checked = jQuery.inArray( jQuery(elem).val(), value ) >= 0 );
			}
		}
	});
});




var rformElems = /^(?:textarea|input|select)$/i,
	rtypenamespace = /^([^\.]*)?(?:\.(.+))?$/,
	rhoverHack = /(?:^|\s)hover(\.\S+)?\b/,
	rkeyEvent = /^key/,
	rmouseEvent = /^(?:mouse|contextmenu)|click/,
	rfocusMorph = /^(?:focusinfocus|focusoutblur)$/,
	rquickIs = /^(\w*)(?:#([\w\-]+))?(?:\.([\w\-]+))?$/,
	quickParse = function( selector ) {
		var quick = rquickIs.exec( selector );
		if ( quick ) {
			//   0  1    2   3
			// [ _, tag, id, class ]
			quick[1] = ( quick[1] || "" ).toLowerCase();
			quick[3] = quick[3] && new RegExp( "(?:^|\\s)" + quick[3] + "(?:\\s|$)" );
		}
		return quick;
	},
	quickIs = function( elem, m ) {
		var attrs = elem.attributes || {};
		return (
			(!m[1] || elem.nodeName.toLowerCase() === m[1]) &&
			(!m[2] || (attrs.id || {}).value === m[2]) &&
			(!m[3] || m[3].test( (attrs[ "class" ] || {}).value ))
		);
	},
	hoverHack = function( events ) {
		return jQuery.event.special.hover ? events : events.replace( rhoverHack, "mouseenter$1 mouseleave$1" );
	};

/*
 * Helper functions for managing events -- not part of the public interface.
 * Props to Dean Edwards' addEvent library for many of the ideas.
 */
jQuery.event = {

	add: function( elem, types, handler, data, selector ) {

		var elemData, eventHandle, events,
			t, tns, type, namespaces, handleObj,
			handleObjIn, quick, handlers, special;

		// Don't attach events to noData or text/comment nodes (allow plain objects tho)
		if ( elem.nodeType === 3 || elem.nodeType === 8 || !types || !handler || !(elemData = jQuery._data( elem )) ) {
			return;
		}

		// Caller can pass in an object of custom data in lieu of the handler
		if ( handler.handler ) {
			handleObjIn = handler;
			handler = handleObjIn.handler;
			selector = handleObjIn.selector;
		}

		// Make sure that the handler has a unique ID, used to find/remove it later
		if ( !handler.guid ) {
			handler.guid = jQuery.guid++;
		}

		// Init the element's event structure and main handler, if this is the first
		events = elemData.events;
		if ( !events ) {
			elemData.events = events = {};
		}
		eventHandle = elemData.handle;
		if ( !eventHandle ) {
			elemData.handle = eventHandle = function( e ) {
				// Discard the second event of a jQuery.event.trigger() and
				// when an event is called after a page has unloaded
				return typeof jQuery !== "undefined" && (!e || jQuery.event.triggered !== e.type) ?
					jQuery.event.dispatch.apply( eventHandle.elem, arguments ) :
					undefined;
			};
			// Add elem as a property of the handle fn to prevent a memory leak with IE non-native events
			eventHandle.elem = elem;
		}

		// Handle multiple events separated by a space
		// jQuery(...).bind("mouseover mouseout", fn);
		types = jQuery.trim( hoverHack(types) ).split( " " );
		for ( t = 0; t < types.length; t++ ) {

			tns = rtypenamespace.exec( types[t] ) || [];
			type = tns[1];
			namespaces = ( tns[2] || "" ).split( "." ).sort();

			// If event changes its type, use the special event handlers for the changed type
			special = jQuery.event.special[ type ] || {};

			// If selector defined, determine special event api type, otherwise given type
			type = ( selector ? special.delegateType : special.bindType ) || type;

			// Update special based on newly reset type
			special = jQuery.event.special[ type ] || {};

			// handleObj is passed to all event handlers
			handleObj = jQuery.extend({
				type: type,
				origType: tns[1],
				data: data,
				handler: handler,
				guid: handler.guid,
				selector: selector,
				quick: selector && quickParse( selector ),
				namespace: namespaces.join(".")
			}, handleObjIn );

			// Init the event handler queue if we're the first
			handlers = events[ type ];
			if ( !handlers ) {
				handlers = events[ type ] = [];
				handlers.delegateCount = 0;

				// Only use addEventListener/attachEvent if the special events handler returns false
				if ( !special.setup || special.setup.call( elem, data, namespaces, eventHandle ) === false ) {
					// Bind the global event handler to the element
					if ( elem.addEventListener ) {
						elem.addEventListener( type, eventHandle, false );

					} else if ( elem.attachEvent ) {
						elem.attachEvent( "on" + type, eventHandle );
					}
				}
			}

			if ( special.add ) {
				special.add.call( elem, handleObj );

				if ( !handleObj.handler.guid ) {
					handleObj.handler.guid = handler.guid;
				}
			}

			// Add to the element's handler list, delegates in front
			if ( selector ) {
				handlers.splice( handlers.delegateCount++, 0, handleObj );
			} else {
				handlers.push( handleObj );
			}

			// Keep track of which events have ever been used, for event optimization
			jQuery.event.global[ type ] = true;
		}

		// Nullify elem to prevent memory leaks in IE
		elem = null;
	},

	global: {},

	// Detach an event or set of events from an element
	remove: function( elem, types, handler, selector, mappedTypes ) {

		var elemData = jQuery.hasData( elem ) && jQuery._data( elem ),
			t, tns, type, origType, namespaces, origCount,
			j, events, special, handle, eventType, handleObj;

		if ( !elemData || !(events = elemData.events) ) {
			return;
		}

		// Once for each type.namespace in types; type may be omitted
		types = jQuery.trim( hoverHack( types || "" ) ).split(" ");
		for ( t = 0; t < types.length; t++ ) {
			tns = rtypenamespace.exec( types[t] ) || [];
			type = origType = tns[1];
			namespaces = tns[2];

			// Unbind all events (on this namespace, if provided) for the element
			if ( !type ) {
				for ( type in events ) {
					jQuery.event.remove( elem, type + types[ t ], handler, selector, true );
				}
				continue;
			}

			special = jQuery.event.special[ type ] || {};
			type = ( selector? special.delegateType : special.bindType ) || type;
			eventType = events[ type ] || [];
			origCount = eventType.length;
			namespaces = namespaces ? new RegExp("(^|\\.)" + namespaces.split(".").sort().join("\\.(?:.*\\.)?") + "(\\.|$)") : null;

			// Remove matching events
			for ( j = 0; j < eventType.length; j++ ) {
				handleObj = eventType[ j ];

				if ( ( mappedTypes || origType === handleObj.origType ) &&
					 ( !handler || handler.guid === handleObj.guid ) &&
					 ( !namespaces || namespaces.test( handleObj.namespace ) ) &&
					 ( !selector || selector === handleObj.selector || selector === "**" && handleObj.selector ) ) {
					eventType.splice( j--, 1 );

					if ( handleObj.selector ) {
						eventType.delegateCount--;
					}
					if ( special.remove ) {
						special.remove.call( elem, handleObj );
					}
				}
			}

			// Remove generic event handler if we removed something and no more handlers exist
			// (avoids potential for endless recursion during removal of special event handlers)
			if ( eventType.length === 0 && origCount !== eventType.length ) {
				if ( !special.teardown || special.teardown.call( elem, namespaces ) === false ) {
					jQuery.removeEvent( elem, type, elemData.handle );
				}

				delete events[ type ];
			}
		}

		// Remove the expando if it's no longer used
		if ( jQuery.isEmptyObject( events ) ) {
			handle = elemData.handle;
			if ( handle ) {
				handle.elem = null;
			}

			// removeData also checks for emptiness and clears the expando if empty
			// so use it instead of delete
			jQuery.removeData( elem, [ "events", "handle" ], true );
		}
	},

	// Events that are safe to short-circuit if no handlers are attached.
	// Native DOM events should not be added, they may have inline handlers.
	customEvent: {
		"getData": true,
		"setData": true,
		"changeData": true
	},

	trigger: function( event, data, elem, onlyHandlers ) {
		// Don't do events on text and comment nodes
		if ( elem && (elem.nodeType === 3 || elem.nodeType === 8) ) {
			return;
		}

		// Event object or event type
		var type = event.type || event,
			namespaces = [],
			cache, exclusive, i, cur, old, ontype, special, handle, eventPath, bubbleType;

		// focus/blur morphs to focusin/out; ensure we're not firing them right now
		if ( rfocusMorph.test( type + jQuery.event.triggered ) ) {
			return;
		}

		if ( type.indexOf( "!" ) >= 0 ) {
			// Exclusive events trigger only for the exact event (no namespaces)
			type = type.slice(0, -1);
			exclusive = true;
		}

		if ( type.indexOf( "." ) >= 0 ) {
			// Namespaced trigger; create a regexp to match event type in handle()
			namespaces = type.split(".");
			type = namespaces.shift();
			namespaces.sort();
		}

		if ( (!elem || jQuery.event.customEvent[ type ]) && !jQuery.event.global[ type ] ) {
			// No jQuery handlers for this event type, and it can't have inline handlers
			return;
		}

		// Caller can pass in an Event, Object, or just an event type string
		event = typeof event === "object" ?
			// jQuery.Event object
			event[ jQuery.expando ] ? event :
			// Object literal
			new jQuery.Event( type, event ) :
			// Just the event type (string)
			new jQuery.Event( type );

		event.type = type;
		event.isTrigger = true;
		event.exclusive = exclusive;
		event.namespace = namespaces.join( "." );
		event.namespace_re = event.namespace? new RegExp("(^|\\.)" + namespaces.join("\\.(?:.*\\.)?") + "(\\.|$)") : null;
		ontype = type.indexOf( ":" ) < 0 ? "on" + type : "";

		// Handle a global trigger
		if ( !elem ) {

			// TODO: Stop taunting the data cache; remove global events and always attach to document
			cache = jQuery.cache;
			for ( i in cache ) {
				if ( cache[ i ].events && cache[ i ].events[ type ] ) {
					jQuery.event.trigger( event, data, cache[ i ].handle.elem, true );
				}
			}
			return;
		}

		// Clean up the event in case it is being reused
		event.result = undefined;
		if ( !event.target ) {
			event.target = elem;
		}

		// Clone any incoming data and prepend the event, creating the handler arg list
		data = data != null ? jQuery.makeArray( data ) : [];
		data.unshift( event );

		// Allow special events to draw outside the lines
		special = jQuery.event.special[ type ] || {};
		if ( special.trigger && special.trigger.apply( elem, data ) === false ) {
			return;
		}

		// Determine event propagation path in advance, per W3C events spec (#9951)
		// Bubble up to document, then to window; watch for a global ownerDocument var (#9724)
		eventPath = [[ elem, special.bindType || type ]];
		if ( !onlyHandlers && !special.noBubble && !jQuery.isWindow( elem ) ) {

			bubbleType = special.delegateType || type;
			cur = rfocusMorph.test( bubbleType + type ) ? elem : elem.parentNode;
			old = null;
			for ( ; cur; cur = cur.parentNode ) {
				eventPath.push([ cur, bubbleType ]);
				old = cur;
			}

			// Only add window if we got to document (e.g., not plain obj or detached DOM)
			if ( old && old === elem.ownerDocument ) {
				eventPath.push([ old.defaultView || old.parentWindow || window, bubbleType ]);
			}
		}

		// Fire handlers on the event path
		for ( i = 0; i < eventPath.length && !event.isPropagationStopped(); i++ ) {

			cur = eventPath[i][0];
			event.type = eventPath[i][1];

			handle = ( jQuery._data( cur, "events" ) || {} )[ event.type ] && jQuery._data( cur, "handle" );
			if ( handle ) {
				handle.apply( cur, data );
			}
			// Note that this is a bare JS function and not a jQuery handler
			handle = ontype && cur[ ontype ];
			if ( handle && jQuery.acceptData( cur ) && handle.apply( cur, data ) === false ) {
				event.preventDefault();
			}
		}
		event.type = type;

		// If nobody prevented the default action, do it now
		if ( !onlyHandlers && !event.isDefaultPrevented() ) {

			if ( (!special._default || special._default.apply( elem.ownerDocument, data ) === false) &&
				!(type === "click" && jQuery.nodeName( elem, "a" )) && jQuery.acceptData( elem ) ) {

				// Call a native DOM method on the target with the same name name as the event.
				// Can't use an .isFunction() check here because IE6/7 fails that test.
				// Don't do default actions on window, that's where global variables be (#6170)
				// IE<9 dies on focus/blur to hidden element (#1486)
				if ( ontype && elem[ type ] && ((type !== "focus" && type !== "blur") || event.target.offsetWidth !== 0) && !jQuery.isWindow( elem ) ) {

					// Don't re-trigger an onFOO event when we call its FOO() method
					old = elem[ ontype ];

					if ( old ) {
						elem[ ontype ] = null;
					}

					// Prevent re-triggering of the same event, since we already bubbled it above
					jQuery.event.triggered = type;
					elem[ type ]();
					jQuery.event.triggered = undefined;

					if ( old ) {
						elem[ ontype ] = old;
					}
				}
			}
		}

		return event.result;
	},

	dispatch: function( event ) {

		// Make a writable jQuery.Event from the native event object
		event = jQuery.event.fix( event || window.event );

		var handlers = ( (jQuery._data( this, "events" ) || {} )[ event.type ] || []),
			delegateCount = handlers.delegateCount,
			args = [].slice.call( arguments, 0 ),
			run_all = !event.exclusive && !event.namespace,
			special = jQuery.event.special[ event.type ] || {},
			handlerQueue = [],
			i, j, cur, jqcur, ret, selMatch, matched, matches, handleObj, sel, related;

		// Use the fix-ed jQuery.Event rather than the (read-only) native event
		args[0] = event;
		event.delegateTarget = this;

		// Call the preDispatch hook for the mapped type, and let it bail if desired
		if ( special.preDispatch && special.preDispatch.call( this, event ) === false ) {
			return;
		}

		// Determine handlers that should run if there are delegated events
		// Avoid non-left-click bubbling in Firefox (#3861)
		if ( delegateCount && !(event.button && event.type === "click") ) {

			// Pregenerate a single jQuery object for reuse with .is()
			jqcur = jQuery(this);
			jqcur.context = this.ownerDocument || this;

			for ( cur = event.target; cur != this; cur = cur.parentNode || this ) {

				// Don't process events on disabled elements (#6911, #8165)
				if ( cur.disabled !== true ) {
					selMatch = {};
					matches = [];
					jqcur[0] = cur;
					for ( i = 0; i < delegateCount; i++ ) {
						handleObj = handlers[ i ];
						sel = handleObj.selector;

						if ( selMatch[ sel ] === undefined ) {
							selMatch[ sel ] = (
								handleObj.quick ? quickIs( cur, handleObj.quick ) : jqcur.is( sel )
							);
						}
						if ( selMatch[ sel ] ) {
							matches.push( handleObj );
						}
					}
					if ( matches.length ) {
						handlerQueue.push({ elem: cur, matches: matches });
					}
				}
			}
		}

		// Add the remaining (directly-bound) handlers
		if ( handlers.length > delegateCount ) {
			handlerQueue.push({ elem: this, matches: handlers.slice( delegateCount ) });
		}

		// Run delegates first; they may want to stop propagation beneath us
		for ( i = 0; i < handlerQueue.length && !event.isPropagationStopped(); i++ ) {
			matched = handlerQueue[ i ];
			event.currentTarget = matched.elem;

			for ( j = 0; j < matched.matches.length && !event.isImmediatePropagationStopped(); j++ ) {
				handleObj = matched.matches[ j ];

				// Triggered event must either 1) be non-exclusive and have no namespace, or
				// 2) have namespace(s) a subset or equal to those in the bound event (both can have no namespace).
				if ( run_all || (!event.namespace && !handleObj.namespace) || event.namespace_re && event.namespace_re.test( handleObj.namespace ) ) {

					event.data = handleObj.data;
					event.handleObj = handleObj;

					ret = ( (jQuery.event.special[ handleObj.origType ] || {}).handle || handleObj.handler )
							.apply( matched.elem, args );

					if ( ret !== undefined ) {
						event.result = ret;
						if ( ret === false ) {
							event.preventDefault();
							event.stopPropagation();
						}
					}
				}
			}
		}

		// Call the postDispatch hook for the mapped type
		if ( special.postDispatch ) {
			special.postDispatch.call( this, event );
		}

		return event.result;
	},

	// Includes some event props shared by KeyEvent and MouseEvent
	// *** attrChange attrName relatedNode srcElement  are not normalized, non-W3C, deprecated, will be removed in 1.8 ***
	props: "attrChange attrName relatedNode srcElement altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "),

	fixHooks: {},

	keyHooks: {
		props: "char charCode key keyCode".split(" "),
		filter: function( event, original ) {

			// Add which for key events
			if ( event.which == null ) {
				event.which = original.charCode != null ? original.charCode : original.keyCode;
			}

			return event;
		}
	},

	mouseHooks: {
		props: "button buttons clientX clientY fromElement offsetX offsetY pageX pageY screenX screenY toElement".split(" "),
		filter: function( event, original ) {
			var eventDoc, doc, body,
				button = original.button,
				fromElement = original.fromElement;

			// Calculate pageX/Y if missing and clientX/Y available
			if ( event.pageX == null && original.clientX != null ) {
				eventDoc = event.target.ownerDocument || document;
				doc = eventDoc.documentElement;
				body = eventDoc.body;

				event.pageX = original.clientX + ( doc && doc.scrollLeft || body && body.scrollLeft || 0 ) - ( doc && doc.clientLeft || body && body.clientLeft || 0 );
				event.pageY = original.clientY + ( doc && doc.scrollTop  || body && body.scrollTop  || 0 ) - ( doc && doc.clientTop  || body && body.clientTop  || 0 );
			}

			// Add relatedTarget, if necessary
			if ( !event.relatedTarget && fromElement ) {
				event.relatedTarget = fromElement === event.target ? original.toElement : fromElement;
			}

			// Add which for click: 1 === left; 2 === middle; 3 === right
			// Note: button is not normalized, so don't use it
			if ( !event.which && button !== undefined ) {
				event.which = ( button & 1 ? 1 : ( button & 2 ? 3 : ( button & 4 ? 2 : 0 ) ) );
			}

			return event;
		}
	},

	fix: function( event ) {
		if ( event[ jQuery.expando ] ) {
			return event;
		}

		// Create a writable copy of the event object and normalize some properties
		var i, prop,
			originalEvent = event,
			fixHook = jQuery.event.fixHooks[ event.type ] || {},
			copy = fixHook.props ? this.props.concat( fixHook.props ) : this.props;

		event = jQuery.Event( originalEvent );

		for ( i = copy.length; i; ) {
			prop = copy[ --i ];
			event[ prop ] = originalEvent[ prop ];
		}

		// Fix target property, if necessary (#1925, IE 6/7/8 & Safari2)
		if ( !event.target ) {
			event.target = originalEvent.srcElement || document;
		}

		// Target should not be a text node (#504, Safari)
		if ( event.target.nodeType === 3 ) {
			event.target = event.target.parentNode;
		}

		// For mouse/key events; add metaKey if it's not there (#3368, IE6/7/8)
		if ( event.metaKey === undefined ) {
			event.metaKey = event.ctrlKey;
		}

		return fixHook.filter? fixHook.filter( event, originalEvent ) : event;
	},

	special: {
		ready: {
			// Make sure the ready event is setup
			setup: jQuery.bindReady
		},

		load: {
			// Prevent triggered image.load events from bubbling to window.load
			noBubble: true
		},

		focus: {
			delegateType: "focusin"
		},
		blur: {
			delegateType: "focusout"
		},

		beforeunload: {
			setup: function( data, namespaces, eventHandle ) {
				// We only want to do this special case on windows
				if ( jQuery.isWindow( this ) ) {
					this.onbeforeunload = eventHandle;
				}
			},

			teardown: function( namespaces, eventHandle ) {
				if ( this.onbeforeunload === eventHandle ) {
					this.onbeforeunload = null;
				}
			}
		}
	},

	simulate: function( type, elem, event, bubble ) {
		// Piggyback on a donor event to simulate a different one.
		// Fake originalEvent to avoid donor's stopPropagation, but if the
		// simulated event prevents default then we do the same on the donor.
		var e = jQuery.extend(
			new jQuery.Event(),
			event,
			{ type: type,
				isSimulated: true,
				originalEvent: {}
			}
		);
		if ( bubble ) {
			jQuery.event.trigger( e, null, elem );
		} else {
			jQuery.event.dispatch.call( elem, e );
		}
		if ( e.isDefaultPrevented() ) {
			event.preventDefault();
		}
	}
};

// Some plugins are using, but it's undocumented/deprecated and will be removed.
// The 1.7 special event interface should provide all the hooks needed now.
jQuery.event.handle = jQuery.event.dispatch;

jQuery.removeEvent = document.removeEventListener ?
	function( elem, type, handle ) {
		if ( elem.removeEventListener ) {
			elem.removeEventListener( type, handle, false );
		}
	} :
	function( elem, type, handle ) {
		if ( elem.detachEvent ) {
			elem.detachEvent( "on" + type, handle );
		}
	};

jQuery.Event = function( src, props ) {
	// Allow instantiation without the 'new' keyword
	if ( !(this instanceof jQuery.Event) ) {
		return new jQuery.Event( src, props );
	}

	// Event object
	if ( src && src.type ) {
		this.originalEvent = src;
		this.type = src.type;

		// Events bubbling up the document may have been marked as prevented
		// by a handler lower down the tree; reflect the correct value.
		this.isDefaultPrevented = ( src.defaultPrevented || src.returnValue === false ||
			src.getPreventDefault && src.getPreventDefault() ) ? returnTrue : returnFalse;

	// Event type
	} else {
		this.type = src;
	}

	// Put explicitly provided properties onto the event object
	if ( props ) {
		jQuery.extend( this, props );
	}

	// Create a timestamp if incoming event doesn't have one
	this.timeStamp = src && src.timeStamp || jQuery.now();

	// Mark it as fixed
	this[ jQuery.expando ] = true;
};

function returnFalse() {
	return false;
}
function returnTrue() {
	return true;
}

// jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
// http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
jQuery.Event.prototype = {
	preventDefault: function() {
		this.isDefaultPrevented = returnTrue;

		var e = this.originalEvent;
		if ( !e ) {
			return;
		}

		// if preventDefault exists run it on the original event
		if ( e.preventDefault ) {
			e.preventDefault();

		// otherwise set the returnValue property of the original event to false (IE)
		} else {
			e.returnValue = false;
		}
	},
	stopPropagation: function() {
		this.isPropagationStopped = returnTrue;

		var e = this.originalEvent;
		if ( !e ) {
			return;
		}
		// if stopPropagation exists run it on the original event
		if ( e.stopPropagation ) {
			e.stopPropagation();
		}
		// otherwise set the cancelBubble property of the original event to true (IE)
		e.cancelBubble = true;
	},
	stopImmediatePropagation: function() {
		this.isImmediatePropagationStopped = returnTrue;
		this.stopPropagation();
	},
	isDefaultPrevented: returnFalse,
	isPropagationStopped: returnFalse,
	isImmediatePropagationStopped: returnFalse
};

// Create mouseenter/leave events using mouseover/out and event-time checks
jQuery.each({
	mouseenter: "mouseover",
	mouseleave: "mouseout"
}, function( orig, fix ) {
	jQuery.event.special[ orig ] = {
		delegateType: fix,
		bindType: fix,

		handle: function( event ) {
			var target = this,
				related = event.relatedTarget,
				handleObj = event.handleObj,
				selector = handleObj.selector,
				ret;

			// For mousenter/leave call the handler if related is outside the target.
			// NB: No relatedTarget if the mouse left/entered the browser window
			if ( !related || (related !== target && !jQuery.contains( target, related )) ) {
				event.type = handleObj.origType;
				ret = handleObj.handler.apply( this, arguments );
				event.type = fix;
			}
			return ret;
		}
	};
});

// IE submit delegation
if ( !jQuery.support.submitBubbles ) {

	jQuery.event.special.submit = {
		setup: function() {
			// Only need this for delegated form submit events
			if ( jQuery.nodeName( this, "form" ) ) {
				return false;
			}

			// Lazy-add a submit handler when a descendant form may potentially be submitted
			jQuery.event.add( this, "click._submit keypress._submit", function( e ) {
				// Node name check avoids a VML-related crash in IE (#9807)
				var elem = e.target,
					form = jQuery.nodeName( elem, "input" ) || jQuery.nodeName( elem, "button" ) ? elem.form : undefined;
				if ( form && !form._submit_attached ) {
					jQuery.event.add( form, "submit._submit", function( event ) {
						event._submit_bubble = true;
					});
					form._submit_attached = true;
				}
			});
			// return undefined since we don't need an event listener
		},
		
		postDispatch: function( event ) {
			// If form was submitted by the user, bubble the event up the tree
			if ( event._submit_bubble ) {
				delete event._submit_bubble;
				if ( this.parentNode && !event.isTrigger ) {
					jQuery.event.simulate( "submit", this.parentNode, event, true );
				}
			}
		},

		teardown: function() {
			// Only need this for delegated form submit events
			if ( jQuery.nodeName( this, "form" ) ) {
				return false;
			}

			// Remove delegated handlers; cleanData eventually reaps submit handlers attached above
			jQuery.event.remove( this, "._submit" );
		}
	};
}

// IE change delegation and checkbox/radio fix
if ( !jQuery.support.changeBubbles ) {

	jQuery.event.special.change = {

		setup: function() {

			if ( rformElems.test( this.nodeName ) ) {
				// IE doesn't fire change on a check/radio until blur; trigger it on click
				// after a propertychange. Eat the blur-change in special.change.handle.
				// This still fires onchange a second time for check/radio after blur.
				if ( this.type === "checkbox" || this.type === "radio" ) {
					jQuery.event.add( this, "propertychange._change", function( event ) {
						if ( event.originalEvent.propertyName === "checked" ) {
							this._just_changed = true;
						}
					});
					jQuery.event.add( this, "click._change", function( event ) {
						if ( this._just_changed && !event.isTrigger ) {
							this._just_changed = false;
							jQuery.event.simulate( "change", this, event, true );
						}
					});
				}
				return false;
			}
			// Delegated event; lazy-add a change handler on descendant inputs
			jQuery.event.add( this, "beforeactivate._change", function( e ) {
				var elem = e.target;

				if ( rformElems.test( elem.nodeName ) && !elem._change_attached ) {
					jQuery.event.add( elem, "change._change", function( event ) {
						if ( this.parentNode && !event.isSimulated && !event.isTrigger ) {
							jQuery.event.simulate( "change", this.parentNode, event, true );
						}
					});
					elem._change_attached = true;
				}
			});
		},

		handle: function( event ) {
			var elem = event.target;

			// Swallow native change events from checkbox/radio, we already triggered them above
			if ( this !== elem || event.isSimulated || event.isTrigger || (elem.type !== "radio" && elem.type !== "checkbox") ) {
				return event.handleObj.handler.apply( this, arguments );
			}
		},

		teardown: function() {
			jQuery.event.remove( this, "._change" );

			return rformElems.test( this.nodeName );
		}
	};
}

// Create "bubbling" focus and blur events
if ( !jQuery.support.focusinBubbles ) {
	jQuery.each({ focus: "focusin", blur: "focusout" }, function( orig, fix ) {

		// Attach a single capturing handler while someone wants focusin/focusout
		var attaches = 0,
			handler = function( event ) {
				jQuery.event.simulate( fix, event.target, jQuery.event.fix( event ), true );
			};

		jQuery.event.special[ fix ] = {
			setup: function() {
				if ( attaches++ === 0 ) {
					document.addEventListener( orig, handler, true );
				}
			},
			teardown: function() {
				if ( --attaches === 0 ) {
					document.removeEventListener( orig, handler, true );
				}
			}
		};
	});
}

jQuery.fn.extend({

	on: function( types, selector, data, fn, /*INTERNAL*/ one ) {
		var origFn, type;

		// Types can be a map of types/handlers
		if ( typeof types === "object" ) {
			// ( types-Object, selector, data )
			if ( typeof selector !== "string" ) { // && selector != null
				// ( types-Object, data )
				data = data || selector;
				selector = undefined;
			}
			for ( type in types ) {
				this.on( type, selector, data, types[ type ], one );
			}
			return this;
		}

		if ( data == null && fn == null ) {
			// ( types, fn )
			fn = selector;
			data = selector = undefined;
		} else if ( fn == null ) {
			if ( typeof selector === "string" ) {
				// ( types, selector, fn )
				fn = data;
				data = undefined;
			} else {
				// ( types, data, fn )
				fn = data;
				data = selector;
				selector = undefined;
			}
		}
		if ( fn === false ) {
			fn = returnFalse;
		} else if ( !fn ) {
			return this;
		}

		if ( one === 1 ) {
			origFn = fn;
			fn = function( event ) {
				// Can use an empty set, since event contains the info
				jQuery().off( event );
				return origFn.apply( this, arguments );
			};
			// Use same guid so caller can remove using origFn
			fn.guid = origFn.guid || ( origFn.guid = jQuery.guid++ );
		}
		return this.each( function() {
			jQuery.event.add( this, types, fn, data, selector );
		});
	},
	one: function( types, selector, data, fn ) {
		return this.on( types, selector, data, fn, 1 );
	},
	off: function( types, selector, fn ) {
		if ( types && types.preventDefault && types.handleObj ) {
			// ( event )  dispatched jQuery.Event
			var handleObj = types.handleObj;
			jQuery( types.delegateTarget ).off(
				handleObj.namespace ? handleObj.origType + "." + handleObj.namespace : handleObj.origType,
				handleObj.selector,
				handleObj.handler
			);
			return this;
		}
		if ( typeof types === "object" ) {
			// ( types-object [, selector] )
			for ( var type in types ) {
				this.off( type, selector, types[ type ] );
			}
			return this;
		}
		if ( selector === false || typeof selector === "function" ) {
			// ( types [, fn] )
			fn = selector;
			selector = undefined;
		}
		if ( fn === false ) {
			fn = returnFalse;
		}
		return this.each(function() {
			jQuery.event.remove( this, types, fn, selector );
		});
	},

	bind: function( types, data, fn ) {
		return this.on( types, null, data, fn );
	},
	unbind: function( types, fn ) {
		return this.off( types, null, fn );
	},

	live: function( types, data, fn ) {
		jQuery( this.context ).on( types, this.selector, data, fn );
		return this;
	},
	die: function( types, fn ) {
		jQuery( this.context ).off( types, this.selector || "**", fn );
		return this;
	},

	delegate: function( selector, types, data, fn ) {
		return this.on( types, selector, data, fn );
	},
	undelegate: function( selector, types, fn ) {
		// ( namespace ) or ( selector, types [, fn] )
		return arguments.length == 1? this.off( selector, "**" ) : this.off( types, selector, fn );
	},

	trigger: function( type, data ) {
		return this.each(function() {
			jQuery.event.trigger( type, data, this );
		});
	},
	triggerHandler: function( type, data ) {
		if ( this[0] ) {
			return jQuery.event.trigger( type, data, this[0], true );
		}
	},

	toggle: function( fn ) {
		// Save reference to arguments for access in closure
		var args = arguments,
			guid = fn.guid || jQuery.guid++,
			i = 0,
			toggler = function( event ) {
				// Figure out which function to execute
				var lastToggle = ( jQuery._data( this, "lastToggle" + fn.guid ) || 0 ) % i;
				jQuery._data( this, "lastToggle" + fn.guid, lastToggle + 1 );

				// Make sure that clicks stop
				event.preventDefault();

				// and execute the function
				return args[ lastToggle ].apply( this, arguments ) || false;
			};

		// link all the functions, so any of them can unbind this click handler
		toggler.guid = guid;
		while ( i < args.length ) {
			args[ i++ ].guid = guid;
		}

		return this.click( toggler );
	},

	hover: function( fnOver, fnOut ) {
		return this.mouseenter( fnOver ).mouseleave( fnOut || fnOver );
	}
});

jQuery.each( ("blur focus focusin focusout load resize scroll unload click dblclick " +
	"mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " +
	"change select submit keydown keypress keyup error contextmenu").split(" "), function( i, name ) {

	// Handle event binding
	jQuery.fn[ name ] = function( data, fn ) {
		if ( fn == null ) {
			fn = data;
			data = null;
		}

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};

	if ( jQuery.attrFn ) {
		jQuery.attrFn[ name ] = true;
	}

	if ( rkeyEvent.test( name ) ) {
		jQuery.event.fixHooks[ name ] = jQuery.event.keyHooks;
	}

	if ( rmouseEvent.test( name ) ) {
		jQuery.event.fixHooks[ name ] = jQuery.event.mouseHooks;
	}
});



/*!
 * Sizzle CSS Selector Engine
 *  Copyright 2011, The Dojo Foundation
 *  Released under the MIT, BSD, and GPL Licenses.
 *  More information: http://sizzlejs.com/
 */
(function(){

var chunker = /((?:\((?:\([^()]+\)|[^()]+)+\)|\[(?:\[[^\[\]]*\]|['"][^'"]*['"]|[^\[\]'"]+)+\]|\\.|[^ >+~,(\[\\]+)+|[>+~])(\s*,\s*)?((?:.|\r|\n)*)/g,
	expando = "sizcache" + (Math.random() + '').replace('.', ''),
	done = 0,
	toString = Object.prototype.toString,
	hasDuplicate = false,
	baseHasDuplicate = true,
	rBackslash = /\\/g,
	rReturn = /\r\n/g,
	rNonWord = /\W/;

// Here we check if the JavaScript engine is using some sort of
// optimization where it does not always call our comparision
// function. If that is the case, discard the hasDuplicate value.
//   Thus far that includes Google Chrome.
[0, 0].sort(function() {
	baseHasDuplicate = false;
	return 0;
});

var Sizzle = function( selector, context, results, seed ) {
	results = results || [];
	context = context || document;

	var origContext = context;

	if ( context.nodeType !== 1 && context.nodeType !== 9 ) {
		return [];
	}

	if ( !selector || typeof selector !== "string" ) {
		return results;
	}

	var m, set, checkSet, extra, ret, cur, pop, i,
		prune = true,
		contextXML = Sizzle.isXML( context ),
		parts = [],
		soFar = selector;

	// Reset the position of the chunker regexp (start from head)
	do {
		chunker.exec( "" );
		m = chunker.exec( soFar );

		if ( m ) {
			soFar = m[3];

			parts.push( m[1] );

			if ( m[2] ) {
				extra = m[3];
				break;
			}
		}
	} while ( m );

	if ( parts.length > 1 && origPOS.exec( selector ) ) {

		if ( parts.length === 2 && Expr.relative[ parts[0] ] ) {
			set = posProcess( parts[0] + parts[1], context, seed );

		} else {
			set = Expr.relative[ parts[0] ] ?
				[ context ] :
				Sizzle( parts.shift(), context );

			while ( parts.length ) {
				selector = parts.shift();

				if ( Expr.relative[ selector ] ) {
					selector += parts.shift();
				}

				set = posProcess( selector, set, seed );
			}
		}

	} else {
		// Take a shortcut and set the context if the root selector is an ID
		// (but not if it'll be faster if the inner selector is an ID)
		if ( !seed && parts.length > 1 && context.nodeType === 9 && !contextXML &&
				Expr.match.ID.test(parts[0]) && !Expr.match.ID.test(parts[parts.length - 1]) ) {

			ret = Sizzle.find( parts.shift(), context, contextXML );
			context = ret.expr ?
				Sizzle.filter( ret.expr, ret.set )[0] :
				ret.set[0];
		}

		if ( context ) {
			ret = seed ?
				{ expr: parts.pop(), set: makeArray(seed) } :
				Sizzle.find( parts.pop(), parts.length === 1 && (parts[0] === "~" || parts[0] === "+") && context.parentNode ? context.parentNode : context, contextXML );

			set = ret.expr ?
				Sizzle.filter( ret.expr, ret.set ) :
				ret.set;

			if ( parts.length > 0 ) {
				checkSet = makeArray( set );

			} else {
				prune = false;
			}

			while ( parts.length ) {
				cur = parts.pop();
				pop = cur;

				if ( !Expr.relative[ cur ] ) {
					cur = "";
				} else {
					pop = parts.pop();
				}

				if ( pop == null ) {
					pop = context;
				}

				Expr.relative[ cur ]( checkSet, pop, contextXML );
			}

		} else {
			checkSet = parts = [];
		}
	}

	if ( !checkSet ) {
		checkSet = set;
	}

	if ( !checkSet ) {
		Sizzle.error( cur || selector );
	}

	if ( toString.call(checkSet) === "[object Array]" ) {
		if ( !prune ) {
			results.push.apply( results, checkSet );

		} else if ( context && context.nodeType === 1 ) {
			for ( i = 0; checkSet[i] != null; i++ ) {
				if ( checkSet[i] && (checkSet[i] === true || checkSet[i].nodeType === 1 && Sizzle.contains(context, checkSet[i])) ) {
					results.push( set[i] );
				}
			}

		} else {
			for ( i = 0; checkSet[i] != null; i++ ) {
				if ( checkSet[i] && checkSet[i].nodeType === 1 ) {
					results.push( set[i] );
				}
			}
		}

	} else {
		makeArray( checkSet, results );
	}

	if ( extra ) {
		Sizzle( extra, origContext, results, seed );
		Sizzle.uniqueSort( results );
	}

	return results;
};

Sizzle.uniqueSort = function( results ) {
	if ( sortOrder ) {
		hasDuplicate = baseHasDuplicate;
		results.sort( sortOrder );

		if ( hasDuplicate ) {
			for ( var i = 1; i < results.length; i++ ) {
				if ( results[i] === results[ i - 1 ] ) {
					results.splice( i--, 1 );
				}
			}
		}
	}

	return results;
};

Sizzle.matches = function( expr, set ) {
	return Sizzle( expr, null, null, set );
};

Sizzle.matchesSelector = function( node, expr ) {
	return Sizzle( expr, null, null, [node] ).length > 0;
};

Sizzle.find = function( expr, context, isXML ) {
	var set, i, len, match, type, left;

	if ( !expr ) {
		return [];
	}

	for ( i = 0, len = Expr.order.length; i < len; i++ ) {
		type = Expr.order[i];

		if ( (match = Expr.leftMatch[ type ].exec( expr )) ) {
			left = match[1];
			match.splice( 1, 1 );

			if ( left.substr( left.length - 1 ) !== "\\" ) {
				match[1] = (match[1] || "").replace( rBackslash, "" );
				set = Expr.find[ type ]( match, context, isXML );

				if ( set != null ) {
					expr = expr.replace( Expr.match[ type ], "" );
					break;
				}
			}
		}
	}

	if ( !set ) {
		set = typeof context.getElementsByTagName !== "undefined" ?
			context.getElementsByTagName( "*" ) :
			[];
	}

	return { set: set, expr: expr };
};

Sizzle.filter = function( expr, set, inplace, not ) {
	var match, anyFound,
		type, found, item, filter, left,
		i, pass,
		old = expr,
		result = [],
		curLoop = set,
		isXMLFilter = set && set[0] && Sizzle.isXML( set[0] );

	while ( expr && set.length ) {
		for ( type in Expr.filter ) {
			if ( (match = Expr.leftMatch[ type ].exec( expr )) != null && match[2] ) {
				filter = Expr.filter[ type ];
				left = match[1];

				anyFound = false;

				match.splice(1,1);

				if ( left.substr( left.length - 1 ) === "\\" ) {
					continue;
				}

				if ( curLoop === result ) {
					result = [];
				}

				if ( Expr.preFilter[ type ] ) {
					match = Expr.preFilter[ type ]( match, curLoop, inplace, result, not, isXMLFilter );

					if ( !match ) {
						anyFound = found = true;

					} else if ( match === true ) {
						continue;
					}
				}

				if ( match ) {
					for ( i = 0; (item = curLoop[i]) != null; i++ ) {
						if ( item ) {
							found = filter( item, match, i, curLoop );
							pass = not ^ found;

							if ( inplace && found != null ) {
								if ( pass ) {
									anyFound = true;

								} else {
									curLoop[i] = false;
								}

							} else if ( pass ) {
								result.push( item );
								anyFound = true;
							}
						}
					}
				}

				if ( found !== undefined ) {
					if ( !inplace ) {
						curLoop = result;
					}

					expr = expr.replace( Expr.match[ type ], "" );

					if ( !anyFound ) {
						return [];
					}

					break;
				}
			}
		}

		// Improper expression
		if ( expr === old ) {
			if ( anyFound == null ) {
				Sizzle.error( expr );

			} else {
				break;
			}
		}

		old = expr;
	}

	return curLoop;
};

Sizzle.error = function( msg ) {
	throw new Error( "Syntax error, unrecognized expression: " + msg );
};

/**
 * Utility function for retreiving the text value of an array of DOM nodes
 * @param {Array|Element} elem
 */
var getText = Sizzle.getText = function( elem ) {
    var i, node,
		nodeType = elem.nodeType,
		ret = "";

	if ( nodeType ) {
		if ( nodeType === 1 || nodeType === 9 || nodeType === 11 ) {
			// Use textContent || innerText for elements
			if ( typeof elem.textContent === 'string' ) {
				return elem.textContent;
			} else if ( typeof elem.innerText === 'string' ) {
				// Replace IE's carriage returns
				return elem.innerText.replace( rReturn, '' );
			} else {
				// Traverse it's children
				for ( elem = elem.firstChild; elem; elem = elem.nextSibling) {
					ret += getText( elem );
				}
			}
		} else if ( nodeType === 3 || nodeType === 4 ) {
			return elem.nodeValue;
		}
	} else {

		// If no nodeType, this is expected to be an array
		for ( i = 0; (node = elem[i]); i++ ) {
			// Do not traverse comment nodes
			if ( node.nodeType !== 8 ) {
				ret += getText( node );
			}
		}
	}
	return ret;
};

var Expr = Sizzle.selectors = {
	order: [ "ID", "NAME", "TAG" ],

	match: {
		ID: /#((?:[\w\u00c0-\uFFFF\-]|\\.)+)/,
		CLASS: /\.((?:[\w\u00c0-\uFFFF\-]|\\.)+)/,
		NAME: /\[name=['"]*((?:[\w\u00c0-\uFFFF\-]|\\.)+)['"]*\]/,
		ATTR: /\[\s*((?:[\w\u00c0-\uFFFF\-]|\\.)+)\s*(?:(\S?=)\s*(?:(['"])(.*?)\3|(#?(?:[\w\u00c0-\uFFFF\-]|\\.)*)|)|)\s*\]/,
		TAG: /^((?:[\w\u00c0-\uFFFF\*\-]|\\.)+)/,
		CHILD: /:(only|nth|last|first)-child(?:\(\s*(even|odd|(?:[+\-]?\d+|(?:[+\-]?\d*)?n\s*(?:[+\-]\s*\d+)?))\s*\))?/,
		POS: /:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^\-]|$)/,
		PSEUDO: /:((?:[\w\u00c0-\uFFFF\-]|\\.)+)(?:\((['"]?)((?:\([^\)]+\)|[^\(\)]*)+)\2\))?/
	},

	leftMatch: {},

	attrMap: {
		"class": "className",
		"for": "htmlFor"
	},

	attrHandle: {
		href: function( elem ) {
			return elem.getAttribute( "href" );
		},
		type: function( elem ) {
			return elem.getAttribute( "type" );
		}
	},

	relative: {
		"+": function(checkSet, part){
			var isPartStr = typeof part === "string",
				isTag = isPartStr && !rNonWord.test( part ),
				isPartStrNotTag = isPartStr && !isTag;

			if ( isTag ) {
				part = part.toLowerCase();
			}

			for ( var i = 0, l = checkSet.length, elem; i < l; i++ ) {
				if ( (elem = checkSet[i]) ) {
					while ( (elem = elem.previousSibling) && elem.nodeType !== 1 ) {}

					checkSet[i] = isPartStrNotTag || elem && elem.nodeName.toLowerCase() === part ?
						elem || false :
						elem === part;
				}
			}

			if ( isPartStrNotTag ) {
				Sizzle.filter( part, checkSet, true );
			}
		},

		">": function( checkSet, part ) {
			var elem,
				isPartStr = typeof part === "string",
				i = 0,
				l = checkSet.length;

			if ( isPartStr && !rNonWord.test( part ) ) {
				part = part.toLowerCase();

				for ( ; i < l; i++ ) {
					elem = checkSet[i];

					if ( elem ) {
						var parent = elem.parentNode;
						checkSet[i] = parent.nodeName.toLowerCase() === part ? parent : false;
					}
				}

			} else {
				for ( ; i < l; i++ ) {
					elem = checkSet[i];

					if ( elem ) {
						checkSet[i] = isPartStr ?
							elem.parentNode :
							elem.parentNode === part;
					}
				}

				if ( isPartStr ) {
					Sizzle.filter( part, checkSet, true );
				}
			}
		},

		"": function(checkSet, part, isXML){
			var nodeCheck,
				doneName = done++,
				checkFn = dirCheck;

			if ( typeof part === "string" && !rNonWord.test( part ) ) {
				part = part.toLowerCase();
				nodeCheck = part;
				checkFn = dirNodeCheck;
			}

			checkFn( "parentNode", part, doneName, checkSet, nodeCheck, isXML );
		},

		"~": function( checkSet, part, isXML ) {
			var nodeCheck,
				doneName = done++,
				checkFn = dirCheck;

			if ( typeof part === "string" && !rNonWord.test( part ) ) {
				part = part.toLowerCase();
				nodeCheck = part;
				checkFn = dirNodeCheck;
			}

			checkFn( "previousSibling", part, doneName, checkSet, nodeCheck, isXML );
		}
	},

	find: {
		ID: function( match, context, isXML ) {
			if ( typeof context.getElementById !== "undefined" && !isXML ) {
				var m = context.getElementById(match[1]);
				// Check parentNode to catch when Blackberry 4.6 returns
				// nodes that are no longer in the document #6963
				return m && m.parentNode ? [m] : [];
			}
		},

		NAME: function( match, context ) {
			if ( typeof context.getElementsByName !== "undefined" ) {
				var ret = [],
					results = context.getElementsByName( match[1] );

				for ( var i = 0, l = results.length; i < l; i++ ) {
					if ( results[i].getAttribute("name") === match[1] ) {
						ret.push( results[i] );
					}
				}

				return ret.length === 0 ? null : ret;
			}
		},

		TAG: function( match, context ) {
			if ( typeof context.getElementsByTagName !== "undefined" ) {
				return context.getElementsByTagName( match[1] );
			}
		}
	},
	preFilter: {
		CLASS: function( match, curLoop, inplace, result, not, isXML ) {
			match = " " + match[1].replace( rBackslash, "" ) + " ";

			if ( isXML ) {
				return match;
			}

			for ( var i = 0, elem; (elem = curLoop[i]) != null; i++ ) {
				if ( elem ) {
					if ( not ^ (elem.className && (" " + elem.className + " ").replace(/[\t\n\r]/g, " ").indexOf(match) >= 0) ) {
						if ( !inplace ) {
							result.push( elem );
						}

					} else if ( inplace ) {
						curLoop[i] = false;
					}
				}
			}

			return false;
		},

		ID: function( match ) {
			return match[1].replace( rBackslash, "" );
		},

		TAG: function( match, curLoop ) {
			return match[1].replace( rBackslash, "" ).toLowerCase();
		},

		CHILD: function( match ) {
			if ( match[1] === "nth" ) {
				if ( !match[2] ) {
					Sizzle.error( match[0] );
				}

				match[2] = match[2].replace(/^\+|\s*/g, '');

				// parse equations like 'even', 'odd', '5', '2n', '3n+2', '4n-1', '-n+6'
				var test = /(-?)(\d*)(?:n([+\-]?\d*))?/.exec(
					match[2] === "even" && "2n" || match[2] === "odd" && "2n+1" ||
					!/\D/.test( match[2] ) && "0n+" + match[2] || match[2]);

				// calculate the numbers (first)n+(last) including if they are negative
				match[2] = (test[1] + (test[2] || 1)) - 0;
				match[3] = test[3] - 0;
			}
			else if ( match[2] ) {
				Sizzle.error( match[0] );
			}

			// TODO: Move to normal caching system
			match[0] = done++;

			return match;
		},

		ATTR: function( match, curLoop, inplace, result, not, isXML ) {
			var name = match[1] = match[1].replace( rBackslash, "" );

			if ( !isXML && Expr.attrMap[name] ) {
				match[1] = Expr.attrMap[name];
			}

			// Handle if an un-quoted value was used
			match[4] = ( match[4] || match[5] || "" ).replace( rBackslash, "" );

			if ( match[2] === "~=" ) {
				match[4] = " " + match[4] + " ";
			}

			return match;
		},

		PSEUDO: function( match, curLoop, inplace, result, not ) {
			if ( match[1] === "not" ) {
				// If we're dealing with a complex expression, or a simple one
				if ( ( chunker.exec(match[3]) || "" ).length > 1 || /^\w/.test(match[3]) ) {
					match[3] = Sizzle(match[3], null, null, curLoop);

				} else {
					var ret = Sizzle.filter(match[3], curLoop, inplace, true ^ not);

					if ( !inplace ) {
						result.push.apply( result, ret );
					}

					return false;
				}

			} else if ( Expr.match.POS.test( match[0] ) || Expr.match.CHILD.test( match[0] ) ) {
				return true;
			}

			return match;
		},

		POS: function( match ) {
			match.unshift( true );

			return match;
		}
	},

	filters: {
		enabled: function( elem ) {
			return elem.disabled === false && elem.type !== "hidden";
		},

		disabled: function( elem ) {
			return elem.disabled === true;
		},

		checked: function( elem ) {
			return elem.checked === true;
		},

		selected: function( elem ) {
			// Accessing this property makes selected-by-default
			// options in Safari work properly
			if ( elem.parentNode ) {
				elem.parentNode.selectedIndex;
			}

			return elem.selected === true;
		},

		parent: function( elem ) {
			return !!elem.firstChild;
		},

		empty: function( elem ) {
			return !elem.firstChild;
		},

		has: function( elem, i, match ) {
			return !!Sizzle( match[3], elem ).length;
		},

		header: function( elem ) {
			return (/h\d/i).test( elem.nodeName );
		},

		text: function( elem ) {
			var attr = elem.getAttribute( "type" ), type = elem.type;
			// IE6 and 7 will map elem.type to 'text' for new HTML5 types (search, etc)
			// use getAttribute instead to test this case
			return elem.nodeName.toLowerCase() === "input" && "text" === type && ( attr === type || attr === null );
		},

		radio: function( elem ) {
			return elem.nodeName.toLowerCase() === "input" && "radio" === elem.type;
		},

		checkbox: function( elem ) {
			return elem.nodeName.toLowerCase() === "input" && "checkbox" === elem.type;
		},

		file: function( elem ) {
			return elem.nodeName.toLowerCase() === "input" && "file" === elem.type;
		},

		password: function( elem ) {
			return elem.nodeName.toLowerCase() === "input" && "password" === elem.type;
		},

		submit: function( elem ) {
			var name = elem.nodeName.toLowerCase();
			return (name === "input" || name === "button") && "submit" === elem.type;
		},

		image: function( elem ) {
			return elem.nodeName.toLowerCase() === "input" && "image" === elem.type;
		},

		reset: function( elem ) {
			var name = elem.nodeName.toLowerCase();
			return (name === "input" || name === "button") && "reset" === elem.type;
		},

		button: function( elem ) {
			var name = elem.nodeName.toLowerCase();
			return name === "input" && "button" === elem.type || name === "button";
		},

		input: function( elem ) {
			return (/input|select|textarea|button/i).test( elem.nodeName );
		},

		focus: function( elem ) {
			return elem === elem.ownerDocument.activeElement;
		}
	},
	setFilters: {
		first: function( elem, i ) {
			return i === 0;
		},

		last: function( elem, i, match, array ) {
			return i === array.length - 1;
		},

		even: function( elem, i ) {
			return i % 2 === 0;
		},

		odd: function( elem, i ) {
			return i % 2 === 1;
		},

		lt: function( elem, i, match ) {
			return i < match[3] - 0;
		},

		gt: function( elem, i, match ) {
			return i > match[3] - 0;
		},

		nth: function( elem, i, match ) {
			return match[3] - 0 === i;
		},

		eq: function( elem, i, match ) {
			return match[3] - 0 === i;
		}
	},
	filter: {
		PSEUDO: function( elem, match, i, array ) {
			var name = match[1],
				filter = Expr.filters[ name ];

			if ( filter ) {
				return filter( elem, i, match, array );

			} else if ( name === "contains" ) {
				return (elem.textContent || elem.innerText || getText([ elem ]) || "").indexOf(match[3]) >= 0;

			} else if ( name === "not" ) {
				var not = match[3];

				for ( var j = 0, l = not.length; j < l; j++ ) {
					if ( not[j] === elem ) {
						return false;
					}
				}

				return true;

			} else {
				Sizzle.error( name );
			}
		},

		CHILD: function( elem, match ) {
			var first, last,
				doneName, parent, cache,
				count, diff,
				type = match[1],
				node = elem;

			switch ( type ) {
				case "only":
				case "first":
					while ( (node = node.previousSibling) ) {
						if ( node.nodeType === 1 ) {
							return false;
						}
					}

					if ( type === "first" ) {
						return true;
					}

					node = elem;

					/* falls through */
				case "last":
					while ( (node = node.nextSibling) ) {
						if ( node.nodeType === 1 ) {
							return false;
						}
					}

					return true;

				case "nth":
					first = match[2];
					last = match[3];

					if ( first === 1 && last === 0 ) {
						return true;
					}

					doneName = match[0];
					parent = elem.parentNode;

					if ( parent && (parent[ expando ] !== doneName || !elem.nodeIndex) ) {
						count = 0;

						for ( node = parent.firstChild; node; node = node.nextSibling ) {
							if ( node.nodeType === 1 ) {
								node.nodeIndex = ++count;
							}
						}

						parent[ expando ] = doneName;
					}

					diff = elem.nodeIndex - last;

					if ( first === 0 ) {
						return diff === 0;

					} else {
						return ( diff % first === 0 && diff / first >= 0 );
					}
			}
		},

		ID: function( elem, match ) {
			return elem.nodeType === 1 && elem.getAttribute("id") === match;
		},

		TAG: function( elem, match ) {
			return (match === "*" && elem.nodeType === 1) || !!elem.nodeName && elem.nodeName.toLowerCase() === match;
		},

		CLASS: function( elem, match ) {
			return (" " + (elem.className || elem.getAttribute("class")) + " ")
				.indexOf( match ) > -1;
		},

		ATTR: function( elem, match ) {
			var name = match[1],
				result = Sizzle.attr ?
					Sizzle.attr( elem, name ) :
					Expr.attrHandle[ name ] ?
					Expr.attrHandle[ name ]( elem ) :
					elem[ name ] != null ?
						elem[ name ] :
						elem.getAttribute( name ),
				value = result + "",
				type = match[2],
				check = match[4];

			return result == null ?
				type === "!=" :
				!type && Sizzle.attr ?
				result != null :
				type === "=" ?
				value === check :
				type === "*=" ?
				value.indexOf(check) >= 0 :
				type === "~=" ?
				(" " + value + " ").indexOf(check) >= 0 :
				!check ?
				value && result !== false :
				type === "!=" ?
				value !== check :
				type === "^=" ?
				value.indexOf(check) === 0 :
				type === "$=" ?
				value.substr(value.length - check.length) === check :
				type === "|=" ?
				value === check || value.substr(0, check.length + 1) === check + "-" :
				false;
		},

		POS: function( elem, match, i, array ) {
			var name = match[2],
				filter = Expr.setFilters[ name ];

			if ( filter ) {
				return filter( elem, i, match, array );
			}
		}
	}
};

var origPOS = Expr.match.POS,
	fescape = function(all, num){
		return "\\" + (num - 0 + 1);
	};

for ( var type in Expr.match ) {
	Expr.match[ type ] = new RegExp( Expr.match[ type ].source + (/(?![^\[]*\])(?![^\(]*\))/.source) );
	Expr.leftMatch[ type ] = new RegExp( /(^(?:.|\r|\n)*?)/.source + Expr.match[ type ].source.replace(/\\(\d+)/g, fescape) );
}
// Expose origPOS
// "global" as in regardless of relation to brackets/parens
Expr.match.globalPOS = origPOS;

var makeArray = function( array, results ) {
	array = Array.prototype.slice.call( array, 0 );

	if ( results ) {
		results.push.apply( results, array );
		return results;
	}

	return array;
};

// Perform a simple check to determine if the browser is capable of
// converting a NodeList to an array using builtin methods.
// Also verifies that the returned array holds DOM nodes
// (which is not the case in the Blackberry browser)
try {
	Array.prototype.slice.call( document.documentElement.childNodes, 0 )[0].nodeType;

// Provide a fallback method if it does not work
} catch( e ) {
	makeArray = function( array, results ) {
		var i = 0,
			ret = results || [];

		if ( toString.call(array) === "[object Array]" ) {
			Array.prototype.push.apply( ret, array );

		} else {
			if ( typeof array.length === "number" ) {
				for ( var l = array.length; i < l; i++ ) {
					ret.push( array[i] );
				}

			} else {
				for ( ; array[i]; i++ ) {
					ret.push( array[i] );
				}
			}
		}

		return ret;
	};
}

var sortOrder, siblingCheck;

if ( document.documentElement.compareDocumentPosition ) {
	sortOrder = function( a, b ) {
		if ( a === b ) {
			hasDuplicate = true;
			return 0;
		}

		if ( !a.compareDocumentPosition || !b.compareDocumentPosition ) {
			return a.compareDocumentPosition ? -1 : 1;
		}

		return a.compareDocumentPosition(b) & 4 ? -1 : 1;
	};

} else {
	sortOrder = function( a, b ) {
		// The nodes are identical, we can exit early
		if ( a === b ) {
			hasDuplicate = true;
			return 0;

		// Fallback to using sourceIndex (in IE) if it's available on both nodes
		} else if ( a.sourceIndex && b.sourceIndex ) {
			return a.sourceIndex - b.sourceIndex;
		}

		var al, bl,
			ap = [],
			bp = [],
			aup = a.parentNode,
			bup = b.parentNode,
			cur = aup;

		// If the nodes are siblings (or identical) we can do a quick check
		if ( aup === bup ) {
			return siblingCheck( a, b );

		// If no parents were found then the nodes are disconnected
		} else if ( !aup ) {
			return -1;

		} else if ( !bup ) {
			return 1;
		}

		// Otherwise they're somewhere else in the tree so we need
		// to build up a full list of the parentNodes for comparison
		while ( cur ) {
			ap.unshift( cur );
			cur = cur.parentNode;
		}

		cur = bup;

		while ( cur ) {
			bp.unshift( cur );
			cur = cur.parentNode;
		}

		al = ap.length;
		bl = bp.length;

		// Start walking down the tree looking for a discrepancy
		for ( var i = 0; i < al && i < bl; i++ ) {
			if ( ap[i] !== bp[i] ) {
				return siblingCheck( ap[i], bp[i] );
			}
		}

		// We ended someplace up the tree so do a sibling check
		return i === al ?
			siblingCheck( a, bp[i], -1 ) :
			siblingCheck( ap[i], b, 1 );
	};

	siblingCheck = function( a, b, ret ) {
		if ( a === b ) {
			return ret;
		}

		var cur = a.nextSibling;

		while ( cur ) {
			if ( cur === b ) {
				return -1;
			}

			cur = cur.nextSibling;
		}

		return 1;
	};
}

// Check to see if the browser returns elements by name when
// querying by getElementById (and provide a workaround)
(function(){
	// We're going to inject a fake input element with a specified name
	var form = document.createElement("div"),
		id = "script" + (new Date()).getTime(),
		root = document.documentElement;

	form.innerHTML = "<a name='" + id + "'/>";

	// Inject it into the root element, check its status, and remove it quickly
	root.insertBefore( form, root.firstChild );

	// The workaround has to do additional checks after a getElementById
	// Which slows things down for other browsers (hence the branching)
	if ( document.getElementById( id ) ) {
		Expr.find.ID = function( match, context, isXML ) {
			if ( typeof context.getElementById !== "undefined" && !isXML ) {
				var m = context.getElementById(match[1]);

				return m ?
					m.id === match[1] || typeof m.getAttributeNode !== "undefined" && m.getAttributeNode("id").nodeValue === match[1] ?
						[m] :
						undefined :
					[];
			}
		};

		Expr.filter.ID = function( elem, match ) {
			var node = typeof elem.getAttributeNode !== "undefined" && elem.getAttributeNode("id");

			return elem.nodeType === 1 && node && node.nodeValue === match;
		};
	}

	root.removeChild( form );

	// release memory in IE
	root = form = null;
})();

(function(){
	// Check to see if the browser returns only elements
	// when doing getElementsByTagName("*")

	// Create a fake element
	var div = document.createElement("div");
	div.appendChild( document.createComment("") );

	// Make sure no comments are found
	if ( div.getElementsByTagName("*").length > 0 ) {
		Expr.find.TAG = function( match, context ) {
			var results = context.getElementsByTagName( match[1] );

			// Filter out possible comments
			if ( match[1] === "*" ) {
				var tmp = [];

				for ( var i = 0; results[i]; i++ ) {
					if ( results[i].nodeType === 1 ) {
						tmp.push( results[i] );
					}
				}

				results = tmp;
			}

			return results;
		};
	}

	// Check to see if an attribute returns normalized href attributes
	div.innerHTML = "<a href='#'></a>";

	if ( div.firstChild && typeof div.firstChild.getAttribute !== "undefined" &&
			div.firstChild.getAttribute("href") !== "#" ) {

		Expr.attrHandle.href = function( elem ) {
			return elem.getAttribute( "href", 2 );
		};
	}

	// release memory in IE
	div = null;
})();

if ( document.querySelectorAll ) {
	(function(){
		var oldSizzle = Sizzle,
			div = document.createElement("div"),
			id = "__sizzle__";

		div.innerHTML = "<p class='TEST'></p>";

		// Safari can't handle uppercase or unicode characters when
		// in quirks mode.
		if ( div.querySelectorAll && div.querySelectorAll(".TEST").length === 0 ) {
			return;
		}

		Sizzle = function( query, context, extra, seed ) {
			context = context || document;

			// Only use querySelectorAll on non-XML documents
			// (ID selectors don't work in non-HTML documents)
			if ( !seed && !Sizzle.isXML(context) ) {
				// See if we find a selector to speed up
				var match = /^(\w+$)|^\.([\w\-]+$)|^#([\w\-]+$)/.exec( query );

				if ( match && (context.nodeType === 1 || context.nodeType === 9) ) {
					// Speed-up: Sizzle("TAG")
					if ( match[1] ) {
						return makeArray( context.getElementsByTagName( query ), extra );

					// Speed-up: Sizzle(".CLASS")
					} else if ( match[2] && Expr.find.CLASS && context.getElementsByClassName ) {
						return makeArray( context.getElementsByClassName( match[2] ), extra );
					}
				}

				if ( context.nodeType === 9 ) {
					// Speed-up: Sizzle("body")
					// The body element only exists once, optimize finding it
					if ( query === "body" && context.body ) {
						return makeArray( [ context.body ], extra );

					// Speed-up: Sizzle("#ID")
					} else if ( match && match[3] ) {
						var elem = context.getElementById( match[3] );

						// Check parentNode to catch when Blackberry 4.6 returns
						// nodes that are no longer in the document #6963
						if ( elem && elem.parentNode ) {
							// Handle the case where IE and Opera return items
							// by name instead of ID
							if ( elem.id === match[3] ) {
								return makeArray( [ elem ], extra );
							}

						} else {
							return makeArray( [], extra );
						}
					}

					try {
						return makeArray( context.querySelectorAll(query), extra );
					} catch(qsaError) {}

				// qSA works strangely on Element-rooted queries
				// We can work around this by specifying an extra ID on the root
				// and working up from there (Thanks to Andrew Dupont for the technique)
				// IE 8 doesn't work on object elements
				} else if ( context.nodeType === 1 && context.nodeName.toLowerCase() !== "object" ) {
					var oldContext = context,
						old = context.getAttribute( "id" ),
						nid = old || id,
						hasParent = context.parentNode,
						relativeHierarchySelector = /^\s*[+~]/.test( query );

					if ( !old ) {
						context.setAttribute( "id", nid );
					} else {
						nid = nid.replace( /'/g, "\\$&" );
					}
					if ( relativeHierarchySelector && hasParent ) {
						context = context.parentNode;
					}

					try {
						if ( !relativeHierarchySelector || hasParent ) {
							return makeArray( context.querySelectorAll( "[id='" + nid + "'] " + query ), extra );
						}

					} catch(pseudoError) {
					} finally {
						if ( !old ) {
							oldContext.removeAttribute( "id" );
						}
					}
				}
			}

			return oldSizzle(query, context, extra, seed);
		};

		for ( var prop in oldSizzle ) {
			Sizzle[ prop ] = oldSizzle[ prop ];
		}

		// release memory in IE
		div = null;
	})();
}

(function(){
	var html = document.documentElement,
		matches = html.matchesSelector || html.mozMatchesSelector || html.webkitMatchesSelector || html.msMatchesSelector;

	if ( matches ) {
		// Check to see if it's possible to do matchesSelector
		// on a disconnected node (IE 9 fails this)
		var disconnectedMatch = !matches.call( document.createElement( "div" ), "div" ),
			pseudoWorks = false;

		try {
			// This should fail with an exception
			// Gecko does not error, returns false instead
			matches.call( document.documentElement, "[test!='']:sizzle" );

		} catch( pseudoError ) {
			pseudoWorks = true;
		}

		Sizzle.matchesSelector = function( node, expr ) {
			// Make sure that attribute selectors are quoted
			expr = expr.replace(/\=\s*([^'"\]]*)\s*\]/g, "='$1']");

			if ( !Sizzle.isXML( node ) ) {
				try {
					if ( pseudoWorks || !Expr.match.PSEUDO.test( expr ) && !/!=/.test( expr ) ) {
						var ret = matches.call( node, expr );

						// IE 9's matchesSelector returns false on disconnected nodes
						if ( ret || !disconnectedMatch ||
								// As well, disconnected nodes are said to be in a document
								// fragment in IE 9, so check for that
								node.document && node.document.nodeType !== 11 ) {
							return ret;
						}
					}
				} catch(e) {}
			}

			return Sizzle(expr, null, null, [node]).length > 0;
		};
	}
})();

(function(){
	var div = document.createElement("div");

	div.innerHTML = "<div class='test e'></div><div class='test'></div>";

	// Opera can't find a second classname (in 9.6)
	// Also, make sure that getElementsByClassName actually exists
	if ( !div.getElementsByClassName || div.getElementsByClassName("e").length === 0 ) {
		return;
	}

	// Safari caches class attributes, doesn't catch changes (in 3.2)
	div.lastChild.className = "e";

	if ( div.getElementsByClassName("e").length === 1 ) {
		return;
	}

	Expr.order.splice(1, 0, "CLASS");
	Expr.find.CLASS = function( match, context, isXML ) {
		if ( typeof context.getElementsByClassName !== "undefined" && !isXML ) {
			return context.getElementsByClassName(match[1]);
		}
	};

	// release memory in IE
	div = null;
})();

function dirNodeCheck( dir, cur, doneName, checkSet, nodeCheck, isXML ) {
	for ( var i = 0, l = checkSet.length; i < l; i++ ) {
		var elem = checkSet[i];

		if ( elem ) {
			var match = false;

			elem = elem[dir];

			while ( elem ) {
				if ( elem[ expando ] === doneName ) {
					match = checkSet[elem.sizset];
					break;
				}

				if ( elem.nodeType === 1 && !isXML ){
					elem[ expando ] = doneName;
					elem.sizset = i;
				}

				if ( elem.nodeName.toLowerCase() === cur ) {
					match = elem;
					break;
				}

				elem = elem[dir];
			}

			checkSet[i] = match;
		}
	}
}

function dirCheck( dir, cur, doneName, checkSet, nodeCheck, isXML ) {
	for ( var i = 0, l = checkSet.length; i < l; i++ ) {
		var elem = checkSet[i];

		if ( elem ) {
			var match = false;

			elem = elem[dir];

			while ( elem ) {
				if ( elem[ expando ] === doneName ) {
					match = checkSet[elem.sizset];
					break;
				}

				if ( elem.nodeType === 1 ) {
					if ( !isXML ) {
						elem[ expando ] = doneName;
						elem.sizset = i;
					}

					if ( typeof cur !== "string" ) {
						if ( elem === cur ) {
							match = true;
							break;
						}

					} else if ( Sizzle.filter( cur, [elem] ).length > 0 ) {
						match = elem;
						break;
					}
				}

				elem = elem[dir];
			}

			checkSet[i] = match;
		}
	}
}

if ( document.documentElement.contains ) {
	Sizzle.contains = function( a, b ) {
		return a !== b && (a.contains ? a.contains(b) : true);
	};

} else if ( document.documentElement.compareDocumentPosition ) {
	Sizzle.contains = function( a, b ) {
		return !!(a.compareDocumentPosition(b) & 16);
	};

} else {
	Sizzle.contains = function() {
		return false;
	};
}

Sizzle.isXML = function( elem ) {
	// documentElement is verified for cases where it doesn't yet exist
	// (such as loading iframes in IE - #4833)
	var documentElement = (elem ? elem.ownerDocument || elem : 0).documentElement;

	return documentElement ? documentElement.nodeName !== "HTML" : false;
};

var posProcess = function( selector, context, seed ) {
	var match,
		tmpSet = [],
		later = "",
		root = context.nodeType ? [context] : context;

	// Position selectors must be done after the filter
	// And so must :not(positional) so we move all PSEUDOs to the end
	while ( (match = Expr.match.PSEUDO.exec( selector )) ) {
		later += match[0];
		selector = selector.replace( Expr.match.PSEUDO, "" );
	}

	selector = Expr.relative[selector] ? selector + "*" : selector;

	for ( var i = 0, l = root.length; i < l; i++ ) {
		Sizzle( selector, root[i], tmpSet, seed );
	}

	return Sizzle.filter( later, tmpSet );
};

// EXPOSE
// Override sizzle attribute retrieval
Sizzle.attr = jQuery.attr;
Sizzle.selectors.attrMap = {};
jQuery.find = Sizzle;
jQuery.expr = Sizzle.selectors;
jQuery.expr[":"] = jQuery.expr.filters;
jQuery.unique = Sizzle.uniqueSort;
jQuery.text = Sizzle.getText;
jQuery.isXMLDoc = Sizzle.isXML;
jQuery.contains = Sizzle.contains;


})();


var runtil = /Until$/,
	rparentsprev = /^(?:parents|prevUntil|prevAll)/,
	// Note: This RegExp should be improved, or likely pulled from Sizzle
	rmultiselector = /,/,
	isSimple = /^.[^:#\[\.,]*$/,
	slice = Array.prototype.slice,
	POS = jQuery.expr.match.globalPOS,
	// methods guaranteed to produce a unique set when starting from a unique set
	guaranteedUnique = {
		children: true,
		contents: true,
		next: true,
		prev: true
	};

jQuery.fn.extend({
	find: function( selector ) {
		var self = this,
			i, l;

		if ( typeof selector !== "string" ) {
			return jQuery( selector ).filter(function() {
				for ( i = 0, l = self.length; i < l; i++ ) {
					if ( jQuery.contains( self[ i ], this ) ) {
						return true;
					}
				}
			});
		}

		var ret = this.pushStack( "", "find", selector ),
			length, n, r;

		for ( i = 0, l = this.length; i < l; i++ ) {
			length = ret.length;
			jQuery.find( selector, this[i], ret );

			if ( i > 0 ) {
				// Make sure that the results are unique
				for ( n = length; n < ret.length; n++ ) {
					for ( r = 0; r < length; r++ ) {
						if ( ret[r] === ret[n] ) {
							ret.splice(n--, 1);
							break;
						}
					}
				}
			}
		}

		return ret;
	},

	has: function( target ) {
		var targets = jQuery( target );
		return this.filter(function() {
			for ( var i = 0, l = targets.length; i < l; i++ ) {
				if ( jQuery.contains( this, targets[i] ) ) {
					return true;
				}
			}
		});
	},

	not: function( selector ) {
		return this.pushStack( winnow(this, selector, false), "not", selector);
	},

	filter: function( selector ) {
		return this.pushStack( winnow(this, selector, true), "filter", selector );
	},

	is: function( selector ) {
		return !!selector && (
			typeof selector === "string" ?
				// If this is a positional selector, check membership in the returned set
				// so $("p:first").is("p:last") won't return true for a doc with two "p".
				POS.test( selector ) ?
					jQuery( selector, this.context ).index( this[0] ) >= 0 :
					jQuery.filter( selector, this ).length > 0 :
				this.filter( selector ).length > 0 );
	},

	closest: function( selectors, context ) {
		var ret = [], i, l, cur = this[0];

		// Array (deprecated as of jQuery 1.7)
		if ( jQuery.isArray( selectors ) ) {
			var level = 1;

			while ( cur && cur.ownerDocument && cur !== context ) {
				for ( i = 0; i < selectors.length; i++ ) {

					if ( jQuery( cur ).is( selectors[ i ] ) ) {
						ret.push({ selector: selectors[ i ], elem: cur, level: level });
					}
				}

				cur = cur.parentNode;
				level++;
			}

			return ret;
		}

		// String
		var pos = POS.test( selectors ) || typeof selectors !== "string" ?
				jQuery( selectors, context || this.context ) :
				0;

		for ( i = 0, l = this.length; i < l; i++ ) {
			cur = this[i];

			while ( cur ) {
				if ( pos ? pos.index(cur) > -1 : jQuery.find.matchesSelector(cur, selectors) ) {
					ret.push( cur );
					break;

				} else {
					cur = cur.parentNode;
					if ( !cur || !cur.ownerDocument || cur === context || cur.nodeType === 11 ) {
						break;
					}
				}
			}
		}

		ret = ret.length > 1 ? jQuery.unique( ret ) : ret;

		return this.pushStack( ret, "closest", selectors );
	},

	// Determine the position of an element within
	// the matched set of elements
	index: function( elem ) {

		// No argument, return index in parent
		if ( !elem ) {
			return ( this[0] && this[0].parentNode ) ? this.prevAll().length : -1;
		}

		// index in selector
		if ( typeof elem === "string" ) {
			return jQuery.inArray( this[0], jQuery( elem ) );
		}

		// Locate the position of the desired element
		return jQuery.inArray(
			// If it receives a jQuery object, the first element is used
			elem.jquery ? elem[0] : elem, this );
	},

	add: function( selector, context ) {
		var set = typeof selector === "string" ?
				jQuery( selector, context ) :
				jQuery.makeArray( selector && selector.nodeType ? [ selector ] : selector ),
			all = jQuery.merge( this.get(), set );

		return this.pushStack( isDisconnected( set[0] ) || isDisconnected( all[0] ) ?
			all :
			jQuery.unique( all ) );
	},

	andSelf: function() {
		return this.add( this.prevObject );
	}
});

// A painfully simple check to see if an element is disconnected
// from a document (should be improved, where feasible).
function isDisconnected( node ) {
	return !node || !node.parentNode || node.parentNode.nodeType === 11;
}

jQuery.each({
	parent: function( elem ) {
		var parent = elem.parentNode;
		return parent && parent.nodeType !== 11 ? parent : null;
	},
	parents: function( elem ) {
		return jQuery.dir( elem, "parentNode" );
	},
	parentsUntil: function( elem, i, until ) {
		return jQuery.dir( elem, "parentNode", until );
	},
	next: function( elem ) {
		return jQuery.nth( elem, 2, "nextSibling" );
	},
	prev: function( elem ) {
		return jQuery.nth( elem, 2, "previousSibling" );
	},
	nextAll: function( elem ) {
		return jQuery.dir( elem, "nextSibling" );
	},
	prevAll: function( elem ) {
		return jQuery.dir( elem, "previousSibling" );
	},
	nextUntil: function( elem, i, until ) {
		return jQuery.dir( elem, "nextSibling", until );
	},
	prevUntil: function( elem, i, until ) {
		return jQuery.dir( elem, "previousSibling", until );
	},
	siblings: function( elem ) {
		return jQuery.sibling( ( elem.parentNode || {} ).firstChild, elem );
	},
	children: function( elem ) {
		return jQuery.sibling( elem.firstChild );
	},
	contents: function( elem ) {
		return jQuery.nodeName( elem, "iframe" ) ?
			elem.contentDocument || elem.contentWindow.document :
			jQuery.makeArray( elem.childNodes );
	}
}, function( name, fn ) {
	jQuery.fn[ name ] = function( until, selector ) {
		var ret = jQuery.map( this, fn, until );

		if ( !runtil.test( name ) ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		ret = this.length > 1 && !guaranteedUnique[ name ] ? jQuery.unique( ret ) : ret;

		if ( (this.length > 1 || rmultiselector.test( selector )) && rparentsprev.test( name ) ) {
			ret = ret.reverse();
		}

		return this.pushStack( ret, name, slice.call( arguments ).join(",") );
	};
});

jQuery.extend({
	filter: function( expr, elems, not ) {
		if ( not ) {
			expr = ":not(" + expr + ")";
		}

		return elems.length === 1 ?
			jQuery.find.matchesSelector(elems[0], expr) ? [ elems[0] ] : [] :
			jQuery.find.matches(expr, elems);
	},

	dir: function( elem, dir, until ) {
		var matched = [],
			cur = elem[ dir ];

		while ( cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !jQuery( cur ).is( until )) ) {
			if ( cur.nodeType === 1 ) {
				matched.push( cur );
			}
			cur = cur[dir];
		}
		return matched;
	},

	nth: function( cur, result, dir, elem ) {
		result = result || 1;
		var num = 0;

		for ( ; cur; cur = cur[dir] ) {
			if ( cur.nodeType === 1 && ++num === result ) {
				break;
			}
		}

		return cur;
	},

	sibling: function( n, elem ) {
		var r = [];

		for ( ; n; n = n.nextSibling ) {
			if ( n.nodeType === 1 && n !== elem ) {
				r.push( n );
			}
		}

		return r;
	}
});

// Implement the identical functionality for filter and not
function winnow( elements, qualifier, keep ) {

	// Can't pass null or undefined to indexOf in Firefox 4
	// Set to 0 to skip string check
	qualifier = qualifier || 0;

	if ( jQuery.isFunction( qualifier ) ) {
		return jQuery.grep(elements, function( elem, i ) {
			var retVal = !!qualifier.call( elem, i, elem );
			return retVal === keep;
		});

	} else if ( qualifier.nodeType ) {
		return jQuery.grep(elements, function( elem, i ) {
			return ( elem === qualifier ) === keep;
		});

	} else if ( typeof qualifier === "string" ) {
		var filtered = jQuery.grep(elements, function( elem ) {
			return elem.nodeType === 1;
		});

		if ( isSimple.test( qualifier ) ) {
			return jQuery.filter(qualifier, filtered, !keep);
		} else {
			qualifier = jQuery.filter( qualifier, filtered );
		}
	}

	return jQuery.grep(elements, function( elem, i ) {
		return ( jQuery.inArray( elem, qualifier ) >= 0 ) === keep;
	});
}




function createSafeFragment( document ) {
	var list = nodeNames.split( "|" ),
	safeFrag = document.createDocumentFragment();

	if ( safeFrag.createElement ) {
		while ( list.length ) {
			safeFrag.createElement(
				list.pop()
			);
		}
	}
	return safeFrag;
}

var nodeNames = "abbr|article|aside|audio|bdi|canvas|data|datalist|details|figcaption|figure|footer|" +
		"header|hgroup|mark|meter|nav|output|progress|section|summary|time|video",
	rinlinejQuery = / jQuery\d+="(?:\d+|null)"/g,
	rleadingWhitespace = /^\s+/,
	rxhtmlTag = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/ig,
	rtagName = /<([\w:]+)/,
	rtbody = /<tbody/i,
	rhtml = /<|&#?\w+;/,
	rnoInnerhtml = /<(?:script|style)/i,
	rnocache = /<(?:script|object|embed|option|style)/i,
	rnoshimcache = new RegExp("<(?:" + nodeNames + ")[\\s/>]", "i"),
	// checked="checked" or checked
	rchecked = /checked\s*(?:[^=]|=\s*.checked.)/i,
	rscriptType = /\/(java|ecma)script/i,
	rcleanScript = /^\s*<!(?:\[CDATA\[|\-\-)/,
	wrapMap = {
		option: [ 1, "<select multiple='multiple'>", "</select>" ],
		legend: [ 1, "<fieldset>", "</fieldset>" ],
		thead: [ 1, "<table>", "</table>" ],
		tr: [ 2, "<table><tbody>", "</tbody></table>" ],
		td: [ 3, "<table><tbody><tr>", "</tr></tbody></table>" ],
		col: [ 2, "<table><tbody></tbody><colgroup>", "</colgroup></table>" ],
		area: [ 1, "<map>", "</map>" ],
		_default: [ 0, "", "" ]
	},
	safeFragment = createSafeFragment( document );

wrapMap.optgroup = wrapMap.option;
wrapMap.tbody = wrapMap.tfoot = wrapMap.colgroup = wrapMap.caption = wrapMap.thead;
wrapMap.th = wrapMap.td;

// IE can't serialize <link> and <script> tags normally
if ( !jQuery.support.htmlSerialize ) {
	wrapMap._default = [ 1, "div<div>", "</div>" ];
}

jQuery.fn.extend({
	text: function( value ) {
		return jQuery.access( this, function( value ) {
			return value === undefined ?
				jQuery.text( this ) :
				this.empty().append( ( this[0] && this[0].ownerDocument || document ).createTextNode( value ) );
		}, null, value, arguments.length );
	},

	wrapAll: function( html ) {
		if ( jQuery.isFunction( html ) ) {
			return this.each(function(i) {
				jQuery(this).wrapAll( html.call(this, i) );
			});
		}

		if ( this[0] ) {
			// The elements to wrap the target around
			var wrap = jQuery( html, this[0].ownerDocument ).eq(0).clone(true);

			if ( this[0].parentNode ) {
				wrap.insertBefore( this[0] );
			}

			wrap.map(function() {
				var elem = this;

				while ( elem.firstChild && elem.firstChild.nodeType === 1 ) {
					elem = elem.firstChild;
				}

				return elem;
			}).append( this );
		}

		return this;
	},

	wrapInner: function( html ) {
		if ( jQuery.isFunction( html ) ) {
			return this.each(function(i) {
				jQuery(this).wrapInner( html.call(this, i) );
			});
		}

		return this.each(function() {
			var self = jQuery( this ),
				contents = self.contents();

			if ( contents.length ) {
				contents.wrapAll( html );

			} else {
				self.append( html );
			}
		});
	},

	wrap: function( html ) {
		var isFunction = jQuery.isFunction( html );

		return this.each(function(i) {
			jQuery( this ).wrapAll( isFunction ? html.call(this, i) : html );
		});
	},

	unwrap: function() {
		return this.parent().each(function() {
			if ( !jQuery.nodeName( this, "body" ) ) {
				jQuery( this ).replaceWith( this.childNodes );
			}
		}).end();
	},

	append: function() {
		return this.domManip(arguments, true, function( elem ) {
			if ( this.nodeType === 1 ) {
				this.appendChild( elem );
			}
		});
	},

	prepend: function() {
		return this.domManip(arguments, true, function( elem ) {
			if ( this.nodeType === 1 ) {
				this.insertBefore( elem, this.firstChild );
			}
		});
	},

	before: function() {
		if ( this[0] && this[0].parentNode ) {
			return this.domManip(arguments, false, function( elem ) {
				this.parentNode.insertBefore( elem, this );
			});
		} else if ( arguments.length ) {
			var set = jQuery.clean( arguments );
			set.push.apply( set, this.toArray() );
			return this.pushStack( set, "before", arguments );
		}
	},

	after: function() {
		if ( this[0] && this[0].parentNode ) {
			return this.domManip(arguments, false, function( elem ) {
				this.parentNode.insertBefore( elem, this.nextSibling );
			});
		} else if ( arguments.length ) {
			var set = this.pushStack( this, "after", arguments );
			set.push.apply( set, jQuery.clean(arguments) );
			return set;
		}
	},

	// keepData is for internal use only--do not document
	remove: function( selector, keepData ) {
		for ( var i = 0, elem; (elem = this[i]) != null; i++ ) {
			if ( !selector || jQuery.filter( selector, [ elem ] ).length ) {
				if ( !keepData && elem.nodeType === 1 ) {
					jQuery.cleanData( elem.getElementsByTagName("*") );
					jQuery.cleanData( [ elem ] );
				}

				if ( elem.parentNode ) {
					elem.parentNode.removeChild( elem );
				}
			}
		}

		return this;
	},

	empty: function() {
		for ( var i = 0, elem; (elem = this[i]) != null; i++ ) {
			// Remove element nodes and prevent memory leaks
			if ( elem.nodeType === 1 ) {
				jQuery.cleanData( elem.getElementsByTagName("*") );
			}

			// Remove any remaining nodes
			while ( elem.firstChild ) {
				elem.removeChild( elem.firstChild );
			}
		}

		return this;
	},

	clone: function( dataAndEvents, deepDataAndEvents ) {
		dataAndEvents = dataAndEvents == null ? false : dataAndEvents;
		deepDataAndEvents = deepDataAndEvents == null ? dataAndEvents : deepDataAndEvents;

		return this.map( function () {
			return jQuery.clone( this, dataAndEvents, deepDataAndEvents );
		});
	},

	html: function( value ) {
		return jQuery.access( this, function( value ) {
			var elem = this[0] || {},
				i = 0,
				l = this.length;

			if ( value === undefined ) {
				return elem.nodeType === 1 ?
					elem.innerHTML.replace( rinlinejQuery, "" ) :
					null;
			}


			if ( typeof value === "string" && !rnoInnerhtml.test( value ) &&
				( jQuery.support.leadingWhitespace || !rleadingWhitespace.test( value ) ) &&
				!wrapMap[ ( rtagName.exec( value ) || ["", ""] )[1].toLowerCase() ] ) {

				value = value.replace( rxhtmlTag, "<$1></$2>" );

				try {
					for (; i < l; i++ ) {
						// Remove element nodes and prevent memory leaks
						elem = this[i] || {};
						if ( elem.nodeType === 1 ) {
							jQuery.cleanData( elem.getElementsByTagName( "*" ) );
							elem.innerHTML = value;
						}
					}

					elem = 0;

				// If using innerHTML throws an exception, use the fallback method
				} catch(e) {}
			}

			if ( elem ) {
				this.empty().append( value );
			}
		}, null, value, arguments.length );
	},

	replaceWith: function( value ) {
		if ( this[0] && this[0].parentNode ) {
			// Make sure that the elements are removed from the DOM before they are inserted
			// this can help fix replacing a parent with child elements
			if ( jQuery.isFunction( value ) ) {
				return this.each(function(i) {
					var self = jQuery(this), old = self.html();
					self.replaceWith( value.call( this, i, old ) );
				});
			}

			if ( typeof value !== "string" ) {
				value = jQuery( value ).detach();
			}

			return this.each(function() {
				var next = this.nextSibling,
					parent = this.parentNode;

				jQuery( this ).remove();

				if ( next ) {
					jQuery(next).before( value );
				} else {
					jQuery(parent).append( value );
				}
			});
		} else {
			return this.length ?
				this.pushStack( jQuery(jQuery.isFunction(value) ? value() : value), "replaceWith", value ) :
				this;
		}
	},

	detach: function( selector ) {
		return this.remove( selector, true );
	},

	domManip: function( args, table, callback ) {
		var results, first, fragment, parent,
			value = args[0],
			scripts = [];

		// We can't cloneNode fragments that contain checked, in WebKit
		if ( !jQuery.support.checkClone && arguments.length === 3 && typeof value === "string" && rchecked.test( value ) ) {
			return this.each(function() {
				jQuery(this).domManip( args, table, callback, true );
			});
		}

		if ( jQuery.isFunction(value) ) {
			return this.each(function(i) {
				var self = jQuery(this);
				args[0] = value.call(this, i, table ? self.html() : undefined);
				self.domManip( args, table, callback );
			});
		}

		if ( this[0] ) {
			parent = value && value.parentNode;

			// If we're in a fragment, just use that instead of building a new one
			if ( jQuery.support.parentNode && parent && parent.nodeType === 11 && parent.childNodes.length === this.length ) {
				results = { fragment: parent };

			} else {
				results = jQuery.buildFragment( args, this, scripts );
			}

			fragment = results.fragment;

			if ( fragment.childNodes.length === 1 ) {
				first = fragment = fragment.firstChild;
			} else {
				first = fragment.firstChild;
			}

			if ( first ) {
				table = table && jQuery.nodeName( first, "tr" );

				for ( var i = 0, l = this.length, lastIndex = l - 1; i < l; i++ ) {
					callback.call(
						table ?
							root(this[i], first) :
							this[i],
						// Make sure that we do not leak memory by inadvertently discarding
						// the original fragment (which might have attached data) instead of
						// using it; in addition, use the original fragment object for the last
						// item instead of first because it can end up being emptied incorrectly
						// in certain situations (Bug #8070).
						// Fragments from the fragment cache must always be cloned and never used
						// in place.
						results.cacheable || ( l > 1 && i < lastIndex ) ?
							jQuery.clone( fragment, true, true ) :
							fragment
					);
				}
			}

			if ( scripts.length ) {
				jQuery.each( scripts, function( i, elem ) {
					if ( elem.src ) {
						jQuery.ajax({
							type: "GET",
							global: false,
							url: elem.src,
							async: false,
							dataType: "script"
						});
					} else {
						jQuery.globalEval( ( elem.text || elem.textContent || elem.innerHTML || "" ).replace( rcleanScript, "/*$0*/" ) );
					}

					if ( elem.parentNode ) {
						elem.parentNode.removeChild( elem );
					}
				});
			}
		}

		return this;
	}
});

function root( elem, cur ) {
	return jQuery.nodeName(elem, "table") ?
		(elem.getElementsByTagName("tbody")[0] ||
		elem.appendChild(elem.ownerDocument.createElement("tbody"))) :
		elem;
}

function cloneCopyEvent( src, dest ) {

	if ( dest.nodeType !== 1 || !jQuery.hasData( src ) ) {
		return;
	}

	var type, i, l,
		oldData = jQuery._data( src ),
		curData = jQuery._data( dest, oldData ),
		events = oldData.events;

	if ( events ) {
		delete curData.handle;
		curData.events = {};

		for ( type in events ) {
			for ( i = 0, l = events[ type ].length; i < l; i++ ) {
				jQuery.event.add( dest, type, events[ type ][ i ] );
			}
		}
	}

	// make the cloned public data object a copy from the original
	if ( curData.data ) {
		curData.data = jQuery.extend( {}, curData.data );
	}
}

function cloneFixAttributes( src, dest ) {
	var nodeName;

	// We do not need to do anything for non-Elements
	if ( dest.nodeType !== 1 ) {
		return;
	}

	// clearAttributes removes the attributes, which we don't want,
	// but also removes the attachEvent events, which we *do* want
	if ( dest.clearAttributes ) {
		dest.clearAttributes();
	}

	// mergeAttributes, in contrast, only merges back on the
	// original attributes, not the events
	if ( dest.mergeAttributes ) {
		dest.mergeAttributes( src );
	}

	nodeName = dest.nodeName.toLowerCase();

	// IE6-8 fail to clone children inside object elements that use
	// the proprietary classid attribute value (rather than the type
	// attribute) to identify the type of content to display
	if ( nodeName === "object" ) {
		dest.outerHTML = src.outerHTML;

	} else if ( nodeName === "input" && (src.type === "checkbox" || src.type === "radio") ) {
		// IE6-8 fails to persist the checked state of a cloned checkbox
		// or radio button. Worse, IE6-7 fail to give the cloned element
		// a checked appearance if the defaultChecked value isn't also set
		if ( src.checked ) {
			dest.defaultChecked = dest.checked = src.checked;
		}

		// IE6-7 get confused and end up setting the value of a cloned
		// checkbox/radio button to an empty string instead of "on"
		if ( dest.value !== src.value ) {
			dest.value = src.value;
		}

	// IE6-8 fails to return the selected option to the default selected
	// state when cloning options
	} else if ( nodeName === "option" ) {
		dest.selected = src.defaultSelected;

	// IE6-8 fails to set the defaultValue to the correct value when
	// cloning other types of input fields
	} else if ( nodeName === "input" || nodeName === "textarea" ) {
		dest.defaultValue = src.defaultValue;

	// IE blanks contents when cloning scripts
	} else if ( nodeName === "script" && dest.text !== src.text ) {
		dest.text = src.text;
	}

	// Event data gets referenced instead of copied if the expando
	// gets copied too
	dest.removeAttribute( jQuery.expando );

	// Clear flags for bubbling special change/submit events, they must
	// be reattached when the newly cloned events are first activated
	dest.removeAttribute( "_submit_attached" );
	dest.removeAttribute( "_change_attached" );
}

jQuery.buildFragment = function( args, nodes, scripts ) {
	var fragment, cacheable, cacheresults, doc,
	first = args[ 0 ];

	// nodes may contain either an explicit document object,
	// a jQuery collection or context object.
	// If nodes[0] contains a valid object to assign to doc
	if ( nodes && nodes[0] ) {
		doc = nodes[0].ownerDocument || nodes[0];
	}

	// Ensure that an attr object doesn't incorrectly stand in as a document object
	// Chrome and Firefox seem to allow this to occur and will throw exception
	// Fixes #8950
	if ( !doc.createDocumentFragment ) {
		doc = document;
	}

	// Only cache "small" (1/2 KB) HTML strings that are associated with the main document
	// Cloning options loses the selected state, so don't cache them
	// IE 6 doesn't like it when you put <object> or <embed> elements in a fragment
	// Also, WebKit does not clone 'checked' attributes on cloneNode, so don't cache
	// Lastly, IE6,7,8 will not correctly reuse cached fragments that were created from unknown elems #10501
	if ( args.length === 1 && typeof first === "string" && first.length < 512 && doc === document &&
		first.charAt(0) === "<" && !rnocache.test( first ) &&
		(jQuery.support.checkClone || !rchecked.test( first )) &&
		(jQuery.support.html5Clone || !rnoshimcache.test( first )) ) {

		cacheable = true;

		cacheresults = jQuery.fragments[ first ];
		if ( cacheresults && cacheresults !== 1 ) {
			fragment = cacheresults;
		}
	}

	if ( !fragment ) {
		fragment = doc.createDocumentFragment();
		jQuery.clean( args, doc, fragment, scripts );
	}

	if ( cacheable ) {
		jQuery.fragments[ first ] = cacheresults ? fragment : 1;
	}

	return { fragment: fragment, cacheable: cacheable };
};

jQuery.fragments = {};

jQuery.each({
	appendTo: "append",
	prependTo: "prepend",
	insertBefore: "before",
	insertAfter: "after",
	replaceAll: "replaceWith"
}, function( name, original ) {
	jQuery.fn[ name ] = function( selector ) {
		var ret = [],
			insert = jQuery( selector ),
			parent = this.length === 1 && this[0].parentNode;

		if ( parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1 ) {
			insert[ original ]( this[0] );
			return this;

		} else {
			for ( var i = 0, l = insert.length; i < l; i++ ) {
				var elems = ( i > 0 ? this.clone(true) : this ).get();
				jQuery( insert[i] )[ original ]( elems );
				ret = ret.concat( elems );
			}

			return this.pushStack( ret, name, insert.selector );
		}
	};
});

function getAll( elem ) {
	if ( typeof elem.getElementsByTagName !== "undefined" ) {
		return elem.getElementsByTagName( "*" );

	} else if ( typeof elem.querySelectorAll !== "undefined" ) {
		return elem.querySelectorAll( "*" );

	} else {
		return [];
	}
}

// Used in clean, fixes the defaultChecked property
function fixDefaultChecked( elem ) {
	if ( elem.type === "checkbox" || elem.type === "radio" ) {
		elem.defaultChecked = elem.checked;
	}
}
// Finds all inputs and passes them to fixDefaultChecked
function findInputs( elem ) {
	var nodeName = ( elem.nodeName || "" ).toLowerCase();
	if ( nodeName === "input" ) {
		fixDefaultChecked( elem );
	// Skip scripts, get other children
	} else if ( nodeName !== "script" && typeof elem.getElementsByTagName !== "undefined" ) {
		jQuery.grep( elem.getElementsByTagName("input"), fixDefaultChecked );
	}
}

// Derived From: http://www.iecss.com/shimprove/javascript/shimprove.1-0-1.js
function shimCloneNode( elem ) {
	var div = document.createElement( "div" );
	safeFragment.appendChild( div );

	div.innerHTML = elem.outerHTML;
	return div.firstChild;
}

jQuery.extend({
	clone: function( elem, dataAndEvents, deepDataAndEvents ) {
		var srcElements,
			destElements,
			i,
			// IE<=8 does not properly clone detached, unknown element nodes
			clone = jQuery.support.html5Clone || jQuery.isXMLDoc(elem) || !rnoshimcache.test( "<" + elem.nodeName + ">" ) ?
				elem.cloneNode( true ) :
				shimCloneNode( elem );

		if ( (!jQuery.support.noCloneEvent || !jQuery.support.noCloneChecked) &&
				(elem.nodeType === 1 || elem.nodeType === 11) && !jQuery.isXMLDoc(elem) ) {
			// IE copies events bound via attachEvent when using cloneNode.
			// Calling detachEvent on the clone will also remove the events
			// from the original. In order to get around this, we use some
			// proprietary methods to clear the events. Thanks to MooTools
			// guys for this hotness.

			cloneFixAttributes( elem, clone );

			// Using Sizzle here is crazy slow, so we use getElementsByTagName instead
			srcElements = getAll( elem );
			destElements = getAll( clone );

			// Weird iteration because IE will replace the length property
			// with an element if you are cloning the body and one of the
			// elements on the page has a name or id of "length"
			for ( i = 0; srcElements[i]; ++i ) {
				// Ensure that the destination node is not null; Fixes #9587
				if ( destElements[i] ) {
					cloneFixAttributes( srcElements[i], destElements[i] );
				}
			}
		}

		// Copy the events from the original to the clone
		if ( dataAndEvents ) {
			cloneCopyEvent( elem, clone );

			if ( deepDataAndEvents ) {
				srcElements = getAll( elem );
				destElements = getAll( clone );

				for ( i = 0; srcElements[i]; ++i ) {
					cloneCopyEvent( srcElements[i], destElements[i] );
				}
			}
		}

		srcElements = destElements = null;

		// Return the cloned set
		return clone;
	},

	clean: function( elems, context, fragment, scripts ) {
		var checkScriptType, script, j,
				ret = [];

		context = context || document;

		// !context.createElement fails in IE with an error but returns typeof 'object'
		if ( typeof context.createElement === "undefined" ) {
			context = context.ownerDocument || context[0] && context[0].ownerDocument || document;
		}

		for ( var i = 0, elem; (elem = elems[i]) != null; i++ ) {
			if ( typeof elem === "number" ) {
				elem += "";
			}

			if ( !elem ) {
				continue;
			}

			// Convert html string into DOM nodes
			if ( typeof elem === "string" ) {
				if ( !rhtml.test( elem ) ) {
					elem = context.createTextNode( elem );
				} else {
					// Fix "XHTML"-style tags in all browsers
					elem = elem.replace(rxhtmlTag, "<$1></$2>");

					// Trim whitespace, otherwise indexOf won't work as expected
					var tag = ( rtagName.exec( elem ) || ["", ""] )[1].toLowerCase(),
						wrap = wrapMap[ tag ] || wrapMap._default,
						depth = wrap[0],
						div = context.createElement("div"),
						safeChildNodes = safeFragment.childNodes,
						remove;

					// Append wrapper element to unknown element safe doc fragment
					if ( context === document ) {
						// Use the fragment we've already created for this document
						safeFragment.appendChild( div );
					} else {
						// Use a fragment created with the owner document
						createSafeFragment( context ).appendChild( div );
					}

					// Go to html and back, then peel off extra wrappers
					div.innerHTML = wrap[1] + elem + wrap[2];

					// Move to the right depth
					while ( depth-- ) {
						div = div.lastChild;
					}

					// Remove IE's autoinserted <tbody> from table fragments
					if ( !jQuery.support.tbody ) {

						// String was a <table>, *may* have spurious <tbody>
						var hasBody = rtbody.test(elem),
							tbody = tag === "table" && !hasBody ?
								div.firstChild && div.firstChild.childNodes :

								// String was a bare <thead> or <tfoot>
								wrap[1] === "<table>" && !hasBody ?
									div.childNodes :
									[];

						for ( j = tbody.length - 1; j >= 0 ; --j ) {
							if ( jQuery.nodeName( tbody[ j ], "tbody" ) && !tbody[ j ].childNodes.length ) {
								tbody[ j ].parentNode.removeChild( tbody[ j ] );
							}
						}
					}

					// IE completely kills leading whitespace when innerHTML is used
					if ( !jQuery.support.leadingWhitespace && rleadingWhitespace.test( elem ) ) {
						div.insertBefore( context.createTextNode( rleadingWhitespace.exec(elem)[0] ), div.firstChild );
					}

					elem = div.childNodes;

					// Clear elements from DocumentFragment (safeFragment or otherwise)
					// to avoid hoarding elements. Fixes #11356
					if ( div ) {
						div.parentNode.removeChild( div );

						// Guard against -1 index exceptions in FF3.6
						if ( safeChildNodes.length > 0 ) {
							remove = safeChildNodes[ safeChildNodes.length - 1 ];

							if ( remove && remove.parentNode ) {
								remove.parentNode.removeChild( remove );
							}
						}
					}
				}
			}

			// Resets defaultChecked for any radios and checkboxes
			// about to be appended to the DOM in IE 6/7 (#8060)
			var len;
			if ( !jQuery.support.appendChecked ) {
				if ( elem[0] && typeof (len = elem.length) === "number" ) {
					for ( j = 0; j < len; j++ ) {
						findInputs( elem[j] );
					}
				} else {
					findInputs( elem );
				}
			}

			if ( elem.nodeType ) {
				ret.push( elem );
			} else {
				ret = jQuery.merge( ret, elem );
			}
		}

		if ( fragment ) {
			checkScriptType = function( elem ) {
				return !elem.type || rscriptType.test( elem.type );
			};
			for ( i = 0; ret[i]; i++ ) {
				script = ret[i];
				if ( scripts && jQuery.nodeName( script, "script" ) && (!script.type || rscriptType.test( script.type )) ) {
					scripts.push( script.parentNode ? script.parentNode.removeChild( script ) : script );

				} else {
					if ( script.nodeType === 1 ) {
						var jsTags = jQuery.grep( script.getElementsByTagName( "script" ), checkScriptType );

						ret.splice.apply( ret, [i + 1, 0].concat( jsTags ) );
					}
					fragment.appendChild( script );
				}
			}
		}

		return ret;
	},

	cleanData: function( elems ) {
		var data, id,
			cache = jQuery.cache,
			special = jQuery.event.special,
			deleteExpando = jQuery.support.deleteExpando;

		for ( var i = 0, elem; (elem = elems[i]) != null; i++ ) {
			if ( elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()] ) {
				continue;
			}

			id = elem[ jQuery.expando ];

			if ( id ) {
				data = cache[ id ];

				if ( data && data.events ) {
					for ( var type in data.events ) {
						if ( special[ type ] ) {
							jQuery.event.remove( elem, type );

						// This is a shortcut to avoid jQuery.event.remove's overhead
						} else {
							jQuery.removeEvent( elem, type, data.handle );
						}
					}

					// Null the DOM reference to avoid IE6/7/8 leak (#7054)
					if ( data.handle ) {
						data.handle.elem = null;
					}
				}

				if ( deleteExpando ) {
					delete elem[ jQuery.expando ];

				} else if ( elem.removeAttribute ) {
					elem.removeAttribute( jQuery.expando );
				}

				delete cache[ id ];
			}
		}
	}
});




var ralpha = /alpha\([^)]*\)/i,
	ropacity = /opacity=([^)]*)/,
	// fixed for IE9, see #8346
	rupper = /([A-Z]|^ms)/g,
	rnum = /^[\-+]?(?:\d*\.)?\d+$/i,
	rnumnonpx = /^-?(?:\d*\.)?\d+(?!px)[^\d\s]+$/i,
	rrelNum = /^([\-+])=([\-+.\de]+)/,
	rmargin = /^margin/,

	cssShow = { position: "absolute", visibility: "hidden", display: "block" },

	// order is important!
	cssExpand = [ "Top", "Right", "Bottom", "Left" ],

	curCSS,

	getComputedStyle,
	currentStyle;

jQuery.fn.css = function( name, value ) {
	return jQuery.access( this, function( elem, name, value ) {
		return value !== undefined ?
			jQuery.style( elem, name, value ) :
			jQuery.css( elem, name );
	}, name, value, arguments.length > 1 );
};

jQuery.extend({
	// Add in style property hooks for overriding the default
	// behavior of getting and setting a style property
	cssHooks: {
		opacity: {
			get: function( elem, computed ) {
				if ( computed ) {
					// We should always get a number back from opacity
					var ret = curCSS( elem, "opacity" );
					return ret === "" ? "1" : ret;

				} else {
					return elem.style.opacity;
				}
			}
		}
	},

	// Exclude the following css properties to add px
	cssNumber: {
		"fillOpacity": true,
		"fontWeight": true,
		"lineHeight": true,
		"opacity": true,
		"orphans": true,
		"widows": true,
		"zIndex": true,
		"zoom": true
	},

	// Add in properties whose names you wish to fix before
	// setting or getting the value
	cssProps: {
		// normalize float css property
		"float": jQuery.support.cssFloat ? "cssFloat" : "styleFloat"
	},

	// Get and set the style property on a DOM Node
	style: function( elem, name, value, extra ) {
		// Don't set styles on text and comment nodes
		if ( !elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style ) {
			return;
		}

		// Make sure that we're working with the right name
		var ret, type, origName = jQuery.camelCase( name ),
			style = elem.style, hooks = jQuery.cssHooks[ origName ];

		name = jQuery.cssProps[ origName ] || origName;

		// Check if we're setting a value
		if ( value !== undefined ) {
			type = typeof value;

			// convert relative number strings (+= or -=) to relative numbers. #7345
			if ( type === "string" && (ret = rrelNum.exec( value )) ) {
				value = ( +( ret[1] + 1) * +ret[2] ) + parseFloat( jQuery.css( elem, name ) );
				// Fixes bug #9237
				type = "number";
			}

			// Make sure that NaN and null values aren't set. See: #7116
			if ( value == null || type === "number" && isNaN( value ) ) {
				return;
			}

			// If a number was passed in, add 'px' to the (except for certain CSS properties)
			if ( type === "number" && !jQuery.cssNumber[ origName ] ) {
				value += "px";
			}

			// If a hook was provided, use that value, otherwise just set the specified value
			if ( !hooks || !("set" in hooks) || (value = hooks.set( elem, value )) !== undefined ) {
				// Wrapped to prevent IE from throwing errors when 'invalid' values are provided
				// Fixes bug #5509
				try {
					style[ name ] = value;
				} catch(e) {}
			}

		} else {
			// If a hook was provided get the non-computed value from there
			if ( hooks && "get" in hooks && (ret = hooks.get( elem, false, extra )) !== undefined ) {
				return ret;
			}

			// Otherwise just get the value from the style object
			return style[ name ];
		}
	},

	css: function( elem, name, extra ) {
		var ret, hooks;

		// Make sure that we're working with the right name
		name = jQuery.camelCase( name );
		hooks = jQuery.cssHooks[ name ];
		name = jQuery.cssProps[ name ] || name;

		// cssFloat needs a special treatment
		if ( name === "cssFloat" ) {
			name = "float";
		}

		// If a hook was provided get the computed value from there
		if ( hooks && "get" in hooks && (ret = hooks.get( elem, true, extra )) !== undefined ) {
			return ret;

		// Otherwise, if a way to get the computed value exists, use that
		} else if ( curCSS ) {
			return curCSS( elem, name );
		}
	},

	// A method for quickly swapping in/out CSS properties to get correct calculations
	swap: function( elem, options, callback ) {
		var old = {},
			ret, name;

		// Remember the old values, and insert the new ones
		for ( name in options ) {
			old[ name ] = elem.style[ name ];
			elem.style[ name ] = options[ name ];
		}

		ret = callback.call( elem );

		// Revert the old values
		for ( name in options ) {
			elem.style[ name ] = old[ name ];
		}

		return ret;
	}
});

// DEPRECATED in 1.3, Use jQuery.css() instead
jQuery.curCSS = jQuery.css;

if ( document.defaultView && document.defaultView.getComputedStyle ) {
	getComputedStyle = function( elem, name ) {
		var ret, defaultView, computedStyle, width,
			style = elem.style;

		name = name.replace( rupper, "-$1" ).toLowerCase();

		if ( (defaultView = elem.ownerDocument.defaultView) &&
				(computedStyle = defaultView.getComputedStyle( elem, null )) ) {

			ret = computedStyle.getPropertyValue( name );
			if ( ret === "" && !jQuery.contains( elem.ownerDocument.documentElement, elem ) ) {
				ret = jQuery.style( elem, name );
			}
		}

		// A tribute to the "awesome hack by Dean Edwards"
		// WebKit uses "computed value (percentage if specified)" instead of "used value" for margins
		// which is against the CSSOM draft spec: http://dev.w3.org/csswg/cssom/#resolved-values
		if ( !jQuery.support.pixelMargin && computedStyle && rmargin.test( name ) && rnumnonpx.test( ret ) ) {
			width = style.width;
			style.width = ret;
			ret = computedStyle.width;
			style.width = width;
		}

		return ret;
	};
}

if ( document.documentElement.currentStyle ) {
	currentStyle = function( elem, name ) {
		var left, rsLeft, uncomputed,
			ret = elem.currentStyle && elem.currentStyle[ name ],
			style = elem.style;

		// Avoid setting ret to empty string here
		// so we don't default to auto
		if ( ret == null && style && (uncomputed = style[ name ]) ) {
			ret = uncomputed;
		}

		// From the awesome hack by Dean Edwards
		// http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

		// If we're not dealing with a regular pixel number
		// but a number that has a weird ending, we need to convert it to pixels
		if ( rnumnonpx.test( ret ) ) {

			// Remember the original values
			left = style.left;
			rsLeft = elem.runtimeStyle && elem.runtimeStyle.left;

			// Put in the new values to get a computed value out
			if ( rsLeft ) {
				elem.runtimeStyle.left = elem.currentStyle.left;
			}
			style.left = name === "fontSize" ? "1em" : ret;
			ret = style.pixelLeft + "px";

			// Revert the changed values
			style.left = left;
			if ( rsLeft ) {
				elem.runtimeStyle.left = rsLeft;
			}
		}

		return ret === "" ? "auto" : ret;
	};
}

curCSS = getComputedStyle || currentStyle;

function getWidthOrHeight( elem, name, extra ) {

	// Start with offset property
	var val = name === "width" ? elem.offsetWidth : elem.offsetHeight,
		i = name === "width" ? 1 : 0,
		len = 4;

	if ( val > 0 ) {
		if ( extra !== "border" ) {
			for ( ; i < len; i += 2 ) {
				if ( !extra ) {
					val -= parseFloat( jQuery.css( elem, "padding" + cssExpand[ i ] ) ) || 0;
				}
				if ( extra === "margin" ) {
					val += parseFloat( jQuery.css( elem, extra + cssExpand[ i ] ) ) || 0;
				} else {
					val -= parseFloat( jQuery.css( elem, "border" + cssExpand[ i ] + "Width" ) ) || 0;
				}
			}
		}

		return val + "px";
	}

	// Fall back to computed then uncomputed css if necessary
	val = curCSS( elem, name );
	if ( val < 0 || val == null ) {
		val = elem.style[ name ];
	}

	// Computed unit is not pixels. Stop here and return.
	if ( rnumnonpx.test(val) ) {
		return val;
	}

	// Normalize "", auto, and prepare for extra
	val = parseFloat( val ) || 0;

	// Add padding, border, margin
	if ( extra ) {
		for ( ; i < len; i += 2 ) {
			val += parseFloat( jQuery.css( elem, "padding" + cssExpand[ i ] ) ) || 0;
			if ( extra !== "padding" ) {
				val += parseFloat( jQuery.css( elem, "border" + cssExpand[ i ] + "Width" ) ) || 0;
			}
			if ( extra === "margin" ) {
				val += parseFloat( jQuery.css( elem, extra + cssExpand[ i ]) ) || 0;
			}
		}
	}

	return val + "px";
}

jQuery.each([ "height", "width" ], function( i, name ) {
	jQuery.cssHooks[ name ] = {
		get: function( elem, computed, extra ) {
			if ( computed ) {
				if ( elem.offsetWidth !== 0 ) {
					return getWidthOrHeight( elem, name, extra );
				} else {
					return jQuery.swap( elem, cssShow, function() {
						return getWidthOrHeight( elem, name, extra );
					});
				}
			}
		},

		set: function( elem, value ) {
			return rnum.test( value ) ?
				value + "px" :
				value;
		}
	};
});

if ( !jQuery.support.opacity ) {
	jQuery.cssHooks.opacity = {
		get: function( elem, computed ) {
			// IE uses filters for opacity
			return ropacity.test( (computed && elem.currentStyle ? elem.currentStyle.filter : elem.style.filter) || "" ) ?
				( parseFloat( RegExp.$1 ) / 100 ) + "" :
				computed ? "1" : "";
		},

		set: function( elem, value ) {
			var style = elem.style,
				currentStyle = elem.currentStyle,
				opacity = jQuery.isNumeric( value ) ? "alpha(opacity=" + value * 100 + ")" : "",
				filter = currentStyle && currentStyle.filter || style.filter || "";

			// IE has trouble with opacity if it does not have layout
			// Force it by setting the zoom level
			style.zoom = 1;

			// if setting opacity to 1, and no other filters exist - attempt to remove filter attribute #6652
			if ( value >= 1 && jQuery.trim( filter.replace( ralpha, "" ) ) === "" ) {

				// Setting style.filter to null, "" & " " still leave "filter:" in the cssText
				// if "filter:" is present at all, clearType is disabled, we want to avoid this
				// style.removeAttribute is IE Only, but so apparently is this code path...
				style.removeAttribute( "filter" );

				// if there there is no filter style applied in a css rule, we are done
				if ( currentStyle && !currentStyle.filter ) {
					return;
				}
			}

			// otherwise, set new filter values
			style.filter = ralpha.test( filter ) ?
				filter.replace( ralpha, opacity ) :
				filter + " " + opacity;
		}
	};
}

jQuery(function() {
	// This hook cannot be added until DOM ready because the support test
	// for it is not run until after DOM ready
	if ( !jQuery.support.reliableMarginRight ) {
		jQuery.cssHooks.marginRight = {
			get: function( elem, computed ) {
				// WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
				// Work around by temporarily setting element display to inline-block
				return jQuery.swap( elem, { "display": "inline-block" }, function() {
					if ( computed ) {
						return curCSS( elem, "margin-right" );
					} else {
						return elem.style.marginRight;
					}
				});
			}
		};
	}
});

if ( jQuery.expr && jQuery.expr.filters ) {
	jQuery.expr.filters.hidden = function( elem ) {
		var width = elem.offsetWidth,
			height = elem.offsetHeight;

		return ( width === 0 && height === 0 ) || (!jQuery.support.reliableHiddenOffsets && ((elem.style && elem.style.display) || jQuery.css( elem, "display" )) === "none");
	};

	jQuery.expr.filters.visible = function( elem ) {
		return !jQuery.expr.filters.hidden( elem );
	};
}

// These hooks are used by animate to expand properties
jQuery.each({
	margin: "",
	padding: "",
	border: "Width"
}, function( prefix, suffix ) {

	jQuery.cssHooks[ prefix + suffix ] = {
		expand: function( value ) {
			var i,

				// assumes a single number if not a string
				parts = typeof value === "string" ? value.split(" ") : [ value ],
				expanded = {};

			for ( i = 0; i < 4; i++ ) {
				expanded[ prefix + cssExpand[ i ] + suffix ] =
					parts[ i ] || parts[ i - 2 ] || parts[ 0 ];
			}

			return expanded;
		}
	};
});




var r20 = /%20/g,
	rbracket = /\[\]$/,
	rCRLF = /\r?\n/g,
	rhash = /#.*$/,
	rheaders = /^(.*?):[ \t]*([^\r\n]*)\r?$/mg, // IE leaves an \r character at EOL
	rinput = /^(?:color|date|datetime|datetime-local|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i,
	// #7653, #8125, #8152: local protocol detection
	rlocalProtocol = /^(?:about|app|app\-storage|.+\-extension|file|res|widget):$/,
	rnoContent = /^(?:GET|HEAD)$/,
	rprotocol = /^\/\//,
	rquery = /\?/,
	rscript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi,
	rselectTextarea = /^(?:select|textarea)/i,
	rspacesAjax = /\s+/,
	rts = /([?&])_=[^&]*/,
	rurl = /^([\w\+\.\-]+:)(?:\/\/([^\/?#:]*)(?::(\d+))?)?/,

	// Keep a copy of the old load method
	_load = jQuery.fn.load,

	/* Prefilters
	 * 1) They are useful to introduce custom dataTypes (see ajax/jsonp.js for an example)
	 * 2) These are called:
	 *    - BEFORE asking for a transport
	 *    - AFTER param serialization (s.data is a string if s.processData is true)
	 * 3) key is the dataType
	 * 4) the catchall symbol "*" can be used
	 * 5) execution will start with transport dataType and THEN continue down to "*" if needed
	 */
	prefilters = {},

	/* Transports bindings
	 * 1) key is the dataType
	 * 2) the catchall symbol "*" can be used
	 * 3) selection will start with transport dataType and THEN go to "*" if needed
	 */
	transports = {},

	// Document location
	ajaxLocation,

	// Document location segments
	ajaxLocParts,

	// Avoid comment-prolog char sequence (#10098); must appease lint and evade compression
	allTypes = ["*/"] + ["*"];

// #8138, IE may throw an exception when accessing
// a field from window.location if document.domain has been set
try {
	ajaxLocation = location.href;
} catch( e ) {
	// Use the href attribute of an A element
	// since IE will modify it given document.location
	ajaxLocation = document.createElement( "a" );
	ajaxLocation.href = "";
	ajaxLocation = ajaxLocation.href;
}

// Segment location into parts
ajaxLocParts = rurl.exec( ajaxLocation.toLowerCase() ) || [];

// Base "constructor" for jQuery.ajaxPrefilter and jQuery.ajaxTransport
function addToPrefiltersOrTransports( structure ) {

	// dataTypeExpression is optional and defaults to "*"
	return function( dataTypeExpression, func ) {

		if ( typeof dataTypeExpression !== "string" ) {
			func = dataTypeExpression;
			dataTypeExpression = "*";
		}

		if ( jQuery.isFunction( func ) ) {
			var dataTypes = dataTypeExpression.toLowerCase().split( rspacesAjax ),
				i = 0,
				length = dataTypes.length,
				dataType,
				list,
				placeBefore;

			// For each dataType in the dataTypeExpression
			for ( ; i < length; i++ ) {
				dataType = dataTypes[ i ];
				// We control if we're asked to add before
				// any existing element
				placeBefore = /^\+/.test( dataType );
				if ( placeBefore ) {
					dataType = dataType.substr( 1 ) || "*";
				}
				list = structure[ dataType ] = structure[ dataType ] || [];
				// then we add to the structure accordingly
				list[ placeBefore ? "unshift" : "push" ]( func );
			}
		}
	};
}

// Base inspection function for prefilters and transports
function inspectPrefiltersOrTransports( structure, options, originalOptions, jqXHR,
		dataType /* internal */, inspected /* internal */ ) {

	dataType = dataType || options.dataTypes[ 0 ];
	inspected = inspected || {};

	inspected[ dataType ] = true;

	var list = structure[ dataType ],
		i = 0,
		length = list ? list.length : 0,
		executeOnly = ( structure === prefilters ),
		selection;

	for ( ; i < length && ( executeOnly || !selection ); i++ ) {
		selection = list[ i ]( options, originalOptions, jqXHR );
		// If we got redirected to another dataType
		// we try there if executing only and not done already
		if ( typeof selection === "string" ) {
			if ( !executeOnly || inspected[ selection ] ) {
				selection = undefined;
			} else {
				options.dataTypes.unshift( selection );
				selection = inspectPrefiltersOrTransports(
						structure, options, originalOptions, jqXHR, selection, inspected );
			}
		}
	}
	// If we're only executing or nothing was selected
	// we try the catchall dataType if not done already
	if ( ( executeOnly || !selection ) && !inspected[ "*" ] ) {
		selection = inspectPrefiltersOrTransports(
				structure, options, originalOptions, jqXHR, "*", inspected );
	}
	// unnecessary when only executing (prefilters)
	// but it'll be ignored by the caller in that case
	return selection;
}

// A special extend for ajax options
// that takes "flat" options (not to be deep extended)
// Fixes #9887
function ajaxExtend( target, src ) {
	var key, deep,
		flatOptions = jQuery.ajaxSettings.flatOptions || {};
	for ( key in src ) {
		if ( src[ key ] !== undefined ) {
			( flatOptions[ key ] ? target : ( deep || ( deep = {} ) ) )[ key ] = src[ key ];
		}
	}
	if ( deep ) {
		jQuery.extend( true, target, deep );
	}
}

jQuery.fn.extend({
	load: function( url, params, callback ) {
		if ( typeof url !== "string" && _load ) {
			return _load.apply( this, arguments );

		// Don't do a request if no elements are being requested
		} else if ( !this.length ) {
			return this;
		}

		var off = url.indexOf( " " );
		if ( off >= 0 ) {
			var selector = url.slice( off, url.length );
			url = url.slice( 0, off );
		}

		// Default to a GET request
		var type = "GET";

		// If the second parameter was provided
		if ( params ) {
			// If it's a function
			if ( jQuery.isFunction( params ) ) {
				// We assume that it's the callback
				callback = params;
				params = undefined;

			// Otherwise, build a param string
			} else if ( typeof params === "object" ) {
				params = jQuery.param( params, jQuery.ajaxSettings.traditional );
				type = "POST";
			}
		}

		var self = this;

		// Request the remote document
		jQuery.ajax({
			url: url,
			type: type,
			dataType: "html",
			data: params,
			// Complete callback (responseText is used internally)
			complete: function( jqXHR, status, responseText ) {
				// Store the response as specified by the jqXHR object
				responseText = jqXHR.responseText;
				// If successful, inject the HTML into all the matched elements
				if ( jqXHR.isResolved() ) {
					// #4825: Get the actual response in case
					// a dataFilter is present in ajaxSettings
					jqXHR.done(function( r ) {
						responseText = r;
					});
					// See if a selector was specified
					self.html( selector ?
						// Create a dummy div to hold the results
						jQuery("<div>")
							// inject the contents of the document in, removing the scripts
							// to avoid any 'Permission Denied' errors in IE
							.append(responseText.replace(rscript, ""))

							// Locate the specified elements
							.find(selector) :

						// If not, just inject the full result
						responseText );
				}

				if ( callback ) {
					self.each( callback, [ responseText, status, jqXHR ] );
				}
			}
		});

		return this;
	},

	serialize: function() {
		return jQuery.param( this.serializeArray() );
	},

	serializeArray: function() {
		return this.map(function(){
			return this.elements ? jQuery.makeArray( this.elements ) : this;
		})
		.filter(function(){
			return this.name && !this.disabled &&
				( this.checked || rselectTextarea.test( this.nodeName ) ||
					rinput.test( this.type ) );
		})
		.map(function( i, elem ){
			var val = jQuery( this ).val();

			return val == null ?
				null :
				jQuery.isArray( val ) ?
					jQuery.map( val, function( val, i ){
						return { name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
					}) :
					{ name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
		}).get();
	}
});

// Attach a bunch of functions for handling common AJAX events
jQuery.each( "ajaxStart ajaxStop ajaxComplete ajaxError ajaxSuccess ajaxSend".split( " " ), function( i, o ){
	jQuery.fn[ o ] = function( f ){
		return this.on( o, f );
	};
});

jQuery.each( [ "get", "post" ], function( i, method ) {
	jQuery[ method ] = function( url, data, callback, type ) {
		// shift arguments if data argument was omitted
		if ( jQuery.isFunction( data ) ) {
			type = type || callback;
			callback = data;
			data = undefined;
		}

		return jQuery.ajax({
			type: method,
			url: url,
			data: data,
			success: callback,
			dataType: type
		});
	};
});

jQuery.extend({

	getScript: function( url, callback ) {
		return jQuery.get( url, undefined, callback, "script" );
	},

	getJSON: function( url, data, callback ) {
		return jQuery.get( url, data, callback, "json" );
	},

	// Creates a full fledged settings object into target
	// with both ajaxSettings and settings fields.
	// If target is omitted, writes into ajaxSettings.
	ajaxSetup: function( target, settings ) {
		if ( settings ) {
			// Building a settings object
			ajaxExtend( target, jQuery.ajaxSettings );
		} else {
			// Extending ajaxSettings
			settings = target;
			target = jQuery.ajaxSettings;
		}
		ajaxExtend( target, settings );
		return target;
	},

	ajaxSettings: {
		url: ajaxLocation,
		isLocal: rlocalProtocol.test( ajaxLocParts[ 1 ] ),
		global: true,
		type: "GET",
		contentType: "application/x-www-form-urlencoded; charset=UTF-8",
		processData: true,
		async: true,
		/*
		timeout: 0,
		data: null,
		dataType: null,
		username: null,
		password: null,
		cache: null,
		traditional: false,
		headers: {},
		*/

		accepts: {
			xml: "application/xml, text/xml",
			html: "text/html",
			text: "text/plain",
			json: "application/json, text/javascript",
			"*": allTypes
		},

		contents: {
			xml: /xml/,
			html: /html/,
			json: /json/
		},

		responseFields: {
			xml: "responseXML",
			text: "responseText"
		},

		// List of data converters
		// 1) key format is "source_type destination_type" (a single space in-between)
		// 2) the catchall symbol "*" can be used for source_type
		converters: {

			// Convert anything to text
			"* text": window.String,

			// Text to html (true = no transformation)
			"text html": true,

			// Evaluate text as a json expression
			"text json": jQuery.parseJSON,

			// Parse text as xml
			"text xml": jQuery.parseXML
		},

		// For options that shouldn't be deep extended:
		// you can add your own custom options here if
		// and when you create one that shouldn't be
		// deep extended (see ajaxExtend)
		flatOptions: {
			context: true,
			url: true
		}
	},

	ajaxPrefilter: addToPrefiltersOrTransports( prefilters ),
	ajaxTransport: addToPrefiltersOrTransports( transports ),

	// Main method
	ajax: function( url, options ) {

		// If url is an object, simulate pre-1.5 signature
		if ( typeof url === "object" ) {
			options = url;
			url = undefined;
		}

		// Force options to be an object
		options = options || {};

		var // Create the final options object
			s = jQuery.ajaxSetup( {}, options ),
			// Callbacks context
			callbackContext = s.context || s,
			// Context for global events
			// It's the callbackContext if one was provided in the options
			// and if it's a DOM node or a jQuery collection
			globalEventContext = callbackContext !== s &&
				( callbackContext.nodeType || callbackContext instanceof jQuery ) ?
						jQuery( callbackContext ) : jQuery.event,
			// Deferreds
			deferred = jQuery.Deferred(),
			completeDeferred = jQuery.Callbacks( "once memory" ),
			// Status-dependent callbacks
			statusCode = s.statusCode || {},
			// ifModified key
			ifModifiedKey,
			// Headers (they are sent all at once)
			requestHeaders = {},
			requestHeadersNames = {},
			// Response headers
			responseHeadersString,
			responseHeaders,
			// transport
			transport,
			// timeout handle
			timeoutTimer,
			// Cross-domain detection vars
			parts,
			// The jqXHR state
			state = 0,
			// To know if global events are to be dispatched
			fireGlobals,
			// Loop variable
			i,
			// Fake xhr
			jqXHR = {

				readyState: 0,

				// Caches the header
				setRequestHeader: function( name, value ) {
					if ( !state ) {
						var lname = name.toLowerCase();
						name = requestHeadersNames[ lname ] = requestHeadersNames[ lname ] || name;
						requestHeaders[ name ] = value;
					}
					return this;
				},

				// Raw string
				getAllResponseHeaders: function() {
					return state === 2 ? responseHeadersString : null;
				},

				// Builds headers hashtable if needed
				getResponseHeader: function( key ) {
					var match;
					if ( state === 2 ) {
						if ( !responseHeaders ) {
							responseHeaders = {};
							while( ( match = rheaders.exec( responseHeadersString ) ) ) {
								responseHeaders[ match[1].toLowerCase() ] = match[ 2 ];
							}
						}
						match = responseHeaders[ key.toLowerCase() ];
					}
					return match === undefined ? null : match;
				},

				// Overrides response content-type header
				overrideMimeType: function( type ) {
					if ( !state ) {
						s.mimeType = type;
					}
					return this;
				},

				// Cancel the request
				abort: function( statusText ) {
					statusText = statusText || "abort";
					if ( transport ) {
						transport.abort( statusText );
					}
					done( 0, statusText );
					return this;
				}
			};

		// Callback for when everything is done
		// It is defined here because jslint complains if it is declared
		// at the end of the function (which would be more logical and readable)
		function done( status, nativeStatusText, responses, headers ) {

			// Called once
			if ( state === 2 ) {
				return;
			}

			// State is "done" now
			state = 2;

			// Clear timeout if it exists
			if ( timeoutTimer ) {
				clearTimeout( timeoutTimer );
			}

			// Dereference transport for early garbage collection
			// (no matter how long the jqXHR object will be used)
			transport = undefined;

			// Cache response headers
			responseHeadersString = headers || "";

			// Set readyState
			jqXHR.readyState = status > 0 ? 4 : 0;

			var isSuccess,
				success,
				error,
				statusText = nativeStatusText,
				response = responses ? ajaxHandleResponses( s, jqXHR, responses ) : undefined,
				lastModified,
				etag;

			// If successful, handle type chaining
			if ( status >= 200 && status < 300 || status === 304 ) {

				// Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
				if ( s.ifModified ) {

					if ( ( lastModified = jqXHR.getResponseHeader( "Last-Modified" ) ) ) {
						jQuery.lastModified[ ifModifiedKey ] = lastModified;
					}
					if ( ( etag = jqXHR.getResponseHeader( "Etag" ) ) ) {
						jQuery.etag[ ifModifiedKey ] = etag;
					}
				}

				// If not modified
				if ( status === 304 ) {

					statusText = "notmodified";
					isSuccess = true;

				// If we have data
				} else {

					try {
						success = ajaxConvert( s, response );
						statusText = "success";
						isSuccess = true;
					} catch(e) {
						// We have a parsererror
						statusText = "parsererror";
						error = e;
					}
				}
			} else {
				// We extract error from statusText
				// then normalize statusText and status for non-aborts
				error = statusText;
				if ( !statusText || status ) {
					statusText = "error";
					if ( status < 0 ) {
						status = 0;
					}
				}
			}

			// Set data for the fake xhr object
			jqXHR.status = status;
			jqXHR.statusText = "" + ( nativeStatusText || statusText );

			// Success/Error
			if ( isSuccess ) {
				deferred.resolveWith( callbackContext, [ success, statusText, jqXHR ] );
			} else {
				deferred.rejectWith( callbackContext, [ jqXHR, statusText, error ] );
			}

			// Status-dependent callbacks
			jqXHR.statusCode( statusCode );
			statusCode = undefined;

			if ( fireGlobals ) {
				globalEventContext.trigger( "ajax" + ( isSuccess ? "Success" : "Error" ),
						[ jqXHR, s, isSuccess ? success : error ] );
			}

			// Complete
			completeDeferred.fireWith( callbackContext, [ jqXHR, statusText ] );

			if ( fireGlobals ) {
				globalEventContext.trigger( "ajaxComplete", [ jqXHR, s ] );
				// Handle the global AJAX counter
				if ( !( --jQuery.active ) ) {
					jQuery.event.trigger( "ajaxStop" );
				}
			}
		}

		// Attach deferreds
		deferred.promise( jqXHR );
		jqXHR.success = jqXHR.done;
		jqXHR.error = jqXHR.fail;
		jqXHR.complete = completeDeferred.add;

		// Status-dependent callbacks
		jqXHR.statusCode = function( map ) {
			if ( map ) {
				var tmp;
				if ( state < 2 ) {
					for ( tmp in map ) {
						statusCode[ tmp ] = [ statusCode[tmp], map[tmp] ];
					}
				} else {
					tmp = map[ jqXHR.status ];
					jqXHR.then( tmp, tmp );
				}
			}
			return this;
		};

		// Remove hash character (#7531: and string promotion)
		// Add protocol if not provided (#5866: IE7 issue with protocol-less urls)
		// We also use the url parameter if available
		s.url = ( ( url || s.url ) + "" ).replace( rhash, "" ).replace( rprotocol, ajaxLocParts[ 1 ] + "//" );

		// Extract dataTypes list
		s.dataTypes = jQuery.trim( s.dataType || "*" ).toLowerCase().split( rspacesAjax );

		// Determine if a cross-domain request is in order
		if ( s.crossDomain == null ) {
			parts = rurl.exec( s.url.toLowerCase() );
			s.crossDomain = !!( parts &&
				( parts[ 1 ] != ajaxLocParts[ 1 ] || parts[ 2 ] != ajaxLocParts[ 2 ] ||
					( parts[ 3 ] || ( parts[ 1 ] === "http:" ? 80 : 443 ) ) !=
						( ajaxLocParts[ 3 ] || ( ajaxLocParts[ 1 ] === "http:" ? 80 : 443 ) ) )
			);
		}

		// Convert data if not already a string
		if ( s.data && s.processData && typeof s.data !== "string" ) {
			s.data = jQuery.param( s.data, s.traditional );
		}

		// Apply prefilters
		inspectPrefiltersOrTransports( prefilters, s, options, jqXHR );

		// If request was aborted inside a prefilter, stop there
		if ( state === 2 ) {
			return false;
		}

		// We can fire global events as of now if asked to
		fireGlobals = s.global;

		// Uppercase the type
		s.type = s.type.toUpperCase();

		// Determine if request has content
		s.hasContent = !rnoContent.test( s.type );

		// Watch for a new set of requests
		if ( fireGlobals && jQuery.active++ === 0 ) {
			jQuery.event.trigger( "ajaxStart" );
		}

		// More options handling for requests with no content
		if ( !s.hasContent ) {

			// If data is available, append data to url
			if ( s.data ) {
				s.url += ( rquery.test( s.url ) ? "&" : "?" ) + s.data;
				// #9682: remove data so that it's not used in an eventual retry
				delete s.data;
			}

			// Get ifModifiedKey before adding the anti-cache parameter
			ifModifiedKey = s.url;

			// Add anti-cache in url if needed
			if ( s.cache === false ) {

				var ts = jQuery.now(),
					// try replacing _= if it is there
					ret = s.url.replace( rts, "$1_=" + ts );

				// if nothing was replaced, add timestamp to the end
				s.url = ret + ( ( ret === s.url ) ? ( rquery.test( s.url ) ? "&" : "?" ) + "_=" + ts : "" );
			}
		}

		// Set the correct header, if data is being sent
		if ( s.data && s.hasContent && s.contentType !== false || options.contentType ) {
			jqXHR.setRequestHeader( "Content-Type", s.contentType );
		}

		// Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
		if ( s.ifModified ) {
			ifModifiedKey = ifModifiedKey || s.url;
			if ( jQuery.lastModified[ ifModifiedKey ] ) {
				jqXHR.setRequestHeader( "If-Modified-Since", jQuery.lastModified[ ifModifiedKey ] );
			}
			if ( jQuery.etag[ ifModifiedKey ] ) {
				jqXHR.setRequestHeader( "If-None-Match", jQuery.etag[ ifModifiedKey ] );
			}
		}

		// Set the Accepts header for the server, depending on the dataType
		jqXHR.setRequestHeader(
			"Accept",
			s.dataTypes[ 0 ] && s.accepts[ s.dataTypes[0] ] ?
				s.accepts[ s.dataTypes[0] ] + ( s.dataTypes[ 0 ] !== "*" ? ", " + allTypes + "; q=0.01" : "" ) :
				s.accepts[ "*" ]
		);

		// Check for headers option
		for ( i in s.headers ) {
			jqXHR.setRequestHeader( i, s.headers[ i ] );
		}

		// Allow custom headers/mimetypes and early abort
		if ( s.beforeSend && ( s.beforeSend.call( callbackContext, jqXHR, s ) === false || state === 2 ) ) {
				// Abort if not done already
				jqXHR.abort();
				return false;

		}

		// Install callbacks on deferreds
		for ( i in { success: 1, error: 1, complete: 1 } ) {
			jqXHR[ i ]( s[ i ] );
		}

		// Get transport
		transport = inspectPrefiltersOrTransports( transports, s, options, jqXHR );

		// If no transport, we auto-abort
		if ( !transport ) {
			done( -1, "No Transport" );
		} else {
			jqXHR.readyState = 1;
			// Send global event
			if ( fireGlobals ) {
				globalEventContext.trigger( "ajaxSend", [ jqXHR, s ] );
			}
			// Timeout
			if ( s.async && s.timeout > 0 ) {
				timeoutTimer = setTimeout( function(){
					jqXHR.abort( "timeout" );
				}, s.timeout );
			}

			try {
				state = 1;
				transport.send( requestHeaders, done );
			} catch (e) {
				// Propagate exception as error if not done
				if ( state < 2 ) {
					done( -1, e );
				// Simply rethrow otherwise
				} else {
					throw e;
				}
			}
		}

		return jqXHR;
	},

	// Serialize an array of form elements or a set of
	// key/values into a query string
	param: function( a, traditional ) {
		var s = [],
			add = function( key, value ) {
				// If value is a function, invoke it and return its value
				value = jQuery.isFunction( value ) ? value() : value;
				s[ s.length ] = encodeURIComponent( key ) + "=" + encodeURIComponent( value );
			};

		// Set traditional to true for jQuery <= 1.3.2 behavior.
		if ( traditional === undefined ) {
			traditional = jQuery.ajaxSettings.traditional;
		}

		// If an array was passed in, assume that it is an array of form elements.
		if ( jQuery.isArray( a ) || ( a.jquery && !jQuery.isPlainObject( a ) ) ) {
			// Serialize the form elements
			jQuery.each( a, function() {
				add( this.name, this.value );
			});

		} else {
			// If traditional, encode the "old" way (the way 1.3.2 or older
			// did it), otherwise encode params recursively.
			for ( var prefix in a ) {
				buildParams( prefix, a[ prefix ], traditional, add );
			}
		}

		// Return the resulting serialization
		return s.join( "&" ).replace( r20, "+" );
	}
});

function buildParams( prefix, obj, traditional, add ) {
	if ( jQuery.isArray( obj ) ) {
		// Serialize array item.
		jQuery.each( obj, function( i, v ) {
			if ( traditional || rbracket.test( prefix ) ) {
				// Treat each array item as a scalar.
				add( prefix, v );

			} else {
				// If array item is non-scalar (array or object), encode its
				// numeric index to resolve deserialization ambiguity issues.
				// Note that rack (as of 1.0.0) can't currently deserialize
				// nested arrays properly, and attempting to do so may cause
				// a server error. Possible fixes are to modify rack's
				// deserialization algorithm or to provide an option or flag
				// to force array serialization to be shallow.
				buildParams( prefix + "[" + ( typeof v === "object" ? i : "" ) + "]", v, traditional, add );
			}
		});

	} else if ( !traditional && jQuery.type( obj ) === "object" ) {
		// Serialize object item.
		for ( var name in obj ) {
			buildParams( prefix + "[" + name + "]", obj[ name ], traditional, add );
		}

	} else {
		// Serialize scalar item.
		add( prefix, obj );
	}
}

// This is still on the jQuery object... for now
// Want to move this to jQuery.ajax some day
jQuery.extend({

	// Counter for holding the number of active queries
	active: 0,

	// Last-Modified header cache for next request
	lastModified: {},
	etag: {}

});

/* Handles responses to an ajax request:
 * - sets all responseXXX fields accordingly
 * - finds the right dataType (mediates between content-type and expected dataType)
 * - returns the corresponding response
 */
function ajaxHandleResponses( s, jqXHR, responses ) {

	var contents = s.contents,
		dataTypes = s.dataTypes,
		responseFields = s.responseFields,
		ct,
		type,
		finalDataType,
		firstDataType;

	// Fill responseXXX fields
	for ( type in responseFields ) {
		if ( type in responses ) {
			jqXHR[ responseFields[type] ] = responses[ type ];
		}
	}

	// Remove auto dataType and get content-type in the process
	while( dataTypes[ 0 ] === "*" ) {
		dataTypes.shift();
		if ( ct === undefined ) {
			ct = s.mimeType || jqXHR.getResponseHeader( "content-type" );
		}
	}

	// Check if we're dealing with a known content-type
	if ( ct ) {
		for ( type in contents ) {
			if ( contents[ type ] && contents[ type ].test( ct ) ) {
				dataTypes.unshift( type );
				break;
			}
		}
	}

	// Check to see if we have a response for the expected dataType
	if ( dataTypes[ 0 ] in responses ) {
		finalDataType = dataTypes[ 0 ];
	} else {
		// Try convertible dataTypes
		for ( type in responses ) {
			if ( !dataTypes[ 0 ] || s.converters[ type + " " + dataTypes[0] ] ) {
				finalDataType = type;
				break;
			}
			if ( !firstDataType ) {
				firstDataType = type;
			}
		}
		// Or just use first one
		finalDataType = finalDataType || firstDataType;
	}

	// If we found a dataType
	// We add the dataType to the list if needed
	// and return the corresponding response
	if ( finalDataType ) {
		if ( finalDataType !== dataTypes[ 0 ] ) {
			dataTypes.unshift( finalDataType );
		}
		return responses[ finalDataType ];
	}
}

// Chain conversions given the request and the original response
function ajaxConvert( s, response ) {

	// Apply the dataFilter if provided
	if ( s.dataFilter ) {
		response = s.dataFilter( response, s.dataType );
	}

	var dataTypes = s.dataTypes,
		converters = {},
		i,
		key,
		length = dataTypes.length,
		tmp,
		// Current and previous dataTypes
		current = dataTypes[ 0 ],
		prev,
		// Conversion expression
		conversion,
		// Conversion function
		conv,
		// Conversion functions (transitive conversion)
		conv1,
		conv2;

	// For each dataType in the chain
	for ( i = 1; i < length; i++ ) {

		// Create converters map
		// with lowercased keys
		if ( i === 1 ) {
			for ( key in s.converters ) {
				if ( typeof key === "string" ) {
					converters[ key.toLowerCase() ] = s.converters[ key ];
				}
			}
		}

		// Get the dataTypes
		prev = current;
		current = dataTypes[ i ];

		// If current is auto dataType, update it to prev
		if ( current === "*" ) {
			current = prev;
		// If no auto and dataTypes are actually different
		} else if ( prev !== "*" && prev !== current ) {

			// Get the converter
			conversion = prev + " " + current;
			conv = converters[ conversion ] || converters[ "* " + current ];

			// If there is no direct converter, search transitively
			if ( !conv ) {
				conv2 = undefined;
				for ( conv1 in converters ) {
					tmp = conv1.split( " " );
					if ( tmp[ 0 ] === prev || tmp[ 0 ] === "*" ) {
						conv2 = converters[ tmp[1] + " " + current ];
						if ( conv2 ) {
							conv1 = converters[ conv1 ];
							if ( conv1 === true ) {
								conv = conv2;
							} else if ( conv2 === true ) {
								conv = conv1;
							}
							break;
						}
					}
				}
			}
			// If we found no converter, dispatch an error
			if ( !( conv || conv2 ) ) {
				jQuery.error( "No conversion from " + conversion.replace(" "," to ") );
			}
			// If found converter is not an equivalence
			if ( conv !== true ) {
				// Convert with 1 or 2 converters accordingly
				response = conv ? conv( response ) : conv2( conv1(response) );
			}
		}
	}
	return response;
}




var jsc = jQuery.now(),
	jsre = /(\=)\?(&|$)|\?\?/i;

// Default jsonp settings
jQuery.ajaxSetup({
	jsonp: "callback",
	jsonpCallback: function() {
		return jQuery.expando + "_" + ( jsc++ );
	}
});

// Detect, normalize options and install callbacks for jsonp requests
jQuery.ajaxPrefilter( "json jsonp", function( s, originalSettings, jqXHR ) {

	var inspectData = ( typeof s.data === "string" ) && /^application\/x\-www\-form\-urlencoded/.test( s.contentType );

	if ( s.dataTypes[ 0 ] === "jsonp" ||
		s.jsonp !== false && ( jsre.test( s.url ) ||
				inspectData && jsre.test( s.data ) ) ) {

		var responseContainer,
			jsonpCallback = s.jsonpCallback =
				jQuery.isFunction( s.jsonpCallback ) ? s.jsonpCallback() : s.jsonpCallback,
			previous = window[ jsonpCallback ],
			url = s.url,
			data = s.data,
			replace = "$1" + jsonpCallback + "$2";

		if ( s.jsonp !== false ) {
			url = url.replace( jsre, replace );
			if ( s.url === url ) {
				if ( inspectData ) {
					data = data.replace( jsre, replace );
				}
				if ( s.data === data ) {
					// Add callback manually
					url += (/\?/.test( url ) ? "&" : "?") + s.jsonp + "=" + jsonpCallback;
				}
			}
		}

		s.url = url;
		s.data = data;

		// Install callback
		window[ jsonpCallback ] = function( response ) {
			responseContainer = [ response ];
		};

		// Clean-up function
		jqXHR.always(function() {
			// Set callback back to previous value
			window[ jsonpCallback ] = previous;
			// Call if it was a function and we have a response
			if ( responseContainer && jQuery.isFunction( previous ) ) {
				window[ jsonpCallback ]( responseContainer[ 0 ] );
			}
		});

		// Use data converter to retrieve json after script execution
		s.converters["script json"] = function() {
			if ( !responseContainer ) {
				jQuery.error( jsonpCallback + " was not called" );
			}
			return responseContainer[ 0 ];
		};

		// force json dataType
		s.dataTypes[ 0 ] = "json";

		// Delegate to script
		return "script";
	}
});




// Install script dataType
jQuery.ajaxSetup({
	accepts: {
		script: "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"
	},
	contents: {
		script: /javascript|ecmascript/
	},
	converters: {
		"text script": function( text ) {
			jQuery.globalEval( text );
			return text;
		}
	}
});

// Handle cache's special case and global
jQuery.ajaxPrefilter( "script", function( s ) {
	if ( s.cache === undefined ) {
		s.cache = false;
	}
	if ( s.crossDomain ) {
		s.type = "GET";
		s.global = false;
	}
});

// Bind script tag hack transport
jQuery.ajaxTransport( "script", function(s) {

	// This transport only deals with cross domain requests
	if ( s.crossDomain ) {

		var script,
			head = document.head || document.getElementsByTagName( "head" )[0] || document.documentElement;

		return {

			send: function( _, callback ) {

				script = document.createElement( "script" );

				script.async = "async";

				if ( s.scriptCharset ) {
					script.charset = s.scriptCharset;
				}

				script.src = s.url;

				// Attach handlers for all browsers
				script.onload = script.onreadystatechange = function( _, isAbort ) {

					if ( isAbort || !script.readyState || /loaded|complete/.test( script.readyState ) ) {

						// Handle memory leak in IE
						script.onload = script.onreadystatechange = null;

						// Remove the script
						if ( head && script.parentNode ) {
							head.removeChild( script );
						}

						// Dereference the script
						script = undefined;

						// Callback if not abort
						if ( !isAbort ) {
							callback( 200, "success" );
						}
					}
				};
				// Use insertBefore instead of appendChild  to circumvent an IE6 bug.
				// This arises when a base node is used (#2709 and #4378).
				head.insertBefore( script, head.firstChild );
			},

			abort: function() {
				if ( script ) {
					script.onload( 0, 1 );
				}
			}
		};
	}
});




var // #5280: Internet Explorer will keep connections alive if we don't abort on unload
	xhrOnUnloadAbort = window.ActiveXObject ? function() {
		// Abort all pending requests
		for ( var key in xhrCallbacks ) {
			xhrCallbacks[ key ]( 0, 1 );
		}
	} : false,
	xhrId = 0,
	xhrCallbacks;

// Functions to create xhrs
function createStandardXHR() {
	try {
		return new window.XMLHttpRequest();
	} catch( e ) {}
}

function createActiveXHR() {
	try {
		return new window.ActiveXObject( "Microsoft.XMLHTTP" );
	} catch( e ) {}
}

// Create the request object
// (This is still attached to ajaxSettings for backward compatibility)
jQuery.ajaxSettings.xhr = window.ActiveXObject ?
	/* Microsoft failed to properly
	 * implement the XMLHttpRequest in IE7 (can't request local files),
	 * so we use the ActiveXObject when it is available
	 * Additionally XMLHttpRequest can be disabled in IE7/IE8 so
	 * we need a fallback.
	 */
	function() {
		return !this.isLocal && createStandardXHR() || createActiveXHR();
	} :
	// For all other browsers, use the standard XMLHttpRequest object
	createStandardXHR;

// Determine support properties
(function( xhr ) {
	jQuery.extend( jQuery.support, {
		ajax: !!xhr,
		cors: !!xhr && ( "withCredentials" in xhr )
	});
})( jQuery.ajaxSettings.xhr() );

// Create transport if the browser can provide an xhr
if ( jQuery.support.ajax ) {

	jQuery.ajaxTransport(function( s ) {
		// Cross domain only allowed if supported through XMLHttpRequest
		if ( !s.crossDomain || jQuery.support.cors ) {

			var callback;

			return {
				send: function( headers, complete ) {

					// Get a new xhr
					var xhr = s.xhr(),
						handle,
						i;

					// Open the socket
					// Passing null username, generates a login popup on Opera (#2865)
					if ( s.username ) {
						xhr.open( s.type, s.url, s.async, s.username, s.password );
					} else {
						xhr.open( s.type, s.url, s.async );
					}

					// Apply custom fields if provided
					if ( s.xhrFields ) {
						for ( i in s.xhrFields ) {
							xhr[ i ] = s.xhrFields[ i ];
						}
					}

					// Override mime type if needed
					if ( s.mimeType && xhr.overrideMimeType ) {
						xhr.overrideMimeType( s.mimeType );
					}

					// X-Requested-With header
					// For cross-domain requests, seeing as conditions for a preflight are
					// akin to a jigsaw puzzle, we simply never set it to be sure.
					// (it can always be set on a per-request basis or even using ajaxSetup)
					// For same-domain requests, won't change header if already provided.
					if ( !s.crossDomain && !headers["X-Requested-With"] ) {
						headers[ "X-Requested-With" ] = "XMLHttpRequest";
					}

					// Need an extra try/catch for cross domain requests in Firefox 3
					try {
						for ( i in headers ) {
							xhr.setRequestHeader( i, headers[ i ] );
						}
					} catch( _ ) {}

					// Do send the request
					// This may raise an exception which is actually
					// handled in jQuery.ajax (so no try/catch here)
					xhr.send( ( s.hasContent && s.data ) || null );

					// Listener
					callback = function( _, isAbort ) {

						var status,
							statusText,
							responseHeaders,
							responses,
							xml;

						// Firefox throws exceptions when accessing properties
						// of an xhr when a network error occured
						// http://helpful.knobs-dials.com/index.php/Component_returned_failure_code:_0x80040111_(NS_ERROR_NOT_AVAILABLE)
						try {

							// Was never called and is aborted or complete
							if ( callback && ( isAbort || xhr.readyState === 4 ) ) {

								// Only called once
								callback = undefined;

								// Do not keep as active anymore
								if ( handle ) {
									xhr.onreadystatechange = jQuery.noop;
									if ( xhrOnUnloadAbort ) {
										delete xhrCallbacks[ handle ];
									}
								}

								// If it's an abort
								if ( isAbort ) {
									// Abort it manually if needed
									if ( xhr.readyState !== 4 ) {
										xhr.abort();
									}
								} else {
									status = xhr.status;
									responseHeaders = xhr.getAllResponseHeaders();
									responses = {};
									xml = xhr.responseXML;

									// Construct response list
									if ( xml && xml.documentElement /* #4958 */ ) {
										responses.xml = xml;
									}

									// When requesting binary data, IE6-9 will throw an exception
									// on any attempt to access responseText (#11426)
									try {
										responses.text = xhr.responseText;
									} catch( _ ) {
									}

									// Firefox throws an exception when accessing
									// statusText for faulty cross-domain requests
									try {
										statusText = xhr.statusText;
									} catch( e ) {
										// We normalize with Webkit giving an empty statusText
										statusText = "";
									}

									// Filter status for non standard behaviors

									// If the request is local and we have data: assume a success
									// (success with no data won't get notified, that's the best we
									// can do given current implementations)
									if ( !status && s.isLocal && !s.crossDomain ) {
										status = responses.text ? 200 : 404;
									// IE - #1450: sometimes returns 1223 when it should be 204
									} else if ( status === 1223 ) {
										status = 204;
									}
								}
							}
						} catch( firefoxAccessException ) {
							if ( !isAbort ) {
								complete( -1, firefoxAccessException );
							}
						}

						// Call complete if needed
						if ( responses ) {
							complete( status, statusText, responses, responseHeaders );
						}
					};

					// if we're in sync mode or it's in cache
					// and has been retrieved directly (IE6 & IE7)
					// we need to manually fire the callback
					if ( !s.async || xhr.readyState === 4 ) {
						callback();
					} else {
						handle = ++xhrId;
						if ( xhrOnUnloadAbort ) {
							// Create the active xhrs callbacks list if needed
							// and attach the unload handler
							if ( !xhrCallbacks ) {
								xhrCallbacks = {};
								jQuery( window ).unload( xhrOnUnloadAbort );
							}
							// Add to list of active xhrs callbacks
							xhrCallbacks[ handle ] = callback;
						}
						xhr.onreadystatechange = callback;
					}
				},

				abort: function() {
					if ( callback ) {
						callback(0,1);
					}
				}
			};
		}
	});
}




var elemdisplay = {},
	iframe, iframeDoc,
	rfxtypes = /^(?:toggle|show|hide)$/,
	rfxnum = /^([+\-]=)?([\d+.\-]+)([a-z%]*)$/i,
	timerId,
	fxAttrs = [
		// height animations
		[ "height", "marginTop", "marginBottom", "paddingTop", "paddingBottom" ],
		// width animations
		[ "width", "marginLeft", "marginRight", "paddingLeft", "paddingRight" ],
		// opacity animations
		[ "opacity" ]
	],
	fxNow;

jQuery.fn.extend({
	show: function( speed, easing, callback ) {
		var elem, display;

		if ( speed || speed === 0 ) {
			return this.animate( genFx("show", 3), speed, easing, callback );

		} else {
			for ( var i = 0, j = this.length; i < j; i++ ) {
				elem = this[ i ];

				if ( elem.style ) {
					display = elem.style.display;

					// Reset the inline display of this element to learn if it is
					// being hidden by cascaded rules or not
					if ( !jQuery._data(elem, "olddisplay") && display === "none" ) {
						display = elem.style.display = "";
					}

					// Set elements which have been overridden with display: none
					// in a stylesheet to whatever the default browser style is
					// for such an element
					if ( (display === "" && jQuery.css(elem, "display") === "none") ||
						!jQuery.contains( elem.ownerDocument.documentElement, elem ) ) {
						jQuery._data( elem, "olddisplay", defaultDisplay(elem.nodeName) );
					}
				}
			}

			// Set the display of most of the elements in a second loop
			// to avoid the constant reflow
			for ( i = 0; i < j; i++ ) {
				elem = this[ i ];

				if ( elem.style ) {
					display = elem.style.display;

					if ( display === "" || display === "none" ) {
						elem.style.display = jQuery._data( elem, "olddisplay" ) || "";
					}
				}
			}

			return this;
		}
	},

	hide: function( speed, easing, callback ) {
		if ( speed || speed === 0 ) {
			return this.animate( genFx("hide", 3), speed, easing, callback);

		} else {
			var elem, display,
				i = 0,
				j = this.length;

			for ( ; i < j; i++ ) {
				elem = this[i];
				if ( elem.style ) {
					display = jQuery.css( elem, "display" );

					if ( display !== "none" && !jQuery._data( elem, "olddisplay" ) ) {
						jQuery._data( elem, "olddisplay", display );
					}
				}
			}

			// Set the display of the elements in a second loop
			// to avoid the constant reflow
			for ( i = 0; i < j; i++ ) {
				if ( this[i].style ) {
					this[i].style.display = "none";
				}
			}

			return this;
		}
	},

	// Save the old toggle function
	_toggle: jQuery.fn.toggle,

	toggle: function( fn, fn2, callback ) {
		var bool = typeof fn === "boolean";

		if ( jQuery.isFunction(fn) && jQuery.isFunction(fn2) ) {
			this._toggle.apply( this, arguments );

		} else if ( fn == null || bool ) {
			this.each(function() {
				var state = bool ? fn : jQuery(this).is(":hidden");
				jQuery(this)[ state ? "show" : "hide" ]();
			});

		} else {
			this.animate(genFx("toggle", 3), fn, fn2, callback);
		}

		return this;
	},

	fadeTo: function( speed, to, easing, callback ) {
		return this.filter(":hidden").css("opacity", 0).show().end()
					.animate({opacity: to}, speed, easing, callback);
	},

	animate: function( prop, speed, easing, callback ) {
		var optall = jQuery.speed( speed, easing, callback );

		if ( jQuery.isEmptyObject( prop ) ) {
			return this.each( optall.complete, [ false ] );
		}

		// Do not change referenced properties as per-property easing will be lost
		prop = jQuery.extend( {}, prop );

		function doAnimation() {
			// XXX 'this' does not always have a nodeName when running the
			// test suite

			if ( optall.queue === false ) {
				jQuery._mark( this );
			}

			var opt = jQuery.extend( {}, optall ),
				isElement = this.nodeType === 1,
				hidden = isElement && jQuery(this).is(":hidden"),
				name, val, p, e, hooks, replace,
				parts, start, end, unit,
				method;

			// will store per property easing and be used to determine when an animation is complete
			opt.animatedProperties = {};

			// first pass over propertys to expand / normalize
			for ( p in prop ) {
				name = jQuery.camelCase( p );
				if ( p !== name ) {
					prop[ name ] = prop[ p ];
					delete prop[ p ];
				}

				if ( ( hooks = jQuery.cssHooks[ name ] ) && "expand" in hooks ) {
					replace = hooks.expand( prop[ name ] );
					delete prop[ name ];

					// not quite $.extend, this wont overwrite keys already present.
					// also - reusing 'p' from above because we have the correct "name"
					for ( p in replace ) {
						if ( ! ( p in prop ) ) {
							prop[ p ] = replace[ p ];
						}
					}
				}
			}

			for ( name in prop ) {
				val = prop[ name ];
				// easing resolution: per property > opt.specialEasing > opt.easing > 'swing' (default)
				if ( jQuery.isArray( val ) ) {
					opt.animatedProperties[ name ] = val[ 1 ];
					val = prop[ name ] = val[ 0 ];
				} else {
					opt.animatedProperties[ name ] = opt.specialEasing && opt.specialEasing[ name ] || opt.easing || 'swing';
				}

				if ( val === "hide" && hidden || val === "show" && !hidden ) {
					return opt.complete.call( this );
				}

				if ( isElement && ( name === "height" || name === "width" ) ) {
					// Make sure that nothing sneaks out
					// Record all 3 overflow attributes because IE does not
					// change the overflow attribute when overflowX and
					// overflowY are set to the same value
					opt.overflow = [ this.style.overflow, this.style.overflowX, this.style.overflowY ];

					// Set display property to inline-block for height/width
					// animations on inline elements that are having width/height animated
					if ( jQuery.css( this, "display" ) === "inline" &&
							jQuery.css( this, "float" ) === "none" ) {

						// inline-level elements accept inline-block;
						// block-level elements need to be inline with layout
						if ( !jQuery.support.inlineBlockNeedsLayout || defaultDisplay( this.nodeName ) === "inline" ) {
							this.style.display = "inline-block";

						} else {
							this.style.zoom = 1;
						}
					}
				}
			}

			if ( opt.overflow != null ) {
				this.style.overflow = "hidden";
			}

			for ( p in prop ) {
				e = new jQuery.fx( this, opt, p );
				val = prop[ p ];

				if ( rfxtypes.test( val ) ) {

					// Tracks whether to show or hide based on private
					// data attached to the element
					method = jQuery._data( this, "toggle" + p ) || ( val === "toggle" ? hidden ? "show" : "hide" : 0 );
					if ( method ) {
						jQuery._data( this, "toggle" + p, method === "show" ? "hide" : "show" );
						e[ method ]();
					} else {
						e[ val ]();
					}

				} else {
					parts = rfxnum.exec( val );
					start = e.cur();

					if ( parts ) {
						end = parseFloat( parts[2] );
						unit = parts[3] || ( jQuery.cssNumber[ p ] ? "" : "px" );

						// We need to compute starting value
						if ( unit !== "px" ) {
							jQuery.style( this, p, (end || 1) + unit);
							start = ( (end || 1) / e.cur() ) * start;
							jQuery.style( this, p, start + unit);
						}

						// If a +=/-= token was provided, we're doing a relative animation
						if ( parts[1] ) {
							end = ( (parts[ 1 ] === "-=" ? -1 : 1) * end ) + start;
						}

						e.custom( start, end, unit );

					} else {
						e.custom( start, val, "" );
					}
				}
			}

			// For JS strict compliance
			return true;
		}

		return optall.queue === false ?
			this.each( doAnimation ) :
			this.queue( optall.queue, doAnimation );
	},

	stop: function( type, clearQueue, gotoEnd ) {
		if ( typeof type !== "string" ) {
			gotoEnd = clearQueue;
			clearQueue = type;
			type = undefined;
		}
		if ( clearQueue && type !== false ) {
			this.queue( type || "fx", [] );
		}

		return this.each(function() {
			var index,
				hadTimers = false,
				timers = jQuery.timers,
				data = jQuery._data( this );

			// clear marker counters if we know they won't be
			if ( !gotoEnd ) {
				jQuery._unmark( true, this );
			}

			function stopQueue( elem, data, index ) {
				var hooks = data[ index ];
				jQuery.removeData( elem, index, true );
				hooks.stop( gotoEnd );
			}

			if ( type == null ) {
				for ( index in data ) {
					if ( data[ index ] && data[ index ].stop && index.indexOf(".run") === index.length - 4 ) {
						stopQueue( this, data, index );
					}
				}
			} else if ( data[ index = type + ".run" ] && data[ index ].stop ){
				stopQueue( this, data, index );
			}

			for ( index = timers.length; index--; ) {
				if ( timers[ index ].elem === this && (type == null || timers[ index ].queue === type) ) {
					if ( gotoEnd ) {

						// force the next step to be the last
						timers[ index ]( true );
					} else {
						timers[ index ].saveState();
					}
					hadTimers = true;
					timers.splice( index, 1 );
				}
			}

			// start the next in the queue if the last step wasn't forced
			// timers currently will call their complete callbacks, which will dequeue
			// but only if they were gotoEnd
			if ( !( gotoEnd && hadTimers ) ) {
				jQuery.dequeue( this, type );
			}
		});
	}

});

// Animations created synchronously will run synchronously
function createFxNow() {
	setTimeout( clearFxNow, 0 );
	return ( fxNow = jQuery.now() );
}

function clearFxNow() {
	fxNow = undefined;
}

// Generate parameters to create a standard animation
function genFx( type, num ) {
	var obj = {};

	jQuery.each( fxAttrs.concat.apply([], fxAttrs.slice( 0, num )), function() {
		obj[ this ] = type;
	});

	return obj;
}

// Generate shortcuts for custom animations
jQuery.each({
	slideDown: genFx( "show", 1 ),
	slideUp: genFx( "hide", 1 ),
	slideToggle: genFx( "toggle", 1 ),
	fadeIn: { opacity: "show" },
	fadeOut: { opacity: "hide" },
	fadeToggle: { opacity: "toggle" }
}, function( name, props ) {
	jQuery.fn[ name ] = function( speed, easing, callback ) {
		return this.animate( props, speed, easing, callback );
	};
});

jQuery.extend({
	speed: function( speed, easing, fn ) {
		var opt = speed && typeof speed === "object" ? jQuery.extend( {}, speed ) : {
			complete: fn || !fn && easing ||
				jQuery.isFunction( speed ) && speed,
			duration: speed,
			easing: fn && easing || easing && !jQuery.isFunction( easing ) && easing
		};

		opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
			opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[ opt.duration ] : jQuery.fx.speeds._default;

		// normalize opt.queue - true/undefined/null -> "fx"
		if ( opt.queue == null || opt.queue === true ) {
			opt.queue = "fx";
		}

		// Queueing
		opt.old = opt.complete;

		opt.complete = function( noUnmark ) {
			if ( jQuery.isFunction( opt.old ) ) {
				opt.old.call( this );
			}

			if ( opt.queue ) {
				jQuery.dequeue( this, opt.queue );
			} else if ( noUnmark !== false ) {
				jQuery._unmark( this );
			}
		};

		return opt;
	},

	easing: {
		linear: function( p ) {
			return p;
		},
		swing: function( p ) {
			return ( -Math.cos( p*Math.PI ) / 2 ) + 0.5;
		}
	},

	timers: [],

	fx: function( elem, options, prop ) {
		this.options = options;
		this.elem = elem;
		this.prop = prop;

		options.orig = options.orig || {};
	}

});

jQuery.fx.prototype = {
	// Simple function for setting a style value
	update: function() {
		if ( this.options.step ) {
			this.options.step.call( this.elem, this.now, this );
		}

		( jQuery.fx.step[ this.prop ] || jQuery.fx.step._default )( this );
	},

	// Get the current size
	cur: function() {
		if ( this.elem[ this.prop ] != null && (!this.elem.style || this.elem.style[ this.prop ] == null) ) {
			return this.elem[ this.prop ];
		}

		var parsed,
			r = jQuery.css( this.elem, this.prop );
		// Empty strings, null, undefined and "auto" are converted to 0,
		// complex values such as "rotate(1rad)" are returned as is,
		// simple values such as "10px" are parsed to Float.
		return isNaN( parsed = parseFloat( r ) ) ? !r || r === "auto" ? 0 : r : parsed;
	},

	// Start an animation from one number to another
	custom: function( from, to, unit ) {
		var self = this,
			fx = jQuery.fx;

		this.startTime = fxNow || createFxNow();
		this.end = to;
		this.now = this.start = from;
		this.pos = this.state = 0;
		this.unit = unit || this.unit || ( jQuery.cssNumber[ this.prop ] ? "" : "px" );

		function t( gotoEnd ) {
			return self.step( gotoEnd );
		}

		t.queue = this.options.queue;
		t.elem = this.elem;
		t.saveState = function() {
			if ( jQuery._data( self.elem, "fxshow" + self.prop ) === undefined ) {
				if ( self.options.hide ) {
					jQuery._data( self.elem, "fxshow" + self.prop, self.start );
				} else if ( self.options.show ) {
					jQuery._data( self.elem, "fxshow" + self.prop, self.end );
				}
			}
		};

		if ( t() && jQuery.timers.push(t) && !timerId ) {
			timerId = setInterval( fx.tick, fx.interval );
		}
	},

	// Simple 'show' function
	show: function() {
		var dataShow = jQuery._data( this.elem, "fxshow" + this.prop );

		// Remember where we started, so that we can go back to it later
		this.options.orig[ this.prop ] = dataShow || jQuery.style( this.elem, this.prop );
		this.options.show = true;

		// Begin the animation
		// Make sure that we start at a small width/height to avoid any flash of content
		if ( dataShow !== undefined ) {
			// This show is picking up where a previous hide or show left off
			this.custom( this.cur(), dataShow );
		} else {
			this.custom( this.prop === "width" || this.prop === "height" ? 1 : 0, this.cur() );
		}

		// Start by showing the element
		jQuery( this.elem ).show();
	},

	// Simple 'hide' function
	hide: function() {
		// Remember where we started, so that we can go back to it later
		this.options.orig[ this.prop ] = jQuery._data( this.elem, "fxshow" + this.prop ) || jQuery.style( this.elem, this.prop );
		this.options.hide = true;

		// Begin the animation
		this.custom( this.cur(), 0 );
	},

	// Each step of an animation
	step: function( gotoEnd ) {
		var p, n, complete,
			t = fxNow || createFxNow(),
			done = true,
			elem = this.elem,
			options = this.options;

		if ( gotoEnd || t >= options.duration + this.startTime ) {
			this.now = this.end;
			this.pos = this.state = 1;
			this.update();

			options.animatedProperties[ this.prop ] = true;

			for ( p in options.animatedProperties ) {
				if ( options.animatedProperties[ p ] !== true ) {
					done = false;
				}
			}

			if ( done ) {
				// Reset the overflow
				if ( options.overflow != null && !jQuery.support.shrinkWrapBlocks ) {

					jQuery.each( [ "", "X", "Y" ], function( index, value ) {
						elem.style[ "overflow" + value ] = options.overflow[ index ];
					});
				}

				// Hide the element if the "hide" operation was done
				if ( options.hide ) {
					jQuery( elem ).hide();
				}

				// Reset the properties, if the item has been hidden or shown
				if ( options.hide || options.show ) {
					for ( p in options.animatedProperties ) {
						jQuery.style( elem, p, options.orig[ p ] );
						jQuery.removeData( elem, "fxshow" + p, true );
						// Toggle data is no longer needed
						jQuery.removeData( elem, "toggle" + p, true );
					}
				}

				// Execute the complete function
				// in the event that the complete function throws an exception
				// we must ensure it won't be called twice. #5684

				complete = options.complete;
				if ( complete ) {

					options.complete = false;
					complete.call( elem );
				}
			}

			return false;

		} else {
			// classical easing cannot be used with an Infinity duration
			if ( options.duration == Infinity ) {
				this.now = t;
			} else {
				n = t - this.startTime;
				this.state = n / options.duration;

				// Perform the easing function, defaults to swing
				this.pos = jQuery.easing[ options.animatedProperties[this.prop] ]( this.state, n, 0, 1, options.duration );
				this.now = this.start + ( (this.end - this.start) * this.pos );
			}
			// Perform the next step of the animation
			this.update();
		}

		return true;
	}
};

jQuery.extend( jQuery.fx, {
	tick: function() {
		var timer,
			timers = jQuery.timers,
			i = 0;

		for ( ; i < timers.length; i++ ) {
			timer = timers[ i ];
			// Checks the timer has not already been removed
			if ( !timer() && timers[ i ] === timer ) {
				timers.splice( i--, 1 );
			}
		}

		if ( !timers.length ) {
			jQuery.fx.stop();
		}
	},

	interval: 13,

	stop: function() {
		clearInterval( timerId );
		timerId = null;
	},

	speeds: {
		slow: 600,
		fast: 200,
		// Default speed
		_default: 400
	},

	step: {
		opacity: function( fx ) {
			jQuery.style( fx.elem, "opacity", fx.now );
		},

		_default: function( fx ) {
			if ( fx.elem.style && fx.elem.style[ fx.prop ] != null ) {
				fx.elem.style[ fx.prop ] = fx.now + fx.unit;
			} else {
				fx.elem[ fx.prop ] = fx.now;
			}
		}
	}
});

// Ensure props that can't be negative don't go there on undershoot easing
jQuery.each( fxAttrs.concat.apply( [], fxAttrs ), function( i, prop ) {
	// exclude marginTop, marginLeft, marginBottom and marginRight from this list
	if ( prop.indexOf( "margin" ) ) {
		jQuery.fx.step[ prop ] = function( fx ) {
			jQuery.style( fx.elem, prop, Math.max(0, fx.now) + fx.unit );
		};
	}
});

if ( jQuery.expr && jQuery.expr.filters ) {
	jQuery.expr.filters.animated = function( elem ) {
		return jQuery.grep(jQuery.timers, function( fn ) {
			return elem === fn.elem;
		}).length;
	};
}

// Try to restore the default display value of an element
function defaultDisplay( nodeName ) {

	if ( !elemdisplay[ nodeName ] ) {

		var body = document.body,
			elem = jQuery( "<" + nodeName + ">" ).appendTo( body ),
			display = elem.css( "display" );
		elem.remove();

		// If the simple way fails,
		// get element's real default display by attaching it to a temp iframe
		if ( display === "none" || display === "" ) {
			// No iframe to use yet, so create it
			if ( !iframe ) {
				iframe = document.createElement( "iframe" );
				iframe.frameBorder = iframe.width = iframe.height = 0;
			}

			body.appendChild( iframe );

			// Create a cacheable copy of the iframe document on first call.
			// IE and Opera will allow us to reuse the iframeDoc without re-writing the fake HTML
			// document to it; WebKit & Firefox won't allow reusing the iframe document.
			if ( !iframeDoc || !iframe.createElement ) {
				iframeDoc = ( iframe.contentWindow || iframe.contentDocument ).document;
				iframeDoc.write( ( jQuery.support.boxModel ? "<!doctype html>" : "" ) + "<html><body>" );
				iframeDoc.close();
			}

			elem = iframeDoc.createElement( nodeName );

			iframeDoc.body.appendChild( elem );

			display = jQuery.css( elem, "display" );
			body.removeChild( iframe );
		}

		// Store the correct default display
		elemdisplay[ nodeName ] = display;
	}

	return elemdisplay[ nodeName ];
}




var getOffset,
	rtable = /^t(?:able|d|h)$/i,
	rroot = /^(?:body|html)$/i;

if ( "getBoundingClientRect" in document.documentElement ) {
	getOffset = function( elem, doc, docElem, box ) {
		try {
			box = elem.getBoundingClientRect();
		} catch(e) {}

		// Make sure we're not dealing with a disconnected DOM node
		if ( !box || !jQuery.contains( docElem, elem ) ) {
			return box ? { top: box.top, left: box.left } : { top: 0, left: 0 };
		}

		var body = doc.body,
			win = getWindow( doc ),
			clientTop  = docElem.clientTop  || body.clientTop  || 0,
			clientLeft = docElem.clientLeft || body.clientLeft || 0,
			scrollTop  = win.pageYOffset || jQuery.support.boxModel && docElem.scrollTop  || body.scrollTop,
			scrollLeft = win.pageXOffset || jQuery.support.boxModel && docElem.scrollLeft || body.scrollLeft,
			top  = box.top  + scrollTop  - clientTop,
			left = box.left + scrollLeft - clientLeft;

		return { top: top, left: left };
	};

} else {
	getOffset = function( elem, doc, docElem ) {
		var computedStyle,
			offsetParent = elem.offsetParent,
			prevOffsetParent = elem,
			body = doc.body,
			defaultView = doc.defaultView,
			prevComputedStyle = defaultView ? defaultView.getComputedStyle( elem, null ) : elem.currentStyle,
			top = elem.offsetTop,
			left = elem.offsetLeft;

		while ( (elem = elem.parentNode) && elem !== body && elem !== docElem ) {
			if ( jQuery.support.fixedPosition && prevComputedStyle.position === "fixed" ) {
				break;
			}

			computedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle;
			top  -= elem.scrollTop;
			left -= elem.scrollLeft;

			if ( elem === offsetParent ) {
				top  += elem.offsetTop;
				left += elem.offsetLeft;

				if ( jQuery.support.doesNotAddBorder && !(jQuery.support.doesAddBorderForTableAndCells && rtable.test(elem.nodeName)) ) {
					top  += parseFloat( computedStyle.borderTopWidth  ) || 0;
					left += parseFloat( computedStyle.borderLeftWidth ) || 0;
				}

				prevOffsetParent = offsetParent;
				offsetParent = elem.offsetParent;
			}

			if ( jQuery.support.subtractsBorderForOverflowNotVisible && computedStyle.overflow !== "visible" ) {
				top  += parseFloat( computedStyle.borderTopWidth  ) || 0;
				left += parseFloat( computedStyle.borderLeftWidth ) || 0;
			}

			prevComputedStyle = computedStyle;
		}

		if ( prevComputedStyle.position === "relative" || prevComputedStyle.position === "static" ) {
			top  += body.offsetTop;
			left += body.offsetLeft;
		}

		if ( jQuery.support.fixedPosition && prevComputedStyle.position === "fixed" ) {
			top  += Math.max( docElem.scrollTop, body.scrollTop );
			left += Math.max( docElem.scrollLeft, body.scrollLeft );
		}

		return { top: top, left: left };
	};
}

jQuery.fn.offset = function( options ) {
	if ( arguments.length ) {
		return options === undefined ?
			this :
			this.each(function( i ) {
				jQuery.offset.setOffset( this, options, i );
			});
	}

	var elem = this[0],
		doc = elem && elem.ownerDocument;

	if ( !doc ) {
		return null;
	}

	if ( elem === doc.body ) {
		return jQuery.offset.bodyOffset( elem );
	}

	return getOffset( elem, doc, doc.documentElement );
};

jQuery.offset = {

	bodyOffset: function( body ) {
		var top = body.offsetTop,
			left = body.offsetLeft;

		if ( jQuery.support.doesNotIncludeMarginInBodyOffset ) {
			top  += parseFloat( jQuery.css(body, "marginTop") ) || 0;
			left += parseFloat( jQuery.css(body, "marginLeft") ) || 0;
		}

		return { top: top, left: left };
	},

	setOffset: function( elem, options, i ) {
		var position = jQuery.css( elem, "position" );

		// set position first, in-case top/left are set even on static elem
		if ( position === "static" ) {
			elem.style.position = "relative";
		}

		var curElem = jQuery( elem ),
			curOffset = curElem.offset(),
			curCSSTop = jQuery.css( elem, "top" ),
			curCSSLeft = jQuery.css( elem, "left" ),
			calculatePosition = ( position === "absolute" || position === "fixed" ) && jQuery.inArray("auto", [curCSSTop, curCSSLeft]) > -1,
			props = {}, curPosition = {}, curTop, curLeft;

		// need to be able to calculate position if either top or left is auto and position is either absolute or fixed
		if ( calculatePosition ) {
			curPosition = curElem.position();
			curTop = curPosition.top;
			curLeft = curPosition.left;
		} else {
			curTop = parseFloat( curCSSTop ) || 0;
			curLeft = parseFloat( curCSSLeft ) || 0;
		}

		if ( jQuery.isFunction( options ) ) {
			options = options.call( elem, i, curOffset );
		}

		if ( options.top != null ) {
			props.top = ( options.top - curOffset.top ) + curTop;
		}
		if ( options.left != null ) {
			props.left = ( options.left - curOffset.left ) + curLeft;
		}

		if ( "using" in options ) {
			options.using.call( elem, props );
		} else {
			curElem.css( props );
		}
	}
};


jQuery.fn.extend({

	position: function() {
		if ( !this[0] ) {
			return null;
		}

		var elem = this[0],

		// Get *real* offsetParent
		offsetParent = this.offsetParent(),

		// Get correct offsets
		offset       = this.offset(),
		parentOffset = rroot.test(offsetParent[0].nodeName) ? { top: 0, left: 0 } : offsetParent.offset();

		// Subtract element margins
		// note: when an element has margin: auto the offsetLeft and marginLeft
		// are the same in Safari causing offset.left to incorrectly be 0
		offset.top  -= parseFloat( jQuery.css(elem, "marginTop") ) || 0;
		offset.left -= parseFloat( jQuery.css(elem, "marginLeft") ) || 0;

		// Add offsetParent borders
		parentOffset.top  += parseFloat( jQuery.css(offsetParent[0], "borderTopWidth") ) || 0;
		parentOffset.left += parseFloat( jQuery.css(offsetParent[0], "borderLeftWidth") ) || 0;

		// Subtract the two offsets
		return {
			top:  offset.top  - parentOffset.top,
			left: offset.left - parentOffset.left
		};
	},

	offsetParent: function() {
		return this.map(function() {
			var offsetParent = this.offsetParent || document.body;
			while ( offsetParent && (!rroot.test(offsetParent.nodeName) && jQuery.css(offsetParent, "position") === "static") ) {
				offsetParent = offsetParent.offsetParent;
			}
			return offsetParent;
		});
	}
});


// Create scrollLeft and scrollTop methods
jQuery.each( {scrollLeft: "pageXOffset", scrollTop: "pageYOffset"}, function( method, prop ) {
	var top = /Y/.test( prop );

	jQuery.fn[ method ] = function( val ) {
		return jQuery.access( this, function( elem, method, val ) {
			var win = getWindow( elem );

			if ( val === undefined ) {
				return win ? (prop in win) ? win[ prop ] :
					jQuery.support.boxModel && win.document.documentElement[ method ] ||
						win.document.body[ method ] :
					elem[ method ];
			}

			if ( win ) {
				win.scrollTo(
					!top ? val : jQuery( win ).scrollLeft(),
					 top ? val : jQuery( win ).scrollTop()
				);

			} else {
				elem[ method ] = val;
			}
		}, method, val, arguments.length, null );
	};
});

function getWindow( elem ) {
	return jQuery.isWindow( elem ) ?
		elem :
		elem.nodeType === 9 ?
			elem.defaultView || elem.parentWindow :
			false;
}




// Create width, height, innerHeight, innerWidth, outerHeight and outerWidth methods
jQuery.each( { Height: "height", Width: "width" }, function( name, type ) {
	var clientProp = "client" + name,
		scrollProp = "scroll" + name,
		offsetProp = "offset" + name;

	// innerHeight and innerWidth
	jQuery.fn[ "inner" + name ] = function() {
		var elem = this[0];
		return elem ?
			elem.style ?
			parseFloat( jQuery.css( elem, type, "padding" ) ) :
			this[ type ]() :
			null;
	};

	// outerHeight and outerWidth
	jQuery.fn[ "outer" + name ] = function( margin ) {
		var elem = this[0];
		return elem ?
			elem.style ?
			parseFloat( jQuery.css( elem, type, margin ? "margin" : "border" ) ) :
			this[ type ]() :
			null;
	};

	jQuery.fn[ type ] = function( value ) {
		return jQuery.access( this, function( elem, type, value ) {
			var doc, docElemProp, orig, ret;

			if ( jQuery.isWindow( elem ) ) {
				// 3rd condition allows Nokia support, as it supports the docElem prop but not CSS1Compat
				doc = elem.document;
				docElemProp = doc.documentElement[ clientProp ];
				return jQuery.support.boxModel && docElemProp ||
					doc.body && doc.body[ clientProp ] || docElemProp;
			}

			// Get document width or height
			if ( elem.nodeType === 9 ) {
				// Either scroll[Width/Height] or offset[Width/Height], whichever is greater
				doc = elem.documentElement;

				// when a window > document, IE6 reports a offset[Width/Height] > client[Width/Height]
				// so we can't use max, as it'll choose the incorrect offset[Width/Height]
				// instead we use the correct client[Width/Height]
				// support:IE6
				if ( doc[ clientProp ] >= doc[ scrollProp ] ) {
					return doc[ clientProp ];
				}

				return Math.max(
					elem.body[ scrollProp ], doc[ scrollProp ],
					elem.body[ offsetProp ], doc[ offsetProp ]
				);
			}

			// Get width or height on the element
			if ( value === undefined ) {
				orig = jQuery.css( elem, type );
				ret = parseFloat( orig );
				return jQuery.isNumeric( ret ) ? ret : orig;
			}

			// Set the width or height on the element
			jQuery( elem ).css( type, value );
		}, type, value, arguments.length, null );
	};
});




// Expose jQuery to the global object
window.jQuery = window.$ = jQuery;

// Expose jQuery as an AMD module, but only for AMD loaders that
// understand the issues with loading multiple versions of jQuery
// in a page that all might call define(). The loader will indicate
// they have special allowances for multiple jQuery versions by
// specifying define.amd.jQuery = true. Register as a named module,
// since jQuery can be concatenated with other files that may use define,
// but not use a proper concatenation script that understands anonymous
// AMD modules. A named AMD is safest and most robust way to register.
// Lowercase jquery is used because AMD module names are derived from
// file names, and jQuery is normally delivered in a lowercase file name.
// Do this after creating the global so that if an AMD module wants to call
// noConflict to hide this version of jQuery, it will work.
if ( typeof define === "function" && define.amd && define.amd.jQuery ) {
	define( "jquery", [], function () { return jQuery; } );
}



})( window );

//     Underscore.js 1.3.2
//     (c) 2009-2012 Jeremy Ashkenas, DocumentCloud Inc.
//     Underscore is freely distributable under the MIT license.
//     Portions of Underscore are inspired or borrowed from Prototype,
//     Oliver Steele's Functional, and John Resig's Micro-Templating.
//     For all details and documentation:
//     http://documentcloud.github.com/underscore

(function() {

  // Baseline setup
  // --------------

  // Establish the root object, `window` in the browser, or `global` on the server.
  var root = this;

  // Save the previous value of the `_` variable.
  var previousUnderscore = root._;

  // Establish the object that gets returned to break out of a loop iteration.
  var breaker = {};

  // Save bytes in the minified (but not gzipped) version:
  var ArrayProto = Array.prototype, ObjProto = Object.prototype, FuncProto = Function.prototype;

  // Create quick reference variables for speed access to core prototypes.
  var slice            = ArrayProto.slice,
      unshift          = ArrayProto.unshift,
      toString         = ObjProto.toString,
      hasOwnProperty   = ObjProto.hasOwnProperty;

  // All **ECMAScript 5** native function implementations that we hope to use
  // are declared here.
  var
    nativeForEach      = ArrayProto.forEach,
    nativeMap          = ArrayProto.map,
    nativeReduce       = ArrayProto.reduce,
    nativeReduceRight  = ArrayProto.reduceRight,
    nativeFilter       = ArrayProto.filter,
    nativeEvery        = ArrayProto.every,
    nativeSome         = ArrayProto.some,
    nativeIndexOf      = ArrayProto.indexOf,
    nativeLastIndexOf  = ArrayProto.lastIndexOf,
    nativeIsArray      = Array.isArray,
    nativeKeys         = Object.keys,
    nativeBind         = FuncProto.bind;

  // Create a safe reference to the Underscore object for use below.
  var _ = function(obj) { return new wrapper(obj); };

  // Export the Underscore object for **Node.js** and **"CommonJS"**, with
  // backwards-compatibility for the old `require()` API. If we're not in
  // CommonJS, add `_` to the global object via a string identifier for
  // the Closure Compiler "advanced" mode. Registration as an AMD module
  // via define() happens at the end of this file.
  if (typeof exports !== 'undefined') {
    if (typeof module !== 'undefined' && module.exports) {
      exports = module.exports = _;
    }
    exports._ = _;
  } else {
    root['_'] = _;
  }

  // Current version.
  _.VERSION = '1.3.2';

  // Collection Functions
  // --------------------

  // The cornerstone, an `each` implementation, aka `forEach`.
  // Handles objects with the built-in `forEach`, arrays, and raw objects.
  // Delegates to **ECMAScript 5**'s native `forEach` if available.
  var each = _.each = _.forEach = function(obj, iterator, context) {
    if (obj == null) return;
    if (nativeForEach && obj.forEach === nativeForEach) {
      obj.forEach(iterator, context);
    } else if (obj.length === +obj.length) {
      for (var i = 0, l = obj.length; i < l; i++) {
        if (i in obj && iterator.call(context, obj[i], i, obj) === breaker) return;
      }
    } else {
      for (var key in obj) {
        if (_.has(obj, key)) {
          if (iterator.call(context, obj[key], key, obj) === breaker) return;
        }
      }
    }
  };

  // Return the results of applying the iterator to each element.
  // Delegates to **ECMAScript 5**'s native `map` if available.
  _.map = _.collect = function(obj, iterator, context) {
    var results = [];
    if (obj == null) return results;
    if (nativeMap && obj.map === nativeMap) return obj.map(iterator, context);
    each(obj, function(value, index, list) {
      results[results.length] = iterator.call(context, value, index, list);
    });
    if (obj.length === +obj.length) results.length = obj.length;
    return results;
  };

  // **Reduce** builds up a single result from a list of values, aka `inject`,
  // or `foldl`. Delegates to **ECMAScript 5**'s native `reduce` if available.
  _.reduce = _.foldl = _.inject = function(obj, iterator, memo, context) {
    var initial = arguments.length > 2;
    if (obj == null) obj = [];
    if (nativeReduce && obj.reduce === nativeReduce) {
      if (context) iterator = _.bind(iterator, context);
      return initial ? obj.reduce(iterator, memo) : obj.reduce(iterator);
    }
    each(obj, function(value, index, list) {
      if (!initial) {
        memo = value;
        initial = true;
      } else {
        memo = iterator.call(context, memo, value, index, list);
      }
    });
    if (!initial) throw new TypeError('Reduce of empty array with no initial value');
    return memo;
  };

  // The right-associative version of reduce, also known as `foldr`.
  // Delegates to **ECMAScript 5**'s native `reduceRight` if available.
  _.reduceRight = _.foldr = function(obj, iterator, memo, context) {
    var initial = arguments.length > 2;
    if (obj == null) obj = [];
    if (nativeReduceRight && obj.reduceRight === nativeReduceRight) {
      if (context) iterator = _.bind(iterator, context);
      return initial ? obj.reduceRight(iterator, memo) : obj.reduceRight(iterator);
    }
    var reversed = _.toArray(obj).reverse();
    if (context && !initial) iterator = _.bind(iterator, context);
    return initial ? _.reduce(reversed, iterator, memo, context) : _.reduce(reversed, iterator);
  };

  // Return the first value which passes a truth test. Aliased as `detect`.
  _.find = _.detect = function(obj, iterator, context) {
    var result;
    any(obj, function(value, index, list) {
      if (iterator.call(context, value, index, list)) {
        result = value;
        return true;
      }
    });
    return result;
  };

  // Return all the elements that pass a truth test.
  // Delegates to **ECMAScript 5**'s native `filter` if available.
  // Aliased as `select`.
  _.filter = _.select = function(obj, iterator, context) {
    var results = [];
    if (obj == null) return results;
    if (nativeFilter && obj.filter === nativeFilter) return obj.filter(iterator, context);
    each(obj, function(value, index, list) {
      if (iterator.call(context, value, index, list)) results[results.length] = value;
    });
    return results;
  };

  // Return all the elements for which a truth test fails.
  _.reject = function(obj, iterator, context) {
    var results = [];
    if (obj == null) return results;
    each(obj, function(value, index, list) {
      if (!iterator.call(context, value, index, list)) results[results.length] = value;
    });
    return results;
  };

  // Determine whether all of the elements match a truth test.
  // Delegates to **ECMAScript 5**'s native `every` if available.
  // Aliased as `all`.
  _.every = _.all = function(obj, iterator, context) {
    var result = true;
    if (obj == null) return result;
    if (nativeEvery && obj.every === nativeEvery) return obj.every(iterator, context);
    each(obj, function(value, index, list) {
      if (!(result = result && iterator.call(context, value, index, list))) return breaker;
    });
    return !!result;
  };

  // Determine if at least one element in the object matches a truth test.
  // Delegates to **ECMAScript 5**'s native `some` if available.
  // Aliased as `any`.
  var any = _.some = _.any = function(obj, iterator, context) {
    iterator || (iterator = _.identity);
    var result = false;
    if (obj == null) return result;
    if (nativeSome && obj.some === nativeSome) return obj.some(iterator, context);
    each(obj, function(value, index, list) {
      if (result || (result = iterator.call(context, value, index, list))) return breaker;
    });
    return !!result;
  };

  // Determine if a given value is included in the array or object using `===`.
  // Aliased as `contains`.
  _.include = _.contains = function(obj, target) {
    var found = false;
    if (obj == null) return found;
    if (nativeIndexOf && obj.indexOf === nativeIndexOf) return obj.indexOf(target) != -1;
    found = any(obj, function(value) {
      return value === target;
    });
    return found;
  };

  // Invoke a method (with arguments) on every item in a collection.
  _.invoke = function(obj, method) {
    var args = slice.call(arguments, 2);
    return _.map(obj, function(value) {
      return (_.isFunction(method) ? method || value : value[method]).apply(value, args);
    });
  };

  // Convenience version of a common use case of `map`: fetching a property.
  _.pluck = function(obj, key) {
    return _.map(obj, function(value){ return value[key]; });
  };

  // Return the maximum element or (element-based computation).
  _.max = function(obj, iterator, context) {
    if (!iterator && _.isArray(obj) && obj[0] === +obj[0]) return Math.max.apply(Math, obj);
    if (!iterator && _.isEmpty(obj)) return -Infinity;
    var result = {computed : -Infinity};
    each(obj, function(value, index, list) {
      var computed = iterator ? iterator.call(context, value, index, list) : value;
      computed >= result.computed && (result = {value : value, computed : computed});
    });
    return result.value;
  };

  // Return the minimum element (or element-based computation).
  _.min = function(obj, iterator, context) {
    if (!iterator && _.isArray(obj) && obj[0] === +obj[0]) return Math.min.apply(Math, obj);
    if (!iterator && _.isEmpty(obj)) return Infinity;
    var result = {computed : Infinity};
    each(obj, function(value, index, list) {
      var computed = iterator ? iterator.call(context, value, index, list) : value;
      computed < result.computed && (result = {value : value, computed : computed});
    });
    return result.value;
  };

  // Shuffle an array.
  _.shuffle = function(obj) {
    var shuffled = [], rand;
    each(obj, function(value, index, list) {
      rand = Math.floor(Math.random() * (index + 1));
      shuffled[index] = shuffled[rand];
      shuffled[rand] = value;
    });
    return shuffled;
  };

  // Sort the object's values by a criterion produced by an iterator.
  _.sortBy = function(obj, val, context) {
    var iterator = _.isFunction(val) ? val : function(obj) { return obj[val]; };
    return _.pluck(_.map(obj, function(value, index, list) {
      return {
        value : value,
        criteria : iterator.call(context, value, index, list)
      };
    }).sort(function(left, right) {
      var a = left.criteria, b = right.criteria;
      if (a === void 0) return 1;
      if (b === void 0) return -1;
      return a < b ? -1 : a > b ? 1 : 0;
    }), 'value');
  };

  // Groups the object's values by a criterion. Pass either a string attribute
  // to group by, or a function that returns the criterion.
  _.groupBy = function(obj, val) {
    var result = {};
    var iterator = _.isFunction(val) ? val : function(obj) { return obj[val]; };
    each(obj, function(value, index) {
      var key = iterator(value, index);
      (result[key] || (result[key] = [])).push(value);
    });
    return result;
  };

  // Use a comparator function to figure out at what index an object should
  // be inserted so as to maintain order. Uses binary search.
  _.sortedIndex = function(array, obj, iterator) {
    iterator || (iterator = _.identity);
    var low = 0, high = array.length;
    while (low < high) {
      var mid = (low + high) >> 1;
      iterator(array[mid]) < iterator(obj) ? low = mid + 1 : high = mid;
    }
    return low;
  };

  // Safely convert anything iterable into a real, live array.
  _.toArray = function(obj) {
    if (!obj)                                     return [];
    if (_.isArray(obj))                           return slice.call(obj);
    if (_.isArguments(obj))                       return slice.call(obj);
    if (obj.toArray && _.isFunction(obj.toArray)) return obj.toArray();
    return _.values(obj);
  };

  // Return the number of elements in an object.
  _.size = function(obj) {
    return _.isArray(obj) ? obj.length : _.keys(obj).length;
  };

  // Array Functions
  // ---------------

  // Get the first element of an array. Passing **n** will return the first N
  // values in the array. Aliased as `head` and `take`. The **guard** check
  // allows it to work with `_.map`.
  _.first = _.head = _.take = function(array, n, guard) {
    return (n != null) && !guard ? slice.call(array, 0, n) : array[0];
  };

  // Returns everything but the last entry of the array. Especcialy useful on
  // the arguments object. Passing **n** will return all the values in
  // the array, excluding the last N. The **guard** check allows it to work with
  // `_.map`.
  _.initial = function(array, n, guard) {
    return slice.call(array, 0, array.length - ((n == null) || guard ? 1 : n));
  };

  // Get the last element of an array. Passing **n** will return the last N
  // values in the array. The **guard** check allows it to work with `_.map`.
  _.last = function(array, n, guard) {
    if ((n != null) && !guard) {
      return slice.call(array, Math.max(array.length - n, 0));
    } else {
      return array[array.length - 1];
    }
  };

  // Returns everything but the first entry of the array. Aliased as `tail`.
  // Especially useful on the arguments object. Passing an **index** will return
  // the rest of the values in the array from that index onward. The **guard**
  // check allows it to work with `_.map`.
  _.rest = _.tail = function(array, index, guard) {
    return slice.call(array, (index == null) || guard ? 1 : index);
  };

  // Trim out all falsy values from an array.
  _.compact = function(array) {
    return _.filter(array, function(value){ return !!value; });
  };

  // Return a completely flattened version of an array.
  _.flatten = function(array, shallow) {
    return _.reduce(array, function(memo, value) {
      if (_.isArray(value)) return memo.concat(shallow ? value : _.flatten(value));
      memo[memo.length] = value;
      return memo;
    }, []);
  };

  // Return a version of the array that does not contain the specified value(s).
  _.without = function(array) {
    return _.difference(array, slice.call(arguments, 1));
  };

  // Produce a duplicate-free version of the array. If the array has already
  // been sorted, you have the option of using a faster algorithm.
  // Aliased as `unique`.
  _.uniq = _.unique = function(array, isSorted, iterator) {
    var initial = iterator ? _.map(array, iterator) : array;
    var results = [];
    // The `isSorted` flag is irrelevant if the array only contains two elements.
    if (array.length < 3) isSorted = true;
    _.reduce(initial, function (memo, value, index) {
      if (isSorted ? _.last(memo) !== value || !memo.length : !_.include(memo, value)) {
        memo.push(value);
        results.push(array[index]);
      }
      return memo;
    }, []);
    return results;
  };

  // Produce an array that contains the union: each distinct element from all of
  // the passed-in arrays.
  _.union = function() {
    return _.uniq(_.flatten(arguments, true));
  };

  // Produce an array that contains every item shared between all the
  // passed-in arrays. (Aliased as "intersect" for back-compat.)
  _.intersection = _.intersect = function(array) {
    var rest = slice.call(arguments, 1);
    return _.filter(_.uniq(array), function(item) {
      return _.every(rest, function(other) {
        return _.indexOf(other, item) >= 0;
      });
    });
  };

  // Take the difference between one array and a number of other arrays.
  // Only the elements present in just the first array will remain.
  _.difference = function(array) {
    var rest = _.flatten(slice.call(arguments, 1), true);
    return _.filter(array, function(value){ return !_.include(rest, value); });
  };

  // Zip together multiple lists into a single array -- elements that share
  // an index go together.
  _.zip = function() {
    var args = slice.call(arguments);
    var length = _.max(_.pluck(args, 'length'));
    var results = new Array(length);
    for (var i = 0; i < length; i++) results[i] = _.pluck(args, "" + i);
    return results;
  };

  // If the browser doesn't supply us with indexOf (I'm looking at you, **MSIE**),
  // we need this function. Return the position of the first occurrence of an
  // item in an array, or -1 if the item is not included in the array.
  // Delegates to **ECMAScript 5**'s native `indexOf` if available.
  // If the array is large and already in sort order, pass `true`
  // for **isSorted** to use binary search.
  _.indexOf = function(array, item, isSorted) {
    if (array == null) return -1;
    var i, l;
    if (isSorted) {
      i = _.sortedIndex(array, item);
      return array[i] === item ? i : -1;
    }
    if (nativeIndexOf && array.indexOf === nativeIndexOf) return array.indexOf(item);
    for (i = 0, l = array.length; i < l; i++) if (i in array && array[i] === item) return i;
    return -1;
  };

  // Delegates to **ECMAScript 5**'s native `lastIndexOf` if available.
  _.lastIndexOf = function(array, item) {
    if (array == null) return -1;
    if (nativeLastIndexOf && array.lastIndexOf === nativeLastIndexOf) return array.lastIndexOf(item);
    var i = array.length;
    while (i--) if (i in array && array[i] === item) return i;
    return -1;
  };

  // Generate an integer Array containing an arithmetic progression. A port of
  // the native Python `range()` function. See
  // [the Python documentation](http://docs.python.org/library/functions.html#range).
  _.range = function(start, stop, step) {
    if (arguments.length <= 1) {
      stop = start || 0;
      start = 0;
    }
    step = arguments[2] || 1;

    var len = Math.max(Math.ceil((stop - start) / step), 0);
    var idx = 0;
    var range = new Array(len);

    while(idx < len) {
      range[idx++] = start;
      start += step;
    }

    return range;
  };

  // Function (ahem) Functions
  // ------------------

  // Reusable constructor function for prototype setting.
  var ctor = function(){};

  // Create a function bound to a given object (assigning `this`, and arguments,
  // optionally). Binding with arguments is also known as `curry`.
  // Delegates to **ECMAScript 5**'s native `Function.bind` if available.
  // We check for `func.bind` first, to fail fast when `func` is undefined.
  _.bind = function bind(func, context) {
    var bound, args;
    if (func.bind === nativeBind && nativeBind) return nativeBind.apply(func, slice.call(arguments, 1));
    if (!_.isFunction(func)) throw new TypeError;
    args = slice.call(arguments, 2);
    return bound = function() {
      if (!(this instanceof bound)) return func.apply(context, args.concat(slice.call(arguments)));
      ctor.prototype = func.prototype;
      var self = new ctor;
      var result = func.apply(self, args.concat(slice.call(arguments)));
      if (Object(result) === result) return result;
      return self;
    };
  };

  // Bind all of an object's methods to that object. Useful for ensuring that
  // all callbacks defined on an object belong to it.
  _.bindAll = function(obj) {
    var funcs = slice.call(arguments, 1);
    if (funcs.length == 0) funcs = _.functions(obj);
    each(funcs, function(f) { obj[f] = _.bind(obj[f], obj); });
    return obj;
  };

  // Memoize an expensive function by storing its results.
  _.memoize = function(func, hasher) {
    var memo = {};
    hasher || (hasher = _.identity);
    return function() {
      var key = hasher.apply(this, arguments);
      return _.has(memo, key) ? memo[key] : (memo[key] = func.apply(this, arguments));
    };
  };

  // Delays a function for the given number of milliseconds, and then calls
  // it with the arguments supplied.
  _.delay = function(func, wait) {
    var args = slice.call(arguments, 2);
    return setTimeout(function(){ return func.apply(null, args); }, wait);
  };

  // Defers a function, scheduling it to run after the current call stack has
  // cleared.
  _.defer = function(func) {
    return _.delay.apply(_, [func, 1].concat(slice.call(arguments, 1)));
  };

  // Returns a function, that, when invoked, will only be triggered at most once
  // during a given window of time.
  _.throttle = function(func, wait) {
    var context, args, timeout, throttling, more, result;
    var whenDone = _.debounce(function(){ more = throttling = false; }, wait);
    return function() {
      context = this; args = arguments;
      var later = function() {
        timeout = null;
        if (more) func.apply(context, args);
        whenDone();
      };
      if (!timeout) timeout = setTimeout(later, wait);
      if (throttling) {
        more = true;
      } else {
        result = func.apply(context, args);
      }
      whenDone();
      throttling = true;
      return result;
    };
  };

  // Returns a function, that, as long as it continues to be invoked, will not
  // be triggered. The function will be called after it stops being called for
  // N milliseconds. If `immediate` is passed, trigger the function on the
  // leading edge, instead of the trailing.
  _.debounce = function(func, wait, immediate) {
    var timeout;
    return function() {
      var context = this, args = arguments;
      var later = function() {
        timeout = null;
        if (!immediate) func.apply(context, args);
      };
      if (immediate && !timeout) func.apply(context, args);
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  };

  // Returns a function that will be executed at most one time, no matter how
  // often you call it. Useful for lazy initialization.
  _.once = function(func) {
    var ran = false, memo;
    return function() {
      if (ran) return memo;
      ran = true;
      return memo = func.apply(this, arguments);
    };
  };

  // Returns the first function passed as an argument to the second,
  // allowing you to adjust arguments, run code before and after, and
  // conditionally execute the original function.
  _.wrap = function(func, wrapper) {
    return function() {
      var args = [func].concat(slice.call(arguments, 0));
      return wrapper.apply(this, args);
    };
  };

  // Returns a function that is the composition of a list of functions, each
  // consuming the return value of the function that follows.
  _.compose = function() {
    var funcs = arguments;
    return function() {
      var args = arguments;
      for (var i = funcs.length - 1; i >= 0; i--) {
        args = [funcs[i].apply(this, args)];
      }
      return args[0];
    };
  };

  // Returns a function that will only be executed after being called N times.
  _.after = function(times, func) {
    if (times <= 0) return func();
    return function() {
      if (--times < 1) { return func.apply(this, arguments); }
    };
  };

  // Object Functions
  // ----------------

  // Retrieve the names of an object's properties.
  // Delegates to **ECMAScript 5**'s native `Object.keys`
  _.keys = nativeKeys || function(obj) {
    if (obj !== Object(obj)) throw new TypeError('Invalid object');
    var keys = [];
    for (var key in obj) if (_.has(obj, key)) keys[keys.length] = key;
    return keys;
  };

  // Retrieve the values of an object's properties.
  _.values = function(obj) {
    return _.map(obj, _.identity);
  };

  // Return a sorted list of the function names available on the object.
  // Aliased as `methods`
  _.functions = _.methods = function(obj) {
    var names = [];
    for (var key in obj) {
      if (_.isFunction(obj[key])) names.push(key);
    }
    return names.sort();
  };

  // Extend a given object with all the properties in passed-in object(s).
  _.extend = function(obj) {
    each(slice.call(arguments, 1), function(source) {
      for (var prop in source) {
        obj[prop] = source[prop];
      }
    });
    return obj;
  };

  // Return a copy of the object only containing the whitelisted properties.
  _.pick = function(obj) {
    var result = {};
    each(_.flatten(slice.call(arguments, 1)), function(key) {
      if (key in obj) result[key] = obj[key];
    });
    return result;
  };

  // Fill in a given object with default properties.
  _.defaults = function(obj) {
    each(slice.call(arguments, 1), function(source) {
      for (var prop in source) {
        if (obj[prop] == null) obj[prop] = source[prop];
      }
    });
    return obj;
  };

  // Create a (shallow-cloned) duplicate of an object.
  _.clone = function(obj) {
    if (!_.isObject(obj)) return obj;
    return _.isArray(obj) ? obj.slice() : _.extend({}, obj);
  };

  // Invokes interceptor with the obj, and then returns obj.
  // The primary purpose of this method is to "tap into" a method chain, in
  // order to perform operations on intermediate results within the chain.
  _.tap = function(obj, interceptor) {
    interceptor(obj);
    return obj;
  };

  // Internal recursive comparison function.
  function eq(a, b, stack) {
    // Identical objects are equal. `0 === -0`, but they aren't identical.
    // See the Harmony `egal` proposal: http://wiki.ecmascript.org/doku.php?id=harmony:egal.
    if (a === b) return a !== 0 || 1 / a == 1 / b;
    // A strict comparison is necessary because `null == undefined`.
    if (a == null || b == null) return a === b;
    // Unwrap any wrapped objects.
    if (a._chain) a = a._wrapped;
    if (b._chain) b = b._wrapped;
    // Invoke a custom `isEqual` method if one is provided.
    if (a.isEqual && _.isFunction(a.isEqual)) return a.isEqual(b);
    if (b.isEqual && _.isFunction(b.isEqual)) return b.isEqual(a);
    // Compare `[[Class]]` names.
    var className = toString.call(a);
    if (className != toString.call(b)) return false;
    switch (className) {
      // Strings, numbers, dates, and booleans are compared by value.
      case '[object String]':
        // Primitives and their corresponding object wrappers are equivalent; thus, `"5"` is
        // equivalent to `new String("5")`.
        return a == String(b);
      case '[object Number]':
        // `NaN`s are equivalent, but non-reflexive. An `egal` comparison is performed for
        // other numeric values.
        return a != +a ? b != +b : (a == 0 ? 1 / a == 1 / b : a == +b);
      case '[object Date]':
      case '[object Boolean]':
        // Coerce dates and booleans to numeric primitive values. Dates are compared by their
        // millisecond representations. Note that invalid dates with millisecond representations
        // of `NaN` are not equivalent.
        return +a == +b;
      // RegExps are compared by their source patterns and flags.
      case '[object RegExp]':
        return a.source == b.source &&
               a.global == b.global &&
               a.multiline == b.multiline &&
               a.ignoreCase == b.ignoreCase;
    }
    if (typeof a != 'object' || typeof b != 'object') return false;
    // Assume equality for cyclic structures. The algorithm for detecting cyclic
    // structures is adapted from ES 5.1 section 15.12.3, abstract operation `JO`.
    var length = stack.length;
    while (length--) {
      // Linear search. Performance is inversely proportional to the number of
      // unique nested structures.
      if (stack[length] == a) return true;
    }
    // Add the first object to the stack of traversed objects.
    stack.push(a);
    var size = 0, result = true;
    // Recursively compare objects and arrays.
    if (className == '[object Array]') {
      // Compare array lengths to determine if a deep comparison is necessary.
      size = a.length;
      result = size == b.length;
      if (result) {
        // Deep compare the contents, ignoring non-numeric properties.
        while (size--) {
          // Ensure commutative equality for sparse arrays.
          if (!(result = size in a == size in b && eq(a[size], b[size], stack))) break;
        }
      }
    } else {
      // Objects with different constructors are not equivalent.
      if ('constructor' in a != 'constructor' in b || a.constructor != b.constructor) return false;
      // Deep compare objects.
      for (var key in a) {
        if (_.has(a, key)) {
          // Count the expected number of properties.
          size++;
          // Deep compare each member.
          if (!(result = _.has(b, key) && eq(a[key], b[key], stack))) break;
        }
      }
      // Ensure that both objects contain the same number of properties.
      if (result) {
        for (key in b) {
          if (_.has(b, key) && !(size--)) break;
        }
        result = !size;
      }
    }
    // Remove the first object from the stack of traversed objects.
    stack.pop();
    return result;
  }

  // Perform a deep comparison to check if two objects are equal.
  _.isEqual = function(a, b) {
    return eq(a, b, []);
  };

  // Is a given array, string, or object empty?
  // An "empty" object has no enumerable own-properties.
  _.isEmpty = function(obj) {
    if (obj == null) return true;
    if (_.isArray(obj) || _.isString(obj)) return obj.length === 0;
    for (var key in obj) if (_.has(obj, key)) return false;
    return true;
  };

  // Is a given value a DOM element?
  _.isElement = function(obj) {
    return !!(obj && obj.nodeType == 1);
  };

  // Is a given value an array?
  // Delegates to ECMA5's native Array.isArray
  _.isArray = nativeIsArray || function(obj) {
    return toString.call(obj) == '[object Array]';
  };

  // Is a given variable an object?
  _.isObject = function(obj) {
    return obj === Object(obj);
  };

  // Is a given variable an arguments object?
  _.isArguments = function(obj) {
    return toString.call(obj) == '[object Arguments]';
  };
  if (!_.isArguments(arguments)) {
    _.isArguments = function(obj) {
      return !!(obj && _.has(obj, 'callee'));
    };
  }

  // Is a given value a function?
  _.isFunction = function(obj) {
    return toString.call(obj) == '[object Function]';
  };

  // Is a given value a string?
  _.isString = function(obj) {
    return toString.call(obj) == '[object String]';
  };

  // Is a given value a number?
  _.isNumber = function(obj) {
    return toString.call(obj) == '[object Number]';
  };

  // Is a given object a finite number?
  _.isFinite = function(obj) {
    return _.isNumber(obj) && isFinite(obj);
  };

  // Is the given value `NaN`?
  _.isNaN = function(obj) {
    // `NaN` is the only value for which `===` is not reflexive.
    return obj !== obj;
  };

  // Is a given value a boolean?
  _.isBoolean = function(obj) {
    return obj === true || obj === false || toString.call(obj) == '[object Boolean]';
  };

  // Is a given value a date?
  _.isDate = function(obj) {
    return toString.call(obj) == '[object Date]';
  };

  // Is the given value a regular expression?
  _.isRegExp = function(obj) {
    return toString.call(obj) == '[object RegExp]';
  };

  // Is a given value equal to null?
  _.isNull = function(obj) {
    return obj === null;
  };

  // Is a given variable undefined?
  _.isUndefined = function(obj) {
    return obj === void 0;
  };

  // Has own property?
  _.has = function(obj, key) {
    return hasOwnProperty.call(obj, key);
  };

  // Utility Functions
  // -----------------

  // Run Underscore.js in *noConflict* mode, returning the `_` variable to its
  // previous owner. Returns a reference to the Underscore object.
  _.noConflict = function() {
    root._ = previousUnderscore;
    return this;
  };

  // Keep the identity function around for default iterators.
  _.identity = function(value) {
    return value;
  };

  // Run a function **n** times.
  _.times = function (n, iterator, context) {
    for (var i = 0; i < n; i++) iterator.call(context, i);
  };

  // Escape a string for HTML interpolation.
  _.escape = function(string) {
    return (''+string).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#x27;').replace(/\//g,'&#x2F;');
  };

  // If the value of the named property is a function then invoke it;
  // otherwise, return it.
  _.result = function(object, property) {
    if (object == null) return null;
    var value = object[property];
    return _.isFunction(value) ? value.call(object) : value;
  };

  // Add your own custom functions to the Underscore object, ensuring that
  // they're correctly added to the OOP wrapper as well.
  _.mixin = function(obj) {
    each(_.functions(obj), function(name){
      addToWrapper(name, _[name] = obj[name]);
    });
  };

  // Generate a unique integer id (unique within the entire client session).
  // Useful for temporary DOM ids.
  var idCounter = 0;
  _.uniqueId = function(prefix) {
    var id = idCounter++;
    return prefix ? prefix + id : id;
  };

  // By default, Underscore uses ERB-style template delimiters, change the
  // following template settings to use alternative delimiters.
  _.templateSettings = {
    evaluate    : /<%([\s\S]+?)%>/g,
    interpolate : /<%=([\s\S]+?)%>/g,
    escape      : /<%-([\s\S]+?)%>/g
  };

  // When customizing `templateSettings`, if you don't want to define an
  // interpolation, evaluation or escaping regex, we need one that is
  // guaranteed not to match.
  var noMatch = /.^/;

  // Certain characters need to be escaped so that they can be put into a
  // string literal.
  var escapes = {
    '\\': '\\',
    "'": "'",
    'r': '\r',
    'n': '\n',
    't': '\t',
    'u2028': '\u2028',
    'u2029': '\u2029'
  };

  for (var p in escapes) escapes[escapes[p]] = p;
  var escaper = /\\|'|\r|\n|\t|\u2028|\u2029/g;
  var unescaper = /\\(\\|'|r|n|t|u2028|u2029)/g;

  // Within an interpolation, evaluation, or escaping, remove HTML escaping
  // that had been previously added.
  var unescape = function(code) {
    return code.replace(unescaper, function(match, escape) {
      return escapes[escape];
    });
  };

  // JavaScript micro-templating, similar to John Resig's implementation.
  // Underscore templating handles arbitrary delimiters, preserves whitespace,
  // and correctly escapes quotes within interpolated code.
  _.template = function(text, data, settings) {
    settings = _.extend(_.templateSettings, settings);

    // Compile the template source, taking care to escape characters that
    // cannot be included in a string literal and then unescape them in code
    // blocks.
    var source = "__p+='" + text
      .replace(escaper, function(match) {
        return '\\' + escapes[match];
      })
      .replace(settings.escape || noMatch, function(match, code) {
        return "'+\n_.escape(" + unescape(code) + ")+\n'";
      })
      .replace(settings.interpolate || noMatch, function(match, code) {
        return "'+\n(" + unescape(code) + ")+\n'";
      })
      .replace(settings.evaluate || noMatch, function(match, code) {
        return "';\n" + unescape(code) + "\n;__p+='";
      }) + "';\n";

    // If a variable is not specified, place data values in local scope.
    if (!settings.variable) source = 'with(obj||{}){\n' + source + '}\n';

    source = "var __p='';" +
      "var print=function(){__p+=Array.prototype.join.call(arguments, '')};\n" +
      source + "return __p;\n";

    var render = new Function(settings.variable || 'obj', '_', source);
    if (data) return render(data, _);
    var template = function(data) {
      return render.call(this, data, _);
    };

    // Provide the compiled function source as a convenience for build time
    // precompilation.
    template.source = 'function(' + (settings.variable || 'obj') + '){\n' +
      source + '}';

    return template;
  };

  // Add a "chain" function, which will delegate to the wrapper.
  _.chain = function(obj) {
    return _(obj).chain();
  };

  // The OOP Wrapper
  // ---------------

  // If Underscore is called as a function, it returns a wrapped object that
  // can be used OO-style. This wrapper holds altered versions of all the
  // underscore functions. Wrapped objects may be chained.
  var wrapper = function(obj) { this._wrapped = obj; };

  // Expose `wrapper.prototype` as `_.prototype`
  _.prototype = wrapper.prototype;

  // Helper function to continue chaining intermediate results.
  var result = function(obj, chain) {
    return chain ? _(obj).chain() : obj;
  };

  // A method to easily add functions to the OOP wrapper.
  var addToWrapper = function(name, func) {
    wrapper.prototype[name] = function() {
      var args = slice.call(arguments);
      unshift.call(args, this._wrapped);
      return result(func.apply(_, args), this._chain);
    };
  };

  // Add all of the Underscore functions to the wrapper object.
  _.mixin(_);

  // Add all mutator Array functions to the wrapper.
  each(['pop', 'push', 'reverse', 'shift', 'sort', 'splice', 'unshift'], function(name) {
    var method = ArrayProto[name];
    wrapper.prototype[name] = function() {
      var wrapped = this._wrapped;
      method.apply(wrapped, arguments);
      var length = wrapped.length;
      if ((name == 'shift' || name == 'splice') && length === 0) delete wrapped[0];
      return result(wrapped, this._chain);
    };
  });

  // Add all accessor Array functions to the wrapper.
  each(['concat', 'join', 'slice'], function(name) {
    var method = ArrayProto[name];
    wrapper.prototype[name] = function() {
      return result(method.apply(this._wrapped, arguments), this._chain);
    };
  });

  // Start chaining a wrapped Underscore object.
  wrapper.prototype.chain = function() {
    this._chain = true;
    return this;
  };

  // Extracts the result from a wrapped and chained object.
  wrapper.prototype.value = function() {
    return this._wrapped;
  };

  // AMD define happens at the end for compatibility with AMD loaders
  // that don't enforce next-turn semantics on modules.
  if (typeof define === 'function' && define.amd) {
    define('underscore',[], function() {
      return _;
    });
  }

}).call(this);

//     Backbone.js 0.9.2

//     (c) 2010-2012 Jeremy Ashkenas, DocumentCloud Inc.
//     Backbone may be freely distributed under the MIT license.
//     For all details and documentation:
//     http://backbonejs.org

(function(root, factory) {
  // Set up Backbone appropriately for the environment.
  if (typeof exports !== 'undefined') {
    // Node/CommonJS, no need for jQuery in that case.
    factory(root, exports, require('underscore'));
  } else if (typeof define === 'function' && define.amd) {
    // AMD
    define('backbone',['underscore', 'jquery', 'exports'], function(_, $, exports) {
      // Export global even in AMD case in case this script is loaded with
      // others that may still expect a global Backbone.
      root.Backbone = factory(root, exports, _, $);
    });
  } else {
    // Browser globals
    root.Backbone = factory(root, {}, root._, (root.jQuery || root.Zepto || root.ender));
  }
}(this, function(root, Backbone, _, $) {

  // Initial Setup
  // -------------

  // Save the previous value of the `Backbone` variable, so that it can be
  // restored later on, if `noConflict` is used.
  var previousBackbone = root.Backbone;

  // Create a local reference to slice/splice.
  var slice = Array.prototype.slice;
  var splice = Array.prototype.splice;

  // Current version of the library. Keep in sync with `package.json`.
  Backbone.VERSION = '0.9.2';

  // Set the JavaScript library that will be used for DOM manipulation and
  // Ajax calls (a.k.a. the `$` variable). By default Backbone will use: jQuery,
  // Zepto, or Ender; but the `setDomLibrary()` method lets you inject an
  // alternate JavaScript library (or a mock library for testing your views
  // outside of a browser).
  Backbone.setDomLibrary = function(lib) {
    $ = lib;
  };

  // Runs Backbone.js in *noConflict* mode, returning the `Backbone` variable
  // to its previous owner. Returns a reference to this Backbone object.
  Backbone.noConflict = function() {
    root.Backbone = previousBackbone;
    return Backbone;
  };

  // Turn on `emulateHTTP` to support legacy HTTP servers. Setting this option
  // will fake `"PUT"` and `"DELETE"` requests via the `_method` parameter and
  // set a `X-Http-Method-Override` header.
  Backbone.emulateHTTP = false;

  // Turn on `emulateJSON` to support legacy servers that can't deal with direct
  // `application/json` requests ... will encode the body as
  // `application/x-www-form-urlencoded` instead and will send the model in a
  // form param named `model`.
  Backbone.emulateJSON = false;

  // Backbone.Events
  // -----------------

  // Regular expression used to split event strings
  var eventSplitter = /\s+/;

  // A module that can be mixed in to *any object* in order to provide it with
  // custom events. You may bind with `on` or remove with `off` callback functions
  // to an event; trigger`-ing an event fires all callbacks in succession.
  //
  //     var object = {};
  //     _.extend(object, Backbone.Events);
  //     object.on('expand', function(){ alert('expanded'); });
  //     object.trigger('expand');
  //
  var Events = Backbone.Events = {

    // Bind one or more space separated events, `events`, to a `callback`
    // function. Passing `"all"` will bind the callback to all events fired.
    on: function(events, callback, context) {

      var calls, event, node, tail, list;
      if (!callback) return this;
      events = events.split(eventSplitter);
      calls = this._callbacks || (this._callbacks = {});

      // Create an immutable callback list, allowing traversal during
      // modification.  The tail is an empty object that will always be used
      // as the next node.
      while (event = events.shift()) {
        list = calls[event];
        node = list ? list.tail : {};
        node.next = tail = {};
        node.context = context;
        node.callback = callback;
        calls[event] = {tail: tail, next: list ? list.next : node};
      }

      return this;
    },

    // Remove one or many callbacks. If `context` is null, removes all callbacks
    // with that function. If `callback` is null, removes all callbacks for the
    // event. If `events` is null, removes all bound callbacks for all events.
    off: function(events, callback, context) {
      var event, calls, node, tail, cb, ctx;

      // No events, or removing *all* events.
      if (!(calls = this._callbacks)) return;
      if (!(events || callback || context)) {
        delete this._callbacks;
        return this;
      }

      // Loop through the listed events and contexts, splicing them out of the
      // linked list of callbacks if appropriate.
      events = events ? events.split(eventSplitter) : _.keys(calls);
      while (event = events.shift()) {
        node = calls[event];
        delete calls[event];
        if (!node || !(callback || context)) continue;
        // Create a new list, omitting the indicated callbacks.
        tail = node.tail;
        while ((node = node.next) !== tail) {
          cb = node.callback;
          ctx = node.context;
          if ((callback && cb !== callback) || (context && ctx !== context)) {
            this.on(event, cb, ctx);
          }
        }
      }

      return this;
    },

    // Trigger one or many events, firing all bound callbacks. Callbacks are
    // passed the same arguments as `trigger` is, apart from the event name
    // (unless you're listening on `"all"`, which will cause your callback to
    // receive the true name of the event as the first argument).
    trigger: function(events) {
      var event, node, calls, tail, args, all, rest;
      if (!(calls = this._callbacks)) return this;
      all = calls.all;
      events = events.split(eventSplitter);
      rest = slice.call(arguments, 1);

      // For each event, walk through the linked list of callbacks twice,
      // first to trigger the event, then to trigger any `"all"` callbacks.
      while (event = events.shift()) {
        if (node = calls[event]) {
          tail = node.tail;
          while ((node = node.next) !== tail) {
            node.callback.apply(node.context || this, rest);
          }
        }
        if (node = all) {
          tail = node.tail;
          args = [event].concat(rest);
          while ((node = node.next) !== tail) {
            node.callback.apply(node.context || this, args);
          }
        }
      }

      return this;
    }

  };

  // Aliases for backwards compatibility.
  Events.bind   = Events.on;
  Events.unbind = Events.off;

  // Backbone.Model
  // --------------

  // Create a new model, with defined attributes. A client id (`cid`)
  // is automatically generated and assigned for you.
  var Model = Backbone.Model = function(attributes, options) {
    var defaults;
    attributes || (attributes = {});
    if (options && options.parse) attributes = this.parse(attributes);
    if (defaults = getValue(this, 'defaults')) {
      attributes = _.extend({}, defaults, attributes);
    }
    if (options && options.collection) this.collection = options.collection;
    this.attributes = {};
    this._escapedAttributes = {};
    this.cid = _.uniqueId('c');
    this.changed = {};
    this._silent = {};
    this._pending = {};
    this.set(attributes, {silent: true});
    // Reset change tracking.
    this.changed = {};
    this._silent = {};
    this._pending = {};
    this._previousAttributes = _.clone(this.attributes);
    this.initialize.apply(this, arguments);
  };

  // Attach all inheritable methods to the Model prototype.
  _.extend(Model.prototype, Events, {

    // A hash of attributes whose current and previous value differ.
    changed: null,

    // A hash of attributes that have silently changed since the last time
    // `change` was called.  Will become pending attributes on the next call.
    _silent: null,

    // A hash of attributes that have changed since the last `'change'` event
    // began.
    _pending: null,

    // The default name for the JSON `id` attribute is `"id"`. MongoDB and
    // CouchDB users may want to set this to `"_id"`.
    idAttribute: 'id',

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // Return a copy of the model's `attributes` object.
    toJSON: function(options) {
      return _.clone(this.attributes);
    },

    // Get the value of an attribute.
    get: function(attr) {
      return this.attributes[attr];
    },

    // Get the HTML-escaped value of an attribute.
    escape: function(attr) {
      var html;
      if (html = this._escapedAttributes[attr]) return html;
      var val = this.get(attr);
      return this._escapedAttributes[attr] = _.escape(val == null ? '' : '' + val);
    },

    // Returns `true` if the attribute contains a value that is not null
    // or undefined.
    has: function(attr) {
      return this.get(attr) != null;
    },

    // Set a hash of model attributes on the object, firing `"change"` unless
    // you choose to silence it.
    set: function(key, value, options) {
      var attrs, attr, val;

      // Handle both `"key", value` and `{key: value}` -style arguments.
      if (_.isObject(key) || key == null) {
        attrs = key;
        options = value;
      } else {
        attrs = {};
        attrs[key] = value;
      }

      // Extract attributes and options.
      options || (options = {});
      if (!attrs) return this;
      if (attrs instanceof Model) attrs = attrs.attributes;
      if (options.unset) for (attr in attrs) attrs[attr] = void 0;

      // Run validation.
      if (!this._validate(attrs, options)) return false;

      // Check for changes of `id`.
      if (this.idAttribute in attrs) this.id = attrs[this.idAttribute];

      var changes = options.changes = {};
      var now = this.attributes;
      var escaped = this._escapedAttributes;
      var prev = this._previousAttributes || {};

      // For each `set` attribute...
      for (attr in attrs) {
        val = attrs[attr];

        // If the new and current value differ, record the change.
        if (!_.isEqual(now[attr], val) || (options.unset && _.has(now, attr))) {
          delete escaped[attr];
          (options.silent ? this._silent : changes)[attr] = true;
        }

        // Update or delete the current value.
        options.unset ? delete now[attr] : now[attr] = val;

        // If the new and previous value differ, record the change.  If not,
        // then remove changes for this attribute.
        if (!_.isEqual(prev[attr], val) || (_.has(now, attr) != _.has(prev, attr))) {
          this.changed[attr] = val;
          if (!options.silent) this._pending[attr] = true;
        } else {
          delete this.changed[attr];
          delete this._pending[attr];
        }
      }

      // Fire the `"change"` events.
      if (!options.silent) this.change(options);
      return this;
    },

    // Remove an attribute from the model, firing `"change"` unless you choose
    // to silence it. `unset` is a noop if the attribute doesn't exist.
    unset: function(attr, options) {
      (options || (options = {})).unset = true;
      return this.set(attr, null, options);
    },

    // Clear all attributes on the model, firing `"change"` unless you choose
    // to silence it.
    clear: function(options) {
      (options || (options = {})).unset = true;
      return this.set(_.clone(this.attributes), options);
    },

    // Fetch the model from the server. If the server's representation of the
    // model differs from its current attributes, they will be overriden,
    // triggering a `"change"` event.
    fetch: function(options) {
      options = options ? _.clone(options) : {};
      var model = this;
      var success = options.success;
      options.success = function(resp, status, xhr) {
        if (!model.set(model.parse(resp, xhr), options)) return false;
        if (success) success(model, resp);
      };
      options.error = Backbone.wrapError(options.error, model, options);
      return (this.sync || Backbone.sync).call(this, 'read', this, options);
    },

    // Set a hash of model attributes, and sync the model to the server.
    // If the server returns an attributes hash that differs, the model's
    // state will be `set` again.
    save: function(key, value, options) {
      var attrs, current;

      // Handle both `("key", value)` and `({key: value})` -style calls.
      if (_.isObject(key) || key == null) {
        attrs = key;
        options = value;
      } else {
        attrs = {};
        attrs[key] = value;
      }
      options = options ? _.clone(options) : {};

      // If we're "wait"-ing to set changed attributes, validate early.
      if (options.wait) {
        if (!this._validate(attrs, options)) return false;
        current = _.clone(this.attributes);
      }

      // Regular saves `set` attributes before persisting to the server.
      var silentOptions = _.extend({}, options, {silent: true});
      if (attrs && !this.set(attrs, options.wait ? silentOptions : options)) {
        return false;
      }

      // After a successful server-side save, the client is (optionally)
      // updated with the server-side state.
      var model = this;
      var success = options.success;
      options.success = function(resp, status, xhr) {
        var serverAttrs = model.parse(resp, xhr);
        if (options.wait) {
          delete options.wait;
          serverAttrs = _.extend(attrs || {}, serverAttrs);
        }
        if (!model.set(serverAttrs, options)) return false;
        if (success) {
          success(model, resp);
        } else {
          model.trigger('sync', model, resp, options);
        }
      };

      // Finish configuring and sending the Ajax request.
      options.error = Backbone.wrapError(options.error, model, options);
      var method = this.isNew() ? 'create' : 'update';
      var xhr = (this.sync || Backbone.sync).call(this, method, this, options);
      if (options.wait) this.set(current, silentOptions);
      return xhr;
    },

    // Destroy this model on the server if it was already persisted.
    // Optimistically removes the model from its collection, if it has one.
    // If `wait: true` is passed, waits for the server to respond before removal.
    destroy: function(options) {
      options = options ? _.clone(options) : {};
      var model = this;
      var success = options.success;

      var triggerDestroy = function() {
        model.trigger('destroy', model, model.collection, options);
      };

      if (this.isNew()) {
        triggerDestroy();
        return false;
      }

      options.success = function(resp) {
        if (options.wait) triggerDestroy();
        if (success) {
          success(model, resp);
        } else {
          model.trigger('sync', model, resp, options);
        }
      };

      options.error = Backbone.wrapError(options.error, model, options);
      var xhr = (this.sync || Backbone.sync).call(this, 'delete', this, options);
      if (!options.wait) triggerDestroy();
      return xhr;
    },

    // Default URL for the model's representation on the server -- if you're
    // using Backbone's restful methods, override this to change the endpoint
    // that will be called.
    url: function() {
      var base = getValue(this, 'urlRoot') || getValue(this.collection, 'url') || urlError();
      if (this.isNew()) return base;
      return base + (base.charAt(base.length - 1) == '/' ? '' : '/') + encodeURIComponent(this.id);
    },

    // **parse** converts a response into the hash of attributes to be `set` on
    // the model. The default implementation is just to pass the response along.
    parse: function(resp, xhr) {
      return resp;
    },

    // Create a new model with identical attributes to this one.
    clone: function() {
      return new this.constructor(this.attributes);
    },

    // A model is new if it has never been saved to the server, and lacks an id.
    isNew: function() {
      return this.id == null;
    },

    // Call this method to manually fire a `"change"` event for this model and
    // a `"change:attribute"` event for each changed attribute.
    // Calling this will cause all objects observing the model to update.
    change: function(options) {
      options || (options = {});
      var changing = this._changing;
      this._changing = true;

      // Silent changes become pending changes.
      for (var attr in this._silent) this._pending[attr] = true;

      // Silent changes are triggered.
      var changes = _.extend({}, options.changes, this._silent);
      this._silent = {};
      for (var attr in changes) {
        this.trigger('change:' + attr, this, this.get(attr), options);
      }
      if (changing) return this;

      // Continue firing `"change"` events while there are pending changes.
      while (!_.isEmpty(this._pending)) {
        this._pending = {};
        this.trigger('change', this, options);
        // Pending and silent changes still remain.
        for (var attr in this.changed) {
          if (this._pending[attr] || this._silent[attr]) continue;
          delete this.changed[attr];
        }
        this._previousAttributes = _.clone(this.attributes);
      }

      this._changing = false;
      return this;
    },

    // Determine if the model has changed since the last `"change"` event.
    // If you specify an attribute name, determine if that attribute has changed.
    hasChanged: function(attr) {
      if (!arguments.length) return !_.isEmpty(this.changed);
      return _.has(this.changed, attr);
    },

    // Return an object containing all the attributes that have changed, or
    // false if there are no changed attributes. Useful for determining what
    // parts of a view need to be updated and/or what attributes need to be
    // persisted to the server. Unset attributes will be set to undefined.
    // You can also pass an attributes object to diff against the model,
    // determining if there *would be* a change.
    changedAttributes: function(diff) {
      if (!diff) return this.hasChanged() ? _.clone(this.changed) : false;
      var val, changed = false, old = this._previousAttributes;
      for (var attr in diff) {
        if (_.isEqual(old[attr], (val = diff[attr]))) continue;
        (changed || (changed = {}))[attr] = val;
      }
      return changed;
    },

    // Get the previous value of an attribute, recorded at the time the last
    // `"change"` event was fired.
    previous: function(attr) {
      if (!arguments.length || !this._previousAttributes) return null;
      return this._previousAttributes[attr];
    },

    // Get all of the attributes of the model at the time of the previous
    // `"change"` event.
    previousAttributes: function() {
      return _.clone(this._previousAttributes);
    },

    // Check if the model is currently in a valid state. It's only possible to
    // get into an *invalid* state if you're using silent changes.
    isValid: function() {
      return !this.validate(this.attributes);
    },

    // Run validation against the next complete set of model attributes,
    // returning `true` if all is well. If a specific `error` callback has
    // been passed, call that instead of firing the general `"error"` event.
    _validate: function(attrs, options) {
      if (options.silent || !this.validate) return true;
      attrs = _.extend({}, this.attributes, attrs);
      var error = this.validate(attrs, options);
      if (!error) return true;
      if (options && options.error) {
        options.error(this, error, options);
      } else {
        this.trigger('error', this, error, options);
      }
      return false;
    }

  });

  // Backbone.Collection
  // -------------------

  // Provides a standard collection class for our sets of models, ordered
  // or unordered. If a `comparator` is specified, the Collection will maintain
  // its models in sort order, as they're added and removed.
  var Collection = Backbone.Collection = function(models, options) {
    options || (options = {});
    if (options.model) this.model = options.model;
    if (options.comparator) this.comparator = options.comparator;
    this._reset();
    this.initialize.apply(this, arguments);
    if (models) this.reset(models, {silent: true, parse: options.parse});
  };

  // Define the Collection's inheritable methods.
  _.extend(Collection.prototype, Events, {

    // The default model for a collection is just a **Backbone.Model**.
    // This should be overridden in most cases.
    model: Model,

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // The JSON representation of a Collection is an array of the
    // models' attributes.
    toJSON: function(options) {
      return this.map(function(model){ return model.toJSON(options); });
    },

    // Add a model, or list of models to the set. Pass **silent** to avoid
    // firing the `add` event for every new model.
    add: function(models, options) {
      var i, index, length, model, cid, id, cids = {}, ids = {}, dups = [];
      options || (options = {});
      models = _.isArray(models) ? models.slice() : [models];

      // Begin by turning bare objects into model references, and preventing
      // invalid models or duplicate models from being added.
      for (i = 0, length = models.length; i < length; i++) {
        if (!(model = models[i] = this._prepareModel(models[i], options))) {
          throw new Error("Can't add an invalid model to a collection");
        }
        cid = model.cid;
        id = model.id;
        if (cids[cid] || this._byCid[cid] || ((id != null) && (ids[id] || this._byId[id]))) {
          dups.push(i);
          continue;
        }
        cids[cid] = ids[id] = model;
      }

      // Remove duplicates.
      i = dups.length;
      while (i--) {
        models.splice(dups[i], 1);
      }

      // Listen to added models' events, and index models for lookup by
      // `id` and by `cid`.
      for (i = 0, length = models.length; i < length; i++) {
        (model = models[i]).on('all', this._onModelEvent, this);
        this._byCid[model.cid] = model;
        if (model.id != null) this._byId[model.id] = model;
      }

      // Insert models into the collection, re-sorting if needed, and triggering
      // `add` events unless silenced.
      this.length += length;
      index = options.at != null ? options.at : this.models.length;
      splice.apply(this.models, [index, 0].concat(models));
      if (this.comparator) this.sort({silent: true});
      if (options.silent) return this;
      for (i = 0, length = this.models.length; i < length; i++) {
        if (!cids[(model = this.models[i]).cid]) continue;
        options.index = i;
        model.trigger('add', model, this, options);
      }
      return this;
    },

    // Remove a model, or a list of models from the set. Pass silent to avoid
    // firing the `remove` event for every model removed.
    remove: function(models, options) {
      var i, l, index, model;
      options || (options = {});
      models = _.isArray(models) ? models.slice() : [models];
      for (i = 0, l = models.length; i < l; i++) {
        model = this.getByCid(models[i]) || this.get(models[i]);
        if (!model) continue;
        delete this._byId[model.id];
        delete this._byCid[model.cid];
        index = this.indexOf(model);
        this.models.splice(index, 1);
        this.length--;
        if (!options.silent) {
          options.index = index;
          model.trigger('remove', model, this, options);
        }
        this._removeReference(model);
      }
      return this;
    },

    // Add a model to the end of the collection.
    push: function(model, options) {
      model = this._prepareModel(model, options);
      this.add(model, options);
      return model;
    },

    // Remove a model from the end of the collection.
    pop: function(options) {
      var model = this.at(this.length - 1);
      this.remove(model, options);
      return model;
    },

    // Add a model to the beginning of the collection.
    unshift: function(model, options) {
      model = this._prepareModel(model, options);
      this.add(model, _.extend({at: 0}, options));
      return model;
    },

    // Remove a model from the beginning of the collection.
    shift: function(options) {
      var model = this.at(0);
      this.remove(model, options);
      return model;
    },

    // Get a model from the set by id.
    get: function(id) {
      if (id == null) return void 0;
      return this._byId[id.id != null ? id.id : id];
    },

    // Get a model from the set by client id.
    getByCid: function(cid) {
      return cid && this._byCid[cid.cid || cid];
    },

    // Get the model at the given index.
    at: function(index) {
      return this.models[index];
    },

    // Return models with matching attributes. Useful for simple cases of `filter`.
    where: function(attrs) {
      if (_.isEmpty(attrs)) return [];
      return this.filter(function(model) {
        for (var key in attrs) {
          if (attrs[key] !== model.get(key)) return false;
        }
        return true;
      });
    },

    // Force the collection to re-sort itself. You don't need to call this under
    // normal circumstances, as the set will maintain sort order as each item
    // is added.
    sort: function(options) {
      options || (options = {});
      if (!this.comparator) throw new Error('Cannot sort a set without a comparator');
      var boundComparator = _.bind(this.comparator, this);
      if (this.comparator.length == 1) {
        this.models = this.sortBy(boundComparator);
      } else {
        this.models.sort(boundComparator);
      }
      if (!options.silent) this.trigger('reset', this, options);
      return this;
    },

    // Pluck an attribute from each model in the collection.
    pluck: function(attr) {
      return _.map(this.models, function(model){ return model.get(attr); });
    },

    // When you have more items than you want to add or remove individually,
    // you can reset the entire set with a new list of models, without firing
    // any `add` or `remove` events. Fires `reset` when finished.
    reset: function(models, options) {
      models  || (models = []);
      options || (options = {});
      for (var i = 0, l = this.models.length; i < l; i++) {
        this._removeReference(this.models[i]);
      }
      this._reset();
      this.add(models, _.extend({silent: true}, options));
      if (!options.silent) this.trigger('reset', this, options);
      return this;
    },

    // Fetch the default set of models for this collection, resetting the
    // collection when they arrive. If `add: true` is passed, appends the
    // models to the collection instead of resetting.
    fetch: function(options) {
      options = options ? _.clone(options) : {};
      if (options.parse === undefined) options.parse = true;
      var collection = this;
      var success = options.success;
      options.success = function(resp, status, xhr) {
        collection[options.add ? 'add' : 'reset'](collection.parse(resp, xhr), options);
        if (success) success(collection, resp);
      };
      options.error = Backbone.wrapError(options.error, collection, options);
      return (this.sync || Backbone.sync).call(this, 'read', this, options);
    },

    // Create a new instance of a model in this collection. Add the model to the
    // collection immediately, unless `wait: true` is passed, in which case we
    // wait for the server to agree.
    create: function(model, options) {
      var coll = this;
      options = options ? _.clone(options) : {};
      model = this._prepareModel(model, options);
      if (!model) return false;
      if (!options.wait) coll.add(model, options);
      var success = options.success;
      options.success = function(nextModel, resp, xhr) {
        if (options.wait) coll.add(nextModel, options);
        if (success) {
          success(nextModel, resp);
        } else {
          nextModel.trigger('sync', model, resp, options);
        }
      };
      model.save(null, options);
      return model;
    },

    // **parse** converts a response into a list of models to be added to the
    // collection. The default implementation is just to pass it through.
    parse: function(resp, xhr) {
      return resp;
    },

    // Proxy to _'s chain. Can't be proxied the same way the rest of the
    // underscore methods are proxied because it relies on the underscore
    // constructor.
    chain: function () {
      return _(this.models).chain();
    },

    // Reset all internal state. Called when the collection is reset.
    _reset: function(options) {
      this.length = 0;
      this.models = [];
      this._byId  = {};
      this._byCid = {};
    },

    // Prepare a model or hash of attributes to be added to this collection.
    _prepareModel: function(model, options) {
      options || (options = {});
      if (!(model instanceof Model)) {
        var attrs = model;
        options.collection = this;
        model = new this.model(attrs, options);
        if (!model._validate(model.attributes, options)) model = false;
      } else if (!model.collection) {
        model.collection = this;
      }
      return model;
    },

    // Internal method to remove a model's ties to a collection.
    _removeReference: function(model) {
      if (this == model.collection) {
        delete model.collection;
      }
      model.off('all', this._onModelEvent, this);
    },

    // Internal method called every time a model in the set fires an event.
    // Sets need to update their indexes when models change ids. All other
    // events simply proxy through. "add" and "remove" events that originate
    // in other collections are ignored.
    _onModelEvent: function(event, model, collection, options) {
      if ((event == 'add' || event == 'remove') && collection != this) return;
      if (event == 'destroy') {
        this.remove(model, options);
      }
      if (model && event === 'change:' + model.idAttribute) {
        delete this._byId[model.previous(model.idAttribute)];
        this._byId[model.id] = model;
      }
      this.trigger.apply(this, arguments);
    }

  });

  // Underscore methods that we want to implement on the Collection.
  var methods = ['forEach', 'each', 'map', 'reduce', 'reduceRight', 'find',
    'detect', 'filter', 'select', 'reject', 'every', 'all', 'some', 'any',
    'include', 'contains', 'invoke', 'max', 'min', 'sortBy', 'sortedIndex',
    'toArray', 'size', 'first', 'initial', 'rest', 'last', 'without', 'indexOf',
    'shuffle', 'lastIndexOf', 'isEmpty', 'groupBy'];

  // Mix in each Underscore method as a proxy to `Collection#models`.
  _.each(methods, function(method) {
    Collection.prototype[method] = function() {
      return _[method].apply(_, [this.models].concat(_.toArray(arguments)));
    };
  });

  // Backbone.Router
  // -------------------

  // Routers map faux-URLs to actions, and fire events when routes are
  // matched. Creating a new one sets its `routes` hash, if not set statically.
  var Router = Backbone.Router = function(options) {
    options || (options = {});
    if (options.routes) this.routes = options.routes;
    this._bindRoutes();
    this.initialize.apply(this, arguments);
  };

  // Cached regular expressions for matching named param parts and splatted
  // parts of route strings.
  var namedParam    = /:\w+/g;
  var splatParam    = /\*\w+/g;
  var escapeRegExp  = /[-[\]{}()+?.,\\^$|#\s]/g;

  // Set up all inheritable **Backbone.Router** properties and methods.
  _.extend(Router.prototype, Events, {

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // Manually bind a single named route to a callback. For example:
    //
    //     this.route('search/:query/p:num', 'search', function(query, num) {
    //       ...
    //     });
    //
    route: function(route, name, callback) {
      Backbone.history || (Backbone.history = new History);
      if (!_.isRegExp(route)) route = this._routeToRegExp(route);
      if (!callback) callback = this[name];
      Backbone.history.route(route, _.bind(function(fragment) {
        var args = this._extractParameters(route, fragment);
        callback && callback.apply(this, args);
        this.trigger.apply(this, ['route:' + name].concat(args));
        Backbone.history.trigger('route', this, name, args);
      }, this));
      return this;
    },

    // Simple proxy to `Backbone.history` to save a fragment into the history.
    navigate: function(fragment, options) {
      Backbone.history.navigate(fragment, options);
    },

    // Bind all defined routes to `Backbone.history`. We have to reverse the
    // order of the routes here to support behavior where the most general
    // routes can be defined at the bottom of the route map.
    _bindRoutes: function() {
      if (!this.routes) return;
      var routes = [];
      for (var route in this.routes) {
        routes.unshift([route, this.routes[route]]);
      }
      for (var i = 0, l = routes.length; i < l; i++) {
        this.route(routes[i][0], routes[i][1], this[routes[i][1]]);
      }
    },

    // Convert a route string into a regular expression, suitable for matching
    // against the current location hash.
    _routeToRegExp: function(route) {
      route = route.replace(escapeRegExp, '\\$&')
                   .replace(namedParam, '([^\/]+)')
                   .replace(splatParam, '(.*?)');
      return new RegExp('^' + route + '$');
    },

    // Given a route, and a URL fragment that it matches, return the array of
    // extracted parameters.
    _extractParameters: function(route, fragment) {
      return route.exec(fragment).slice(1);
    }

  });

  // Backbone.History
  // ----------------

  // Handles cross-browser history management, based on URL fragments. If the
  // browser does not support `onhashchange`, falls back to polling.
  var History = Backbone.History = function() {
    this.handlers = [];
    _.bindAll(this, 'checkUrl');
  };

  // Cached regex for cleaning leading hashes and slashes .
  var routeStripper = /^[#\/]/;

  // Cached regex for detecting MSIE.
  var isExplorer = /msie [\w.]+/;

  // Has the history handling already been started?
  History.started = false;

  // Set up all inheritable **Backbone.History** properties and methods.
  _.extend(History.prototype, Events, {

    // The default interval to poll for hash changes, if necessary, is
    // twenty times a second.
    interval: 50,

    // Gets the true hash value. Cannot use location.hash directly due to bug
    // in Firefox where location.hash will always be decoded.
    getHash: function(windowOverride) {
      var loc = windowOverride ? windowOverride.location : window.location;
      var match = loc.href.match(/#(.*)$/);
      return match ? match[1] : '';
    },

    // Get the cross-browser normalized URL fragment, either from the URL,
    // the hash, or the override.
    getFragment: function(fragment, forcePushState) {
      if (fragment == null) {
        if (this._hasPushState || forcePushState) {
          fragment = window.location.pathname;
          var search = window.location.search;
          if (search) fragment += search;
        } else {
          fragment = this.getHash();
        }
      }
      if (!fragment.indexOf(this.options.root)) fragment = fragment.substr(this.options.root.length);
      return fragment.replace(routeStripper, '');
    },

    // Start the hash change handling, returning `true` if the current URL matches
    // an existing route, and `false` otherwise.
    start: function(options) {
      if (History.started) throw new Error("Backbone.history has already been started");
      History.started = true;

      // Figure out the initial configuration. Do we need an iframe?
      // Is pushState desired ... is it available?
      this.options          = _.extend({}, {root: '/'}, this.options, options);
      this._wantsHashChange = this.options.hashChange !== false;
      this._wantsPushState  = !!this.options.pushState;
      this._hasPushState    = !!(this.options.pushState && window.history && window.history.pushState);
      var fragment          = this.getFragment();
      var docMode           = document.documentMode;
      var oldIE             = (isExplorer.exec(navigator.userAgent.toLowerCase()) && (!docMode || docMode <= 7));

      if (oldIE) {
        this.iframe = $('<iframe src="javascript:0" tabindex="-1" />').hide().appendTo('body')[0].contentWindow;
        this.navigate(fragment);
      }

      // Depending on whether we're using pushState or hashes, and whether
      // 'onhashchange' is supported, determine how we check the URL state.
      if (this._hasPushState) {
        $(window).bind('popstate', this.checkUrl);
      } else if (this._wantsHashChange && ('onhashchange' in window) && !oldIE) {
        $(window).bind('hashchange', this.checkUrl);
      } else if (this._wantsHashChange) {
        this._checkUrlInterval = setInterval(this.checkUrl, this.interval);
      }

      // Determine if we need to change the base url, for a pushState link
      // opened by a non-pushState browser.
      this.fragment = fragment;
      var loc = window.location;
      var atRoot  = loc.pathname == this.options.root;

      // If we've started off with a route from a `pushState`-enabled browser,
      // but we're currently in a browser that doesn't support it...
      if (this._wantsHashChange && this._wantsPushState && !this._hasPushState && !atRoot) {
        this.fragment = this.getFragment(null, true);
        window.location.replace(this.options.root + '#' + this.fragment);
        // Return immediately as browser will do redirect to new url
        return true;

      // Or if we've started out with a hash-based route, but we're currently
      // in a browser where it could be `pushState`-based instead...
      } else if (this._wantsPushState && this._hasPushState && atRoot && loc.hash) {
        this.fragment = this.getHash().replace(routeStripper, '');
        window.history.replaceState({}, document.title, loc.protocol + '//' + loc.host + this.options.root + this.fragment);
      }

      if (!this.options.silent) {
        return this.loadUrl();
      }
    },

    // Disable Backbone.history, perhaps temporarily. Not useful in a real app,
    // but possibly useful for unit testing Routers.
    stop: function() {
      $(window).unbind('popstate', this.checkUrl).unbind('hashchange', this.checkUrl);
      clearInterval(this._checkUrlInterval);
      History.started = false;
    },

    // Add a route to be tested when the fragment changes. Routes added later
    // may override previous routes.
    route: function(route, callback) {
      this.handlers.unshift({route: route, callback: callback});
    },

    // Checks the current URL to see if it has changed, and if it has,
    // calls `loadUrl`, normalizing across the hidden iframe.
    checkUrl: function(e) {
      var current = this.getFragment();
      if (current == this.fragment && this.iframe) current = this.getFragment(this.getHash(this.iframe));
      if (current == this.fragment) return false;
      if (this.iframe) this.navigate(current);
      this.loadUrl() || this.loadUrl(this.getHash());
    },

    // Attempt to load the current URL fragment. If a route succeeds with a
    // match, returns `true`. If no defined routes matches the fragment,
    // returns `false`.
    loadUrl: function(fragmentOverride) {
      var fragment = this.fragment = this.getFragment(fragmentOverride);
      var matched = _.any(this.handlers, function(handler) {
        if (handler.route.test(fragment)) {
          handler.callback(fragment);
          return true;
        }
      });
      return matched;
    },

    // Save a fragment into the hash history, or replace the URL state if the
    // 'replace' option is passed. You are responsible for properly URL-encoding
    // the fragment in advance.
    //
    // The options object can contain `trigger: true` if you wish to have the
    // route callback be fired (not usually desirable), or `replace: true`, if
    // you wish to modify the current URL without adding an entry to the history.
    navigate: function(fragment, options) {
      if (!History.started) return false;
      if (!options || options === true) options = {trigger: options};
      var frag = (fragment || '').replace(routeStripper, '');
      if (this.fragment == frag) return;

      // If pushState is available, we use it to set the fragment as a real URL.
      if (this._hasPushState) {
        if (frag.indexOf(this.options.root) != 0) frag = this.options.root + frag;
        this.fragment = frag;
        window.history[options.replace ? 'replaceState' : 'pushState']({}, document.title, frag);

      // If hash changes haven't been explicitly disabled, update the hash
      // fragment to store history.
      } else if (this._wantsHashChange) {
        this.fragment = frag;
        this._updateHash(window.location, frag, options.replace);
        if (this.iframe && (frag != this.getFragment(this.getHash(this.iframe)))) {
          // Opening and closing the iframe tricks IE7 and earlier to push a history entry on hash-tag change.
          // When replace is true, we don't want this.
          if(!options.replace) this.iframe.document.open().close();
          this._updateHash(this.iframe.location, frag, options.replace);
        }

      // If you've told us that you explicitly don't want fallback hashchange-
      // based history, then `navigate` becomes a page refresh.
      } else {
        window.location.assign(this.options.root + fragment);
      }
      if (options.trigger) this.loadUrl(fragment);
    },

    // Update the hash location, either replacing the current entry, or adding
    // a new one to the browser history.
    _updateHash: function(location, fragment, replace) {
      if (replace) {
        location.replace(location.toString().replace(/(javascript:|#).*$/, '') + '#' + fragment);
      } else {
        location.hash = fragment;
      }
    }
  });

  // Backbone.View
  // -------------

  // Creating a Backbone.View creates its initial element outside of the DOM,
  // if an existing element is not provided...
  var View = Backbone.View = function(options) {
    this.cid = _.uniqueId('view');
    this._configure(options || {});
    this._ensureElement();
    this.initialize.apply(this, arguments);
    this.delegateEvents();
  };

  // Cached regex to split keys for `delegate`.
  var delegateEventSplitter = /^(\S+)\s*(.*)$/;

  // List of view options to be merged as properties.
  var viewOptions = ['model', 'collection', 'el', 'id', 'attributes', 'className', 'tagName'];

  // Set up all inheritable **Backbone.View** properties and methods.
  _.extend(View.prototype, Events, {

    // The default `tagName` of a View's element is `"div"`.
    tagName: 'div',

    // jQuery delegate for element lookup, scoped to DOM elements within the
    // current view. This should be prefered to global lookups where possible.
    $: function(selector) {
      return this.$el.find(selector);
    },

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // **render** is the core function that your view should override, in order
    // to populate its element (`this.el`), with the appropriate HTML. The
    // convention is for **render** to always return `this`.
    render: function() {
      return this;
    },

    // Remove this view from the DOM. Note that the view isn't present in the
    // DOM by default, so calling this method may be a no-op.
    remove: function() {
      this.$el.remove();
      return this;
    },

    // For small amounts of DOM Elements, where a full-blown template isn't
    // needed, use **make** to manufacture elements, one at a time.
    //
    //     var el = this.make('li', {'class': 'row'}, this.model.escape('title'));
    //
    make: function(tagName, attributes, content) {
      var el = document.createElement(tagName);
      if (attributes) $(el).attr(attributes);
      if (content != null) $(el).html(content);
      return el;
    },

    // Change the view's element (`this.el` property), including event
    // re-delegation.
    setElement: function(element, delegate) {
      if (this.$el) this.undelegateEvents();
      this.$el = (element instanceof $) ? element : $(element);
      this.el = this.$el[0];
      if (delegate !== false) this.delegateEvents();
      return this;
    },

    // Set callbacks, where `this.events` is a hash of
    //
    // *{"event selector": "callback"}*
    //
    //     {
    //       'mousedown .title':  'edit',
    //       'click .button':     'save'
    //       'click .open':       function(e) { ... }
    //     }
    //
    // pairs. Callbacks will be bound to the view, with `this` set properly.
    // Uses event delegation for efficiency.
    // Omitting the selector binds the event to `this.el`.
    // This only works for delegate-able events: not `focus`, `blur`, and
    // not `change`, `submit`, and `reset` in Internet Explorer.
    delegateEvents: function(events) {
      if (!(events || (events = getValue(this, 'events')))) return;
      this.undelegateEvents();
      for (var key in events) {
        var method = events[key];
        if (!_.isFunction(method)) method = this[events[key]];
        if (!method) throw new Error('Method "' + events[key] + '" does not exist');
        var match = key.match(delegateEventSplitter);
        var eventName = match[1], selector = match[2];
        method = _.bind(method, this);
        eventName += '.delegateEvents' + this.cid;
        if (selector === '') {
          this.$el.bind(eventName, method);
        } else {
          this.$el.delegate(selector, eventName, method);
        }
      }
    },

    // Clears all callbacks previously bound to the view with `delegateEvents`.
    // You usually don't need to use this, but may wish to if you have multiple
    // Backbone views attached to the same DOM element.
    undelegateEvents: function() {
      this.$el.unbind('.delegateEvents' + this.cid);
    },

    // Performs the initial configuration of a View with a set of options.
    // Keys with special meaning *(model, collection, id, className)*, are
    // attached directly to the view.
    _configure: function(options) {
      if (this.options) options = _.extend({}, this.options, options);
      for (var i = 0, l = viewOptions.length; i < l; i++) {
        var attr = viewOptions[i];
        if (options[attr]) this[attr] = options[attr];
      }
      this.options = options;
    },

    // Ensure that the View has a DOM element to render into.
    // If `this.el` is a string, pass it through `$()`, take the first
    // matching element, and re-assign it to `el`. Otherwise, create
    // an element from the `id`, `className` and `tagName` properties.
    _ensureElement: function() {
      if (!this.el) {
        var attrs = getValue(this, 'attributes') || {};
        if (this.id) attrs.id = this.id;
        if (this.className) attrs['class'] = this.className;
        this.setElement(this.make(this.tagName, attrs), false);
      } else {
        this.setElement(this.el, false);
      }
    }

  });

  // The self-propagating extend function that Backbone classes use.
  var extend = function (protoProps, classProps) {
    var child = inherits(this, protoProps, classProps);
    child.extend = this.extend;
    return child;
  };

  // Set up inheritance for the model, collection, and view.
  Model.extend = Collection.extend = Router.extend = View.extend = extend;

  // Backbone.sync
  // -------------

  // Map from CRUD to HTTP for our default `Backbone.sync` implementation.
  var methodMap = {
    'create': 'POST',
    'update': 'PUT',
    'delete': 'DELETE',
    'read':   'GET'
  };

  // Override this function to change the manner in which Backbone persists
  // models to the server. You will be passed the type of request, and the
  // model in question. By default, makes a RESTful Ajax request
  // to the model's `url()`. Some possible customizations could be:
  //
  // * Use `setTimeout` to batch rapid-fire updates into a single request.
  // * Send up the models as XML instead of JSON.
  // * Persist models via WebSockets instead of Ajax.
  //
  // Turn on `Backbone.emulateHTTP` in order to send `PUT` and `DELETE` requests
  // as `POST`, with a `_method` parameter containing the true HTTP method,
  // as well as all requests with the body as `application/x-www-form-urlencoded`
  // instead of `application/json` with the model in a param named `model`.
  // Useful when interfacing with server-side languages like **PHP** that make
  // it difficult to read the body of `PUT` requests.
  Backbone.sync = function(method, model, options) {
    var type = methodMap[method];

    // Default options, unless specified.
    options || (options = {});

    // Default JSON-request options.
    var params = {type: type, dataType: 'json'};

    // Ensure that we have a URL.
    if (!options.url) {
      params.url = getValue(model, 'url') || urlError();
    }

    // Ensure that we have the appropriate request data.
    if (!options.data && model && (method == 'create' || method == 'update')) {
      params.contentType = 'application/json';
      params.data = JSON.stringify(model.toJSON());
    }

    // For older servers, emulate JSON by encoding the request into an HTML-form.
    if (Backbone.emulateJSON) {
      params.contentType = 'application/x-www-form-urlencoded';
      params.data = params.data ? {model: params.data} : {};
    }

    // For older servers, emulate HTTP by mimicking the HTTP method with `_method`
    // And an `X-HTTP-Method-Override` header.
    if (Backbone.emulateHTTP) {
      if (type === 'PUT' || type === 'DELETE') {
        if (Backbone.emulateJSON) params.data._method = type;
        params.type = 'POST';
        params.beforeSend = function(xhr) {
          xhr.setRequestHeader('X-HTTP-Method-Override', type);
        };
      }
    }

    // Don't process data on a non-GET request.
    if (params.type !== 'GET' && !Backbone.emulateJSON) {
      params.processData = false;
    }

    // Make the request, allowing the user to override any Ajax options.
    return $.ajax(_.extend(params, options));
  };

  // Wrap an optional error callback with a fallback error event.
  Backbone.wrapError = function(onError, originalModel, options) {
    return function(model, resp) {
      resp = model === originalModel ? resp : model;
      if (onError) {
        onError(originalModel, resp, options);
      } else {
        originalModel.trigger('error', originalModel, resp, options);
      }
    };
  };

  // Helpers
  // -------

  // Shared empty constructor function to aid in prototype-chain creation.
  var ctor = function(){};

  // Helper function to correctly set up the prototype chain, for subclasses.
  // Similar to `goog.inherits`, but uses a hash of prototype properties and
  // class properties to be extended.
  var inherits = function(parent, protoProps, staticProps) {
    var child;

    // The constructor function for the new subclass is either defined by you
    // (the "constructor" property in your `extend` definition), or defaulted
    // by us to simply call the parent's constructor.
    if (protoProps && protoProps.hasOwnProperty('constructor')) {
      child = protoProps.constructor;
    } else {
      child = function(){ parent.apply(this, arguments); };
    }

    // Inherit class (static) properties from parent.
    _.extend(child, parent);

    // Set the prototype chain to inherit from `parent`, without calling
    // `parent`'s constructor function.
    ctor.prototype = parent.prototype;
    child.prototype = new ctor();

    // Add prototype properties (instance properties) to the subclass,
    // if supplied.
    if (protoProps) _.extend(child.prototype, protoProps);

    // Add static properties to the constructor function, if supplied.
    if (staticProps) _.extend(child, staticProps);

    // Correctly set child's `prototype.constructor`.
    child.prototype.constructor = child;

    // Set a convenience property in case the parent's prototype is needed later.
    child.__super__ = parent.prototype;

    return child;
  };

  // Helper function to get a value from a Backbone object as a property
  // or as a function.
  var getValue = function(object, prop) {
    if (!(object && object[prop])) return null;
    return _.isFunction(object[prop]) ? object[prop]() : object[prop];
  };

  // Throw an error when a URL is needed, and none is supplied.
  var urlError = function() {
    throw new Error('A "url" property or function must be specified');
  };

  return Backbone;
}));

/*!
ICanHaz.js version 0.10 -- by @HenrikJoreteg
More info at: http://icanhazjs.com
*/
(function () {
/*
  mustache.js — Logic-less templates in JavaScript

  See http://mustache.github.com/ for more info.
*/

var Mustache = function () {
  var _toString = Object.prototype.toString;

  Array.isArray = Array.isArray || function (obj) {
    return _toString.call(obj) == "[object Array]";
  }

  var _trim = String.prototype.trim, trim;

  if (_trim) {
    trim = function (text) {
      return text == null ? "" : _trim.call(text);
    }
  } else {
    var trimLeft, trimRight;

    // IE doesn't match non-breaking spaces with \s.
    if ((/\S/).test("\xA0")) {
      trimLeft = /^[\s\xA0]+/;
      trimRight = /[\s\xA0]+$/;
    } else {
      trimLeft = /^\s+/;
      trimRight = /\s+$/;
    }

    trim = function (text) {
      return text == null ? "" :
        text.toString().replace(trimLeft, "").replace(trimRight, "");
    }
  }

  var escapeMap = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': '&quot;',
    "'": '&#39;'
  };

  function escapeHTML(string) {
    return String(string).replace(/&(?!\w+;)|[<>"']/g, function (s) {
      return escapeMap[s] || s;
    });
  }

  var regexCache = {};
  var Renderer = function () {};

  Renderer.prototype = {
    otag: "{{",
    ctag: "}}",
    pragmas: {},
    buffer: [],
    pragmas_implemented: {
      "IMPLICIT-ITERATOR": true
    },
    context: {},

    render: function (template, context, partials, in_recursion) {
      // reset buffer & set context
      if (!in_recursion) {
        this.context = context;
        this.buffer = []; // TODO: make this non-lazy
      }

      // fail fast
      if (!this.includes("", template)) {
        if (in_recursion) {
          return template;
        } else {
          this.send(template);
          return;
        }
      }

      // get the pragmas together
      template = this.render_pragmas(template);

      // render the template
      var html = this.render_section(template, context, partials);

      // render_section did not find any sections, we still need to render the tags
      if (html === false) {
        html = this.render_tags(template, context, partials, in_recursion);
      }

      if (in_recursion) {
        return html;
      } else {
        this.sendLines(html);
      }
    },

    /*
      Sends parsed lines
    */
    send: function (line) {
      if (line !== "") {
        this.buffer.push(line);
      }
    },

    sendLines: function (text) {
      if (text) {
        var lines = text.split("\n");
        for (var i = 0; i < lines.length; i++) {
          this.send(lines[i]);
        }
      }
    },

    /*
      Looks for %PRAGMAS
    */
    render_pragmas: function (template) {
      // no pragmas
      if (!this.includes("%", template)) {
        return template;
      }

      var that = this;
      var regex = this.getCachedRegex("render_pragmas", function (otag, ctag) {
        return new RegExp(otag + "%([\\w-]+) ?([\\w]+=[\\w]+)?" + ctag, "g");
      });

      return template.replace(regex, function (match, pragma, options) {
        if (!that.pragmas_implemented[pragma]) {
          throw({message:
            "This implementation of mustache doesn't understand the '" +
            pragma + "' pragma"});
        }
        that.pragmas[pragma] = {};
        if (options) {
          var opts = options.split("=");
          that.pragmas[pragma][opts[0]] = opts[1];
        }
        return "";
        // ignore unknown pragmas silently
      });
    },

    /*
      Tries to find a partial in the curent scope and render it
    */
    render_partial: function (name, context, partials) {
      name = trim(name);
      if (!partials || partials[name] === undefined) {
        throw({message: "unknown_partial '" + name + "'"});
      }
      if (!context || typeof context[name] != "object") {
        return this.render(partials[name], context, partials, true);
      }
      return this.render(partials[name], context[name], partials, true);
    },

    /*
      Renders inverted (^) and normal (#) sections
    */
    render_section: function (template, context, partials) {
      if (!this.includes("#", template) && !this.includes("^", template)) {
        // did not render anything, there were no sections
        return false;
      }

      var that = this;

      var regex = this.getCachedRegex("render_section", function (otag, ctag) {
        // This regex matches _the first_ section ({{#foo}}{{/foo}}), and captures the remainder
        return new RegExp(
          "^([\\s\\S]*?)" +         // all the crap at the beginning that is not {{*}} ($1)

          otag +                    // {{
          "(\\^|\\#)\\s*(.+)\\s*" + //  #foo (# == $2, foo == $3)
          ctag +                    // }}

          "\n*([\\s\\S]*?)" +       // between the tag ($2). leading newlines are dropped

          otag +                    // {{
          "\\/\\s*\\3\\s*" +        //  /foo (backreference to the opening tag).
          ctag +                    // }}

          "\\s*([\\s\\S]*)$",       // everything else in the string ($4). leading whitespace is dropped.

        "g");
      });


      // for each {{#foo}}{{/foo}} section do...
      return template.replace(regex, function (match, before, type, name, content, after) {
        // before contains only tags, no sections
        var renderedBefore = before ? that.render_tags(before, context, partials, true) : "",

        // after may contain both sections and tags, so use full rendering function
            renderedAfter = after ? that.render(after, context, partials, true) : "",

        // will be computed below
            renderedContent,

            value = that.find(name, context);

        if (type === "^") { // inverted section
          if (!value || Array.isArray(value) && value.length === 0) {
            // false or empty list, render it
            renderedContent = that.render(content, context, partials, true);
          } else {
            renderedContent = "";
          }
        } else if (type === "#") { // normal section
          if (Array.isArray(value)) { // Enumerable, Let's loop!
            renderedContent = that.map(value, function (row) {
              return that.render(content, that.create_context(row), partials, true);
            }).join("");
          } else if (that.is_object(value)) { // Object, Use it as subcontext!
            renderedContent = that.render(content, that.create_context(value),
              partials, true);
          } else if (typeof value == "function") {
            // higher order section
            renderedContent = value.call(context, content, function (text) {
              return that.render(text, context, partials, true);
            });
          } else if (value) { // boolean section
            renderedContent = that.render(content, context, partials, true);
          } else {
            renderedContent = "";
          }
        }

        return renderedBefore + renderedContent + renderedAfter;
      });
    },

    /*
      Replace {{foo}} and friends with values from our view
    */
    render_tags: function (template, context, partials, in_recursion) {
      // tit for tat
      var that = this;

      var new_regex = function () {
        return that.getCachedRegex("render_tags", function (otag, ctag) {
          return new RegExp(otag + "(=|!|>|&|\\{|%)?([^#\\^]+?)\\1?" + ctag + "+", "g");
        });
      };

      var regex = new_regex();
      var tag_replace_callback = function (match, operator, name) {
        switch(operator) {
        case "!": // ignore comments
          return "";
        case "=": // set new delimiters, rebuild the replace regexp
          that.set_delimiters(name);
          regex = new_regex();
          return "";
        case ">": // render partial
          return that.render_partial(name, context, partials);
        case "{": // the triple mustache is unescaped
        case "&": // & operator is an alternative unescape method
          return that.find(name, context);
        default: // escape the value
          return escapeHTML(that.find(name, context));
        }
      };
      var lines = template.split("\n");
      for(var i = 0; i < lines.length; i++) {
        lines[i] = lines[i].replace(regex, tag_replace_callback, this);
        if (!in_recursion) {
          this.send(lines[i]);
        }
      }

      if (in_recursion) {
        return lines.join("\n");
      }
    },

    set_delimiters: function (delimiters) {
      var dels = delimiters.split(" ");
      this.otag = this.escape_regex(dels[0]);
      this.ctag = this.escape_regex(dels[1]);
    },

    escape_regex: function (text) {
      // thank you Simon Willison
      if (!arguments.callee.sRE) {
        var specials = [
          '/', '.', '*', '+', '?', '|',
          '(', ')', '[', ']', '{', '}', '\\'
        ];
        arguments.callee.sRE = new RegExp(
          '(\\' + specials.join('|\\') + ')', 'g'
        );
      }
      return text.replace(arguments.callee.sRE, '\\$1');
    },

    /*
      find `name` in current `context`. That is find me a value
      from the view object
    */
    find: function (name, context) {
      name = trim(name);

      // Checks whether a value is thruthy or false or 0
      function is_kinda_truthy(bool) {
        return bool === false || bool === 0 || bool;
      }

      var value;

      // check for dot notation eg. foo.bar
      if (name.match(/([a-z_]+)\./ig)) {
        var childValue = this.walk_context(name, context);
        if (is_kinda_truthy(childValue)) {
          value = childValue;
        }
      } else {
        if (is_kinda_truthy(context[name])) {
          value = context[name];
        } else if (is_kinda_truthy(this.context[name])) {
          value = this.context[name];
        }
      }

      if (typeof value == "function") {
        return value.apply(context);
      }
      if (value !== undefined) {
        return value;
      }
      // silently ignore unkown variables
      return "";
    },

    walk_context: function (name, context) {
      var path = name.split('.');
      // if the var doesn't exist in current context, check the top level context
      var value_context = (context[path[0]] != undefined) ? context : this.context;
      var value = value_context[path.shift()];
      while (value != undefined && path.length > 0) {
        value_context = value;
        value = value[path.shift()];
      }
      // if the value is a function, call it, binding the correct context
      if (typeof value == "function") {
        return value.apply(value_context);
      }
      return value;
    },

    // Utility methods

    /* includes tag */
    includes: function (needle, haystack) {
      return haystack.indexOf(this.otag + needle) != -1;
    },

    // by @langalex, support for arrays of strings
    create_context: function (_context) {
      if (this.is_object(_context)) {
        return _context;
      } else {
        var iterator = ".";
        if (this.pragmas["IMPLICIT-ITERATOR"]) {
          iterator = this.pragmas["IMPLICIT-ITERATOR"].iterator;
        }
        var ctx = {};
        ctx[iterator] = _context;
        return ctx;
      }
    },

    is_object: function (a) {
      return a && typeof a == "object";
    },

    /*
      Why, why, why? Because IE. Cry, cry cry.
    */
    map: function (array, fn) {
      if (typeof array.map == "function") {
        return array.map(fn);
      } else {
        var r = [];
        var l = array.length;
        for(var i = 0; i < l; i++) {
          r.push(fn(array[i]));
        }
        return r;
      }
    },

    getCachedRegex: function (name, generator) {
      var byOtag = regexCache[this.otag];
      if (!byOtag) {
        byOtag = regexCache[this.otag] = {};
      }

      var byCtag = byOtag[this.ctag];
      if (!byCtag) {
        byCtag = byOtag[this.ctag] = {};
      }

      var regex = byCtag[name];
      if (!regex) {
        regex = byCtag[name] = generator(this.otag, this.ctag);
      }

      return regex;
    }
  };

  return({
    name: "mustache.js",
    version: "0.4.0",

    /*
      Turns a template and view into HTML
    */
    to_html: function (template, view, partials, send_fun) {
      var renderer = new Renderer();
      if (send_fun) {
        renderer.send = send_fun;
      }
      renderer.render(template, view || {}, partials);
      if (!send_fun) {
        return renderer.buffer.join("\n");
      }
    }
  });
}();
/*!
  ICanHaz.js -- by @HenrikJoreteg
*/
/*global  */
(function () {
    function trim(stuff) {
        if (''.trim) return stuff.trim();
        else return stuff.replace(/^\s+/, '').replace(/\s+$/, '');
    }
    var ich = {
        VERSION: "0.10",
        templates: {},
        
        // grab jquery or zepto if it's there
        $: (typeof window !== 'undefined') ? window.jQuery || window.Zepto || null : null,
        
        // public function for adding templates
        // can take a name and template string arguments
        // or can take an object with name/template pairs
        // We're enforcing uniqueness to avoid accidental template overwrites.
        // If you want a different template, it should have a different name.
        addTemplate: function (name, templateString) {
            if (typeof name === 'object') {
                for (var template in name) {
                    this.addTemplate(template, name[template]);
                }
                return;
            }
            if (ich[name]) {
                console.error("Invalid name: " + name + "."); 
            } else if (ich.templates[name]) {
                console.error("Template \"" + name + "  \" exists");
            } else {
                ich.templates[name] = templateString;
                ich[name] = function (data, raw) {
                    data = data || {};
                    var result = Mustache.to_html(ich.templates[name], data, ich.templates);
                    return (ich.$ && !raw) ? ich.$(result) : result;
                };
            }
        },
        
        // clears all retrieval functions and empties cache
        clearAll: function () {
            for (var key in ich.templates) {
                delete ich[key];
            }
            ich.templates = {};
        },
        
        // clears/grabs
        refresh: function () {
            ich.clearAll();
            ich.grabTemplates();
        },
        
        // grabs templates from the DOM and caches them.
        // Loop through and add templates.
        // Whitespace at beginning and end of all templates inside <script> tags will 
        // be trimmed. If you want whitespace around a partial, add it in the parent, 
        // not the partial. Or do it explicitly using <br/> or &nbsp;
        grabTemplates: function () {        
            var i, 
                scripts = document.getElementsByTagName('script'), 
                script,
                trash = [];
            for (i = 0, l = scripts.length; i < l; i++) {
                script = scripts[i];
                if (script && script.innerHTML && script.id && (script.type === "text/html" || script.type === "text/x-icanhaz")) {
                    ich.addTemplate(script.id, trim(script.innerHTML));
                    trash.unshift(script);
                }
            }
            for (i = 0, l = trash.length; i < l; i++) {
                trash[i].parentNode.removeChild(trash[i]);
            }
        }
    };

    /********************** START FR added from https://github.com/hay/ICanHaz.js/blob/20a9d2366ff1f53afd7558aa43e9471f946cc941/source/main.js *******************/
    // Use AMD
    if (typeof define !== 'undefined' && define.amd) {
        define('ich',[],function () {
            return ich;
        });
    } else if (typeof require !== 'undefined') {
        // Use CommonJS
        module.exports = ich;
    } else {
        // else attach it to the window
        window.ich = ich;
    }
    /********************** END FR added from https://github.com/hay/ICanHaz.js/blob/20a9d2366ff1f53afd7558aa43e9471f946cc941/source/main.js *******************/
    
    if (typeof document !== 'undefined') {
        if (ich.$) {
            ich.$(function () {
                ich.grabTemplates();
            });
        } else {
            document.addEventListener('DOMContentLoaded', function () {
                ich.grabTemplates();
            }, true);
        }
    }
        
})();
})();

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// User
// ----

define('models/user',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var User = Backbone.Model.extend({
        defaults: {
            FirstName: '',
            LastName: '',
            Email: '',
            AvatarId: null,
            Type: 'User'
        },

        idAttribute: 'Id',

        urlRoot: '/users',

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return User;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserCollection
// --------------

define('collections/usercollection',['jquery', 'underscore', 'backbone', 'models/user'], function ($, _, Backbone, User) {

    var UserCollection = Backbone.Collection.extend({
        model: User,

        url: '/users',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return UserCollection;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Project
// -------

define('models/project',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Project = Backbone.Model.extend({
        defaults: {
            Name: '',
            Description: '',
            Website: '',
            TeamId: null,
            Type: 'Project'
        },

        idAttribute: 'Id',

        urlRoot: '/projects',

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return Project;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectCollection
// -----------------

define('collections/projectcollection',['jquery', 'underscore', 'backbone', 'models/project'], function ($, _, Backbone, Project) {

    var ProjectCollection = Backbone.Collection.extend({
        model: Project,

        url: '/projects',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        comparator: function (project) {
            return project.get('Name');
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (project) {
                viewModels.push(project.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return ProjectCollection;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Team
// ----

define('models/team',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Team = Backbone.Model.extend({
        defaults: {
            Name: '',
            Description: '',
            Website: '',
            AvatarId: null,
            Organisation: null,
            Type: 'Team'
        },

        idAttribute: 'Id',

        urlRoot: '/teams',

//        toJSON: function () {
//            return {
//                Name: this.get('Name'),
//                Description: this.get('Description'),
//                Website: this.get('Website'),
//                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
//                Organisation: this.get('Organisation'),
//                Type: 'Team'
//            };
//        },

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return Team;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamCollection
// --------------

define('collections/teamcollection',['jquery', 'underscore', 'backbone', 'models/team'], 
function ($, _, Backbone, Team) {

    var TeamCollection = Backbone.Collection.extend({
        model: Team,

        url: '/teams',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (team) {
                viewModels.push(team.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return TeamCollection;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Organisation
// ------------

define('models/organisation',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Organisation = Backbone.Model.extend({
        defaults: {
            Name: '',
            Description: '',
            Website: '',
            AvatarId: null,
            Type: 'Organisation'
        },

        idAttribute: 'Id',

        urlRoot: '/organisations',

//        toJSON: function () {
//            return {
//                Name: this.get('Name'),
//                Description: this.get('Description'),
//                Website: this.get('Website'),
//                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
//                Type: 'Organisation'
//            };
//        },

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return Organisation;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationCollection
// ----------------------

define('collections/organisationcollection',['jquery', 'underscore', 'backbone', 'models/organisation'], 
function ($, _, Backbone, Organisation) {

    var OrganisationCollection = Backbone.Collection.extend({
        model: Organisation,

        url: '/organisations',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (organisation) {
                viewModels.push(organisation.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return OrganisationCollection;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PaginatedCollection
// -------------------

// Base of paginated collections
define('collections/paginatedcollection',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var PaginatedCollection = Backbone.Collection.extend({
        initialize: function (options) {
            _.extend(this, Backbone.Events);
            typeof (options) != 'undefined' || (options = {});
            this.page = 1;
            typeof (this.pageSize) != 'undefined' || (this.pageSize = 10);
        },

        fetch: function (options) {
            typeof (options) != 'undefined' || (options = {});
            this.trigger("fetching", this);
            var self = this;
            var success = options.success;
            options.success = function (resp) {
                self.trigger("fetched", self);
                if (success) { success(self, resp); }
            };
            return Backbone.Collection.prototype.fetch.call(this, options);
        },

        parse: function (resp) {
            this.page = resp.Model.Page;
            this.pageSize = resp.Model.PageSize;
            this.total = resp.Model.TotalResultCount;
            return resp.Model.PagedListItems;
        },

        url: function () {
            return this.baseUrl + '?' + $.param({ page: this.page, pageSize: this.pageSize });
        },

        pageInfo: function () {
            var info = {
                total: this.total,
                page: this.page,
                pageSize: this.pageSize,
                pages: Math.ceil(this.total / this.pageSize),
                prev: false,
                next: false
            };

            var max = Math.min(this.total, this.page * this.pageSize);

            if (this.total == this.pages * this.pageSize) {
                max = this.total;
            }

            info.range = [(this.page - 1) * this.pageSize + 1, max];

            if (this.page > 1) {
                info.prev = this.page - 1;
            }

            if (this.page < info.pages) {
                info.next = this.page + 1;
            }

            return info;
        },

        firstPage: function (options) {
            this.page = 1;
            return this.fetch(options);
        },

        nextPage: function (options) {
            if (!this.pageInfo().next) {
                return false;
            }
            this.page = this.page + 1;
            return this.fetch(options);
        },

        previousPage: function () {
            if (!this.pageInfo().prev) {
                return false;
            }
            this.page = this.page - 1;
            return this.fetch(options);
        }
    });

    return PaginatedCollection;

});

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Activity
// --------

define('models/activity',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Activity = Backbone.Model.extend({
        idAttribute: 'Id'
    });

    return Activity;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityCollection
// ------------------

define('collections/activitycollection',['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/activity', 'models/user', 'models/project'], function ($, _, Backbone, PaginatedCollection, Activity, User, Project) {

    var ActivityCollection = PaginatedCollection.extend({
        model: Activity,

        baseUrl: '/',

        groupOrUser: null,

        initialize: function (models, options) {
            _.bindAll(this, 'onSuccess', 'onSuccessWithAddFix', 'getFetchOptions');
            PaginatedCollection.prototype.initialize.apply(this, arguments);
            typeof (options) != 'undefined' || (options = {});

            if (options.groupOrUser) {
                this.groupOrUser = options.groupOrUser;
                this.baseUrl = '/' + options.groupOrUser.id + '/activity';
            }

            // Set the moment in time (UTC) around which all activity queries will be performed
            var now = new Date();
            this.baselineDateTime = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
        },

        parse: function (resp) {
            var activities = null;
            if (this.groupOrUser) {
                activities = resp.Model;
            } else {
                activities = resp.Model.Activities;
            }
            this.page = activities.Page;
            this.pageSize = activities.PageSize;
            this.total = activities.TotalResultCount;
            return activities.PagedListItems;
        },

        comparator: function (streamItem) {
            return -parseInt(streamItem.get('CreatedDateTimeOrder'));
        },

        fetchFirstPage: function () {
            this.firstPage(this.getFetchOptions(true, 'olderThan'));
        },

        fetchNextPage: function () {
            this.nextPage(this.getFetchOptions(true, 'olderThan'));
        },

        fetchNewItems: function () {
            this.fetch(this.getFetchOptions(true, 'newerThan'));
        },

        getFetchOptions: function (add, newerOrOlder) {
            var options = {
                data: {},
                add: add,
                success: null
            };
            if (add) {
                options.success = this.onSuccess;
            } else {
                options.success = this.onSuccessWithAddFix;
            }
            if (this.groupOrUser) {
                //if (this.groupOrUser instanceof Organisation || this.groupOrUser instanceof Team || this.groupOrUser instanceof Project) {
                if (this.groupOrUser instanceof Project) {
                    options.data.groupId = this.groupOrUser.id;
                } else if (this.groupOrUser instanceof User) {
                    options.data.userId = this.groupOrUser.id;
                }
            }

            // Get either items newer than or older than baseline
            options.data[newerOrOlder] = this.baselineDateTime;

            return options;
        },

        onSuccess: function (collection, response) {
            //app.stream.trigger('fetchingItemsComplete', app.stream, response);
        },

        onSuccessWithAddFix: function (collection, response) {
            this.onSuccess(collection, response);
            // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
            var self = this;
            response.each(function (item, index) {
                self.trigger('add', item, self, { Index: index });
            });
        }
    });

    return ActivityCollection;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ExploreProjectCollection
// ------------------------

define('collections/exploreprojectcollection',[
'jquery',
'underscore',
'backbone',
'collections/paginatedcollection',
'models/user',
'models/project'],
function (
$,
_,
Backbone,
PaginatedCollection,
User,
Project
) {
    var ExploreProjectCollection = PaginatedCollection.extend({
        
        model: Project,

        baseUrl: '/projects',

        groupOrUser: null,

        initialize: function () {
            _.bindAll(this,
            'onSuccess',
            'onSuccessWithAddFix',
            'getFetchOptions');
            PaginatedCollection.prototype.initialize.apply(this, arguments);

            typeof (options.groupOrUser) != 'undefined' || (this.groupOrUser = options.groupOrUser);
        },

        comparator: function (streamItem1) {
            //            log(streamItem1.get('CreatedDateTime').substr(6));
            //            log(streamItem2.get('CreatedDateTime').substr(6));
            //            var streamItem1CreateDate = new Date(parseInt(streamItem1.get('CreatedDateTimeOrder')));
            //            var streamItem2CreateDate = new Date(parseInt(streamItem2.get('CreatedDateTimeOrder')));

            //            if (streamItem1CreateDate.isAfter(streamItem2CreateDate)) {
            //                return -1;
            //            }

            //            if (streamItem1CreateDate.isBefore(streamItem2CreateDate)) {
            //                return 1;
            //            }

            //            return -1;
            return -parseInt(streamItem1.get('CreatedDateTimeOrder'));
        },

        fetchFirstPage: function () {
            this.firstPage(this.getFetchOptions(true));
        },

        fetchNextPage: function () {
            this.nextPage(this.getFetchOptions(true));
        },

        getFetchOptions: function (add) {
            var options = {
                data: {},
                add: add,
                success: null
            };
            if (add) {
                options.success = this.onSuccess;
            } else {
                options.success = this.onSuccessWithAddFix;
            }
            if (this.groupOrUser) {
                //if (this.groupOrUser instanceof Organisation || this.groupOrUser instanceof Team || this.groupOrUser instanceof Project) {
                if (this.groupOrUser instanceof Project) {
                    options.data.groupId = this.groupOrUser.id;
                } else if (this.groupOrUser instanceof User) {
                    options.data.userId = this.groupOrUser.id;
                }
            }
            //            if (stream.get('Filter') != null) {
            //                options.data.filter = stream.get('Filter');
            //            }
            return options;
        },

        onSuccess: function (collection, response) {
            //app.stream.trigger('fetchingItemsComplete', app.stream, response);
        },

        onSuccessWithAddFix: function (collection, response) {
            this.onSuccess(collection, response);
            // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
            var self = this;
            response.each(function (item, index) {
                self.trigger('add', item, self, { Index: index });
            });
        }
    });

    return ExploreProjectCollection;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessage
// -----------

// Displays a message from a user in a chat window
define('models/chatmessage',['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) 
{
    var ChatMessage = Backbone.Model.extend({
        defaults: {
            // Template has: User.Name, Message, Timestamp
        },

        idAttribute: 'Id'
    });

    return ChatMessage;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageCollection
// ---------------------

define('collections/chatmessagecollection',['jquery', 'underscore', 'backbone', 'models/chatmessage'],
function ($, _, Backbone, ChatMessage) 
{
    var ChatMessageCollection = Backbone.Collection.extend({
        
        model: ChatMessage,

        url: '/chatmessages',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return ChatMessageCollection;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Chat
// ----

define('models/chat',['jquery', 'underscore', 'backbone', 'app', 'collections/usercollection', 'collections/chatmessagecollection', 'models/user', 'models/chatmessage'],
function ($, _, Backbone, app, UserCollection, ChatMessageCollection, User, ChatMessage) {

    var Chat = Backbone.Model.extend({
        defaults: {
            IsStarted: false,
            Group: null // If group chat, this must be set
        },

        idAttribute: 'Id',

        initialize: function (attributes, options) {
            this.chatUsers = options.chatUsers;
            this.chatMessages = options.chatMessages;
        },

        chatType: function () {
            if (this.get('Group') != null) {
                return 'group';
            }
            return 'private';
        },

        // Call this method once chat is setup and user can start sending messages
        start: function (chatDetails) {
            if (chatDetails) {
                this.chatUsers.reset(chatDetails.Users);
                this.chatMessages.reset(chatDetails.Messages);
            }
            log('chat started', this.id);
            this.set('IsStarted', true);
        }
    });

    return Chat;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatCollection
// --------------

define('collections/chatcollection',['jquery', 'underscore', 'backbone', 'models/chat'],
function ($, _, Backbone, Chat) 
{
    var ChatCollection = Backbone.Collection.extend({
        
        model: Chat,

        url: '/chats',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return ChatCollection;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResource
// -------------

define('models/mediaresource',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var MediaResource = Backbone.Model.extend({
        defaults: {
            Key: '',
            MediaType: '',
            Files: {
                FullMedium: {
                    RelativeUri: '/img/image-upload.png'
                }
            },
            VideoUri: ''
        },

        idAttribute: 'Id',

        urlRoot: '/mediaresources'
    });

    return MediaResource;

});
// Backbone.Marionette v0.7.2
//
// Copyright (C)2011 Derick Bailey, Muted Solutions, LLC
// Distributed Under MIT License
//
// Documentation and Full License Available at:
// http://github.com/derickbailey/backbone.marionette

(function (root, factory) {
  if (typeof exports === 'object') {

    var jquery = require('jquery');
    var underscore = require('underscore');
    var backbone = require('backbone');

    module.exports = factory(jquery, underscore, backbone);

  } else if (typeof define === 'function' && define.amd) {

    define('marionette',['jquery', 'underscore', 'backbone'], factory);

  } 
}(this, function ($, _, Backbone) {

  Backbone.Marionette = (function(Backbone, _, $){
  var Marionette = {};

  Marionette.version = "0.7.2";

  // Marionette.View
  // ---------------

  // The core view type that other Marionette views extend from.
  Marionette.View = Backbone.View.extend({
    // Get the template or template id/selector for this view
    // instance. You can set a `template` attribute in the view
    // definition or pass a `template: "whatever"` parameter in
    // to the constructor options. The `template` can also be
    // a function that returns a selector string.
    getTemplateSelector: function(){
      var template;

      // Get the template from `this.options.template` or
      // `this.template`. The `options` takes precedence.
      if (this.options && this.options.template){
        template = this.options.template;
      } else {
        template = this.template;
      }

      // check if it's a function and execute it, if it is
      if (_.isFunction(template)){
        template  = template.call(this);
      }

      return template;
    },

    // Serialize the model or collection for the view. If a model is
    // found, `.toJSON()` is called. If a collection is found, `.toJSON()`
    // is also called, but is used to populate an `items` array in the
    // resulting data. If both are found, defaults to the model. 
    // You can override the `serializeData` method in your own view 
    // definition, to provide custom serialization for your view's data.
    serializeData: function(){
      var data;

      if (this.model) { data = this.model.toJSON(); }
      else if (this.collection) {
        data = { items: this.collection.toJSON() };
      }

      return data;
    },

    // Default `close` implementation, for removing a view from the
    // DOM and unbinding it. Regions will call this method
    // for you. You can specify an `onClose` method in your view to
    // add custom code that is called after the view is closed.
    close: function(){
      if (this.beforeClose) { this.beforeClose(); }

      this.unbindAll();
      this.remove();

      if (this.onClose) { this.onClose(); }
      this.trigger('close');
      this.unbind();
    }
  });

  // Item View
  // ---------
  
  // A single item view implementation that contains code for rendering
  // with underscore.js templates, serializing the view's model or collection,
  // and calling several methods on extended views, such as `onRender`.
  Marionette.ItemView = Marionette.View.extend({
    constructor: function(){
      var args = slice.call(arguments);
      Marionette.View.prototype.constructor.apply(this, args);

      _.bindAll(this, "render");

      this.initialEvents();
    },

    // Configured the initial events that the item view 
    // binds to. Override this method to prevent the initial
    // events, or to add your own initial events.
    initialEvents: function(){
      if (this.collection){
        this.bindTo(this.collection, "reset", this.render, this);
      }
    },

    // Render the view, defaulting to underscore.js templates.
    // You can override this in your view definition.
    render: function(){
      var that = this;

      var deferredRender = $.Deferred();
      var deferredData = this.serializeData();

      if (this.beforeRender) { this.beforeRender(); }
      this.trigger("item:before:render", that);

      $.when(deferredData).then(function(data) {

        var asyncRender = that.renderHtml(data);

        $.when(asyncRender).then(function(html){
          that.$el.html(html);
          var onRenderPromise = {};
          if (that.onRender) { onRenderPromise = that.onRender(); }
          $.when(onRenderPromise).then(function() {
            that.trigger("item:rendered", that);
            that.trigger("render", that);
            deferredRender.resolve();
          });
        });

      });

      return deferredRender.promise();
    },

    // Render the data for this item view in to some HTML.
    // Override this method to replace the specific way in
    // which an item view has it's data rendered in to html.
    renderHtml: function(data) {
      var template = this.getTemplateSelector();
      return Marionette.Renderer.render(template, data);
    },

    // Override the default close event to add a few
    // more events that are triggered.
    close: function(){
      this.trigger('item:before:close');
      Marionette.View.prototype.close.apply(this, arguments);
      this.trigger('item:closed');
    }
  });

  // Collection View
  // ---------------

  // A view that iterates over a Backbone.Collection
  // and renders an individual ItemView for each model.
  Marionette.CollectionView = Marionette.View.extend({
    constructor: function(){
      Marionette.View.prototype.constructor.apply(this, arguments);

      _.bindAll(this, "addItemView", "render");
      this.initialEvents();
    },

    // Configured the initial events that the collection view 
    // binds to. Override this method to prevent the initial
    // events, or to add your own initial events.
    initialEvents: function(){
      if (this.collection){
        this.bindTo(this.collection, "add", this.addChildView, this);
        this.bindTo(this.collection, "remove", this.removeItemView, this);
        this.bindTo(this.collection, "reset", this.render, this);
      }
    },

    // Handle a child item added to the collection
    addChildView: function(item){
      var ItemView = this.getItemView();
      return this.addItemView(item, ItemView);
    },

    // Loop through all of the items and render 
    // each of them with the specified `itemView`.
    render: function(){
      var that = this;
      var deferredRender = $.Deferred();
      var promises = [];
      var ItemView = this.getItemView();

      if (this.beforeRender) { this.beforeRender(); }
      this.trigger("collection:before:render", this);

      this.closeChildren();

      if (this.collection) {
        this.collection.each(function(item){
          var promise = that.addItemView(item, ItemView);
          promises.push(promise);
        });
      }

      deferredRender.done(function(){
        if (this.onRender) { this.onRender(); }
        this.trigger("collection:rendered", this);
      });

      $.when.apply(this, promises).then(function(){
        deferredRender.resolveWith(that);
      });

      return deferredRender.promise();
    },

    // Retrieve the itemView type, either from `this.options.itemView`
    // or from the `itemView` in the object definition. The "options"
    // takes precedence.
    getItemView: function(){
      var itemView = this.options.itemView || this.itemView;

      if (!itemView){
        var err = new Error("An `itemView` must be specified");
        err.name = "NoItemViewError";
        throw err;
      }

      return itemView;
    },

    // Render the child item's view and add it to the
    // HTML for the collection view.
    addItemView: function(item, ItemView){
      var that = this;

      var view = this.buildItemView(item, ItemView);
      this.storeChild(view);
      this.trigger("item:added", view);

      var viewRendered = view.render();
      $.when(viewRendered).then(function(){
          that.appendHtml(that, view);
          that.trigger("item:afteradd", view); // FR added event 19/06/2012
      });
      
      return viewRendered;
    },

    // Build an `itemView` for every model in the collection. 
    buildItemView: function(item, ItemView){
      var view = new ItemView({
        model: item
      });
      return view;
    },

    // Remove the child view and close it
    removeItemView: function(item){
      var view = this.children[item.cid];
      if (view){
        view.close();
        delete this.children[item.cid];
      }
      this.trigger("item:removed", view);
    },

    // Append the HTML to the collection's `el`.
    // Override this method to do something other
    // then `.append`.
    appendHtml: function(collectionView, itemView){
      collectionView.$el.append(itemView.el);
    },

    // Store references to all of the child `itemView`
    // instances so they can be managed and cleaned up, later.
    storeChild: function(view){
      if (!this.children){
        this.children = {};
      }
      this.children[view.model.cid] = view;
    },
    
    // Handle cleanup and other closing needs for
    // the collection of views.
    close: function(){
      this.trigger("collection:before:close");
      this.closeChildren();
      Marionette.View.prototype.close.apply(this, arguments);
      this.trigger("collection:closed");
    },

    closeChildren: function(){
      if (this.children){
        _.each(this.children, function(childView){
          childView.close();
        });
      }
    }
  });

  // Composite View
  // --------------

  // Used for rendering a branch-leaf, hierarchical structure.
  // Extends directly from CollectionView and also renders an
  // an item view as `modelView`, for the top leaf
  Marionette.CompositeView = Marionette.CollectionView.extend({
    constructor: function(options){
      Marionette.CollectionView.apply(this, arguments);
      this.itemView = this.getItemView();
    },

    // Retrieve the `itemView` to be used when rendering each of
    // the items in the collection. The default is to return
    // `this.itemView` or Marionette.CompositeView if no `itemView`
    // has been defined
    getItemView: function(){
      return this.itemView || this.constructor;
    },

    // Renders the model once, and the collection once. Calling
    // this again will tell the model's view to re-render itself
    // but the collection will not re-render.
    render: function(){
      var that = this;
      var compositeRendered = $.Deferred();

      var modelIsRendered = this.renderModel();
      $.when(modelIsRendered).then(function(html){
        that.$el.html(html);
        that.trigger("composite:model:rendered");
        that.trigger("render");

        var collectionIsRendered = that.renderCollection();
        $.when(collectionIsRendered).then(function(){
          compositeRendered.resolve();
        });
      });

      compositeRendered.done(function(){
        that.trigger("composite:rendered");
      });

      return compositeRendered.promise();
    },

    // Render the collection for the composite view
    renderCollection: function(){
      var collectionDeferred = Marionette.CollectionView.prototype.render.apply(this, arguments);
      collectionDeferred.done(function(){
        this.trigger("composite:collection:rendered");
      });
      return collectionDeferred.promise();
    },

    // Render an individual model, if we have one, as
    // part of a composite view (branch / leaf). For example:
    // a treeview.
    renderModel: function(){
      var data = {};
      data = this.serializeData();

      var template = this.getTemplateSelector();
      return Marionette.Renderer.render(template, data);
    }
  });

  // Region 
  // ------

  // Manage the visual regions of your composite application. See
  // http://lostechies.com/derickbailey/2011/12/12/composite-js-apps-regions-and-region-managers/
  Marionette.Region = function(options){
    this.options = options || {};

    _.extend(this, options);

    if (!this.el){
      var err = new Error("An 'el' must be specified");
      err.name = "NoElError";
      throw err;
    }

    if (this.initialize){
      this.initialize.apply(this, arguments);
    }
  };

  _.extend(Marionette.Region.prototype, Backbone.Events, {

    // Displays a backbone view instance inside of the region.
    // Handles calling the `render` method for you. Reads content
    // directly from the `el` attribute. Also calls an optional
    // `onShow` and `close` method on your view, just after showing
    // or just before closing the view, respectively.
    show: function(view, appendMethod){
      this.ensureEl();

      this.close();
      this.open(view, appendMethod);

      this.currentView = view;
    },

    ensureEl: function(){
      if (!this.$el || this.$el.length === 0){
        this.$el = this.getEl(this.el);
      }
    },

    // Override this method to change how the region finds the
    // DOM element that it manages. Return a jQuery selector object.
    getEl: function(selector){
        return $(selector);
    },

    // Internal method to render and display a view. Not meant 
    // to be called from any external code.
    open: function(view, appendMethod){
      var that = this;
      appendMethod = appendMethod || "html";

      $.when(view.render()).then(function () {
        that.$el[appendMethod](view.el);
        if (view.onShow) { view.onShow(); }
        if (that.onShow) { that.onShow(view); }
        view.trigger("show");
        that.trigger("view:show", view);
      });
    },

    // Close the current view, if there is one. If there is no
    // current view, it does nothing and returns immediately.
    close: function(){
      var view = this.currentView;
      if (!view){ return; }

      if (view.close) { view.close(); }
      this.trigger("view:closed", view);

      delete this.currentView;
    },

    // Attach an existing view to the region. This 
    // will not call `render` or `onShow` for the new view, 
    // and will not replace the current HTML for the `el`
    // of the region.
    attachView: function(view){
      this.currentView = view;
    }
  });

  // Layout
  // ------

  // Formerly known as Composite Region.
  //
  // Used for managing application layouts, nested layouts and
  // multiple regions within an application or sub-application.
  //
  // A specialized view type that renders an area of HTML and then
  // attaches `Region` instances to the specified `regions`.
  // Used for composite view management and sub-application areas.
  Marionette.Layout = Marionette.ItemView.extend({
    constructor: function () {
      this.vent = new Backbone.Marionette.EventAggregator();
      Backbone.Marionette.ItemView.apply(this, arguments);
      this.regionManagers = {};
    },

    render: function () {
      this.initializeRegions();
      return Backbone.Marionette.ItemView.prototype.render.call(this, arguments);
    },

    close: function () {
      this.closeRegions();
      Backbone.Marionette.ItemView.prototype.close.call(this, arguments);
    },

    initializeRegions: function () {
      var that = this;
      _.each(this.regions, function (selector, name) {
        var regionManager = new Backbone.Marionette.Region({
            el: selector,

            getEl: function(selector){
              return that.$(selector);
            }
        });
        that.regionManagers[name] = regionManager;
        that[name] = regionManager;
      });
    },

    closeRegions: function () {
      var that = this;
      _.each(this.regionManagers, function (manager, name) {
        manager.close();
        delete that[name];
      });
      this.regionManagers = {};
    }
  });

  // AppRouter
  // ---------

  // Reduce the boilerplate code of handling route events
  // and then calling a single method on another object.
  // Have your routers configured to call the method on
  // your object, directly.
  //
  // Configure an AppRouter with `appRoutes`.
  //
  // App routers can only take one `controller` object. 
  // It is reocmmended that you divide your controller
  // objects in to smaller peices of related functionality
  // and have multiple routers / controllers, instead of
  // just one giant router and controller.
  //
  // You can also add standard routes to an AppRouter.
  
  Marionette.AppRouter = Backbone.Router.extend({

    constructor: function(options){
      Backbone.Router.prototype.constructor.call(this, options);

      if (this.appRoutes){
        var controller = this.controller;
        if (options && options.controller) {
          controller = options.controller;
        }
        this.processAppRoutes(controller, this.appRoutes);
      }
    },

    processAppRoutes: function(controller, appRoutes){
      var method, methodName;
      var route, routesLength, i;
      var routes = [];
      var router = this;

      for(route in appRoutes){
        if (appRoutes.hasOwnProperty(route)){
          routes.unshift([route, appRoutes[route]]);
        }
      }

      routesLength = routes.length;
      for (i = 0; i < routesLength; i++){
        route = routes[i][0];
        methodName = routes[i][1];
        method = _.bind(controller[methodName], controller);
        router.route(route, methodName, method);
      }
    }
  });
  
  // Composite Application
  // ---------------------

  // Contain and manage the composite application as a whole.
  // Stores and starts up `Region` objects, includes an
  // event aggregator as `app.vent`
  Marionette.Application = function(options){
    this.initCallbacks = new Marionette.Callbacks();
    this.vent = new Marionette.EventAggregator();
    _.extend(this, options);
  };

  _.extend(Marionette.Application.prototype, Backbone.Events, {
    // Add an initializer that is either run at when the `start`
    // method is called, or run immediately if added after `start`
    // has already been called.
    addInitializer: function(initializer){
      this.initCallbacks.add(initializer);
    },

    // kick off all of the application's processes.
    // initializes all of the regions that have been added
    // to the app, and runs all of the initializer functions
    start: function(options){
      this.trigger("initialize:before", options);
      this.initCallbacks.run(this, options);
      this.trigger("initialize:after", options);

      this.trigger("start", options);
    },

    // Add regions to your app. 
    // Accepts a hash of named strings or Region objects
    // addRegions({something: "#someRegion"})
    // addRegions{{something: Region.extend({el: "#someRegion"}) });
    addRegions: function(regions){
      var regionValue, regionObj, region;

      for(region in regions){
        if (regions.hasOwnProperty(region)){
          regionValue = regions[region];
    
          if (typeof regionValue === "string"){
            regionObj = new Marionette.Region({
              el: regionValue
            });
          } else {
            regionObj = new regionValue();
          }

          this[region] = regionObj;
        }
      }
    }
  });

  // BindTo: Event Binding
  // ---------------------
  
  // BindTo facilitates the binding and unbinding of events
  // from objects that extend `Backbone.Events`. It makes
  // unbinding events, even with anonymous callback functions,
  // easy. 
  //
  // Thanks to Johnny Oshika for this code.
  // http://stackoverflow.com/questions/7567404/backbone-js-repopulate-or-recreate-the-view/7607853#7607853
  Marionette.BindTo = {
    // Store the event binding in array so it can be unbound
    // easily, at a later point in time.
    bindTo: function (obj, eventName, callback, context) {
      context = context || this;
      obj.on(eventName, callback, context);

      if (!this.bindings) { this.bindings = []; }

      var binding = { 
        obj: obj, 
        eventName: eventName, 
        callback: callback, 
        context: context 
      }

      this.bindings.push(binding);

      return binding;
    },

    // Unbind from a single binding object. Binding objects are
    // returned from the `bindTo` method call. 
    unbindFrom: function(binding){
      binding.obj.off(binding.eventName, binding.callback);
      var index = _.indexOf(this.bindings, binding);
      Array.remove(this.bindings, index);
    },

    // Unbind all of the events that we have stored.
    unbindAll: function () {
      var that = this;

      _.each(this.bindings, function (binding, index) {
        that.unbindFrom(binding);
      });

      this.bindings = [];
    }
  };

  // Callbacks
  // ---------

  // A simple way of managing a collection of callbacks
  // and executing them at a later point in time, using jQuery's
  // `Deferred` object.
  Marionette.Callbacks = function(){
    this.deferred = $.Deferred();
    this.promise = this.deferred.promise();
  };

  _.extend(Marionette.Callbacks.prototype, {
    
    // Add a callback to be executed. Callbacks added here are
    // guaranteed to execute, even if they are added after the 
    // `run` method is called.
    add: function(callback){
      this.promise.done(function(context, options){
        callback.call(context, options);
      });
    },

    // Run all registered callbacks with the context specified. 
    // Additional callbacks can be added after this has been run 
    // and they will still be executed.
    run: function(context, options){
      this.deferred.resolve(context, options);
    }
  });

  // Event Aggregator
  // ----------------

  // A pub-sub object that can be used to decouple various parts
  // of an application through event-driven architecture.
  Marionette.EventAggregator = function(options){
    _.extend(this, options);
  };

  _.extend(Marionette.EventAggregator.prototype, Backbone.Events, Marionette.BindTo, {
    // Assumes the event aggregator itself is the 
    // object being bound to.
    bindTo: function(eventName, callback, context){
      Marionette.BindTo.bindTo.call(this, this, eventName, callback, context);
    }
  });

  // Template Cache
  // --------------
  
  // Manage templates stored in `<script>` blocks,
  // caching them for faster access.
  Marionette.TemplateCache = {
    templates: {},
    loaders: {},

    // Get the specified template by id. Either
    // retrieves the cached version, or loads it
    // from the DOM.
    get: function(templateId){
      var that = this;
      var templateRetrieval = $.Deferred();
      var cachedTemplate = this.templates[templateId];

      if (cachedTemplate){
        templateRetrieval.resolve(cachedTemplate);
      } else {
        var loader = this.loaders[templateId];
        if(loader) {
          templateRetrieval = loader;
        } else {
          this.loaders[templateId] = templateRetrieval;

          this.loadTemplate(templateId, function(template){
            delete that.loaders[templateId];
            that.templates[templateId] = template;
            templateRetrieval.resolve(template);
          });
        }

      }

      return templateRetrieval.promise();
    },

    // Load a template from the DOM, by default. Override
    // this method to provide your own template retrieval,
    // such as asynchronous loading from a server.
    loadTemplate: function(templateId, callback){
      var template = $(templateId).html();

      // Make sure we have a template before trying to compile it
      if (!template || template.length === 0){
        var msg = "Could not find template: '" + templateId + "'";
        var err = new Error(msg);
        err.name = "NoTemplateError";
        throw err;
      }

      template = this.compileTemplate(template);

      callback.call(this, template);
    },

    // Pre-compile the template before caching it. Override
    // this method if you do not need to pre-compile a template
    // (JST / RequireJS for example) or if you want to change
    // the template engine used (Handebars, etc).
    compileTemplate: function(rawTemplate){
      return _.template(rawTemplate);
    },

    // Clear templates from the cache. If no arguments
    // are specified, clears all templates:
    // `clear()`
    //
    // If arguments are specified, clears each of the 
    // specified templates from the cache:
    // `clear("#t1", "#t2", "...")`
    clear: function(){
      var i;
      var length = arguments.length;

      if (length > 0){
        for(i=0; i<length; i++){
          delete this.templates[arguments[i]];
        }
      } else {
        this.templates = {};
      }
    }
  };

  // Renderer
  // --------
  
  // Render a template with data by passing in the template
  // selector and the data to render.
  Marionette.Renderer = {

    // Render a template with data. The `template` parameter is
    // passed to the `TemplateCache` object to retrieve the
    // actual template. Override this method to provide your own
    // custom rendering and template handling for all of Marionette.
    render: function(template, data){
      var that = this;
      var asyncRender = $.Deferred();

      var templateRetrieval = Marionette.TemplateCache.get(template);

      $.when(templateRetrieval).then(function(template){
        var html = that.renderTemplate(template, data);
        asyncRender.resolve(html);
      });

      return asyncRender.promise();
    },

    // Default implementation uses underscore.js templates. Override
    // this method to use your own templating engine.
    renderTemplate: function(template, data){
      var html = template(data);
      return html;
    }

  };

  // Helpers
  // -------

  // For slicing `arguments` in functions
  var slice = Array.prototype.slice;
  
  // Copy the `extend` function used by Backbone's classes
  var extend = Marionette.View.extend;
  Marionette.Region.extend = extend;
  Marionette.Application.extend = extend;

  // Copy the features of `BindTo` on to these objects
  _.extend(Marionette.View.prototype, Marionette.BindTo);
  _.extend(Marionette.Application.prototype, Marionette.BindTo);
  _.extend(Marionette.Region.prototype, Marionette.BindTo);

  // Array Remove - By John Resig (MIT Licensed)
  if (!Array.remove){
    Array.remove = function(array, from, to) {
      var rest = array.slice((to || from) + 1 || array.length);
      array.length = from < 0 ? array.length + from : from;
      return array.push.apply(array, rest);
    };
  }

  return Marionette;
})(Backbone, _, window.jQuery || window.Zepto || window.ender);

  return Backbone.Marionette; 

}));

define('json2',[],function () {
    /*
    json2.js
    2011-10-19

    Public Domain.

    NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.

    See http://www.JSON.org/js.html


    This code should be minified before deployment.
    See http://javascript.crockford.com/jsmin.html

    USE YOUR OWN COPY. IT IS EXTREMELY UNWISE TO LOAD CODE FROM SERVERS YOU DO
    NOT CONTROL.


    This file creates a global JSON object containing two methods: stringify
    and parse.

    JSON.stringify(value, replacer, space)
    value       any JavaScript value, usually an object or array.

    replacer    an optional parameter that determines how object
    values are stringified for objects. It can be a
    function or an array of strings.

    space       an optional parameter that specifies the indentation
    of nested structures. If it is omitted, the text will
    be packed without extra whitespace. If it is a number,
    it will specify the number of spaces to indent at each
    level. If it is a string (such as '\t' or '&nbsp;'),
    it contains the characters used to indent at each level.

    This method produces a JSON text from a JavaScript value.

    When an object value is found, if the object contains a toJSON
    method, its toJSON method will be called and the result will be
    stringified. A toJSON method does not serialize: it returns the
    value represented by the name/value pair that should be serialized,
    or undefined if nothing should be serialized. The toJSON method
    will be passed the key associated with the value, and this will be
    bound to the value

    For example, this would serialize Dates as ISO strings.

    Date.prototype.toJSON = function (key) {
    function f(n) {
    // Format integers to have at least two digits.
    return n < 10 ? '0' + n : n;
    }

    return this.getUTCFullYear()   + '-' +
    f(this.getUTCMonth() + 1) + '-' +
    f(this.getUTCDate())      + 'T' +
    f(this.getUTCHours())     + ':' +
    f(this.getUTCMinutes())   + ':' +
    f(this.getUTCSeconds())   + 'Z';
    };

    You can provide an optional replacer method. It will be passed the
    key and value of each member, with this bound to the containing
    object. The value that is returned from your method will be
    serialized. If your method returns undefined, then the member will
    be excluded from the serialization.

    If the replacer parameter is an array of strings, then it will be
    used to select the members to be serialized. It filters the results
    such that only members with keys listed in the replacer array are
    stringified.

    Values that do not have JSON representations, such as undefined or
    functions, will not be serialized. Such values in objects will be
    dropped; in arrays they will be replaced with null. You can use
    a replacer function to replace those with JSON values.
    JSON.stringify(undefined) returns undefined.

    The optional space parameter produces a stringification of the
    value that is filled with line breaks and indentation to make it
    easier to read.

    If the space parameter is a non-empty string, then that string will
    be used for indentation. If the space parameter is a number, then
    the indentation will be that many spaces.

    Example:

    text = JSON.stringify(['e', {pluribus: 'unum'}]);
    // text is '["e",{"pluribus":"unum"}]'


    text = JSON.stringify(['e', {pluribus: 'unum'}], null, '\t');
    // text is '[\n\t"e",\n\t{\n\t\t"pluribus": "unum"\n\t}\n]'

    text = JSON.stringify([new Date()], function (key, value) {
    return this[key] instanceof Date ?
    'Date(' + this[key] + ')' : value;
    });
    // text is '["Date(---current time---)"]'


    JSON.parse(text, reviver)
    This method parses a JSON text to produce an object or array.
    It can throw a SyntaxError exception.

    The optional reviver parameter is a function that can filter and
    transform the results. It receives each of the keys and values,
    and its return value is used instead of the original value.
    If it returns what it received, then the structure is not modified.
    If it returns undefined then the member is deleted.

    Example:

    // Parse the text. Values that look like ISO date strings will
    // be converted to Date objects.

    myData = JSON.parse(text, function (key, value) {
    var a;
    if (typeof value === 'string') {
    a =
    /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/.exec(value);
    if (a) {
    return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4],
    +a[5], +a[6]));
    }
    }
    return value;
    });

    myData = JSON.parse('["Date(09/09/2001)"]', function (key, value) {
    var d;
    if (typeof value === 'string' &&
    value.slice(0, 5) === 'Date(' &&
    value.slice(-1) === ')') {
    d = new Date(value.slice(5, -1));
    if (d) {
    return d;
    }
    }
    return value;
    });


    This is a reference implementation. You are free to copy, modify, or
    redistribute.
    */

    /*jslint evil: true, regexp: true */

    /*members "", "\b", "\t", "\n", "\f", "\r", "\"", JSON, "\\", apply,
    call, charCodeAt, getUTCDate, getUTCFullYear, getUTCHours,
    getUTCMinutes, getUTCMonth, getUTCSeconds, hasOwnProperty, join,
    lastIndex, length, parse, prototype, push, replace, slice, stringify,
    test, toJSON, toString, valueOf
    */


    // Create a JSON object only if one does not already exist. We create the
    // methods in a closure to avoid creating global variables.

    var JSON;
    if (!JSON) {
        JSON = {};
    }

    (function () {
        

        function f(n) {
            // Format integers to have at least two digits.
            return n < 10 ? '0' + n : n;
        }

        if (typeof Date.prototype.toJSON !== 'function') {

            Date.prototype.toJSON = function (key) {

                return isFinite(this.valueOf())
                ? this.getUTCFullYear() + '-' +
                    f(this.getUTCMonth() + 1) + '-' +
                    f(this.getUTCDate()) + 'T' +
                    f(this.getUTCHours()) + ':' +
                    f(this.getUTCMinutes()) + ':' +
                    f(this.getUTCSeconds()) + 'Z'
                : null;
            };

            String.prototype.toJSON =
            Number.prototype.toJSON =
            Boolean.prototype.toJSON = function (key) {
                return this.valueOf();
            };
        }

        var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        gap,
        indent,
        meta = {    // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"': '\\"',
            '\\': '\\\\'
        },
        rep;


        function quote(string) {

            // If the string contains no control characters, no quote characters, and no
            // backslash characters, then we can safely slap some quotes around it.
            // Otherwise we must also replace the offending characters with safe escape
            // sequences.

            escapable.lastIndex = 0;
            return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
                var c = meta[a];
                return typeof c === 'string'
                ? c
                : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
            }) + '"' : '"' + string + '"';
        }


        function str(key, holder) {

            // Produce a string from holder[key].

            var i,          // The loop counter.
            k,          // The member key.
            v,          // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];

            // If the value has a toJSON method, call it to obtain a replacement value.

            if (value && typeof value === 'object' &&
                typeof value.toJSON === 'function') {
                value = value.toJSON(key);
            }

            // If we were called with a replacer function, then call the replacer to
            // obtain a replacement value.

            if (typeof rep === 'function') {
                value = rep.call(holder, key, value);
            }

            // What happens next depends on the value's type.

            switch (typeof value) {
                case 'string':
                    return quote(value);

                case 'number':

                    // JSON numbers must be finite. Encode non-finite numbers as null.

                    return isFinite(value) ? String(value) : 'null';

                case 'boolean':
                case 'null':

                    // If the value is a boolean or null, convert it to a string. Note:
                    // typeof null does not produce 'null'. The case is included here in
                    // the remote chance that this gets fixed someday.

                    return String(value);

                    // If the type is 'object', we might be dealing with an object or an array or
                    // null.

                case 'object':

                    // Due to a specification blunder in ECMAScript, typeof null is 'object',
                    // so watch out for that case.

                    if (!value) {
                        return 'null';
                    }

                    // Make an array to hold the partial results of stringifying this object value.

                    gap += indent;
                    partial = [];

                    // Is the value an array?

                    if (Object.prototype.toString.apply(value) === '[object Array]') {

                        // The value is an array. Stringify every element. Use null as a placeholder
                        // for non-JSON values.

                        length = value.length;
                        for (i = 0; i < length; i += 1) {
                            partial[i] = str(i, value) || 'null';
                        }

                        // Join all of the elements together, separated with commas, and wrap them in
                        // brackets.

                        v = partial.length === 0
                    ? '[]'
                    : gap
                    ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']'
                    : '[' + partial.join(',') + ']';
                        gap = mind;
                        return v;
                    }

                    // If the replacer is an array, use it to select the members to be stringified.

                    if (rep && typeof rep === 'object') {
                        length = rep.length;
                        for (i = 0; i < length; i += 1) {
                            if (typeof rep[i] === 'string') {
                                k = rep[i];
                                v = str(k, value);
                                if (v) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                }
                            }
                        }
                    } else {

                        // Otherwise, iterate through all of the keys in the object.

                        for (k in value) {
                            if (Object.prototype.hasOwnProperty.call(value, k)) {
                                v = str(k, value);
                                if (v) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                }
                            }
                        }
                    }

                    // Join all of the member texts together, separated with commas,
                    // and wrap them in braces.

                    v = partial.length === 0
                ? '{}'
                : gap
                ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}'
                : '{' + partial.join(',') + '}';
                    gap = mind;
                    return v;
            }
        }

        // If the JSON object does not yet have a stringify method, give it one.

        if (typeof JSON.stringify !== 'function') {
            JSON.stringify = function (value, replacer, space) {

                // The stringify method takes a value and an optional replacer, and an optional
                // space parameter, and returns a JSON text. The replacer can be a function
                // that can replace values, or an array of strings that will select the keys.
                // A default replacer method can be provided. Use of the space parameter can
                // produce text that is more easily readable.

                var i;
                gap = '';
                indent = '';

                // If the space parameter is a number, make an indent string containing that
                // many spaces.

                if (typeof space === 'number') {
                    for (i = 0; i < space; i += 1) {
                        indent += ' ';
                    }

                    // If the space parameter is a string, it will be used as the indent string.

                } else if (typeof space === 'string') {
                    indent = space;
                }

                // If there is a replacer, it must be a function or an array.
                // Otherwise, throw an error.

                rep = replacer;
                if (replacer && typeof replacer !== 'function' &&
                    (typeof replacer !== 'object' ||
                    typeof replacer.length !== 'number')) {
                    throw new Error('JSON.stringify');
                }

                // Make a fake root object containing our value under the key of ''.
                // Return the result of stringifying the value.

                return str('', { '': value });
            };
        }


        // If the JSON object does not yet have a parse method, give it one.

        if (typeof JSON.parse !== 'function') {
            JSON.parse = function (text, reviver) {

                // The parse method takes a text and an optional reviver function, and returns
                // a JavaScript value if the text is a valid JSON text.

                var j;

                function walk(holder, key) {

                    // The walk method is used to recursively walk the resulting structure so
                    // that modifications can be made.

                    var k, v, value = holder[key];
                    if (value && typeof value === 'object') {
                        for (k in value) {
                            if (Object.prototype.hasOwnProperty.call(value, k)) {
                                v = walk(value, k);
                                if (v !== undefined) {
                                    value[k] = v;
                                } else {
                                    delete value[k];
                                }
                            }
                        }
                    }
                    return reviver.call(holder, key, value);
                }


                // Parsing happens in four stages. In the first stage, we replace certain
                // Unicode characters with escape sequences. JavaScript handles many characters
                // incorrectly, either silently deleting them, or treating them as line endings.

                text = String(text);
                cx.lastIndex = 0;
                if (cx.test(text)) {
                    text = text.replace(cx, function (a) {
                        return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                    });
                }

                // In the second stage, we run the text against regular expressions that look
                // for non-JSON patterns. We are especially concerned with '()' and 'new'
                // because they can cause invocation, and '=' because it can cause mutation.
                // But just to be safe, we want to reject all unexpected forms.

                // We split the second stage into 4 regexp operations in order to work around
                // crippling inefficiencies in IE's and Safari's regexp engines. First we
                // replace the JSON backslash pairs with '@' (a non-JSON character). Second, we
                // replace all simple value tokens with ']' characters. Third, we delete all
                // open brackets that follow a colon or comma or that begin the text. Finally,
                // we look to see that the remaining characters are only whitespace or ']' or
                // ',' or ':' or '{' or '}'. If that is so, then the text is safe for eval.

                if (/^[\],:{}\s]*$/
                    .test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
                        .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
                        .replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                    // In the third stage we use the eval function to compile the text into a
                    // JavaScript structure. The '{' operator is subject to a syntactic ambiguity
                    // in JavaScript: it can begin a block or an object literal. We wrap the text
                    // in parens to eliminate the ambiguity.

                    j = eval('(' + text + ')');

                    // In the optional fourth stage, we recursively walk the new structure, passing
                    // each name/value pair to a reviver function for possible transformation.

                    return typeof reviver === 'function'
                    ? walk({ '': j }, '')
                    : j;
                }

                // If the text is not JSON parseable, then a SyntaxError is thrown.

                throw new SyntaxError('JSON.parse');
            };
        }
    } ());

});
define('signalr',['jquery', 'json2'], function ($) {

    /*!
    * SignalR JavaScript Library v0.5
    * http://signalr.net/
    *
    * Copyright David Fowler and Damian Edwards 2012
    * Licensed under the MIT.
    * https://github.com/SignalR/SignalR/blob/master/LICENSE.md
    */

    /// <reference path="jquery-1.6.2.js" />
    (function ($, window) {
        /// <param name="$" type="jQuery" />
        

        if (typeof ($) !== "function") {
            // no jQuery!
            throw "SignalR: jQuery not found. Please ensure jQuery is referenced before the SignalR.js file.";
        }

        if (!window.JSON) {
            // no JSON!
            throw "SignalR: No JSON parser found. Please ensure json2.js is referenced before the SignalR.js file if you need to support clients without native JSON parsing support, e.g. IE<8.";
        }

        var signalR,
        _connection,
        events = {
            onStart: "onStart",
            onStarting: "onStarting",
            onSending: "onSending",
            onReceived: "onReceived",
            onError: "onError",
            onReconnect: "onReconnect",
            onDisconnect: "onDisconnect"
        },
        log = function (msg, logging) {
            if (logging === false) {
                return;
            }
            var m;
            if (typeof (window.console) === "undefined") {
                return;
            }
            m = "[" + new Date().toTimeString() + "] SignalR: " + msg;
            if (window.console.debug) {
                window.console.debug(m);
            } else if (window.console.log) {
                window.console.log(m);
            }
        };

        signalR = function (url, qs, logging) {
            /// <summary>Creates a new SignalR connection for the given url</summary>
            /// <param name="url" type="String">The URL of the long polling endpoint</param>
            /// <param name="qs" type="Object">
            ///     [Optional] Custom querystring parameters to add to the connection URL.
            ///     If an object, every non-function member will be added to the querystring.
            ///     If a string, it's added to the QS as specified.
            /// </param>
            /// <param name="logging" type="Boolean">
            ///     [Optional] A flag indicating whether connection logging is enabled to the browser
            ///     console/log. Defaults to false.
            /// </param>
            /// <returns type="signalR" />

            return new signalR.fn.init(url, qs, logging);
        };

        signalR.fn = signalR.prototype = {
            init: function (url, qs, logging) {
                this.url = url;
                this.qs = qs;
                if (typeof (logging) === "boolean") {
                    this.logging = logging;
                }
            },
            ajaxDataType: "json",

            logging: false,

            reconnectDelay: 2000,

            start: function (options, callback) {
                /// <summary>Starts the connection</summary>
                /// <param name="options" type="Object">Options map</param>
                /// <param name="callback" type="Function">A callback function to execute when the connection has started</param>
                var connection = this,
                config = {
                    transport: "auto",
                    xdomain: false
                },
                initialize,
                deferred = $.Deferred();

                if (connection.transport) {
                    // Already started, just return
                    deferred.resolve(connection);
                    return deferred.promise();
                }

                if ($.type(options) === "function") {
                    // Support calling with single callback parameter
                    callback = options;
                } else if ($.type(options) === "object") {
                    $.extend(config, options);
                    if ($.type(config.callback) === "function") {
                        callback = config.callback;
                    }
                }
                connection.ajaxDataType = config.xdomain ? "jsonp" : "json";

                $(connection).bind(events.onStart, function (e, data) {
                    if ($.type(callback) === "function") {
                        callback.call(connection);
                    }
                    deferred.resolve(connection);
                });

                initialize = function (transports, index) {
                    index = index || 0;
                    if (index >= transports.length) {
                        if (!connection.transport) {
                            // No transport initialized successfully
                            deferred.reject("SignalR: No transport could be initialized successfully. Try specifying a different transport or none at all for auto initialization.");
                        }
                        return;
                    }

                    var transportName = transports[index],
                    transport = $.type(transportName) === "object" ? transportName : signalR.transports[transportName];

                    transport.start(connection, function () {
                        connection.transport = transport;
                        $(connection).trigger(events.onStart);
                    }, function () {
                        initialize(transports, index + 1);
                    });
                };

                window.setTimeout(function () {
                    var url = connection.url + "/negotiate";
                    $.ajax({
                        url: url,
                        global: false,
                        cache: false,
                        type: "GET",
                        data: {},
                        dataType: connection.ajaxDataType,
                        error: function (error) {
                            $(connection).trigger(events.onError, [error]);
                            deferred.reject("SignalR: Error during negotiation request: " + error);
                        },
                        success: function (res) {
                            connection.appRelativeUrl = res.Url;
                            connection.id = res.ConnectionId;
                            connection.webSocketServerUrl = res.WebSocketServerUrl;

                            if (!res.ProtocolVersion || res.ProtocolVersion !== "1.0") {
                                $(connection).trigger(events.onError, "SignalR: Incompatible protocol version.");
                                deferred.reject("SignalR: Incompatible protocol version.");
                                return;
                            }

                            $(connection).trigger(events.onStarting);

                            var transports = [],
                            supportedTransports = [];

                            $.each(signalR.transports, function (key) {
                                if (key === "webSockets" && !res.TryWebSockets) {
                                    // Server said don't even try WebSockets, but keep processing the loop
                                    return true;
                                }
                                supportedTransports.push(key);
                            });

                            if ($.isArray(config.transport)) {
                                // ordered list provided
                                $.each(config.transport, function () {
                                    var transport = this;
                                    if ($.type(transport) === "object" || ($.type(transport) === "string" && $.inArray("" + transport, supportedTransports) >= 0)) {
                                        transports.push($.type(transport) === "string" ? "" + transport : transport);
                                    }
                                });
                            } else if ($.type(config.transport) === "object" ||
                                       $.inArray(config.transport, supportedTransports) >= 0) {
                                // specific transport provided, as object or a named transport, e.g. "longPolling"
                                transports.push(config.transport);
                            } else { // default "auto"
                                transports = supportedTransports;
                            }
                            initialize(transports);
                        }
                    });
                }, 0);

                return deferred.promise();
            },

            starting: function (callback) {
                /// <summary>Adds a callback that will be invoked before the connection is started</summary>
                /// <param name="callback" type="Function">A callback function to execute when the connection is starting</param>
                /// <returns type="signalR" />
                var connection = this,
                $connection = $(connection);

                $connection.bind(events.onStarting, function (e, data) {
                    callback.call(connection);
                    // Unbind immediately, we don't want to call this callback again
                    $connection.unbind(events.onStarting);
                });

                return connection;
            },

            send: function (data) {
                /// <summary>Sends data over the connection</summary>
                /// <param name="data" type="String">The data to send over the connection</param>
                /// <returns type="signalR" />
                var connection = this;

                if (!connection.transport) {
                    // Connection hasn't been started yet
                    throw "SignalR: Connection must be started before data can be sent. Call .start() before .send()";
                }

                connection.transport.send(connection, data);

                return connection;
            },

            sending: function (callback) {
                /// <summary>Adds a callback that will be invoked before anything is sent over the connection</summary>
                /// <param name="callback" type="Function">A callback function to execute before each time data is sent on the connection</param>
                /// <returns type="signalR" />
                var connection = this;
                $(connection).bind(events.onSending, function (e, data) {
                    callback.call(connection);
                });
                return connection;
            },

            received: function (callback) {
                /// <summary>Adds a callback that will be invoked after anything is received over the connection</summary>
                /// <param name="callback" type="Function">A callback function to execute when any data is received on the connection</param>
                /// <returns type="signalR" />
                var connection = this;
                $(connection).bind(events.onReceived, function (e, data) {
                    callback.call(connection, data);
                });
                return connection;
            },

            error: function (callback) {
                /// <summary>Adds a callback that will be invoked after an error occurs with the connection</summary>
                /// <param name="callback" type="Function">A callback function to execute when an error occurs on the connection</param>
                /// <returns type="signalR" />
                var connection = this;
                $(connection).bind(events.onError, function (e, data) {
                    callback.call(connection, data);
                });
                return connection;
            },

            disconnected: function (callback) {
                /// <summary>Adds a callback that will be invoked when the client disconnects</summary>
                /// <param name="callback" type="Function">A callback function to execute when the connection is broken</param>
                /// <returns type="signalR" />
                var connection = this;
                $(connection).bind(events.onDisconnect, function (e, data) {
                    callback.call(connection);
                });
                return connection;
            },

            reconnected: function (callback) {
                /// <summary>Adds a callback that will be invoked when the underlying transport reconnects</summary>
                /// <param name="callback" type="Function">A callback function to execute when the connection is restored</param>
                /// <returns type="signalR" />
                var connection = this;
                $(connection).bind(events.onReconnect, function (e, data) {
                    callback.call(connection);
                });
                return connection;
            },

            stop: function () {
                /// <summary>Stops listening</summary>
                /// <returns type="signalR" />
                var connection = this;

                if (connection.transport) {
                    connection.transport.stop(connection);
                    connection.transport = null;
                }

                delete connection.messageId;
                delete connection.groups;

                // Trigger the disconnect event
                $(connection).trigger(events.onDisconnect);

                return connection;
            },

            log: log
        };

        signalR.fn.init.prototype = signalR.fn;


        // Transports
        var transportLogic = {

            addQs: function (url, connection) {
                if (!connection.qs) {
                    return url;
                }

                if (typeof (connection.qs) === "object") {
                    return url + "&" + $.param(connection.qs);
                }

                if (typeof (connection.qs) === "string") {
                    return url + "&" + connection.qs;
                }

                return url + "&" + window.escape(connection.qs.toString());
            },

            getUrl: function (connection, transport, reconnecting, appendReconnectUrl) {
                /// <summary>Gets the url for making a GET based connect request</summary>
                var url = connection.url,
                qs = "transport=" + transport + "&connectionId=" + window.escape(connection.id);

                if (connection.data) {
                    qs += "&connectionData=" + window.escape(connection.data);
                }

                if (!reconnecting) {
                    url = url + "/connect";
                } else {
                    if (appendReconnectUrl) {
                        url = url + "/reconnect";
                    }

                    if (connection.messageId) {
                        qs += "&messageId=" + connection.messageId;
                    }
                    if (connection.groups) {
                        qs += "&groups=" + window.escape(JSON.stringify(connection.groups));
                    }
                }
                url += "?" + qs;
                url = this.addQs(url, connection);
                return url;
            },

            ajaxSend: function (connection, data) {

                var url = connection.url + "/send" + "?transport=" + connection.transport.name + "&connectionId=" + window.escape(connection.id);
                url = this.addQs(url, connection);
                $.ajax({
                    url: url,
                    global: false,
                    type: "POST",
                    dataType: connection.ajaxDataType,
                    data: {
                        data: data
                    },
                    success: function (result) {
                        if (result) {
                            $(connection).trigger(events.onReceived, [result]);
                        }
                    },
                    error: function (errData, textStatus) {
                        if (textStatus === "abort" ||
                        (textStatus === "parsererror" && connection.ajaxDataType === "jsonp")) {
                            // The parsererror happens for sends that don't return any data, and hence
                            // don't write the jsonp callback to the response. This is harder to fix on the server
                            // so just hack around it on the client for now.
                            return;
                        }
                        $(connection).trigger(events.onError, [errData]);
                    }
                });
            },

            processMessages: function (connection, data) {
                var $connection = $(connection);

                if (data) {
                    if (data.Disconnect) {
                        log("Disconnect command received from server", connection.logging);

                        // Disconnected by the server
                        connection.stop();

                        // Trigger the disconnect event
                        $connection.trigger(events.onDisconnect);
                        return;
                    }

                    if (data.Messages) {
                        $.each(data.Messages, function () {
                            try {
                                $connection.trigger(events.onReceived, [this]);
                            }
                            catch (e) {
                                log("Error raising received " + e, connection.logging);
                                $(connection).trigger(events.onError, [e]);
                            }
                        });
                    }

                    if (data.MessageId) {
                        connection.messageId = data.MessageId;
                    }

                    if (data.TransportData) {
                        connection.groups = data.TransportData.Groups;
                    }
                }
            },

            foreverFrame: {
                count: 0,
                connections: {}
            }
        };

        signalR.transports = {

            webSockets: {
                name: "webSockets",

                send: function (connection, data) {
                    connection.socket.send(data);
                },

                start: function (connection, onSuccess, onFailed) {
                    var url,
                    opened = false,
                    protocol;

                    if (window.MozWebSocket) {
                        window.WebSocket = window.MozWebSocket;
                    }

                    if (!window.WebSocket) {
                        onFailed();
                        return;
                    }

                    if (!connection.socket) {
                        if (connection.webSocketServerUrl) {
                            url = connection.webSocketServerUrl;
                        }
                        else {
                            // Determine the protocol
                            protocol = document.location.protocol === "https:" ? "wss://" : "ws://";

                            url = protocol + document.location.host + connection.appRelativeUrl;
                        }

                        // Build the url
                        $(connection).trigger(events.onSending);
                        if (connection.data) {
                            url += "?connectionData=" + connection.data + "&transport=webSockets&connectionId=" + connection.id;
                        } else {
                            url += "?transport=webSockets&connectionId=" + connection.id;
                        }

                        connection.socket = new window.WebSocket(url);
                        connection.socket.onopen = function () {
                            opened = true;
                            if (onSuccess) {
                                onSuccess();
                            }
                        };

                        connection.socket.onclose = function (event) {
                            if (!opened) {
                                if (onFailed) {
                                    onFailed();
                                }
                            } else if (typeof event.wasClean != "undefined" && event.wasClean === false) {
                                // Ideally this would use the websocket.onerror handler (rather than checking wasClean in onclose) but
                                // I found in some circumstances Chrome won't call onerror. This implementation seems to work on all browsers.
                                $(connection).trigger(events.onError);
                                // TODO: Support reconnect attempt here, need to ensure last message id, groups, and connection data go up on reconnect
                            }
                            connection.socket = null;
                        };

                        connection.socket.onmessage = function (event) {
                            var data = window.JSON.parse(event.data),
                            $connection;
                            if (data) {
                                $connection = $(connection);

                                if (data.Messages) {
                                    $.each(data.Messages, function () {
                                        try {
                                            $connection.trigger(events.onReceived, [this]);
                                        }
                                        catch (e) {
                                            log("Error raising received " + e, connection.logging);
                                        }
                                    });
                                } else {
                                    $connection.trigger(events.onReceived, [data]);
                                }
                            }
                        };
                    }
                },

                stop: function (connection) {
                    if (connection.socket !== null) {
                        connection.socket.close();
                        connection.socket = null;
                    }
                }
            },

            serverSentEvents: {
                name: "serverSentEvents",

                timeOut: 3000,

                start: function (connection, onSuccess, onFailed) {
                    var that = this,
                    opened = false,
                    $connection = $(connection),
                    reconnecting = !onSuccess,
                    url,
                    connectTimeOut;

                    if (connection.eventSource) {
                        connection.stop();
                    }

                    if (!window.EventSource) {
                        if (onFailed) {
                            onFailed();
                        }
                        return;
                    }

                    $connection.trigger(events.onSending);

                    url = transportLogic.getUrl(connection, this.name, reconnecting);

                    try {
                        connection.eventSource = new window.EventSource(url);
                    }
                    catch (e) {
                        log("EventSource failed trying to connect with error " + e.Message, connection.logging);
                        if (onFailed) {
                            // The connection failed, call the failed callback
                            onFailed();
                        }
                        else {
                            $connection.trigger(events.onError, [e]);
                            if (reconnecting) {
                                // If we were reconnecting, rather than doing initial connect, then try reconnect again
                                log("EventSource reconnecting", connection.logging);
                                that.reconnect(connection);
                            }
                        }
                        return;
                    }

                    // After connecting, if after the specified timeout there's no response stop the connection
                    // and raise on failed
                    connectTimeOut = window.setTimeout(function () {
                        if (opened === false) {
                            log("EventSource timed out trying to connect", connection.logging);

                            if (onFailed) {
                                onFailed();
                            }

                            if (reconnecting) {
                                // If we were reconnecting, rather than doing initial connect, then try reconnect again
                                log("EventSource reconnecting", connection.logging);
                                that.reconnect(connection);
                            } else {
                                that.stop(connection);
                            }
                        }
                    },
                that.timeOut);

                    connection.eventSource.addEventListener("open", function (e) {
                        log("EventSource connected", connection.logging);

                        if (connectTimeOut) {
                            window.clearTimeout(connectTimeOut);
                        }

                        if (opened === false) {
                            opened = true;

                            if (onSuccess) {
                                onSuccess();
                            }

                            if (reconnecting) {
                                $connection.trigger(events.onReconnect);
                            }
                        }
                    }, false);

                    connection.eventSource.addEventListener("message", function (e) {
                        // process messages
                        if (e.data === "initialized") {
                            return;
                        }
                        transportLogic.processMessages(connection, window.JSON.parse(e.data));
                    }, false);

                    connection.eventSource.addEventListener("error", function (e) {
                        if (!opened) {
                            if (onFailed) {
                                onFailed();
                            }
                            return;
                        }

                        log("EventSource readyState: " + connection.eventSource.readyState, connection.logging);

                        if (e.eventPhase === window.EventSource.CLOSED) {
                            // connection closed
                            if (connection.eventSource.readyState === window.EventSource.CONNECTING) {
                                // We don't use the EventSource's native reconnect function as it
                                // doesn't allow us to change the URL when reconnecting. We need
                                // to change the URL to not include the /connect suffix, and pass
                                // the last message id we received.
                                log("EventSource reconnecting due to the server connection ending", connection.logging);
                                that.reconnect(connection);
                            }
                            else {
                                // The EventSource has closed, either because its close() method was called,
                                // or the server sent down a "don't reconnect" frame.
                                log("EventSource closed", connection.logging);
                                that.stop(connection);
                            }
                        } else {
                            // connection error
                            log("EventSource error", connection.logging);
                            $connection.trigger(events.onError);
                        }
                    }, false);
                },

                reconnect: function (connection) {
                    var that = this;
                    window.setTimeout(function () {
                        that.stop(connection);
                        that.start(connection);
                    }, connection.reconnectDelay);
                },

                send: function (connection, data) {
                    transportLogic.ajaxSend(connection, data);
                },

                stop: function (connection) {
                    if (connection && connection.eventSource) {
                        connection.eventSource.close();
                        connection.eventSource = null;
                        delete connection.eventSource;
                    }
                }
            },

            foreverFrame: {
                name: "foreverFrame",

                timeOut: 3000,

                start: function (connection, onSuccess, onFailed) {
                    var that = this,
                    frameId = (transportLogic.foreverFrame.count += 1),
                    url,
                    connectTimeOut,
                    frame = $("<iframe data-signalr-connection-id='" + connection.id + "' style='position:absolute;top:0;left:0;width:0;height:0;visibility:hidden;'></iframe>");

                    if (window.EventSource) {
                        // If the browser supports SSE, don't use Forever Frame
                        if (onFailed) {
                            onFailed();
                        }
                        return;
                    }

                    $(connection).trigger(events.onSending);

                    // Build the url
                    url = transportLogic.getUrl(connection, this.name);
                    url += "&frameId=" + frameId;

                    frame.prop("src", url);
                    transportLogic.foreverFrame.connections[frameId] = connection;

                    frame.bind("readystatechange", function () {
                        if ($.inArray(this.readyState, ["loaded", "complete"]) >= 0) {
                            log("Forever frame iframe readyState changed to " + this.readyState + ", reconnecting", connection.logging);
                            that.reconnect(connection);
                        }
                    });

                    connection.frame = frame[0];
                    connection.frameId = frameId;

                    if (onSuccess) {
                        connection.onSuccess = onSuccess;
                    }

                    $("body").append(frame);

                    // After connecting, if after the specified timeout there's no response stop the connection
                    // and raise on failed
                    connectTimeOut = window.setTimeout(function () {
                        if (connection.onSuccess) {
                            that.stop(connection);

                            if (onFailed) {
                                onFailed();
                            }
                        }
                    }, that.timeOut);
                },

                reconnect: function (connection) {
                    var that = this;
                    window.setTimeout(function () {
                        var frame = connection.frame,
                        src = transportLogic.getUrl(connection, that.name, true) + "&frameId=" + connection.frameId;
                        frame.src = src;
                    }, connection.reconnectDelay);
                },

                send: function (connection, data) {
                    transportLogic.ajaxSend(connection, data);
                },

                receive: transportLogic.processMessages,

                stop: function (connection) {
                    if (connection.frame) {
                        if (connection.frame.stop) {
                            connection.frame.stop();
                        } else if (connection.frame.document && connection.frame.document.execCommand) {
                            connection.frame.document.execCommand("Stop");
                        }
                        $(connection.frame).remove();
                        delete transportLogic.foreverFrame.connections[connection.frameId];
                        connection.frame = null;
                        connection.frameId = null;
                        delete connection.frame;
                        delete connection.frameId;
                    }
                },

                getConnection: function (id) {
                    return transportLogic.foreverFrame.connections[id];
                },

                started: function (connection) {
                    if (connection.onSuccess) {
                        connection.onSuccess();
                        connection.onSuccess = null;
                        delete connection.onSuccess;
                    }
                    else {
                        // If there's no onSuccess handler we assume this is a reconnect
                        $(connection).trigger(events.onReconnect);
                    }
                }
            },

            longPolling: {
                name: "longPolling",

                reconnectDelay: 3000,

                start: function (connection, onSuccess, onFailed) {
                    /// <summary>Starts the long polling connection</summary>
                    /// <param name="connection" type="signalR">The SignalR connection to start</param>
                    var that = this;
                    if (connection.pollXhr) {
                        connection.stop();
                    }

                    connection.messageId = null;

                    window.setTimeout(function () {
                        (function poll(instance, raiseReconnect) {
                            $(instance).trigger(events.onSending);

                            var messageId = instance.messageId,
                            connect = (messageId === null),
                            url = transportLogic.getUrl(instance, that.name, !connect, raiseReconnect),
                            reconnectTimeOut = null,
                            reconnectFired = false;

                            instance.pollXhr = $.ajax({
                                url: url,
                                global: false,
                                type: "GET",
                                dataType: connection.ajaxDataType,
                                success: function (data) {
                                    var delay = 0,
                                    timedOutReceived = false;

                                    if (raiseReconnect === true) {
                                        // Fire the reconnect event if it hasn't been fired as yet
                                        if (reconnectFired === false) {
                                            $(instance).trigger(events.onReconnect);
                                            reconnectFired = true;
                                        }
                                    }

                                    transportLogic.processMessages(instance, data);
                                    if (data &&
                                    data.TransportData &&
                                    $.type(data.TransportData.LongPollDelay) === "number") {
                                        delay = data.TransportData.LongPollDelay;
                                    }

                                    if (data && data.TimedOut) {
                                        timedOutReceived = data.TimedOut;
                                    }

                                    if (delay > 0) {
                                        window.setTimeout(function () {
                                            poll(instance, timedOutReceived);
                                        }, delay);
                                    } else {
                                        poll(instance, timedOutReceived);
                                    }
                                },

                                error: function (data, textStatus) {
                                    if (textStatus === "abort") {
                                        return;
                                    }

                                    if (reconnectTimeOut) {
                                        // If the request failed then we clear the timeout so that the 
                                        // reconnect event doesn't get fired
                                        clearTimeout(reconnectTimeOut);
                                    }

                                    $(instance).trigger(events.onError, [data]);

                                    window.setTimeout(function () {
                                        poll(instance, true);
                                    }, connection.reconnectDelay);
                                }
                            });

                            if (raiseReconnect === true) {
                                reconnectTimeOut = window.setTimeout(function () {
                                    if (reconnectFired === false) {
                                        $(instance).trigger(events.onReconnect);
                                        reconnectFired = true;
                                    }
                                },
                            that.reconnectDelay);
                            }

                        } (connection));

                        // Now connected
                        // There's no good way know when the long poll has actually started so 
                        // we assume it only takes around 150ms (max) to start the connection
                        window.setTimeout(onSuccess, 150);

                    }, 250); // Have to delay initial poll so Chrome doesn't show loader spinner in tab
                },

                send: function (connection, data) {
                    transportLogic.ajaxSend(connection, data);
                },

                stop: function (connection) {
                    /// <summary>Stops the long polling connection</summary>
                    /// <param name="connection" type="signalR">The SignalR connection to stop</param>
                    if (connection.pollXhr) {
                        connection.pollXhr.abort();
                        connection.pollXhr = null;
                        delete connection.pollXhr;
                    }
                }
            }
        };

        signalR.noConflict = function () {
            /// <summary>Reinstates the original value of $.connection and returns the signalR object for manual assignment</summary>
            /// <returns type="signalR" />
            if ($.connection === signalR) {
                $.connection = _connection;
            }
            return signalR;
        };

        if ($.connection) {
            _connection = $.connection;
        }

        $.connection = $.signalR = signalR;

    } (window.jQuery, window));

});
/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../libs/jquery.signalr/jquery-1.6.2-vsdoc.js" />

// Bowerbird.app
// -------------

// Initialises the app, but does not start rendering. That is done 
// when app.start() is called
define('app',['jquery', 'underscore', 'backbone', 'ich', 'bootstrap-data', 'models/user', 'collections/usercollection', 'collections/projectcollection', 'collections/teamcollection', 'collections/organisationcollection', 'collections/activitycollection', 'collections/exploreprojectcollection', 'collections/chatcollection', 'models/mediaresource', 'marionette', 'signalr'],
function ($, _, Backbone, ich, bootstrapData, User, UserCollection, ProjectCollection, TeamCollection, OrganisationCollection, ActivityCollection, ExploreProjectCollection, ChatCollection, MediaResource) {
    // Create an instance of the app
    var app = new Backbone.Marionette.Application();

    // Let's pollute the global namespace, just a little, for debug purposes :)
    window.Bowerbird = window.Bowerbird || {};
    window.Bowerbird.app = app;

    var AuthenticatedUser = function (data) {
        this.user = new User(data.User);
        this.memberships = data.Memberships;
        this.projects = new ProjectCollection(data.Projects);
        this.teams = new TeamCollection(data.Teams);
        this.organisations = new OrganisationCollection(data.Organisations);
        this.appRoot = data.AppRoot;

        this.hasGroupPermission = function (groupId, permissionId) {
            var membership = _.find(this.memberships, function (m) {
                return m.GroupId === groupId;
            });
            if (!membership) {
                return false;
            }
            return _.any(membership.PermissionIds, function (p) {
                return p === permissionId;
            });
        };
    };

    app.vent.on('newactivity:groupadded', function (activity) {
        var group = activity.get('GroupAdded').Group;
        if (group.GroupType === 'project') {
            app.vent.trigger('projectAdded:', group);
            if (group.User.Id == app.authenticatedUser.user.id) {
                app.authenticatedUser.projects.add(group);
            }
        }
        if (group.GroupType === 'team') {
            app.vent.trigger('teamAdded:', group);
            if (group.User.Id == app.authenticatedUser.user.id) {
                app.authenticatedUser.teams.add(group);
            }
        }
        if (group.GroupType === 'organisation') {
            app.vent.trigger('organisationAdded:', group);
            if (group.User.Id == app.authenticatedUser.user.id) {
                app.authenticatedUser.organisations.add(group);
            }
        }

    }, this);

    app.addRegions({
        header: 'header',
        footer: 'footer',
        sidebar: '#sidebar',
        content: '#content',
        notifications: '#notifications',
        usersonline: '#onlineusers'
    });

    app.isPrerendering = function (name) {
        return name === app.prerenderedView.name && !app.prerenderedView.isBound;
    };

    app.setPrerenderComplete = function () {
        app.prerenderedView.isBound = true;
    };

    app.getShowViewMethodName = function (name) {
        if (!name) {
            var err = new Error("A name must be provided!");
            err.name = "BowerbirdNoViewNameProvidedError";
            throw err;
        }
        return app.isPrerendering(name) ? 'attachView' : 'show';
    };

    app.updateTitle = function (titleSegment) {
        var newTitle = 'Bowerbird';
        if (titleSegment.length > 0) {
            newTitle = titleSegment + ' - ' + newTitle;
        }
        document.title = newTitle;
    };

    app.routeHistory = [];
    app.previousContent = null;

    app.showFormContentView = function (view, name) {
        if (app.content.currentView) {
            // Store previous view in cache
            app.previousContent = {
                view: app.content.currentView,
                $el: app.content.currentView.$el.detach()
            };

            // Clear out current view
            app.content.currentView = null;
        }

        // Show new view
        app.content[app.getShowViewMethodName(name)](view);
    };

    app.showPreviousContentView = function () {
        if (app.previousContent) {
            // Close current view
            app.content.close();

            // Reinstate previous view
            app.content.$el.append(app.previousContent.$el);
            app.content.currentView = app.previousContent.view;
            app.routeHistory.shift();
            app.homeRouter.navigate(_.first(app.routeHistory));

            // Clear out previous view cache
            app.previousContent = null;
        } else {
            // If we don't have a previous view, take user back to home stream
            app.homeRouter.navigate('', { trigger: true });
        }
    };

    app.generateGuid = function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    };

    // Load the bootstrapped data into place
    app.bind('initialize:before', function () {
        // Override the marionette renderer so that it uses mustache templates 
        // together with icanhaz caching
        Backbone.Marionette.Renderer.render = function (template, data) {
            if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
                return ich[template](data);
            }
        };

        // Add additional capability if authenticated user
        if (bootstrapData.AuthenticatedUser) {
            // Add the authenticated user to the app for future reference
            app.authenticatedUser = new AuthenticatedUser(bootstrapData.AuthenticatedUser);

            // Online users
            app.onlineUsers = new UserCollection();

            // Chats
            app.chats = new ChatCollection();
            app.chatRegions = [];
        }

        // Add the prerendered view string to the app for use by controller duing init of first view
        app.prerenderedView = {
            name: bootstrapData.PrerenderedView,
            isBound: false, // Flag used to determine if prerenderd view has been bound to the object/DOM model
            data: bootstrapData.Model
        };
    });

    app.bind('initialize:after', function () {
        // Tasks to perform on DOM ready
        $(function () {
            // Only start history once app is fully initialised
            if (Backbone.history) {
                Backbone.history.on('route', function (route, name) {
                    app.routeHistory.unshift(Backbone.history.fragment);
                });

                // Start URL and history routing
                Backbone.history.start({ pushState: true });
            }

            // initialise the hub connection
            //$.connection.hub.start({ transport: 'longPolling' }, function () {
            $.connection.hub.start({ transport: ['webSockets', 'longPolling'] }, function () {

                // Keep the client id
                app.clientId = $.signalR.hub.id;
                log('browser connected via signalr as ' + app.clientId);

                // Subscribe authenticated user to all their groups
                if (app.authenticatedUser) {
                    $.connection.userHub.registerUserClient(app.authenticatedUser.user.id)
                        .done(function () {
                            log('Added user to hub');
                        })
                        .fail(function (e) {
                            log('could not register client with hub', e);
                        });
                }
            });

            // Register closing of all popup menus in entire page
            $("body").click(function () {
                $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
            });
        });
    });

    return app;

});
define('log',[],function () {

    // usage: log('inside coolFunc', this, arguments);
    // paulirish.com/2009/log-a-lightweight-wrapper-for-consolelog/
    var logger = function () {
        log.history = log.history || [];   // store logs to an array for reference
        log.history.push(arguments);
        if (this.console) console.log(Array.prototype.slice.call(arguments));
    };

    window.log = logger;

    return logger;

});
/*!
 * RequireJS plugin for loading files without adding the JS extension, useful for
 * JSONP services and any other kind of resource that already contain a file
 * extension or that shouldn't have one (like dynamic scripts).
 * @author Miller Medeiros
 * @version 0.3.0 (2011/10/26)
 * Released under the WTFPL <http://sam.zoy.org/wtfpl/>
 */
define('noext',[],function(){
    
    var QUERY_PARAM = 'noext';

    //API
    return {
        load : function(name, req, onLoad, config){
            var url = req.toUrl(name).replace(/\.js$/, '');
            req([url], function(mod){
                onLoad(mod);
            });
        },
        normalize : function(name, norm){
            //append query string to avoid adding .js extension
            name += (name.indexOf('?') < 0)? '?' : '&';
            return name + QUERY_PARAM +'=1';
        }

    };
});
/*
 * JavaScript Load Image 1.1.6
 * https://github.com/blueimp/JavaScript-Load-Image
 *
 * Copyright 2011, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true */
/*global window, document, URL, webkitURL, Blob, File, FileReader, define */

(function ($) {
    

    // Loads an image for a given File object.
    // Invokes the callback with an img or optional canvas
    // element (if supported by the browser) as parameter:
    var loadImage = function (file, callback, options) {
            var img = document.createElement('img'),
                url,
                oUrl;
            img.onerror = callback;
            img.onload = function () {
                if (oUrl) {
                    loadImage.revokeObjectURL(oUrl);
                }
                callback(loadImage.scale(img, options));
            };
            if ((window.Blob && file instanceof Blob) ||
                // Files are also Blob instances, but some browsers
                // (Firefox 3.6) support the File API but not Blobs:
                    (window.File && file instanceof File)) {
                url = oUrl = loadImage.createObjectURL(file);
            } else {
                url = file;
            }
            if (url) {
                img.src = url;
                return img;
            } else {
                return loadImage.readFile(file, function (url) {
                    img.src = url;
                });
            }
        },
        urlAPI = (window.createObjectURL && window) ||
            (window.URL && URL) || (window.webkitURL && webkitURL);

    // Scales the given image (img or canvas HTML element)
    // using the given options.
    // Returns a canvas object if the browser supports canvas
    // and the canvas option is true or a canvas object is passed
    // as image, else the scaled image:
    loadImage.scale = function (img, options) {
        options = options || {};
        var canvas = document.createElement('canvas'),
            width = img.width,
            height = img.height,
            scale = Math.max(
                (options.minWidth || width) / width,
                (options.minHeight || height) / height
            );
        if (scale > 1) {
            width = parseInt(width * scale, 10);
            height = parseInt(height * scale, 10);
        }
        scale = Math.min(
            (options.maxWidth || width) / width,
            (options.maxHeight || height) / height
        );
        if (scale < 1) {
            width = parseInt(width * scale, 10);
            height = parseInt(height * scale, 10);
        }
        if (img.getContext || (options.canvas && canvas.getContext)) {
            canvas.width = width;
            canvas.height = height;
            canvas.getContext('2d')
                .drawImage(img, 0, 0, width, height);
            return canvas;
        }
        img.width = width;
        img.height = height;
        return img;
    };

    loadImage.createObjectURL = function (file) {
        return urlAPI ? urlAPI.createObjectURL(file) : false;
    };

    loadImage.revokeObjectURL = function (url) {
        return urlAPI ? urlAPI.revokeObjectURL(url) : false;
    };

    // Loads a given File object via FileReader interface,
    // invokes the callback with a data url:
    loadImage.readFile = function (file, callback) {
        if (window.FileReader && FileReader.prototype.readAsDataURL) {
            var fileReader = new FileReader();
            fileReader.onload = function (e) {
                callback(e.target.result);
            };
            fileReader.readAsDataURL(file);
            return fileReader;
        }
        return false;
    };

    if (typeof define !== 'undefined' && define.amd) {
        define('loadimage',[],function () {
            return loadImage;
        });
    } else {
        $.loadImage = loadImage;
    }
}(this));

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AvatarItemView
// -------------------------

define('views/avataritemview',['jquery', 'underscore', 'backbone', 'app', 'ich'], function ($, _, Backbone, app, ich) {

    var AvatarItemView = Backbone.View.extend({

        className: 'avatar-uploaded',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        //template: "AvatarUploaded",

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'showTempMedia',
            '_showUploadedMedia'
            );
            this.mediaResource = options.model;
            log('avatarItemView.initialize: mediaResource >');
            log(this.mediaResource);
            //this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
        },

        render: function () {
            log('avatarItemView:render');
            //            this.$el.append(ich.AvatarUploaded(this.MediaResource.toJSON()));
            //            return this;
            this._showUploadedMedia();
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.remove();
            $('#avatar-add-pane').show();
        },

        showTempMedia: function (img) {
            log('avatarItemView:_showTempMedia');
            this.$el.find('div:first-child img').replaceWith($(img));
        },

        _showUploadedMedia: function (mediaResource) {
            log('avatarItemView:_showUploadedMedia');
            this.$el.find('div:first-child img').replaceWith($('<img src="' + this.mediaResource.get('MediumImageUri') + '" alt="" />'));
        }
    });

    return AvatarItemView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/icanhaz/icanhaz.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditAvatarView
// -------------------------

define('views/editavatarview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/avataritemview', 'loadimage', 'models/mediaresource'], function ($, _, Backbone, app, ich, AvatarItemView, loadImage, MediaResource) {

    var EditAvatarView = Backbone.View.extend({

        id: 'avatar-fieldset',

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'render',
            '_initMediaUploader',
            '_onUploadDone',
            '_onSubmitUpload',
            '_onUploadAdd'
            );
            this.model = options.model;
            this.currentUploadKey = 0;
            this.avatarItemView = null;
            log('EditAvatarView.Model is...:');
            log(this.model);
        },

        render: function () {
            log('editAvatarView:render');
            this._initMediaUploader();
            $('#avatar-viewer').append('<img src="' + this.model.get('Avatar').Files.ThumbnailMedium.RelativeUri + '" />');
            return this;
        },

        _initMediaUploader: function () {
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources/avatarupload',
                add: this._onUploadAd,
                submit: this._onSubmitUpload,
                done: this._onUploadDone,
                limitConcurrentUploads: 1
            });
        },

        _onUploadAdd: function (e, data) {
            log('editAvatarView:_onUploadAdd');
            this.currentUploadKey++;
            var mediaResource = new MediaResource({ Key: this.currentUploadKey.toString() });
            //this.model.setAvatar(mediaResource);
            var self = this;
            var tempImage = loadImage(
                data.files[0],
                function (img) {
                    if (img.type === "error") {
                        //log('Error loading image', img);
                    } else {
                        self.filesAdded++;
                        mediaResourceItemView.showTempMedia(img);
                        self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), self.filesAdded === data.originalFiles.length);
                    }
                },
                { maxHeight: 220 }
            );

            if (!tempImage) {
                alert('No support for file/blob API!');
            }

            data.submit();
        },

        _onSubmitUpload: function (e, data) {
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
        },

        _onUploadDone: function (e, data) {
            log('editAvatarView:_onUploadDone');
            this.model.set('AvatarId', data.result.Id);
            var mediaResource = new MediaResource(data.result);
            //this.$el.find('#avatar-viewer img').replaceWith($('<img src="' + mediaResource.get('ProfileImageUri') + '" alt="" />'));
            $('#avatar-viewer').empty().append('<img src="' + mediaResource.get('Files').ThumbnailMedium.RelativeUri + '" width="200px;" />');
        }
    });

    return EditAvatarView;
});
define('jqueryui/widget', ['jquery'], function (jQuery) {
/*!
 * jQuery UI Widget 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Widget
 */
(function( $, undefined ) {

// jQuery 1.4+
if ( $.cleanData ) {
	var _cleanData = $.cleanData;
	$.cleanData = function( elems ) {
		for ( var i = 0, elem; (elem = elems[i]) != null; i++ ) {
			$( elem ).triggerHandler( "remove" );
		}
		_cleanData( elems );
	};
} else {
	var _remove = $.fn.remove;
	$.fn.remove = function( selector, keepData ) {
		return this.each(function() {
			if ( !keepData ) {
				if ( !selector || $.filter( selector, [ this ] ).length ) {
					$( "*", this ).add( [ this ] ).each(function() {
						$( this ).triggerHandler( "remove" );
					});
				}
			}
			return _remove.call( $(this), selector, keepData );
		});
	};
}

$.widget = function( name, base, prototype ) {
	var namespace = name.split( "." )[ 0 ],
		fullName;
	name = name.split( "." )[ 1 ];
	fullName = namespace + "-" + name;

	if ( !prototype ) {
		prototype = base;
		base = $.Widget;
	}

	// create selector for plugin
	$.expr[ ":" ][ fullName ] = function( elem ) {
		return !!$.data( elem, name );
	};

	$[ namespace ] = $[ namespace ] || {};
	$[ namespace ][ name ] = function( options, element ) {
		// allow instantiation without initializing for simple inheritance
		if ( arguments.length ) {
			this._createWidget( options, element );
		}
	};

	var basePrototype = new base();
	// we need to make the options hash a property directly on the new instance
	// otherwise we'll modify the options hash on the prototype that we're
	// inheriting from
//	$.each( basePrototype, function( key, val ) {
//		if ( $.isPlainObject(val) ) {
//			basePrototype[ key ] = $.extend( {}, val );
//		}
//	});
	basePrototype.options = $.extend( true, {}, basePrototype.options );
	$[ namespace ][ name ].prototype = $.extend( true, basePrototype, {
		namespace: namespace,
		widgetName: name,
		widgetEventPrefix: $[ namespace ][ name ].prototype.widgetEventPrefix || name,
		widgetBaseClass: fullName
	}, prototype );

	$.widget.bridge( name, $[ namespace ][ name ] );
};

$.widget.bridge = function( name, object ) {
	$.fn[ name ] = function( options ) {
		var isMethodCall = typeof options === "string",
			args = Array.prototype.slice.call( arguments, 1 ),
			returnValue = this;

		// allow multiple hashes to be passed on init
		options = !isMethodCall && args.length ?
			$.extend.apply( null, [ true, options ].concat(args) ) :
			options;

		// prevent calls to internal methods
		if ( isMethodCall && options.charAt( 0 ) === "_" ) {
			return returnValue;
		}

		if ( isMethodCall ) {
			this.each(function() {
				var instance = $.data( this, name ),
					methodValue = instance && $.isFunction( instance[options] ) ?
						instance[ options ].apply( instance, args ) :
						instance;
				// TODO: add this back in 1.9 and use $.error() (see #5972)
//				if ( !instance ) {
//					throw "cannot call methods on " + name + " prior to initialization; " +
//						"attempted to call method '" + options + "'";
//				}
//				if ( !$.isFunction( instance[options] ) ) {
//					throw "no such method '" + options + "' for " + name + " widget instance";
//				}
//				var methodValue = instance[ options ].apply( instance, args );
				if ( methodValue !== instance && methodValue !== undefined ) {
					returnValue = methodValue;
					return false;
				}
			});
		} else {
			this.each(function() {
				var instance = $.data( this, name );
				if ( instance ) {
					instance.option( options || {} )._init();
				} else {
					$.data( this, name, new object( options, this ) );
				}
			});
		}

		return returnValue;
	};
};

$.Widget = function( options, element ) {
	// allow instantiation without initializing for simple inheritance
	if ( arguments.length ) {
		this._createWidget( options, element );
	}
};

$.Widget.prototype = {
	widgetName: "widget",
	widgetEventPrefix: "",
	options: {
		disabled: false
	},
	_createWidget: function( options, element ) {
		// $.widget.bridge stores the plugin instance, but we do it anyway
		// so that it's stored even before the _create function runs
		$.data( element, this.widgetName, this );
		this.element = $( element );
		this.options = $.extend( true, {},
			this.options,
			this._getCreateOptions(),
			options );

		var self = this;
		this.element.bind( "remove." + this.widgetName, function() {
			self.destroy();
		});

		this._create();
		this._trigger( "create" );
		this._init();
	},
	_getCreateOptions: function() {
		return $.metadata && $.metadata.get( this.element[0] )[ this.widgetName ];
	},
	_create: function() {},
	_init: function() {},

	destroy: function() {
		this.element
			.unbind( "." + this.widgetName )
			.removeData( this.widgetName );
		this.widget()
			.unbind( "." + this.widgetName )
			.removeAttr( "aria-disabled" )
			.removeClass(
				this.widgetBaseClass + "-disabled " +
				"ui-state-disabled" );
	},

	widget: function() {
		return this.element;
	},

	option: function( key, value ) {
		var options = key;

		if ( arguments.length === 0 ) {
			// don't return a reference to the internal hash
			return $.extend( {}, this.options );
		}

		if  (typeof key === "string" ) {
			if ( value === undefined ) {
				return this.options[ key ];
			}
			options = {};
			options[ key ] = value;
		}

		this._setOptions( options );

		return this;
	},
	_setOptions: function( options ) {
		var self = this;
		$.each( options, function( key, value ) {
			self._setOption( key, value );
		});

		return this;
	},
	_setOption: function( key, value ) {
		this.options[ key ] = value;

		if ( key === "disabled" ) {
			this.widget()
				[ value ? "addClass" : "removeClass"](
					this.widgetBaseClass + "-disabled" + " " +
					"ui-state-disabled" )
				.attr( "aria-disabled", value );
		}

		return this;
	},

	enable: function() {
		return this._setOption( "disabled", false );
	},
	disable: function() {
		return this._setOption( "disabled", true );
	},

	_trigger: function( type, event, data ) {
		var callback = this.options[ type ];

		event = $.Event( event );
		event.type = ( type === this.widgetEventPrefix ?
			type :
			this.widgetEventPrefix + type ).toLowerCase();
		data = data || {};

		// copy original event properties over to the new event
		// this would happen if we could call $.event.fix instead of $.Event
		// but we don't have a way to force an event to be fixed multiple times
		if ( event.originalEvent ) {
			for ( var i = $.event.props.length, prop; i; ) {
				prop = $.event.props[ --i ];
				event[ prop ] = event.originalEvent[ prop ];
			}
		}

		this.element.trigger( event, data );

		return !( $.isFunction(callback) &&
			callback.call( this.element[0], event, data ) === false ||
			event.isDefaultPrevented() );
	}
};

})( jQuery );

});
/*
 * jQuery File Upload Plugin 5.8.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, unparam: true, regexp: true */
/*global define, window, document, XMLHttpRequestUpload, Blob, File, FormData, location */

(function (factory) {
    
    if (typeof define === 'function' && define.amd) {
        // Register as an anonymous AMD module:
        define('fileupload',[
            'jquery',
            'jqueryui/widget' // FR 8/05/2012: Changed reference to follow our folder AMD pattern
        ], factory);
    } else {
        // Browser globals:
        factory(window.jQuery);
    }
}(function ($) {
    

    // The fileupload widget listens for change events on file input fields defined
    // via fileInput setting and paste or drop events of the given dropZone.
    // In addition to the default jQuery Widget methods, the fileupload widget
    // exposes the "add" and "send" methods, to add or directly send files using
    // the fileupload API.
    // By default, files added via file input selection, paste, drag & drop or
    // "add" method are uploaded immediately, but it is possible to override
    // the "add" callback option to queue file uploads.
    $.widget('blueimp.fileupload', {

        options: {
            // The namespace used for event handler binding on the dropZone and
            // fileInput collections.
            // If not set, the name of the widget ("fileupload") is used.
            namespace: undefined,
            // The drop target collection, by the default the complete document.
            // Set to null or an empty collection to disable drag & drop support:
            dropZone: $(document),
            // The file input field collection, that is listened for change events.
            // If undefined, it is set to the file input fields inside
            // of the widget element on plugin initialization.
            // Set to null or an empty collection to disable the change listener.
            fileInput: undefined,
            // By default, the file input field is replaced with a clone after
            // each input field change event. This is required for iframe transport
            // queues and allows change events to be fired for the same file
            // selection, but can be disabled by setting the following option to false:
            replaceFileInput: true,
            // The parameter name for the file form data (the request argument name).
            // If undefined or empty, the name property of the file input field is
            // used, or "files[]" if the file input name property is also empty:
            paramName: undefined,
            // By default, each file of a selection is uploaded using an individual
            // request for XHR type uploads. Set to false to upload file
            // selections in one request each:
            singleFileUploads: true,
            // To limit the number of files uploaded with one XHR request,
            // set the following option to an integer greater than 0:
            limitMultiFileUploads: undefined,
            // Set the following option to true to issue all file upload requests
            // in a sequential order:
            sequentialUploads: false,
            // To limit the number of concurrent uploads,
            // set the following option to an integer greater than 0:
            limitConcurrentUploads: undefined,
            // Set the following option to true to force iframe transport uploads:
            forceIframeTransport: false,
            // Set the following option to the location of a redirect url on the
            // origin server, for cross-domain iframe transport uploads:
            redirect: undefined,
            // The parameter name for the redirect url, sent as part of the form
            // data and set to 'redirect' if this option is empty:
            redirectParamName: undefined,
            // Set the following option to the location of a postMessage window,
            // to enable postMessage transport uploads:
            postMessage: undefined,
            // By default, XHR file uploads are sent as multipart/form-data.
            // The iframe transport is always using multipart/form-data.
            // Set to false to enable non-multipart XHR uploads:
            multipart: true,
            // To upload large files in smaller chunks, set the following option
            // to a preferred maximum chunk size. If set to 0, null or undefined,
            // or the browser does not support the required Blob API, files will
            // be uploaded as a whole.
            maxChunkSize: undefined,
            // When a non-multipart upload or a chunked multipart upload has been
            // aborted, this option can be used to resume the upload by setting
            // it to the size of the already uploaded bytes. This option is most
            // useful when modifying the options object inside of the "add" or
            // "send" callbacks, as the options are cloned for each file upload.
            uploadedBytes: undefined,
            // By default, failed (abort or error) file uploads are removed from the
            // global progress calculation. Set the following option to false to
            // prevent recalculating the global progress data:
            recalculateProgress: true,

            // Additional form data to be sent along with the file uploads can be set
            // using this option, which accepts an array of objects with name and
            // value properties, a function returning such an array, a FormData
            // object (for XHR file uploads), or a simple object.
            // The form of the first fileInput is given as parameter to the function:
            formData: function (form) {
                return form.serializeArray();
            },

            // The add callback is invoked as soon as files are added to the fileupload
            // widget (via file input selection, drag & drop, paste or add API call).
            // If the singleFileUploads option is enabled, this callback will be
            // called once for each file in the selection for XHR file uplaods, else
            // once for each file selection.
            // The upload starts when the submit method is invoked on the data parameter.
            // The data object contains a files property holding the added files
            // and allows to override plugin options as well as define ajax settings.
            // Listeners for this callback can also be bound the following way:
            // .bind('fileuploadadd', func);
            // data.submit() returns a Promise object and allows to attach additional
            // handlers using jQuery's Deferred callbacks:
            // data.submit().done(func).fail(func).always(func);
            add: function (e, data) {
                data.submit();
            },

            // Other callbacks:
            // Callback for the submit event of each file upload:
            // submit: function (e, data) {}, // .bind('fileuploadsubmit', func);
            // Callback for the start of each file upload request:
            // send: function (e, data) {}, // .bind('fileuploadsend', func);
            // Callback for successful uploads:
            // done: function (e, data) {}, // .bind('fileuploaddone', func);
            // Callback for failed (abort or error) uploads:
            // fail: function (e, data) {}, // .bind('fileuploadfail', func);
            // Callback for completed (success, abort or error) requests:
            // always: function (e, data) {}, // .bind('fileuploadalways', func);
            // Callback for upload progress events:
            // progress: function (e, data) {}, // .bind('fileuploadprogress', func);
            // Callback for global upload progress events:
            // progressall: function (e, data) {}, // .bind('fileuploadprogressall', func);
            // Callback for uploads start, equivalent to the global ajaxStart event:
            // start: function (e) {}, // .bind('fileuploadstart', func);
            // Callback for uploads stop, equivalent to the global ajaxStop event:
            // stop: function (e) {}, // .bind('fileuploadstop', func);
            // Callback for change events of the fileInput collection:
            // change: function (e, data) {}, // .bind('fileuploadchange', func);
            // Callback for paste events to the dropZone collection:
            // paste: function (e, data) {}, // .bind('fileuploadpaste', func);
            // Callback for drop events of the dropZone collection:
            // drop: function (e, data) {}, // .bind('fileuploaddrop', func);
            // Callback for dragover events of the dropZone collection:
            // dragover: function (e) {}, // .bind('fileuploaddragover', func);

            // The plugin options are used as settings object for the ajax calls.
            // The following are jQuery ajax settings required for the file uploads:
            processData: false,
            contentType: false,
            cache: false
        },

        // A list of options that require a refresh after assigning a new value:
        _refreshOptionsList: ['namespace', 'dropZone', 'fileInput'],

        _isXHRUpload: function (options) {
            var undef = 'undefined';
            return !options.forceIframeTransport &&
                typeof XMLHttpRequestUpload !== undef && typeof File !== undef &&
                (!options.multipart || typeof FormData !== undef);
        },

        _getFormData: function (options) {
            var formData;
            if (typeof options.formData === 'function') {
                return options.formData(options.form);
            } else if ($.isArray(options.formData)) {
                return options.formData;
            } else if (options.formData) {
                formData = [];
                $.each(options.formData, function (name, value) {
                    formData.push({name: name, value: value});
                });
                return formData;
            }
            return [];
        },

        _getTotal: function (files) {
            var total = 0;
            $.each(files, function (index, file) {
                total += file.size || 1;
            });
            return total;
        },

        _onProgress: function (e, data) {
            if (e.lengthComputable) {
                var total = data.total || this._getTotal(data.files),
                    loaded = parseInt(
                        e.loaded / e.total * (data.chunkSize || total),
                        10
                    ) + (data.uploadedBytes || 0);
                this._loaded += loaded - (data.loaded || data.uploadedBytes || 0);
                data.lengthComputable = true;
                data.loaded = loaded;
                data.total = total;
                // Trigger a custom progress event with a total data property set
                // to the file size(s) of the current upload and a loaded data
                // property calculated accordingly:
                this._trigger('progress', e, data);
                // Trigger a global progress event for all current file uploads,
                // including ajax calls queued for sequential file uploads:
                this._trigger('progressall', e, {
                    lengthComputable: true,
                    loaded: this._loaded,
                    total: this._total
                });
            }
        },

        _initProgressListener: function (options) {
            var that = this,
                xhr = options.xhr ? options.xhr() : $.ajaxSettings.xhr();
            // Accesss to the native XHR object is required to add event listeners
            // for the upload progress event:
            if (xhr.upload) {
                $(xhr.upload).bind('progress', function (e) {
                    var oe = e.originalEvent;
                    // Make sure the progress event properties get copied over:
                    e.lengthComputable = oe.lengthComputable;
                    e.loaded = oe.loaded;
                    e.total = oe.total;
                    that._onProgress(e, options);
                });
                options.xhr = function () {
                    return xhr;
                };
            }
        },

        _initXHRData: function (options) {
            var formData,
                file = options.files[0];
            if (!options.multipart || options.blob) {
                // For non-multipart uploads and chunked uploads,
                // file meta data is not part of the request body,
                // so we transmit this data as part of the HTTP headers.
                // For cross domain requests, these headers must be allowed
                // via Access-Control-Allow-Headers or removed using
                // the beforeSend callback:
                options.headers = $.extend(options.headers, {
                    'X-File-Name': file.name,
                    'X-File-Type': file.type,
                    'X-File-Size': file.size
                });
                if (!options.blob) {
                    // Non-chunked non-multipart upload:
                    options.contentType = file.type;
                    options.data = file;
                } else if (!options.multipart) {
                    // Chunked non-multipart upload:
                    options.contentType = 'application/octet-stream';
                    options.data = options.blob;
                }
            }
            if (options.multipart && typeof FormData !== 'undefined') {
                if (options.postMessage) {
                    // window.postMessage does not allow sending FormData
                    // objects, so we just add the File/Blob objects to
                    // the formData array and let the postMessage window
                    // create the FormData object out of this array:
                    formData = this._getFormData(options);
                    if (options.blob) {
                        formData.push({
                            name: options.paramName,
                            value: options.blob
                        });
                    } else {
                        $.each(options.files, function (index, file) {
                            formData.push({
                                name: options.paramName,
                                value: file
                            });
                        });
                    }
                } else {
                    if (options.formData instanceof FormData) {
                        formData = options.formData;
                    } else {
                        formData = new FormData();
                        $.each(this._getFormData(options), function (index, field) {
                            formData.append(field.name, field.value);
                        });
                    }
                    if (options.blob) {
                        formData.append(options.paramName, options.blob, file.name);
                    } else {
                        $.each(options.files, function (index, file) {
                            // File objects are also Blob instances.
                            // This check allows the tests to run with
                            // dummy objects:
                            if (file instanceof Blob) {
                                formData.append(options.paramName, file, file.name);
                            }
                        });
                    }
                }
                options.data = formData;
            }
            // Blob reference is not needed anymore, free memory:
            options.blob = null;
        },

        _initIframeSettings: function (options) {
            // Setting the dataType to iframe enables the iframe transport:
            options.dataType = 'iframe ' + (options.dataType || '');
            // The iframe transport accepts a serialized array as form data:
            options.formData = this._getFormData(options);
            // Add redirect url to form data on cross-domain uploads:
            if (options.redirect && $('<a></a>').prop('href', options.url)
                    .prop('host') !== location.host) {
                options.formData.push({
                    name: options.redirectParamName || 'redirect',
                    value: options.redirect
                });
            }
        },

        _initDataSettings: function (options) {
            if (this._isXHRUpload(options)) {
                if (!this._chunkedUpload(options, true)) {
                    if (!options.data) {
                        this._initXHRData(options);
                    }
                    this._initProgressListener(options);
                }
                if (options.postMessage) {
                    // Setting the dataType to postmessage enables the
                    // postMessage transport:
                    options.dataType = 'postmessage ' + (options.dataType || '');
                }
            } else {
                this._initIframeSettings(options, 'iframe');
            }
        },

        _initFormSettings: function (options) {
            // Retrieve missing options from the input field and the
            // associated form, if available:
            if (!options.form || !options.form.length) {
                options.form = $(options.fileInput.prop('form'));
            }
            if (!options.paramName) {
                options.paramName = options.fileInput.prop('name') ||
                    'files[]';
            }
            if (!options.url) {
                options.url = options.form.prop('action') || location.href;
            }
            // The HTTP request method must be "POST" or "PUT":
            options.type = (options.type || options.form.prop('method') || '')
                .toUpperCase();
            if (options.type !== 'POST' && options.type !== 'PUT') {
                options.type = 'POST';
            }
        },

        _getAJAXSettings: function (data) {
            var options = $.extend({}, this.options, data);
            this._initFormSettings(options);
            this._initDataSettings(options);
            return options;
        },

        // Maps jqXHR callbacks to the equivalent
        // methods of the given Promise object:
        _enhancePromise: function (promise) {
            promise.success = promise.done;
            promise.error = promise.fail;
            promise.complete = promise.always;
            return promise;
        },

        // Creates and returns a Promise object enhanced with
        // the jqXHR methods abort, success, error and complete:
        _getXHRPromise: function (resolveOrReject, context, args) {
            var dfd = $.Deferred(),
                promise = dfd.promise();
            context = context || this.options.context || promise;
            if (resolveOrReject === true) {
                dfd.resolveWith(context, args);
            } else if (resolveOrReject === false) {
                dfd.rejectWith(context, args);
            }
            promise.abort = dfd.promise;
            return this._enhancePromise(promise);
        },

        // Uploads a file in multiple, sequential requests
        // by splitting the file up in multiple blob chunks.
        // If the second parameter is true, only tests if the file
        // should be uploaded in chunks, but does not invoke any
        // upload requests:
        _chunkedUpload: function (options, testOnly) {
            var that = this,
                file = options.files[0],
                fs = file.size,
                ub = options.uploadedBytes = options.uploadedBytes || 0,
                mcs = options.maxChunkSize || fs,
                // Use the Blob methods with the slice implementation
                // according to the W3C Blob API specification:
                slice = file.webkitSlice || file.mozSlice || file.slice,
                upload,
                n,
                jqXHR,
                pipe;
            if (!(this._isXHRUpload(options) && slice && (ub || mcs < fs)) ||
                    options.data) {
                return false;
            }
            if (testOnly) {
                return true;
            }
            if (ub >= fs) {
                file.error = 'uploadedBytes';
                return this._getXHRPromise(
                    false,
                    options.context,
                    [null, 'error', file.error]
                );
            }
            // n is the number of blobs to upload,
            // calculated via filesize, uploaded bytes and max chunk size:
            n = Math.ceil((fs - ub) / mcs);
            // The chunk upload method accepting the chunk number as parameter:
            upload = function (i) {
                if (!i) {
                    return that._getXHRPromise(true, options.context);
                }
                // Upload the blobs in sequential order:
                return upload(i -= 1).pipe(function () {
                    // Clone the options object for each chunk upload:
                    var o = $.extend({}, options);
                    o.blob = slice.call(
                        file,
                        ub + i * mcs,
                        ub + (i + 1) * mcs
                    );
                    // Store the current chunk size, as the blob itself
                    // will be dereferenced after data processing:
                    o.chunkSize = o.blob.size;
                    // Process the upload data (the blob and potential form data):
                    that._initXHRData(o);
                    // Add progress listeners for this chunk upload:
                    that._initProgressListener(o);
                    jqXHR = ($.ajax(o) || that._getXHRPromise(false, o.context))
                        .done(function () {
                            // Create a progress event if upload is done and
                            // no progress event has been invoked for this chunk:
                            if (!o.loaded) {
                                that._onProgress($.Event('progress', {
                                    lengthComputable: true,
                                    loaded: o.chunkSize,
                                    total: o.chunkSize
                                }), o);
                            }
                            options.uploadedBytes = o.uploadedBytes +=
                                o.chunkSize;
                        });
                    return jqXHR;
                });
            };
            // Return the piped Promise object, enhanced with an abort method,
            // which is delegated to the jqXHR object of the current upload,
            // and jqXHR callbacks mapped to the equivalent Promise methods:
            pipe = upload(n);
            pipe.abort = function () {
                return jqXHR.abort();
            };
            return this._enhancePromise(pipe);
        },

        _beforeSend: function (e, data) {
            if (this._active === 0) {
                // the start callback is triggered when an upload starts
                // and no other uploads are currently running,
                // equivalent to the global ajaxStart event:
                this._trigger('start');
            }
            this._active += 1;
            // Initialize the global progress values:
            this._loaded += data.uploadedBytes || 0;
            this._total += this._getTotal(data.files);
        },

        _onDone: function (result, textStatus, jqXHR, options) {
            if (!this._isXHRUpload(options)) {
                // Create a progress event for each iframe load:
                this._onProgress($.Event('progress', {
                    lengthComputable: true,
                    loaded: 1,
                    total: 1
                }), options);
            }
            options.result = result;
            options.textStatus = textStatus;
            options.jqXHR = jqXHR;
            this._trigger('done', null, options);
        },

        _onFail: function (jqXHR, textStatus, errorThrown, options) {
            options.jqXHR = jqXHR;
            options.textStatus = textStatus;
            options.errorThrown = errorThrown;
            this._trigger('fail', null, options);
            if (options.recalculateProgress) {
                // Remove the failed (error or abort) file upload from
                // the global progress calculation:
                this._loaded -= options.loaded || options.uploadedBytes || 0;
                this._total -= options.total || this._getTotal(options.files);
            }
        },

        _onAlways: function (jqXHRorResult, textStatus, jqXHRorError, options) {
            this._active -= 1;
            options.textStatus = textStatus;
            if (jqXHRorError && jqXHRorError.always) {
                options.jqXHR = jqXHRorError;
                options.result = jqXHRorResult;
            } else {
                options.jqXHR = jqXHRorResult;
                options.errorThrown = jqXHRorError;
            }
            this._trigger('always', null, options);
            if (this._active === 0) {
                // The stop callback is triggered when all uploads have
                // been completed, equivalent to the global ajaxStop event:
                this._trigger('stop');
                // Reset the global progress values:
                this._loaded = this._total = 0;
            }
        },

        _onSend: function (e, data) {
            var that = this,
                jqXHR,
                slot,
                pipe,
                options = that._getAJAXSettings(data),
                send = function (resolve, args) {
                    that._sending += 1;
                    jqXHR = jqXHR || (
                        (resolve !== false &&
                        that._trigger('send', e, options) !== false &&
                        (that._chunkedUpload(options) || $.ajax(options))) ||
                        that._getXHRPromise(false, options.context, args)
                    ).done(function (result, textStatus, jqXHR) {
                        that._onDone(result, textStatus, jqXHR, options);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        that._onFail(jqXHR, textStatus, errorThrown, options);
                    }).always(function (jqXHRorResult, textStatus, jqXHRorError) {
                        that._sending -= 1;
                        that._onAlways(
                            jqXHRorResult,
                            textStatus,
                            jqXHRorError,
                            options
                        );
                        if (options.limitConcurrentUploads &&
                                options.limitConcurrentUploads > that._sending) {
                            // Start the next queued upload,
                            // that has not been aborted:
                            var nextSlot = that._slots.shift();
                            while (nextSlot) {
                                if (!nextSlot.isRejected()) {
                                    nextSlot.resolve();
                                    break;
                                }
                                nextSlot = that._slots.shift();
                            }
                        }
                    });
                    return jqXHR;
                };
            this._beforeSend(e, options);
            if (this.options.sequentialUploads ||
                    (this.options.limitConcurrentUploads &&
                    this.options.limitConcurrentUploads <= this._sending)) {
                if (this.options.limitConcurrentUploads > 1) {
                    slot = $.Deferred();
                    this._slots.push(slot);
                    pipe = slot.pipe(send);
                } else {
                    pipe = (this._sequence = this._sequence.pipe(send, send));
                }
                // Return the piped Promise object, enhanced with an abort method,
                // which is delegated to the jqXHR object of the current upload,
                // and jqXHR callbacks mapped to the equivalent Promise methods:
                pipe.abort = function () {
                    var args = [undefined, 'abort', 'abort'];
                    if (!jqXHR) {
                        if (slot) {
                            slot.rejectWith(args);
                        }
                        return send(false, args);
                    }
                    return jqXHR.abort();
                };
                return this._enhancePromise(pipe);
            }
            return send();
        },

        _onAdd: function (e, data) {
            var that = this,
                result = true,
                options = $.extend({}, this.options, data),
                limit = options.limitMultiFileUploads,
                fileSet,
                i;
            if (!(options.singleFileUploads || limit) ||
                    !this._isXHRUpload(options)) {
                fileSet = [data.files];
            } else if (!options.singleFileUploads && limit) {
                fileSet = [];
                for (i = 0; i < data.files.length; i += limit) {
                    fileSet.push(data.files.slice(i, i + limit));
                }
            }
            data.originalFiles = data.files;
            $.each(fileSet || data.files, function (index, element) {
                var files = fileSet ? element : [element],
                    newData = $.extend({}, data, {files: files});
                newData.submit = function () {
                    newData.jqXHR = this.jqXHR =
                        (that._trigger('submit', e, this) !== false) &&
                        that._onSend(e, this);
                    return this.jqXHR;
                };
                return (result = that._trigger('add', e, newData));
            });
            return result;
        },

        // File Normalization for Gecko 1.9.1 (Firefox 3.5) support:
        _normalizeFile: function (index, file) {
            if (file.name === undefined && file.size === undefined) {
                file.name = file.fileName;
                file.size = file.fileSize;
            }
        },

        _replaceFileInput: function (input) {
            var inputClone = input.clone(true);
            $('<form></form>').append(inputClone)[0].reset();
            // Detaching allows to insert the fileInput on another form
            // without loosing the file input value:
            input.after(inputClone).detach();
            // Avoid memory leaks with the detached file input:
            $.cleanData(input.unbind('remove'));
            // Replace the original file input element in the fileInput
            // collection with the clone, which has been copied including
            // event handlers:
            this.options.fileInput = this.options.fileInput.map(function (i, el) {
                if (el === input[0]) {
                    return inputClone[0];
                }
                return el;
            });
            // If the widget has been initialized on the file input itself,
            // override this.element with the file input clone:
            if (input[0] === this.element[0]) {
                this.element = inputClone;
            }
        },

        _onChange: function (e) {
            var that = e.data.fileupload,
                data = {
                    files: $.each($.makeArray(e.target.files), that._normalizeFile),
                    fileInput: $(e.target),
                    form: $(e.target.form)
                };
            if (!data.files.length) {
                // If the files property is not available, the browser does not
                // support the File API and we add a pseudo File object with
                // the input value as name with path information removed:
                data.files = [{name: e.target.value.replace(/^.*\\/, '')}];
            }
            if (that.options.replaceFileInput) {
                that._replaceFileInput(data.fileInput);
            }
            if (that._trigger('change', e, data) === false ||
                    that._onAdd(e, data) === false) {
                return false;
            }
        },

        _onPaste: function (e) {
            var that = e.data.fileupload,
                cbd = e.originalEvent.clipboardData,
                items = (cbd && cbd.items) || [],
                data = {files: []};
            $.each(items, function (index, item) {
                var file = item.getAsFile && item.getAsFile();
                if (file) {
                    data.files.push(file);
                }
            });
            if (that._trigger('paste', e, data) === false ||
                    that._onAdd(e, data) === false) {
                return false;
            }
        },

        _onDrop: function (e) {
            var that = e.data.fileupload,
                dataTransfer = e.dataTransfer = e.originalEvent.dataTransfer,
                data = {
                    files: $.each(
                        $.makeArray(dataTransfer && dataTransfer.files),
                        that._normalizeFile
                    )
                };
            if (that._trigger('drop', e, data) === false ||
                    that._onAdd(e, data) === false) {
                return false;
            }
            e.preventDefault();
        },

        _onDragOver: function (e) {
            var that = e.data.fileupload,
                dataTransfer = e.dataTransfer = e.originalEvent.dataTransfer;
            if (that._trigger('dragover', e) === false) {
                return false;
            }
            if (dataTransfer) {
                dataTransfer.dropEffect = dataTransfer.effectAllowed = 'copy';
            }
            e.preventDefault();
        },

        _initEventHandlers: function () {
            var ns = this.options.namespace;
            this.options.dropZone
                .bind('dragover.' + ns, {fileupload: this}, this._onDragOver)
                .bind('drop.' + ns, {fileupload: this}, this._onDrop)
                .bind('paste.' + ns, {fileupload: this}, this._onPaste);
            this.options.fileInput
                .bind('change.' + ns, {fileupload: this}, this._onChange);
        },

        _destroyEventHandlers: function () {
            var ns = this.options.namespace;
            this.options.dropZone
                .unbind('dragover.' + ns, this._onDragOver)
                .unbind('drop.' + ns, this._onDrop)
                .unbind('paste.' + ns, this._onPaste);
            this.options.fileInput
                .unbind('change.' + ns, this._onChange);
        },

        _setOption: function (key, value) {
            var refresh = $.inArray(key, this._refreshOptionsList) !== -1;
            if (refresh) {
                this._destroyEventHandlers();
            }
            $.Widget.prototype._setOption.call(this, key, value);
            if (refresh) {
                this._initSpecialOptions();
                this._initEventHandlers();
            }
        },

        _initSpecialOptions: function () {
            var options = this.options;
            if (options.fileInput === undefined) {
                options.fileInput = this.element.is('input:file') ?
                        this.element : this.element.find('input:file');
            } else if (!(options.fileInput instanceof $)) {
                options.fileInput = $(options.fileInput);
            }
            if (!(options.dropZone instanceof $)) {
                options.dropZone = $(options.dropZone);
            }
        },

        _create: function () {
            var options = this.options,
                dataOpts = $.extend({}, this.element.data());
            dataOpts[this.widgetName] = undefined;
            $.extend(options, dataOpts);
            options.namespace = options.namespace || this.widgetName;
            this._initSpecialOptions();
            this._slots = [];
            this._sequence = this._getXHRPromise(true);
            this._sending = this._active = this._loaded = this._total = 0;
            this._initEventHandlers();
        },

        destroy: function () {
            this._destroyEventHandlers();
            $.Widget.prototype.destroy.call(this);
        },

        enable: function () {
            $.Widget.prototype.enable.call(this);
            this._initEventHandlers();
        },

        disable: function () {
            this._destroyEventHandlers();
            $.Widget.prototype.disable.call(this);
        },

        // This method is exposed to the widget API and allows adding files
        // using the fileupload API. The data parameter accepts an object which
        // must have a files property and can contain additional options:
        // .fileupload('add', {files: filesList});
        add: function (data) {
            if (!data || this.options.disabled) {
                return;
            }
            data.files = $.each($.makeArray(data.files), this._normalizeFile);
            this._onAdd(null, data);
        },

        // This method is exposed to the widget API and allows sending files
        // using the fileupload API. The data parameter accepts an object which
        // must have a files property and can contain additional options:
        // .fileupload('send', {files: filesList});
        // The method returns a Promise object for the file upload call.
        send: function (data) {
            if (data && !this.options.disabled) {
                data.files = $.each($.makeArray(data.files), this._normalizeFile);
                if (data.files.length) {
                    return this._onSend(null, data);
                }
            }
            return this._getXHRPromise(false, data && data.context);
        }

    });

}));

define('multiselect',['jquery'], function (jQuery) {
/*
// jQuery multiSelect
//
// Version 1.2.2 beta
//
// Cory S.N. LaViska
// A Beautiful Site (http://abeautifulsite.net/)
// 09 September 2009
//
// Visit http://abeautifulsite.net/notebook/62 for more information
//
// (Amended by Andy Richmond, Letters & Science Deans' Office, University of California, Davis)
//
// Usage: $('#control_id').multiSelect( options, callback )
//
// Options:  selectAll          - whether or not to display the Select All option; true/false, default = true
//           selectAllText      - text to display for selecting/unselecting all options simultaneously
//           noneSelected       - text to display when there are no selected items in the list
//           oneOrMoreSelected  - text to display when there are one or more selected items in the list
//                                (note: you can use % as a placeholder for the number of items selected).
//                                Use * to show a comma separated list of all selected; default = '% selected'
//           optGroupSelectable - whether or not optgroups are selectable if you use them; true/false, default = false
//           listHeight         - the max height of the droptdown options
//
// Dependencies:  jQuery 1.2.6 or higher (http://jquery.com/)
//
// Change Log:
//
//		1.0.1	- Updated to work with jQuery 1.2.6+ (no longer requires the dimensions plugin)
//				- Changed $(this).offset() to $(this).position(), per James' and Jono's suggestions
//
//		1.0.2	- Fixed issue where dropdown doesn't scroll up/down with keyboard shortcuts
//				- Changed '$' in setTimeout to use 'jQuery' to support jQuery.noConflict
//				- Renamed from jqueryMultiSelect.* to jquery.multiSelect.* per the standard recommended at
//				  http://docs.jquery.com/Plugins/Authoring (does not affect API methods)
//
//		1.0.3	- Now uses the bgiframe plugin (if it exists) to fix the IE6 layering bug.
//              - Forces IE6 to use a min-height of 200px (this needs to be added to the options)
//
//		1.1.0	- Added the ability to update the options dynamically via javascript: multiSelectOptionsUpdate(JSON)
//              - Added a title that displays the whole comma delimited list when using oneOrMoreSelected = *
//              - Moved some of the functions to be closured to make them private
//              - Changed the way the keyboard navigation worked to more closely match how a standard dropdown works
//              - ** by Andy Richmond **
//
//		1.2.0	- Added support for optgroups
//              - Added the ability for selectable optgroups (i.e. select all for an optgroup)
//              - ** by Andy Richmond **
//
//		1.2.1	- Fixed bug where input text overlapped dropdown arrow in IE (i.e. when using oneOrMoreSelected = *)
//              - Added option "listHeight" for min-height of the dropdown
//              - Fixed bug where bgiframe was causing a horizontal scrollbar and on short lists extra whitespace below the options
//              - ** by Andy Richmond **
//
//		1.2.2	- Fixed bug where the keypress stopped showing the dropdown because in jQuery 1.3.2 they changed the way ':visible' works
//              - Fixed some other bugs in the way the keyboard interface worked
//              - Changed the main textbox to an <a> tag (with 'display: inline-block') to prevent the display text from being selected/highlighted
//              - Added the ability to jump to an option by typing the first character of that option (simular to a normal drop down)
//              - ** by Andy Richmond **
//				- Added [] to make each control submit an HTML array so $.serialize() works properly
//
// Licensing & Terms of Use
// 
// This plugin is dual-licensed under the GNU General Public License and the MIT License and
// is copyright 2008 A Beautiful Site, LLC. 
//	
*/
if (jQuery) (function ($) {

    // render the html for a single option
    function renderOption(id, option) {
        var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
        if (option.selected) {
            html += ' checked="checked"';
        }
        html += ' />' + option.text + '</label>';

        return html;
    }

    // render the html for the options/optgroups
    function renderOptions(id, options, o) {
        var html = "";

        for (var i = 0; i < options.length; i++) {
            if (options[i].optgroup) {
                html += '<label class="optGroup">';

                if (o.optGroupSelectable) {
                    html += '<input type="checkbox" class="optGroup" />' + options[i].optgroup;
                }
                else {
                    html += options[i].optgroup;
                }

                html += '</label><div class="optGroupContainer">';

                html += renderOptions(id, options[i].options, o);

                html += '</div>';
            }
            else {
                html += o.renderOption(id, options[i]);
            }
        }

        if (options.length == 0) {
            html += '<label style="padding:0 13px">' + (o.noOptionsText ? o.noOptionsText : '-') + '</label>';
        }

        return html;
    }

    // Building the actual options
    function buildOptions(options) {
        var multiSelect = $(this);
        var multiSelectOptions = multiSelect.next('.multiSelectOptions');
        var o = multiSelect.data("config");
        var callback = multiSelect.data("callback");

        // clear the existing options
        multiSelectOptions.html("");
        var html = "";

        if (o.messageText) {
            html += '<div class="selectMessage">' + o.messageText + '</div>';
        }

        // if we should have a select all option then add it
        if (o.selectAll) {
            html += '<label class="selectAll"><input type="checkbox" class="selectAll" />' + o.selectAllText + '</label>';
        }

        // generate the html for the new options
        html += renderOptions(multiSelect.attr('id'), options, o);

        multiSelectOptions.html(html);

        // variables needed to account for width changes due to a scrollbar
        var initialWidth = multiSelectOptions.width();
        var hasScrollbar = false;

        // set the height of the dropdown options
        if (multiSelectOptions.height() > o.listHeight) {
            multiSelectOptions.css("height", o.listHeight + 'px');
            hasScrollbar = true;
        } else {
            multiSelectOptions.css("height", '');
        }

        // if the there is a scrollbar and the browser did not already handle adjusting the width (i.e. Firefox) then we will need to manaually add the scrollbar width
        var scrollbarWidth = hasScrollbar && (initialWidth == multiSelectOptions.width()) ? 17 : 0;

        // set the width of the dropdown options
        if ((multiSelectOptions.width() + scrollbarWidth) < multiSelect.outerWidth()) {
            multiSelectOptions.css("width", multiSelect.outerWidth() - 2/*border*/ + 'px');
        } else {
            multiSelectOptions.css("width", (multiSelectOptions.width() + scrollbarWidth) + 'px');
        }

        // Apply bgiframe if available on IE6
        if ($.fn.bgiframe) multiSelect.next('.multiSelectOptions').bgiframe({ width: multiSelectOptions.width(), height: multiSelectOptions.height() });

        // Handle selectAll oncheck
        if (o.selectAll) {
            multiSelectOptions.find('INPUT.selectAll').click(function () {
                // update all the child checkboxes
                multiSelectOptions.find('INPUT:checkbox').attr('checked', $(this).attr('checked')).parent("LABEL").toggleClass('checked', $(this).attr('checked'));
            });
        }

        // Handle OptGroup oncheck
        if (o.optGroupSelectable) {
            multiSelectOptions.addClass('optGroupHasCheckboxes');

            multiSelectOptions.find('INPUT.optGroup').click(function () {
                // update all the child checkboxes
                $(this).parent().next().find('INPUT:checkbox').attr('checked', $(this).attr('checked')).parent("LABEL").toggleClass('checked', $(this).attr('checked'));
            });
        }

        // Handle all checkboxes
        multiSelectOptions.find('INPUT:checkbox').click(function () {
            var o = multiSelect.data("config");
            var multiSelectOptions = multiSelect.next('.multiSelectOptions');

            // set the label checked class
            if (o.singleSelect) { // deselect all except the current item if in single mode
                multiSelectOptions.find('INPUT:checkbox').not($(this)).attr('checked', false).parent("LABEL").toggleClass('checked', false);
            }
            $(this).parent("LABEL").toggleClass('checked', $(this).attr('checked'));

            updateSelected.call(multiSelect);
            multiSelect.focus();
            if ($(this).parent().parent().hasClass('optGroupContainer')) {
                updateOptGroup.call(multiSelect, $(this).parent().parent().prev());
            }
            if (callback) {
                callback($(this));
            }

            // If single select widget, hide it as per standard UI dropdown
            if (o.singleSelect) {
                multiSelect.multiSelectOptionsHide();
            }
        });

        // Initial display
        multiSelectOptions.each(function () {
            $(this).find('INPUT:checked').parent().addClass('checked');
        });

        // Initialize selected and select all 
        updateSelected.call(multiSelect);

        // Initialize optgroups
        if (o.optGroupSelectable) {
            multiSelectOptions.find('LABEL.optGroup').each(function () {
                updateOptGroup.call(multiSelect, $(this));
            });
        }

        // Handle hovers
        multiSelectOptions.find('LABEL:has(INPUT)').hover(function () {
            $(this).parent().find('LABEL').removeClass('hover');
            $(this).addClass('hover');
        }, function () {
            $(this).parent().find('LABEL').removeClass('hover');
        });

        // Keyboard
        multiSelect.keydown(function (e) {

            var multiSelectOptions = $(this).next('.multiSelectOptions');

            // Is dropdown visible?
            if (multiSelectOptions.css('visibility') != 'hidden') {
                // Dropdown is visible
                // Tab
                if (e.keyCode == 9) {
                    $(this).addClass('focus').trigger('click'); // esc, left, right - hide
                    $(this).focus().next(':input').focus();
                    return true;
                }

                // ESC, Left, Right
                if (e.keyCode == 27 || e.keyCode == 37 || e.keyCode == 39) {
                    // Hide dropdown
                    $(this).addClass('focus').trigger('click');
                }
                // Down || Up
                if (e.keyCode == 40 || e.keyCode == 38) {
                    var allOptions = multiSelectOptions.find('LABEL');
                    var oldHoverIndex = allOptions.index(allOptions.filter('.hover'));
                    var newHoverIndex = -1;

                    // if there is no current highlighted item then highlight the first item
                    if (oldHoverIndex < 0) {
                        // Default to first item
                        multiSelectOptions.find('LABEL:first').addClass('hover');
                    }
                    // else if we are moving down and there is a next item then move
                    else if (e.keyCode == 40 && oldHoverIndex < allOptions.length - 1) {
                        newHoverIndex = oldHoverIndex + 1;
                    }
                    // else if we are moving up and there is a prev item then move
                    else if (e.keyCode == 38 && oldHoverIndex > 0) {
                        newHoverIndex = oldHoverIndex - 1;
                    }

                    if (newHoverIndex >= 0) {
                        $(allOptions.get(oldHoverIndex)).removeClass('hover'); // remove the current highlight
                        $(allOptions.get(newHoverIndex)).addClass('hover'); // add the new highlight

                        // Adjust the viewport if necessary
                        adjustViewPort(multiSelectOptions);
                    }

                    return false;
                }

                // Enter, Space
                if (e.keyCode == 13 || e.keyCode == 32) {
                    var selectedCheckbox = multiSelectOptions.find('LABEL.hover INPUT:checkbox');

                    if (o.singleSelect) { // deselect all except the current item if in single mode
                        multiSelectOptions.find('INPUT:checkbox').not(selectedCheckbox).attr('checked', false).parent("LABEL").toggleClass('checked', false);
                    }
                    // Set the checkbox (and label class)
                    selectedCheckbox.attr('checked', !selectedCheckbox.attr('checked')).parent("LABEL").toggleClass('checked', selectedCheckbox.attr('checked'));

                    // if the checkbox was the select all then set all the checkboxes
                    if (selectedCheckbox.hasClass("selectAll")) {
                        multiSelectOptions.find('INPUT:checkbox').attr('checked', selectedCheckbox.attr('checked')).parent("LABEL").addClass('checked').toggleClass('checked', selectedCheckbox.attr('checked'));
                    }

                    updateSelected.call(multiSelect);

                    if (callback) callback($(this));

                    // If single select widget, hide it as per standard UI dropdown
                    if (o.singleSelect) {
                        multiSelect.multiSelectOptionsHide();
                    }

                    return false;
                }

                // Any other standard keyboard character (try and match the first character of an option)
                if (e.keyCode >= 33 && e.keyCode <= 126) {
                    // find the next matching item after the current hovered item
                    var match = multiSelectOptions.find('LABEL:startsWith(' + String.fromCharCode(e.keyCode) + ')');

                    var currentHoverIndex = match.index(match.filter('LABEL.hover'));

                    // filter the set to any items after the current hovered item
                    var afterHoverMatch = match.filter(function (index) {
                        return index > currentHoverIndex;
                    });

                    // if there were no item after the current hovered item then try using the full search results (filtered to the first one)
                    match = (afterHoverMatch.length >= 1 ? afterHoverMatch : match).filter("LABEL:first");

                    if (match.length == 1) {
                        // if we found a match then move the hover
                        multiSelectOptions.find('LABEL.hover').removeClass('hover');
                        match.addClass('hover');

                        adjustViewPort(multiSelectOptions);
                    }
                }
            } else {
                // Dropdown is not visible
                if (e.keyCode == 38 || e.keyCode == 40 || e.keyCode == 13 || e.keyCode == 32) { //up, down, enter, space - show
                    // Show dropdown
                    $(this).removeClass('focus').trigger('click');
                    multiSelectOptions.find('LABEL:first').addClass('hover');
                    return false;
                }
                //  Tab key
                if (e.keyCode == 9) {
                    // Shift focus to next INPUT element on page
                    multiSelectOptions.next(':input').focus();
                    return true;
                }
            }
            // Prevent enter key from submitting form
            if (e.keyCode == 13) return false;
        });
    }

    // Adjust the viewport if necessary
    function adjustViewPort(multiSelectOptions) {
        // check for and move down
        var selectionBottom = multiSelectOptions.find('LABEL.hover').position().top + multiSelectOptions.find('LABEL.hover').outerHeight();

        if (selectionBottom > multiSelectOptions.innerHeight()) {
            multiSelectOptions.scrollTop(multiSelectOptions.scrollTop() + selectionBottom - multiSelectOptions.innerHeight());
        }

        // check for and move up						
        if (multiSelectOptions.find('LABEL.hover').position().top < 0) {
            multiSelectOptions.scrollTop(multiSelectOptions.scrollTop() + multiSelectOptions.find('LABEL.hover').position().top);
        }
    }

    // Update the optgroup checked status
    function updateOptGroup(optGroup) {
        var multiSelect = $(this);
        var o = multiSelect.data("config");

        // Determine if the optgroup should be checked
        if (o.optGroupSelectable) {
            var optGroupSelected = true;
            $(optGroup).next().find('INPUT:checkbox').each(function () {
                if (!$(this).attr('checked')) {
                    optGroupSelected = false;
                    return false;
                }
            });

            $(optGroup).find('INPUT.optGroup').attr('checked', optGroupSelected).parent("LABEL").toggleClass('checked', optGroupSelected);
        }
    }

    // Update the textbox with the total number of selected items, and determine select all
    function updateSelected() {
        var multiSelect = $(this);
        var multiSelectOptions = multiSelect.next('.multiSelectOptions');
        var o = multiSelect.data("config");

        var i = 0;
        var selectAll = true;
        var display = '';
        var selectedOptions = [];
        multiSelectOptions.find('INPUT:checkbox').not('.selectAll, .optGroup').each(function () {
            if ($(this).attr('checked')) {
                i++;
                display = display + $(this).parent().text() + ', ';
                selectedOptions.push({ text: $(this).parent().text(), value: $(this).attr('value') });
            }
            else selectAll = false;
        });

        // trim any end comma and surounding whitespace
        display = display.replace(/\s*\,\s*$/, '');

        if (i == 0) {
            multiSelect.find("span").html(o.noneSelected);
        } else {
            if (typeof o.oneOrMoreSelected === 'function') {
                multiSelect.find("span").html(o.oneOrMoreSelected(selectedOptions));
            }
            else if (o.oneOrMoreSelected == '*') {
                multiSelect.find("span").html(display);
                multiSelect.attr("title", display);
            } else {
                multiSelect.find("span").html(o.oneOrMoreSelected.replace('%', i));
            }
        }

        // Determine if Select All should be checked
        if (o.selectAll) {
            multiSelectOptions.find('INPUT.selectAll').attr('checked', selectAll).parent("LABEL").toggleClass('checked', selectAll);
        }
    }

    $.extend($.fn, {
        multiSelect: function (o, callback) {
            // Default options
            if (!o) o = {};
            if (o.selectAll == undefined) o.selectAll = true;
            if (o.selectAllText == undefined) o.selectAllText = "Select All";
            if (o.noneSelected == undefined) o.noneSelected = 'Select options';
            if (o.oneOrMoreSelected == undefined) o.oneOrMoreSelected = '% selected';
            if (o.optGroupSelectable == undefined) o.optGroupSelectable = false;
            if (o.listHeight == undefined) o.listHeight = 150;
            if (o.renderOption == undefined) o.renderOption = renderOption;
            if (o.singleSelect == undefined) o.singleSelect = false;

            // Initialize each multiSelect
            $(this).each(function () {
                var select = $(this);
                var html = '<a href="javascript:;" class="multiSelect"><span></span><i></i></a>';
                html += '<div class="multiSelectOptions" style="position: absolute; z-index: 99999; visibility: hidden;"></div>';
                $(select).after(html);

                var multiSelect = $(select).next('.multiSelect');
                var multiSelectOptions = multiSelect.next('.multiSelectOptions');

                // if the select object had a width defined then match the new multilsect to it
                //multiSelect.find("span").css("width", $(select).width() + 'px');

                // Attach the config options to the multiselect
                multiSelect.data("config", o);

                // Attach the callback to the multiselect
                multiSelect.data("callback", callback);

                // Serialize the select options into json options
                var options = [];
                $(select).children().each(function () {
                    if (this.tagName.toUpperCase() == 'OPTGROUP') {
                        var suboptions = [];
                        options.push({ optgroup: $(this).attr('label'), options: suboptions });

                        $(this).children('OPTION').each(function () {
                            if ($(this).val() != '') {
                                suboptions.push({ text: $(this).html(), value: $(this).val(), selected: $(this).attr('selected') });
                            }
                        });
                    }
                    else if (this.tagName.toUpperCase() == 'OPTION') {
                        if ($(this).val() != '') {
                            options.push({ text: $(this).html(), value: $(this).val(), selected: $(this).attr('selected') });
                        }
                    }
                });

                // Eliminate the original form element
                $(select).remove();

                // Add the id that was on the original select element to the new input
                multiSelect.attr("id", $(select).attr("id"));

                // Build the dropdown options
                buildOptions.call(multiSelect, options);

                // Events
                multiSelect.hover(function () {
                    $(this).addClass('hover');
                }, function () {
                    $(this).removeClass('hover');
                }).click(function () {
                    // Show/hide on click
                    if ($(this).hasClass('active')) {
                        $(this).multiSelectOptionsHide();
                    } else {
                        $(this).multiSelectOptionsShow();
                    }
                    return false;
                }).focus(function () {
                    // So it can be styled with CSS
                    $(this).addClass('focus');
                }).blur(function () {
                    // So it can be styled with CSS
                    $(this).removeClass('focus');
                });

                // Add an event listener to the window to close the multiselect if the user clicks off
                $(document).click(function (event) {
                    // If somewhere outside of the multiselect was clicked then hide the multiselect
                    if (!($(event.target).parents().andSelf().is('.multiSelectOptions'))) {
                        multiSelect.multiSelectOptionsHide();
                    }
                });
            });
        },

        // Update the dropdown options
        multiSelectOptionsUpdate: function (options) {
            buildOptions.call($(this), options);
        },

        // Hide the dropdown
        multiSelectOptionsHide: function () {
            $(this).removeClass('active').removeClass('hover').next('.multiSelectOptions').css('visibility', 'hidden');
            $(this).removeClass('active').removeClass('hover').next('.multiSelectOptions').css('display', 'none');
        },

        // Show the dropdown
        multiSelectOptionsShow: function () {
            var multiSelect = $(this);
            var multiSelectOptions = multiSelect.next('.multiSelectOptions');
            var o = multiSelect.data("config");

            // Hide any open option boxes
            $('.multiSelect').multiSelectOptionsHide();
            multiSelectOptions.find('LABEL').removeClass('hover');
            multiSelect.addClass('active').next('.multiSelectOptions').css('visibility', 'visible');
            multiSelect.addClass('active').next('.multiSelectOptions').css('display', 'block');
            multiSelect.focus();

            /**FR: Added widget resizing here to account for when user resizes window**/
            // reset the scroll to the top
            multiSelect.next('.multiSelectOptions').scrollTop(0);

            // variables needed to account for width changes due to a scrollbar
            //var initialWidth = multiSelectOptions.width();
            //var hasScrollbar = false;

            // set the height of the dropdown options
            if (multiSelectOptions.height() > o.listHeight) {
                multiSelectOptions.css("height", o.listHeight + 'px');
                //hasScrollbar = true;
            } else {
                multiSelectOptions.css("height", '');
            }
            // if the there is a scrollbar and the browser did not already handle adjusting the width (i.e. Firefox) then we will need to manaually add the scrollbar width
            //var scrollbarWidth = hasScrollbar && (initialWidth == multiSelectOptions.width()) ? 17 : 0;

            // set the width of the dropdown options
            multiSelectOptions.css("width", multiSelect.outerWidth() - 2/*border*/ + 'px');
            /**FR**/

            // Position it
            var offset = multiSelect.position();
            multiSelect.next('.multiSelectOptions').css({ top: offset.top + $(this).outerHeight() + 'px' });
            multiSelect.next('.multiSelectOptions').css({ left: offset.left + 'px' });
        },

        // get a coma-delimited list of selected values
        selectedValuesString: function () {
            var selectedValues = "";
            $(this).next('.multiSelectOptions').find('INPUT:checkbox:checked').not('.optGroup, .selectAll').each(function () {
                selectedValues += $(this).attr('value') + ",";
            });
            // trim any end comma and surounding whitespace
            return selectedValues.replace(/\s*\,\s*$/, '');
        }
    });

    // add a new ":startsWith" search filter
    $.expr[":"].startsWith = function (el, i, m) {
        var search = m[3];
        if (!search) return false;
        return eval("/^[/s]*" + search + "/i").test($(el).text());
    };

})(jQuery);

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationFormLayoutView
// --------------------------

define('views/userformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) 
{
    var UserFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium user-form',

        template: 'UserForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#FirstName': '_contentChanged',
            'change input#LastName': '_contentChanged',
            'change input#Email': '_contentChanged'
        },

        initialize: function (options) {
            log('userFormLayoutView.initialize');
        },

        serializeData: function () {
            log('userFormLayoutView:serializeData');
            return {
                Model: {
                    User: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            log('userFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('userFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this.$el = $('#content .user-form');
            this._showDetails();
        },

        _showDetails: function () {
            log('userFormLayoutView:_showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();
        },

        _contentChanged: function (e) {
            log('userFormLayoutView:_contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return UserFormLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserController & UserRouter
// ---------------------------

define('controllers/usercontroller',['jquery', 'underscore', 'backbone', 'app', 'views/userformlayoutview', 'models/user'],
function ($, _, Backbone, app, UserFormLayoutView, User) {
    var UserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'users/:id/update': 'showUserForm'
        }
    });

    var UserHubRouter = function (options) {
        this.userHub = options.hub;
        this.userHub.setupOnlineUsers = setupOnlineUsers;
        this.userHub.userStatusUpdate = userStatusUpdate;
        this.userHub.joinedGroup = joinedGroup;
        this.userHub.mediaResourceUploaded = mediaResourceUploaded;
    };

    var UserController = {};

    var getModel = function (id) {
        var url = '/users/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('users')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Hub Callbacks
    // -------------

    // Receive a usee status update
    var userStatusUpdate = function (userStatus) {
        log('activityController.userStatusUpdate', this, userStatus);

        app.onlineUsers.add(userStatus.User);

        // Then set their status
        app.onlineUsers.get(userStatus.User.Id).set('Status', 'Online');
    };

    // Receive a list of users that are online
    var setupOnlineUsers = function (onlineUsers) {
        log('activityController.setupOnlineUsers', this, onlineUsers);
        app.onlineUsers.add(onlineUsers);
    };

    var joinedGroup = function (group) {
        if (group.GroupType === 'project') {
            app.authenticatedUser.projects.add(group);
        }
        if (group.GroupType === 'team') {
            app.authenticatedUser.teams.add(group);
        }
        if (group.GroupType === 'organisation') {
            app.authenticatedUser.organisations.add(group);
        }
    };

    var mediaResourceUploaded = function (mediaResource) {
        // detect model type then raise event
        log('userController.mediaResourceUploaded', mediaResource);

        if (mediaResource.MediaType == "video") {
            app.vent.trigger('videomediaresourceuploaded:', mediaResource);
        }
        else if (mediaResource.MediaType == "image") {
            app.vent.trigger('imagemediaresourceuploaded:', mediaResource);
        }
    };

    // Show an project form
    UserController.showUserForm = function (id) {
        log('userController:showUserForm');
        $.when(getModel(id))
            .done(function (model) {
                var user = new User(model.User);
                var userFormLayoutView = new UserFormLayoutView({ model: user });
                app.showFormContentView(userFormLayoutView, 'users');
                if (app.isPrerendering('users')) {
                    userFormLayoutView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        log('userController.initialize');

        this.userRouter = new UserRouter({
            controller: UserController
        });
        this.userHubRouter = new UserHubRouter({
            hub: $.connection.userHub
        });
    });

    return UserController;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityController & ActivityRouter
// -----------------------------------

define('controllers/activitycontroller',['jquery', 'underscore', 'backbone', 'app', 'models/activity'],
function ($, _, Backbone, app, Activity) {

    var ActivityRouter = function (options) {
        this.hub = options.groupHub;

        this.hub.newActivity = newActivity;
    };

    var ActivityController = {};

    var newActivity = function (groupId, activityData) {
        var activity = new Activity(activityData);

        app.vent.trigger('newactivity', activity);
        app.vent.trigger('newactivity:' + activity.get('Type'), activity);
        app.vent.trigger('newactivity:' + groupId, activity);
        app.vent.trigger('newactivity:' + groupId + ':' + activity.get('Type'), activity);
    };

    app.addInitializer(function () {
        this.activityRouter = new ActivityRouter({
            groupHub: $.connection.groupHub
        });
    });

    return ActivityController;

});

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// DebugController & DebugRouter
// -----------------------------

define('controllers/debugcontroller',['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {

    var DebugRouter = function (options) {

        this.hub = $.connection.debugHub;
        this.controller = options.controller;
        this.hub.debugToClient = this.controller.debugToClient;
    };

    var DebugController = {};

    DebugController.hub = $.connection.debugHub;

    // DebugController Public API To HUB
    //----------------------------------

    DebugController.debugToLog = function (message) {
        log(message);
    }

    // ChatController Public API From HUB
    // ----------------------------------

    DebugController.debugToClient = function (message) {
        log(message);
    }

    app.addInitializer(function () {
        this.debugRouter = new DebugRouter({
            controller: DebugController
        });
    });

    return DebugController;

});
define('timeago',['jquery'], function ($) {

/**
 * Timeago is a jQuery plugin that makes it easy to support automatically
 * updating fuzzy timestamps (e.g. "4 minutes ago" or "about 1 day ago").
 *
 * @name timeago
 * @version 0.11.3
 * @requires jQuery v1.2.3+
 * @author Ryan McGeary
 * @license MIT License - http://www.opensource.org/licenses/mit-license.php
 *
 * For usage and examples, visit:
 * http://timeago.yarp.com/
 *
 * Copyright (c) 2008-2012, Ryan McGeary (ryan -[at]- mcgeary [*dot*] org)
 */
(function($) {
  $.timeago = function(timestamp) {
    if (timestamp instanceof Date) {
      return inWords(timestamp);
    } else if (typeof timestamp === "string") {
      return inWords($.timeago.parse(timestamp));
    } else if (typeof timestamp === "number") {
      return inWords(new Date(timestamp));
    } else {
      return inWords($.timeago.datetime(timestamp));
    }
  };
  var $t = $.timeago;

  $.extend($.timeago, {
    settings: {
      refreshMillis: 60000,
      allowFuture: false,
      strings: {
        prefixAgo: null,
        prefixFromNow: null,
        suffixAgo: "ago",
        suffixFromNow: "from now",
        seconds: "less than a minute",
        minute: "about a minute",
        minutes: "%d minutes",
        hour: "about an hour",
        hours: "about %d hours",
        day: "a day",
        days: "%d days",
        month: "about a month",
        months: "%d months",
        year: "about a year",
        years: "%d years",
        wordSeparator: " ",
        numbers: []
      }
    },
    inWords: function(distanceMillis) {
      var $l = this.settings.strings;
      var prefix = $l.prefixAgo;
      var suffix = $l.suffixAgo;
      if (this.settings.allowFuture) {
        if (distanceMillis < 0) {
          prefix = $l.prefixFromNow;
          suffix = $l.suffixFromNow;
        }
      }

      var seconds = Math.abs(distanceMillis) / 1000;
      var minutes = seconds / 60;
      var hours = minutes / 60;
      var days = hours / 24;
      var years = days / 365;

      function substitute(stringOrFunction, number) {
        var string = $.isFunction(stringOrFunction) ? stringOrFunction(number, distanceMillis) : stringOrFunction;
        var value = ($l.numbers && $l.numbers[number]) || number;
        return string.replace(/%d/i, value);
      }

      var words = seconds < 45 && substitute($l.seconds, Math.round(seconds)) ||
        seconds < 90 && substitute($l.minute, 1) ||
        minutes < 45 && substitute($l.minutes, Math.round(minutes)) ||
        minutes < 90 && substitute($l.hour, 1) ||
        hours < 24 && substitute($l.hours, Math.round(hours)) ||
        hours < 42 && substitute($l.day, 1) ||
        days < 30 && substitute($l.days, Math.round(days)) ||
        days < 45 && substitute($l.month, 1) ||
        days < 365 && substitute($l.months, Math.round(days / 30)) ||
        years < 1.5 && substitute($l.year, 1) ||
        substitute($l.years, Math.round(years));

      var separator = $l.wordSeparator === undefined ?  " " : $l.wordSeparator;
      return $.trim([prefix, words, suffix].join(separator));
    },
    parse: function(iso8601) {
      var s = $.trim(iso8601);
      s = s.replace(/\.\d\d\d+/,""); // remove milliseconds
      s = s.replace(/-/,"/").replace(/-/,"/");
      s = s.replace(/T/," ").replace(/Z/," UTC");
      s = s.replace(/([\+\-]\d\d)\:?(\d\d)/," $1$2"); // -04:00 -> -0400
      return new Date(s);
    },
    datetime: function(elem) {
      var iso8601 = $t.isTime(elem) ? $(elem).attr("datetime") : $(elem).attr("title");
      return $t.parse(iso8601);
    },
    isTime: function(elem) {
      // jQuery's `is()` doesn't play well with HTML5 in IE
      return $(elem).get(0).tagName.toLowerCase() === "time"; // $(elem).is("time");
    }
  });

  $.fn.timeago = function() {
    var self = this;
    self.each(refresh);

    var $s = $t.settings;
    if ($s.refreshMillis > 0) {
      setInterval(function() { self.each(refresh); }, $s.refreshMillis);
    }
    return self;
  };

  function refresh() {
    var data = prepareData(this);
    if (!isNaN(data.datetime)) {
      $(this).text(inWords(data.datetime));
    }
    return this;
  }

  function prepareData(element) {
    element = $(element);
    if (!element.data("timeago")) {
      element.data("timeago", { datetime: $t.datetime(element) });
      var text = $.trim(element.text());
      if (text.length > 0 && !($t.isTime(element) && element.attr("title"))) {
        element.attr("title", text);
      }
    }
    return element.data("timeago");
  }

  function inWords(date) {
    return $t.inWords(distance(date));
  }

  function distance(date) {
    return (new Date().getTime() - date.getTime());
  }

  // fix for IE6 suckage
  document.createElement("abbr");
  document.createElement("time");
}(jQuery));

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// StreamItemView
// --------------

// Shows an individual stream item
define('views/streamitemview',['jquery', 'underscore', 'backbone', 'app', 'timeago'], function ($, _, Backbone, app) {

    var parseISO8601 = function (str) {
        // we assume str is a UTC date ending in 'Z'

        var parts = str.split('T'),
        dateParts = parts[0].split('-'),
        timeParts = parts[1].split('Z'),
        timeSubParts = timeParts[0].split(':'),
        timeSecParts = timeSubParts[2].split('.'),
        timeHours = Number(timeSubParts[0]),
        _date = new Date;

        _date.setUTCFullYear(Number(dateParts[0]));
        _date.setUTCMonth(Number(dateParts[1]) - 1);
        _date.setUTCDate(Number(dateParts[2]));
        _date.setUTCHours(Number(timeHours));
        _date.setUTCMinutes(Number(timeSubParts[1]));
        _date.setUTCSeconds(Number(timeSecParts[0]));
        if (timeSecParts[1]) _date.setUTCMilliseconds(Number(timeSecParts[1]));

        // by using setUTC methods the date has already been converted to local time(?)
        return _date;
    };

    var StreamItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'stream-item observation-stream-item',

        template: 'StreamItem',

        serializeData: function () {
            var json = { Model: this.model.toJSON() };
            json.Model.CreatedDateTimeDescription = parseISO8601(this.model.get('CreatedDateTime'));
            json.Model.ObservedOnDescription = ''; //parseISO8601(this.model.get('ObservedOn') + 'Z').format('d MMM yyyy')
            json.Model.ShowThumbnails = this.model.get('ObservationAdded').Observation.Media.length > 1 ? true : false;
            return json;
        },

        onRender: function () {
            this.$el.find('.time-description').timeago();

            this.$el.find('h2 a').on('click', function (e) {
                e.preventDefault();
                app.observationRouter.navigate($(this).attr('href'), { trigger: true });
                return false;
            });
        }
        //        render: function () {
        //            switch (this.StreamItem.get('Type')) {
        //                case 'observation':
        //                    var streamitemJSON = this.streamItem.toJSON();
        //                    streamitemJSON['ObservedOnDate'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('d MMM yyyy');
        //                    streamitemJSON['ObservedOnTime'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('h:mm');
        //                    streamitemJSON['HighlightMedia'] = streamitemJSON.item.observationMedia[0];
        //                    this.$el.append(ich.ObservationStreamListItem(streamitemJSON)).addClass('observation-stream-item');
        //                    break;
        //                default:
        //                    break;
        //            }
        //            return this;
        //        }
    });

    return StreamItemView;

});
define('date',[],function () {

/**
* @version: 1.0 Alpha-1
* @author: Coolite Inc. http://www.coolite.com/
* @date: 2008-04-13
* @copyright: Copyright (c) 2006-2008, Coolite Inc. (http://www.coolite.com/). All rights reserved.
* @license: Licensed under The MIT License. See license.txt and http://www.datejs.com/license/. 
* @website: http://www.datejs.com/
*/


Date.CultureInfo = {
    /* Culture Name */
    name: "en-AU",
    englishName: "English (Australia)",
    nativeName: "English (Australia)",

    /* Day Name Strings */
    dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
    abbreviatedDayNames: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
    shortestDayNames: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
    firstLetterDayNames: ["S", "M", "T", "W", "T", "F", "S"],

    /* Month Name Strings */
    monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
    abbreviatedMonthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],

    /* AM/PM Designators */
    amDesignator: "AM",
    pmDesignator: "PM",

    firstDayOfWeek: 1,
    twoDigitYearMax: 2029,

    /**
    * The dateElementOrder is based on the order of the 
    * format specifiers in the formatPatterns.DatePattern. 
    *
    * Example:
    <pre>
    shortDatePattern    dateElementOrder
    ------------------  ---------------- 
    "M/d/yyyy"          "mdy"
    "dd/MM/yyyy"        "dmy"
    "yyyy-MM-dd"        "ymd"
    </pre>
    *
    * The correct dateElementOrder is required by the parser to
    * determine the expected order of the date elements in the
    * string being parsed.
    */
    dateElementOrder: "dmy",

    /* Standard date and time format patterns */
    formatPatterns: {
        shortDate: "d/MM/yyyy",
        longDate: "dddd, d MMMM yyyy",
        shortTime: "h:mm tt",
        longTime: "h:mm:ss tt",
        fullDateTime: "dddd, d MMMM yyyy h:mm:ss tt",
        sortableDateTime: "yyyy-MM-ddTHH:mm:ss",
        universalSortableDateTime: "yyyy-MM-dd HH:mm:ssZ",
        rfc1123: "ddd, dd MMM yyyy HH:mm:ss GMT",
        monthDay: "dd MMMM",
        yearMonth: "MMMM yyyy"
    },

    /**
    * NOTE: If a string format is not parsing correctly, but
    * you would expect it parse, the problem likely lies below. 
    * 
    * The following regex patterns control most of the string matching
    * within the parser.
    * 
    * The Month name and Day name patterns were automatically generated
    * and in general should be (mostly) correct. 
    *
    * Beyond the month and day name patterns are natural language strings.
    * Example: "next", "today", "months"
    *
    * These natural language string may NOT be correct for this culture. 
    * If they are not correct, please translate and edit this file
    * providing the correct regular expression pattern. 
    *
    * If you modify this file, please post your revised CultureInfo file
    * to the Datejs Forum located at http://www.datejs.com/forums/.
    *
    * Please mark the subject of the post with [CultureInfo]. Example:
    *    Subject: [CultureInfo] Translated "da-DK" Danish(Denmark)
    * 
    * We will add the modified patterns to the master source files.
    *
    * As well, please review the list of "Future Strings" section below. 
    */
    regexPatterns: {
        jan: /^jan(uary)?/i,
        feb: /^feb(ruary)?/i,
        mar: /^mar(ch)?/i,
        apr: /^apr(il)?/i,
        may: /^may/i,
        jun: /^jun(e)?/i,
        jul: /^jul(y)?/i,
        aug: /^aug(ust)?/i,
        sep: /^sep(t(ember)?)?/i,
        oct: /^oct(ober)?/i,
        nov: /^nov(ember)?/i,
        dec: /^dec(ember)?/i,

        sun: /^su(n(day)?)?/i,
        mon: /^mo(n(day)?)?/i,
        tue: /^tu(e(s(day)?)?)?/i,
        wed: /^we(d(nesday)?)?/i,
        thu: /^th(u(r(s(day)?)?)?)?/i,
        fri: /^fr(i(day)?)?/i,
        sat: /^sa(t(urday)?)?/i,

        future: /^next/i,
        past: /^last|past|prev(ious)?/i,
        add: /^(\+|aft(er)?|from|hence)/i,
        subtract: /^(\-|bef(ore)?|ago)/i,

        yesterday: /^yes(terday)?/i,
        today: /^t(od(ay)?)?/i,
        tomorrow: /^tom(orrow)?/i,
        now: /^n(ow)?/i,

        millisecond: /^ms|milli(second)?s?/i,
        second: /^sec(ond)?s?/i,
        minute: /^mn|min(ute)?s?/i,
        hour: /^h(our)?s?/i,
        week: /^w(eek)?s?/i,
        month: /^m(onth)?s?/i,
        day: /^d(ay)?s?/i,
        year: /^y(ear)?s?/i,

        shortMeridian: /^(a|p)/i,
        longMeridian: /^(a\.?m?\.?|p\.?m?\.?)/i,
        timezone: /^((e(s|d)t|c(s|d)t|m(s|d)t|p(s|d)t)|((gmt)?\s*(\+|\-)\s*\d\d\d\d?)|gmt|utc)/i,
        ordinalSuffix: /^\s*(st|nd|rd|th)/i,
        timeContext: /^\s*(\:|a(?!u|p)|p)/i
    },

    timezones: [{ name: "UTC", offset: "-000" }, { name: "GMT", offset: "-000" }, { name: "EST", offset: "-0500" }, { name: "EDT", offset: "-0400" }, { name: "CST", offset: "-0600" }, { name: "CDT", offset: "-0500" }, { name: "MST", offset: "-0700" }, { name: "MDT", offset: "-0600" }, { name: "PST", offset: "-0800" }, { name: "PDT", offset: "-0700"}]
};

/********************
** Future Strings **
********************
* 
* The following list of strings may not be currently being used, but 
* may be incorporated into the Datejs library later. 
*
* We would appreciate any help translating the strings below.
* 
* If you modify this file, please post your revised CultureInfo file
* to the Datejs Forum located at http://www.datejs.com/forums/.
*
* Please mark the subject of the post with [CultureInfo]. Example:
*    Subject: [CultureInfo] Translated "da-DK" Danish(Denmark)b
*
* English Name        Translated
* ------------------  -----------------
* about               about
* ago                 ago
* date                date
* time                time
* calendar            calendar
* show                show
* hourly              hourly
* daily               daily
* weekly              weekly
* bi-weekly           bi-weekly
* fortnight           fortnight
* monthly             monthly
* bi-monthly          bi-monthly
* quarter             quarter
* quarterly           quarterly
* yearly              yearly
* annual              annual
* annually            annually
* annum               annum
* again               again
* between             between
* after               after
* from now            from now
* repeat              repeat
* times               times
* per                 per
* min (abbrev minute) min
* morning             morning
* noon                noon
* night               night
* midnight            midnight
* mid-night           mid-night
* evening             evening
* final               final
* future              future
* spring              spring
* summer              summer
* fall                fall
* winter              winter
* end of              end of
* end                 end
* long                long
* short               short
*/

(function () {
    var $D = Date,
        $P = $D.prototype,
        $C = $D.CultureInfo,
        p = function (s, l) {
            if (!l) {
                l = 2;
            }
            return ("000" + s).slice(l * -1);
        };

    /**
    * Resets the time of this Date object to 12:00 AM (00:00), which is the start of the day.
    * @param {Boolean}  .clone() this date instance before clearing Time
    * @return {Date}    this
    */
    $P.clearTime = function () {
        this.setHours(0);
        this.setMinutes(0);
        this.setSeconds(0);
        this.setMilliseconds(0);
        return this;
    };

    /**
    * Resets the time of this Date object to the current time ('now').
    * @return {Date}    this
    */
    $P.setTimeToNow = function () {
        var n = new Date();
        this.setHours(n.getHours());
        this.setMinutes(n.getMinutes());
        this.setSeconds(n.getSeconds());
        this.setMilliseconds(n.getMilliseconds());
        return this;
    };

    /** 
    * Gets a date that is set to the current date. The time is set to the start of the day (00:00 or 12:00 AM).
    * @return {Date}    The current date.
    */
    $D.today = function () {
        return new Date().clearTime();
    };

    /**
    * Compares the first date to the second date and returns an number indication of their relative values.  
    * @param {Date}     First Date object to compare [Required].
    * @param {Date}     Second Date object to compare to [Required].
    * @return {Number}  -1 = date1 is lessthan date2. 0 = values are equal. 1 = date1 is greaterthan date2.
    */
    $D.compare = function (date1, date2) {
        if (isNaN(date1) || isNaN(date2)) {
            throw new Error(date1 + " - " + date2);
        } else if (date1 instanceof Date && date2 instanceof Date) {
            return (date1 < date2) ? -1 : (date1 > date2) ? 1 : 0;
        } else {
            throw new TypeError(date1 + " - " + date2);
        }
    };

    /**
    * Compares the first Date object to the second Date object and returns true if they are equal.  
    * @param {Date}     First Date object to compare [Required]
    * @param {Date}     Second Date object to compare to [Required]
    * @return {Boolean} true if dates are equal. false if they are not equal.
    */
    $D.equals = function (date1, date2) {
        return (date1.compareTo(date2) === 0);
    };

    /**
    * Gets the day number (0-6) if given a CultureInfo specific string which is a valid dayName, abbreviatedDayName or shortestDayName (two char).
    * @param {String}   The name of the day (eg. "Monday, "Mon", "tuesday", "tue", "We", "we").
    * @return {Number}  The day number
    */
    $D.getDayNumberFromName = function (name) {
        var n = $C.dayNames, m = $C.abbreviatedDayNames, o = $C.shortestDayNames, s = name.toLowerCase();
        for (var i = 0; i < n.length; i++) {
            if (n[i].toLowerCase() == s || m[i].toLowerCase() == s || o[i].toLowerCase() == s) {
                return i;
            }
        }
        return -1;
    };

    /**
    * Gets the month number (0-11) if given a Culture Info specific string which is a valid monthName or abbreviatedMonthName.
    * @param {String}   The name of the month (eg. "February, "Feb", "october", "oct").
    * @return {Number}  The day number
    */
    $D.getMonthNumberFromName = function (name) {
        var n = $C.monthNames, m = $C.abbreviatedMonthNames, s = name.toLowerCase();
        for (var i = 0; i < n.length; i++) {
            if (n[i].toLowerCase() == s || m[i].toLowerCase() == s) {
                return i;
            }
        }
        return -1;
    };

    /**
    * Determines if the current date instance is within a LeapYear.
    * @param {Number}   The year.
    * @return {Boolean} true if date is within a LeapYear, otherwise false.
    */
    $D.isLeapYear = function (year) {
        return ((year % 4 === 0 && year % 100 !== 0) || year % 400 === 0);
    };

    /**
    * Gets the number of days in the month, given a year and month value. Automatically corrects for LeapYear.
    * @param {Number}   The year.
    * @param {Number}   The month (0-11).
    * @return {Number}  The number of days in the month.
    */
    $D.getDaysInMonth = function (year, month) {
        return [31, ($D.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
    };

    $D.getTimezoneAbbreviation = function (offset) {
        var z = $C.timezones, p;
        for (var i = 0; i < z.length; i++) {
            if (z[i].offset === offset) {
                return z[i].name;
            }
        }
        return null;
    };

    $D.getTimezoneOffset = function (name) {
        var z = $C.timezones, p;
        for (var i = 0; i < z.length; i++) {
            if (z[i].name === name.toUpperCase()) {
                return z[i].offset;
            }
        }
        return null;
    };

    /**
    * Returns a new Date object that is an exact date and time copy of the original instance.
    * @return {Date}    A new Date instance
    */
    $P.clone = function () {
        return new Date(this.getTime());
    };

    /**
    * Compares this instance to a Date object and returns an number indication of their relative values.  
    * @param {Date}     Date object to compare [Required]
    * @return {Number}  -1 = this is lessthan date. 0 = values are equal. 1 = this is greaterthan date.
    */
    $P.compareTo = function (date) {
        return Date.compare(this, date);
    };

    /**
    * Compares this instance to another Date object and returns true if they are equal.  
    * @param {Date}     Date object to compare. If no date to compare, new Date() [now] is used.
    * @return {Boolean} true if dates are equal. false if they are not equal.
    */
    $P.equals = function (date) {
        return Date.equals(this, date || new Date());
    };

    /**
    * Determines if this instance is between a range of two dates or equal to either the start or end dates.
    * @param {Date}     Start of range [Required]
    * @param {Date}     End of range [Required]
    * @return {Boolean} true is this is between or equal to the start and end dates, else false
    */
    $P.between = function (start, end) {
        return this.getTime() >= start.getTime() && this.getTime() <= end.getTime();
    };

    /**
    * Determines if this date occurs after the date to compare to.
    * @param {Date}     Date object to compare. If no date to compare, new Date() ("now") is used.
    * @return {Boolean} true if this date instance is greater than the date to compare to (or "now"), otherwise false.
    */
    $P.isAfter = function (date) {
        return this.compareTo(date || new Date()) === 1;
    };

    /**
    * Determines if this date occurs before the date to compare to.
    * @param {Date}     Date object to compare. If no date to compare, new Date() ("now") is used.
    * @return {Boolean} true if this date instance is less than the date to compare to (or "now").
    */
    $P.isBefore = function (date) {
        return (this.compareTo(date || new Date()) === -1);
    };

    /**
    * Determines if the current Date instance occurs today.
    * @return {Boolean} true if this date instance is 'today', otherwise false.
    */

    /**
    * Determines if the current Date instance occurs on the same Date as the supplied 'date'. 
    * If no 'date' to compare to is provided, the current Date instance is compared to 'today'. 
    * @param {date}     Date object to compare. If no date to compare, the current Date ("now") is used.
    * @return {Boolean} true if this Date instance occurs on the same Day as the supplied 'date'.
    */
    $P.isToday = $P.isSameDay = function (date) {
        return this.clone().clearTime().equals((date || new Date()).clone().clearTime());
    };

    /**
    * Adds the specified number of milliseconds to this instance. 
    * @param {Number}   The number of milliseconds to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addMilliseconds = function (value) {
        this.setMilliseconds(this.getMilliseconds() + value * 1);
        return this;
    };

    /**
    * Adds the specified number of seconds to this instance. 
    * @param {Number}   The number of seconds to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addSeconds = function (value) {
        return this.addMilliseconds(value * 1000);
    };

    /**
    * Adds the specified number of seconds to this instance. 
    * @param {Number}   The number of seconds to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addMinutes = function (value) {
        return this.addMilliseconds(value * 60000); /* 60*1000 */
    };

    /**
    * Adds the specified number of hours to this instance. 
    * @param {Number}   The number of hours to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addHours = function (value) {
        return this.addMilliseconds(value * 3600000); /* 60*60*1000 */
    };

    /**
    * Adds the specified number of days to this instance. 
    * @param {Number}   The number of days to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addDays = function (value) {
        this.setDate(this.getDate() + value * 1);
        return this;
    };

    /**
    * Adds the specified number of weeks to this instance. 
    * @param {Number}   The number of weeks to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addWeeks = function (value) {
        return this.addDays(value * 7);
    };

    /**
    * Adds the specified number of months to this instance. 
    * @param {Number}   The number of months to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addMonths = function (value) {
        var n = this.getDate();
        this.setDate(1);
        this.setMonth(this.getMonth() + value * 1);
        this.setDate(Math.min(n, $D.getDaysInMonth(this.getFullYear(), this.getMonth())));
        return this;
    };

    /**
    * Adds the specified number of years to this instance. 
    * @param {Number}   The number of years to add. The number can be positive or negative [Required]
    * @return {Date}    this
    */
    $P.addYears = function (value) {
        return this.addMonths(value * 12);
    };

    /**
    * Adds (or subtracts) to the value of the years, months, weeks, days, hours, minutes, seconds, milliseconds of the date instance using given configuration object. Positive and Negative values allowed.
    * Example
    <pre><code>
    Date.today().add( { days: 1, months: 1 } )
     
    new Date().add( { years: -1 } )
    </code></pre> 
    * @param {Object}   Configuration object containing attributes (months, days, etc.)
    * @return {Date}    this
    */
    $P.add = function (config) {
        if (typeof config == "number") {
            this._orient = config;
            return this;
        }

        var x = config;

        if (x.milliseconds) {
            this.addMilliseconds(x.milliseconds);
        }
        if (x.seconds) {
            this.addSeconds(x.seconds);
        }
        if (x.minutes) {
            this.addMinutes(x.minutes);
        }
        if (x.hours) {
            this.addHours(x.hours);
        }
        if (x.weeks) {
            this.addWeeks(x.weeks);
        }
        if (x.months) {
            this.addMonths(x.months);
        }
        if (x.years) {
            this.addYears(x.years);
        }
        if (x.days) {
            this.addDays(x.days);
        }
        return this;
    };

    var $y, $m, $d;

    /**
    * Get the week number. Week one (1) is the week which contains the first Thursday of the year. Monday is considered the first day of the week.
    * This algorithm is a JavaScript port of the work presented by Claus Tøndering at http://www.tondering.dk/claus/cal/node8.html#SECTION00880000000000000000
    * .getWeek() Algorithm Copyright (c) 2008 Claus Tondering.
    * The .getWeek() function does NOT convert the date to UTC. The local datetime is used. Please use .getISOWeek() to get the week of the UTC converted date.
    * @return {Number}  1 to 53
    */
    $P.getWeek = function () {
        var a, b, c, d, e, f, g, n, s, w;

        $y = (!$y) ? this.getFullYear() : $y;
        $m = (!$m) ? this.getMonth() + 1 : $m;
        $d = (!$d) ? this.getDate() : $d;

        if ($m <= 2) {
            a = $y - 1;
            b = (a / 4 | 0) - (a / 100 | 0) + (a / 400 | 0);
            c = ((a - 1) / 4 | 0) - ((a - 1) / 100 | 0) + ((a - 1) / 400 | 0);
            s = b - c;
            e = 0;
            f = $d - 1 + (31 * ($m - 1));
        } else {
            a = $y;
            b = (a / 4 | 0) - (a / 100 | 0) + (a / 400 | 0);
            c = ((a - 1) / 4 | 0) - ((a - 1) / 100 | 0) + ((a - 1) / 400 | 0);
            s = b - c;
            e = s + 1;
            f = $d + ((153 * ($m - 3) + 2) / 5) + 58 + s;
        }

        g = (a + b) % 7;
        d = (f + g - e) % 7;
        n = (f + 3 - d) | 0;

        if (n < 0) {
            w = 53 - ((g - s) / 5 | 0);
        } else if (n > 364 + s) {
            w = 1;
        } else {
            w = (n / 7 | 0) + 1;
        }

        $y = $m = $d = null;

        return w;
    };

    /**
    * Get the ISO 8601 week number. Week one ("01") is the week which contains the first Thursday of the year. Monday is considered the first day of the week.
    * The .getISOWeek() function does convert the date to it's UTC value. Please use .getWeek() to get the week of the local date.
    * @return {String}  "01" to "53"
    */
    $P.getISOWeek = function () {
        $y = this.getUTCFullYear();
        $m = this.getUTCMonth() + 1;
        $d = this.getUTCDate();
        return p(this.getWeek());
    };

    /**
    * Moves the date to Monday of the week set. Week one (1) is the week which contains the first Thursday of the year.
    * @param {Number}   A Number (1 to 53) that represents the week of the year.
    * @return {Date}    this
    */
    $P.setWeek = function (n) {
        return this.moveToDayOfWeek(1).addWeeks(n - this.getWeek());
    };

    // private
    var validate = function (n, min, max, name) {
        if (typeof n == "undefined") {
            return false;
        } else if (typeof n != "number") {
            throw new TypeError(n + " is not a Number.");
        } else if (n < min || n > max) {
            throw new RangeError(n + " is not a valid value for " + name + ".");
        }
        return true;
    };

    /**
    * Validates the number is within an acceptable range for milliseconds [0-999].
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateMillisecond = function (value) {
        return validate(value, 0, 999, "millisecond");
    };

    /**
    * Validates the number is within an acceptable range for seconds [0-59].
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateSecond = function (value) {
        return validate(value, 0, 59, "second");
    };

    /**
    * Validates the number is within an acceptable range for minutes [0-59].
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateMinute = function (value) {
        return validate(value, 0, 59, "minute");
    };

    /**
    * Validates the number is within an acceptable range for hours [0-23].
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateHour = function (value) {
        return validate(value, 0, 23, "hour");
    };

    /**
    * Validates the number is within an acceptable range for the days in a month [0-MaxDaysInMonth].
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateDay = function (value, year, month) {
        return validate(value, 1, $D.getDaysInMonth(year, month), "day");
    };

    /**
    * Validates the number is within an acceptable range for months [0-11].
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateMonth = function (value) {
        return validate(value, 0, 11, "month");
    };

    /**
    * Validates the number is within an acceptable range for years.
    * @param {Number}   The number to check if within range.
    * @return {Boolean} true if within range, otherwise false.
    */
    $D.validateYear = function (value) {
        return validate(value, 0, 9999, "year");
    };

    /**
    * Set the value of year, month, day, hour, minute, second, millisecond of date instance using given configuration object.
    * Example
    <pre><code>
    Date.today().set( { day: 20, month: 1 } )

    new Date().set( { millisecond: 0 } )
    </code></pre>
    * 
    * @param {Object}   Configuration object containing attributes (month, day, etc.)
    * @return {Date}    this
    */
    $P.set = function (config) {
        if ($D.validateMillisecond(config.millisecond)) {
            this.addMilliseconds(config.millisecond - this.getMilliseconds());
        }

        if ($D.validateSecond(config.second)) {
            this.addSeconds(config.second - this.getSeconds());
        }

        if ($D.validateMinute(config.minute)) {
            this.addMinutes(config.minute - this.getMinutes());
        }

        if ($D.validateHour(config.hour)) {
            this.addHours(config.hour - this.getHours());
        }

        if ($D.validateMonth(config.month)) {
            this.addMonths(config.month - this.getMonth());
        }

        if ($D.validateYear(config.year)) {
            this.addYears(config.year - this.getFullYear());
        }

        /* day has to go last because you can't validate the day without first knowing the month */
        if ($D.validateDay(config.day, this.getFullYear(), this.getMonth())) {
            this.addDays(config.day - this.getDate());
        }

        if (config.timezone) {
            this.setTimezone(config.timezone);
        }

        if (config.timezoneOffset) {
            this.setTimezoneOffset(config.timezoneOffset);
        }

        if (config.week && validate(config.week, 0, 53, "week")) {
            this.setWeek(config.week);
        }

        return this;
    };

    /**
    * Moves the date to the first day of the month.
    * @return {Date}    this
    */
    $P.moveToFirstDayOfMonth = function () {
        return this.set({ day: 1 });
    };

    /**
    * Moves the date to the last day of the month.
    * @return {Date}    this
    */
    $P.moveToLastDayOfMonth = function () {
        return this.set({ day: $D.getDaysInMonth(this.getFullYear(), this.getMonth()) });
    };

    /**
    * Moves the date to the next n'th occurrence of the dayOfWeek starting from the beginning of the month. The number (-1) is a magic number and will return the last occurrence of the dayOfWeek in the month.
    * @param {Number}   The dayOfWeek to move to
    * @param {Number}   The n'th occurrence to move to. Use (-1) to return the last occurrence in the month
    * @return {Date}    this
    */
    $P.moveToNthOccurrence = function (dayOfWeek, occurrence) {
        var shift = 0;
        if (occurrence > 0) {
            shift = occurrence - 1;
        }
        else if (occurrence === -1) {
            this.moveToLastDayOfMonth();
            if (this.getDay() !== dayOfWeek) {
                this.moveToDayOfWeek(dayOfWeek, -1);
            }
            return this;
        }
        return this.moveToFirstDayOfMonth().addDays(-1).moveToDayOfWeek(dayOfWeek, +1).addWeeks(shift);
    };

    /**
    * Move to the next or last dayOfWeek based on the orient value.
    * @param {Number}   The dayOfWeek to move to
    * @param {Number}   Forward (+1) or Back (-1). Defaults to +1. [Optional]
    * @return {Date}    this
    */
    $P.moveToDayOfWeek = function (dayOfWeek, orient) {
        var diff = (dayOfWeek - this.getDay() + 7 * (orient || +1)) % 7;
        return this.addDays((diff === 0) ? diff += 7 * (orient || +1) : diff);
    };

    /**
    * Move to the next or last month based on the orient value.
    * @param {Number}   The month to move to. 0 = January, 11 = December
    * @param {Number}   Forward (+1) or Back (-1). Defaults to +1. [Optional]
    * @return {Date}    this
    */
    $P.moveToMonth = function (month, orient) {
        var diff = (month - this.getMonth() + 12 * (orient || +1)) % 12;
        return this.addMonths((diff === 0) ? diff += 12 * (orient || +1) : diff);
    };

    /**
    * Get the Ordinal day (numeric day number) of the year, adjusted for leap year.
    * @return {Number} 1 through 365 (366 in leap years)
    */
    $P.getOrdinalNumber = function () {
        return Math.ceil((this.clone().clearTime() - new Date(this.getFullYear(), 0, 1)) / 86400000) + 1;
    };

    /**
    * Get the time zone abbreviation of the current date.
    * @return {String} The abbreviated time zone name (e.g. "EST")
    */
    $P.getTimezone = function () {
        return $D.getTimezoneAbbreviation(this.getUTCOffset());
    };

    $P.setTimezoneOffset = function (offset) {
        var here = this.getTimezoneOffset(), there = Number(offset) * -6 / 10;
        return this.addMinutes(there - here);
    };

    $P.setTimezone = function (offset) {
        return this.setTimezoneOffset($D.getTimezoneOffset(offset));
    };

    /**
    * Indicates whether Daylight Saving Time is observed in the current time zone.
    * @return {Boolean} true|false
    */
    $P.hasDaylightSavingTime = function () {
        return (Date.today().set({ month: 0, day: 1 }).getTimezoneOffset() !== Date.today().set({ month: 6, day: 1 }).getTimezoneOffset());
    };

    /**
    * Indicates whether this Date instance is within the Daylight Saving Time range for the current time zone.
    * @return {Boolean} true|false
    */
    $P.isDaylightSavingTime = function () {
        return Date.today().set({ month: 0, day: 1 }).getTimezoneOffset() != this.getTimezoneOffset();
    };

    /**
    * Get the offset from UTC of the current date.
    * @return {String} The 4-character offset string prefixed with + or - (e.g. "-0500")
    */
    $P.getUTCOffset = function () {
        var n = this.getTimezoneOffset() * -10 / 6, r;
        if (n < 0) {
            r = (n - 10000).toString();
            return r.charAt(0) + r.substr(2);
        } else {
            r = (n + 10000).toString();
            return "+" + r.substr(1);
        }
    };

    /**
    * Returns the number of milliseconds between this date and date.
    * @param {Date} Defaults to now
    * @return {Number} The diff in milliseconds
    */
    $P.getElapsed = function (date) {
        return (date || new Date()) - this;
    };

    if (!$P.toISOString) {
        /**
        * Converts the current date instance into a string with an ISO 8601 format. The date is converted to it's UTC value.
        * @return {String}  ISO 8601 string of date
        */
        $P.toISOString = function () {
            // From http://www.json.org/json.js. Public Domain. 
            function f(n) {
                return n < 10 ? '0' + n : n;
            }

            return '"' + this.getUTCFullYear() + '-' +
                f(this.getUTCMonth() + 1) + '-' +
                f(this.getUTCDate()) + 'T' +
                f(this.getUTCHours()) + ':' +
                f(this.getUTCMinutes()) + ':' +
                f(this.getUTCSeconds()) + 'Z"';
        };
    }

    // private
    $P._toString = $P.toString;

    /**
    * Converts the value of the current Date object to its equivalent string representation.
    * Format Specifiers
    <pre>
    CUSTOM DATE AND TIME FORMAT STRINGS
    Format  Description                                                                  Example
    ------  ---------------------------------------------------------------------------  -----------------------
    s      The seconds of the minute between 0-59.                                      "0" to "59"
    ss     The seconds of the minute with leading zero if required.                     "00" to "59"
     
    m      The minute of the hour between 0-59.                                         "0"  or "59"
    mm     The minute of the hour with leading zero if required.                        "00" or "59"
     
    h      The hour of the day between 1-12.                                            "1"  to "12"
    hh     The hour of the day with leading zero if required.                           "01" to "12"
     
    H      The hour of the day between 0-23.                                            "0"  to "23"
    HH     The hour of the day with leading zero if required.                           "00" to "23"
     
    d      The day of the month between 1 and 31.                                       "1"  to "31"
    dd     The day of the month with leading zero if required.                          "01" to "31"
    ddd    Abbreviated day name. $C.abbreviatedDayNames.                                "Mon" to "Sun" 
    dddd   The full day name. $C.dayNames.                                              "Monday" to "Sunday"
     
    M      The month of the year between 1-12.                                          "1" to "12"
    MM     The month of the year with leading zero if required.                         "01" to "12"
    MMM    Abbreviated month name. $C.abbreviatedMonthNames.                            "Jan" to "Dec"
    MMMM   The full month name. $C.monthNames.                                          "January" to "December"

    yy     The year as a two-digit number.                                              "99" or "08"
    yyyy   The full four digit year.                                                    "1999" or "2008"
     
    t      Displays the first character of the A.M./P.M. designator.                    "A" or "P"
    $C.amDesignator or $C.pmDesignator
    tt     Displays the A.M./P.M. designator.                                           "AM" or "PM"
    $C.amDesignator or $C.pmDesignator
     
    S      The ordinal suffix ("st, "nd", "rd" or "th") of the current day.            "st, "nd", "rd" or "th"

    || *Format* || *Description* || *Example* ||
    || d      || The CultureInfo shortDate Format Pattern                                     || "M/d/yyyy" ||
    || D      || The CultureInfo longDate Format Pattern                                      || "dddd, MMMM dd, yyyy" ||
    || F      || The CultureInfo fullDateTime Format Pattern                                  || "dddd, MMMM dd, yyyy h:mm:ss tt" ||
    || m      || The CultureInfo monthDay Format Pattern                                      || "MMMM dd" ||
    || r      || The CultureInfo rfc1123 Format Pattern                                       || "ddd, dd MMM yyyy HH:mm:ss GMT" ||
    || s      || The CultureInfo sortableDateTime Format Pattern                              || "yyyy-MM-ddTHH:mm:ss" ||
    || t      || The CultureInfo shortTime Format Pattern                                     || "h:mm tt" ||
    || T      || The CultureInfo longTime Format Pattern                                      || "h:mm:ss tt" ||
    || u      || The CultureInfo universalSortableDateTime Format Pattern                     || "yyyy-MM-dd HH:mm:ssZ" ||
    || y      || The CultureInfo yearMonth Format Pattern                                     || "MMMM, yyyy" ||
     

    STANDARD DATE AND TIME FORMAT STRINGS
    Format  Description                                                                  Example ("en-US")
    ------  ---------------------------------------------------------------------------  -----------------------
    d      The CultureInfo shortDate Format Pattern                                     "M/d/yyyy"
    D      The CultureInfo longDate Format Pattern                                      "dddd, MMMM dd, yyyy"
    F      The CultureInfo fullDateTime Format Pattern                                  "dddd, MMMM dd, yyyy h:mm:ss tt"
    m      The CultureInfo monthDay Format Pattern                                      "MMMM dd"
    r      The CultureInfo rfc1123 Format Pattern                                       "ddd, dd MMM yyyy HH:mm:ss GMT"
    s      The CultureInfo sortableDateTime Format Pattern                              "yyyy-MM-ddTHH:mm:ss"
    t      The CultureInfo shortTime Format Pattern                                     "h:mm tt"
    T      The CultureInfo longTime Format Pattern                                      "h:mm:ss tt"
    u      The CultureInfo universalSortableDateTime Format Pattern                     "yyyy-MM-dd HH:mm:ssZ"
    y      The CultureInfo yearMonth Format Pattern                                     "MMMM, yyyy"
    </pre>
    * @param {String}   A format string consisting of one or more format spcifiers [Optional].
    * @return {String}  A string representation of the current Date object.
    */
    $P.toString = function (format) {
        var x = this;

        // Standard Date and Time Format Strings. Formats pulled from CultureInfo file and
        // may vary by culture. 
        if (format && format.length == 1) {
            var c = $C.formatPatterns;
            x.t = x.toString;
            switch (format) {
                case "d":
                    return x.t(c.shortDate);
                case "D":
                    return x.t(c.longDate);
                case "F":
                    return x.t(c.fullDateTime);
                case "m":
                    return x.t(c.monthDay);
                case "r":
                    return x.t(c.rfc1123);
                case "s":
                    return x.t(c.sortableDateTime);
                case "t":
                    return x.t(c.shortTime);
                case "T":
                    return x.t(c.longTime);
                case "u":
                    return x.t(c.universalSortableDateTime);
                case "y":
                    return x.t(c.yearMonth);
            }
        }

        var ord = function (n) {
            switch (n * 1) {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        };

        return format ? format.replace(/(\\)?(dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|S)/g,
        function (m) {
            if (m.charAt(0) === "\\") {
                return m.replace("\\", "");
            }
            x.h = x.getHours;
            switch (m) {
                case "hh":
                    return p(x.h() < 13 ? (x.h() === 0 ? 12 : x.h()) : (x.h() - 12));
                case "h":
                    return x.h() < 13 ? (x.h() === 0 ? 12 : x.h()) : (x.h() - 12);
                case "HH":
                    return p(x.h());
                case "H":
                    return x.h();
                case "mm":
                    return p(x.getMinutes());
                case "m":
                    return x.getMinutes();
                case "ss":
                    return p(x.getSeconds());
                case "s":
                    return x.getSeconds();
                case "yyyy":
                    return p(x.getFullYear(), 4);
                case "yy":
                    return p(x.getFullYear());
                case "dddd":
                    return $C.dayNames[x.getDay()];
                case "ddd":
                    return $C.abbreviatedDayNames[x.getDay()];
                case "dd":
                    return p(x.getDate());
                case "d":
                    return x.getDate();
                case "MMMM":
                    return $C.monthNames[x.getMonth()];
                case "MMM":
                    return $C.abbreviatedMonthNames[x.getMonth()];
                case "MM":
                    return p((x.getMonth() + 1));
                case "M":
                    return x.getMonth() + 1;
                case "t":
                    return x.h() < 12 ? $C.amDesignator.substring(0, 1) : $C.pmDesignator.substring(0, 1);
                case "tt":
                    return x.h() < 12 ? $C.amDesignator : $C.pmDesignator;
                case "S":
                    return ord(x.getDate());
                default:
                    return m;
            }
        }
        ) : this._toString();
    };
} ());

(function () {
    Date.Parsing = {
        Exception: function (s) {
            this.message = "Parse error at '" + s.substring(0, 10) + " ...'";
        }
    };
   
    var $P = Date.Parsing;
    var _ = $P.Operators = {
        //
        // Tokenizers
        //
        rtoken: function (r) { // regex token
            return function (s) {
                var mx = s.match(r);
                if (mx) {
                    return ([ mx[0], s.substring(mx[0].length) ]);
                } else {
                    throw new $P.Exception(s);
                }
            };
        },
        token: function (s) { // whitespace-eating token
            return function (s) {
                return _.rtoken(new RegExp("^\s*" + s + "\s*"))(s);
                // Removed .strip()
                // return _.rtoken(new RegExp("^\s*" + s + "\s*"))(s).strip();
            };
        },
        stoken: function (s) { // string token
            return _.rtoken(new RegExp("^" + s));
        },

        //
        // Atomic Operators
        //

        until: function (p) {
            return function (s) {
                var qx = [], rx = null;
                while (s.length) {
                    try {
                        rx = p.call(this, s);
                    } catch (e) {
                        qx.push(rx[0]);
                        s = rx[1];
                        continue;
                    }
                    break;
                }
                return [ qx, s ];
            };
        },
        many: function (p) {
            return function (s) {
                var rx = [], r = null;
                while (s.length) {
                    try {
                        r = p.call(this, s);
                    } catch (e) {
                        return [ rx, s ];
                    }
                    rx.push(r[0]);
                    s = r[1];
                }
                return [ rx, s ];
            };
        },

        // generator operators -- see below
        optional: function (p) {
            return function (s) {
                var r = null;
                try {
                    r = p.call(this, s);
                } catch (e) {
                    return [ null, s ];
                }
                return [ r[0], r[1] ];
            };
        },
        not: function (p) {
            return function (s) {
                try {
                    p.call(this, s);
                } catch (e) {
                    return [null, s];
                }
                throw new $P.Exception(s);
            };
        },
        ignore: function (p) {
            return p ?
            function (s) {
                var r = null;
                r = p.call(this, s);
                return [null, r[1]];
            } : null;
        },
        product: function () {
            var px = arguments[0],
            qx = Array.prototype.slice.call(arguments, 1), rx = [];
            for (var i = 0 ; i < px.length ; i++) {
                rx.push(_.each(px[i], qx));
            }
            return rx;
        },
        cache: function (rule) {
            var cache = {}, r = null;
            return function (s) {
                try {
                    r = cache[s] = (cache[s] || rule.call(this, s));
                } catch (e) {
                    r = cache[s] = e;
                }
                if (r instanceof $P.Exception) {
                    throw r;
                } else {
                    return r;
                }
            };
        },
         
        // vector operators -- see below
        any: function () {
            var px = arguments;
            return function (s) {
                var r = null;
                for (var i = 0; i < px.length; i++) {
                    if (px[i] == null) {
                        continue;
                    }
                    try {
                        r = (px[i].call(this, s));
                    } catch (e) {
                        r = null;
                    }
                    if (r) {
                        return r;
                    }
                }
                throw new $P.Exception(s);
            };
        },
        each: function () {
            var px = arguments;
            return function (s) {
                var rx = [], r = null;
                for (var i = 0; i < px.length ; i++) {
                    if (px[i] == null) {
                        continue;
                    }
                    try {
                        r = (px[i].call(this, s));
                    } catch (e) {
                        throw new $P.Exception(s);
                    }
                    rx.push(r[0]);
                    s = r[1];
                }
                return [ rx, s];
            };
        },
        all: function () {
            var px = arguments, _ = _;
            return _.each(_.optional(px));
        },

        // delimited operators
        sequence: function (px, d, c) {
            d = d || _.rtoken(/^\s*/);  
            c = c || null;
           
            if (px.length == 1) {
                return px[0];
            }
            return function (s) {
                var r = null, q = null;
                var rx = [];
                for (var i = 0; i < px.length ; i++) {
                    try {
                        r = px[i].call(this, s);
                    } catch (e) {
                        break;
                    }
                    rx.push(r[0]);
                    try {
                        q = d.call(this, r[1]);
                    } catch (ex) {
                        q = null;
                        break;
                    }
                    s = q[1];
                }
                if (!r) {
                    throw new $P.Exception(s);
                }
                if (q) {
                    throw new $P.Exception(q[1]);
                }
                if (c) {
                    try {
                        r = c.call(this, r[1]);
                    } catch (ey) {
                        throw new $P.Exception(r[1]);
                    }
                }
                return [ rx, (r?r[1]:s) ];
            };
        },
               
            //
            // Composite Operators
            //
               
        between: function (d1, p, d2) {
            d2 = d2 || d1;
            var _fn = _.each(_.ignore(d1), p, _.ignore(d2));
            return function (s) {
                var rx = _fn.call(this, s);
                return [[rx[0][0], r[0][2]], rx[1]];
            };
        },
        list: function (p, d, c) {
            d = d || _.rtoken(/^\s*/);  
            c = c || null;
            return (p instanceof Array ?
                _.each(_.product(p.slice(0, -1), _.ignore(d)), p.slice(-1), _.ignore(c)) :
                _.each(_.many(_.each(p, _.ignore(d))), px, _.ignore(c)));
        },
        set: function (px, d, c) {
            d = d || _.rtoken(/^\s*/);
            c = c || null;
            return function (s) {
                // r is the current match, best the current 'best' match
                // which means it parsed the most amount of input
                var r = null, p = null, q = null, rx = null, best = [[], s], last = false;

                // go through the rules in the given set
                for (var i = 0; i < px.length ; i++) {

                    // last is a flag indicating whether this must be the last element
                    // if there is only 1 element, then it MUST be the last one
                    q = null;
                    p = null;
                    r = null;
                    last = (px.length == 1);

                    // first, we try simply to match the current pattern
                    // if not, try the next pattern
                    try {
                        r = px[i].call(this, s);
                    } catch (e) {
                        continue;
                    }

                    // since we are matching against a set of elements, the first
                    // thing to do is to add r[0] to matched elements
                    rx = [[r[0]], r[1]];

                    // if we matched and there is still input to parse and
                    // we don't already know this is the last element,
                    // we're going to next check for the delimiter ...
                    // if there's none, or if there's no input left to parse
                    // than this must be the last element after all ...
                    if (r[1].length > 0 && ! last) {
                        try {
                            q = d.call(this, r[1]);
                        } catch (ex) {
                            last = true;
                        }
                    } else {
                        last = true;
                    }

                                    // if we parsed the delimiter and now there's no more input,
                                    // that means we shouldn't have parsed the delimiter at all
                                    // so don't update r and mark this as the last element ...
                    if (!last && q[1].length === 0) {
                        last = true;
                    }


                                    // so, if this isn't the last element, we're going to see if
                                    // we can get any more matches from the remaining (unmatched)
                                    // elements ...
                    if (!last) {

                        // build a list of the remaining rules we can match against,
                        // i.e., all but the one we just matched against
                        var qx = [];
                        for (var j = 0; j < px.length ; j++) {
                            if (i != j) {
                                qx.push(px[j]);
                            }
                        }

                        // now invoke recursively set with the remaining input
                        // note that we don't include the closing delimiter ...
                        // we'll check for that ourselves at the end
                        p = _.set(qx, d).call(this, q[1]);

                        // if we got a non-empty set as a result ...
                        // (otw rx already contains everything we want to match)
                        if (p[0].length > 0) {
                            // update current result, which is stored in rx ...
                            // basically, pick up the remaining text from p[1]
                            // and concat the result from p[0] so that we don't
                            // get endless nesting ...
                            rx[0] = rx[0].concat(p[0]);
                            rx[1] = p[1];
                        }
                    }

                                    // at this point, rx either contains the last matched element
                                    // or the entire matched set that starts with this element.

                                    // now we just check to see if this variation is better than
                                    // our best so far, in terms of how much of the input is parsed
                    if (rx[1].length < best[1].length) {
                        best = rx;
                    }

                                    // if we've parsed all the input, then we're finished
                    if (best[1].length === 0) {
                        break;
                    }
                }

                            // so now we've either gone through all the patterns trying them
                            // as the initial match; or we found one that parsed the entire
                            // input string ...

                            // if best has no matches, just return empty set ...
                if (best[0].length === 0) {
                    return best;
                }

                            // if a closing delimiter is provided, then we have to check it also
                if (c) {
                    // we try this even if there is no remaining input because the pattern
                    // may well be optional or match empty input ...
                    try {
                        q = c.call(this, best[1]);
                    } catch (ey) {
                        throw new $P.Exception(best[1]);
                    }

                    // it parsed ... be sure to update the best match remaining input
                    best[1] = q[1];
                }

                            // if we're here, either there was no closing delimiter or we parsed it
                            // so now we have the best match; just return it!
                return best;
            };
        },
        forward: function (gr, fname) {
            return function (s) {
                return gr[fname].call(this, s);
            };
        },

        //
        // Translation Operators
        //
        replace: function (rule, repl) {
            return function (s) {
                var r = rule.call(this, s);
                return [repl, r[1]];
            };
        },
        process: function (rule, fn) {
            return function (s) {  
                var r = rule.call(this, s);
                return [fn.call(this, r[0]), r[1]];
            };
        },
        min: function (min, rule) {
            return function (s) {
                var rx = rule.call(this, s);
                if (rx[0].length < min) {
                    throw new $P.Exception(s);
                }
                return rx;
            };
        }
    };
       

        // Generator Operators And Vector Operators

        // Generators are operators that have a signature of F(R) => R,
        // taking a given rule and returning another rule, such as
        // ignore, which parses a given rule and throws away the result.

        // Vector operators are those that have a signature of F(R1,R2,...) => R,
        // take a list of rules and returning a new rule, such as each.

        // Generator operators are converted (via the following _generator
        // function) into functions that can also take a list or array of rules
        // and return an array of new rules as though the function had been
        // called on each rule in turn (which is what actually happens).

        // This allows generators to be used with vector operators more easily.
        // Example:
        // each(ignore(foo, bar)) instead of each(ignore(foo), ignore(bar))

        // This also turns generators into vector operators, which allows
        // constructs like:
        // not(cache(foo, bar))
       
    var _generator = function (op) {
        return function () {
            var args = null, rx = [];
            if (arguments.length > 1) {
                args = Array.prototype.slice.call(arguments);
            } else if (arguments[0] instanceof Array) {
                args = arguments[0];
            }
            if (args) {
                for (var i = 0, px = args.shift() ; i < px.length ; i++) {
                    args.unshift(px[i]);
                    rx.push(op.apply(null, args));
                    args.shift();
                    return rx;
                }
            } else {
                return op.apply(null, arguments);
            }
        };
    };
   
    var gx = "optional not ignore cache".split(/\s/);
   
    for (var i = 0 ; i < gx.length ; i++) {
        _[gx[i]] = _generator(_[gx[i]]);
    }

    var _vector = function (op) {
        return function () {
            if (arguments[0] instanceof Array) {
                return op.apply(null, arguments[0]);
            } else {
                return op.apply(null, arguments);
            }
        };
    };
   
    var vx = "each any all".split(/\s/);
   
    for (var j = 0 ; j < vx.length ; j++) {
        _[vx[j]] = _vector(_[vx[j]]);
    }
       
}());

(function () {
    var $D = Date, $P = $D.prototype, $C = $D.CultureInfo;

    var flattenAndCompact = function (ax) {
        var rx = [];
        for (var i = 0; i < ax.length; i++) {
            if (ax[i] instanceof Array) {
                rx = rx.concat(flattenAndCompact(ax[i]));
            } else {
                if (ax[i]) {
                    rx.push(ax[i]);
                }
            }
        }
        return rx;
    };
   
    $D.Grammar = {};
       
    $D.Translator = {
        hour: function (s) {
            return function () {
                this.hour = Number(s);
            };
        },
        minute: function (s) {
            return function () {
                this.minute = Number(s);
            };
        },
        second: function (s) {
            return function () {
                this.second = Number(s);
            };
        },
        meridian: function (s) {
            return function () {
                this.meridian = s.slice(0, 1).toLowerCase();
            };
        },
        timezone: function (s) {
            return function () {
                var n = s.replace(/[^\d\+\-]/g, "");
                if (n.length) {
                    this.timezoneOffset = Number(n);
                } else {
                    this.timezone = s.toLowerCase();
                }
            };
        },
        day: function (x) {
            var s = x[0];
            return function () {
                this.day = Number(s.match(/\d+/)[0]);
            };
        },
        month: function (s) {
            return function () {
                this.month = (s.length == 3) ? "jan feb mar apr may jun jul aug sep oct nov dec".indexOf(s)/4 : Number(s) - 1;
            };
        },
        year: function (s) {
            return function () {
                var n = Number(s);
                this.year = ((s.length > 2) ? n :
                    (n + (((n + 2000) < $C.twoDigitYearMax) ? 2000 : 1900)));
            };
        },
        rday: function (s) {
            return function () {
                switch (s) {
                case "yesterday":
                    this.days = -1;
                    break;
                case "tomorrow":  
                    this.days = 1;
                    break;
                case "today":
                    this.days = 0;
                    break;
                case "now":
                    this.days = 0;
                    this.now = true;
                    break;
                }
            };
        },
        finishExact: function (x) {  
            x = (x instanceof Array) ? x : [ x ];

            for (var i = 0 ; i < x.length ; i++) {
                if (x[i]) {
                    x[i].call(this);
                }
            }
           
            var now = new Date();
           
            if ((this.hour || this.minute) && (!this.month && !this.year && !this.day)) {
                this.day = now.getDate();
            }

            if (!this.year) {
                this.year = now.getFullYear();
            }
           
            if (!this.month && this.month !== 0) {
                this.month = now.getMonth();
            }
           
            if (!this.day) {
                this.day = 1;
            }
           
            if (!this.hour) {
                this.hour = 0;
            }
           
            if (!this.minute) {
                this.minute = 0;
            }

            if (!this.second) {
                this.second = 0;
            }

            if (this.meridian && this.hour) {
                if (this.meridian == "p" && this.hour < 12) {
                    this.hour = this.hour + 12;
                } else if (this.meridian == "a" && this.hour == 12) {
                    this.hour = 0;
                }
            }
           
            if (this.day > $D.getDaysInMonth(this.year, this.month)) {
                throw new RangeError(this.day + " is not a valid value for days.");
            }

            var r = new Date(this.year, this.month, this.day, this.hour, this.minute, this.second);

            if (this.timezone) {
                r.set({ timezone: this.timezone });
            } else if (this.timezoneOffset) {
                r.set({ timezoneOffset: this.timezoneOffset });
            }
           
            return r;
        },                      
        finish: function (x) {
            x = (x instanceof Array) ? flattenAndCompact(x) : [ x ];

            if (x.length === 0) {
                return null;
            }

            for (var i = 0 ; i < x.length ; i++) {
                if (typeof x[i] == "function") {
                    x[i].call(this);
                }
            }
           
            var today = $D.today();
           
            if (this.now && !this.unit && !this.operator) {
                return new Date();
            } else if (this.now) {
                today = new Date();
            }
           
            var expression = !!(this.days && this.days !== null || this.orient || this.operator);
           
            var gap, mod, orient;
            orient = ((this.orient == "past" || this.operator == "subtract") ? -1 : 1);
           
            if(!this.now && "hour minute second".indexOf(this.unit) != -1) {
                today.setTimeToNow();
            }

            if (this.month || this.month === 0) {
                if ("year day hour minute second".indexOf(this.unit) != -1) {
                    this.value = this.month + 1;
                    this.month = null;
                    expression = true;
                }
            }
           
            if (!expression && this.weekday && !this.day && !this.days) {
                var temp = Date[this.weekday]();
                this.day = temp.getDate();
                if (!this.month) {
                    this.month = temp.getMonth();
                }
                this.year = temp.getFullYear();
            }
           
            if (expression && this.weekday && this.unit != "month") {
                this.unit = "day";
                gap = ($D.getDayNumberFromName(this.weekday) - today.getDay());
                mod = 7;
                this.days = gap ? ((gap + (orient * mod)) % mod) : (orient * mod);
            }
           
            if (this.month && this.unit == "day" && this.operator) {
                this.value = (this.month + 1);
                this.month = null;
            }
       
            if (this.value != null && this.month != null && this.year != null) {
                this.day = this.value * 1;
            }
     
            if (this.month && !this.day && this.value) {
                today.set({ day: this.value * 1 });
                if (!expression) {
                    this.day = this.value * 1;
                }
            }

            if (!this.month && this.value && this.unit == "month" && !this.now) {
                this.month = this.value;
                expression = true;
            }

            if (expression && (this.month || this.month === 0) && this.unit != "year") {
                this.unit = "month";
                gap = (this.month - today.getMonth());
                mod = 12;
                this.months = gap ? ((gap + (orient * mod)) % mod) : (orient * mod);
                this.month = null;
            }

            if (!this.unit) {
                this.unit = "day";
            }
           
            if (!this.value && this.operator && this.operator !== null && this[this.unit + "s"] && this[this.unit + "s"] !== null) {
                this[this.unit + "s"] = this[this.unit + "s"] + ((this.operator == "add") ? 1 : -1) + (this.value||0) * orient;
            } else if (this[this.unit + "s"] == null || this.operator != null) {
                if (!this.value) {
                    this.value = 1;
                }
                this[this.unit + "s"] = this.value * orient;
            }

            if (this.meridian && this.hour) {
                if (this.meridian == "p" && this.hour < 12) {
                    this.hour = this.hour + 12;
                } else if (this.meridian == "a" && this.hour == 12) {
                    this.hour = 0;
                }
            }
           
            if (this.weekday && !this.day && !this.days) {
                var temp = Date[this.weekday]();
                this.day = temp.getDate();
                if (temp.getMonth() !== today.getMonth()) {
                    this.month = temp.getMonth();
                }
            }
           
            if ((this.month || this.month === 0) && !this.day) {
                this.day = 1;
            }
           
            if (!this.orient && !this.operator && this.unit == "week" && this.value && !this.day && !this.month) {
                return Date.today().setWeek(this.value);
            }

            if (expression && this.timezone && this.day && this.days) {
                this.day = this.days;
            }
           
            return (expression) ? today.add(this) : today.set(this);
        }
    };

    var _ = $D.Parsing.Operators, g = $D.Grammar, t = $D.Translator, _fn;

    g.datePartDelimiter = _.rtoken(/^([\s\-\.\,\/\x27]+)/);
    g.timePartDelimiter = _.stoken(":");
    g.whiteSpace = _.rtoken(/^\s*/);
    g.generalDelimiter = _.rtoken(/^(([\s\,]|at|@|on)+)/);
 
    var _C = {};
    g.ctoken = function (keys) {
        var fn = _C[keys];
        if (! fn) {
            var c = $C.regexPatterns;
            var kx = keys.split(/\s+/), px = [];
            for (var i = 0; i < kx.length ; i++) {
                px.push(_.replace(_.rtoken(c[kx[i]]), kx[i]));
            }
            fn = _C[keys] = _.any.apply(null, px);
        }
        return fn;
    };
    g.ctoken2 = function (key) {
        return _.rtoken($C.regexPatterns[key]);
    };

    // hour, minute, second, meridian, timezone
    g.h = _.cache(_.process(_.rtoken(/^(0[0-9]|1[0-2]|[1-9])/), t.hour));
    g.hh = _.cache(_.process(_.rtoken(/^(0[0-9]|1[0-2])/), t.hour));
    g.H = _.cache(_.process(_.rtoken(/^([0-1][0-9]|2[0-3]|[0-9])/), t.hour));
    g.HH = _.cache(_.process(_.rtoken(/^([0-1][0-9]|2[0-3])/), t.hour));
    g.m = _.cache(_.process(_.rtoken(/^([0-5][0-9]|[0-9])/), t.minute));
    g.mm = _.cache(_.process(_.rtoken(/^[0-5][0-9]/), t.minute));
    g.s = _.cache(_.process(_.rtoken(/^([0-5][0-9]|[0-9])/), t.second));
    g.ss = _.cache(_.process(_.rtoken(/^[0-5][0-9]/), t.second));
    g.hms = _.cache(_.sequence([g.H, g.m, g.s], g.timePartDelimiter));
 
    // _.min(1, _.set([ g.H, g.m, g.s ], g._t));
    g.t = _.cache(_.process(g.ctoken2("shortMeridian"), t.meridian));
    g.tt = _.cache(_.process(g.ctoken2("longMeridian"), t.meridian));
    g.z = _.cache(_.process(_.rtoken(/^((\+|\-)\s*\d\d\d\d)|((\+|\-)\d\d\:?\d\d)/), t.timezone));
    g.zz = _.cache(_.process(_.rtoken(/^((\+|\-)\s*\d\d\d\d)|((\+|\-)\d\d\:?\d\d)/), t.timezone));
   
    g.zzz = _.cache(_.process(g.ctoken2("timezone"), t.timezone));
    g.timeSuffix = _.each(_.ignore(g.whiteSpace), _.set([ g.tt, g.zzz ]));
    g.time = _.each(_.optional(_.ignore(_.stoken("T"))), g.hms, g.timeSuffix);
         
    // days, months, years
    g.d = _.cache(_.process(_.each(_.rtoken(/^([0-2]\d|3[0-1]|\d)/),
        _.optional(g.ctoken2("ordinalSuffix"))), t.day));
    g.dd = _.cache(_.process(_.each(_.rtoken(/^([0-2]\d|3[0-1])/),
        _.optional(g.ctoken2("ordinalSuffix"))), t.day));
    g.ddd = g.dddd = _.cache(_.process(g.ctoken("sun mon tue wed thu fri sat"),
        function (s) {
            return function () {
                this.weekday = s;
            };
        }
    ));
    g.M = _.cache(_.process(_.rtoken(/^(1[0-2]|0\d|\d)/), t.month));
    g.MM = _.cache(_.process(_.rtoken(/^(1[0-2]|0\d)/), t.month));
    g.MMM = g.MMMM = _.cache(_.process(
        g.ctoken("jan feb mar apr may jun jul aug sep oct nov dec"), t.month));
    g.y = _.cache(_.process(_.rtoken(/^(\d\d?)/), t.year));
    g.yy = _.cache(_.process(_.rtoken(/^(\d\d)/), t.year));
    g.yyy = _.cache(_.process(_.rtoken(/^(\d\d?\d?\d?)/), t.year));
    g.yyyy = _.cache(_.process(_.rtoken(/^(\d\d\d\d)/), t.year));
       
        // rolling these up into general purpose rules
    _fn = function () {
        return _.each(_.any.apply(null, arguments), _.not(g.ctoken2("timeContext")));
    };
   
    g.day = _fn(g.d, g.dd);
    g.month = _fn(g.M, g.MMM);
    g.year = _fn(g.yyyy, g.yy);

    // relative date / time expressions
    g.orientation = _.process(g.ctoken("past future"),
        function (s) {
            return function () {
                this.orient = s;
            };
        }
    );
    g.operator = _.process(g.ctoken("add subtract"),
        function (s) {
            return function () {
                this.operator = s;
            };
        }
    );  
    g.rday = _.process(g.ctoken("yesterday tomorrow today now"), t.rday);
    g.unit = _.process(g.ctoken("second minute hour day week month year"),
        function (s) {
            return function () {
                this.unit = s;
            };
        }
    );
    g.value = _.process(_.rtoken(/^\d\d?(st|nd|rd|th)?/),
        function (s) {
            return function () {
                this.value = s.replace(/\D/g, "");
            };
        }
    );
    g.expression = _.set([ g.rday, g.operator, g.value, g.unit, g.orientation, g.ddd, g.MMM ]);

    // pre-loaded rules for different date part order preferences
    _fn = function () {
        return  _.set(arguments, g.datePartDelimiter);
    };
    g.mdy = _fn(g.ddd, g.month, g.day, g.year);
    g.ymd = _fn(g.ddd, g.year, g.month, g.day);
    g.dmy = _fn(g.ddd, g.day, g.month, g.year);
    g.date = function (s) {
        return ((g[$C.dateElementOrder] || g.mdy).call(this, s));
    };

    // parsing date format specifiers - ex: "h:m:s tt"
    // this little guy will generate a custom parser based
    // on the format string, ex: g.format("h:m:s tt")
    g.format = _.process(_.many(
        _.any(
        // translate format specifiers into grammar rules
        _.process(
        _.rtoken(/^(dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|zz?z?)/),
        function (fmt) {
        if (g[fmt]) {
            return g[fmt];
        } else {
            throw $D.Parsing.Exception(fmt);
        }
    }
    ),
    // translate separator tokens into token rules
    _.process(
    _.rtoken(/^[^dMyhHmstz]+/), // all legal separators
        function (s) {
            return _.ignore(_.stoken(s));
        }
    )
    )),
        // construct the parser ...
        function (rules) {
            return _.process(_.each.apply(null, rules), t.finishExact);
        }
    );
   
    var _F = {
                //"M/d/yyyy": function (s) {
                //      var m = s.match(/^([0-2]\d|3[0-1]|\d)\/(1[0-2]|0\d|\d)\/(\d\d\d\d)/);
                //      if (m!=null) {
                //              var r =  [ t.month.call(this,m[1]), t.day.call(this,m[2]), t.year.call(this,m[3]) ];
                //              r = t.finishExact.call(this,r);
                //              return [ r, "" ];
                //      } else {
                //              throw new Date.Parsing.Exception(s);
                //      }
                //}
                //"M/d/yyyy": function (s) { return [ new Date(Date._parse(s)), ""]; }
        };
    var _get = function (f) {
        return _F[f] = (_F[f] || g.format(f)[0]);      
    };
 
    g.formats = function (fx) {
        if (fx instanceof Array) {
            var rx = [];
            for (var i = 0 ; i < fx.length ; i++) {
                rx.push(_get(fx[i]));
            }
            return _.any.apply(null, rx);
        } else {
            return _get(fx);
        }
    };

        // check for these formats first
    g._formats = g.formats([
        "\"yyyy-MM-ddTHH:mm:ssZ\"",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ssz",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mmZ",
        "yyyy-MM-ddTHH:mmz",
        "yyyy-MM-ddTHH:mm",
        "ddd, MMM dd, yyyy H:mm:ss tt",
        "ddd MMM d yyyy HH:mm:ss zzz",
        "MMddyyyy",
        "ddMMyyyy",
        "Mddyyyy",
        "ddMyyyy",
        "Mdyyyy",
        "dMyyyy",
        "yyyy",
        "Mdyy",
        "dMyy",
        "d"
    ]);

        // starting rule for general purpose grammar
    g._start = _.process(_.set([ g.date, g.time, g.expression ],
        g.generalDelimiter, g.whiteSpace), t.finish);
       
        // real starting rule: tries selected formats first,
        // then general purpose rule
    g.start = function (s) {
        try {
            var r = g._formats.call({}, s);
            if (r[1].length === 0) {
                return r;
            }
        } catch (e) {}
        return g._start.call({}, s);
    };
       
        $D._parse = $D.parse;

    /**
     * Converts the specified string value into its JavaScript Date equivalent using CultureInfo specific format information.
     *
     * Example
    <pre><code>
    ///////////
    // Dates //
    ///////////

    // 15-Oct-2004
    var d1 = Date.parse("10/15/2004");

    // 15-Oct-2004
    var d1 = Date.parse("15-Oct-2004");

    // 15-Oct-2004
    var d1 = Date.parse("2004.10.15");

    //Fri Oct 15, 2004
    var d1 = Date.parse("Fri Oct 15, 2004");

    ///////////
    // Times //
    ///////////

    // Today at 10 PM.
    var d1 = Date.parse("10 PM");

    // Today at 10:30 PM.
    var d1 = Date.parse("10:30 P.M.");

    // Today at 6 AM.
    var d1 = Date.parse("06am");

    /////////////////////
    // Dates and Times //
    /////////////////////

    // 8-July-2004 @ 10:30 PM
    var d1 = Date.parse("July 8th, 2004, 10:30 PM");

    // 1-July-2004 @ 10:30 PM
    var d1 = Date.parse("2004-07-01T22:30:00");

    ////////////////////
    // Relative Dates //
    ////////////////////

    // Returns today's date. The string "today" is culture specific.
    var d1 = Date.parse("today");

    // Returns yesterday's date. The string "yesterday" is culture specific.
    var d1 = Date.parse("yesterday");

    // Returns the date of the next thursday.
    var d1 = Date.parse("Next thursday");

    // Returns the date of the most previous monday.
    var d1 = Date.parse("last monday");

    // Returns today's day + one year.
    var d1 = Date.parse("next year");

    ///////////////
    // Date Math //
    ///////////////

    // Today + 2 days
    var d1 = Date.parse("t+2");

    // Today + 2 days
    var d1 = Date.parse("today + 2 days");

    // Today + 3 months
    var d1 = Date.parse("t+3m");

    // Today - 1 year
    var d1 = Date.parse("today - 1 year");

    // Today - 1 year
    var d1 = Date.parse("t-1y");


    /////////////////////////////
    // Partial Dates and Times //
    /////////////////////////////

    // July 15th of this year.
    var d1 = Date.parse("July 15");

    // 15th day of current day and year.
    var d1 = Date.parse("15");

    // July 1st of current year at 10pm.
    var d1 = Date.parse("7/1 10pm");
    </code></pre>
     *
     * @param {String}   The string value to convert into a Date object [Required]
     * @return {Date}    A Date object or null if the string cannot be converted into a Date.
     */
    $D.parse = function (s) {
        var r = null;
        if (!s) {
            return null;
        }
        if (s instanceof Date) {
            return s;
        }
        try {
            r = $D.Grammar.start.call({}, s.replace(/^\s*(\S*(\s+\S+)*)\s*$/, "$1"));
        } catch (e) {
            return null;
        }
        return ((r[1].length === 0) ? r[0] : null);
    };

    $D.getParseFunction = function (fx) {
        var fn = $D.Grammar.formats(fx);
        return function (s) {
            var r = null;
            try {
                r = fn.call({}, s);
            } catch (e) {
                return null;
            }
            return ((r[1].length === 0) ? r[0] : null);
        };
    };
   
    /**
     * Converts the specified string value into its JavaScript Date equivalent using the specified format {String} or formats {Array} and the CultureInfo specific format information.
     * The format of the string value must match one of the supplied formats exactly.
     *
     * Example
    <pre><code>
    // 15-Oct-2004
    var d1 = Date.parseExact("10/15/2004", "M/d/yyyy");

    // 15-Oct-2004
    var d1 = Date.parse("15-Oct-2004", "M-ddd-yyyy");

    // 15-Oct-2004
    var d1 = Date.parse("2004.10.15", "yyyy.MM.dd");

    // Multiple formats
    var d1 = Date.parseExact("10/15/2004", ["M/d/yyyy", "MMMM d, yyyy"]);
    </code></pre>
     *
     * @param {String}   The string value to convert into a Date object [Required].
     * @param {Object}   The expected format {String} or an array of expected formats {Array} of the date string [Required].
     * @return {Date}    A Date object or null if the string cannot be converted into a Date.
     */
    $D.parseExact = function (s, fx) {
        return $D.getParseFunction(fx)(s);
    };  
}());

(function () {
    var $D = Date, $P = $D.prototype, $C = $D.CultureInfo, $N = Number.prototype;

    // private
    $P._orient = +1;

    // private
    $P._nth = null;

    // private
    $P._is = false;

    // private
    $P._same = false;
    
    // private
    $P._isSecond = false;

    // private
    $N._dateElement = "day";

    /** 
     * Moves the date to the next instance of a date as specified by the subsequent date element function (eg. .day(), .month()), month name function (eg. .january(), .jan()) or day name function (eg. .friday(), fri()).
     * Example
    <pre><code>
    Date.today().next().friday();
    Date.today().next().fri();
    Date.today().next().march();
    Date.today().next().mar();
    Date.today().next().week();
    </code></pre>
     * 
     * @return {Date}    date
     */
    $P.next = function () {
        this._orient = +1;
        return this;
    };

    /** 
     * Creates a new Date (Date.today()) and moves the date to the next instance of the date as specified by the subsequent date element function (eg. .day(), .month()), month name function (eg. .january(), .jan()) or day name function (eg. .friday(), fri()).
     * Example
    <pre><code>
    Date.next().friday();
    Date.next().fri();
    Date.next().march();
    Date.next().mar();
    Date.next().week();
    </code></pre>
     * 
     * @return {Date}    date
     */    
    $D.next = function () {
        return $D.today().next();
    };

    /** 
     * Moves the date to the previous instance of a date as specified by the subsequent date element function (eg. .day(), .month()), month name function (eg. .january(), .jan()) or day name function (eg. .friday(), fri()).
     * Example
    <pre><code>
    Date.today().last().friday();
    Date.today().last().fri();
    Date.today().last().march();
    Date.today().last().mar();
    Date.today().last().week();
    </code></pre>
     *  
     * @return {Date}    date
     */
    $P.last = $P.prev = $P.previous = function () {
        this._orient = -1;
        return this;
    };

    /** 
     * Creates a new Date (Date.today()) and moves the date to the previous instance of the date as specified by the subsequent date element function (eg. .day(), .month()), month name function (eg. .january(), .jan()) or day name function (eg. .friday(), fri()).
     * Example
    <pre><code>
    Date.last().friday();
    Date.last().fri();
    Date.previous().march();
    Date.prev().mar();
    Date.last().week();
    </code></pre>
     *  
     * @return {Date}    date
     */
    $D.last = $D.prev = $D.previous = function () {
        return $D.today().last();
    };    

    /** 
     * Performs a equality check when followed by either a month name, day name or .weekday() function.
     * Example
    <pre><code>
    Date.today().is().friday(); // true|false
    Date.today().is().fri();
    Date.today().is().march();
    Date.today().is().mar();
    </code></pre>
     *  
     * @return {Boolean}    true|false
     */
    $P.is = function () { 
        this._is = true; 
        return this; 
    };

    /** 
     * Determines if two date objects occur on/in exactly the same instance of the subsequent date part function.
     * The function .same() must be followed by a date part function (example: .day(), .month(), .year(), etc).
     *
     * An optional Date can be passed in the date part function. If now date is passed as a parameter, 'Now' is used. 
     *
     * The following example demonstrates how to determine if two dates fall on the exact same day.
     *
     * Example
    <pre><code>
    var d1 = Date.today(); // today at 00:00
    var d2 = new Date();   // exactly now.

    // Do they occur on the same day?
    d1.same().day(d2); // true
    
     // Do they occur on the same hour?
    d1.same().hour(d2); // false, unless d2 hour is '00' (midnight).
    
    // What if it's the same day, but one year apart?
    var nextYear = Date.today().add(1).year();

    d1.same().day(nextYear); // false, because the dates must occur on the exact same day. 
    </code></pre>
     *
     * Scenario: Determine if a given date occurs during some week period 2 months from now. 
     *
     * Example
    <pre><code>
    var future = Date.today().add(2).months();
    return someDate.same().week(future); // true|false;
    </code></pre>
     *  
     * @return {Boolean}    true|false
     */    
    $P.same = function () { 
        this._same = true;
        this._isSecond = false;
        return this; 
    };

    /** 
     * Determines if the current date/time occurs during Today. Must be preceded by the .is() function.
     * Example
    <pre><code>
    someDate.is().today();    // true|false
    new Date().is().today();  // true
    Date.today().is().today();// true
    Date.today().add(-1).day().is().today(); // false
    </code></pre>
     *  
     * @return {Boolean}    true|false
     */    
    $P.today = function () {
        return this.same().day();
    };

    /** 
     * Determines if the current date is a weekday. This function must be preceded by the .is() function.
     * Example
    <pre><code>
    Date.today().is().weekday(); // true|false
    </code></pre>
     *  
     * @return {Boolean}    true|false
     */
    $P.weekday = function () {
        if (this._is) { 
            this._is = false;
            return (!this.is().sat() && !this.is().sun());
        }
        return false;
    };

    /** 
     * Sets the Time of the current Date instance. A string "6:15 pm" or config object {hour:18, minute:15} are accepted.
     * Example
    <pre><code>
    // Set time to 6:15pm with a String
    Date.today().at("6:15pm");

    // Set time to 6:15pm with a config object
    Date.today().at({hour:18, minute:15});
    </code></pre>
     *  
     * @return {Date}    date
     */
    $P.at = function (time) {
        return (typeof time === "string") ? $D.parse(this.toString("d") + " " + time) : this.set(time);
    }; 
        
    /** 
     * Creates a new Date() and adds this (Number) to the date based on the preceding date element function (eg. second|minute|hour|day|month|year).
     * Example
    <pre><code>
    // Undeclared Numbers must be wrapped with parentheses. Requirment of JavaScript.
    (3).days().fromNow();
    (6).months().fromNow();

    // Declared Number variables do not require parentheses. 
    var n = 6;
    n.months().fromNow();
    </code></pre>
     *  
     * @return {Date}    A new Date instance
     */
    $N.fromNow = $N.after = function (date) {
        var c = {};
        c[this._dateElement] = this;
        return ((!date) ? new Date() : date.clone()).add(c);
    };

    /** 
     * Creates a new Date() and subtract this (Number) from the date based on the preceding date element function (eg. second|minute|hour|day|month|year).
     * Example
    <pre><code>
    // Undeclared Numbers must be wrapped with parentheses. Requirment of JavaScript.
    (3).days().ago();
    (6).months().ago();

    // Declared Number variables do not require parentheses. 
    var n = 6;
    n.months().ago();
    </code></pre>
     *  
     * @return {Date}    A new Date instance
     */
    $N.ago = $N.before = function (date) {
        var c = {};
        c[this._dateElement] = this * -1;
        return ((!date) ? new Date() : date.clone()).add(c);
    };

    // Do NOT modify the following string tokens. These tokens are used to build dynamic functions.
    // All culture-specific strings can be found in the CultureInfo files. See /trunk/src/globalization/.
    var dx = ("sunday monday tuesday wednesday thursday friday saturday").split(/\s/),
        mx = ("january february march april may june july august september october november december").split(/\s/),
        px = ("Millisecond Second Minute Hour Day Week Month Year").split(/\s/),
        pxf = ("Milliseconds Seconds Minutes Hours Date Week Month FullYear").split(/\s/),
                nth = ("final first second third fourth fifth").split(/\s/),
        de;

   /** 
     * Returns an object literal of all the date parts.
     * Example
    <pre><code>
        var o = new Date().toObject();
        
        // { year: 2008, month: 4, week: 20, day: 13, hour: 18, minute: 9, second: 32, millisecond: 812 }
        
        // The object properties can be referenced directly from the object.
        
        alert(o.day);  // alerts "13"
        alert(o.year); // alerts "2008"
    </code></pre>
     *  
     * @return {Date}    An object literal representing the original date object.
     */
    $P.toObject = function () {
        var o = {};
        for (var i = 0; i < px.length; i++) {
            o[px[i].toLowerCase()] = this["get" + pxf[i]]();
        }
        return o;
    }; 
   
   /** 
     * Returns a date created from an object literal. Ignores the .week property if set in the config. 
     * Example
    <pre><code>
        var o = new Date().toObject();
        
        return Date.fromObject(o); // will return the same date. 

    var o2 = {month: 1, day: 20, hour: 18}; // birthday party!
    Date.fromObject(o2);
    </code></pre>
     *  
     * @return {Date}    An object literal representing the original date object.
     */    
    $D.fromObject = function(config) {
        config.week = null;
        return Date.today().set(config);
    };
        
    // Create day name functions and abbreviated day name functions (eg. monday(), friday(), fri()).
    var df = function (n) {
        return function () { 
            if (this._is) { 
                this._is = false; 
                return this.getDay() == n; 
            }
            if (this._nth !== null) {
                // If the .second() function was called earlier, remove the _orient 
                // from the date, and then continue.
                // This is required because 'second' can be used in two different context.
                // 
                // Example
                //
                //   Date.today().add(1).second();
                //   Date.march().second().monday();
                // 
                // Things get crazy with the following...
                //   Date.march().add(1).second().second().monday(); // but it works!!
                //  
                if (this._isSecond) {
                    this.addSeconds(this._orient * -1);
                }
                // make sure we reset _isSecond
                this._isSecond = false;

                var ntemp = this._nth;
                this._nth = null;
                var temp = this.clone().moveToLastDayOfMonth();
                this.moveToNthOccurrence(n, ntemp);
                if (this > temp) {
                    throw new RangeError($D.getDayName(n) + " does not occur " + ntemp + " times in the month of " + $D.getMonthName(temp.getMonth()) + " " + temp.getFullYear() + ".");
                }
                return this;
            }                   
            return this.moveToDayOfWeek(n, this._orient);
        };
    };
    
    var sdf = function (n) {
        return function () {
            var t = $D.today(), shift = n - t.getDay();
            if (n === 0 && $C.firstDayOfWeek === 1 && t.getDay() !== 0) {
                shift = shift + 7;
            }
            return t.addDays(shift);
        };
    };
        
    for (var i = 0; i < dx.length; i++) {
        // Create constant static Day Name variables. Example: Date.MONDAY or Date.MON
        $D[dx[i].toUpperCase()] = $D[dx[i].toUpperCase().substring(0, 3)] = i;

        // Create Day Name functions. Example: Date.monday() or Date.mon()
        $D[dx[i]] = $D[dx[i].substring(0, 3)] = sdf(i);

        // Create Day Name instance functions. Example: Date.today().next().monday()
        $P[dx[i]] = $P[dx[i].substring(0, 3)] = df(i);
    }
    
    // Create month name functions and abbreviated month name functions (eg. january(), march(), mar()).
    var mf = function (n) { 
        return function () {
            if (this._is) { 
                this._is = false; 
                return this.getMonth() === n; 
            }
            return this.moveToMonth(n, this._orient); 
        };
    };
    
    var smf = function (n) {
        return function () {
            return $D.today().set({ month: n, day: 1 });
        };
    };
    
    for (var j = 0; j < mx.length; j++) {
        // Create constant static Month Name variables. Example: Date.MARCH or Date.MAR
        $D[mx[j].toUpperCase()] = $D[mx[j].toUpperCase().substring(0, 3)] = j;

        // Create Month Name functions. Example: Date.march() or Date.mar()
        $D[mx[j]] = $D[mx[j].substring(0, 3)] = smf(j);

        // Create Month Name instance functions. Example: Date.today().next().march()
        $P[mx[j]] = $P[mx[j].substring(0, 3)] = mf(j);
    }
    
    // Create date element functions and plural date element functions used with Date (eg. day(), days(), months()).
    var ef = function (j) {
        return function () {
            // if the .second() function was called earlier, the _orient 
            // has alread been added. Just return this and reset _isSecond.
            if (this._isSecond) {
                this._isSecond = false;
                return this;
            }

            if (this._same) {
                this._same = this._is = false; 
                var o1 = this.toObject(),
                    o2 = (arguments[0] || new Date()).toObject(),
                    v = "",
                    k = j.toLowerCase();
                    
                for (var m = (px.length - 1); m > -1; m--) {
                    v = px[m].toLowerCase();
                    if (o1[v] != o2[v]) {
                        return false;
                    }
                    if (k == v) {
                        break;
                    }
                }
                return true;
            }
            
            if (j.substring(j.length - 1) != "s") {
                j += "s"; 
            }
            return this["add" + j](this._orient);
        };
    };
    
    
    var nf = function (n) {
        return function () {
            this._dateElement = n;
            return this;
        };
    };
   
    for (var k = 0; k < px.length; k++) {
        de = px[k].toLowerCase();
    
        // Create date element functions and plural date element functions used with Date (eg. day(), days(), months()).
        $P[de] = $P[de + "s"] = ef(px[k]);
        
        // Create date element functions and plural date element functions used with Number (eg. day(), days(), months()).
        $N[de] = $N[de + "s"] = nf(de);
    }
    
    $P._ss = ef("Second");
        
    var nthfn = function (n) {
        return function (dayOfWeek) {
            if (this._same) {
                return this._ss(arguments[0]);
            }
            if (dayOfWeek || dayOfWeek === 0) {
                return this.moveToNthOccurrence(dayOfWeek, n);
            }
            this._nth = n;

            // if the operator is 'second' add the _orient, then deal with it later...
            if (n === 2 && (dayOfWeek === undefined || dayOfWeek === null)) {
                this._isSecond = true;
                return this.addSeconds(this._orient);
            }
            return this;
        };
    };

    for (var l = 0; l < nth.length; l++) {
        $P[nth[l]] = (l === 0) ? nthfn(-1) : nthfn(l);
    }
}());

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// StreamView
// ----------

// Shows stream items for selected user/group
define('views/streamview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/streamitemview', 'date'],
function ($, _, Backbone, app, ich, StreamItemView) {

    var StreamView = Backbone.Marionette.CompositeView.extend({
        template: 'Stream',

        itemView: StreamItemView,

        className: 'stream',

        events: {
            'click #stream-load-more-button': 'onLoadMoreClicked',
            'click #stream-load-new-button': 'onLoadNewClicked'
        },

        initialize: function (options) {
            _.bindAll(this, 'onNewStreamItemReceived', 'appendHtml');

            this.newItemsCount = 0;
            this.isHomeStream = options.isHomeStream && options.isHomeStream === true ? true : false;

            this.collection.on('fetching', this.onStreamLoadingStart, this);
            this.collection.on('fetched', this.onStreamLoadingComplete, this);

            this.newStreamItemsCache = [];

            app.vent.on('newactivity:observationadded newactivity:postadded newactivity:observationnoteadded', this.onNewStreamItemReceived);
        },

        showBootstrappedDetails: function () {
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);
            log(index);

            var $li = collectionView.$el.find('.stream-list > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.stream-list').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }
        },

        onLoadMoreClicked: function () {
            this.$el.find('.stream-load-more').remove();
            this.collection.fetchNextPage();
        },

        onLoadNewClicked: function () {
            this.$el.find('.stream-load-new').remove();
            this.collection.add(this.newStreamItemsCache);
            this.newStreamItemsCache = [];
            this.newItemsCount = 0;
        },

        onStreamLoadingStart: function (collection) {
            this.$el.append(ich.StreamMessage({ Text: 'Loading', ShowLoader: true }));
        },

        onStreamLoadingComplete: function (collection) {
            this.$el.find('.stream-message').remove();
            if (collection.length === 0) {
                this.$el.append(ich.StreamMessage({ Text: 'No activity yet! Start now by adding an observation.', ShowLoader: false }));
            }
            if (collection.pageInfo().next) {
                this.$el.append(ich.StreamLoadMore());
            }
        },

        onNewStreamItemReceived: function (streamItem) {
            if (_.any(this.newStreamItemsCache, function (item) { return item.id === streamItem.id; }, this)) {
                return;
            }
            this.$el.find('.stream-message').remove();
            var streamItemCreatedDateTime = Date.parseExact(streamItem.get('CreatedDateTime'), 'yyyy-MM-ddTHH:mm:ssZ');

            log('streamItemCreatedDateTime', streamItemCreatedDateTime);
            log('baselineDateTime', this.collection.baselineDateTime);

            // Only show a new items message if the item is newer than what we have already
            if (streamItemCreatedDateTime.isAfter(this.collection.baselineDateTime)) {
                log('is after!');
                this.newItemsCount++;
                this.newStreamItemsCache.push(streamItem);
            }

            if (this.newItemsCount > 0) {
                // Show load new items button
                if (this.$el.find('.stream-load-new').length === 0) {
                    this.$el.prepend(ich.StreamLoadNew());
                } else {
                    this.$el.find('#stream-load-new-button').val('Load ' + this.newItemsCount + ' New Items');
                }
            }
        }
    });

    return StreamView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectLayoutView
// -----------------

define('views/projectlayoutview',['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/activitycollection'], function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var ProjectLayoutView = Backbone.Marionette.Layout.extend({
        className: 'project',

        template: 'Project',

        regions: {
            details: '.details'
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    ObservationCountDescription: this.model.get('ObservationCount') === 1 ? 'Observation' : 'Observations',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts'
                }
            };
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .project');
        },

        showStream: function () {
            var activityCollection = new ActivityCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: activityCollection
            };
            if (app.isPrerendering('projects')) {
                options['el'] = '.stream';
            }
            var streamView = new StreamView(options);
            if (app.isPrerendering('projects')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }
            activityCollection.fetchFirstPage();
        }
    });

    return ProjectLayoutView;

}); 
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// GroupUserController & GroupUserRouter
// -------------------------------------

// This is the controller for groups/users. It contains all of the 
// high level knowledge of how to run the app when it's in group/user mode.
define('controllers/groupusercontroller',['jquery', 'underscore', 'backbone', 'app', 'views/projectlayoutview', 'models/project', 'collections/activitycollection'],
function ($, _, Backbone, app, ProjectLayoutView, Project, ActivityCollection) 
{
    var GroupUserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            //'teams/:id': 'showTeam',
            //'projects/:id': 'showProjectStream',
            //'projects/:id/about': 'showProjectAbout',
            //'projects/:id/members': 'showProjectMembers',
            'users/:id': 'showUser'
        }
    });

    var GroupUserController = {};

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
//            if (id) {
//                params['id'] = id;
//            }
            $.ajax({
                url: '/projects/' + id,
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }

        return deferred.promise();
    };

    // GroupUserController Public API
    // ------------------------------

    // Show a team
    GroupUserController.showTeam = function () {
        log('team:show');
    };

    // Show project activity
    GroupUserController.showProjectStream = function (id) {
        log('showing projects home stream', this, this);

        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                var projectLayoutView = new ProjectLayoutView({ model: project });

                app.content[app.getShowViewMethodName('projects')](projectLayoutView);

                if (app.isPrerendering('projects')) {
                    projectLayoutView.showBootstrappedDetails();
                }

                projectLayoutView.showStream();

                app.setPrerenderComplete();
            });
    };

    // Show project observations
    GroupUserController.showProjectObservations = function (id) {
        log('project:show:observations');
        var projectLayoutView = showProjectLayoutView(id);
        //projectLayout.details.showObservations();
    };

    // Show project about
    GroupUserController.showProjectAbout = function (id) {
        log('project:show:about');
        var projectLayoutView = showProjectLayoutView(id);
        //projectLayout.details.showAbout();
    };

    // Show project members
    GroupUserController.showProjectMembers = function (id) {
        log('project:show:members');
        var projectLayoutView = showProjectLayoutView(id);
        //projectLayout.details.showMembers();
    };

    // Show a user
    GroupUserController.showUser = function () {
        log('user:show');
    };

    //    // Show a list of email for the given category.
    //    MailApp.showCategory = function (category) {
    //        showFilteredEmailList(category);
    //        MailApp.Categories.showCategoryList();
    //    };

    //    // Show an individual email message, by Id
    //    MailApp.showMessage = function (messageId) {
    //        MailApp.emailList.onReset(function (list) {
    //            var email = list.get(messageId);
    //            MailApp.MailBox.showMessage(email);
    //        });
    //        MailApp.Categories.showCategoryList();
    //    };

    // GroupUserController Event Handlers
    // ----------------------------------

    app.vent.on('home:show', function (id) {
        //GroupUserController.showHome(id);
    });

    app.vent.on('project:show:stream', function (id) {
        //GroupUserController.showProjectStream(id);
    });

    app.vent.on('project:show:observations', function (id) {
        //GroupUserController.showProjectObservations(id);
    });

    //    // When the mail app is shown or `inbox` is clicked,
    //    // show all the mail.
    //    BBCloneMail.vent.bind("mail:show", function () {
    //        showFilteredEmailList();
    //    });

    // GroupUserController Initializer
    // -------------------------------

    // Initializes the email collection object with the list
    // of emails that are passed in from the call to 
    // `BBCloneMail.start`.
    //    app.addInitializer(function (options) {
    //        app.groupUserController = new GroupUserController();
    //        //MailApp.emailList.fetch();
    //    });

    app.addInitializer(function () {
        this.groupUserRouter = new GroupUserRouter({
            controller: GroupUserController
        });
    });

    return GroupUserController;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePublicLayoutView
// --------------------

// The home page view when not logged in
define('views/homepubliclayoutview',['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var HomePublicLayoutView = Backbone.Marionette.Layout.extend({
        className: 'home',

        template: 'Home',

//        regions: {
//            summary: '.summary',
//            details: '.details'
//        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .home');
        }

//        showStream: function () {
        //            var activityCollection = new ActivityCollection();
//            var options = {
//                model: app.authenticatedUser.user,
        //                collection: activityCollection
//            };

//            if (app.isPrerendering('home')) {
//                options['el'] = '.stream';
//            }

//            var streamView = new StreamView(options);

//            if (app.isPrerendering('home')) {
//                this.details.attachView(streamView);
//                streamView.showBootstrappedDetails();
//            } else {
//                this.details.show(streamView);
//            }

        //            activityCollection.fetchFirstPage();
//        }
    });

    return HomePublicLayoutView;

}); 
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePrivateLayoutView
// ---------------------

// The home page view when logged in
define('views/homeprivatelayoutview',['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/activitycollection'], function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var HomePrivateLayoutView = Backbone.Marionette.Layout.extend({
        className: 'home',

        template: 'Home',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            this.showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .home');
            this.showDetails();
        },

        showDetails: function () {
            var that = this;
            this.$el.find('.close-intro').on('click', function (e) {
                e.preventDefault();
                $('.intro').slideUp('fast', function () {
                    that.$el.find('.intro').remove();
                });
                // TODO: Save intro closed status
                return false;
            });
        },

        showStream: function () {
            var activityCollection = new ActivityCollection();
            var options = {
                model: app.authenticatedUser.user,
                collection: activityCollection,
                isHomeStream: true
            };

            if (app.isPrerendering('home')) {
                options['el'] = '.stream';
            }

            var streamView = new StreamView(options);

            if (app.isPrerendering('home')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }

            activityCollection.fetchFirstPage();
        }
    });

    return HomePrivateLayoutView;

}); 
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeController & HomeRouter
// ---------------------------
define('controllers/homecontroller',['jquery', 'underscore', 'backbone', 'app', 'views/homepubliclayoutview', 'views/homeprivatelayoutview'],
function ($, _, Backbone, app, HomePublicLayoutView, HomePrivateLayoutView) {
    var HomeRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showHomeStream'
        }
    });

    var HomeController = {};

    // Public API
    // ----------

    HomeController.showHomeStream = function () {
        app.updateTitle('');
        var homeLayoutView = null;

        if (app.authenticatedUser) {
            var homeLayoutView = new HomePrivateLayoutView({ model: app.authenticatedUser.user });
        } else {
            var homeLayoutView = new HomePublicLayoutView();
        }

        app.content[app.getShowViewMethodName('home')](homeLayoutView);

        if (app.isPrerendering('home')) {
            homeLayoutView.showBootstrappedDetails();
        }

        if (app.authenticatedUser) {
            homeLayoutView.showStream();
        }

        app.setPrerenderComplete();
    };

    app.addInitializer(function () {
        this.homeRouter = new HomeRouter({
            controller: HomeController
        });
    });

    return HomeController;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationDetailsView
// ----------------------

define('views/observationdetailsview',['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var ObservationDetailsView = Backbone.Marionette.ItemView.extend({
        className: 'observation-details',

        template: 'ObservationDetails',

        serializeData: function () {
            var json = { Model: { Observation: this.model.toViewJSON() } };
            json.Model.ShowThumbnails = this.model.get('Media').length > 1 ? true : false;
            return json;
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {

        }
    });

    return ObservationDetailsView;

});
/** @license
 * RequireJS plugin for async dependency load like JSONP and Google Maps
 * Author: Miller Medeiros
 * Version: 0.1.1 (2011/11/17)
 * Released under the MIT license
 */
define('async',[],function(){

    var DEFAULT_PARAM_NAME = 'callback',
        _uid = 0;

    function injectScript(src){
        var s, t;
        s = document.createElement('script'); s.type = 'text/javascript'; s.async = true; s.src = src;
        t = document.getElementsByTagName('script')[0]; t.parentNode.insertBefore(s,t);
    }

    function formatUrl(name, id){
        var paramRegex = /!(.+)/,
            url = name.replace(paramRegex, ''),
            param = (paramRegex.test(name))? name.replace(/.+!/, '') : DEFAULT_PARAM_NAME;
        url += (url.indexOf('?') < 0)? '?' : '&';
        return url + param +'='+ id;
    }

    function uid() {
        _uid += 1;
        return '__async_req_'+ _uid +'__';
    }

    return{
        load : function(name, req, onLoad, config){
            if(config.isBuild){
                onLoad(null); //avoid errors on the optimizer
            }else{
                var id = uid();
                window[id] = onLoad; //create a global variable that stores onLoad so callback function can define new module after async load
                injectScript(formatUrl(name, id));
            }
        }
    };
});

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// DummyOverlayView
// ----------------

// Google maps dummy overlay to allow drag and drop of pin
define('views/dummyoverlayview',['jquery', 'underscore', 'backbone', 'app', 'async!http://maps.google.com/maps/api/js?sensor=false&region=AU'], function ($, _, Backbone, app) {

    var DummyOverlayView = function (map) {
        // Bind this to the map to access MapCanvasProjection
        this.setMap(map);
        // MapCanvasProjection is only available after draw has been called.
        this.draw = function () { };
    };

    DummyOverlayView.prototype = new google.maps.OverlayView();

    return DummyOverlayView;

});
define('jqueryui/core', ['jquery'], function (jQuery) {
/*!
 * jQuery UI 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI
 */
(function( $, undefined ) {

// prevent duplicate loading
// this is only a problem because we proxy existing functions
// and we don't want to double proxy them
$.ui = $.ui || {};
if ( $.ui.version ) {
	return;
}

$.extend( $.ui, {
	version: "1.8.14",

	keyCode: {
		ALT: 18,
		BACKSPACE: 8,
		CAPS_LOCK: 20,
		COMMA: 188,
		COMMAND: 91,
		COMMAND_LEFT: 91, // COMMAND
		COMMAND_RIGHT: 93,
		CONTROL: 17,
		DELETE: 46,
		DOWN: 40,
		END: 35,
		ENTER: 13,
		ESCAPE: 27,
		HOME: 36,
		INSERT: 45,
		LEFT: 37,
		MENU: 93, // COMMAND_RIGHT
		NUMPAD_ADD: 107,
		NUMPAD_DECIMAL: 110,
		NUMPAD_DIVIDE: 111,
		NUMPAD_ENTER: 108,
		NUMPAD_MULTIPLY: 106,
		NUMPAD_SUBTRACT: 109,
		PAGE_DOWN: 34,
		PAGE_UP: 33,
		PERIOD: 190,
		RIGHT: 39,
		SHIFT: 16,
		SPACE: 32,
		TAB: 9,
		UP: 38,
		WINDOWS: 91 // COMMAND
	}
});

// plugins
$.fn.extend({
	_focus: $.fn.focus,
	focus: function( delay, fn ) {
		return typeof delay === "number" ?
			this.each(function() {
				var elem = this;
				setTimeout(function() {
					$( elem ).focus();
					if ( fn ) {
						fn.call( elem );
					}
				}, delay );
			}) :
			this._focus.apply( this, arguments );
	},

	scrollParent: function() {
		var scrollParent;
		if (($.browser.msie && (/(static|relative)/).test(this.css('position'))) || (/absolute/).test(this.css('position'))) {
			scrollParent = this.parents().filter(function() {
				return (/(relative|absolute|fixed)/).test($.curCSS(this,'position',1)) && (/(auto|scroll)/).test($.curCSS(this,'overflow',1)+$.curCSS(this,'overflow-y',1)+$.curCSS(this,'overflow-x',1));
			}).eq(0);
		} else {
			scrollParent = this.parents().filter(function() {
				return (/(auto|scroll)/).test($.curCSS(this,'overflow',1)+$.curCSS(this,'overflow-y',1)+$.curCSS(this,'overflow-x',1));
			}).eq(0);
		}

		return (/fixed/).test(this.css('position')) || !scrollParent.length ? $(document) : scrollParent;
	},

	zIndex: function( zIndex ) {
		if ( zIndex !== undefined ) {
			return this.css( "zIndex", zIndex );
		}

		if ( this.length ) {
			var elem = $( this[ 0 ] ), position, value;
			while ( elem.length && elem[ 0 ] !== document ) {
				// Ignore z-index if position is set to a value where z-index is ignored by the browser
				// This makes behavior of this function consistent across browsers
				// WebKit always returns auto if the element is positioned
				position = elem.css( "position" );
				if ( position === "absolute" || position === "relative" || position === "fixed" ) {
					// IE returns 0 when zIndex is not specified
					// other browsers return a string
					// we ignore the case of nested elements with an explicit value of 0
					// <div style="z-index: -10;"><div style="z-index: 0;"></div></div>
					value = parseInt( elem.css( "zIndex" ), 10 );
					if ( !isNaN( value ) && value !== 0 ) {
						return value;
					}
				}
				elem = elem.parent();
			}
		}

		return 0;
	},

	disableSelection: function() {
		return this.bind( ( $.support.selectstart ? "selectstart" : "mousedown" ) +
			".ui-disableSelection", function( event ) {
				event.preventDefault();
			});
	},

	enableSelection: function() {
		return this.unbind( ".ui-disableSelection" );
	}
});

$.each( [ "Width", "Height" ], function( i, name ) {
	var side = name === "Width" ? [ "Left", "Right" ] : [ "Top", "Bottom" ],
		type = name.toLowerCase(),
		orig = {
			innerWidth: $.fn.innerWidth,
			innerHeight: $.fn.innerHeight,
			outerWidth: $.fn.outerWidth,
			outerHeight: $.fn.outerHeight
		};

	function reduce( elem, size, border, margin ) {
		$.each( side, function() {
			size -= parseFloat( $.curCSS( elem, "padding" + this, true) ) || 0;
			if ( border ) {
				size -= parseFloat( $.curCSS( elem, "border" + this + "Width", true) ) || 0;
			}
			if ( margin ) {
				size -= parseFloat( $.curCSS( elem, "margin" + this, true) ) || 0;
			}
		});
		return size;
	}

	$.fn[ "inner" + name ] = function( size ) {
		if ( size === undefined ) {
			return orig[ "inner" + name ].call( this );
		}

		return this.each(function() {
			$( this ).css( type, reduce( this, size ) + "px" );
		});
	};

	$.fn[ "outer" + name] = function( size, margin ) {
		if ( typeof size !== "number" ) {
			return orig[ "outer" + name ].call( this, size );
		}

		return this.each(function() {
			$( this).css( type, reduce( this, size, true, margin ) + "px" );
		});
	};
});

// selectors
function focusable( element, isTabIndexNotNaN ) {
	var nodeName = element.nodeName.toLowerCase();
	if ( "area" === nodeName ) {
		var map = element.parentNode,
			mapName = map.name,
			img;
		if ( !element.href || !mapName || map.nodeName.toLowerCase() !== "map" ) {
			return false;
		}
		img = $( "img[usemap=#" + mapName + "]" )[0];
		return !!img && visible( img );
	}
	return ( /input|select|textarea|button|object/.test( nodeName )
		? !element.disabled
		: "a" == nodeName
			? element.href || isTabIndexNotNaN
			: isTabIndexNotNaN)
		// the element and all of its ancestors must be visible
		&& visible( element );
}

function visible( element ) {
	return !$( element ).parents().andSelf().filter(function() {
		return $.curCSS( this, "visibility" ) === "hidden" ||
			$.expr.filters.hidden( this );
	}).length;
}

$.extend( $.expr[ ":" ], {
	data: function( elem, i, match ) {
		return !!$.data( elem, match[ 3 ] );
	},

	focusable: function( element ) {
		return focusable( element, !isNaN( $.attr( element, "tabindex" ) ) );
	},

	tabbable: function( element ) {
		var tabIndex = $.attr( element, "tabindex" ),
			isTabIndexNaN = isNaN( tabIndex );
		return ( isTabIndexNaN || tabIndex >= 0 ) && focusable( element, !isTabIndexNaN );
	}
});

// support
$(function() {
	var body = document.body,
		div = body.appendChild( div = document.createElement( "div" ) );

	$.extend( div.style, {
		minHeight: "100px",
		height: "auto",
		padding: 0,
		borderWidth: 0
	});

	$.support.minHeight = div.offsetHeight === 100;
	$.support.selectstart = "onselectstart" in div;

	// set display to none to avoid a layout bug in IE
	// http://dev.jquery.com/ticket/4014
	body.removeChild( div ).style.display = "none";
});





// deprecated
$.extend( $.ui, {
	// $.ui.plugin is deprecated.  Use the proxy pattern instead.
	plugin: {
		add: function( module, option, set ) {
			var proto = $.ui[ module ].prototype;
			for ( var i in set ) {
				proto.plugins[ i ] = proto.plugins[ i ] || [];
				proto.plugins[ i ].push( [ option, set[ i ] ] );
			}
		},
		call: function( instance, name, args ) {
			var set = instance.plugins[ name ];
			if ( !set || !instance.element[ 0 ].parentNode ) {
				return;
			}
	
			for ( var i = 0; i < set.length; i++ ) {
				if ( instance.options[ set[ i ][ 0 ] ] ) {
					set[ i ][ 1 ].apply( instance.element, args );
				}
			}
		}
	},
	
	// will be deprecated when we switch to jQuery 1.4 - use jQuery.contains()
	contains: function( a, b ) {
		return document.compareDocumentPosition ?
			a.compareDocumentPosition( b ) & 16 :
			a !== b && a.contains( b );
	},
	
	// only used by resizable
	hasScroll: function( el, a ) {
	
		//If overflow is hidden, the element might have extra content, but the user wants to hide it
		if ( $( el ).css( "overflow" ) === "hidden") {
			return false;
		}
	
		var scroll = ( a && a === "left" ) ? "scrollLeft" : "scrollTop",
			has = false;
	
		if ( el[ scroll ] > 0 ) {
			return true;
		}
	
		// TODO: determine which cases actually cause this to happen
		// if the element doesn't have the scroll set, see if it's possible to
		// set the scroll
		el[ scroll ] = 1;
		has = ( el[ scroll ] > 0 );
		el[ scroll ] = 0;
		return has;
	},
	
	// these are odd functions, fix the API or move into individual plugins
	isOverAxis: function( x, reference, size ) {
		//Determines when x coordinate is over "b" element axis
		return ( x > reference ) && ( x < ( reference + size ) );
	},
	isOver: function( y, x, top, left, height, width ) {
		//Determines when x, y coordinates is over "b" element
		return $.ui.isOverAxis( y, top, height ) && $.ui.isOverAxis( x, left, width );
	}
});

})( jQuery );

});
define('jqueryui/position', ['jquery'], function (jQuery) {
/*
 * jQuery UI Position 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Position
 */
(function( $, undefined ) {

$.ui = $.ui || {};

var horizontalPositions = /left|center|right/,
	verticalPositions = /top|center|bottom/,
	center = "center",
	_position = $.fn.position,
	_offset = $.fn.offset;

$.fn.position = function( options ) {
	if ( !options || !options.of ) {
		return _position.apply( this, arguments );
	}

	// make a copy, we don't want to modify arguments
	options = $.extend( {}, options );

	var target = $( options.of ),
		targetElem = target[0],
		collision = ( options.collision || "flip" ).split( " " ),
		offset = options.offset ? options.offset.split( " " ) : [ 0, 0 ],
		targetWidth,
		targetHeight,
		basePosition;

	if ( targetElem.nodeType === 9 ) {
		targetWidth = target.width();
		targetHeight = target.height();
		basePosition = { top: 0, left: 0 };
	// TODO: use $.isWindow() in 1.9
	} else if ( targetElem.setTimeout ) {
		targetWidth = target.width();
		targetHeight = target.height();
		basePosition = { top: target.scrollTop(), left: target.scrollLeft() };
	} else if ( targetElem.preventDefault ) {
		// force left top to allow flipping
		options.at = "left top";
		targetWidth = targetHeight = 0;
		basePosition = { top: options.of.pageY, left: options.of.pageX };
	} else {
		targetWidth = target.outerWidth();
		targetHeight = target.outerHeight();
		basePosition = target.offset();
	}

	// force my and at to have valid horizontal and veritcal positions
	// if a value is missing or invalid, it will be converted to center 
	$.each( [ "my", "at" ], function() {
		var pos = ( options[this] || "" ).split( " " );
		if ( pos.length === 1) {
			pos = horizontalPositions.test( pos[0] ) ?
				pos.concat( [center] ) :
				verticalPositions.test( pos[0] ) ?
					[ center ].concat( pos ) :
					[ center, center ];
		}
		pos[ 0 ] = horizontalPositions.test( pos[0] ) ? pos[ 0 ] : center;
		pos[ 1 ] = verticalPositions.test( pos[1] ) ? pos[ 1 ] : center;
		options[ this ] = pos;
	});

	// normalize collision option
	if ( collision.length === 1 ) {
		collision[ 1 ] = collision[ 0 ];
	}

	// normalize offset option
	offset[ 0 ] = parseInt( offset[0], 10 ) || 0;
	if ( offset.length === 1 ) {
		offset[ 1 ] = offset[ 0 ];
	}
	offset[ 1 ] = parseInt( offset[1], 10 ) || 0;

	if ( options.at[0] === "right" ) {
		basePosition.left += targetWidth;
	} else if ( options.at[0] === center ) {
		basePosition.left += targetWidth / 2;
	}

	if ( options.at[1] === "bottom" ) {
		basePosition.top += targetHeight;
	} else if ( options.at[1] === center ) {
		basePosition.top += targetHeight / 2;
	}

	basePosition.left += offset[ 0 ];
	basePosition.top += offset[ 1 ];

	return this.each(function() {
		var elem = $( this ),
			elemWidth = elem.outerWidth(),
			elemHeight = elem.outerHeight(),
			marginLeft = parseInt( $.curCSS( this, "marginLeft", true ) ) || 0,
			marginTop = parseInt( $.curCSS( this, "marginTop", true ) ) || 0,
			collisionWidth = elemWidth + marginLeft +
				( parseInt( $.curCSS( this, "marginRight", true ) ) || 0 ),
			collisionHeight = elemHeight + marginTop +
				( parseInt( $.curCSS( this, "marginBottom", true ) ) || 0 ),
			position = $.extend( {}, basePosition ),
			collisionPosition;

		if ( options.my[0] === "right" ) {
			position.left -= elemWidth;
		} else if ( options.my[0] === center ) {
			position.left -= elemWidth / 2;
		}

		if ( options.my[1] === "bottom" ) {
			position.top -= elemHeight;
		} else if ( options.my[1] === center ) {
			position.top -= elemHeight / 2;
		}

		// prevent fractions (see #5280)
		position.left = Math.round( position.left );
		position.top = Math.round( position.top );

		collisionPosition = {
			left: position.left - marginLeft,
			top: position.top - marginTop
		};

		$.each( [ "left", "top" ], function( i, dir ) {
			if ( $.ui.position[ collision[i] ] ) {
				$.ui.position[ collision[i] ][ dir ]( position, {
					targetWidth: targetWidth,
					targetHeight: targetHeight,
					elemWidth: elemWidth,
					elemHeight: elemHeight,
					collisionPosition: collisionPosition,
					collisionWidth: collisionWidth,
					collisionHeight: collisionHeight,
					offset: offset,
					my: options.my,
					at: options.at
				});
			}
		});

		if ( $.fn.bgiframe ) {
			elem.bgiframe();
		}
		elem.offset( $.extend( position, { using: options.using } ) );
	});
};

$.ui.position = {
	fit: {
		left: function( position, data ) {
			var win = $( window ),
				over = data.collisionPosition.left + data.collisionWidth - win.width() - win.scrollLeft();
			position.left = over > 0 ? position.left - over : Math.max( position.left - data.collisionPosition.left, position.left );
		},
		top: function( position, data ) {
			var win = $( window ),
				over = data.collisionPosition.top + data.collisionHeight - win.height() - win.scrollTop();
			position.top = over > 0 ? position.top - over : Math.max( position.top - data.collisionPosition.top, position.top );
		}
	},

	flip: {
		left: function( position, data ) {
			if ( data.at[0] === center ) {
				return;
			}
			var win = $( window ),
				over = data.collisionPosition.left + data.collisionWidth - win.width() - win.scrollLeft(),
				myOffset = data.my[ 0 ] === "left" ?
					-data.elemWidth :
					data.my[ 0 ] === "right" ?
						data.elemWidth :
						0,
				atOffset = data.at[ 0 ] === "left" ?
					data.targetWidth :
					-data.targetWidth,
				offset = -2 * data.offset[ 0 ];
			position.left += data.collisionPosition.left < 0 ?
				myOffset + atOffset + offset :
				over > 0 ?
					myOffset + atOffset + offset :
					0;
		},
		top: function( position, data ) {
			if ( data.at[1] === center ) {
				return;
			}
			var win = $( window ),
				over = data.collisionPosition.top + data.collisionHeight - win.height() - win.scrollTop(),
				myOffset = data.my[ 1 ] === "top" ?
					-data.elemHeight :
					data.my[ 1 ] === "bottom" ?
						data.elemHeight :
						0,
				atOffset = data.at[ 1 ] === "top" ?
					data.targetHeight :
					-data.targetHeight,
				offset = -2 * data.offset[ 1 ];
			position.top += data.collisionPosition.top < 0 ?
				myOffset + atOffset + offset :
				over > 0 ?
					myOffset + atOffset + offset :
					0;
		}
	}
};

// offset setter from jQuery 1.4
if ( !$.offset.setOffset ) {
	$.offset.setOffset = function( elem, options ) {
		// set position first, in-case top/left are set even on static elem
		if ( /static/.test( $.curCSS( elem, "position" ) ) ) {
			elem.style.position = "relative";
		}
		var curElem   = $( elem ),
			curOffset = curElem.offset(),
			curTop    = parseInt( $.curCSS( elem, "top",  true ), 10 ) || 0,
			curLeft   = parseInt( $.curCSS( elem, "left", true ), 10)  || 0,
			props     = {
				top:  (options.top  - curOffset.top)  + curTop,
				left: (options.left - curOffset.left) + curLeft
			};
		
		if ( 'using' in options ) {
			options.using.call( elem, props );
		} else {
			curElem.css( props );
		}
	};

	$.fn.offset = function( options ) {
		var elem = this[ 0 ];
		if ( !elem || !elem.ownerDocument ) { return null; }
		if ( options ) { 
			return this.each(function() {
				$.offset.setOffset( this, options );
			});
		}
		return _offset.call( this );
	};
}

}( jQuery ));

});
define('jqueryui/autocomplete', ['jquery','jqueryui/core','jqueryui/widget','jqueryui/position'], function (jQuery) {
/*
 * jQuery UI Autocomplete 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Autocomplete
 *
 * Depends:
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *	jquery.ui.position.js
 */
(function( $, undefined ) {

// used to prevent race conditions with remote data sources
var requestIndex = 0;

$.widget( "ui.autocomplete", {
	options: {
		appendTo: "body",
		autoFocus: false,
		delay: 300,
		minLength: 1,
		position: {
			my: "left top",
			at: "left bottom",
			collision: "none"
		},
		source: null
	},

	pending: 0,

	_create: function() {
		var self = this,
			doc = this.element[ 0 ].ownerDocument,
			suppressKeyPress;

		this.element
			.addClass( "ui-autocomplete-input" )
			.attr( "autocomplete", "off" )
			// TODO verify these actually work as intended
			.attr({
				role: "textbox",
				"aria-autocomplete": "list",
				"aria-haspopup": "true"
			})
			.bind( "keydown.autocomplete", function( event ) {
				if ( self.options.disabled || self.element.attr( "readonly" ) ) {
					return;
				}

				suppressKeyPress = false;
				var keyCode = $.ui.keyCode;
				switch( event.keyCode ) {
				case keyCode.PAGE_UP:
					self._move( "previousPage", event );
					break;
				case keyCode.PAGE_DOWN:
					self._move( "nextPage", event );
					break;
				case keyCode.UP:
					self._move( "previous", event );
					// prevent moving cursor to beginning of text field in some browsers
					event.preventDefault();
					break;
				case keyCode.DOWN:
					self._move( "next", event );
					// prevent moving cursor to end of text field in some browsers
					event.preventDefault();
					break;
				case keyCode.ENTER:
				case keyCode.NUMPAD_ENTER:
					// when menu is open and has focus
					if ( self.menu.active ) {
						// #6055 - Opera still allows the keypress to occur
						// which causes forms to submit
						suppressKeyPress = true;
						event.preventDefault();
					}
					//passthrough - ENTER and TAB both select the current element
				case keyCode.TAB:
					if ( !self.menu.active ) {
						return;
					}
					self.menu.select( event );
					break;
				case keyCode.ESCAPE:
					self.element.val( self.term );
					self.close( event );
					break;
				default:
					// keypress is triggered before the input value is changed
					clearTimeout( self.searching );
					self.searching = setTimeout(function() {
						// only search if the value has changed
						if ( self.term != self.element.val() ) {
							self.selectedItem = null;
							self.search( null, event );
						}
					}, self.options.delay );
					break;
				}
			})
			.bind( "keypress.autocomplete", function( event ) {
				if ( suppressKeyPress ) {
					suppressKeyPress = false;
					event.preventDefault();
				}
			})
			.bind( "focus.autocomplete", function() {
				if ( self.options.disabled ) {
					return;
				}

				self.selectedItem = null;
				self.previous = self.element.val();
			})
			.bind( "blur.autocomplete", function( event ) {
				if ( self.options.disabled ) {
					return;
				}

				clearTimeout( self.searching );
				// clicks on the menu (or a button to trigger a search) will cause a blur event
				self.closing = setTimeout(function() {
					self.close( event );
					self._change( event );
				}, 150 );
			});
		this._initSource();
		this.response = function() {
			return self._response.apply( self, arguments );
		};
		this.menu = $( "<ul></ul>" )
			.addClass( "ui-autocomplete" )
			.appendTo( $( this.options.appendTo || "body", doc )[0] )
			// prevent the close-on-blur in case of a "slow" click on the menu (long mousedown)
			.mousedown(function( event ) {
				// clicking on the scrollbar causes focus to shift to the body
				// but we can't detect a mouseup or a click immediately afterward
				// so we have to track the next mousedown and close the menu if
				// the user clicks somewhere outside of the autocomplete
				var menuElement = self.menu.element[ 0 ];
				if ( !$( event.target ).closest( ".ui-menu-item" ).length ) {
					setTimeout(function() {
						$( document ).one( 'mousedown', function( event ) {
							if ( event.target !== self.element[ 0 ] &&
								event.target !== menuElement &&
								!$.ui.contains( menuElement, event.target ) ) {
								self.close();
							}
						});
					}, 1 );
				}

				// use another timeout to make sure the blur-event-handler on the input was already triggered
				setTimeout(function() {
					clearTimeout( self.closing );
				}, 13);
			})
			.menu({
				focus: function( event, ui ) {
					var item = ui.item.data( "item.autocomplete" );
					if ( false !== self._trigger( "focus", event, { item: item } ) ) {
						// use value to match what will end up in the input, if it was a key event
						if ( /^key/.test(event.originalEvent.type) ) {
							self.element.val( item.value );
						}
					}
				},
				selected: function( event, ui ) {
					var item = ui.item.data( "item.autocomplete" ),
						previous = self.previous;

					// only trigger when focus was lost (click on menu)
					if ( self.element[0] !== doc.activeElement ) {
						self.element.focus();
						self.previous = previous;
						// #6109 - IE triggers two focus events and the second
						// is asynchronous, so we need to reset the previous
						// term synchronously and asynchronously :-(
						setTimeout(function() {
							self.previous = previous;
							self.selectedItem = item;
						}, 1);
					}

					if ( false !== self._trigger( "select", event, { item: item } ) ) {
						self.element.val( item.value );
					}
					// reset the term after the select event
					// this allows custom select handling to work properly
					self.term = self.element.val();

					self.close( event );
					self.selectedItem = item;
				},
				blur: function( event, ui ) {
					// don't set the value of the text field if it's already correct
					// this prevents moving the cursor unnecessarily
					if ( self.menu.element.is(":visible") &&
						( self.element.val() !== self.term ) ) {
						self.element.val( self.term );
					}
				}
			})
			.zIndex( this.element.zIndex() + 1 )
			// workaround for jQuery bug #5781 http://dev.jquery.com/ticket/5781
			.css({ top: 0, left: 0 })
			.hide()
			.data( "menu" );
		if ( $.fn.bgiframe ) {
			 this.menu.element.bgiframe();
		}
	},

	destroy: function() {
		this.element
			.removeClass( "ui-autocomplete-input" )
			.removeAttr( "autocomplete" )
			.removeAttr( "role" )
			.removeAttr( "aria-autocomplete" )
			.removeAttr( "aria-haspopup" );
		this.menu.element.remove();
		$.Widget.prototype.destroy.call( this );
	},

	_setOption: function( key, value ) {
		$.Widget.prototype._setOption.apply( this, arguments );
		if ( key === "source" ) {
			this._initSource();
		}
		if ( key === "appendTo" ) {
			this.menu.element.appendTo( $( value || "body", this.element[0].ownerDocument )[0] )
		}
		if ( key === "disabled" && value && this.xhr ) {
			this.xhr.abort();
		}
	},

	_initSource: function() {
		var self = this,
			array,
			url;
		if ( $.isArray(this.options.source) ) {
			array = this.options.source;
			this.source = function( request, response ) {
				response( $.ui.autocomplete.filter(array, request.term) );
			};
		} else if ( typeof this.options.source === "string" ) {
			url = this.options.source;
			this.source = function( request, response ) {
				if ( self.xhr ) {
					self.xhr.abort();
				}
				self.xhr = $.ajax({
					url: url,
					data: request,
					dataType: "json",
					autocompleteRequest: ++requestIndex,
					success: function( data, status ) {
						if ( this.autocompleteRequest === requestIndex ) {
							response( data );
						}
					},
					error: function() {
						if ( this.autocompleteRequest === requestIndex ) {
							response( [] );
						}
					}
				});
			};
		} else {
			this.source = this.options.source;
		}
	},

	search: function( value, event ) {
		value = value != null ? value : this.element.val();

		// always save the actual value, not the one passed as an argument
		this.term = this.element.val();

		if ( value.length < this.options.minLength ) {
			return this.close( event );
		}

		clearTimeout( this.closing );
		if ( this._trigger( "search", event ) === false ) {
			return;
		}

		return this._search( value );
	},

	_search: function( value ) {
		this.pending++;
		this.element.addClass( "ui-autocomplete-loading" );

		this.source( { term: value }, this.response );
	},

	_response: function( content ) {
		if ( !this.options.disabled && content && content.length ) {
			content = this._normalize( content );
			this._suggest( content );
			this._trigger( "open" );
		} else {
			this.close();
		}
		this.pending--;
		if ( !this.pending ) {
			this.element.removeClass( "ui-autocomplete-loading" );
		}
	},

	close: function( event ) {
		clearTimeout( this.closing );
		if ( this.menu.element.is(":visible") ) {
			this.menu.element.hide();
			this.menu.deactivate();
			this._trigger( "close", event );
		}
	},
	
	_change: function( event ) {
		if ( this.previous !== this.element.val() ) {
			this._trigger( "change", event, { item: this.selectedItem } );
		}
	},

	_normalize: function( items ) {
		// assume all items have the right format when the first item is complete
		if ( items.length && items[0].label && items[0].value ) {
			return items;
		}
		return $.map( items, function(item) {
			if ( typeof item === "string" ) {
				return {
					label: item,
					value: item
				};
			}
			return $.extend({
				label: item.label || item.value,
				value: item.value || item.label
			}, item );
		});
	},

	_suggest: function( items ) {
		var ul = this.menu.element
			.empty()
			.zIndex( this.element.zIndex() + 1 );
		this._renderMenu( ul, items );
		// TODO refresh should check if the active item is still in the dom, removing the need for a manual deactivate
		this.menu.deactivate();
		this.menu.refresh();

		// size and position menu
		ul.show();
		this._resizeMenu();
		ul.position( $.extend({
			of: this.element
		}, this.options.position ));

		if ( this.options.autoFocus ) {
			this.menu.next( new $.Event("mouseover") );
		}
	},

	_resizeMenu: function() {
		var ul = this.menu.element;
		ul.outerWidth( Math.max(
			ul.width( "" ).outerWidth(),
			this.element.outerWidth()
		) );
	},

	_renderMenu: function( ul, items ) {
		var self = this;
		$.each( items, function( index, item ) {
			self._renderItem( ul, item );
		});
	},

	_renderItem: function( ul, item) {
		return $( "<li></li>" )
			.data( "item.autocomplete", item )
			.append( $( "<a></a>" ).text( item.label ) )
			.appendTo( ul );
	},

	_move: function( direction, event ) {
		if ( !this.menu.element.is(":visible") ) {
			this.search( null, event );
			return;
		}
		if ( this.menu.first() && /^previous/.test(direction) ||
				this.menu.last() && /^next/.test(direction) ) {
			this.element.val( this.term );
			this.menu.deactivate();
			return;
		}
		this.menu[ direction ]( event );
	},

	widget: function() {
		return this.menu.element;
	}
});

$.extend( $.ui.autocomplete, {
	escapeRegex: function( value ) {
		return value.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
	},
	filter: function(array, term) {
		var matcher = new RegExp( $.ui.autocomplete.escapeRegex(term), "i" );
		return $.grep( array, function(value) {
			return matcher.test( value.label || value.value || value );
		});
	}
});

}( jQuery ));

/*
 * jQuery UI Menu (not officially released)
 * 
 * This widget isn't yet finished and the API is subject to change. We plan to finish
 * it for the next release. You're welcome to give it a try anyway and give us feedback,
 * as long as you're okay with migrating your code later on. We can help with that, too.
 *
 * Copyright 2010, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Menu
 *
 * Depends:
 *	jquery.ui.core.js
 *  jquery.ui.widget.js
 */
(function($) {

$.widget("ui.menu", {
	_create: function() {
		var self = this;
		this.element
			.addClass("ui-menu ui-widget ui-widget-content ui-corner-all")
			.attr({
				role: "listbox",
				"aria-activedescendant": "ui-active-menuitem"
			})
			.click(function( event ) {
				if ( !$( event.target ).closest( ".ui-menu-item a" ).length ) {
					return;
				}
				// temporary
				event.preventDefault();
				self.select( event );
			});
		this.refresh();
	},
	
	refresh: function() {
		var self = this;

		// don't refresh list items that are already adapted
		var items = this.element.children("li:not(.ui-menu-item):has(a)")
			.addClass("ui-menu-item")
			.attr("role", "menuitem");
		
		items.children("a")
			.addClass("ui-corner-all")
			.attr("tabindex", -1)
			// mouseenter doesn't work with event delegation
			.mouseenter(function( event ) {
				self.activate( event, $(this).parent() );
			})
			.mouseleave(function() {
				self.deactivate();
			});
	},

	activate: function( event, item ) {
		this.deactivate();
		if (this.hasScroll()) {
			var offset = item.offset().top - this.element.offset().top,
				scroll = this.element.scrollTop(),
				elementHeight = this.element.height();
			if (offset < 0) {
				this.element.scrollTop( scroll + offset);
			} else if (offset >= elementHeight) {
				this.element.scrollTop( scroll + offset - elementHeight + item.height());
			}
		}
		this.active = item.eq(0)
			.children("a")
				.addClass("ui-state-hover")
				.attr("id", "ui-active-menuitem")
			.end();
		this._trigger("focus", event, { item: item });
	},

	deactivate: function() {
		if (!this.active) { return; }

		this.active.children("a")
			.removeClass("ui-state-hover")
			.removeAttr("id");
		this._trigger("blur");
		this.active = null;
	},

	next: function(event) {
		this.move("next", ".ui-menu-item:first", event);
	},

	previous: function(event) {
		this.move("prev", ".ui-menu-item:last", event);
	},

	first: function() {
		return this.active && !this.active.prevAll(".ui-menu-item").length;
	},

	last: function() {
		return this.active && !this.active.nextAll(".ui-menu-item").length;
	},

	move: function(direction, edge, event) {
		if (!this.active) {
			this.activate(event, this.element.children(edge));
			return;
		}
		var next = this.active[direction + "All"](".ui-menu-item").eq(0);
		if (next.length) {
			this.activate(event, next);
		} else {
			this.activate(event, this.element.children(edge));
		}
	},

	// TODO merge with previousPage
	nextPage: function(event) {
		if (this.hasScroll()) {
			// TODO merge with no-scroll-else
			if (!this.active || this.last()) {
				this.activate(event, this.element.children(".ui-menu-item:first"));
				return;
			}
			var base = this.active.offset().top,
				height = this.element.height(),
				result = this.element.children(".ui-menu-item").filter(function() {
					var close = $(this).offset().top - base - height + $(this).height();
					// TODO improve approximation
					return close < 10 && close > -10;
				});

			// TODO try to catch this earlier when scrollTop indicates the last page anyway
			if (!result.length) {
				result = this.element.children(".ui-menu-item:last");
			}
			this.activate(event, result);
		} else {
			this.activate(event, this.element.children(".ui-menu-item")
				.filter(!this.active || this.last() ? ":first" : ":last"));
		}
	},

	// TODO merge with nextPage
	previousPage: function(event) {
		if (this.hasScroll()) {
			// TODO merge with no-scroll-else
			if (!this.active || this.first()) {
				this.activate(event, this.element.children(".ui-menu-item:last"));
				return;
			}

			var base = this.active.offset().top,
				height = this.element.height();
				result = this.element.children(".ui-menu-item").filter(function() {
					var close = $(this).offset().top - base + height - $(this).height();
					// TODO improve approximation
					return close < 10 && close > -10;
				});

			// TODO try to catch this earlier when scrollTop indicates the last page anyway
			if (!result.length) {
				result = this.element.children(".ui-menu-item:first");
			}
			this.activate(event, result);
		} else {
			this.activate(event, this.element.children(".ui-menu-item")
				.filter(!this.active || this.first() ? ":last" : ":first"));
		}
	},

	hasScroll: function() {
		return this.element.height() < this.element[ $.fn.prop ? "prop" : "attr" ]("scrollHeight");
	},

	select: function( event ) {
		this._trigger("selected", event, { item: this.active });
	}
});

}(jQuery));

});
define('jqueryui/mouse', ['jquery','jqueryui/widget'], function (jQuery) {
/*!
 * jQuery UI Mouse 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Mouse
 *
 * Depends:
 *	jquery.ui.widget.js
 */
(function( $, undefined ) {

var mouseHandled = false;
$(document).mousedown(function(e) {
	mouseHandled = false;
});

$.widget("ui.mouse", {
	options: {
		cancel: ':input,option',
		distance: 1,
		delay: 0
	},
	_mouseInit: function() {
		var self = this;

		this.element
			.bind('mousedown.'+this.widgetName, function(event) {
				return self._mouseDown(event);
			})
			.bind('click.'+this.widgetName, function(event) {
				if (true === $.data(event.target, self.widgetName + '.preventClickEvent')) {
				    $.removeData(event.target, self.widgetName + '.preventClickEvent');
					event.stopImmediatePropagation();
					return false;
				}
			});

		this.started = false;
	},

	// TODO: make sure destroying one instance of mouse doesn't mess with
	// other instances of mouse
	_mouseDestroy: function() {
		this.element.unbind('.'+this.widgetName);
	},

	_mouseDown: function(event) {
		// don't let more than one widget handle mouseStart
		if(mouseHandled) {return};

		// we may have missed mouseup (out of window)
		(this._mouseStarted && this._mouseUp(event));

		this._mouseDownEvent = event;

		var self = this,
			btnIsLeft = (event.which == 1),
			elIsCancel = (typeof this.options.cancel == "string" ? $(event.target).closest(this.options.cancel).length : false);
		if (!btnIsLeft || elIsCancel || !this._mouseCapture(event)) {
			return true;
		}

		this.mouseDelayMet = !this.options.delay;
		if (!this.mouseDelayMet) {
			this._mouseDelayTimer = setTimeout(function() {
				self.mouseDelayMet = true;
			}, this.options.delay);
		}

		if (this._mouseDistanceMet(event) && this._mouseDelayMet(event)) {
			this._mouseStarted = (this._mouseStart(event) !== false);
			if (!this._mouseStarted) {
				event.preventDefault();
				return true;
			}
		}

		// Click event may never have fired (Gecko & Opera)
		if (true === $.data(event.target, this.widgetName + '.preventClickEvent')) {
			$.removeData(event.target, this.widgetName + '.preventClickEvent');
		}

		// these delegates are required to keep context
		this._mouseMoveDelegate = function(event) {
			return self._mouseMove(event);
		};
		this._mouseUpDelegate = function(event) {
			return self._mouseUp(event);
		};
		$(document)
			.bind('mousemove.'+this.widgetName, this._mouseMoveDelegate)
			.bind('mouseup.'+this.widgetName, this._mouseUpDelegate);

		event.preventDefault();
		
		mouseHandled = true;
		return true;
	},

	_mouseMove: function(event) {
		// IE mouseup check - mouseup happened when mouse was out of window
		if ($.browser.msie && !(document.documentMode >= 9) && !event.button) {
			return this._mouseUp(event);
		}

		if (this._mouseStarted) {
			this._mouseDrag(event);
			return event.preventDefault();
		}

		if (this._mouseDistanceMet(event) && this._mouseDelayMet(event)) {
			this._mouseStarted =
				(this._mouseStart(this._mouseDownEvent, event) !== false);
			(this._mouseStarted ? this._mouseDrag(event) : this._mouseUp(event));
		}

		return !this._mouseStarted;
	},

	_mouseUp: function(event) {
		$(document)
			.unbind('mousemove.'+this.widgetName, this._mouseMoveDelegate)
			.unbind('mouseup.'+this.widgetName, this._mouseUpDelegate);

		if (this._mouseStarted) {
			this._mouseStarted = false;

			if (event.target == this._mouseDownEvent.target) {
			    $.data(event.target, this.widgetName + '.preventClickEvent', true);
			}

			this._mouseStop(event);
		}

		return false;
	},

	_mouseDistanceMet: function(event) {
		return (Math.max(
				Math.abs(this._mouseDownEvent.pageX - event.pageX),
				Math.abs(this._mouseDownEvent.pageY - event.pageY)
			) >= this.options.distance
		);
	},

	_mouseDelayMet: function(event) {
		return this.mouseDelayMet;
	},

	// These are placeholder methods, to be overriden by extending plugin
	_mouseStart: function(event) {},
	_mouseDrag: function(event) {},
	_mouseStop: function(event) {},
	_mouseCapture: function(event) { return true; }
});

})(jQuery);

});
define('jqueryui/draggable', ['jquery','jqueryui/core','jqueryui/mouse','jqueryui/widget'], function (jQuery) {
/*
 * jQuery UI Draggable 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Draggables
 *
 * Depends:
 *	jquery.ui.core.js
 *	jquery.ui.mouse.js
 *	jquery.ui.widget.js
 */
(function( $, undefined ) {

$.widget("ui.draggable", $.ui.mouse, {
	widgetEventPrefix: "drag",
	options: {
		addClasses: true,
		appendTo: "parent",
		axis: false,
		connectToSortable: false,
		containment: false,
		cursor: "auto",
		cursorAt: false,
		grid: false,
		handle: false,
		helper: "original",
		iframeFix: false,
		opacity: false,
		refreshPositions: false,
		revert: false,
		revertDuration: 500,
		scope: "default",
		scroll: true,
		scrollSensitivity: 20,
		scrollSpeed: 20,
		snap: false,
		snapMode: "both",
		snapTolerance: 20,
		stack: false,
		zIndex: false
	},
	_create: function() {

		if (this.options.helper == 'original' && !(/^(?:r|a|f)/).test(this.element.css("position")))
			this.element[0].style.position = 'relative';

		(this.options.addClasses && this.element.addClass("ui-draggable"));
		(this.options.disabled && this.element.addClass("ui-draggable-disabled"));

		this._mouseInit();

	},

	destroy: function() {
		if(!this.element.data('draggable')) return;
		this.element
			.removeData("draggable")
			.unbind(".draggable")
			.removeClass("ui-draggable"
				+ " ui-draggable-dragging"
				+ " ui-draggable-disabled");
		this._mouseDestroy();

		return this;
	},

	_mouseCapture: function(event) {

		var o = this.options;

		// among others, prevent a drag on a resizable-handle
		if (this.helper || o.disabled || $(event.target).is('.ui-resizable-handle'))
			return false;

		//Quit if we're not on a valid handle
		this.handle = this._getHandle(event);
		if (!this.handle)
			return false;
		
		$(o.iframeFix === true ? "iframe" : o.iframeFix).each(function() {
			$('<div class="ui-draggable-iframeFix" style="background: #fff;"></div>')
			.css({
				width: this.offsetWidth+"px", height: this.offsetHeight+"px",
				position: "absolute", opacity: "0.001", zIndex: 1000
			})
			.css($(this).offset())
			.appendTo("body");
		});

		return true;

	},

	_mouseStart: function(event) {

		var o = this.options;

		//Create and append the visible helper
		this.helper = this._createHelper(event);

		//Cache the helper size
		this._cacheHelperProportions();

		//If ddmanager is used for droppables, set the global draggable
		if($.ui.ddmanager)
			$.ui.ddmanager.current = this;

		/*
		 * - Position generation -
		 * This block generates everything position related - it's the core of draggables.
		 */

		//Cache the margins of the original element
		this._cacheMargins();

		//Store the helper's css position
		this.cssPosition = this.helper.css("position");
		this.scrollParent = this.helper.scrollParent();

		//The element's absolute position on the page minus margins
		this.offset = this.positionAbs = this.element.offset();
		this.offset = {
			top: this.offset.top - this.margins.top,
			left: this.offset.left - this.margins.left
		};

		$.extend(this.offset, {
			click: { //Where the click happened, relative to the element
				left: event.pageX - this.offset.left,
				top: event.pageY - this.offset.top
			},
			parent: this._getParentOffset(),
			relative: this._getRelativeOffset() //This is a relative to absolute position minus the actual position calculation - only used for relative positioned helper
		});

		//Generate the original position
		this.originalPosition = this.position = this._generatePosition(event);
		this.originalPageX = event.pageX;
		this.originalPageY = event.pageY;

		//Adjust the mouse offset relative to the helper if 'cursorAt' is supplied
		(o.cursorAt && this._adjustOffsetFromHelper(o.cursorAt));

		//Set a containment if given in the options
		if(o.containment)
			this._setContainment();

		//Trigger event + callbacks
		if(this._trigger("start", event) === false) {
			this._clear();
			return false;
		}

		//Recache the helper size
		this._cacheHelperProportions();

		//Prepare the droppable offsets
		if ($.ui.ddmanager && !o.dropBehaviour)
			$.ui.ddmanager.prepareOffsets(this, event);

		this.helper.addClass("ui-draggable-dragging");
		this._mouseDrag(event, true); //Execute the drag once - this causes the helper not to be visible before getting its correct position
		
		//If the ddmanager is used for droppables, inform the manager that dragging has started (see #5003)
		if ( $.ui.ddmanager ) $.ui.ddmanager.dragStart(this, event);
		
		return true;
	},

	_mouseDrag: function(event, noPropagation) {

		//Compute the helpers position
		this.position = this._generatePosition(event);
		this.positionAbs = this._convertPositionTo("absolute");

		//Call plugins and callbacks and use the resulting position if something is returned
		if (!noPropagation) {
			var ui = this._uiHash();
			if(this._trigger('drag', event, ui) === false) {
				this._mouseUp({});
				return false;
			}
			this.position = ui.position;
		}

		if(!this.options.axis || this.options.axis != "y") this.helper[0].style.left = this.position.left+'px';
		if(!this.options.axis || this.options.axis != "x") this.helper[0].style.top = this.position.top+'px';
		if($.ui.ddmanager) $.ui.ddmanager.drag(this, event);

		return false;
	},

	_mouseStop: function(event) {

		//If we are using droppables, inform the manager about the drop
		var dropped = false;
		if ($.ui.ddmanager && !this.options.dropBehaviour)
			dropped = $.ui.ddmanager.drop(this, event);

		//if a drop comes from outside (a sortable)
		if(this.dropped) {
			dropped = this.dropped;
			this.dropped = false;
		}
		
		//if the original element is removed, don't bother to continue if helper is set to "original"
		if((!this.element[0] || !this.element[0].parentNode) && this.options.helper == "original")
			return false;

		if((this.options.revert == "invalid" && !dropped) || (this.options.revert == "valid" && dropped) || this.options.revert === true || ($.isFunction(this.options.revert) && this.options.revert.call(this.element, dropped))) {
			var self = this;
			$(this.helper).animate(this.originalPosition, parseInt(this.options.revertDuration, 10), function() {
				if(self._trigger("stop", event) !== false) {
					self._clear();
				}
			});
		} else {
			if(this._trigger("stop", event) !== false) {
				this._clear();
			}
		}

		return false;
	},
	
	_mouseUp: function(event) {
		if (this.options.iframeFix === true) {
			$("div.ui-draggable-iframeFix").each(function() { 
				this.parentNode.removeChild(this); 
			}); //Remove frame helpers
		}
		
		//If the ddmanager is used for droppables, inform the manager that dragging has stopped (see #5003)
		if( $.ui.ddmanager ) $.ui.ddmanager.dragStop(this, event);
		
		return $.ui.mouse.prototype._mouseUp.call(this, event);
	},
	
	cancel: function() {
		
		if(this.helper.is(".ui-draggable-dragging")) {
			this._mouseUp({});
		} else {
			this._clear();
		}
		
		return this;
		
	},

	_getHandle: function(event) {

		var handle = !this.options.handle || !$(this.options.handle, this.element).length ? true : false;
		$(this.options.handle, this.element)
			.find("*")
			.andSelf()
			.each(function() {
				if(this == event.target) handle = true;
			});

		return handle;

	},

	_createHelper: function(event) {

		var o = this.options;
		var helper = $.isFunction(o.helper) ? $(o.helper.apply(this.element[0], [event])) : (o.helper == 'clone' ? this.element.clone().removeAttr('id') : this.element);

		if(!helper.parents('body').length)
			helper.appendTo((o.appendTo == 'parent' ? this.element[0].parentNode : o.appendTo));

		if(helper[0] != this.element[0] && !(/(fixed|absolute)/).test(helper.css("position")))
			helper.css("position", "absolute");

		return helper;

	},

	_adjustOffsetFromHelper: function(obj) {
		if (typeof obj == 'string') {
			obj = obj.split(' ');
		}
		if ($.isArray(obj)) {
			obj = {left: +obj[0], top: +obj[1] || 0};
		}
		if ('left' in obj) {
			this.offset.click.left = obj.left + this.margins.left;
		}
		if ('right' in obj) {
			this.offset.click.left = this.helperProportions.width - obj.right + this.margins.left;
		}
		if ('top' in obj) {
			this.offset.click.top = obj.top + this.margins.top;
		}
		if ('bottom' in obj) {
			this.offset.click.top = this.helperProportions.height - obj.bottom + this.margins.top;
		}
	},

	_getParentOffset: function() {

		//Get the offsetParent and cache its position
		this.offsetParent = this.helper.offsetParent();
		var po = this.offsetParent.offset();

		// This is a special case where we need to modify a offset calculated on start, since the following happened:
		// 1. The position of the helper is absolute, so it's position is calculated based on the next positioned parent
		// 2. The actual offset parent is a child of the scroll parent, and the scroll parent isn't the document, which means that
		//    the scroll is included in the initial calculation of the offset of the parent, and never recalculated upon drag
		if(this.cssPosition == 'absolute' && this.scrollParent[0] != document && $.ui.contains(this.scrollParent[0], this.offsetParent[0])) {
			po.left += this.scrollParent.scrollLeft();
			po.top += this.scrollParent.scrollTop();
		}

		if((this.offsetParent[0] == document.body) //This needs to be actually done for all browsers, since pageX/pageY includes this information
		|| (this.offsetParent[0].tagName && this.offsetParent[0].tagName.toLowerCase() == 'html' && $.browser.msie)) //Ugly IE fix
			po = { top: 0, left: 0 };

		return {
			top: po.top + (parseInt(this.offsetParent.css("borderTopWidth"),10) || 0),
			left: po.left + (parseInt(this.offsetParent.css("borderLeftWidth"),10) || 0)
		};

	},

	_getRelativeOffset: function() {

		if(this.cssPosition == "relative") {
			var p = this.element.position();
			return {
				top: p.top - (parseInt(this.helper.css("top"),10) || 0) + this.scrollParent.scrollTop(),
				left: p.left - (parseInt(this.helper.css("left"),10) || 0) + this.scrollParent.scrollLeft()
			};
		} else {
			return { top: 0, left: 0 };
		}

	},

	_cacheMargins: function() {
		this.margins = {
			left: (parseInt(this.element.css("marginLeft"),10) || 0),
			top: (parseInt(this.element.css("marginTop"),10) || 0),
			right: (parseInt(this.element.css("marginRight"),10) || 0),
			bottom: (parseInt(this.element.css("marginBottom"),10) || 0)
		};
	},

	_cacheHelperProportions: function() {
		this.helperProportions = {
			width: this.helper.outerWidth(),
			height: this.helper.outerHeight()
		};
	},

	_setContainment: function() {

		var o = this.options;
		if(o.containment == 'parent') o.containment = this.helper[0].parentNode;
		if(o.containment == 'document' || o.containment == 'window') this.containment = [
			o.containment == 'document' ? 0 : $(window).scrollLeft() - this.offset.relative.left - this.offset.parent.left,
			o.containment == 'document' ? 0 : $(window).scrollTop() - this.offset.relative.top - this.offset.parent.top,
			(o.containment == 'document' ? 0 : $(window).scrollLeft()) + $(o.containment == 'document' ? document : window).width() - this.helperProportions.width - this.margins.left,
			(o.containment == 'document' ? 0 : $(window).scrollTop()) + ($(o.containment == 'document' ? document : window).height() || document.body.parentNode.scrollHeight) - this.helperProportions.height - this.margins.top
		];

		if(!(/^(document|window|parent)$/).test(o.containment) && o.containment.constructor != Array) {
		        var c = $(o.containment);
			var ce = c[0]; if(!ce) return;
			var co = c.offset();
			var over = ($(ce).css("overflow") != 'hidden');

			this.containment = [
				(parseInt($(ce).css("borderLeftWidth"),10) || 0) + (parseInt($(ce).css("paddingLeft"),10) || 0),
				(parseInt($(ce).css("borderTopWidth"),10) || 0) + (parseInt($(ce).css("paddingTop"),10) || 0),
				(over ? Math.max(ce.scrollWidth,ce.offsetWidth) : ce.offsetWidth) - (parseInt($(ce).css("borderLeftWidth"),10) || 0) - (parseInt($(ce).css("paddingRight"),10) || 0) - this.helperProportions.width - this.margins.left - this.margins.right,
				(over ? Math.max(ce.scrollHeight,ce.offsetHeight) : ce.offsetHeight) - (parseInt($(ce).css("borderTopWidth"),10) || 0) - (parseInt($(ce).css("paddingBottom"),10) || 0) - this.helperProportions.height - this.margins.top  - this.margins.bottom
			];
			this.relative_container = c;

		} else if(o.containment.constructor == Array) {
			this.containment = o.containment;
		}

	},

	_convertPositionTo: function(d, pos) {

		if(!pos) pos = this.position;
		var mod = d == "absolute" ? 1 : -1;
		var o = this.options, scroll = this.cssPosition == 'absolute' && !(this.scrollParent[0] != document && $.ui.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent, scrollIsRootNode = (/(html|body)/i).test(scroll[0].tagName);

		return {
			top: (
				pos.top																	// The absolute mouse position
				+ this.offset.relative.top * mod										// Only for relative positioned nodes: Relative offset from element to offset parent
				+ this.offset.parent.top * mod											// The offsetParent's offset without borders (offset + border)
				- ($.browser.safari && $.browser.version < 526 && this.cssPosition == 'fixed' ? 0 : ( this.cssPosition == 'fixed' ? -this.scrollParent.scrollTop() : ( scrollIsRootNode ? 0 : scroll.scrollTop() ) ) * mod)
			),
			left: (
				pos.left																// The absolute mouse position
				+ this.offset.relative.left * mod										// Only for relative positioned nodes: Relative offset from element to offset parent
				+ this.offset.parent.left * mod											// The offsetParent's offset without borders (offset + border)
				- ($.browser.safari && $.browser.version < 526 && this.cssPosition == 'fixed' ? 0 : ( this.cssPosition == 'fixed' ? -this.scrollParent.scrollLeft() : scrollIsRootNode ? 0 : scroll.scrollLeft() ) * mod)
			)
		};

	},

	_generatePosition: function(event) {

		var o = this.options, scroll = this.cssPosition == 'absolute' && !(this.scrollParent[0] != document && $.ui.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent, scrollIsRootNode = (/(html|body)/i).test(scroll[0].tagName);
		var pageX = event.pageX;
		var pageY = event.pageY;

		/*
		 * - Position constraining -
		 * Constrain the position to a mix of grid, containment.
		 */

		if(this.originalPosition) { //If we are not dragging yet, we won't check for options
		         var containment;
		         if(this.containment) {
				 if (this.relative_container){
				     var co = this.relative_container.offset();
				     containment = [ this.containment[0] + co.left,
						     this.containment[1] + co.top,
						     this.containment[2] + co.left,
						     this.containment[3] + co.top ];
				 }
				 else {
				     containment = this.containment;
				 }

				if(event.pageX - this.offset.click.left < containment[0]) pageX = containment[0] + this.offset.click.left;
				if(event.pageY - this.offset.click.top < containment[1]) pageY = containment[1] + this.offset.click.top;
				if(event.pageX - this.offset.click.left > containment[2]) pageX = containment[2] + this.offset.click.left;
				if(event.pageY - this.offset.click.top > containment[3]) pageY = containment[3] + this.offset.click.top;
			}

			if(o.grid) {
				//Check for grid elements set to 0 to prevent divide by 0 error causing invalid argument errors in IE (see ticket #6950)
				var top = o.grid[1] ? this.originalPageY + Math.round((pageY - this.originalPageY) / o.grid[1]) * o.grid[1] : this.originalPageY;
				pageY = containment ? (!(top - this.offset.click.top < containment[1] || top - this.offset.click.top > containment[3]) ? top : (!(top - this.offset.click.top < containment[1]) ? top - o.grid[1] : top + o.grid[1])) : top;

				var left = o.grid[0] ? this.originalPageX + Math.round((pageX - this.originalPageX) / o.grid[0]) * o.grid[0] : this.originalPageX;
				pageX = containment ? (!(left - this.offset.click.left < containment[0] || left - this.offset.click.left > containment[2]) ? left : (!(left - this.offset.click.left < containment[0]) ? left - o.grid[0] : left + o.grid[0])) : left;
			}

		}

		return {
			top: (
				pageY																// The absolute mouse position
				- this.offset.click.top													// Click offset (relative to the element)
				- this.offset.relative.top												// Only for relative positioned nodes: Relative offset from element to offset parent
				- this.offset.parent.top												// The offsetParent's offset without borders (offset + border)
				+ ($.browser.safari && $.browser.version < 526 && this.cssPosition == 'fixed' ? 0 : ( this.cssPosition == 'fixed' ? -this.scrollParent.scrollTop() : ( scrollIsRootNode ? 0 : scroll.scrollTop() ) ))
			),
			left: (
				pageX																// The absolute mouse position
				- this.offset.click.left												// Click offset (relative to the element)
				- this.offset.relative.left												// Only for relative positioned nodes: Relative offset from element to offset parent
				- this.offset.parent.left												// The offsetParent's offset without borders (offset + border)
				+ ($.browser.safari && $.browser.version < 526 && this.cssPosition == 'fixed' ? 0 : ( this.cssPosition == 'fixed' ? -this.scrollParent.scrollLeft() : scrollIsRootNode ? 0 : scroll.scrollLeft() ))
			)
		};

	},

	_clear: function() {
		this.helper.removeClass("ui-draggable-dragging");
		if(this.helper[0] != this.element[0] && !this.cancelHelperRemoval) this.helper.remove();
		//if($.ui.ddmanager) $.ui.ddmanager.current = null;
		this.helper = null;
		this.cancelHelperRemoval = false;
	},

	// From now on bulk stuff - mainly helpers

	_trigger: function(type, event, ui) {
		ui = ui || this._uiHash();
		$.ui.plugin.call(this, type, [event, ui]);
		if(type == "drag") this.positionAbs = this._convertPositionTo("absolute"); //The absolute position has to be recalculated after plugins
		return $.Widget.prototype._trigger.call(this, type, event, ui);
	},

	plugins: {},

	_uiHash: function(event) {
		return {
			helper: this.helper,
			position: this.position,
			originalPosition: this.originalPosition,
			offset: this.positionAbs
		};
	}

});

$.extend($.ui.draggable, {
	version: "1.8.14"
});

$.ui.plugin.add("draggable", "connectToSortable", {
	start: function(event, ui) {

		var inst = $(this).data("draggable"), o = inst.options,
			uiSortable = $.extend({}, ui, { item: inst.element });
		inst.sortables = [];
		$(o.connectToSortable).each(function() {
			var sortable = $.data(this, 'sortable');
			if (sortable && !sortable.options.disabled) {
				inst.sortables.push({
					instance: sortable,
					shouldRevert: sortable.options.revert
				});
				sortable.refreshPositions();	// Call the sortable's refreshPositions at drag start to refresh the containerCache since the sortable container cache is used in drag and needs to be up to date (this will ensure it's initialised as well as being kept in step with any changes that might have happened on the page).
				sortable._trigger("activate", event, uiSortable);
			}
		});

	},
	stop: function(event, ui) {

		//If we are still over the sortable, we fake the stop event of the sortable, but also remove helper
		var inst = $(this).data("draggable"),
			uiSortable = $.extend({}, ui, { item: inst.element });

		$.each(inst.sortables, function() {
			if(this.instance.isOver) {

				this.instance.isOver = 0;

				inst.cancelHelperRemoval = true; //Don't remove the helper in the draggable instance
				this.instance.cancelHelperRemoval = false; //Remove it in the sortable instance (so sortable plugins like revert still work)

				//The sortable revert is supported, and we have to set a temporary dropped variable on the draggable to support revert: 'valid/invalid'
				if(this.shouldRevert) this.instance.options.revert = true;

				//Trigger the stop of the sortable
				this.instance._mouseStop(event);

				this.instance.options.helper = this.instance.options._helper;

				//If the helper has been the original item, restore properties in the sortable
				if(inst.options.helper == 'original')
					this.instance.currentItem.css({ top: 'auto', left: 'auto' });

			} else {
				this.instance.cancelHelperRemoval = false; //Remove the helper in the sortable instance
				this.instance._trigger("deactivate", event, uiSortable);
			}

		});

	},
	drag: function(event, ui) {

		var inst = $(this).data("draggable"), self = this;

		var checkPos = function(o) {
			var dyClick = this.offset.click.top, dxClick = this.offset.click.left;
			var helperTop = this.positionAbs.top, helperLeft = this.positionAbs.left;
			var itemHeight = o.height, itemWidth = o.width;
			var itemTop = o.top, itemLeft = o.left;

			return $.ui.isOver(helperTop + dyClick, helperLeft + dxClick, itemTop, itemLeft, itemHeight, itemWidth);
		};

		$.each(inst.sortables, function(i) {
			
			//Copy over some variables to allow calling the sortable's native _intersectsWith
			this.instance.positionAbs = inst.positionAbs;
			this.instance.helperProportions = inst.helperProportions;
			this.instance.offset.click = inst.offset.click;
			
			if(this.instance._intersectsWith(this.instance.containerCache)) {

				//If it intersects, we use a little isOver variable and set it once, so our move-in stuff gets fired only once
				if(!this.instance.isOver) {

					this.instance.isOver = 1;
					//Now we fake the start of dragging for the sortable instance,
					//by cloning the list group item, appending it to the sortable and using it as inst.currentItem
					//We can then fire the start event of the sortable with our passed browser event, and our own helper (so it doesn't create a new one)
					this.instance.currentItem = $(self).clone().removeAttr('id').appendTo(this.instance.element).data("sortable-item", true);
					this.instance.options._helper = this.instance.options.helper; //Store helper option to later restore it
					this.instance.options.helper = function() { return ui.helper[0]; };

					event.target = this.instance.currentItem[0];
					this.instance._mouseCapture(event, true);
					this.instance._mouseStart(event, true, true);

					//Because the browser event is way off the new appended portlet, we modify a couple of variables to reflect the changes
					this.instance.offset.click.top = inst.offset.click.top;
					this.instance.offset.click.left = inst.offset.click.left;
					this.instance.offset.parent.left -= inst.offset.parent.left - this.instance.offset.parent.left;
					this.instance.offset.parent.top -= inst.offset.parent.top - this.instance.offset.parent.top;

					inst._trigger("toSortable", event);
					inst.dropped = this.instance.element; //draggable revert needs that
					//hack so receive/update callbacks work (mostly)
					inst.currentItem = inst.element;
					this.instance.fromOutside = inst;

				}

				//Provided we did all the previous steps, we can fire the drag event of the sortable on every draggable drag, when it intersects with the sortable
				if(this.instance.currentItem) this.instance._mouseDrag(event);

			} else {

				//If it doesn't intersect with the sortable, and it intersected before,
				//we fake the drag stop of the sortable, but make sure it doesn't remove the helper by using cancelHelperRemoval
				if(this.instance.isOver) {

					this.instance.isOver = 0;
					this.instance.cancelHelperRemoval = true;
					
					//Prevent reverting on this forced stop
					this.instance.options.revert = false;
					
					// The out event needs to be triggered independently
					this.instance._trigger('out', event, this.instance._uiHash(this.instance));
					
					this.instance._mouseStop(event, true);
					this.instance.options.helper = this.instance.options._helper;

					//Now we remove our currentItem, the list group clone again, and the placeholder, and animate the helper back to it's original size
					this.instance.currentItem.remove();
					if(this.instance.placeholder) this.instance.placeholder.remove();

					inst._trigger("fromSortable", event);
					inst.dropped = false; //draggable revert needs that
				}

			};

		});

	}
});

$.ui.plugin.add("draggable", "cursor", {
	start: function(event, ui) {
		var t = $('body'), o = $(this).data('draggable').options;
		if (t.css("cursor")) o._cursor = t.css("cursor");
		t.css("cursor", o.cursor);
	},
	stop: function(event, ui) {
		var o = $(this).data('draggable').options;
		if (o._cursor) $('body').css("cursor", o._cursor);
	}
});

$.ui.plugin.add("draggable", "opacity", {
	start: function(event, ui) {
		var t = $(ui.helper), o = $(this).data('draggable').options;
		if(t.css("opacity")) o._opacity = t.css("opacity");
		t.css('opacity', o.opacity);
	},
	stop: function(event, ui) {
		var o = $(this).data('draggable').options;
		if(o._opacity) $(ui.helper).css('opacity', o._opacity);
	}
});

$.ui.plugin.add("draggable", "scroll", {
	start: function(event, ui) {
		var i = $(this).data("draggable");
		if(i.scrollParent[0] != document && i.scrollParent[0].tagName != 'HTML') i.overflowOffset = i.scrollParent.offset();
	},
	drag: function(event, ui) {

		var i = $(this).data("draggable"), o = i.options, scrolled = false;

		if(i.scrollParent[0] != document && i.scrollParent[0].tagName != 'HTML') {

			if(!o.axis || o.axis != 'x') {
				if((i.overflowOffset.top + i.scrollParent[0].offsetHeight) - event.pageY < o.scrollSensitivity)
					i.scrollParent[0].scrollTop = scrolled = i.scrollParent[0].scrollTop + o.scrollSpeed;
				else if(event.pageY - i.overflowOffset.top < o.scrollSensitivity)
					i.scrollParent[0].scrollTop = scrolled = i.scrollParent[0].scrollTop - o.scrollSpeed;
			}

			if(!o.axis || o.axis != 'y') {
				if((i.overflowOffset.left + i.scrollParent[0].offsetWidth) - event.pageX < o.scrollSensitivity)
					i.scrollParent[0].scrollLeft = scrolled = i.scrollParent[0].scrollLeft + o.scrollSpeed;
				else if(event.pageX - i.overflowOffset.left < o.scrollSensitivity)
					i.scrollParent[0].scrollLeft = scrolled = i.scrollParent[0].scrollLeft - o.scrollSpeed;
			}

		} else {

			if(!o.axis || o.axis != 'x') {
				if(event.pageY - $(document).scrollTop() < o.scrollSensitivity)
					scrolled = $(document).scrollTop($(document).scrollTop() - o.scrollSpeed);
				else if($(window).height() - (event.pageY - $(document).scrollTop()) < o.scrollSensitivity)
					scrolled = $(document).scrollTop($(document).scrollTop() + o.scrollSpeed);
			}

			if(!o.axis || o.axis != 'y') {
				if(event.pageX - $(document).scrollLeft() < o.scrollSensitivity)
					scrolled = $(document).scrollLeft($(document).scrollLeft() - o.scrollSpeed);
				else if($(window).width() - (event.pageX - $(document).scrollLeft()) < o.scrollSensitivity)
					scrolled = $(document).scrollLeft($(document).scrollLeft() + o.scrollSpeed);
			}

		}

		if(scrolled !== false && $.ui.ddmanager && !o.dropBehaviour)
			$.ui.ddmanager.prepareOffsets(i, event);

	}
});

$.ui.plugin.add("draggable", "snap", {
	start: function(event, ui) {

		var i = $(this).data("draggable"), o = i.options;
		i.snapElements = [];

		$(o.snap.constructor != String ? ( o.snap.items || ':data(draggable)' ) : o.snap).each(function() {
			var $t = $(this); var $o = $t.offset();
			if(this != i.element[0]) i.snapElements.push({
				item: this,
				width: $t.outerWidth(), height: $t.outerHeight(),
				top: $o.top, left: $o.left
			});
		});

	},
	drag: function(event, ui) {

		var inst = $(this).data("draggable"), o = inst.options;
		var d = o.snapTolerance;

		var x1 = ui.offset.left, x2 = x1 + inst.helperProportions.width,
			y1 = ui.offset.top, y2 = y1 + inst.helperProportions.height;

		for (var i = inst.snapElements.length - 1; i >= 0; i--){

			var l = inst.snapElements[i].left, r = l + inst.snapElements[i].width,
				t = inst.snapElements[i].top, b = t + inst.snapElements[i].height;

			//Yes, I know, this is insane ;)
			if(!((l-d < x1 && x1 < r+d && t-d < y1 && y1 < b+d) || (l-d < x1 && x1 < r+d && t-d < y2 && y2 < b+d) || (l-d < x2 && x2 < r+d && t-d < y1 && y1 < b+d) || (l-d < x2 && x2 < r+d && t-d < y2 && y2 < b+d))) {
				if(inst.snapElements[i].snapping) (inst.options.snap.release && inst.options.snap.release.call(inst.element, event, $.extend(inst._uiHash(), { snapItem: inst.snapElements[i].item })));
				inst.snapElements[i].snapping = false;
				continue;
			}

			if(o.snapMode != 'inner') {
				var ts = Math.abs(t - y2) <= d;
				var bs = Math.abs(b - y1) <= d;
				var ls = Math.abs(l - x2) <= d;
				var rs = Math.abs(r - x1) <= d;
				if(ts) ui.position.top = inst._convertPositionTo("relative", { top: t - inst.helperProportions.height, left: 0 }).top - inst.margins.top;
				if(bs) ui.position.top = inst._convertPositionTo("relative", { top: b, left: 0 }).top - inst.margins.top;
				if(ls) ui.position.left = inst._convertPositionTo("relative", { top: 0, left: l - inst.helperProportions.width }).left - inst.margins.left;
				if(rs) ui.position.left = inst._convertPositionTo("relative", { top: 0, left: r }).left - inst.margins.left;
			}

			var first = (ts || bs || ls || rs);

			if(o.snapMode != 'outer') {
				var ts = Math.abs(t - y1) <= d;
				var bs = Math.abs(b - y2) <= d;
				var ls = Math.abs(l - x1) <= d;
				var rs = Math.abs(r - x2) <= d;
				if(ts) ui.position.top = inst._convertPositionTo("relative", { top: t, left: 0 }).top - inst.margins.top;
				if(bs) ui.position.top = inst._convertPositionTo("relative", { top: b - inst.helperProportions.height, left: 0 }).top - inst.margins.top;
				if(ls) ui.position.left = inst._convertPositionTo("relative", { top: 0, left: l }).left - inst.margins.left;
				if(rs) ui.position.left = inst._convertPositionTo("relative", { top: 0, left: r - inst.helperProportions.width }).left - inst.margins.left;
			}

			if(!inst.snapElements[i].snapping && (ts || bs || ls || rs || first))
				(inst.options.snap.snap && inst.options.snap.snap.call(inst.element, event, $.extend(inst._uiHash(), { snapItem: inst.snapElements[i].item })));
			inst.snapElements[i].snapping = (ts || bs || ls || rs || first);

		};

	}
});

$.ui.plugin.add("draggable", "stack", {
	start: function(event, ui) {

		var o = $(this).data("draggable").options;

		var group = $.makeArray($(o.stack)).sort(function(a,b) {
			return (parseInt($(a).css("zIndex"),10) || 0) - (parseInt($(b).css("zIndex"),10) || 0);
		});
		if (!group.length) { return; }
		
		var min = parseInt(group[0].style.zIndex) || 0;
		$(group).each(function(i) {
			this.style.zIndex = min + i;
		});

		this[0].style.zIndex = min + group.length;

	}
});

$.ui.plugin.add("draggable", "zIndex", {
	start: function(event, ui) {
		var t = $(ui.helper), o = $(this).data("draggable").options;
		if(t.css("zIndex")) o._zIndex = t.css("zIndex");
		t.css('zIndex', o.zIndex);
	},
	stop: function(event, ui) {
		var o = $(this).data("draggable").options;
		if(o._zIndex) $(ui.helper).css('zIndex', o._zIndex);
	}
});

})(jQuery);

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMapView
// -----------

// View that allows user to choose location on a mpa or via coordinates
define('views/editmapview',['jquery', 'underscore', 'backbone', 'app', 'views/dummyoverlayview', 'jqueryui/autocomplete', 'jqueryui/draggable', 'async!http://maps.google.com/maps/api/js?sensor=false&region=AU'],
function ($, _, Backbone, app, DummyOverlayView) {
    var EditMapView = Backbone.View.extend({
        id: 'location-fieldset',

        initialize: function (options) {
            this.observation = options.model;
            this.mapMarker = null;
            this.zIndex = 0;

            this.observation.mediaResources.on('change:Metadata', this.onMediaResourceFilesChanged, this);
        },

        render: function () {
            this._initMap();
            this._initAddressField();
            this._initLocationPin();
            return this;
        },

        onMediaResourceFilesChanged: function (mediaResource) {
            log('EditMapView.onMediaResourceAdded', mediaResource);
            var lat = mediaResource.get('Metadata').Latitude;
            var lon = mediaResource.get('Metadata').Longitude;

            if ((this.observation.get('Latitude') !== null || this.observation.get('Longitude') !== null) && lat && lon) {
                this.changeMarkerPosition(lat, lon);
            }
        },

        _initMap: function () {
            var g = google.maps;

            var mapSettings = {
                center: new g.LatLng(-29.191427, 134.472126), // Centre on Australia
                zoom: 4,
                panControl: false,
                streetViewControl: false,
                mapTypeControl: true,
                scrollwheel: false,
                mapTypeId: g.MapTypeId.TERRAIN
            };

            this.map = new g.Map(document.getElementById("location-map"), mapSettings);
            this.iw = new g.InfoWindow();
            this.geocoder = new g.Geocoder();

            // Add a dummy overlay for later use.
            // Needed for API v3 to convert pixels to latlng.
            this.dummy = new DummyOverlayView(this.map);
        },

        _initAddressField: function () {
            var self = this;
            // http://tech.cibul.net/geocode-with-google-maps-api-v3/
            this.$el.find("#Address").autocomplete({
                //This bit uses the geocoder to fetch address values
                source: function (request, response) {
                    self.geocoder.geocode({ address: request.term, region: 'AU' }, function (results, status) {
                        response($.map(results, function (item) {
                            return {
                                label: item.formatted_address,
                                value: item.formatted_address,
                                latitude: item.geometry.location.lat(),
                                longitude: item.geometry.location.lng()
                            }
                        }));
                    })
                },
                //This bit is executed upon selection of an address
                select: function (event, ui) {
                    var location = new google.maps.LatLng(ui.item.latitude, ui.item.longitude);
                    self._positionMarker(location);
                    self._removeMarkerFromPlaceholder();
                    self._reverseGeocode();

                    var position = self.mapMarker.getPosition();
                    var newPoint = new google.maps.LatLng(position.lat() + .02, position.lng());
                    self.map.panTo(newPoint);
                }
            });
        },

        _initLocationPin: function () {
            var self = this;
            $("#location-pin").draggable({
                //            drag: function (event, ui) {
                //                var locationPinLeft = ui.helper.offset().left + ui.helper.width() / 2;
                //                var locationPinTop = ui.helper.offset().top + ui.helper.height() / 2;
                //                $('#debug-info').text('left: ' + locationPinLeft + ', top: ' + locationPinTop);
                //            },
                stop: function (event, ui) {
                    var $mapDiv = $(self.map.getDiv());

                    var mapDivLeft = $mapDiv.offset().left;
                    var mapDivTop = $mapDiv.offset().top;
                    var mapDivWidth = $mapDiv.width();
                    var mapDivHeight = $mapDiv.height();

                    var locationPinLeft = ui.helper.offset().left;
                    var locationPinTop = ui.helper.offset().top;
                    var locationPinWidth = ui.helper.width();
                    var locationPinHeight = ui.helper.height();

                    var locationX = locationPinLeft + (locationPinWidth / 2);
                    var locationY = locationPinTop + (locationPinHeight / 2) - 6;

                    // Check if the cursor is inside the map div
                    if (locationX > mapDivLeft && locationX < (mapDivLeft + mapDivWidth) && locationY > mapDivTop && locationY < (mapDivTop + mapDivHeight)) {
                        // Find the object's pixel position in the map container
                        var g = google.maps;
                        var pixelpoint = new g.Point(locationX - mapDivLeft, locationY - mapDivTop + (locationPinHeight / 2));

                        // Corresponding geo point on the map
                        var proj = self.dummy.getProjection();
                        var latlng = proj.fromContainerPixelToLatLng(pixelpoint);

                        // Create a corresponding marker on the map
                        self._positionMarker(latlng);
                        self._removeMarkerFromPlaceholder();
                        self._reverseGeocode();
                    }
                }
            });
        },

        _positionMarker: function (point) {
            // Remove map marker first
            if (this.mapMarker) {
                this.mapMarker.setMap(null);
                this.mapMarker = null;
            }

            var g = google.maps;

            var image = new g.MarkerImage('http://maps.gstatic.com/mapfiles/ms/icons/blue-dot.png',
                new g.Size(32, 32),
                new g.Point(0, 0),
                new g.Point(15, 32)
                );

            var shadow = new g.MarkerImage("http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png",
                new g.Size(59, 32),
                new g.Point(0, 0),
                new g.Point(15, 32)
                );

            this.mapMarker = new g.Marker({
                position: point,
                map: this.map,
                clickable: true,
                draggable: true,
                raiseOnDrag: false,
                icon: image,
                shadow: shadow,
                zIndex: this._highestOrder()
            });

            var self = this;

            g.event.addListener(this.mapMarker, "drag", function () {
                //        var lat = mapMarker.getPosition().lat();
                //        var lng = mapMarker.getPosition().lng();
                //        iw.setContent(lat.toFixed(6) + ", " + lng.toFixed(6));
                //        iw.open(map, this);
                self._displayLatLong(false);
            });

            g.event.addListener(this.mapMarker, "dragstart", function () {
                // Close infowindow when dragging the marker whose infowindow is open
                //if (actual == this.mapMarker) iw.close();
                // Increment z_index
                self.zIndex++;
                self.mapMarker.setZIndex(self._highestOrder());
            });

            g.event.addListener(this.mapMarker, "dragend", function () {
                self._displayLatLong(true);
                self._reverseGeocode();
            });

            this._displayLatLong();
        },

        _highestOrder: function () {
            /**
            * The currently dragged marker on the map
            * always gets the highest z-index too
            */
            return this.zIndex;
        },

        changeMarkerPosition: function (lat, long) {
            var latlng = new google.maps.LatLng(lat, long);

            if (this.mapMarker === null) {
                this._positionMarker(latlng);
                this._removeMarkerFromPlaceholder();
                this._reverseGeocode();
            }
            else {
                this._positionMarker(latlng);
                this._reverseGeocode();
            }

            if (!this.map.getBounds().contains(this.mapMarker.getPosition())) {
                var position = this.mapMarker.getPosition();
                var newPoint = new google.maps.LatLng(position.lat() + .02, position.lng());
                this.map.panTo(newPoint);
            }
        },

        _displayLatLong: function (fireLatLongFieldsChangeEvent) {
            if (this.mapMarker) {
                var lat = this.mapMarker.getPosition().lat();
                var lng = this.mapMarker.getPosition().lng();
                //            if (this.observation.get('anonymiseLocation') === true) {
                $('#Latitude').val(lat);
                $('#Longitude').val(lng);
                //            }
                //            else {
                //                $('#latitude').val(parseFloat(lat).toFixed(1));
                //                $('#longitude').val(parseFloat(lng).toFixed(1));
                //            }
                if (fireLatLongFieldsChangeEvent) {
                    //$('#latitude').change();
                    $('#Longitude').change();
                }
            }
            else {
                return;
            }
        },

        _removeMarkerFromPlaceholder: function () {
            $('#location-pin').remove();
        },

        _reverseGeocode: function () {
            if (this.mapMarker) {
                this.geocoder.geocode({ latLng: this.mapMarker.getPosition() }, this._reverseGeocodeResult);
            }
        },

        _reverseGeocodeResult: function (results, status) {
            if (status == 'OK') {
                if (results.length == 0) {
                    $('#Address').val('');
                } else {
                    var addressResult = results[0].formatted_address;
                    $('#Address').val(addressResult);
                }
            } else {
                $('#Address').val('');
            }
            $('#Address').change();
        }
    });

    return EditMapView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResourceItemView
// ---------------------

define('views/mediaresourceitemview',['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var MediaResourceItemView = Backbone.View.extend({
        className: 'media-resource-item',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'showTempImageMedia',
            'showUploadedImageMedia',
            'removeMediaResource');

            // if image or video... 
            this.model.on('change:Files', this.showUploadedImageMedia);
        },

        render: function () {
            this.$el.append(ich.ObservationMediaResourceItem(this.model.toJSON())).css({ position: 'absolute', top: '-250px' });
            return this;
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.trigger('mediaresourceview:remove', this.model, this);
        },

        showTempImageMedia: function (img) {
            var $image = $(img);
            this.$el.find('div:first-child img').replaceWith($image);
            this.$el.width($image.width());
            this.imageWidth = $image.width();
        },

        showUploadedImageMedia: function (mediaResource) {
            log('MediaResourceItemView.showUploadedMedia', mediaResource);
            this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('Files').FullMedium.RelativeUri + '" alt="" />'));
        },

        showVideoMedia: function (preview) {
            //var src = mediaResource.get('Preview');
            log('MediaResourceItemView.showVideoMedia:', preview);
            this.$el.find('div:first-child').replaceWith(ich.VideoPreview({ Width: 300, Height: 220, Source: preview }));
            //this.$el.append(ich.VideoPreview({ Width: 220, Height: 200, Source: src }));
        }
    });

    return MediaResourceItemView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// VideoForm
// ---------

// Shows an individual project item
define('views/videoform',['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var VideoForm = Backbone.Marionette.ItemView.extend({

        id: 'video-form',

        template: 'VideoForm',

        events: {
            //'click button#embed-video-clear-button': '_clear',
            'click .cancel-button': '_cancel',
            //'click button#embed-video-next-button': '_next',
            'change input#VideoUri': '_updateLink',
            //'change textarea#embed-video-description-input': '_updateDescription',
            'click .close': '_cancel'
        },

        initialize: function (options) {
            //log('embeddedVideoView:initialize', options);
            //this.ValidVideo = false;
            //this.VisiblePreview = false;
            //this.Preview = '';
            this.model.set('MediaType', 'video');
            //this.model.on('change:Metadata', this.onMediaResourceFilesChanged, this);
            this.model.on('change:Files', this.onMediaResourceFilesChanged, this);
        },

        onRender: function () {
            //log('embeddedVideoView:onRender');
            //this._resetView();
            this._showElement($('#modal-dialog'));
            //this._hideElement($('div#embed-video-preview'));
            return this;
        },

        // fire this event when the link has been updated
        _updateLink: function () {
            //log('embeddedVideoView:_updateLink');
            this._previewVideo($('input#VideoUri').val());
        },

        _previewVideo: function (videoUri) {
            this._getYoutubeVideo(videoUri);
            //log('EmbeddedVideo.previewVideo', videoUri);
            //            var that = this;
            //            $.when(this._getVideoPreview(videoUri))
            //                .done(function (data) {
            //                    log(data, data.PreviewTags, data.success);
            //                    if (data.success) {
            //                        that._next(data.PreviewTags);
            //                    }
            //                    //that.Preview = data.PreviewTags;
            //                    //that.ValidVideo = data.success;
            //                });

            //            this.model.set('VideoUri', videoUri);
        },

        _getVideoPreview: function (url) {
            log('EmbeddedVideo.getVideoPreview');
            var deferred = new $.Deferred();
            var params = {};
            params['url'] = url;
            $.ajax({
                url: '/mediaresources/videopreview',
                type: "POST",
                data: params
            }).done(function (data) {
                deferred.resolve(data);
            });
            return deferred.promise();
        },

        _getYoutubeVideo: function (value) {
            var videoId = '';

            /*
            Handles the following URLs:
            http://www.youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index
            http://www.youtube.com/user/IngridMichaelsonVEVO#p/a/u/1/QdK8U-VIH_o
            http://www.youtube.com/v/0zM3nApSvMg?fs=1&amp;hl=en_US&amp;rel=0
            http://www.youtube.com/watch?v=0zM3nApSvMg#t=0m10s
            http://www.youtube.com/embed/0zM3nApSvMg?rel=0
            http://www.youtube.com/watch?v=0zM3nApSvMg
            http://youtu.be/0zM3nApSvMg
            */
            var uriRegExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=)([^#\&\?]*).*/;
            var uriMatch = value.match(uriRegExp);
            if (uriMatch && uriMatch[2].length == 11) {
                videoId = uriMatch[2];
            } else {
                /*
                Handles actual Id being entered
                */
                var idRegExp = /^[A-Za-z0-9_\-]{8,32}$/;
                var idMatch = value.match(idRegExp);
                if (idMatch && idMatch == true) {
                    videoId = value;
                } else {
                    //error
                }
            }

            var that = this;
            $.ajax({
                type: "GET",
                dataType: "jsonp",
                url: 'http://gdata.youtube.com/feeds/api/videos/' + encodeURIComponent(videoId) + '?v=2&alt=json-in-script',
                success: function (obj) {
                    log('youtube video data', obj);
                    that._youtubeFetchDataCallback(obj);
                }
            });


            //            if (/^https?\:\/\/.+/i.test(tempvar)) {
            //                tempvar = /[\?\&]v=([^\?\&]+)/.exec(tempvar);
            //                if (!tempvar) {
            //                    alert('YouTube video URL has a problem!');
            //                    return;
            //                }
            //                videoid = tempvar[1];
            //            }
            //            else {
            //                if (/^[A-Za-z0-9_\-]{8,32}$/.test(tempvar) == false) {
            //                    alert('YouTube video ID has a problem!');
            //                    return;
            //                }
            //                videoid = tempvar;
            //            }
            //$.getScript('http://gdata.youtube.com/feeds/api/videos/' + encodeURIComponent(videoId) + '?v=2&alt=json-in-script&callback=_youtubeFetchDataCallback');

        },

        _youtubeFetchDataCallback: function (data) {
            var s = '';
            s += '<img src="' + data.entry["media$group"]["media$thumbnail"][0].url + '" width="' + data.entry["media$group"]["media$thumbnail"][0].width + '" height="' + data.entry["media$group"]["media$thumbnail"][0].height + '" alt="Default Thumbnail" align="right"/>';
            s += '<b>Title:</b> ' + data.entry["title"].$t + '<br/>';
            s += '<b>Author:</b> ' + data.entry["author"][0].name.$t + '<br/>';
            s += '<b>Published:</b> ' + new Date(data.entry["published"].$t.substr(0, 4), data.entry["published"].$t.substr(5, 2) - 1, data.entry["published"].$t.substr(8, 2)).toLocaleDateString() + '<br/>';
            s += '<b>Duration:</b> ' + Math.floor(data.entry["media$group"]["yt$duration"].seconds / 60) + ':' + (data.entry["media$group"]["yt$duration"].seconds % 60) + ' (' + data.entry["media$group"]["yt$duration"].seconds + ' seconds)<br/>';
            s += '<b>Rating:</b> ' + new Number(data.entry["gd$rating"].average).toFixed(1) + ' out of ' + data.entry["gd$rating"].max + '; ' + data.entry["gd$rating"].numRaters + ' rating(s)' + '<br/>';
            s += '<b>Statistics:</b> ' + data.entry["yt$statistics"].favoriteCount + ' favorite(s); ' + data.entry["yt$statistics"].viewCount + ' view(s)' + '<br/>';
            s += '<br/>' + data.entry["media$group"]["media$description"].$t.replace(/\n/g, '<br/>') + '<br/>';
            s += '<br/><a href="' + data.entry["media$group"]["media$player"].url + '" target="_blank">Watch on YouTube</a>';
            //$('#youtubeDataFetcherOutput').html(s);
            alert(s);
        },

        //        // Fire this event when the description has been updated
        //        _updateDescription: function () {
        //            this.model.set('Description', $('input#embed-video-description-input').val());
        //        },

        //        // remove the current video preview and reset the form..
        //        _clear: function () {
        //            this._resetView();
        //        },

        // no longer adding a new video
        _cancel: function () {
            this._hideElement($('div#modal-dialog'));
            this._cleanup();
        },

        // set form and model back to their original state
        _resetView: function () {
            log('embeddedVideoView:_resetView');
            this.VisiblePreview = false;
            $('input#embed-video-link-input').val('');
            $('#embed-video-description-input').val('');
            $('div#embed-video-player').html('');
            this._hideElement($('div#embed-video-preview'));
        },

        // this button itself is modal, in that click it once to show a preview, 
        // then if preview is visible, click again to save the viewed video.
        _next: function (preview) {
            log('embeddedVideoView:_next');
            // if we've got a valid video, and we've seen it, we are clicking to save
            //            if (this.ValidVideo && this.VisiblePreview) {
            //                this._save();
            //            }
            // if we've loaded a new video, but haven't seen it, display it:
            //else if (this.ValidVideo) {
            //var src = this.Preview;
            //this._showElement($('div#embed-video-preview'));
            this.$el.find('.video-preview').empty().append(ich.VideoPreview({ Width: 520, Height: 400, Source: preview }));
            // now we've seen it, so next click we want to save it.
            //this.VisiblePreview = true;
            //}
        },

        // trigger the editMediaView to save its model with this model in it..
        _save: function () {
            this.model.set('Description', $('#embed-video-description-input').val());
            this.trigger('videouploaded', this.model, this.Preview);
            //this.model.save();
            this._cleanup();
        },

        _cleanup: function () {
            this.remove();
        },

        _hideElement: function (el) {
            $(el).addClass('make-invisible');
            $(el).removeClass('make-visible');
        },

        _showElement: function (el) {
            $(el).addClass('make-visible');
            $(el).removeClass('make-invisible');
        }

    });

    return VideoForm;
});
/*
 * jQuery Iframe Transport Plugin 1.3
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2011, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint unparam: true, nomen: true */
/*global define, window, document */

(function (factory) {
    
    if (typeof define === 'function' && define.amd) {
        // Register as an anonymous AMD module:
        define('iframetransport',['jquery'], factory);
    } else {
        // Browser globals:
        factory(window.jQuery);
    }
}(function ($) {
    

    // Helper variable to create unique names for the transport iframes:
    var counter = 0;

    // The iframe transport accepts three additional options:
    // options.fileInput: a jQuery collection of file input fields
    // options.paramName: the parameter name for the file form data,
    //  overrides the name property of the file input field(s)
    // options.formData: an array of objects with name and value properties,
    //  equivalent to the return data of .serializeArray(), e.g.:
    //  [{name: 'a', value: 1}, {name: 'b', value: 2}]
    $.ajaxTransport('iframe', function (options) {
        if (options.async && (options.type === 'POST' || options.type === 'GET')) {
            var form,
                iframe;
            return {
                send: function (_, completeCallback) {
                    form = $('<form style="display:none;"></form>');
                    // javascript:false as initial iframe src
                    // prevents warning popups on HTTPS in IE6.
                    // IE versions below IE8 cannot set the name property of
                    // elements that have already been added to the DOM,
                    // so we set the name along with the iframe HTML markup:
                    iframe = $(
                        '<iframe src="javascript:false;" name="iframe-transport-' +
                            (counter += 1) + '"></iframe>'
                    ).bind('load', function () {
                        var fileInputClones;
                        iframe
                            .unbind('load')
                            .bind('load', function () {
                                var response;
                                // Wrap in a try/catch block to catch exceptions thrown
                                // when trying to access cross-domain iframe contents:
                                try {
                                    response = iframe.contents();
                                    // Google Chrome and Firefox do not throw an
                                    // exception when calling iframe.contents() on
                                    // cross-domain requests, so we unify the response:
                                    if (!response.length || !response[0].firstChild) {
                                        throw new Error();
                                    }
                                } catch (e) {
                                    response = undefined;
                                }
                                // The complete callback returns the
                                // iframe content document as response object:
                                completeCallback(
                                    200,
                                    'success',
                                    {'iframe': response}
                                );
                                // Fix for IE endless progress bar activity bug
                                // (happens on form submits to iframe targets):
                                $('<iframe src="javascript:false;"></iframe>')
                                    .appendTo(form);
                                form.remove();
                            });
                        form
                            .prop('target', iframe.prop('name'))
                            .prop('action', options.url)
                            .prop('method', options.type);
                        if (options.formData) {
                            $.each(options.formData, function (index, field) {
                                $('<input type="hidden"/>')
                                    .prop('name', field.name)
                                    .val(field.value)
                                    .appendTo(form);
                            });
                        }
                        if (options.fileInput && options.fileInput.length &&
                                options.type === 'POST') {
                            fileInputClones = options.fileInput.clone();
                            // Insert a clone for each file input field:
                            options.fileInput.after(function (index) {
                                return fileInputClones[index];
                            });
                            if (options.paramName) {
                                options.fileInput.each(function () {
                                    $(this).prop('name', options.paramName);
                                });
                            }
                            // Appending the file input fields to the hidden form
                            // removes them from their original location:
                            form
                                .append(options.fileInput)
                                .prop('enctype', 'multipart/form-data')
                                // enctype must be set as encoding for IE:
                                .prop('encoding', 'multipart/form-data');
                        }
                        form.submit();
                        // Insert the file input fields at their original location
                        // by replacing the clones with the originals:
                        if (fileInputClones && fileInputClones.length) {
                            options.fileInput.each(function (index, input) {
                                var clone = $(fileInputClones[index]);
                                $(input).prop('name', clone.prop('name'));
                                clone.replaceWith(input);
                            });
                        }
                    });
                    form.append(iframe).appendTo(document.body);
                },
                abort: function () {
                    if (iframe) {
                        // javascript:false as iframe src aborts the request
                        // and prevents warning popups on HTTPS in IE6.
                        // concat is used to avoid the "Script URL" JSLint error:
                        iframe
                            .unbind('load')
                            .prop('src', 'javascript'.concat(':false;'));
                    }
                    if (form) {
                        form.remove();
                    }
                }
            };
        }
    });

    // The iframe transport returns the iframe content document as response.
    // The following adds converters from iframe to text, json, html, and script:
    $.ajaxSetup({
        converters: {
            'iframe text': function (iframe) {
                return $(iframe[0].body).text();
            },
            'iframe json': function (iframe) {
                return $.parseJSON($(iframe[0].body).text());
            },
            'iframe html': function (iframe) {
                return $(iframe[0].body).html();
            },
            'iframe script': function (iframe) {
                return $.globalEval($(iframe[0].body).text());
            }
        }
    });

}));

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMediaView
// -------------

// View that allows user to choose location on a mpa or via coordinates
define('views/editmediaview',['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/mediaresourceitemview', 'views/videoform', 'loadimage', 'fileupload', 'iframetransport'],
function ($, _, Backbone, app, MediaResource, MediaResourceItemView, VideoForm, loadImage) {
    var EditMediaView = Backbone.View.extend({

        id: 'media-resources-fieldset',

        events: {
            'click #youtube-upload-button': '_showEmbeddedVideo'
        },

        initialize: function (options) {
            log('editMediaView:initialize');
            _.extend(this, Backbone.Events);
            _.bindAll(this,
                'render',
                '_initMediaUploader',
                '_onImageUploadDone',
                '_onSubmitImageUpload',
                '_onImageUploadAdd');
            this.mediaResourceItemViews = [];
            this.currentUploadKey = '';

            app.vent.on('videomediaresourceuploaded:', this._onVideoUploadDone, this);
            app.vent.on('imagemediaresourceuploaded:', this._onImageUploadDone, this);
        },

        render: function () {
            log('ediMediaView:render');
            this._initMediaUploader();
            return this;
        },

        _initMediaUploader: function () {
            log('ediMediaView:_initMediaUploader');
            this.$el.find('#file').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources',
                add: this._onImageUploadAdd,
                submit: this._onSubmitImageUpload
            });
        },

        filesAdded: 0,  // Used to determine when to fire file upload animations

        _onImageUploadAdd: function (e, data) {
            this.$el.find('.upload-progress').show();

            log('editMediaView:_onImageUploadAdd');
            this.currentUploadKey = app.generateGuid();
            var mediaResource = new MediaResource({ Key: this.currentUploadKey });
            this.model.addMediaResource(mediaResource);

            var mediaResourceItemView = new MediaResourceItemView({ model: mediaResource });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);

            var self = this;
            var tempImage = null;

            if (!window.isIEFail) {
                tempImage = loadImage(
                    data.files[0],
                    function (img) {
                        if (img.type === "error") {
                            //log('Error loading image', img);
                        } else {
                            self.filesAdded++;
                            mediaResourceItemView.showTempImageMedia(img);
                            self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), self.filesAdded === data.originalFiles.length);
                        }
                    },
                    { maxHeight: 220 }
                );
            }

            if (!tempImage) {
                $(mediaResourceItemView.el).width(280);
                this.filesAdded++;
                this._showMediaResourceItemView(this, mediaResourceItemView, 280, this.filesAdded === data.originalFiles.length);
            }

            data.submit();
        },

        _showEmbeddedVideo: function (e) {
            e.preventDefault();
            $('body').append('<div id="modal-dialog" class="make-invisible"></div>');
            var videoForm = new VideoForm({ el: $('#modal-dialog'), model: new MediaResource() });
            videoForm.on('videouploaded', this._videoUploadAdd, this);
            videoForm.render();
        },

        // once we have a previewable video from the host..
        _videoUploadAdd: function (data, preview) { // data in this case is a MediaResource
            this.$el.find('.upload-progress').show();
            log('editMediaView:_onVideoUploadAdd');
            this.currentUploadKey = app.generateGuid();
            data.set('Key', this.currentUploadKey);
            this.model.addMediaResource(data);


            var mediaResourceItemView = new MediaResourceItemView({ model: data });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);
            this.$el.find('.media-resource-items').append(mediaResourceItemView.render().el);

            this.filesAdded++;
            mediaResourceItemView.showVideoMedia(preview);
            this._showMediaResourceItemView(this, mediaResourceItemView, 280, true);

            data.save();
        },

        _showMediaResourceItemView: function (self, mediaResourceItemView, imageWidth, beginAnimation) {
            log('ediMediaView:_showMediaResourceItemView');
            self.$el.find('#media-resource-items')
                .queue('mediaQueue', function (next) {

                    var itemCount = self.$el.find('#media-resource-items > div').length;
                    self.$el.find('#media-resource-items').append(mediaResourceItemView.render().el);

                    if (self.$el.find('#media-resource-items').innerWidth() + self.$el.find('#media-resource-items').scrollLeft() === self.$el.find('#media-resource-items').get(0).scrollWidth) {
                        // Don't do any animation
                        next();
                    }
                    else {
                        var x = self.$el.find('#media-resource-items').get(0).scrollWidth - (self.$el.find('#media-resource-items').innerWidth() + self.$el.find('#media-resource-items').scrollLeft());
                        // Make space for the new item
                        $('#media-resource-items').animate({ scrollLeft: '+=' + x.toString() }, 500, 'swing', next);
                    }
                })
                .queue('mediaQueue', function (next) {
                    $(mediaResourceItemView.el)
                        .animate(
                        { top: '+=250px' },
                        {
                            duration: 800,
                            easing: 'swing',
                            queue: false,
                            complete: next
                        });
                })
                .queue('mediaQueue', function (next) {
                    $(mediaResourceItemView.el).css({ position: 'relative', top: '' });
                    $('#media-resource-items').animate({ scrollLeft: 10000 }, 1);
                    next();
                });

            if (beginAnimation) {
                log('beginnning animation');
                self.filesAdded = 0;
                self.$el.find('#media-resource-items').dequeue('mediaQueue');

                this.$el.find('.upload-progress').hide();
            }
        },

        _onMediaResourceViewRemove: function (model, view) {
            log('ediMediaView:_onMediaResourceViewRemove');
            //            var addToRemoveList = false;
            //            if (app.get('newObservation').mediaResources.find(function (mr) { return mr.id == this.model.id; }) != null) {
            //                addToRemoveList = true;
            //            }
            //            app.get('newObservation').addMediaResources.remove(this.model.id);
            //            app.get('newObservation').mediaResources.remove(this.model.id);
            //            if (addToRemoveList) {
            //                app.get('newObservation').removeMediaResources.add(this.model);
            //            }
            this.model.removeMediaResource(model.id);
            view.remove();
        },

        _onSubmitImageUpload: function (e, data) {
            log('ediMediaView:_onSubmitImageUpload');
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name, MediaType: 'image', Usage: 'observation' };
            if (window.isIEFail) {
                data.formData.ie = true;
            }
        },

        _onImageUploadDone: function (data) {
            log('ediMediaView:_onImageUploadDone', this.model);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') == data.Key;
            });
            mediaResource.set(data);
        },

        // when we get an uploaded video back from the server, update the id of the mediaresource
        _onVideoUploadDone: function (data) {
            log('ediMediaView:_onVideoUploadDone', data);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') == data.Key;
            });
            log('mediaResource found: ', mediaResource);
            mediaResource.set(data);
            log('Video Upload set model to: ', data);
        }
    });

    return EditMediaView;

});
define('datepicker',['jquery', 'date'], function (jQuery) {
    /* ===========================================================
    * bootstrap-datepicker.js v1.3.0
    * http://twitter.github.com/bootstrap/javascript.html#datepicker
    * ===========================================================
    * Copyright 2011 Twitter, Inc.
    *
    * Licensed under the Apache License, Version 2.0 (the "License");
    * you may not use this file except in compliance with the License.
    * You may obtain a copy of the License at
    *
    * http://www.apache.org/licenses/LICENSE-2.0
    *
    * Unless required by applicable law or agreed to in writing, software
    * distributed under the License is distributed on an "AS IS" BASIS,
    * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    * See the License for the specific language governing permissions and
    * limitations under the License.
    *
    * Contributed by Scott Torborg - github.com/storborg
    * Loosely based on jquery.date_input.js by Jon Leighton, heavily updated and
    * rewritten to match bootstrap javascript approach and add UI features.
    * =========================================================== */


    !function ($) {

        var selector = '[data-datepicker]',
      all = [];

        function clearDatePickers(except) {
            var ii;
            for (ii = 0; ii < all.length; ii++) {
                if (all[ii] != except) {
                    all[ii].hide();
                }
            }
        }

        function DatePicker(element, options) {
            this.$el = $(element);
            this.proxy('show').proxy('ahead').proxy('hide').proxy('keyHandler').proxy('selectDate');

            var options = $.extend({}, $.fn.datepicker.defaults, options);

            if ((!!options.parse) || (!!options.format) || !this.detectNative()) {
                $.extend(this, options);
                this.$el.data('datepicker', this);
                all.push(this);
                this.init();
            }
        }

        DatePicker.prototype = {

            detectNative: function (el) {
                // Attempt to activate the native datepicker, if there is a known good
                // one. If successful, return true. Note that input type="date"
                // requires that the string be RFC3339, so if the format/parse methods
                // have been overridden, this won't be used.
                if (navigator.userAgent.match(/(iPad|iPhone); CPU(\ iPhone)? OS 5_\d/i)) {
                    // jQuery will only change the input type of a detached element.
                    var $marker = $('<span>').insertBefore(this.$el);
                    this.$el.detach().attr('type', 'date').insertAfter($marker);
                    $marker.remove();
                    return true;
                }
                return false;
            }

    , init: function () {
        var $months = this.nav('months', 1);
        var $years = this.nav('years', 12);

        var $nav = $('<div>').addClass('nav').append($months, $years);

        this.$month = $('.name', $months);
        this.$year = $('.name', $years);

        $calendar = $("<div>").addClass('calendar');

        // Populate day of week headers, realigned by startOfWeek.
        for (var i = 0; i < this.shortDayNames.length; i++) {
            $calendar.append('<div class="dow">' + this.shortDayNames[(i + this.startOfWeek) % 7] + '</div>');
        };

        this.$days = $('<div>').addClass('days');
        $calendar.append(this.$days);

        this.$picker = $('<div>')
          .click(function (e) { e.stopPropagation() })
        // Use this to prevent accidental text selection.
          .mousedown(function (e) { e.preventDefault() })
          .addClass('datepicker')
          .append($nav, $calendar)
          .insertAfter(this.$el);

        this.$el
          .focus(this.show)
          .click(this.show)
          .change($.proxy(function () { this.selectDate(); }, this));

        this.selectDate();
        this.hide();
    }

    , nav: function (c, months) {
        var $subnav = $('<div>' +
                          '<button type="button" class="prev button"><i></i> &larr;</button>' + //<span class="prev button"><i></i> &larr;</span>
                          '<span class="name"></span>' +
                          '<button type="button" class="next button"><i></i> &rarr;</button>' + //<span class="next button"><i></i> &rarr;</span>
                        '</div>').addClass(c)
        $('.prev', $subnav).click($.proxy(function () { this.ahead(-months, 0) }, this));
        $('.next', $subnav).click($.proxy(function () { this.ahead(months, 0) }, this));
        return $subnav;

    }

    , updateName: function ($area, s) {
        // Update either the month or year field, with a background flash
        // animation.
        var cur = $area.find('.fg').text(),
            $fg = $('<div>').addClass('fg').append(s);
        $area.empty();
        if (cur != s) {
            var $bg = $('<div>').addClass('bg');
            $area.append($bg, $fg);
            $bg.fadeOut('slow', function () {
                $(this).remove();
            });
        } else {
            $area.append($fg);
        }
    }

    , selectMonth: function (date) {
        var newMonth = new Date(date.getFullYear(), date.getMonth(), 1);

        if (!this.curMonth || !(this.curMonth.getFullYear() == newMonth.getFullYear() &&
                                this.curMonth.getMonth() == newMonth.getMonth())) {

            this.curMonth = newMonth;

            var rangeStart = this.rangeStart(date), rangeEnd = this.rangeEnd(date);
            var num_days = this.daysBetween(rangeStart, rangeEnd);
            this.$days.empty();

            for (var ii = 0; ii <= num_days; ii++) {
                var thisDay = new Date(rangeStart.getFullYear(), rangeStart.getMonth(), rangeStart.getDate() + ii, 12, 00);
                var $day = $('<div>').attr('date', this.format(thisDay));
                $day.text(thisDay.getDate());

                if (thisDay.getMonth() != date.getMonth()) {
                    $day.addClass('overlap');
                };

                this.$days.append($day);
            };

            this.updateName(this.$month, this.monthNames[date.getMonth()]);
            this.updateName(this.$year, this.curMonth.getFullYear());

            $('div', this.$days).click($.proxy(function (e) {
                var $targ = $(e.target);

                // The date= attribute is used here to provide relatively fast
                // selectors for setting certain date cells.
                this.update($targ.attr("date"));

                // Don't consider this selection final if we're just going to an
                // adjacent month.
                if (!$targ.hasClass('overlap')) {
                    this.hide();
                }

            }, this));

            $("[date='" + this.format(new Date()) + "']", this.$days).addClass('today');

        };

        $('.selected', this.$days).removeClass('selected');
        $('[date="' + this.selectedDateStr + '"]', this.$days).addClass('selected');
    }

    , selectDate: function (date) {
        if (typeof (date) == "undefined") {
            date = this.parse(this.$el.val());
        };
        if (!date) date = new Date();

        this.selectedDate = date;
        this.selectedDateStr = this.format(this.selectedDate);
        this.selectMonth(this.selectedDate);
    }

    , update: function (s) {
        this.$el.val(s).change();
    }

    , show: function (e) {
        e && e.stopPropagation();

        // Hide all other datepickers.
        clearDatePickers(this);

        var offset = this.$el.offset();

        //        this.$picker.css({
        //            marginLeft: (this.$picker.outerWidth() - (this.$picker.outerWidth() * 2) + this.$el.outerWidth()) - 1
        //        }).show();

        this.$picker.show();

        $('html').on('keydown', this.keyHandler);
    }

    , hide: function () {
        this.$picker.hide();
        $('html').off('keydown', this.keyHandler);
    }

    , keyHandler: function (e) {
        // Keyboard navigation shortcuts.
        switch (e.keyCode) {
            case 9:
            case 27:
                // Tab or escape hides the datepicker. In this case, just return
                // instead of breaking, so that the e doesn't get stopped.
                this.hide(); return;
            case 13:
                // Enter selects the currently highlighted date.
                this.update(this.selectedDateStr); this.hide(); break;
            case 38:
                // Arrow up goes to prev week.
                this.ahead(0, -7); break;
            case 40:
                // Arrow down goes to next week.
                this.ahead(0, 7); break;
            case 37:
                // Arrow left goes to prev day.
                this.ahead(0, -1); break;
            case 39:
                // Arrow right goes to next day.
                this.ahead(0, 1); break;
            default:
                return;
        }
        e.preventDefault();
    }

    , parse: function (s) {
        //        if (Date.isValid(s, 'd MMM yyyy')) {
        //            return Date.parseString(s, 'd MMM yyyy');
        //        }
        //return null;
        return Date.parseExact(s, 'd MMM yyyy');
    }

    , format: function (date) {
        //return date.format('d MMM yyyy');
        return date.toString('d MMM yyyy');
    }

    , ahead: function (months, days) {
        // Move ahead ``months`` months and ``days`` days, both integers, can be
        // negative.
        this.selectDate(new Date(this.selectedDate.getFullYear(),
                                 this.selectedDate.getMonth() + months,
                                 this.selectedDate.getDate() + days));
    }

    , proxy: function (meth) {
        // Bind a method so that it always gets the datepicker instance for
        // ``this``. Return ``this`` so chaining calls works.
        this[meth] = $.proxy(this[meth], this);
        return this;
    }

    , daysBetween: function (start, end) {
        // Return number of days between ``start`` Date object and ``end``.
        var start = Date.UTC(start.getFullYear(), start.getMonth(), start.getDate());
        var end = Date.UTC(end.getFullYear(), end.getMonth(), end.getDate());
        return (end - start) / 86400000;
    }

    , findClosest: function (dow, date, direction) {
        // From a starting date, find the first day ahead of behind it that is
        // a given day of the week.
        var difference = direction * (Math.abs(date.getDay() - dow - (direction * 7)) % 7);
        return new Date(date.getFullYear(), date.getMonth(), date.getDate() + difference);
    }

    , rangeStart: function (date) {
        // Get the first day to show in the current calendar view.
        return this.findClosest(this.startOfWeek,
                                new Date(date.getFullYear(), date.getMonth()),
                                -1);
    }

    , rangeEnd: function (date) {
        // Get the last day to show in the current calendar view.
        return this.findClosest((this.startOfWeek - 1) % 7,
                                new Date(date.getFullYear(), date.getMonth() + 1, 0),
                                1);
    }
        };

        /* DATEPICKER PLUGIN DEFINITION
        * ============================ */

        $.fn.datepicker = function (options) {
            return this.each(function () { new DatePicker(this, options); });
        };

        $(function () {
            $(selector).datepicker();
            $('html').click(clearDatePickers);
        });

        $.fn.datepicker.DatePicker = DatePicker;

        $.fn.datepicker.defaults = {
            monthNames: ["January", "February", "March", "April", "May", "June",
                 "July", "August", "September", "October", "November", "December"]
  , shortDayNames: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]
  , startOfWeek: 1
        };
    } (window.jQuery || window.ender);

});
define('jqueryui/dialog', ['jquery','jqueryui/core','jqueryui/widget'], function (jQuery) {
/*!
 * jQuery UI Dialog 1.8.21
 *
 * Copyright 2012, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Dialog
 *
 * Depends:
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *  jquery.ui.button.js
 *	jquery.ui.draggable.js
 *	jquery.ui.mouse.js
 *	jquery.ui.position.js
 *	jquery.ui.resizable.js
 */
(function( $, undefined ) {

var uiDialogClasses =
		'ui-dialog ' +
		'ui-widget ' +
		'ui-widget-content ' +
		'ui-corner-all ',
	sizeRelatedOptions = {
		buttons: true,
		height: true,
		maxHeight: true,
		maxWidth: true,
		minHeight: true,
		minWidth: true,
		width: true
	},
	resizableRelatedOptions = {
		maxHeight: true,
		maxWidth: true,
		minHeight: true,
		minWidth: true
	},
	// support for jQuery 1.3.2 - handle common attrFn methods for dialog
	attrFn = $.attrFn || {
		val: true,
		css: true,
		html: true,
		text: true,
		data: true,
		width: true,
		height: true,
		offset: true,
		click: true
	};

$.widget("ui.dialog", {
	options: {
		autoOpen: true,
		buttons: {},
		closeOnEscape: true,
		closeText: 'close',
		dialogClass: '',
		draggable: true,
		hide: null,
		height: 500,
		maxHeight: false,
		maxWidth: false,
		minHeight: 150,
		minWidth: 150,
		modal: false,
		position: {
			my: 'center',
			at: 'center',
			collision: 'fit',
			// ensure that the titlebar is never outside the document
			using: function(pos) {
				var topOffset = $(this).css(pos).offset().top;
				if (topOffset < 0) {
					$(this).css('top', pos.top - topOffset);
				}
			}
		},
		resizable: true,
		show: null,
		stack: true,
		title: '',
		width: 300,
		zIndex: 1000
	},

	_create: function() {
		this.originalTitle = this.element.attr('title');
		// #5742 - .attr() might return a DOMElement
		if ( typeof this.originalTitle !== "string" ) {
			this.originalTitle = "";
		}

		this.options.title = this.options.title || this.originalTitle;
		var self = this,
			options = self.options,

			title = options.title || '&#160;',
			titleId = $.ui.dialog.getTitleId(self.element),

			uiDialog = (self.uiDialog = $('<div></div>'))
				.appendTo(document.body)
				.hide()
				.addClass(uiDialogClasses + options.dialogClass)
				.css({
					zIndex: options.zIndex
				})
				// setting tabIndex makes the div focusable
				// setting outline to 0 prevents a border on focus in Mozilla
				.attr('tabIndex', -1).css('outline', 0).keydown(function(event) {
					if (options.closeOnEscape && !event.isDefaultPrevented() && event.keyCode &&
						event.keyCode === $.ui.keyCode.ESCAPE) {
						
						self.close(event);
						event.preventDefault();
					}
				})
				.attr({
					role: 'dialog',
					'aria-labelledby': titleId
				})
				.mousedown(function(event) {
					self.moveToTop(false, event);
				}),

			uiDialogContent = self.element
				.show()
				.removeAttr('title')
				.addClass(
					'ui-dialog-content ' +
					'ui-widget-content')
				.appendTo(uiDialog),

			uiDialogTitlebar = (self.uiDialogTitlebar = $('<div></div>'))
				.addClass(
					'ui-dialog-titlebar ' +
					'ui-widget-header ' +
					'ui-corner-all ' +
					'ui-helper-clearfix'
				)
				.prependTo(uiDialog),

			uiDialogTitlebarClose = $('<a href="#"></a>')
				.addClass(
					'ui-dialog-titlebar-close ' +
					'ui-corner-all'
				)
				.attr('role', 'button')
				.hover(
					function() {
						uiDialogTitlebarClose.addClass('ui-state-hover');
					},
					function() {
						uiDialogTitlebarClose.removeClass('ui-state-hover');
					}
				)
				.focus(function() {
					uiDialogTitlebarClose.addClass('ui-state-focus');
				})
				.blur(function() {
					uiDialogTitlebarClose.removeClass('ui-state-focus');
				})
				.click(function(event) {
					self.close(event);
					return false;
				})
				.appendTo(uiDialogTitlebar),

			uiDialogTitlebarCloseText = (self.uiDialogTitlebarCloseText = $('<span></span>'))
				.addClass(
					'ui-icon ' +
					'ui-icon-closethick'
				)
				.text(options.closeText)
				.appendTo(uiDialogTitlebarClose),

			uiDialogTitle = $('<span></span>')
				.addClass('ui-dialog-title')
				.attr('id', titleId)
				.html(title)
				.prependTo(uiDialogTitlebar);

		//handling of deprecated beforeclose (vs beforeClose) option
		//Ticket #4669 http://dev.jqueryui.com/ticket/4669
		//TODO: remove in 1.9pre
		if ($.isFunction(options.beforeclose) && !$.isFunction(options.beforeClose)) {
			options.beforeClose = options.beforeclose;
		}

		uiDialogTitlebar.find("*").add(uiDialogTitlebar).disableSelection();

		if (options.draggable && $.fn.draggable) {
			self._makeDraggable();
		}
		if (options.resizable && $.fn.resizable) {
			self._makeResizable();
		}

		self._createButtons(options.buttons);
		self._isOpen = false;

		if ($.fn.bgiframe) {
			uiDialog.bgiframe();
		}
	},

	_init: function() {
		if ( this.options.autoOpen ) {
			this.open();
		}
	},

	destroy: function() {
		var self = this;
		
		if (self.overlay) {
			self.overlay.destroy();
		}
		self.uiDialog.hide();
		self.element
			.unbind('.dialog')
			.removeData('dialog')
			.removeClass('ui-dialog-content ui-widget-content')
			.hide().appendTo('body');
		self.uiDialog.remove();

		if (self.originalTitle) {
			self.element.attr('title', self.originalTitle);
		}

		return self;
	},

	widget: function() {
		return this.uiDialog;
	},

	close: function(event) {
		var self = this,
			maxZ, thisZ;
		
		if (false === self._trigger('beforeClose', event)) {
			return;
		}

		if (self.overlay) {
			self.overlay.destroy();
		}
		self.uiDialog.unbind('keypress.ui-dialog');

		self._isOpen = false;

		if (self.options.hide) {
			self.uiDialog.hide(self.options.hide, function() {
				self._trigger('close', event);
			});
		} else {
			self.uiDialog.hide();
			self._trigger('close', event);
		}

		$.ui.dialog.overlay.resize();

		// adjust the maxZ to allow other modal dialogs to continue to work (see #4309)
		if (self.options.modal) {
			maxZ = 0;
			$('.ui-dialog').each(function() {
				if (this !== self.uiDialog[0]) {
					thisZ = $(this).css('z-index');
					if(!isNaN(thisZ)) {
						maxZ = Math.max(maxZ, thisZ);
					}
				}
			});
			$.ui.dialog.maxZ = maxZ;
		}

		return self;
	},

	isOpen: function() {
		return this._isOpen;
	},

	// the force parameter allows us to move modal dialogs to their correct
	// position on open
	moveToTop: function(force, event) {
		var self = this,
			options = self.options,
			saveScroll;

		if ((options.modal && !force) ||
			(!options.stack && !options.modal)) {
			return self._trigger('focus', event);
		}

		if (options.zIndex > $.ui.dialog.maxZ) {
			$.ui.dialog.maxZ = options.zIndex;
		}
		if (self.overlay) {
			$.ui.dialog.maxZ += 1;
			self.overlay.$el.css('z-index', $.ui.dialog.overlay.maxZ = $.ui.dialog.maxZ);
		}

		//Save and then restore scroll since Opera 9.5+ resets when parent z-Index is changed.
		//  http://ui.jquery.com/bugs/ticket/3193
		saveScroll = { scrollTop: self.element.scrollTop(), scrollLeft: self.element.scrollLeft() };
		$.ui.dialog.maxZ += 1;
		self.uiDialog.css('z-index', $.ui.dialog.maxZ);
		self.element.attr(saveScroll);
		self._trigger('focus', event);

		return self;
	},

	open: function() {
		if (this._isOpen) { return; }

		var self = this,
			options = self.options,
			uiDialog = self.uiDialog;

		self.overlay = options.modal ? new $.ui.dialog.overlay(self) : null;
		self._size();
		self._position(options.position);
		uiDialog.show(options.show);
		self.moveToTop(true);

		// prevent tabbing out of modal dialogs
		if ( options.modal ) {
			uiDialog.bind( "keydown.ui-dialog", function( event ) {
				if ( event.keyCode !== $.ui.keyCode.TAB ) {
					return;
				}

				var tabbables = $(':tabbable', this),
					first = tabbables.filter(':first'),
					last  = tabbables.filter(':last');

				if (event.target === last[0] && !event.shiftKey) {
					first.focus(1);
					return false;
				} else if (event.target === first[0] && event.shiftKey) {
					last.focus(1);
					return false;
				}
			});
		}

		// set focus to the first tabbable element in the content area or the first button
		// if there are no tabbable elements, set focus on the dialog itself
		$(self.element.find(':tabbable').get().concat(
			uiDialog.find('.ui-dialog-buttonpane :tabbable').get().concat(
				uiDialog.get()))).eq(0).focus();

		self._isOpen = true;
		self._trigger('open');

		return self;
	},

	_createButtons: function(buttons) {
		var self = this,
			hasButtons = false,
			uiDialogButtonPane = $('<div></div>')
				.addClass(
					'ui-dialog-buttonpane ' +
					'ui-widget-content ' +
					'ui-helper-clearfix'
				),
			uiButtonSet = $( "<div></div>" )
				.addClass( "ui-dialog-buttonset" )
				.appendTo( uiDialogButtonPane );

		// if we already have a button pane, remove it
		self.uiDialog.find('.ui-dialog-buttonpane').remove();

		if (typeof buttons === 'object' && buttons !== null) {
			$.each(buttons, function() {
				return !(hasButtons = true);
			});
		}
		if (hasButtons) {
			$.each(buttons, function(name, props) {
				props = $.isFunction( props ) ?
					{ click: props, text: name } :
					props;
				var button = $('<button type="button"></button>')
					.click(function() {
						props.click.apply(self.element[0], arguments);
					})
					.appendTo(uiButtonSet);
				// can't use .attr( props, true ) with jQuery 1.3.2.
				$.each( props, function( key, value ) {
					if ( key === "click" ) {
						return;
					}
					if ( key in attrFn ) {
						button[ key ]( value );
					} else {
						button.attr( key, value );
					}
				});
				if ($.fn.button) {
					button.button();
				}
			});
			uiDialogButtonPane.appendTo(self.uiDialog);
		}
	},

	_makeDraggable: function() {
		var self = this,
			options = self.options,
			doc = $(document),
			heightBeforeDrag;

		function filteredUi(ui) {
			return {
				position: ui.position,
				offset: ui.offset
			};
		}

		self.uiDialog.draggable({
			cancel: '.ui-dialog-content, .ui-dialog-titlebar-close',
			handle: '.ui-dialog-titlebar',
			containment: 'document',
			start: function(event, ui) {
				heightBeforeDrag = options.height === "auto" ? "auto" : $(this).height();
				$(this).height($(this).height()).addClass("ui-dialog-dragging");
				self._trigger('dragStart', event, filteredUi(ui));
			},
			drag: function(event, ui) {
				self._trigger('drag', event, filteredUi(ui));
			},
			stop: function(event, ui) {
				options.position = [ui.position.left - doc.scrollLeft(),
					ui.position.top - doc.scrollTop()];
				$(this).removeClass("ui-dialog-dragging").height(heightBeforeDrag);
				self._trigger('dragStop', event, filteredUi(ui));
				$.ui.dialog.overlay.resize();
			}
		});
	},

	_makeResizable: function(handles) {
		handles = (handles === undefined ? this.options.resizable : handles);
		var self = this,
			options = self.options,
			// .ui-resizable has position: relative defined in the stylesheet
			// but dialogs have to use absolute or fixed positioning
			position = self.uiDialog.css('position'),
			resizeHandles = (typeof handles === 'string' ?
				handles	:
				'n,e,s,w,se,sw,ne,nw'
			);

		function filteredUi(ui) {
			return {
				originalPosition: ui.originalPosition,
				originalSize: ui.originalSize,
				position: ui.position,
				size: ui.size
			};
		}

		self.uiDialog.resizable({
			cancel: '.ui-dialog-content',
			containment: 'document',
			alsoResize: self.element,
			maxWidth: options.maxWidth,
			maxHeight: options.maxHeight,
			minWidth: options.minWidth,
			minHeight: self._minHeight(),
			handles: resizeHandles,
			start: function(event, ui) {
				$(this).addClass("ui-dialog-resizing");
				self._trigger('resizeStart', event, filteredUi(ui));
			},
			resize: function(event, ui) {
				self._trigger('resize', event, filteredUi(ui));
			},
			stop: function(event, ui) {
				$(this).removeClass("ui-dialog-resizing");
				options.height = $(this).height();
				options.width = $(this).width();
				self._trigger('resizeStop', event, filteredUi(ui));
				$.ui.dialog.overlay.resize();
			}
		})
		.css('position', position)
		.find('.ui-resizable-se').addClass('ui-icon ui-icon-grip-diagonal-se');
	},

	_minHeight: function() {
		var options = this.options;

		if (options.height === 'auto') {
			return options.minHeight;
		} else {
			return Math.min(options.minHeight, options.height);
		}
	},

	_position: function(position) {
		var myAt = [],
			offset = [0, 0],
			isVisible;

		if (position) {
			// deep extending converts arrays to objects in jQuery <= 1.3.2 :-(
	//		if (typeof position == 'string' || $.isArray(position)) {
	//			myAt = $.isArray(position) ? position : position.split(' ');

			if (typeof position === 'string' || (typeof position === 'object' && '0' in position)) {
				myAt = position.split ? position.split(' ') : [position[0], position[1]];
				if (myAt.length === 1) {
					myAt[1] = myAt[0];
				}

				$.each(['left', 'top'], function(i, offsetPosition) {
					if (+myAt[i] === myAt[i]) {
						offset[i] = myAt[i];
						myAt[i] = offsetPosition;
					}
				});

				position = {
					my: myAt.join(" "),
					at: myAt.join(" "),
					offset: offset.join(" ")
				};
			} 

			position = $.extend({}, $.ui.dialog.prototype.options.position, position);
		} else {
			position = $.ui.dialog.prototype.options.position;
		}

		// need to show the dialog to get the actual offset in the position plugin
		isVisible = this.uiDialog.is(':visible');
		if (!isVisible) {
			this.uiDialog.show();
		}
		this.uiDialog
			// workaround for jQuery bug #5781 http://dev.jquery.com/ticket/5781
			.css({ top: 0, left: 0 })
			.position($.extend({ of: window }, position));
		if (!isVisible) {
			this.uiDialog.hide();
		}
	},

	_setOptions: function( options ) {
		var self = this,
			resizableOptions = {},
			resize = false;

		$.each( options, function( key, value ) {
			self._setOption( key, value );
			
			if ( key in sizeRelatedOptions ) {
				resize = true;
			}
			if ( key in resizableRelatedOptions ) {
				resizableOptions[ key ] = value;
			}
		});

		if ( resize ) {
			this._size();
		}
		if ( this.uiDialog.is( ":data(resizable)" ) ) {
			this.uiDialog.resizable( "option", resizableOptions );
		}
	},

	_setOption: function(key, value){
		var self = this,
			uiDialog = self.uiDialog;

		switch (key) {
			//handling of deprecated beforeclose (vs beforeClose) option
			//Ticket #4669 http://dev.jqueryui.com/ticket/4669
			//TODO: remove in 1.9pre
			case "beforeclose":
				key = "beforeClose";
				break;
			case "buttons":
				self._createButtons(value);
				break;
			case "closeText":
				// ensure that we always pass a string
				self.uiDialogTitlebarCloseText.text("" + value);
				break;
			case "dialogClass":
				uiDialog
					.removeClass(self.options.dialogClass)
					.addClass(uiDialogClasses + value);
				break;
			case "disabled":
				if (value) {
					uiDialog.addClass('ui-dialog-disabled');
				} else {
					uiDialog.removeClass('ui-dialog-disabled');
				}
				break;
			case "draggable":
				var isDraggable = uiDialog.is( ":data(draggable)" );
				if ( isDraggable && !value ) {
					uiDialog.draggable( "destroy" );
				}
				
				if ( !isDraggable && value ) {
					self._makeDraggable();
				}
				break;
			case "position":
				self._position(value);
				break;
			case "resizable":
				// currently resizable, becoming non-resizable
				var isResizable = uiDialog.is( ":data(resizable)" );
				if (isResizable && !value) {
					uiDialog.resizable('destroy');
				}

				// currently resizable, changing handles
				if (isResizable && typeof value === 'string') {
					uiDialog.resizable('option', 'handles', value);
				}

				// currently non-resizable, becoming resizable
				if (!isResizable && value !== false) {
					self._makeResizable(value);
				}
				break;
			case "title":
				// convert whatever was passed in o a string, for html() to not throw up
				$(".ui-dialog-title", self.uiDialogTitlebar).html("" + (value || '&#160;'));
				break;
		}

		$.Widget.prototype._setOption.apply(self, arguments);
	},

	_size: function() {
		/* If the user has resized the dialog, the .ui-dialog and .ui-dialog-content
		 * divs will both have width and height set, so we need to reset them
		 */
		var options = this.options,
			nonContentHeight,
			minContentHeight,
			isVisible = this.uiDialog.is( ":visible" );

		// reset content sizing
		this.element.show().css({
			width: 'auto',
			minHeight: 0,
			height: 0
		});

		if (options.minWidth > options.width) {
			options.width = options.minWidth;
		}

		// reset wrapper sizing
		// determine the height of all the non-content elements
		nonContentHeight = this.uiDialog.css({
				height: 'auto',
				width: options.width
			})
			.height();
		minContentHeight = Math.max( 0, options.minHeight - nonContentHeight );
		
		if ( options.height === "auto" ) {
			// only needed for IE6 support
			if ( $.support.minHeight ) {
				this.element.css({
					minHeight: minContentHeight,
					height: "auto"
				});
			} else {
				this.uiDialog.show();
				var autoHeight = this.element.css( "height", "auto" ).height();
				if ( !isVisible ) {
					this.uiDialog.hide();
				}
				this.element.height( Math.max( autoHeight, minContentHeight ) );
			}
		} else {
			this.element.height( Math.max( options.height - nonContentHeight, 0 ) );
		}

		if (this.uiDialog.is(':data(resizable)')) {
			this.uiDialog.resizable('option', 'minHeight', this._minHeight());
		}
	}
});

$.extend($.ui.dialog, {
	version: "1.8.21",

	uuid: 0,
	maxZ: 0,

	getTitleId: function($el) {
		var id = $el.attr('id');
		if (!id) {
			this.uuid += 1;
			id = this.uuid;
		}
		return 'ui-dialog-title-' + id;
	},

	overlay: function(dialog) {
		this.$el = $.ui.dialog.overlay.create(dialog);
	}
});

$.extend($.ui.dialog.overlay, {
	instances: [],
	// reuse old instances due to IE memory leak with alpha transparency (see #5185)
	oldInstances: [],
	maxZ: 0,
	events: $.map('focus,mousedown,mouseup,keydown,keypress,click'.split(','),
		function(event) { return event + '.dialog-overlay'; }).join(' '),
	create: function(dialog) {
		if (this.instances.length === 0) {
			// prevent use of anchors and inputs
			// we use a setTimeout in case the overlay is created from an
			// event that we're going to be cancelling (see #2804)
			setTimeout(function() {
				// handle $(el).dialog().dialog('close') (see #4065)
				if ($.ui.dialog.overlay.instances.length) {
					$(document).bind($.ui.dialog.overlay.events, function(event) {
						// stop events if the z-index of the target is < the z-index of the overlay
						// we cannot return true when we don't want to cancel the event (#3523)
						if ($(event.target).zIndex() < $.ui.dialog.overlay.maxZ) {
							return false;
						}
					});
				}
			}, 1);

			// allow closing by pressing the escape key
			$(document).bind('keydown.dialog-overlay', function(event) {
				if (dialog.options.closeOnEscape && !event.isDefaultPrevented() && event.keyCode &&
					event.keyCode === $.ui.keyCode.ESCAPE) {
					
					dialog.close(event);
					event.preventDefault();
				}
			});

			// handle window resize
			$(window).bind('resize.dialog-overlay', $.ui.dialog.overlay.resize);
		}

		var $el = (this.oldInstances.pop() || $('<div></div>').addClass('ui-widget-overlay'))
			.appendTo(document.body)
			.css({
				width: this.width(),
				height: this.height()
			});

		if ($.fn.bgiframe) {
			$el.bgiframe();
		}

		this.instances.push($el);
		return $el;
	},

	destroy: function($el) {
		var indexOf = $.inArray($el, this.instances);
		if (indexOf != -1){
			this.oldInstances.push(this.instances.splice(indexOf, 1)[0]);
		}

		if (this.instances.length === 0) {
			$([document, window]).unbind('.dialog-overlay');
		}

		$el.remove();
		
		// adjust the maxZ to allow other modal dialogs to continue to work (see #4309)
		var maxZ = 0;
		$.each(this.instances, function() {
			maxZ = Math.max(maxZ, this.css('z-index'));
		});
		this.maxZ = maxZ;
	},

	height: function() {
		var scrollHeight,
			offsetHeight;
		// handle IE 6
		if ($.browser.msie && $.browser.version < 7) {
			scrollHeight = Math.max(
				document.documentElement.scrollHeight,
				document.body.scrollHeight
			);
			offsetHeight = Math.max(
				document.documentElement.offsetHeight,
				document.body.offsetHeight
			);

			if (scrollHeight < offsetHeight) {
				return $(window).height() + 'px';
			} else {
				return scrollHeight + 'px';
			}
		// handle "good" browsers
		} else {
			return $(document).height() + 'px';
		}
	},

	width: function() {
		var scrollWidth,
			offsetWidth;
		// handle IE
		if ( $.browser.msie ) {
			scrollWidth = Math.max(
				document.documentElement.scrollWidth,
				document.body.scrollWidth
			);
			offsetWidth = Math.max(
				document.documentElement.offsetWidth,
				document.body.offsetWidth
			);

			if (scrollWidth < offsetWidth) {
				return $(window).width() + 'px';
			} else {
				return scrollWidth + 'px';
			}
		// handle "good" browsers
		} else {
			return $(document).width() + 'px';
		}
	},

	resize: function() {
		/* If the dialog is draggable and the user drags it past the
		 * right edge of the window, the document becomes wider so we
		 * need to stretch the overlay. If the user then drags the
		 * dialog back to the left, the document will become narrower,
		 * so we need to shrink the overlay to the appropriate size.
		 * This is handled by shrinking the overlay before setting it
		 * to the full document size.
		 */
		var $overlays = $([]);
		$.each($.ui.dialog.overlay.instances, function() {
			$overlays = $overlays.add(this);
		});

		$overlays.css({
			width: 0,
			height: 0
		}).css({
			width: $.ui.dialog.overlay.width(),
			height: $.ui.dialog.overlay.height()
		});
	}
});

$.extend($.ui.dialog.overlay.prototype, {
	destroy: function() {
		$.ui.dialog.overlay.destroy(this.$el);
	}
});

} (jQuery));

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationFormLayoutView
// -------------------------

define('views/observationformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/editmapview', 'views/editmediaview', 'datepicker', 'multiselect', 'jqueryui/dialog'],
function ($, _, Backbone, app, ich, EditMapView, EditMediaView) {
    var ObservationFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form observation-form',

        template: 'ObservationForm',

        regions: {
            media: '#media-resources-fieldset',
            map: '#location-fieldset',
            video: '#video-upload'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Title': '_contentChanged',
            'change input#ObservedOn': '_contentChanged',
            'change input#Address': '_contentChanged',
            'change input#Latitude': '_latLongChanged',
            'change input#Longitude': '_latLongChanged',
            'change input#IsIdentificationRequired': '_isIdentificationRequiredChanged',
            'change input#AnonymiseLocation': '_anonymiseLocationChanged',
            'change #projects-field input:checkbox': '_projectsChanged',
            'change #category-field input:checkbox': '_categoryChanged',
            'click #media-resource-import-button': '_showImportMedia'
        },

        initialize: function (options) {
            this.categories = options.categories;
            this.model.mediaResources.on('change:Metadata', this.onMediaResourceFilesChanged, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Observation: this.model.toJSON(),
                    Categories: this.categories
                }
            };
        },

        onMediaResourceFilesChanged: function (mediaResource) {
            var dateTime = mediaResource.get('Metadata').DateTaken;
            this.model.set('ObservedOn', dateTime);
            this.$el.find('#ObservedOn').val(dateTime);
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            var editMapView = new EditMapView({ el: '#location-fieldset', model: this.model });
            this.map.attachView(editMapView);
            editMapView.render();

            var editMediaView = new EditMediaView({ el: '#media-resources-fieldset', model: this.model });
            this.media.attachView(editMediaView);
            editMediaView.render();

            this.observedOnDatePicker = this.$el.find('#ObservedOn').datepicker();

            this.categoryListSelectView = this.$el.find("#Category").multiSelect({
                selectAll: false,
                singleSelect: true,
                noOptionsText: 'No Categories',
                noneSelected: '<span>Select Category</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            if (!ich.templates.ProjectSelectItem) {
                ich.addTemplate('ProjectSelectItem', '{{#Projects}}<option value="{{Id}}">{{Name}}</option>{{/Projects}}');
            }

            // Add project options
            this.$el.find('#Projects').append(ich.ProjectSelectItem({ Projects: app.authenticatedUser.projects.toJSON() }));

            this.projectListSelectView = this.$el.find('#Projects').multiSelect({
                selectAll: false,
                messageText: 'You can select more than one project',
                noOptionsText: 'No Projects',
                noneSelected: '<span>Select Projects</span>',
                renderOption: function (id, option) {
                    var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
                    if (option.selected) {
                        html += ' checked="checked"';
                    }
                    var project = app.authenticatedUser.projects.get(option.value);

                    html += ' /><img src="' + project.get('Avatar').Files.ThumbnailMedium.RelativeUri + '" alt="" />' + project.get('Name') + '</label>';
                    return html;
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<div />');
                    _.each(selectedOptions, function (option) {
                        var project = app.authenticatedUser.projects.get(option.value);
                        $selectedHtml.append('<span class="selected-project"><img src="' + project.get('Avatar').Files.ThumbnailMedium.RelativeUri + '" alt="" />' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });
        },

        _showImportMedia: function () {
            alert('Coming soon');
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);

            if (target.attr('id') === 'Address') {
                this._latLongChanged(e);
            }
        },

        _latLongChanged: function (e) {
            var oldPosition = { latitude: this.model.get('Latitude'), longitude: this.model.get('Longitude') };
            var newPosition = { latitude: $('#Latitude').val(), longitude: $('#Longitude').val() };

            this.model.set('Latitude', newPosition.latitude);
            this.model.set('Longitude', newPosition.longitude);

            // Only update pin if the location is different to avoid infinite loop
            if (newPosition.Latitude != null && newPosition.Longitude != null && (oldPosition.Latitude !== newPosition.Latitude || oldPosition.Longitude !== newPosition.Longitude)) {
                this.editMapView.changeMarkerPosition(this.model.get('Latitude'), this.model.get('Longitude'));
            }
        },

        _isIdentificationRequiredChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            this.model.set({ IsIdentificationRequired: $checkbox.attr('checked') == 'checked' ? true : false });
        },

        _anonymiseLocationChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            this.model.set({ AnonymiseLocation: $checkbox.attr('checked') == 'checked' ? true : false });
        },

        _projectsChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                var projectId = $checkbox.attr('value');
                this.model.addProject(projectId);
            } else {
                this.model.removeProject($checkbox.attr('value'));
            }
        },

        _categoryChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('Category', $checkbox.attr('value'));
            } else {
                this.model.set('Category', '');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
            //this.trigger('formClosed', this);
        },

        _save: function () {
            log('observationFormLayoutView.save');
            this.model._setMedia();
            this.model.save();
            app.showPreviousContentView();
            //app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
            //this.trigger('formClosed', this);
        }
    });

    return ObservationFormLayoutView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationLayoutView
// ---------------------

// Layout of an observation in both edit and view mode
define('views/observationlayoutview',['jquery', 'underscore', 'backbone', 'app', 'views/observationdetailsview', 'views/observationformlayoutview'], function ($, _, Backbone, app, ObservationDetailsView, ObservationFormLayoutView) {

    var ObservationLayoutView = Backbone.Marionette.Layout.extend({
        className: 'observation',

        template: 'Observation',

        regions: {
            main: '.main',
            notes: '.notes',
            comments: '.comments'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .observation');
        },

        showObservationDetails: function (observation) {
            var options = { model: observation };

            if (app.isPrerendering('observations')) {
                options['el'] = '.observation-details';
            }

            var observationDetailsView = new ObservationDetailsView(options);
            this.main[app.getShowViewMethodName('observations')](observationDetailsView);

            if (app.isPrerendering('observations')) {
                observationDetailsView.showBootstrappedDetails();
            }
        },

        showObservationForm: function (observation, categories) {
            var options = { model: observation, categories: categories };

            if (app.isPrerendering('observations')) {
                options['el'] = '.observation-form';
            }

            var observationFormLayoutView = new ObservationFormLayoutView(options);
            this.main[app.getShowViewMethodName('observations')](observationFormLayoutView);

            if (app.isPrerendering('observations')) {
                observationFormLayoutView.showBootstrappedDetails();
            }
        }
    });

    return ObservationLayoutView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResourceCollection
// -----------------------

define('collections/mediaresourcecollection',['jquery', 'underscore', 'backbone', 'models/mediaresource'], function ($, _, Backbone, MediaResource) {

    var MediaResourceCollection = Backbone.Collection.extend({
        model: MediaResource,

        url: '/mediaresources',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return MediaResourceCollection;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Observation
// -----------

define('models/observation',['jquery', 'underscore', 'backbone', 'collections/projectcollection', 'collections/mediaresourcecollection'],
function ($, _, Backbone, ProjectCollection, MediaResourceCollection) {
    var Observation = Backbone.Model.extend({
        defaults: {
            Title: '',
            ObservedOn: null,
            Address: '',
            Latitude: null,
            Longitude: null,
            Category: '',
            AnonymiseLocation: false,
            Projects: [],
            IsIdentificationRequired: false,
            Media: []
        },

        urlRoot: '/observations',

        idAttribute: 'Id',

        initialize: function (options) {
            this.mediaResources = new MediaResourceCollection();
        },

        toJSON: function () {
            return {
                Title: this.get('Title'),
                ObservedOn: this.get('ObservedOn'),
                Address: this.get('Address'),
                Latitude: this.get('Latitude'),
                Longitude: this.get('Longitude'),
                Category: this.get('Category'),
                AnonymiseLocation: this.get('AnonymiseLocation'),
                Projects: this.get('Projects'),
                IsIdentificationRequired: this.get('IsIdentificationRequired'),
                Media: this.get('Media')
            };
        },

        toViewJSON: function () {
            // returns JSON containing all data for view
            return Backbone.Model.prototype.toJSON.apply(this, arguments);
        },

        addProject: function (id) {
            var projects = this.get('Projects');
            projects.push(id);
            this.set('Projects', projects);
        },

        removeProject: function (id) {
            var projects = this.get('Projects');
            this.set('Projects', _.without(projects, id));
        },

        addMediaResource: function (mediaResource) {
            mediaResource.on('change', this._setMedia, this);
            this.mediaResources.add(mediaResource);
            this._setMedia();
        },

        removeMediaResource: function (id) {
            this.mediaResources.remove(this.mediaResources.get(id));
            this._setMedia();
        },

        _setMedia: function () {
            log('observation._SetMedia');
            var media = this.mediaResources.map(function (mediaResource) {
                log("the mediaresource being mapped: ", mediaResource);
                return { MediaResourceId: mediaResource.id, Description: "Description", Licence: 'licenceX' }
            });
            this.set('Media', media);
            log('The collection of media: ', media);
        }
    });

    return Observation;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// ObservationController & ObservationRouter
// -----------------------------------------
define('controllers/observationcontroller',['jquery', 'underscore', 'backbone', 'app', 'views/observationlayoutview', 'models/observation'],
function ($, _, Backbone, app, ObservationLayoutView, Observation) {
    var ObservationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'observations/create': 'showObservationForm',
            'observations/:id/update': 'showObservationForm',
            'observations/:id': 'showObservationDetails'
        }
    });

    var ObservationController = {};

    // Helper method to load project layout, taking into account bootstrapped data and prerendered view
    var showObservationLayoutView = function (observation) {
        var observationLayoutView = new ObservationLayoutView({ model: observation });
        //app.content[app.getShowViewMethodName('observations')](observationLayoutView);
        app.showFormContentView(observationLayoutView, 'observations');

        if (app.isPrerendering('observations')) {
            observationLayoutView.showBootstrappedDetails();
        }

        return observationLayoutView;
    };

    var getModel = function (id) {
        var url = '/observations/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('observations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Public API
    // ----------

    ObservationController.showObservationDetails = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var observation = new Observation(model.Observation);
                app.updateTitle(observation.get('Title'));
                var observationLayoutView = showObservationLayoutView(observation);
                observationLayoutView.showObservationDetails(observation);
                app.setPrerenderComplete();
            });
    };

    ObservationController.showObservationForm = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var observation = new Observation(model.Observation);
                if (observation.id) {
                    app.updateTitle('Edit Observation');
                } else {
                    app.updateTitle('New Observation');
                }
                
                var observationLayoutView = showObservationLayoutView(observation);
                observationLayoutView.showObservationForm(observation, model.Categories);
                app.setPrerenderComplete();
            });
    };

    ObservationController.mediaResourceUploaded = function (e, mediaResource) {
        app.vent.trigger('mediaResourceUploaded:', mediaResource );
    };

    // Event Handlers
    // --------------

    //    app.vent.on('observation:show', function (id) {
    //        ContributionController.showObservationForm(id);
    //    });
    

    app.addInitializer(function () {
        this.observationRouter = new ObservationRouter({
            controller: ObservationController
        });
    });

    return ObservationController;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationLayoutView
// ----------------------

define('views/organisationlayoutview',['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/activitycollection'], 
function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var OrganisationLayoutView = Backbone.Marionette.Layout.extend({
        
        className: 'organisation',

        template: 'Organisation',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .organisation');
        },

        showStream: function () {
            var activityCollection = new ActivityCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: activityCollection
            };
            if (app.isPrerendering('organisations')) {
                options['el'] = '.stream';
            }
            var streamView = new StreamView(options);
            if (app.isPrerendering('organisations')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }
            activityCollection.fetchFirstPage();
        }
    });

    return OrganisationLayoutView;
}); 
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationFormLayoutView
// --------------------------

define('views/organisationformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) 
{
    var OrganisationFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium organisation-form',

        template: 'OrganisationForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change textarea#Description': '_contentChanged',
            'change input#Website': '_contentChanged'
        },

        initialize: function (options) {
            log('organisationFormLayoutView.initialize');
        },

        serializeData: function () {
            log('organisationFormLayoutView:serializeData');
            return {
                Model: {
                    Organisation: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            log('organisationFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('organisationFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this.$el = $('#content .organisation-form');
            this._showDetails();
        },

        _showDetails: function () {
            log('organisationFormLayoutView:_showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();
        },

        _contentChanged: function (e) {
            log('organisationFormLayoutView:_contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return OrganisationFormLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationItemView
// --------------------

// Shows an individual project item
define('views/organisationitemview',['jquery', 'underscore', 'backbone', 'app'], 
function ($, _, Backbone, app) 
{
    var OrganisationItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-organisation-item',

        template: 'OrganisationItem',

        events: {
            'click .view-organisation-button': 'viewOrganisation'
        },

        viewOrganisation: function (e) {
            e.preventDefault();
            app.vent.trigger('viewOrganisation:', this.model);
        }
    });

    return OrganisationItemView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationCollectionView
// --------------------------

// Shows Explore organisation items
define('views/organisationcollectionview',['jquery','underscore','backbone','app','ich','views/organisationitemview'],
function ($, _, Backbone, app, ich, OrganisationItemView) 
{
    var OrganisationCollectionView = Backbone.Marionette.CollectionView.extend({

        tagName: 'ul',

        className: 'explore-groups',

        itemView: OrganisationItemView

    });

    return OrganisationCollectionView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationController & OrganisationRouter
// -------------------------------------------
define('controllers/organisationcontroller',['jquery', 'underscore', 'backbone', 'app', 'views/organisationlayoutview', 'views/organisationformlayoutview', 'views/organisationcollectionview', 'models/organisation', 'collections/organisationcollection'],
function ($, _, Backbone, app, OrganisationLayoutView, OrganisationFormLayoutView, OrganisationCollectionView, Organisation, OrganisationCollection) {
    var OrganisationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'organisations/explore': 'showOrganisationExplorer',
            'organisations/create': 'showOrganisationForm',
            'organisations/:id/update': 'showOrganisationForm',
            'organisations/:id': 'showOrganisationStream'
        }
    });

    var OrganisationController = {};

    //    app.vent.on('joinOrganisation:', function (organisation) {
    //        OrganisationController.joinOrganisation(organisation);
    //    });

    app.vent.on('organisationAdded:', function (organisation) {
        if (OrganisationController.organisationCollection) {
            OrganisationController.organisationCollection.add(organisation);
        }
    });

    app.vent.on('viewOrganisation:', function (organisation) {
        OrganisationController.showOrganisationStream(organisation.id);
    });

    var getModel = function (id) {
        var url = '/organisations/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('organisations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('organisations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            //            if (page) {
            //                params['page'] = page;
            //            }
            //            if (pageSize) {
            //                params['pageSize'] = pageSize;
            //            }
            //            if (sortField) {
            //                params['sortField'] = sortField;
            //            }
            //            if (sortDirection) {
            //                params['sortDirection'] = sortDirection;
            //            }
            //            if (searchQuery) {
            //                params['searchQuery'] = searchQuery;
            //            }
            $.ajax({
                url: '/organisations/explore',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // OrganisationController Public API
    // ---------------------------------

    // Show organisation activity
    OrganisationController.showOrganisationStream = function (id) {
        log('showing organisation home stream', this, this);
        $.when(getModel(id))
            .done(function (model) {
                var organisation = new Organisation(model.Organisation);
                log('HACK: injected organisations/ into project id value');
                organisation.set('Id', 'organisations/' + id);
                var organisationLayoutView = new OrganisationLayoutView({ model: organisation });
                app.showFormContentView(organisationLayoutView, 'organisations');
                if (app.isPrerendering('organisations')) {
                    organisationLayoutView.showBootstrappedDetails();
                }
                organisationLayoutView.showStream();
                app.setPrerenderComplete();
            });
    };

    // Show an organisation
    OrganisationController.showOrganisationForm = function (id) {
        log('organisationController:showOrganisationForm');
        $.when(getModel(id))
            .done(function (model) {
                log(model);
                var organisation = new Organisation(model.Organisation);
                var organisationFormLayoutView = new OrganisationFormLayoutView({ model: organisation });
                app.showFormContentView(organisationFormLayoutView, 'organisations');
                if (app.isPrerendering('organisations')) {
                    organisationFormLayoutView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    // Show an organisation explore
    OrganisationController.showOrganisationExplorer = function () {
        log('organisationController:showOrganisations');
        $.when(getExploreList())
            .done(function (model) {
                OrganisationController.organisationCollection = new OrganisationCollection(model.Organisations.PagedListItems);
                var organisationCollectionView = new OrganisationCollectionView({ collection: OrganisationController.organisationCollection });
                app.showFormContentView(organisationCollectionView, 'organisations');
                //app.content[app.getShowViewMethodName('projects')](projectCollectionView);
                if (app.isPrerendering('organisations')) {
                    organisationCollectionView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        this.organisationRouter = new OrganisationRouter({
            controller: OrganisationController
        });
    });

    return OrganisationController;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostFormLayoutView
// -------------------------

define('views/postformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) 
{
    var PostFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium post-form',

        template: 'PostForm',

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Subject': '_contentChanged',
            'change textarea#Message': '_contentChanged'
        },

        initialize: function (options) {
            log('postFormLayoutView:initialize');
        },

        serializeData: function () {
            log('postFormLayoutView:serializeData');
            return {
                Model: {
                    Post: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            log('postFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('postFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            log('postFormLayoutView:_showDetails');
        },

        _contentChanged: function (e) {
            log('postFormLayoutView:_contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return PostFormLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Post
// ----

define('models/post',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Post = Backbone.Model.extend({
        defaults: {
            Subject: '',
            Message: ''
        },

        idAttribute: 'Id',

        urlRoot: '/posts',

        toJSON: function () {
            return {
                Subject: this.get('Subject'),
                Message: this.get('Message')
            };
        }

    });

    return Post;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostController & PostRouter
// ---------------------------
define('controllers/postcontroller',['jquery', 'underscore', 'backbone', 'app', 'views/postformlayoutview', 'models/post'],
function ($, _, Backbone, app, PostFormLayoutView, Post) {
    var PostRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'posts/create': 'showCreatePostForm',
            'posts/create?id=:groupId': 'showCreatePostForm',
            'posts/:id/update': 'showUpdatePostForm'
        }
    });

    var PostController = {};

    var getModel = function (id) {
        var url = '/posts/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('posts')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // PostController Public API
    // ----------------------------

    // Show a post form
    PostController.showUpdatePostForm = function (id) {
        log('postController:showPostForm');
        $.when(getModel(id))
            .done(function (model) {
                var post = new Post(model.Post);
                var postFormLayoutView = new PostFormLayoutView({ model: post });

                app.content[app.getShowViewMethodName('posts')](postFormLayoutView);

                if (app.isPrerendering('posts')) {
                    postFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    // Show a post form
        PostController.showCreatePostForm = function (groupId) {
        log(project);
        log('postController:showPostForm');
        $.when(getModel())
            .done(function (model) {
                var post = new Post(model.Post);
                post.set('GroupId', project.id);
                var postFormLayoutView = new PostFormLayoutView({ model: post });

                app.content[app.getShowViewMethodName('posts')](postFormLayoutView);

                if (app.isPrerendering('posts')) {
                    postFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        this.postRouter = new PostRouter({
            controller: PostController
        });
    });

    return PostController;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormLayoutView
// -------------------------

define('views/projectformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'], 
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) {

    var ProjectFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium project-form',

        template: 'ProjectForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change textarea#Description': '_contentChanged',
            'change input#Website': '_contentChanged',
            'change #team-field input:checkbox': '_teamChanged'
        },

        initialize: function (options) {
            this.teams = options.teams;
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    Teams: this.teams
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .project-form');
            this._showDetails();
        },

        _showDetails: function () {
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();

            this.teamListSelectView = this.$el.find("#TeamId").multiSelect({
                selectAll: false,
                singleSelect: true,
                noOptionsText: 'No Teams',
                noneSelected: 'Select A Team',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _teamChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('TeamId', $checkbox.attr('value'));
            } else {
                this.model.set('TeamId', '');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return ProjectFormLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectItemView
// ---------------

// Shows an individual project item
define('views/projectitemview',['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {
    var ProjectItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-project-item',

        template: 'ProjectItem',

        events: {
            'click .join-project-button': 'joinProject',
            'click .leave-project-button': 'leaveProject'
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    ObservationCountDescription: this.model.get('ObservationCount') === 1 ? 'Observation' : 'Observations',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts'
                }
            };
        },

        joinProject: function (e) {
            e.preventDefault();
            app.vent.trigger('joinProject', this.model);
            this.$el.find('.join-project-button').replaceWith('<a href="#" class="leave-project-button button">Leave Project</a>');
        },

        leaveProject: function (e) {
            e.preventDefault();
            app.vent.trigger('leaveProject', this.model);
            this.$el.find('.leave-project-button').replaceWith('<a href="#" class="join-project-button button">Join Project</a>');
        }
    });

    return ProjectItemView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectsCompositeView
// ---------------------

define('views/projectscompositeview',['jquery','underscore','backbone','app','ich','views/projectitemview'],
function ($, _, Backbone, app, ich, ProjectItemView) 
{
    var ProjectsCompositeView = Backbone.Marionette.CompositeView.extend({
        className: 'projects',

        template: 'ProjectList',

        itemView: ProjectItemView,

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.project-list > ul > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.project-list > ul').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }
        },

        showBootstrappedDetails: function () {
            this.$el = $('#content .projects');
        }
    });

    return ProjectsCompositeView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController & ProjectRouter
// ---------------------------------
define('controllers/projectcontroller',['jquery', 'underscore', 'backbone', 'app', 'views/projectlayoutview', 'views/projectformlayoutview', 'views/projectscompositeview', 'models/project', 'collections/projectcollection'],
function ($, _, Backbone, app, ProjectLayoutView, ProjectFormLayoutView, ProjectsCompositeView, Project, ProjectCollection) {

    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects': 'showProjectExplorer',
            'projects/create': 'showProjectForm',
            'projects/:id/update': 'showProjectForm',
            'projects/:id': 'showProjectStream'
        }
    });

    var ProjectController = {};

    var getModel = function (id) {
        var url = '/projects/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            $.ajax({
                url: '/projects',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Join a project
    var joinProject = function (project) {
        var deferred = new $.Deferred();
        $.ajax({
            url: '/' + project.id + '/join',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST'
        })
        .done(function (data) {
            deferred.resolve(data.Model);
        });

        return deferred.promise();
    };

    // Leave a project
    var leaveProject = function (project) {
        var deferred = new $.Deferred();
        $.ajax({
            url: '/' + project.id + '/leave',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST'
        })
        .done(function (data) {
            deferred.resolve(data.Model);
            app.authenticatedUser.projects.remove(project.id);
        });

        return deferred.promise();
    };

    // ProjectController Public API
    // ----------------------------

    // Show project activity
    ProjectController.showProjectStream = function (id) {
        log('showing projects home stream', this, this);
        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                app.updateTitle(project.get('Name'));
                var projectLayoutView = new ProjectLayoutView({ model: project });
                //app.showFormContentView(projectLayoutView, 'projects');
                app.content[app.getShowViewMethodName('projects')](projectLayoutView);
                if (app.isPrerendering('projects')) {
                    projectLayoutView.showBootstrappedDetails();
                }
                projectLayoutView.showStream();
                app.setPrerenderComplete();
            });
    };

    // Show an project form
    ProjectController.showProjectForm = function (id) {
        log('projectController:showProjectForm');
        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                if (project.id) {
                    app.updateTitle('Edit Project');
                } else {
                    app.updateTitle('New Project');
                }
                var projectFormLayoutView = new ProjectFormLayoutView({ model: project, teams: model.Teams });
                app.showFormContentView(projectFormLayoutView, 'projects');
                if (app.isPrerendering('projects')) {
                    projectFormLayoutView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    // Show an project explore
    ProjectController.showProjectExplorer = function () {
        log('projectController:showProjects');
        $.when(getExploreList())
            .done(function (model) {
                app.updateTitle('Projects');
                var projectCollection = new ProjectCollection(model.Projects.PagedListItems);
                var projectsCompositeView = new ProjectsCompositeView({ model: app.authenticatedUser.user, collection: projectCollection });
                //var projectCollectionView = new ProjectCollectionView({ collection: ProjectController.projectCollection });
                //app.showFormContentView(projectCollectionView, 'projects');

                app.content[app.getShowViewMethodName('projects')](projectsCompositeView);
                if (app.isPrerendering('projects')) {
                    projectsCompositeView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.vent.on('joinProject', function (project) {
        joinProject(project);
    });

    app.vent.on('leaveProject', function (project) {
        leaveProject(project);
    });

    app.vent.on('projectAdded:', function (project) {
        if (ProjectController.projectCollection) {
            ProjectController.projectCollection.add(project);
        }
    });

    app.addInitializer(function () {
        this.projectRouter = new ProjectRouter({
            controller: ProjectController
        });
    });

    return ProjectController;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpecies
// ----------------

define('models/referencespecies',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var ReferenceSpecies = Backbone.Model.extend({
        defaults: {
            Species: null,
            SmartTags: []
        },

        idAttribute: 'Id',

        url: '/referencespecies/',

        toJSON: function () {
            return {
                Id: this.id,
                Species: this.get('Species'),
                SmartTags: this.get('SmartTags')
            };
        }
    });

    return ReferenceSpecies;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpeciesFormLayoutView
// -------------------------

define('views/referencespeciesformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect'], function ($, _, Backbone, app, ich) {

    var ReferenceSpeciesFormLayoutView = Backbone.Marionette.Layout.extend({

        tagName: 'section',

        className: 'form single-medium',

        id: 'reference-species-form',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#species': '_contentChanged',
            'change input#smarttags': '_contentChanged'
        },

        onRender: function () {
            alert('referenceSpeciesFormLayoutView ' + Date());

            //            if (app.authenticatedUser.teams != 'undefined') {
            //                // Define Team Option List template and add user's teams
            //                //ich.addTemplate('TeamSelectItem', '{{#Teams}}<option value="{{Id}}">{{Name}}</option>{{/Teams}}');
            //                //log(app);
            //                //this.$el.find('#Teams').append(ich.TeamSelectItem({ Teams: app.prerenderedView.data.Teams.Items.PagedListItems }));

            //                // Apply funky select list with avatar to Team select view
            //                this.teamListSelectView = $("#Team").multiSelect({
            //                    selectAll: false,
            //                    singleSelect: true,
            //                    noneSelected: '<span>Select a Team</span>',
            //                    oneOrMoreSelected: function (selectedOptions) {
            //                        var $selectedHtml = $('<span />');
            //                        _.each(selectedOptions, function (option) {
            //                            $selectedHtml.append('<span>' + option.text + '</span> ');
            //                        });
            //                        return $selectedHtml.children();
            //                    }
            //                });
            //            }
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return ReferenceSpeciesFormLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpeciesController & ReferenceSpeciesRouter
// ---------------------------------------------------

define('controllers/referencespeciescontroller',['jquery','underscore','backbone','app','models/referencespecies','views/referencespeciesformlayoutview'],
function ($, _, Backbone, app, ReferenceSpecies, ReferenceSpeciesFormLayoutView) 
{
    var ReferenceSpeciesRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'referencespecies/create': 'showReferenceSpeciesForm'
        }
    });

    var ReferenceSpeciesController = {};

    // ReferenceSpeciesController Public API
    // ---------------------------------

    // Show a ReferenceSpecies
    ReferenceSpeciesController.showReferenceSpeciesForm = function () {

        var referenceSpeciesFormLayoutView = new ReferenceSpeciesFormLayoutView({ el: $('.reference-species-create-form'), model: new ReferenceSpecies(app.prerenderedView.ReferenceSpecies) });

        referenceSpeciesFormLayoutView.render();

        app.prerenderedView.isBound = true;
    };

    // ReferenceSpecies Event Handlers
    // -------------------------------------

    app.vent.on('referencespecies:show', function () {
        ReferenceSpeciesController.showReferenceSpeciesForm();
    });

    app.addInitializer(function () {
        this.referenceSpeciesRouter = new ReferenceSpeciesRouter({
            controller: ReferenceSpeciesController
        });
    });

    return ReferenceSpeciesController;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Species
// -------

define('models/species',['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Species = Backbone.Model.extend({
        defaults: {
            Kingdom: '',
            Group: '',
            CommonNames: [],
            Taxonomy: '',
            Order: '',
            Family: '',
            GenusName: '',
            SpeciesName: '',
            ProposedAsNewSpecies: false
        },

        idAttribute: 'Id',

        url: '/species/',

        toJSON: function () {
            return {
                Id: this.id,
                Kingdom: this.get('Kingdom'),
                Group: this.get('Group'),
                CommonNames: this.get('CommonNames'),
                Taxonomy: this.get('Taxonomy'), // TODO: Fix this to return id?
                Order: this.get('Order'),
                Family: this.get('Family'),
                GenusName: this.get('GenusName'),
                SpeciesName: this.get('SpeciesName'),
                ProposedAsNewSpecies: this.get('ProposedAsNewSpecies')
            };
        }
    });

    return Species;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesFormItemView
// -------------------------

define('views/speciesformitemview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect'], function ($, _, Backbone, app, ich) {

    var SpeciesFormItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'section',

        className: 'form single-medium',

        id: 'species-form',

        template: 'SpeciesCreate',

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#kingdom': '_contentChanged',
            'change input#group': '_contentChanged',
            'change input#commonnames': '_contentChanged',
            'change input#taxonomy': '_contentChanged',
            'change input#order': '_contentChanged',
            'change input#family': '_contentChanged',
            'change input#genusname': '_contentChanged',
            'change input#speciesname': '_contentChanged',
            'change #proposed-new-species-field input:checkbox': '_proposedNewSpeciesChanged'
        },

        onRender: function () {
        },
        
        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _proposedNewSpeciesChanged: function (e) {
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return SpeciesFormItemView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesController & SpeciesRouter
// ---------------------------------
define('controllers/speciescontroller',['jquery', 'underscore', 'backbone', 'app', 'models/species', 'views/speciesformitemview'],
function ($, _, Backbone, app, Species, SpeciesFormItemView) 
{
    var SpeciesRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'species/create': 'showSpeciesForm'
        }
    });

    var SpeciesController = {};

    var getModel = function (id) {
        var url = '';
        if (!id) {
            url = '/species';
        } else {
            url = '/' + id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('species')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // SpeciesController Public API
    // ---------------------------------

    // Show a species
    SpeciesController.showSpeciesForm = function () {

        var speciesFormItemView = new SpeciesFormItemView({ el: $('.species-create-form'), model: new Species(app.prerenderedView.Species) });

        speciesFormItemView.render();

        app.prerenderedView.isBound = true;
    };

    // SpeciesController Event Handlers
    // -------------------------------------
    app.vent.on('species:show', function () {
        SpeciesController.showSpeciesForm();
    });

    app.addInitializer(function () {
        this.speciesRouter = new SpeciesRouter({
            controller: SpeciesController
        });
    });

    return SpeciesController;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamFormLayoutView
// -------------------------

define('views/teamformlayoutview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'multiselect', 'fileupload'],
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) {

    var TeamFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium team-form',

        template: 'TeamForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change textarea#Description': '_contentChanged',
            'change input#Website': '_contentChanged',
            'change #organisation-field input:checkbox': '_organisationChanged'
        },

        initialize: function (options) {
            log('teamFormLayoutView.initialize');
            this.organisations = options.organisations;
        },

        serializeData: function () {
            log('teamFormLayoutView.serializeData');
            return {
                Model: {
                    Team: this.model.toJSON(),
                    Organisations: this.organisations
                }
            };
        },

        onShow: function () {
            log('teamFormLayoutView.onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('teamFormLayoutView.showBootstrappedDetails');
            this.initializeRegions();
            this.$el = $('#content .team-form');
            this._showDetails();
        },

        _showDetails: function () {
            log('teamFormLayoutView._showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();

            this.organisationListSelectView = this.$el.find("#Organisation").multiSelect({
                selectAll: false,
                singleSelect: true,
                noOptionsText: 'No Organisations',
                noneSelected: 'Select An Organisation',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });
        },

        _contentChanged: function (e) {
            log('teamFormLayoutView._contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _organisationChanged: function (e) {
            log('teamFormLayoutView._organisationChanged');
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('Organisation', $checkbox.attr('value'));
            } else {
                this.model.set('Organisation', '');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            log('teamFormLayoutView._save');
            log(this.model);
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return TeamFormLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectLayoutView
// -----------------

define('views/teamlayoutview',['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/activitycollection'], 
function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var TeamLayoutView = Backbone.Marionette.Layout.extend({
        className: 'team',

        template: 'Team',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .team');
        },

        showStream: function () {
            var activityCollection = new ActivityCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: activityCollection
            };
            if (app.isPrerendering('teams')) {
                options['el'] = '.stream';
            }
            var streamView = new StreamView(options);
            if (app.isPrerendering('teams')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }
            activityCollection.fetchFirstPage();
        }
    });

    return TeamLayoutView;

}); 
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamItemView
// ------------

// Shows an individual project item
define('views/teamitemview',['jquery', 'underscore', 'backbone', 'app'], 
function ($, _, Backbone, app) 
{
    var TeamItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-team-item',

        template: 'TeamItem',

        events: {
            'click .view-team-button': 'viewTeam'
        },

        viewTeam: function (e) {
            e.preventDefault();
            app.vent.trigger('viewTeam:', this.model);
        }
    });

    return TeamItemView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamCollectionView
// ------------------

// Shows Explore project items for selected Project
define('views/teamcollectionview',['jquery','underscore','backbone','app','ich','views/teamitemview'],
function ($, _, Backbone, app, ich, TeamItemView) 
{
    var TeamCollectionView = Backbone.Marionette.CollectionView.extend({

        tagName: 'ul',

        className: 'explore-groups',

        itemView: TeamItemView

    });

    return TeamCollectionView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamController & TeamRouter
// ---------------------------
define('controllers/teamcontroller',['jquery','underscore','backbone','app','models/team','views/teamformlayoutview', 'views/teamlayoutview', 'views/teamcollectionview', 'collections/teamcollection'],
function ($, _, Backbone, app, Team, TeamFormLayoutView, TeamLayoutView, TeamCollectionView, TeamCollection) 
{
    var TeamRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'teams/explore': 'showTeamExplorer',
            'teams/create': 'showTeamForm',
            'teams/:id/update': 'showTeamForm',
            'teams/:id': 'showTeamStream'
        }
    });

    var TeamController = {};

    app.vent.on('teamAdded:', function (team) {
        if (TeamController.teamCollection) {
            TeamController.teamCollection.add(team);
        }
    });

    app.vent.on('viewTeam:', function (team) {
        TeamController.showTeamStream(team.id);
    });

    var getModel = function (id) {
        var url = '/teams/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('teams')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('teams')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            //            if (page) {
            //                params['page'] = page;
            //            }
            //            if (pageSize) {
            //                params['pageSize'] = pageSize;
            //            }
            //            if (sortField) {
            //                params['sortField'] = sortField;
            //            }
            //            if (sortDirection) {
            //                params['sortDirection'] = sortDirection;
            //            }
            //            if (searchQuery) {
            //                params['searchQuery'] = searchQuery;
            //            }
            $.ajax({
                url: '/teams/explore',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // TeamController Public API
    // ---------------------------------

    // Show an team form
    TeamController.showTeamForm = function (id) {
        log('teamController:showTeamForm');
        $.when(getModel(id))
            .done(function (model) {
                var team = new Team(model.Team);
                var teamFormLayoutView = new TeamFormLayoutView({ model: team, organisations: model.Organisations });

                //app.content[app.getShowViewMethodName('teams')](teamFormLayoutView);
                app.showFormContentView(teamFormLayoutView, 'teams');

                if (app.isPrerendering('teams')) {
                    teamFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    // Show team activity
    TeamController.showTeamStream = function (id) {
        log('showing teams home stream', this, this);
        $.when(getModel(id))
        .done(function (model) {
            var team = new Team(model.Team);
            log('HACK: injected teams/ into team id value');
            team.set('Id', 'teams/' + id);
            var teamLayoutView = new TeamLayoutView({ model: team });
            app.showFormContentView(teamLayoutView, 'teams');
            if (app.isPrerendering('teams')) {
                teamLayoutView.showBootstrappedDetails();
            }
            teamLayoutView.showStream();
            app.setPrerenderComplete();
        });
    };

    // Show an team explore
    TeamController.showTeamExplorer = function () {
        log('teamController:showTeams');
        $.when(getExploreList())
            .done(function (model) {
                TeamController.teamCollection = new TeamCollection(model.Teams.PagedListItems);
                var teamCollectionView = new TeamCollectionView({ collection: TeamController.teamCollection });
                //app.content[app.getShowViewMethodName('teams')](teamCollectionView);
                app.showFormContentView(teamCollectionView, 'teams');
                if (app.isPrerendering('teams')) {
                    teamCollectionView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        this.teamRouter = new TeamRouter({
            controller: TeamController
        });
    });

    return TeamController;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountController & AccountRouter
// ---------------------------------

define('controllers/accountcontroller',['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app, HomeLayoutView) {

    var AccountRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'account/login': 'showLogin',
            'account/register': 'showRegister'
        }
    });

    var AccountController = {};

    // Public API
    // ----------

    AccountController.showLogin = function () {
    };

    AccountController.showRegister = function () {
    };

    app.addInitializer(function () {
        this.accountRouter = new AccountRouter({
            controller: AccountController
        });
    });

    return AccountController;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageItemView
// -------------------

// Shows a message from a user in a chat window
define('views/chatmessageitemview',['jquery', 'underscore', 'backbone', 'app', 'models/chatmessage', 'date'],
function ($, _, Backbone, app) {
    var ChatMessageItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'chat-message',

        template: 'ChatMessage',

        serializeData: function () {
            var model = null;
            if (this.model.get('Type') === 'usermessage') {
                var timestamp = this.model.get('Timestamp');
                model = {
                    UserMessage: {
                        From: app.authenticatedUser.user.get('Name') === this.model.get('FromUser').Name ? 'me' : this.model.get('FromUser').Name,
                        Message: this.model.get('Message').replace(/\n/g, '<br />'),
                        Time: timestamp === '' || timestamp == null ? '' : new Date(timestamp).toString('hh:mmtt'),
                        Timestamp: new Date(this.model.get('Timestamp')).toString()
                    }
                };
            }
            if (this.model.get('Type') === 'useradded') {
                model = {
                    UserAdded: {
                        Message: this.model.get('Message')
                    }
                };
            }
            return {
                Model: model
            };
        },

        onRender: function () {
            this.$el.addClass(this.model.get('Type'));
            app.vent.trigger('chats:itemview:added', this);
            this.model.on('change:Timestamp', this.updateTimestamp, this);
        },

        updateTimestamp: function (model) {
            log('timestamp updated', model);
            var timestamp = model.get('Timestamp');
            if (timestamp != null && timestamp != '') {
                this.$el.find('.timestamp').text(new Date(timestamp).toString('hh:mmtt'));
            }
        }
    });

    return ChatMessageItemView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageCollectionView
// -------------------------

// Shows messages in a chat
define('views/chatmessagecollectionview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessageitemview'],
function ($, _, Backbone, app, ich, ChatMessageItemView) {
    var ChatMessageCollectionView = Backbone.Marionette.CollectionView.extend({
        tagName: 'ul',

        itemView: ChatMessageItemView
    });

    return ChatMessageCollectionView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatView
// --------

// Shows chat window for a chat
define('views/chatcompositeview',['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessagecollectionview'],
function ($, _, Backbone, app, ich, ChatMessageCollectionView, ChatUserCollectionView) {
    var ChatLayoutView = Backbone.Marionette.Layout.extend({
        className: 'window chat',

        template: 'Chat',

        events: {
            "click .chat-send-message-button": "sendMessage",
            'click .window-title-bar': 'toggleCollapsed',
            'click .window-title-bar .close': 'closeWindow'
        },

        regions: {
            messages: '.chat-messages'
        },

        initialize: function (options) {
        },

        serializeData: function () {
            var title = '';
            if (this.model.chatType() == 'private') {
                title = _.without(this.model.chatUsers.pluck('Name'), app.authenticatedUser.user.get('Name')).join(', ');
            } else {
                title = this.model.get('Group').Name;
            }
            return {
                Model: {
                    Title: title
                }
            };
        },

        onShow: function () {
            log('chatView.onRender');

            this.$el.find('.chat-send-message-button').attr("disabled", true);
            this.model.on('change:IsStarted', this.onChatStarted, this);

            var left = (app.chats.length * 300) + 20; // (num of chats * width of a chat) + some space between online users
            this.$el.css({ left: left });
            $('body').append(this.el);
            this.$el.find('.new-chat-message').focus();

            var chatMessageCollectionView = new ChatMessageCollectionView({ collection: this.model.chatMessages });
            this.messages.show(chatMessageCollectionView);
            chatMessageCollectionView.render();

            chatMessageCollectionView.on('item:afteradd', this.onItemViewAdded, this);
            var that = this;
            this.$el.find('.new-chat-message').keydown(function (e) {
                // Enter was pressed without shift key
                if (e.keyCode == 13 && !e.shiftKey) {
                    // prevent default behavior
                    e.preventDefault();
                    // Send the message
                    if (that.model.get('IsStarted') === true) {
                        that.sendMessage.call(that);
                    }
                }
            });

            this.wasTyping = false;

            this.$el.find('.new-chat-message').keypress(function () {
                if (that.model.get('IsStarted') === true) {
                    var isTyping = false;
                    if ($(this).val().length > 0) {
                        isTyping = true;
                    }
                    if (that.wasTyping != isTyping) {
                        app.vent.trigger('chats:useristyping', that.model, isTyping);
                    }
                    that.wasTyping = isTyping;
                }
            });

            this.model.chatUsers.on('change:IsTyping', this.updateTypingStatus, this);

            this.model.chatMessages.on('add', this.onChatMessageAdded, this);
        },

        onChatStarted: function () {
            // When chat starts, enable all buttons and add the auth user to the view
            this.$el.find('.chat-send-message-button').attr("disabled", false);
            this.user = this.model.chatUsers.get(app.authenticatedUser.user.id);
        },

        sendMessage: function () {
            var $messageTextArea = this.$el.find('.new-chat-message');
            var message = $.trim($messageTextArea.val());
            if (message.length > 0) {
                app.vent.trigger('chats:sendMessage', this.model, message);
                $messageTextArea.val('').focus();
            }
        },

        closeWindow: function () {
            app.vent.trigger('chats:close', this.model);
        },

        toggleCollapsed: function () {
            this.$el.toggleClass('collapsed');
        },

        onItemViewAdded: function (view) {
            var height = this.$el.find('.chat-messages')[0].scrollHeight;
            this.$el.find('.chat-messages').scrollTop(height);
        },

        onChatMessageAdded: function (message) {
            if (message.get('Type') === 'usermessage') {
                var user = this.model.chatUsers.get(message.get('FromUser').Id).set('IsTyping', false);
                this.updateTypingStatus();
            }
        },

        updateTypingStatus: function () {
            if (this.model.get('IsStarted') === true) {
                var users = _.reject(this.model.chatUsers.where({ 'IsTyping': true }), function (u) { return u.id == this.user.id; }, this);
                var names = _.map(users, function (user) { return user.get('Name'); }, this);

                var desc = '';
                if (names.length == 1) {
                    desc = names[0] + ' is typing...';
                } else if (names.length > 1) {
                    desc = names.join(', ') + ' are typing...';
                }

                this.$el.find('.status').text(desc);
            }
        }
    });

    return ChatLayoutView;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatRegion
// ----------

define('views/chatregion',['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var ChatRegion = Backbone.Marionette.Region.extend({
        el: 'body',

        initialize: function (options) {
            this.chat = options.chat;
        }
    });

    return ChatRegion;
});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatController & ChatRouter
// ---------------------------

define('controllers/chatcontroller',['jquery', 'underscore', 'backbone', 'app', 'models/chat', 'collections/usercollection',
'collections/chatmessagecollection', 'views/chatcompositeview', 'views/chatregion', 'signalr'],
function ($, _, Backbone, app, Chat, UserCollection, ChatMessageCollection, ChatCompositeView, ChatRegion) {

    var ChatRouter = function (options) {
        this.chatHub = options.chatHub;
        this.userHub = options.userHub;

        // Wire up user hub callbacks
        this.userHub.chatJoined = chatJoined;
        this.userHub.chatExited = chatExited;

        // Wire up chat hub callbacks
        this.chatHub.userIsTyping = userIsTyping;
        this.chatHub.userJoinedChat = userJoinedChat;
        this.chatHub.userExitedChat = userExitedChat;
        this.chatHub.newChatMessage = newChatMessage;
        this.chatHub.debugToLog = debugToLog;

        this.getChat = function (chatId) {
            return this.chatHub.getChat(chatId);
        };

        this.joinChat = function (chatId, userIds, groupId) {
            this.chatHub.joinChat(chatId, userIds, groupId);
        };

        this.exitChat = function (chatId) {
            this.chatHub.exitChat(chatId);
        };

        this.typing = function (chatId, isTyping) {
            this.chatHub.typing(chatId, isTyping);
        };

        this.sendChatMessage = function (chatId, messageId, message) {
            this.chatHub.sendChatMessage(chatId, messageId, message);
        };
    };

    // Shows a chat by creating a model, view and adding it to a new region
    var showChat = function (chatId, users, messages, group) {
        var chatUsers = new UserCollection(users);
        var chatMessages = new ChatMessageCollection(messages);
        var chat = new Chat({ Id: chatId, Group: group }, { chatUsers: chatUsers, chatMessages: chatMessages });
        app.chats.add(chat);
        var chatView = new ChatCompositeView({ id: 'chat-' + chatId.replace(/\//g, '-'), model: chat });
        var chatRegion = new ChatRegion({ chat: chat });
        app.chatRegions.push(chatRegion);
        chatRegion.show(chatView, 'append');
        return chat;
    };

    // Close chat window
    var closeChat = function (chat) {
        app.chats.remove(chat.id);
        var chatRegion = _.find(app.chatRegions, function (region) { return region.chat.id === chat.id; });
        app.chatRegions.splice(_.indexOf(app.chatRegions, chatRegion), 1);
        chatRegion.close();
    };

    var generateHash = function (value) {
        var hash = 0;
        if (value.length == 0) return hash;
        for (i = 0; i < value.length; i++) {
            char = value.charCodeAt(i);
            hash = ((hash << 5) - hash) + char;
            hash = hash & hash; // Convert to 32bit integer
        }
        return hash;
    };

    // Generate chat id based of of ids
    var generateChatId = function (ids) {
        var sortedIds = _.sortBy(ids, function (name) { return name });
        return 'chats/' + generateHash(sortedIds.join(''));
    };

    // Hub Callbacks
    // -------------

    // Log info
    var debugToLog = function (message) {
        log(message);
    }

    // Chat joined
    var chatJoined = function (chatDetails) {
        var chat = app.chats.get(chatDetails.ChatId);
        // Chat might not exist if this is a group chat and the user has not received any messages on a secondary browser session
        if (chat == null) {
            chat = showChat(chatDetails.ChatId, chatDetails.Users, chatDetails.Messages, chatDetails.Group);
        }
        if (chat.get('IsStarted') === false) {
            chat.start(chatDetails);
        }
    };

    // Chat exited
    var chatExited = function (chatId) {
        // This callback gets fired so that we can close other browser session windows, if the user has more than one open
        var chat = app.chats.get(chatId);
        if (chat) {
            closeChat(chat);
        }
    };

    // User joined group chat 
    var userJoinedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.add(details.FromUser);
        // Only add other users' joining messages
        if (details.FromUser.Id !== app.authenticatedUser.user.id && chat.chatType() === 'group') {
            chat.chatMessages.add(details);
        }
    };

    // User existed group chat
    var userExitedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.remove(chat.chatUsers.get(details.FromUser.Id));
        // Only add other users' leaving messages
        if (details.FromUser.Id !== app.authenticatedUser.user.id && chat.chatType() === 'group') {
            chat.chatMessages.add(details);
        }
    };

    // Chat message received ready to display
    var newChatMessage = function (chatMessage) {
        // Find the chat
        var chat = app.chats.get(chatMessage.ChatId);

        // If this is a private chat, then this might be the first message we have received. If so, we need to create the chat
        if (chat == null) {
            app.chatRouter.getChat(chatMessage.ChatId)
                .done(function (chatDetails) {
                    log('chat received', chatDetails);
                    chat = showChat(chatMessage.ChatId, chatDetails.Users, chatDetails.Group ? chatDetails.Messages : [chatMessage], chatDetails.Group);
                    chat.start();
                })
                .fail(function (error) {
                    // TODO: Error handling
                });
        } else {
            // Add the message
            var existingChatMessage = chat.chatMessages.get(chatMessage.Id);
            if (existingChatMessage) {
                // If the chat message exists, we just update its details (this will show the datetime of the message being uploaded
                existingChatMessage.set(chatMessage);
            } else {
                chat.chatMessages.add(chatMessage);
            }
        }
    };

    // User is typing a message status
    var userIsTyping = function (typingDetails) {
        var chat = app.chats.get(typingDetails.ChatId);
        chat.chatUsers.get(typingDetails.UserId).set('IsTyping', typingDetails.IsTyping);
    };

    // ChatController Public API
    // -------------------------

    var ChatController = {};

    // Initiate a new private chat
    ChatController.startPrivateChat = function (user) {
        // Check to see if we have this user in a one-on-one private chat already
        var chatId = generateChatId([app.authenticatedUser.user.id, user.id]);
        var chat = app.chats.find(function (c) { return c.id == chatId; }, this);
        if (app.authenticatedUser.user.id != user.id && !chat) { // can't chat with self!
            showChat(chatId, [user], [], null);
            app.chatRouter.joinChat(chatId, [app.authenticatedUser.user.id, user.id], null);
        }
    };

    // Join a group chat
    ChatController.joinGroupChat = function (group) {
        // Check to see if we have this user in a chat
        var chatId = generateChatId([group.id]);
        var chat = app.chats.find(function (c) { return c.id == chatId; }, this);
        if (!chat) {
            showChat(chatId, [app.authenticatedUser.user], [], group.toJSON());
            app.chatRouter.joinChat(chatId, [app.authenticatedUser.user.id], group.id);
        }
    };

    // Leave a chat
    ChatController.exitChat = function (chat) {
        if (chat.chatType() === 'group') {
            app.chatRouter.exitChat(chat.id);
        }
        closeChat(chat);
    };

    // Send typing status to other users
    ChatController.sendTypingStatus = function (chat, isTyping) {
        app.chatRouter.typing(chat.id, isTyping);
    };

    // Send a chat message
    ChatController.sendChatMessage = function (chat, message) {
        // Add the chat message to the message list so that user sees it instantly
        var messageId = app.generateGuid();
        chat.chatMessages.add({ Id: messageId, Type: 'usermessage', ChatId: chat.id, FromUser: app.authenticatedUser.user.toJSON(), Timestamp: '', Message: message });
        app.chatRouter.sendChatMessage(chat.id, messageId, message);
    };

    // ChatController Events
    // ---------------------

    app.vent.on('chats:joinGroupChat', function (group) {
        ChatController.joinGroupChat(group);
    });

    app.vent.on('chats:startPrivateChat', function (user) {
        ChatController.startPrivateChat(user);
    });

    app.vent.on('chats:useristyping', function (chat, isTyping) {
        ChatController.sendTypingStatus(chat, isTyping);
    });

    app.vent.on('chats:sendMessage', function (chat, message) {
        ChatController.sendChatMessage(chat, message);
    });

    app.vent.on('chats:close', function (chat) {
        ChatController.exitChat(chat);
    });

    app.addInitializer(function () {
        this.chatRouter = new ChatRouter({
            chatHub: $.connection.chatHub,
            userHub: $.connection.userHub
        });
    });

    return ChatController;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HeaderView
// ----------

// The app's header
define('views/headerview',['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var HeaderView = Backbone.Marionette.ItemView.extend({
        el: 'header',

        showBootstrappedDetails: function () {
            this.$el.find('#explore-menu a').on('click', function (e) {
                e.preventDefault();
                app.observationRouter.navigate($(this).attr('href'), { trigger: true });
                return false;
            });
            this.$el.find('.user-menu a').on('click', function (e) {
                e.preventDefault();
                app.observationRouter.navigate($(this).attr('href'), { trigger: true });
                return false;
            });
        }
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function (options) {
        $(function () {
            // Render the layout and get it on the screen, first
            var headerView = new HeaderView();

            headerView.on('show', function () {
                app.vent.trigger('headerView:rendered');
            });

            app.header.attachView(headerView);
            headerView.showBootstrappedDetails();
        });
    });

    return HeaderView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// FooterView
// ----------

// The app's footer
define('views/footerview',['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var FooterView = Backbone.Marionette.ItemView.extend({
        el: 'footer'
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function (options) {
        $(function () {
            // Render the layout and get it on the screen, first
            var footerView = new FooterView();

            footerView.on('show', function () {
                Bowerbird.app.vent.trigger('footerView:rendered');
            });

            Bowerbird.app.footer.attachView(footerView);
        });
    });

    return FooterView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarMenuGroupCompositeView
// -----------------------------

// A collection of links in the sidebar
define('views/sidebarmenugroupcompositeview',['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var SidebarMenuGroupCompositeView = Backbone.Marionette.CompositeView.extend({
        className: 'menu-group',

        template: 'SidebarMenuGroup',

        initialize: function (options) {
            this.type = options.type;
            this.label = options.label;
        },

        appendHtml: function (collectionView, itemView) {
            var index = collectionView.collection.indexOf(itemView.model);
            if (index === 0 && collectionView.$el.find('#' + this.type + '-menu-group-list > li').length === 0) {
                collectionView.$el.find('#' + this.type + '-menu-group-list').append(itemView.el);
            } else {
                collectionView.$el.find('#' + this.type + '-menu-group-list > li').eq(index - 1).after(itemView.el);
            }
        },

        serializeData: function () {
            return {
                Model: {
                    Name: this.type,
                    Label: this.label
                }
            };
        }
    });

    return SidebarMenuGroupCompositeView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarItemView
// ---------------

define('views/sidebarprojectitemview',['jquery', 'underscore', 'backbone', 'app', 'models/project'],
function ($, _, Backbone, app, Project) {

    var SidebarProjectItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarProjectItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu-button': 'showMenu',
            //'click li#createnewpost': 'createPost',
            'click .sub-menu-button li': 'selectMenuItem'
        },

        initialize: function () {
            this.activityCount = 0;
        },

        onRender: function () {
            var that = this;
            this.$el.find('#createnewpost a').on('click', function (e) {
                e.preventDefault();
                var location = e.target.attributes["href"]; //$(this).attr('href');
                app.postRouter.navigate(location.nodeValue, { trigger: true });
                //app.projectRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
            $(this.el).children('a').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'), { trigger: true });
                that.activityCount = 0;
                that.$el.find('p span').remove();
                return false;
            });
            
            app.vent.on('newactivity:' + this.model.id + ':observationadded newactivity:' + this.model.id + ':postadded newactivity:' + this.model.id + ':observationnoteadded', this.onNewActivityReceived, this);
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Permissions: {
                    UpdateProject: app.authenticatedUser.hasGroupPermission(this.model.id, 'permissions/updateproject')
                }
            };
        },

        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

//        createPost: function (e) {
//            e.preventDefault();
//            var location = e.target.attributes["href"]; //$(this).attr('href');
//            app.postRouter.navigate(location.nodeValue, { trigger: true });
//            return false;
//        },

        startChat: function (e) {
            e.preventDefault();
            app.vent.trigger('chats:joinGroupChat', this.model);
        },

        onNewActivityReceived: function (activity) {
            _.each(activity.get('Groups'), function (group) {
                if (group.Id === this.model.id) {
                    this.activityCount++;
                    if (this.activityCount == 1) {
                        this.$el.find('p').append('<span title=""></span>');
                    }
                    var title = this.activityCount.toString() + ' New Item' + (this.activityCount > 1 ? 's' : '');
                    this.$el.find('p span').text(this.activityCount).attr('title', title);
                }
            },
            this);
        }
    });

    return SidebarProjectItemView;

});

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarTeamItemView
// -------------------

define('views/sidebarteamitemview',['jquery', 'underscore', 'backbone', 'app', 'models/team'], function ($, _, Backbone, app, Team) {

    var SidebarTeamItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarTeamItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu-button': 'showMenu',
            'click .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            var that = this;
            $(this.el).children('a').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'), { trigger: true });
                app.vent.trigger('team:show:stream', that.model.id);
                return false;
            });
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Permissions: {
                    CreateProject: app.authenticatedUser.hasGroupPermission(this.model.id, 'permissions/createproject'),
                    UpdateTeam: app.authenticatedUser.hasGroupPermission(this.model.id, 'permissions/updateteam')
                }
            };
        },

        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

        startChat: function (e) {
            e.preventDefault();
            app.vent.trigger('chats:joinGroupChat', this.model);
        }
    });

    return SidebarTeamItemView;

});

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarOrganisationItemView
// ---------------------------

define('views/sidebarorganisationitemview',['jquery', 'underscore', 'backbone', 'app', 'models/organisation'], function ($, _, Backbone, app, Organisation) {

    var SidebarOrganisationItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarOrganisationItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu-button': 'showMenu',
            'click .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            var that = this;
            $(this.el).children('a').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'), { trigger: true });
                app.vent.trigger('organisation:show:stream', that.model.id);
                return false;
            });
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Description: this.model.get('Description'),
                Website: this.model.get('Website'),
                Avatar: this.model.get('Avatar'),
                Type: 'Organisation'
            };
        },

        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

        startChat: function (e) {
            e.preventDefault();
            app.vent.trigger('chats:joinGroupChat', this.model);
        }
    });

    return SidebarOrganisationItemView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarLayoutView
// -----------------

// The left hand side bar that is shown to authenticated users.
define('views/sidebarlayoutview',['jquery', 'underscore', 'backbone', 'app', 'views/sidebarmenugroupcompositeview', 'views/sidebarprojectitemview', 'views/sidebarteamitemview', 'views/sidebarorganisationitemview'],
function ($, _, Backbone, app, SidebarMenuGroupCompositeView, SidebarProjectItemView, SidebarTeamItemView, SidebarOrganisationItemView) {

    var SidebarLayoutView = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'sidebar',

        className: 'triple-1',

        template: 'Sidebar',

        regions: {
            projectsMenu: '.menu-projects',
            watchlistsMenu: '#watch-menu-group #watch-list',
            teamsMenu: '.menu-teams',
            organisationsMenu: '.menu-organisations'
        },

        events: {
            'click .menu-group-options .sub-menu-button': 'showMenu',
            'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            $('article').prepend(this.el);

            var sidebarProjectCompositeView = new SidebarMenuGroupCompositeView({ id: 'project-menu-group', collection: this.model.projects, type: 'project', label: 'Projects' });
            sidebarProjectCompositeView.itemView = SidebarProjectItemView;
            this.projectsMenu.show(sidebarProjectCompositeView);

            if (this.model.teams.length > 0) {
                var sidebarTeamCompositeView = new SidebarMenuGroupCompositeView({ id: 'team-menu-group', collection: this.model.teams, type: 'team', label: 'Teams' });
                sidebarTeamCompositeView.itemView = SidebarTeamItemView;
                this.teamsMenu.show(sidebarTeamCompositeView);
            }

            if (this.model.organisations.length > 0) {
                var sidebarOrganisationCompositeView = new SidebarMenuGroupCompositeView({ id: 'organisation-menu-group', collection: this.model.organisations, type: 'organisation', label: 'Organisations' });
                sidebarOrganisationCompositeView.itemView = SidebarOrganisationItemView;
                this.organisationsMenu.show(sidebarOrganisationCompositeView);
            }

            var that = this;
            this.$el.find('#action-menu .new-project-button').on('click', function (e) {
                e.preventDefault();
                app.projectRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
            this.$el.find('#action-menu .new-observation-button').on('click', function (e) {
                e.preventDefault();
                app.projectRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
            this.$el.find('a.user-stream').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });

            app.authenticatedUser.projects.on('add', this.addProject, this);

            app.authenticatedUser.projects.on('remove', this.removeProject, this);
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.user.toJSON(),
                    Teams: this.model.teams.length > 0,
                    Organisations: this.model.organisations.length > 0,
                    AppRoot: this.model.appRoot != null
                }
            };
        },

        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

        addProject: function (project) {
            log('project added', project, this);
        },

        removeProject: function (project) {
            log('project removed', project, this);
        }
    });

    // Initialize the sidebar layout
    app.addInitializer(function (options) {
        $(function () {
            // Only show sidebar if user is authenticated
            if (app.authenticatedUser) {
                var model = {
                    user: app.authenticatedUser.user,
                    projects: app.authenticatedUser.projects,
                    teams: app.authenticatedUser.teams,
                    organisations: app.authenticatedUser.organisations,
                    appRoot: app.authenticatedUser.appRoot
                };

                // Render the layout and get it on the screen, first
                var sidebarLayoutView = new SidebarLayoutView({ model: model });

                sidebarLayoutView.on('show', function () {
                    app.vent.trigger('sidebar:rendered');
                });

                app.sidebar.show(sidebarLayoutView);
            }
        });
    });

    return SidebarLayoutView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// NotificationItemView
// --------------------

define('views/notificationitemview',['jquery', 'underscore', 'backbone', 'app', 'timeago'], function ($, _, Backbone, app) {

    var parseISO8601 = function (str) {
        // we assume str is a UTC date ending in 'Z'

        var parts = str.split('T'),
        dateParts = parts[0].split('-'),
        timeParts = parts[1].split('Z'),
        timeSubParts = timeParts[0].split(':'),
        timeSecParts = timeSubParts[2].split('.'),
        timeHours = Number(timeSubParts[0]),
        _date = new Date;

        _date.setUTCFullYear(Number(dateParts[0]));
        _date.setUTCMonth(Number(dateParts[1]) - 1);
        _date.setUTCDate(Number(dateParts[2]));
        _date.setUTCHours(Number(timeHours));
        _date.setUTCMinutes(Number(timeSubParts[1]));
        _date.setUTCSeconds(Number(timeSecParts[0]));
        if (timeSecParts[1]) _date.setUTCMilliseconds(Number(timeSecParts[1]));

        // by using setUTC methods the date has already been converted to local time(?)
        return _date;
    };

    var NotificationItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'notification-item',

        template: 'NotificationItem',

        serializeData: function () {
            var model = this.model.toJSON();
            model.CreatedDateTimeDescription = parseISO8601(this.model.get('CreatedDateTime') + 'Z');
            //model.ObservedOnDescription = ''; //parseISO8601(this.model.get('ObservedOn') + 'Z').format('d MMM yyyy')
            return {
                Model: model
            };
        },

        onRender: function () {
            this.$el.find('.time-description').timeago();
        }
        //        render: function () {
        //            switch (this.StreamItem.get('Type')) {
        //                case 'observation':
        //                    var streamitemJSON = this.streamItem.toJSON();
        //                    streamitemJSON['ObservedOnDate'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('d MMM yyyy');
        //                    streamitemJSON['ObservedOnTime'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('h:mm');
        //                    streamitemJSON['HighlightMedia'] = streamitemJSON.item.observationMedia[0];
        //                    this.$el.append(ich.ObservationStreamListItem(streamitemJSON)).addClass('observation-stream-item');
        //                    break;
        //                default:
        //                    break;
        //            }
        //            return this;
        //        }
    });

    return NotificationItemView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// NotificationsCompositeView
// --------------------------

// The right hand side bar that is shown to authenticated users.
define('views/notificationscompositeview',['jquery', 'underscore', 'backbone', 'app', 'views/notificationitemview', 'collections/activitycollection'],
function ($, _, Backbone, app, NotificationItemView, ActivityCollection) {

    var NotificationsCompositeView = Backbone.Marionette.CompositeView.extend({
        tagName: 'section',

        id: 'notifications',

        className: 'triple-3',

        template: 'Notifications',

        itemView: NotificationItemView,

        onRender: function () {
            $('article').append(this.el);
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('ul > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('ul').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }
        }
    });

    // Initialize the sidebar layout
    app.addInitializer(function (options) {
        // Only show notifications if user is authenticated
        if (app.authenticatedUser) {
            // Setup activity event listening
            var activityCollection = new ActivityCollection();
            activityCollection.baseUrl = '/notifications';
            app.vent.on('newactivity', function (activity) {
                activityCollection.add(activity);
            });

            var notificationsCompositeView = new NotificationsCompositeView({ model: app.authenticatedUser.user, collection: activityCollection });

            notificationsCompositeView.on('show', function () {
                app.vent.trigger('notifications:rendered');
            });

            app.notifications.show(notificationsCompositeView);

            activityCollection.fetchFirstPage();
        }
    });

    return NotificationsCompositeView;

});
/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUserItemView
// ------------------

define('views/useritemview',['jquery', 'underscore', 'backbone', 'app', 'models/user'], function ($, _, Backbone, app) {

    var UserItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'menu-group-item',

        template: 'UserItem',

        events: {
            'click .menu-item-body': 'startChat'
        },

        startChat: function (e) {
            app.vent.trigger('chats:startPrivateChat', this.model);
        }

    });

    return UserItemView;

});

 /// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUsersCompositeView
// ----------------------------

define('views/onlineusercompositeview',['jquery', 'underscore', 'backbone', 'app', 'views/useritemview'],
function ($, _, Backbone, app, UserItemView) {
    var OnlineUserCompositeView = Backbone.Marionette.CompositeView.extend({

        itemView: UserItemView,

        template: 'OnlineUserList',

        id: 'online-users',

        className: 'window collapsed',

        events: {
            'click .window-title-bar': 'toggleCollapsed'
        },

        initialize: function (options) {
            this.collection.on('add', this.updateUserCount, this);
            this.collection.on('remove', this.updateUserCount, this);
        },

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.find('ul').prepend(itemView.el);
        },

        onShow: function () {
            $('body').append(this.el);
        },

        serializeData: function () {
            return {
                Model: {
                    Count: this.collection.length,
                    TitleDescription: this.getTitle(this.collection.length)
                }
            };
        },

        updateUserCount: function (model, collection) {
            this.$el.find('.user-count').text(collection.length);
            this.$el.find('.title-description').text(this.getTitle(collection.length));
        },

        toggleCollapsed: function () {
            this.$el.toggleClass('collapsed');
        },

        getTitle: function (count) {
            return 'User' + (count == 1 ? '' : 's') + ' Online';
        }
    });

    app.addInitializer(function (options) {
        $(function () {
            if (app.authenticatedUser) {
                var onlineUserCompositeView = new OnlineUserCompositeView({ model: app.onlineUsers, collection: app.onlineUsers });

                onlineUserCompositeView.on('show', function () {
                    app.vent.trigger('onlineUsers:rendered');
                });

                app.usersonline.show(onlineUserCompositeView);
            }
        });
    });

    return OnlineUserCompositeView;

});
define('hubs',['jquery', 'signalr'], function () {
/*!
* SignalR JavaScript Library v0.5.0
* http://signalr.net/
*
* Copyright David Fowler and Damian Edwards 2012
* Licensed under the MIT.
* https://github.com/SignalR/SignalR/blob/master/LICENSE.md
*
*/

/// <reference path="jquery-1.6.2.js" />
(function ($, window) {
    /// <param name="$" type="jQuery" />
    

    if (typeof ($.signalR) !== "function") {
        throw "SignalR: SignalR is not loaded. Please ensure jquery.signalR.js is referenced before ~/signalr/hubs.";
    }

    var hubs = {},
        signalR = $.signalR,
        callbackId = 0,
        callbacks = {};

    // Array.prototype.map
    if (!Array.prototype.hasOwnProperty("map")) {
        Array.prototype.map = function (fun, thisp) {
            var arr = this,
                i,
                length = arr.length,
                result = [];
            for (i = 0; i < length; i += 1) {
                if (arr.hasOwnProperty(i)) {
                    result[i] = fun.call(thisp, arr[i], i, arr);
                }
            }
            return result;
        };
    }

    function executeCallback(hubName, fn, args, state) {
        var hub = hubs[hubName],
            hubMethod;

        if (hub) {
            signalR.hub.processState(hubName, hub.obj, state);

            hubMethod = hub.obj[fn];
            if (hubMethod) {
                hubMethod.apply(hub.obj, args);
            }
        }
    }

    function updateClientMembers(instance) {
        var newHubs = {},
            obj,
            memberValue,
            key,
            memberKey,
            hasSubscription = false;

        for (key in instance) {
            if (instance.hasOwnProperty(key)) {
                // This is a client hub
                obj = instance[key];

                if ($.type(obj) !== "object" ||
                        $.inArray(key, ["prototype", "constructor", "fn", "hub", "transports"]) >= 0) {
                    continue;
                }

                hasSubscription = false;

                for (memberKey in obj) {
                    if (obj.hasOwnProperty(memberKey)) {
                        memberValue = obj[memberKey];

                        if (memberKey === "_" ||
                                $.type(memberValue) !== "function" ||
                                $.inArray(memberKey, obj._.ignoreMembers) >= 0) {
                            continue;
                        }

                        hasSubscription = true;
                        break;
                    }
                }

                if (hasSubscription === true) {
                    newHubs[obj._.hubName] = { obj: obj };
                }
            }
        }

        hubs = {};
        $.extend(hubs, newHubs);
    }

    function getArgValue(a) {
        return $.isFunction(a)
            ? null
            : ($.type(a) === "undefined"
                ? null
                : a);
    }

    function copy(obj, exclude) {
        var newObj = {};
        $.each(obj, function (key, value) {
            if ($.inArray(key, exclude) === -1) {
                // We don't use "this" because browser suck!
                newObj[key] = value;
            }
        });

        return newObj;
    }

    function serverCall(hub, methodName, args) {
        var callback = args[args.length - 1], // last argument
            methodArgs = $.type(callback) === "function"
                ? args.slice(0, -1) // all but last
                : args,
            argValues = methodArgs.map(getArgValue),
            data = { hub: hub._.hubName, method: methodName, args: argValues, state: copy(hub, ["_"]), id: callbackId },
            d = $.Deferred(),
            cb = function (result) {
                signalR.hub.processState(hub._.hubName, hub, result.State);

                if (result.Error) {
                    if (result.StackTrace) {
                        signalR.hub.log(result.Error + "\n" + result.StackTrace);
                    }
                    d.rejectWith(hub, [result.Error]);
                } else {
                    if ($.type(callback) === "function") {
                        callback.call(hub, result.Result);
                    }
                    d.resolveWith(hub, [result.Result]);
                }
            };

        callbacks[callbackId.toString()] = { scope: hub, callback: cb };
        callbackId += 1;
        hub._.connection().send(window.JSON.stringify(data));
        return d;
    }

    // Create hub signalR instance
    $.extend(signalR, {
        userHub: {
            _: {
                hubName: 'UserHub',
                ignoreMembers: ['registerUserClient', 'namespace', 'ignoreMembers', 'callbacks'],
                connection: function () { return signalR.hub; }
            },

            registerUserClient: function (userId, callback) {
                return serverCall(this, "RegisterUserClient", $.makeArray(arguments));
            }
        },
        groupHub: {
            _: {
                hubName: 'GroupHub',
                ignoreMembers: ['joinGroups', 'joinGroup', 'leaveGroup', 'disconnect', 'namespace', 'ignoreMembers', 'callbacks'],
                connection: function () { return signalR.hub; }
            },

            joinGroups: function (userId, callback) {
                return serverCall(this, "JoinGroups", $.makeArray(arguments));
            },

            joinGroup: function (userId, groupId, callback) {
                return serverCall(this, "JoinGroup", $.makeArray(arguments));
            },

            leaveGroup: function (userId, groupId, callback) {
                return serverCall(this, "LeaveGroup", $.makeArray(arguments));
            },

            disconnect: function (callback) {
                return serverCall(this, "Disconnect", $.makeArray(arguments));
            }
        },
        debugHub: {
            _: {
                hubName: 'DebugHub',
                ignoreMembers: ['registerWithDebugger', 'namespace', 'ignoreMembers', 'callbacks'],
                connection: function () { return signalR.hub; }
            },

            registerWithDebugger: function (callback) {
                return serverCall(this, "RegisterWithDebugger", $.makeArray(arguments));
            }
        },
        chatHub: {
            _: {
                hubName: 'ChatHub',
                ignoreMembers: ['getChat', 'joinChat', 'exitChat', 'typing', 'sendChatMessage', 'namespace', 'ignoreMembers', 'callbacks'],
                connection: function () { return signalR.hub; }
            },

            getChat: function (chatId, callback) {
                return serverCall(this, "GetChat", $.makeArray(arguments));
            },

            joinChat: function (chatId, inviteeUserIds, groupId, callback) {
                return serverCall(this, "JoinChat", $.makeArray(arguments));
            },

            exitChat: function (chatId, callback) {
                return serverCall(this, "ExitChat", $.makeArray(arguments));
            },

            typing: function (chatId, isTyping, callback) {
                return serverCall(this, "Typing", $.makeArray(arguments));
            },

            sendChatMessage: function (chatId, messageId, message, callback) {
                return serverCall(this, "SendChatMessage", $.makeArray(arguments));
            }
        }
    });

    signalR.hub = signalR("/signalr")
        .starting(function () {
            updateClientMembers(signalR);
        })
        .sending(function () {
            var localHubs = [];

            $.each(hubs, function (key) {
                localHubs.push({ name: key });
            });

            this.data = window.JSON.stringify(localHubs);
        })
        .received(function (result) {
            var callbackId, cb;
            if (result) {
                if (!result.Id) {
                    executeCallback(result.Hub, result.Method, result.Args, result.State);
                } else {
                    callbackId = result.Id.toString();
                    cb = callbacks[callbackId];
                    if (cb) {
                        callbacks[callbackId] = null;
                        delete callbacks[callbackId];
                        cb.callback.call(cb.scope, result);
                    }
                }
            }
        });

    signalR.hub.processState = function (hubName, left, right) {
        $.extend(left, right);
    };

} (window.jQuery, window));});

/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />

// Require AMD config
// ------------------

// Setup config
require.config({
    baseUrl: '/js/bowerbird',
    paths: {
        jquery: 'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery', // jQuery is now AMD compliant
        json2: '../libs/json/json2',
        underscore: '../libs/underscore/underscore', // AMD version from https://github.com/amdjs
        backbone: '../libs/backbone/backbone', // AMD version from https://github.com/amdjs,
        marionette: '../libs/backbone.marionette/backbone.marionette',
        noext: '../libs/require/noext', //https://github.com/millermedeiros/requirejs-plugins
        async: '../libs/require/async', // Required by google loader
        goog: '../libs/require/goog', // Google async loader
        propertyParser: '../libs/require/propertyparser', // Required by google loader
        ich: '../libs/icanhaz/icanhaz', // Mustache templates cache and renderer
        jqueryui: '../libs/jqueryui',
        datepicker: '../libs/bootstrap/bootstrap-datepicker',
        date: '../libs/date/date',
        multiselect: '../libs/jquery.multiselect/jquery.multiselect',
        loadimage: '../libs/jquery.fileupload/load-image',
        fileupload: '../libs/jquery.fileupload/jquery.fileupload',
        iframetransport: '../libs/jquery.fileupload/jquery.iframe-transport',
        signalr: '../libs/jquery.signalr/jquery.signalr',
        timeago: '../libs/jquery.timeago/jquery.timeago',
        log: '../libs/log/log',
        hubs: 'hubs'//,
        //youtube: 'http://www.youtube.com/player_api',
        //player: '../libs/youtube/player'
    }
});

// Start app with primary dependancies
require([
        'app',
        'bootstrap-data', // Get bootstrapped data from inline AMD module
        'log',
        'backbone',
        'jquery',
        'ich',
        'marionette',
        'noext!/templates', // Load templates from server
        'controllers/usercontroller',
        'controllers/activitycontroller',
        'controllers/debugcontroller',
        'controllers/groupusercontroller',
        'controllers/homecontroller',
        'controllers/observationcontroller',
        'controllers/organisationcontroller',
        'controllers/postcontroller',
        'controllers/projectcontroller',
        'controllers/referencespeciescontroller',
        'controllers/speciescontroller',
        'controllers/teamcontroller',
        'controllers/accountcontroller',
        'controllers/chatcontroller',
        'views/headerview',
        'views/footerview',
        'views/sidebarlayoutview',
        'views/notificationscompositeview',
        'views/onlineusercompositeview',
        'views/chatcompositeview',
        'signalr',
        'hubs'//,
        //'youtube',
        //'player'
    ],
    function (app, bootstrapData) {
        log('bootstrapped data', bootstrapData);
        // Start the app as soon as the DOM is ready, loading in the bootstrapped data
        $(function () {
            app.start(bootstrapData);
        });
    });
define("../main", function(){});

/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountController & AccountRouter
// ---------------------------------

define('controllers/accountController',['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app, HomeLayoutView) {

    var AccountRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'account/login': 'showLogin',
            'account/register': 'showRegister'
        }
    });

    var AccountController = {};

    // Public API
    // ----------

    AccountController.showLogin = function () {
    };

    AccountController.showRegister = function () {
    };

    app.addInitializer(function () {
        this.accountRouter = new AccountRouter({
            controller: AccountController
        });
    });

    return AccountController;

});