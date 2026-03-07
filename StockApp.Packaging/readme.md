Das publishen über die GUI ist tlw. nicht möglich, da Fehler auftreten können.
Daher hier ein paar Befehle, wie es über die Konsole funktioniert.

1. Vorbereitung  
Diese Befehle stellen die für das Projekt StockAppV2.sln benötigten Abhängigkeiten und NuGet-Pakete wieder her, wobei sie spezifisch auf zwei unterschiedliche Zielplattformen optimiert werden. Durch den Parameter -r (Runtime Identifier) werden zusätzlich die plattformspezifischen Bibliotheken für Windows 32-Bit (x86) sowie Windows 64-Bit (x64) vorbereitet. Dies stellt sicher, dass die Anwendung für beide Prozessorarchitekturen über alle notwendigen Komponenten verfügt, um später korrekt kompiliert und ausgeführt werden zu können.
~~~  
    dotnet restore .\StockAppV2.sln -r win-x86  
    dotnet restore .\StockAppV2.sln -r win-x64
~~~
2. Publish  
Der folgende Befehl kompiliert das Packaging-Projekt und erstellt ein gebündeltes App-Paket:
~~~
  msbuild ".\StockApp.Packaging\StockApp.Packaging.wapproj" /t:"Restore;Publish"  `
   /p:Configuration=Release `
   /p:Platform=x64 `
   /p:AppxBundle=Always `
   /p:AppxBundlePlatforms="x86|x64" `
   /p:UapAppxPackageBuildMode=StoreUpload `
   /p:AppxPackageDir="C:\Users\daniel\source\repos\StockAppV2\StockApp.Packaging\AppPackages\" `
   /p:AppxSymbolPackageEnabled=True `
   /p:AppxPackageSigningEnabled=false
~~~

Was dieser Befehl bewirkt:  
- Restore & Publish:  
Er stellt zuerst alle Abhängigkeiten wieder her (Restore) und führt anschließend den Veröffentlichungsprozess (Publish) durch.
- Release-Konfiguration:  
Die Anwendung wird für die produktive Nutzung optimiert kompiliert (/p:Configuration=Release).
- Multi-Plattform Bundle:  
Es wird ein kombiniertes App-Bundle (.msixbundle oder .appxbundle) erstellt, das sowohl x86- als auch x64-Architekturen unterstützt.
- Store-Vorbereitung:  
Der Modus StoreUpload generiert zusätzlich eine .msixupload-Datei, die alle für den Microsoft Store notwendigen Artefakte enthält.
- Symbole & Signierung: 
Er erstellt Symbol-Pakete für das Debugging (AppxSymbolPackageEnabled=True), überspringt jedoch die lokale digitale Signierung (AppxPackageSigningEnabled=false), da diese in der Regel erst beim Store-Upload oder durch einen CI/CD-Prozess erfolgt.
- Ausgabeverzeichnis:  
Das fertige Paket wird im spezifischen Pfad unter \AppPackages\ abgelegt.  

WICHTIG: in allen .cspoj ist folgender Eintrag notwendig, damit das Packaging-Projekt die Abhängigkeiten korrekt findet:
~~~
<RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>
~~~
Weitere Informationen: https://docs.microsoft.com/de-de/windows/msix/desktop/desktop-to-uwp-packaging-dot-net
Hinweis: Es muss das MSBuild verwendet werden, welches zur installierten Visual Studio Version passt.



