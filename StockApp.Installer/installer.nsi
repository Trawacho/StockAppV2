﻿;--------------------------------
!include FileAssociation.nsh 
!include LogicLib.nsh
!include MUI2.nsh 


!ifndef FULL_VERSION
!define FULL_VERSION      "1.0.0.0"
!endif

!ifndef SOURCE_DIR
!define SOURCE_DIR        "..\StockApp.UI\bin\Release\net6.0-windows\win-x64\publish"
!endif

!ifndef INSTALLER_FILENAME
!define INSTALLER_FILENAME    "StockAppInstaller.exe"
!endif

!ifndef COMPANY_NAME
!define COMPANY_NAME      "Sturm Daniel"
!endif

!ifndef COPYRIGHT_TXT
!define COPYRIGHT_TXT     "(c) Copyright 2022"
!endif

!ifndef FILE_DESC
!define FILE_DESC         "StockApp"
!endif

!ifndef ICON_FILE
!define ICON_FILE         "..\StockApp.UI\Ressources\StockApp.ico"
!endif

!ifndef IMAGE_FINISHED
!define IMAGE_FINISHED    "..\StockApp.UI\Ressources\StockApp.bmp"
!endif

!ifndef LICENCSEFILE
!define LICENSEFILE       "..\LICENSE"
!endif

!ifndef PRODUCT_NAME
!define PRODUCT_NAME      "StockApp"
!endif

!define APP_NAME          "${PRODUCT_NAME}"
!define SHORT_NAME        "${PRODUCT_NAME}"
!define APP_EXE           "StockApp.UI.exe"

#USAGE:
!define VERSIONMAJOR "0"
!define VERSIONMINOR "1"
!define VERSIONBUILD "*"

!addplugindir		"."

;--------------------------------

; The name of the installer
Name "${APP_NAME}"
Caption "${APP_NAME}"
BrandingText "${APP_NAME}"
Icon "${ICON_FILE}"

; The file to write
OutFile "${INSTALLER_FILENAME}"

; The default installation directory
InstallDir "$PROGRAMFILES\${APP_NAME}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKCU "Software\StockApp" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel highest

page license
page directory
page instfiles


!macro VerifyUserIsAdmin
UserInfo::GetAccountType
pop $0
${If} $0 != "admin" ;Require admin rights on NT4+
        messageBox mb_iconstop "Administrator rights required!"
        setErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
        quit
${EndIf}
!macroend

function .onInit
	setShellVarContext all
	!insertmacro VerifyUserIsAdmin
functionEnd

;--------------------------------

# The stuff to install
Section "install"
  
  # Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  # Put file there
  File /r  /x *.pdb "${SOURCE_DIR}\*.*"

  WriteUninstaller $INSTDIR\uninstall.exe

  # Start Menu
  CreateDirectory "$SMPROGRAMS\${APP_NAME}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}" "" "$INSTDIR\${APP_EXE}"

  # Desktop ShortCut
  CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}" "" "$INSTDIR\${APP_EXE}"
  
  # Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "DisplayName" "${APP_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "InstallLocation" "$\"$INSTDIR$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "DisplayVersion" "$\"${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}$\""

  # There is no option for modifying or repairing the install
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}" "NoRepair" 1

  # File Association
  ${registerExtension} "$INSTDIR\${APP_EXE}" ".skmr" "StockAPP File"
  
  # Add an application to the firewall exception list - All Networks - All IP Version - Enabled
  SimpleFC::AddApplication "${APP_NAME}" "$INSTDIR\${APP_EXE}" 0 2 "" 1
  Pop $0 ; return error(1)/success(0)

SectionEnd

# --------------------------------


# Uninstaller

function un.onInit
	SetShellVarContext all
 
	#Verify the uninstaller - last chance to back out
	MessageBox MB_OKCANCEL "Permanantly remove ${APPNAME}?" IDOK next
		Abort
	next:
	!insertmacro VerifyUserIsAdmin
functionEnd

Section "Uninstall"
  
  # Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${SHORT_NAME}"

  # Remove FileAssociation
  ${unregisterExtension} ".skmr" "StockAPP File"

  # Remove an application from the firewall exception list
  SimpleFC::RemoveApplication "$INSTDIR\${APP_EXE}"
  Pop $0 ; return error(1)/success(0)
	
  # Remove files and uninstaller (everything)
  RMDir /r "$INSTDIR"

  # Remove desktop icon
  Delete "$DESKTOP\${APP_NAME}.lnk" 

  # Remove shortcuts, if any
  Delete "$SMPROGRAMS\${APP_NAME}\*.*"

  # Remove directories used
  RMDir "$SMPROGRAMS\${APP_NAME}"
  RMDir "$INSTDIR"

SectionEnd