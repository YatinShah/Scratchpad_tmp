REM SONARQUBE Scanning for the dapr project.
REM makesure dotnet sonarscanner is installed globally

REM dotnet tool install --global dotnet-sonarscanner

SET dapr_sq_token=sqp_adb90aa1e9452ac17ca0297bbda15129ea98fd68
SET SQ_Server=http://localhost:9000

dotnet sonarscanner begin /k:"dapr" /d:sonar.host.url="%SQ_Server%"  /d:sonar.token="%dapr_sq_token%"

dotnet build Name2Phone.sln

dotnet sonarscanner end /d:sonar.token="%dapr_sq_token%"