#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Prepares a clean client delivery package for PharmacyInventory
.DESCRIPTION
    - Cleans previous builds
    - Publishes as Release win-x64 self-contained single-file executable
    - Creates delivery folder on Desktop
    - Copies all published files
    - Removes pharmacy.db
    - Creates README.txt with client instructions
    - Optionally creates ZIP archive
.NOTES
    Run from project root: C:\Users\hp\Desktop\New folder\PharmacyInventory\
    Usage: .\PrepareClientDelivery.ps1 [-CreateZip]
#>

param(
    [switch]$CreateZip = $false
)

# ============================================================================
# Configuration
# ============================================================================
$ProjectFoldername     = "New folder"
$ProjectPath           = "C:\Users\hp\Desktop\$ProjectFoldername\PharmacyInventory"
$PublishOutput         = "$ProjectPath\bin\Release\net8.0-windows\win-x64\publish"
$DeliveryFolder        = "C:\Users\hp\Desktop\PharmacyInventory_ClientDelivery"
$DeliveryZip           = "C:\Users\hp\Desktop\PharmacyInventory_ClientDelivery.zip"
$DatabaseFileName      = "pharmacy.db"

# ============================================================================
# Helper Functions
# ============================================================================

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-Host "=" * 70
    Write-Host $Text
    Write-Host "=" * 70
}

function Write-Success {
    param([string]$Text)
    Write-Host "✓ $Text" -ForegroundColor Green
}

function Write-Info {
    param([string]$Text)
    Write-Host "ℹ $Text" -ForegroundColor Cyan
}

function Write-Error-Custom {
    param([string]$Text)
    Write-Host "✗ $Text" -ForegroundColor Red
}

# ============================================================================
# Main Script
# ============================================================================

Write-Header "PharmacyInventory Client Delivery Package Preparation"

# Step 1: Verify project path
Write-Info "Verifying project path: $ProjectPath"
if (-not (Test-Path $ProjectPath)) {
    Write-Error-Custom "Project path not found: $ProjectPath"
    exit 1
}
Write-Success "Project path verified"

# Step 2: Clean previous builds
Write-Header "Step 1: Cleaning previous builds..."
try {
    Push-Location $ProjectPath
    Write-Info "Running: dotnet clean"
    dotnet clean -c Release | Out-Null
    Write-Success "Previous builds cleaned"
}
catch {
    Write-Error-Custom "Failed to clean: $_"
    exit 1
}

# Step 3: Publish Release build
Write-Header "Step 2: Publishing Release build (win-x64, self-contained, single-file)..."
try {
    Write-Info "Running: dotnet publish with parameters..."
    $publishArgs = @(
        "publish",
        "-c", "Release",
        "-r", "win-x64",
        "--self-contained",
        "true",
        "-p:PublishSingleFile=true",
        "-p:IncludeNativeLibrariesForSelfExtract=true",
        "-p:IncludeAllContentForSelfExtract=true"
    )
    
    & dotnet @publishArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Publish failed with exit code $LASTEXITCODE"
        Pop-Location
        exit 1
    }
    
    Write-Success "Release build published successfully"
}
catch {
    Write-Error-Custom "Failed to publish: $_"
    Pop-Location
    exit 1
}

# Verify publish output exists
if (-not (Test-Path $PublishOutput)) {
    Write-Error-Custom "Publish output folder not found: $PublishOutput"
    Pop-Location
    exit 1
}
Write-Success "Publish output verified: $PublishOutput"

Pop-Location

# Step 4: Prepare delivery folder
Write-Header "Step 3: Preparing delivery folder..."
try {
    if (Test-Path $DeliveryFolder) {
        Write-Info "Clearing existing delivery folder..."
        Remove-Item $DeliveryFolder -Recurse -Force | Out-Null
    }
    
    Write-Info "Creating delivery folder: $DeliveryFolder"
    New-Item -ItemType Directory -Path $DeliveryFolder -Force | Out-Null
    Write-Success "Delivery folder prepared"
}
catch {
    Write-Error-Custom "Failed to prepare delivery folder: $_"
    exit 1
}

# Step 5: Copy published files
Write-Header "Step 4: Copying published files..."
try {
    Write-Info "Copying from: $PublishOutput"
    Write-Info "Copying to: $DeliveryFolder"
    
    # Copy all files and directories
    $items = Get-ChildItem -Path $PublishOutput -Force
    foreach ($item in $items) {
        if ($item.PSIsContainer) {
            Copy-Item -Path $item.FullName -Destination $DeliveryFolder -Recurse -Force
        }
        else {
            Copy-Item -Path $item.FullName -Destination $DeliveryFolder -Force
        }
    }
    
    Write-Success "All files copied to delivery folder"
}
catch {
    Write-Error-Custom "Failed to copy files: $_"
    exit 1
}

# Step 6: Remove database file
Write-Header "Step 5: Cleaning up database..."
try {
    $dbPath = Join-Path $DeliveryFolder $DatabaseFileName
    if (Test-Path $dbPath) {
        Write-Info "Found $DatabaseFileName, removing..."
        Remove-Item $dbPath -Force
        Write-Success "Database file removed (will be created fresh on first run)"
    }
    else {
        Write-Info "No database file found (expected)"
    }
}
catch {
    Write-Error-Custom "Failed to remove database: $_"
    exit 1
}

# Step 7: Create README.txt
Write-Header "Step 6: Creating README.txt..."
try {
    $readmeContent = @"
================================================================================
  PHARMACY INVENTORY - CLIENT SETUP GUIDE
================================================================================

Thank you for using Pharmacy Inventory!

SYSTEM REQUIREMENTS:
- Windows 10 or later (x64)
- No additional software installation required (self-contained)

================================================================================
  INSTALLATION & SETUP
================================================================================

1. EXTRACT THE PACKAGE
   - Extract the entire PharmacyInventory folder to your desired location
   - Recommended: C:\PharmacyInventory\
   
   OR
   
   - Copy the PharmacyInventory folder directly to your system

2. RUN THE APPLICATION
   - Double-click PharmacyInventory.exe to launch the application

3. HANDLE WINDOWS SMARTSCREEN (if prompted)
   - Windows Defender SmartScreen may warn about the application
   - This is normal for new applications
   - Click "More info" → "Run anyway" to proceed
   - The application is safe and fully self-contained

4. DATABASE INITIALIZATION
   - On first run, the application will automatically create a database file
   - The database (pharmacy.db) will be saved in the same folder as the .exe
   - No additional configuration needed

================================================================================
  USAGE
================================================================================

DEFAULT LOGIN CREDENTIALS (for first-time setup):
- Admin:
  Username: admin
  Password: admin@123

- Cashier:
  Username: cashier
  Password: pass@123

After first login, consider changing these passwords for security.

MAIN FEATURES:
- Admin Dashboard: Manage products, inventory, reports, and settings
- Cashier Interface: Process sales, manage shopping cart, print receipts
- Inventory Management: Track product quantities and expiration dates
- Sales Reports: View daily and monthly sales statistics

================================================================================
  TROUBLESHOOTING
================================================================================

Q: The application won't start
A: Ensure Windows 10+ (x64), and all files are extracted properly

Q: Windows blocks the application
A: Click "More info" → "Run anyway" in the SmartScreen warning

Q: I get a database error
A: Delete pharmacy.db and restart the application to reinitialize

Q: Login fails
A: Verify username/password. Database may need to be reset (delete pharmacy.db)

================================================================================
  SUPPORT
================================================================================

For issues or questions, please contact your system administrator.

Database file location: [Application Folder]\pharmacy.db
Configuration: Automatic (no manual config needed)

================================================================================
"@
    
    $readmePath = Join-Path $DeliveryFolder "README.txt"
    $readmeContent | Out-File -FilePath $readmePath -Encoding UTF8 -Force
    Write-Success "README.txt created: $readmePath"
}
catch {
    Write-Error-Custom "Failed to create README.txt: $_"
    exit 1
}

# Step 8: Verify executable exists
Write-Header "Step 7: Verifying delivery package..."
try {
    $exePath = Join-Path $DeliveryFolder "PharmacyInventory.exe"
    if (-not (Test-Path $exePath)) {
        Write-Error-Custom "Executable not found: $exePath"
        exit 1
    }
    Write-Success "PharmacyInventory.exe verified"
    
    # Verify database NOT present
    $dbPath = Join-Path $DeliveryFolder "pharmacy.db"
    if (Test-Path $dbPath) {
        Write-Error-Custom "Database file still present (should be removed)"
        Remove-Item $dbPath -Force
    }
    Write-Success "No database file present (correct)"
    
    # Show delivery folder contents
    Write-Info "Delivery folder contents:"
    Get-ChildItem -Path $DeliveryFolder | Select-Object -ExpandProperty Name | ForEach-Object {
        Write-Host "  - $_"
    }
}
catch {
    Write-Error-Custom "Failed to verify delivery package: $_"
    exit 1
}

# Step 9: Optional - Create ZIP archive
if ($CreateZip) {
    Write-Header "Step 8: Creating ZIP archive..."
    try {
        # Remove existing ZIP if present
        if (Test-Path $DeliveryZip) {
            Write-Info "Removing existing ZIP file..."
            Remove-Item $DeliveryZip -Force
        }
        
        Write-Info "Creating: $DeliveryZip"
        Compress-Archive -Path "$DeliveryFolder\*" -DestinationPath $DeliveryZip -Force
        
        $zipSize = (Get-Item $DeliveryZip).Length / 1MB
        Write-Success "ZIP archive created ($([math]::Round($zipSize, 2)) MB)"
    }
    catch {
        Write-Error-Custom "Failed to create ZIP: $_"
        exit 1
    }
}

# ============================================================================
# Summary
# ============================================================================

Write-Header "DELIVERY PACKAGE COMPLETE ✓"

Write-Info "Delivery folder: $DeliveryFolder"
Write-Info "Executable: $(Join-Path $DeliveryFolder 'PharmacyInventory.exe')"
Write-Info "README: $(Join-Path $DeliveryFolder 'README.txt')"

if ($CreateZip) {
    Write-Info "ZIP archive: $DeliveryZip"
}

Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Yellow
Write-Host "1. Share the PharmacyInventory_ClientDelivery folder with the client"
if ($CreateZip) {
    Write-Host "   OR share the PHarmacyInventory_ClientDelivery.zip file"
}
Write-Host "2. Client should extract/copy to C:\PharmacyInventory\"
Write-Host "3. Client runs PharmacyInventory.exe"
Write-Host "4. Database will be created automatically on first run"
Write-Host ""
Write-Success "Ready for client delivery!"
