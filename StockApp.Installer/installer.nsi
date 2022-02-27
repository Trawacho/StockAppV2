;--------------------------------
!include FileAssociation.nsh 
!include MUI2.nsh 


!ifndef FULL_VERSION
!define FULL_VERSION			"1.0.0.0"
!endif
!ifndef SOURCE_DIR
!define SOURCE_DIR				"..\StockApp.UI\bin\Release\net6.0-windows\win-x64"
!endif
!ifndef INSTALLER_FILENAME
!define INSTALLER_FILENAME		"StockAppInstaller.exe"
!endif

!ifndef COMPANY_NAME
!define COMPANY_NAME			""
!endif

!ifndef COPYRIGHT_TXT
!define COPYRIGHT_TXT			"(c) Copyright 2022"
!endif

!ifndef FILE_DESC
!define FILE_DESC			    "StockApp"
!endif

!ifndef ICON_FILE
!define ICON_FILE				"..\StockApp.UI\Ressources\StockApp.ico"
!endif

!ifndef IMAGE_FINISHED
!define IMAGE_FINISHED			"..\StockApp.UI\Ressources\StockApp.bmp"
!endif


!ifndef PRODUCT_NAME
!define PRODUCT_NAME			"StockApp"
!endif

!define APP_NAME				"${PRODUCT_NAME}"
!define SHORT_NAME				"${PRODUCT_NAME}"
!define APP_EXE                 "StockApp.UI.exe"

;;USAGE:
!define MIN_FRA_MAJOR "2"
!define MIN_FRA_MINOR "0"
!define MIN_FRA_BUILD "*"

!addplugindir		"."

;--------------------------------

; The name of the installer
Name "${APP_NAME}"
Caption "${APP_NAME}"
BrandingText "${APP_NAME}"

; The file to write
OutFile "${INSTALLER_FILENAME}"

; The default installation directory
InstallDir "$PROGRAMFILES32\${APP_NAME}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKCU "Software\Runner" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel highest


VIProductVersion "${FULL_VERSION}"
VIAddVersionKey /LANG=1031 "FileVersion" "${FULL_VERSION}"
VIAddVersionKey /LANG=1031 "ProductVersion" "${FULL_VERSION}"
VIAddVersionKey /LANG=1031 "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey /LANG=1031 "CompanyName" "${PRODUCT_PUBLISHER}"
VIAddVersionKey /LANG=1031 "LegalCopyright" "${COPYRIGHT_TXT}"
VIAddVersionKey /LANG=1031 "FileDescription" "${FILE_DESC}"



!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP_NOSTRETCH
!define MUI_ICON 						"${ICON_FILE}"
;!define MUI_WELCOMEFINISHPAGE_BITMAP	"${IMAGE_FINISHED}"
!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH


;--------------------------------

; Pages
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
    # These indented statements modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN_TEXT "Start ${PRODUCT_NAME}"
    !define MUI_FINISHPAGE_RUN "$INSTDIR\${APP_EXE}"
!insertmacro MUI_PAGE_FINISH


UninstPage uninstConfirm
UninstPage instfiles

!insertmacro MUI_LANGUAGE "German"
;--------------------------------

; The stuff to install
Section `${APP_NAME}`
  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
;;;  File "${LICENSE_NAME}"
  File /r "${SOURCE_DIR}\*.*"
  
  ; Write the uninstall keys for Windows
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "DisplayName" "${APP_NAME}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "NoModify" 1
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "NoRepair" 1
  WriteUninstaller $INSTDIR\uninstall.exe
  
  ${registerExtension} "$INSTDIR\${APP_EXE}" ".skmr" "StockAPP File"
  
  
  ; Add an application to the firewall exception list - All Networks - All IP Version - Enabled
  SimpleFC::AddApplication "${APP_NAME}" "$INSTDIR\${APP_EXE}" 0 2 "" 1
  Pop $0 ; return error(1)/success(0)

SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\${APP_NAME}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}" "" "$INSTDIR\${APP_EXE}" 0
  
SectionEnd


; Optional section (can be enabled by the user)
Section /o "Desktop shortcut"

	CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}" "" "$INSTDIR\${APP_EXE}"
  
SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"
  ; Remove registry keys
  DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}"

  ${unregisterExtension} ".skmr" "StockAPP File"

  ; Remove an application from the firewall exception list
  SimpleFC::RemoveApplication "$INSTDIR\${APP_EXE}"
  Pop $0 ; return error(1)/success(0)
	
  ; Remove files and uninstaller (everything)
  RMDir /r "$INSTDIR"

  ; Remove desktop icon
  Delete "$DESKTOP\${APP_NAME}.lnk" 

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${APP_NAME}\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\${APP_NAME}"
  RMDir "$INSTDIR"

SectionEnd