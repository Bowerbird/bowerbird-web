set jsCombined=false
set jsMinified=false

if %1 == DebugProd set jsCombined=true
if %1 == Test set jsCombined=true
if %1 == Stage set jsMinified=true
if %1 == Prod set jsMinified=true

if "%jsCombined%" == "true" (
	node %3\build\r.js -o %3\build\app.build.js optimize=none out=..\js\main-combined.js
)

if "%jsMinified%" == "true" (
	node %3\build\r.js -o %3\build\app.build.js optimize=uglify out=..\js\main-min.js
)
