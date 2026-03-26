dotnet publish -r osx-arm64 -p:PublishSingleFile=true --self-contained true
cp -f ./bin/Release/net10.0/osx-arm64/publish/jmwmachineinfo ~/projects/walljm/personal/profiles/mac/utils
 
