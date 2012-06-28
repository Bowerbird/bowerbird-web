
IF [%1]==[] (
	set BUILD_CONFIG=debug
) ELSE (
	set BUILD_CONFIG=%1
)

IF [%2]==[] (
	set BUILD_NUMBER=0.1.0.0.1.1.1
) ELSE (
	set BUILD_NUMBER=%2
)

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe build.proj /t:Build /verbosity:detailed /p:Configuration=%BUILD_CONFIG% /p:BuildNumber=%BUILD_NUMBER%