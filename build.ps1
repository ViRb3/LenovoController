msbuild source/LenovoController.sln /p:Configuration=Release /m /verbosity:normal /p:WarningLevel=0
Rename-Item -Path "source/bin/Release" -NewName "LenovoController"
Compress-Archive -Path "source/bin/LenovoController" -DestinationPath LenovoController.zip