Für ein neues Release folgender Befehl:
im Ordner der Solution "StockAppV2"
..StockAppV2>dotnet publish StockApp.UI\StockApp.UI.csproj -r win-x64 -c Release --sc 



Dockerfile um NSIS im Container zu starten und somit keine lokale Installation von MAKENSIS
um aus dem Dockerfile ein Image zu erstellen 
-> docker build -f /mnt/c/Users/daniel/source/repos/StockAppV2/StockApp.Installer/Dockerfile  -t nsisimage .

Dann kann das Image gestartet werden und die installer.nsi genutzt werden
-> docker run -it --rm -v /mnt/c/Users/daniel/source/repos/StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi


evtl. die Pfade noch anpassen...



