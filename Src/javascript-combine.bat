if %1 == DebugProd (
node %2\js\build\r.js -o %2\js\build\app.build.js optimize=none out=..\js\main-min.js
)

if %1 == Prod (
node %2\js\build\r.js -o %2\js\build\app.build.js optimize=uglify out=..\js\main-min.js
)

exit 0