#Für ein neues Release folgende Befehle: 
#Dazu in VisualStudio unter "View" ein Terminal Starten ( CTRL+ö)

# Relase-Folder rekursive löschen
..StockAppV2>rm .\StockApp.UI\bin\Release -r

# die Executable erstellen
..StockAppV2>dotnet publish StockApp.UI\StockApp.UI.csproj -r win-x64 -c Release -p:Version=1.0.3.0 --sc 

# Aus dem Dockerfile ein Image erstellen (nur, wenn das Image noch nicht vorhanden ist)
..StockAppV2>docker build -f .\StockApp.Installer\Dockerfile -t nsisimage .
(in WSL: docker build -f /mnt/c/Users/daniel/source/repos/StockAppV2/StockApp.Installer/Dockerfile  -t nsisimage .)

# Das Lizenzfile kopieren
..StockAppV2>copy .\LICENSE .\StockApp.Installer\License.txt

# MakeNSIS mit Docker starten (dadurch keine lokale installation von makensis)
..StockAppV2>docker run -it --rm -v C:\Users\daniel\source\repos\StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi
(in WSL: docker run -it --rm -v /mnt/c/Users/daniel/source/repos/StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi)



Build-Publish: (Version ändern in dieser Datei 2x, installer.nsi, StockApp.UI)
rm .\StockApp.UI\bin\Release -r
dotnet publish StockApp.UI\StockApp.UI.csproj -r win-x64 -c Release -p:Version=1.0.3.0 --sc 
copy .\LICENSE .\StockApp.Installer\License.txt
docker run -it --rm -v C:\Users\daniel\source\repos\StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi