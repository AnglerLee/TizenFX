sdb root on  
sdb push ..\..\src\Tizen.AIAvatar\bin\Debug\net6.0\Tizen.AIAvatar.dll /usr/share/dotnet.tizen/framework/Tizen.AIAvatar.dll
sdb shell chsmack -a _ /usr/share/dotnet.tizen/framework/Tizen.AIAvatar.dll
sdb shell app_launcher -s AIAvatar.Sample  

