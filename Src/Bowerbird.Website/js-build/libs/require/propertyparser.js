/**
 * Basic parser for URL properties
 * @author Miller Medeiros
 * @version 0.1.0 (2011/12/06)
 * MIT license
 */

define([],function(){function c(b){var c,e={};while(c=a.exec(b))e[c[1]]=d(c[2]||c[3]);return e}function d(a){return b.test(a)?a=a.replace(b,"$1").split(","):a==="null"?a=null:a==="false"?a=!1:a==="true"?a=!0:a===""||a==="''"||a==='""'?a="":isNaN(a)||(a=+a),a}var a=/([\w-]+)\s*:\s*(?:(\[[^\]]+\])|([^,]+)),?/g,b=/^\[([^\]]+)\]$/;return{parseProperties:c,typecastVal:d}})