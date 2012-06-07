/*!
* SignalR JavaScript Library v0.5
* http://signalr.net/
*
* Copyright David Fowler and Damian Edwards 2012
* Licensed under the MIT.
* https://github.com/SignalR/SignalR/blob/master/LICENSE.md
*
*/

(function(a,b){function g(a,b,e,f){var g=c[a],h;g&&(d.hub.processState(a,g.obj,f),h=g.obj[b],h&&h.apply(g.obj,e))}function h(b){var d={},e,f,g,h,i=!1;for(g in b)if(b.hasOwnProperty(g)){e=b[g];if(a.type(e)!=="object"||a.inArray(g,["prototype","constructor","fn","hub","transports"])>=0)continue;i=!1;for(h in e)if(e.hasOwnProperty(h)){f=e[h];if(h==="_"||a.type(f)!=="function"||a.inArray(h,e._.ignoreMembers)>=0)continue;i=!0;break}i===!0&&(d[e._.hubName]={obj:e})}c={},a.extend(c,d)}function i(b){return a.isFunction(b)?null:a.type(b)==="undefined"?null:b}function j(b,c){var d={};return a.each(b,function(b,e){a.inArray(b,c)===-1&&(d[b]=e)}),d}function k(c,g,h){var k=h[h.length-1],l=a.type(k)==="function"?h.slice(0,-1):h,m=l.map(i),n={hub:c._.hubName,method:g,args:m,state:j(c,["_"]),id:e},o=a.Deferred(),p=function(b){d.hub.processState(c._.hubName,c,b.State),b.Error?(b.StackTrace&&d.hub.log(b.Error+"\n"+b.StackTrace),o.rejectWith(c,[b.Error])):(a.type(k)==="function"&&k.call(c,b.Result),o.resolveWith
(c,[b.Result]))};return f[e.toString()]={scope:c,callback:p},e+=1,c._.connection().send(b.JSON.stringify(n)),o}if(typeof a.signalR!="function")throw"SignalR: SignalR is not loaded. Please ensure jquery.signalR.js is referenced before ~/signalr/hubs.";var c={},d=a.signalR,e=0,f={};Array.prototype.hasOwnProperty("map")||(Array.prototype.map=function(a,b){var c=this,d,e=c.length,f=[];for(d=0;d<e;d+=1)c.hasOwnProperty(d)&&(f[d]=a.call(b,c[d],d,c));return f}),a.extend(d,{}),d.hub=d("{serviceUrl}").starting(function(){h(d)}).sending(function(){var d=[];a.each(c,function(a){d.push({name:a})}),this.data=b.JSON.stringify(d)}).received(function(a){var b,c;a&&(a.Id?(b=a.Id.toString(),c=f[b],c&&(f[b]=null,delete f[b],c.callback.call(c.scope,a))):g(a.Hub,a.Method,a.Args,a.State))}),d.hub.processState=function(b,c,d){a.extend(c,d)}})(window.jQuery,window)