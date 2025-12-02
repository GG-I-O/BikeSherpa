#!/bin/bash
set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
KEYSTORE_FILE="release.keystore"
KEY_ALIAS="release"
KEYSTORE_PATH="android/app/$KEYSTORE_FILE"
GRADLE_PROPERTIES="android/gradle.properties"

echo -e "${GREEN}=== Expo APK Build Script ===${NC}\n"

# Check for Java/JDK and auto-configure if needed
if ! command -v keytool &> /dev/null; then
    echo -e "${YELLOW}Java JDK not found in PATH!${NC}"
    echo -e "${YELLOW}Searching for JDK in common locations...${NC}\n"
    
    # Common JDK locations
    POSSIBLE_PATHS=(
        "/usr/lib/jvm/java-17-openjdk-amd64/bin"
        "/usr/lib/jvm/java-11-openjdk-amd64/bin"
        "/usr/lib/jvm/default-java/bin"
        "$HOME/Android/Studio/jbr/bin"
        "/opt/android-studio/jbr/bin"
        "/Applications/Android Studio.app/Contents/jbr/Contents/Home/bin"
        "/opt/homebrew/opt/openjdk@17/bin"
        "/usr/local/opt/openjdk@17/bin"
    )
    
    JDK_FOUND=false
    for path in "${POSSIBLE_PATHS[@]}"; do
        if [ -f "$path/keytool" ]; then
            echo -e "${GREEN}Found JDK at: $path${NC}"
            export JAVA_HOME="${path%/bin}"
            export PATH="$path:$PATH"
            JDK_FOUND=true
            echo -e "${GREEN}Added to PATH for this session${NC}\n"
            echo -e "${YELLOW}To make this permanent, add to your ~/.bashrc or ~/.zshrc:${NC}"
            echo -e "${CYAN}export JAVA_HOME=$JAVA_HOME${NC}"
            echo -e "${CYAN}export PATH=\$PATH:\$JAVA_HOME/bin${NC}\n"
            break
        fi
    done
    
    if [ "$JDK_FOUND" = false ]; then
        echo -e "${RED}Java JDK not found!${NC}\n"
        echo -e "${YELLOW}Please install Java JDK 17:${NC}"
        echo -e "${CYAN}  Ubuntu/Debian: sudo apt install openjdk-17-jdk${NC}"
        echo -e "${CYAN}  macOS: brew install openjdk@17${NC}"
        echo -e "${CYAN}  Or download from: https://adoptium.net/${NC}\n"
        exit 1
    fi
fi

# Verify keytool works
if keytool -help &> /dev/null; then
    echo -e "${GREEN}Java keytool is available${NC}\n"
else
    echo -e "${RED}keytool is not working properly${NC}"
    exit 1
fi

# Prebuild
if [ ! -d "android" ]; then
    echo -e "${YELLOW}Android folder not found. Running expo prebuild...${NC}"
    npx expo prebuild --platform android
    echo -e "${GREEN}Native Android code generated${NC}\n"
else
    read -p "Do you want to build a clean apk? [y/n] " NEWBUILD
    echo
    if [ "$NEWBUILD" = "y" ] || [ "$NEWBUILD" = "Y" ]; then
        echo -e "${YELLOW}Running clean expo prebuild...${NC}\n"
        npx expo prebuild --platform android --clean
        echo -e "${GREEN}Native Android code generated${NC}\n"
    fi
fi

# Generate key when needed
if [ ! -f "$KEYSTORE_PATH" ]; then
    echo -e "${YELLOW}Keystore not found. Generating new keystore...${NC}"

    read -sp "Enter keystore password (min 6 characters): " KEYSTORE_PASSWORD
    echo
    read -sp "Confirm keystore password: " KEYSTORE_PASSWORD_CONFIRM
    echo

    if [ "$KEYSTORE_PASSWORD" != "$KEYSTORE_PASSWORD_CONFIRM" ]; then
        echo -e "${RED}Passwords don't match!${NC}"
        exit 1
    fi
    
    if [ ${#KEYSTORE_PASSWORD} -lt 6 ]; then
        echo -e "${RED}Password must be at least 6 characters!${NC}"
        exit 1
    fi
    
    # Get app details from app.json
    APP_NAME=$(grep -o '"name"[[:space:]]*:[[:space:]]*"[^"]*"' app.json | head -1 | sed 's/"name"[[:space:]]*:[[:space:]]*"\([^"]*\)"/\1/')
    PACKAGE_NAME=$(grep -o '"package"[[:space:]]*:[[:space:]]*"[^"]*"' app.json | head -1 | sed 's/"package"[[:space:]]*:[[:space:]]*"\([^"]*\)"/\1/')
    
    if [ -z "$APP_NAME" ]; then
        APP_NAME="Ggio.BikeSherpa.Frontend"
    fi
    
    if [ -z "$PACKAGE_NAME" ]; then
        PACKAGE_NAME="com.anonymous.ggiobikesherpafrontend"
    fi
    
    # Generate keystore
    keytool -genkeypair -v \
        -storetype PKCS12 \
        -keystore "$KEYSTORE_PATH" \
        -alias "$KEY_ALIAS" \
        -keyalg RSA \
        -keysize 2048 \
        -validity 10000 \
        -storepass "$KEYSTORE_PASSWORD" \
        -keypass "$KEYSTORE_PASSWORD" \
        -dname "CN=$APP_NAME, OU=Mobile, O=$PACKAGE_NAME, L=Grenoble, ST=France, C=FR"
    
    echo -e "${GREEN}Keystore generated at $KEYSTORE_PATH${NC}"
    echo -e "${YELLOW}IMPORTANT: Keep this keystore file safe! You'll need it for future updates.${NC}\n"
    
    # Update or create gradle.properties
    if [ -f "$GRADLE_PROPERTIES" ]; then
        # Remove existing signing config if present
        sed -i.bak '/MYAPP_RELEASE_/d' "$GRADLE_PROPERTIES"
    fi
    
    # Append signing configuration
    cat >> "$GRADLE_PROPERTIES" << EOF

# Release signing configuration
MYAPP_RELEASE_STORE_FILE=$KEYSTORE_FILE
MYAPP_RELEASE_KEY_ALIAS=$KEY_ALIAS
MYAPP_RELEASE_STORE_PASSWORD=$KEYSTORE_PASSWORD
MYAPP_RELEASE_KEY_PASSWORD=$KEYSTORE_PASSWORD
EOF
    
    echo -e "${GREEN}gradle.properties configured${NC}\n"
else
    echo -e "${GREEN}Keystore found${NC}\n"
fi

# Check and update build.gradle if needed
BUILD_GRADLE="android/app/build.gradle"
if ! grep -q "signingConfig signingConfigs.release" "$BUILD_GRADLE"; then
    echo -e "${YELLOW}Configuring build.gradle for release signing...${NC}"
    
    # Backup original file
    cp "$BUILD_GRADLE" "$BUILD_GRADLE.bak"
    
    # Add release signing config if not present
    if ! grep -q "signingConfigs {" "$BUILD_GRADLE"; then
        # Insert signingConfigs section before buildTypes
        sed -i.tmp '/buildTypes {/i\
    signingConfigs {\
        release {\
            if (project.hasProperty("MYAPP_RELEASE_STORE_FILE")) {\
                storeFile file(project.property("MYAPP_RELEASE_STORE_FILE"))\
                storePassword project.property("MYAPP_RELEASE_STORE_PASSWORD")\
                keyAlias project.property("MYAPP_RELEASE_KEY_ALIAS")\
                keyPassword project.property("MYAPP_RELEASE_KEY_PASSWORD")\
            }\
        }\
    }\
' "$BUILD_GRADLE"
    fi
    
    # Add signingConfig to release buildType if not present
    if ! grep -q "signingConfig signingConfigs.release" "$BUILD_GRADLE"; then
        sed -i.tmp '/buildTypes {/,/release {/a\
            signingConfig signingConfigs.release
' "$BUILD_GRADLE"
    fi
    
    rm -f "$BUILD_GRADLE.tmp"
    echo -e "${GREEN}build.gradle configured${NC}\n"
fi

# Build the APK
echo -e "${GREEN}Building release APK...${NC}"
cd android

# Clean previous builds
./gradlew clean

# Build release APK
./gradlew assembleRelease

cd ..

# Check result
APK_PATH="android/app/build/outputs/apk/release/app-release.apk"
if [ -f "$APK_PATH" ]; then
    APK_SIZE=$(du -h "$APK_PATH" | cut -f1)
    echo -e "\n${GREEN}SUCCESS! APK built successfully${NC}"
    echo -e "Location: ${YELLOW}$APK_PATH${NC}"
else
    echo -e "${RED}Build failed! APK not found at expected location.${NC}"
    exit 1
fi