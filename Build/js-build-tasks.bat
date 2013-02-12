REM %1\Lib\signalr.exe ghp /o:%2\js\bowerbird\hubs.js /path:%1\src\bowerbird.web\bin

node %1\build\r.js -o %1\build\app.build.js optimize=none out=..\js\main-combined.js

%1\Lib\AjaxMin.exe %2\js\main-combined.js -out %2\js\main-min.js -clobber