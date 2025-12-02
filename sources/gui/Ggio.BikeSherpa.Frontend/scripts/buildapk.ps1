# Expo APK Build Script for Windows PowerShell
$ErrorActionPreference = "Stop"

# Configuration
$KEYSTORE_FILE = "release.keystore"
$KEY_ALIAS = "release"
$KEYSTORE_PATH = "android\app\$KEYSTORE_FILE"
$GRADLE_PROPERTIES = "android\gradle.properties"

Write-Host "`n=== Expo APK Build Script ===`n" -ForegroundColor Green

# Check for Java/JDK
function Test-JavaInstalled {
    try {
        $null = Get-Command keytool -ErrorAction Stop
        return $true
    } catch {
        return $false
    }
}

if (-not (Test-JavaInstalled)) {
    Write-Host "Java JDK not found in PATH!" -ForegroundColor Red
    Write-Host "`nSearching for JDK in common locations..." -ForegroundColor Yellow
    
    # Common JDK locations on Windows
    $jdkPaths = @(
        "$env:ProgramFiles\Java\jdk*\bin",
        "$env:ProgramFiles\Android\Android Studio\jbr\bin",
        "$env:LOCALAPPDATA\Android\Sdk\jbr\bin",
        "C:\Program Files\Java\jdk*\bin",
        "C:\Program Files\Eclipse Adoptium\jdk*\bin"
    )
    
    $foundJdk = $null
    foreach ($pattern in $jdkPaths) {
        $matches = Get-ChildItem -Path $pattern -ErrorAction SilentlyContinue | Where-Object { Test-Path "$_\keytool.exe" }
        if ($matches) {
            $foundJdk = $matches[0].FullName
            break
        }
    }
    
    if ($foundJdk) {
        Write-Host "[OK] Found JDK at: $foundJdk" -ForegroundColor Green
        $env:PATH = "$foundJdk;$env:PATH"
        Write-Host "[OK] Added to PATH for this session" -ForegroundColor Green
        Write-Host "`nTo make this permanent, add to your system PATH:" -ForegroundColor Yellow
        Write-Host "  $foundJdk`n" -ForegroundColor Cyan
    } else {
        Write-Host "`n[ERROR] Java JDK not found!" -ForegroundColor Red
        Write-Host "`nPlease install Java JDK 17:" -ForegroundColor Yellow
        Write-Host "  Download from: https://adoptium.net/`n" -ForegroundColor Cyan
        Write-Host "Or if using Android Studio, the JDK is usually at:" -ForegroundColor Yellow
        Write-Host "  C:\Program Files\Android\Android Studio\jbr\bin`n" -ForegroundColor Cyan
        exit 1
    }
}

# Verify keytool works now
if (Get-Command keytool -ErrorAction SilentlyContinue) {
    Write-Host "[OK] Java keytool is available`n" -ForegroundColor Green
} else {
    Write-Host "[ERROR] keytool still not accessible!" -ForegroundColor Red
    exit 1
}

# Prebuild
if (-not (Test-Path "android")) {
    Write-Host "Android folder not found. Running expo prebuild..." -ForegroundColor Yellow
    npx expo prebuild --platform android
    Write-Host "[OK] Native Android code generated`n" -ForegroundColor Green
} else {
    $newBuild = Read-Host "Do you want to build a clean apk? [y/n]"
    if ($newBuild -eq "y" -or $newBuild -eq "Y") {
        Write-Host "`nRunning clean expo prebuild...`n" -ForegroundColor Yellow
        npx expo prebuild --platform android --clean
        Write-Host "[OK] Native Android code generated`n" -ForegroundColor Green
    }
}

# Generate keystore if needed
if (-not (Test-Path $KEYSTORE_PATH)) {
    Write-Host "Keystore not found. Generating new keystore..." -ForegroundColor Yellow
    
    $keystorePassword = Read-Host "Enter keystore password (min 6 characters)" -AsSecureString
    $keystorePasswordConfirm = Read-Host "Confirm keystore password" -AsSecureString
    
    # Convert SecureString to plain text for comparison
    $pwd1 = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($keystorePassword))
    $pwd2 = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($keystorePasswordConfirm))
    
    if ($pwd1 -ne $pwd2) {
        Write-Host "[ERROR] Passwords don't match!" -ForegroundColor Red
        exit 1
    }
    
    if ($pwd1.Length -lt 6) {
        Write-Host "[ERROR] Password must be at least 6 characters!" -ForegroundColor Red
        exit 1
    }
    
    # Get app details from app.json
    $appJsonContent = Get-Content "app.json" -Raw | ConvertFrom-Json
    $APP_NAME = $appJsonContent.expo.name
    if (-not $APP_NAME) { $APP_NAME = "Ggio.BikeSherpa.Frontend" }
    
    $PACKAGE_NAME = $appJsonContent.expo.android.package
    if (-not $PACKAGE_NAME) { $PACKAGE_NAME = "com.anonymous.ggiobikesherpafrontend" }
    
    # Generate keystore
    & keytool -genkeypair -v `
        -storetype PKCS12 `
        -keystore $KEYSTORE_PATH `
        -alias $KEY_ALIAS `
        -keyalg RSA `
        -keysize 2048 `
        -validity 10000 `
        -storepass $pwd1 `
        -keypass $pwd1 `
        -dname "CN=$APP_NAME, OU=Mobile, O=$PACKAGE_NAME, L=Grenoble, ST=France, C=FR"
    
    Write-Host "[OK] Keystore generated at $KEYSTORE_PATH" -ForegroundColor Green
    Write-Host "[WARNING] IMPORTANT: Keep this keystore file safe! You'll need it for future updates.`n" -ForegroundColor Yellow
    
    # Update gradle.properties
    if (Test-Path $GRADLE_PROPERTIES) {
        $content = Get-Content $GRADLE_PROPERTIES | Where-Object { $_ -notmatch "MYAPP_RELEASE_" }
        $content | Set-Content $GRADLE_PROPERTIES
    }
    
    # Append signing configuration
    $signingConfig = @"

# Release signing configuration
MYAPP_RELEASE_STORE_FILE=$KEYSTORE_FILE
MYAPP_RELEASE_KEY_ALIAS=$KEY_ALIAS
MYAPP_RELEASE_STORE_PASSWORD=$pwd1
MYAPP_RELEASE_KEY_PASSWORD=$pwd1
"@
    Add-Content $GRADLE_PROPERTIES $signingConfig
    
    Write-Host "[OK] gradle.properties configured`n" -ForegroundColor Green
} else {
    Write-Host "[OK] Keystore found`n" -ForegroundColor Green
}

# Check and update build.gradle if needed
$BUILD_GRADLE = "android\app\build.gradle"
$buildGradleContent = Get-Content $BUILD_GRADLE -Raw

if ($buildGradleContent -notmatch "signingConfig signingConfigs\.release") {
    Write-Host "Configuring build.gradle for release signing..." -ForegroundColor Yellow
    
    # Backup
    Copy-Item $BUILD_GRADLE "$BUILD_GRADLE.bak"
    
    # Add release signingConfig block after debug block
    if ($buildGradleContent -match "signingConfigs\s*\{\s*debug\s*\{[^}]+\}") {
        # Add release config after debug config
        $releaseConfigBlock = @'
        release {
            if (project.hasProperty('MYAPP_RELEASE_STORE_FILE')) {
                storeFile file(project.property('MYAPP_RELEASE_STORE_FILE'))
                storePassword project.property('MYAPP_RELEASE_STORE_PASSWORD')
                keyAlias project.property('MYAPP_RELEASE_KEY_ALIAS')
                keyPassword project.property('MYAPP_RELEASE_KEY_PASSWORD')
            }
        }
'@
        $buildGradleContent = $buildGradleContent -replace `
            '(debug\s*\{\s*storeFile[^}]+\}\s*)(\n\s*\})', `
            "`$1`n$releaseConfigBlock`$2"
    }
    
    # Change release buildType to use release signingConfig
    $buildGradleContent = $buildGradleContent -replace `
        '(release\s*\{[^}]*?)signingConfig signingConfigs\.debug', `
        '$1signingConfig signingConfigs.release'
    
    $buildGradleContent | Set-Content $BUILD_GRADLE -NoNewline
    Write-Host "[OK] build.gradle configured`n" -ForegroundColor Green
} else {
    Write-Host "[OK] build.gradle already configured for release signing`n" -ForegroundColor Green
}

# Build APK
Write-Host "Building release APK..." -ForegroundColor Green
Set-Location android

# Clean previous builds
.\gradlew.bat clean

# Build release
.\gradlew.bat assembleRelease

Set-Location ..

# Check result
$APK_PATH = "android\app\build\outputs\apk\release\app-release.apk"
if (Test-Path $APK_PATH) {
    $size = (Get-Item $APK_PATH).Length / 1MB
    Write-Host "`n[SUCCESS] APK built successfully" -ForegroundColor Green
    Write-Host "Location: $APK_PATH" -ForegroundColor Yellow
} else {
    Write-Host "`n[ERROR] Build failed! APK not found at expected location." -ForegroundColor Red
    exit 1
}