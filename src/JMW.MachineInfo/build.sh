dotnet publish -r osx-arm64 -p:PublishSingleFile=true --self-contained true
cp -f ./bin/Release/net10.0/osx-arm64/publish/jmwmachineinfo ~/projects/walljm/personal/profiles/mac/utils

dotnet publish -r linux-arm64 -p:PublishSingleFile=true --self-contained true
cp -f ./bin/Release/net10.0/linux-arm64/publish/jmwmachineinfo ~/projects/walljm/personal/profiles/bash/utils/jmwmachineinfo-linux-arm64

dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true
cp -f ./bin/Release/net10.0/linux-x64/publish/jmwmachineinfo ~/projects/walljm/personal/profiles/bash/utils/jmwmachineinfo-linux-x64

