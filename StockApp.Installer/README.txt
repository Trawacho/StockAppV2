Dockerfile um NSIS im Container zu starten und somit keine lokale Installation von MAKENSIS
um aus dem Dockerfile ein Image zu erstellen 
-> docker build -t nsisimage .

Dann kann das Image gestartet werden und die installer.nsi genutzt werden
-> docker run -it --rm -v /mnt/c/Users/daniel/source/repos/StockAppV2:/build ns /build/StockApp.Installer/installer.nsi


evtl. die Pfade noch anpassen...



