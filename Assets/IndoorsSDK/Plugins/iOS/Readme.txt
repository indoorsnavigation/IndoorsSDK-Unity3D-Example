1. Необходимо отключить bitcode в таргетах приложения и UnityFramework (добавил в постпроцесс)
2. Включить PlayerSettings -> Enable Custom Background Behaviors->Location updates
3. Вставить три поля в plist:
<key>NSLocationWhenInUseUsageDescription</key>
<string>This application uses beacon monitoring even in background for testing beacon notifications and indoor positioning in building.</string>
<key>NSLocationAlwaysUsageDescription</key>
<string>This application uses beacon monitoring even in background for testing beacon notifications and indoor positioning in building.</string>
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>This application uses beacon monitoring even in background for testing beacon notifications and indoor positioning in building.</string>

Сейчас client_id и client_secret стоят из тестового приложения. Позже будет актуальное.


Команды для уничтожения архитектур:
lipo -remove x86_64 IndoorsSDK.framework/IndoorsSDK -o IndoorsSDK.framework/IndoorsSDK
lipo -remove x86_64 Polyline.framework/Polyline -o Polyline.framework/Polyline
lipo -remove i386 Polyline.framework/Polyline -o Polyline.framework/Polyline
lipo -remove i386 Mapbox.framework/Mapbox -o Mapbox.framework/Mapbox
lipo -remove x86_64 Mapbox.framework/Mapbox -o Mapbox.framework/Mapbox
lipo -remove i386 MapboxDirections.framework/MapboxDirections -o MapboxDirections.framework/MapboxDirections
lipo -remove x86_64 MapboxDirections.framework/MapboxDirections -o MapboxDirections.framework/MapboxDirections