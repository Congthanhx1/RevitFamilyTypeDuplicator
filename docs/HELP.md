# CopyFam Help

## About

CopyFam duplicates a Revit Family Type and creates a numbered series of Type names while preserving the source Type parameters.

## Installation

The Autodesk App Store installer installs the add-in automatically. Close Autodesk Revit before installing. Restart Revit after installation.

## Usage

1. Optionally select a Family Instance in the active Revit model.
2. Open the Revit **Add-Ins** tab.
3. In the **CopyFam** panel, click **Copy & rename**.
4. Select **English** or **Tiếng Việt**.
5. Choose the Category, Family, and source Type.
6. Enter the name prefix, start number, and end number.
7. Review the generated-name preview.
8. Click **Create Types**.

Names that already exist in the source Family are skipped. The entire operation is a single Revit transaction and can be undone with Revit's Undo command.

## Uninstallation

Close Autodesk Revit, then uninstall CopyFam from **Windows Settings > Apps > Installed apps**, or rerun the Autodesk App Store installer and choose uninstall.

## Known limitations

- CopyFam duplicates loadable `FamilySymbol` Types. Revit system-family Types are not included.
- The prebuilt Marketplace package currently targets Autodesk Revit 2027.
- A source Type must belong to the active Revit document.

## Privacy

CopyFam does not collect or transmit data. See [Privacy Policy](../PRIVACY.md).

## Support

Report a problem through [GitHub Issues](https://github.com/Congthanhx1/RevitFamilyTypeDuplicator/issues) or email [thanhtklam990@gmail.com](mailto:thanhtklam990@gmail.com). Include the Revit version, steps to reproduce, and any error message.
