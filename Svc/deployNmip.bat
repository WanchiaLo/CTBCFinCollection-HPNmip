setlocal

echo Set Variable


set _sourcePath=.\HPNmip\bin\Release


set _deployPath=\\10.88.20.38\D$
set _deployPath2=\\10.88.20.39\D$



echo copy deploy

xcopy "%_sourcePath%\HPNmip.exe" "%_deployPath%\"  /Y /H /R
xcopy "%_sourcePath%\HPNmip.exe" "%_deployPath2%\"  /Y /H /R


endlocal
 