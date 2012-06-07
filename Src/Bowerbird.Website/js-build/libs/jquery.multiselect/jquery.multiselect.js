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

define(["jquery"],function(jQuery){jQuery&&function($){function renderOption(a,b){var c='<label><input style="display:none;" type="checkbox" name="'+a+'[]" value="'+b.value+'"';return b.selected&&(c+=' checked="checked"'),c+=" />"+b.text+"</label>",c}function renderOptions(a,b,c){var d="";for(var e=0;e<b.length;e++)b[e].optgroup?(d+='<label class="optGroup">',c.optGroupSelectable?d+='<input type="checkbox" class="optGroup" />'+b[e].optgroup:d+=b[e].optgroup,d+='</label><div class="optGroupContainer">',d+=renderOptions(a,b[e].options,c),d+="</div>"):d+=c.renderOption(a,b[e]);return b.length==0&&(d+='<label style="padding:0 13px">'+(c.noOptionsText?c.noOptionsText:"-")+"</label>"),d}function buildOptions(a){var b=$(this),c=b.next(".multiSelectOptions"),d=b.data("config"),e=b.data("callback");c.html("");var f="";d.messageText&&(f+='<div class="selectMessage">'+d.messageText+"</div>"),d.selectAll&&(f+='<label class="selectAll"><input type="checkbox" class="selectAll" />'+d.selectAllText+"</label>"
),f+=renderOptions(b.attr("id"),a,d),c.html(f);var g=c.width(),h=!1;c.height()>d.listHeight?(c.css("height",d.listHeight+"px"),h=!0):c.css("height","");var i=h&&g==c.width()?17:0;c.width()+i<b.outerWidth()?c.css("width",b.outerWidth()-2+"px"):c.css("width",c.width()+i+"px"),$.fn.bgiframe&&b.next(".multiSelectOptions").bgiframe({width:c.width(),height:c.height()}),d.selectAll&&c.find("INPUT.selectAll").click(function(){c.find("INPUT:checkbox").attr("checked",$(this).attr("checked")).parent("LABEL").toggleClass("checked",$(this).attr("checked"))}),d.optGroupSelectable&&(c.addClass("optGroupHasCheckboxes"),c.find("INPUT.optGroup").click(function(){$(this).parent().next().find("INPUT:checkbox").attr("checked",$(this).attr("checked")).parent("LABEL").toggleClass("checked",$(this).attr("checked"))})),c.find("INPUT:checkbox").click(function(){var a=b.data("config"),c=b.next(".multiSelectOptions");a.singleSelect&&c.find("INPUT:checkbox").not($(this)).attr("checked",!1).parent("LABEL").toggleClass
("checked",!1),$(this).parent("LABEL").toggleClass("checked",$(this).attr("checked")),updateSelected.call(b),b.focus(),$(this).parent().parent().hasClass("optGroupContainer")&&updateOptGroup.call(b,$(this).parent().parent().prev()),e&&e($(this)),a.singleSelect&&b.multiSelectOptionsHide()}),c.each(function(){$(this).find("INPUT:checked").parent().addClass("checked")}),updateSelected.call(b),d.optGroupSelectable&&c.find("LABEL.optGroup").each(function(){updateOptGroup.call(b,$(this))}),c.find("LABEL:has(INPUT)").hover(function(){$(this).parent().find("LABEL").removeClass("hover"),$(this).addClass("hover")},function(){$(this).parent().find("LABEL").removeClass("hover")}),b.keydown(function(a){var c=$(this).next(".multiSelectOptions");if(c.css("visibility")!="hidden"){if(a.keyCode==9)return $(this).addClass("focus").trigger("click"),$(this).focus().next(":input").focus(),!0;(a.keyCode==27||a.keyCode==37||a.keyCode==39)&&$(this).addClass("focus").trigger("click");if(a.keyCode==40||a.keyCode==38
){var f=c.find("LABEL"),g=f.index(f.filter(".hover")),h=-1;return g<0?c.find("LABEL:first").addClass("hover"):a.keyCode==40&&g<f.length-1?h=g+1:a.keyCode==38&&g>0&&(h=g-1),h>=0&&($(f.get(g)).removeClass("hover"),$(f.get(h)).addClass("hover"),adjustViewPort(c)),!1}if(a.keyCode==13||a.keyCode==32){var i=c.find("LABEL.hover INPUT:checkbox");return d.singleSelect&&c.find("INPUT:checkbox").not(i).attr("checked",!1).parent("LABEL").toggleClass("checked",!1),i.attr("checked",!i.attr("checked")).parent("LABEL").toggleClass("checked",i.attr("checked")),i.hasClass("selectAll")&&c.find("INPUT:checkbox").attr("checked",i.attr("checked")).parent("LABEL").addClass("checked").toggleClass("checked",i.attr("checked")),updateSelected.call(b),e&&e($(this)),d.singleSelect&&b.multiSelectOptionsHide(),!1}if(a.keyCode>=33&&a.keyCode<=126){var j=c.find("LABEL:startsWith("+String.fromCharCode(a.keyCode)+")"),k=j.index(j.filter("LABEL.hover")),l=j.filter(function(a){return a>k});j=(l.length>=1?l:j).filter("LABEL:first"
),j.length==1&&(c.find("LABEL.hover").removeClass("hover"),j.addClass("hover"),adjustViewPort(c))}}else{if(a.keyCode==38||a.keyCode==40||a.keyCode==13||a.keyCode==32)return $(this).removeClass("focus").trigger("click"),c.find("LABEL:first").addClass("hover"),!1;if(a.keyCode==9)return c.next(":input").focus(),!0}if(a.keyCode==13)return!1})}function adjustViewPort(a){var b=a.find("LABEL.hover").position().top+a.find("LABEL.hover").outerHeight();b>a.innerHeight()&&a.scrollTop(a.scrollTop()+b-a.innerHeight()),a.find("LABEL.hover").position().top<0&&a.scrollTop(a.scrollTop()+a.find("LABEL.hover").position().top)}function updateOptGroup(a){var b=$(this),c=b.data("config");if(c.optGroupSelectable){var d=!0;$(a).next().find("INPUT:checkbox").each(function(){if(!$(this).attr("checked"))return d=!1,!1}),$(a).find("INPUT.optGroup").attr("checked",d).parent("LABEL").toggleClass("checked",d)}}function updateSelected(){var a=$(this),b=a.next(".multiSelectOptions"),c=a.data("config"),d=0,e=!0,f="",g=[
];b.find("INPUT:checkbox").not(".selectAll, .optGroup").each(function(){$(this).attr("checked")?(d++,f=f+$(this).parent().text()+", ",g.push({text:$(this).parent().text(),value:$(this).attr("value")})):e=!1}),f=f.replace(/\s*\,\s*$/,""),d==0?a.find("span").html(c.noneSelected):typeof c.oneOrMoreSelected=="function"?a.find("span").html(c.oneOrMoreSelected(g)):c.oneOrMoreSelected=="*"?(a.find("span").html(f),a.attr("title",f)):a.find("span").html(c.oneOrMoreSelected.replace("%",d)),c.selectAll&&b.find("INPUT.selectAll").attr("checked",e).parent("LABEL").toggleClass("checked",e)}$.extend($.fn,{multiSelect:function(a,b){a||(a={}),a.selectAll==undefined&&(a.selectAll=!0),a.selectAllText==undefined&&(a.selectAllText="Select All"),a.noneSelected==undefined&&(a.noneSelected="Select options"),a.oneOrMoreSelected==undefined&&(a.oneOrMoreSelected="% selected"),a.optGroupSelectable==undefined&&(a.optGroupSelectable=!1),a.listHeight==undefined&&(a.listHeight=150),a.renderOption==undefined&&(a.renderOption=
renderOption),a.singleSelect==undefined&&(a.singleSelect=!1),$(this).each(function(){var c=$(this),d='<a href="javascript:;" class="multiSelect"><span></span><i></i></a>';d+='<div class="multiSelectOptions" style="position: absolute; z-index: 99999; visibility: hidden;"></div>',$(c).after(d);var e=$(c).next(".multiSelect"),f=e.next(".multiSelectOptions");e.data("config",a),e.data("callback",b);var g=[];$(c).children().each(function(){if(this.tagName.toUpperCase()=="OPTGROUP"){var a=[];g.push({optgroup:$(this).attr("label"),options:a}),$(this).children("OPTION").each(function(){$(this).val()!=""&&a.push({text:$(this).html(),value:$(this).val(),selected:$(this).attr("selected")})})}else this.tagName.toUpperCase()=="OPTION"&&$(this).val()!=""&&g.push({text:$(this).html(),value:$(this).val(),selected:$(this).attr("selected")})}),$(c).remove(),e.attr("id",$(c).attr("id")),buildOptions.call(e,g),e.hover(function(){$(this).addClass("hover")},function(){$(this).removeClass("hover")}).click(function(
){return $(this).hasClass("active")?$(this).multiSelectOptionsHide():$(this).multiSelectOptionsShow(),!1}).focus(function(){$(this).addClass("focus")}).blur(function(){$(this).removeClass("focus")}),$(document).click(function(a){$(a.target).parents().andSelf().is(".multiSelectOptions")||e.multiSelectOptionsHide()})})},multiSelectOptionsUpdate:function(a){buildOptions.call($(this),a)},multiSelectOptionsHide:function(){$(this).removeClass("active").removeClass("hover").next(".multiSelectOptions").css("visibility","hidden"),$(this).removeClass("active").removeClass("hover").next(".multiSelectOptions").css("display","none")},multiSelectOptionsShow:function(){var a=$(this),b=a.next(".multiSelectOptions"),c=a.data("config");$(".multiSelect").multiSelectOptionsHide(),b.find("LABEL").removeClass("hover"),a.addClass("active").next(".multiSelectOptions").css("visibility","visible"),a.addClass("active").next(".multiSelectOptions").css("display","block"),a.focus(),a.next(".multiSelectOptions").scrollTop
(0),b.height()>c.listHeight?b.css("height",c.listHeight+"px"):b.css("height",""),b.css("width",a.outerWidth()-2+"px");var d=a.position();a.next(".multiSelectOptions").css({top:d.top+$(this).outerHeight()+"px"}),a.next(".multiSelectOptions").css({left:d.left+"px"})},selectedValuesString:function(){var a="";return $(this).next(".multiSelectOptions").find("INPUT:checkbox:checked").not(".optGroup, .selectAll").each(function(){a+=$(this).attr("value")+","}),a.replace(/\s*\,\s*$/,"")}}),$.expr[":"].startsWith=function(el,i,m){var search=m[3];return search?eval("/^[/s]*"+search+"/i").test($(el).text()):!1}}(jQuery)})