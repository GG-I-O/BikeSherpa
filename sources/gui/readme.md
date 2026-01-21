# Initialisation

## Prérequis
node & npm
```sh
nvm install node --reinstall-packages-from=node
nvm use node

or

npm install -g npm@latest
```

## Install

```sh
# Create the project from scratch
npx create-expo-app@latest Ggio.BikeSherpa.Frontend
cd Ggio.BikeSherpa.Frontend
npm run reset-project

# Download packages
npm update
```

## Launch

### Web

```sh
npx expo start --clear
```

#### HTTPS
##### Setup on PC
```sh
# linux : Add to /etc/hosts
127.0.0.1 dev.bike.local

# windows : Add to C:\Windows\System32\drivers\etc\hosts
127.0.0.1 dev.bike.local
```

##### Generate Certificate
```sh
# generate SSL certificate
# Windows :
choco install mkcert
mkcert dev.bike.local
mkcert --install
```

##### Proxy
```sh
# proxy from 8081 to 443
npm run startHTTPS
```

### Mobile

```sh
npx expo prebuild --clean
npx expo run:android
```

## Release
Follow the steps or run the command :  
```sh
npm run build:apk:windows
or
npm run build:apk:linux
```

### Android

#### Prérequis
JDK 17  
Android Studio  
Variables d'environnement ANDROID_HOME  

#### Prebuild
```sh
npx expo prebuild --platform android --clean
```

#### Generate key
```sh
cd android/app
keytool -genkeypair -v -storetype PKCS12 -keystore release.keystore -alias release -keyalg RSA -keysize 2048 -validity 10000
```

#### Config
android/gradle.properties
```sh
MYAPP_RELEASE_STORE_FILE=my-release-key.keystore
MYAPP_RELEASE_KEY_ALIAS=my-key-alias
MYAPP_RELEASE_STORE_PASSWORD=your_keystore_password
MYAPP_RELEASE_KEY_PASSWORD=your_key_password
```  

android/app/build.gradle
```java
android {
    // All config remain the same except those lines
    signingConfigs {
	    // No changes there
        debug {
            storeFile file('debug.keystore')
            storePassword 'android'
            keyAlias 'androiddebugkey'
            keyPassword 'android'
        }
	    // Add this here
        release {
            if (project.hasProperty('MYAPP_RELEASE_STORE_FILE')) {
                storeFile file(project.property('MYAPP_RELEASE_STORE_FILE'))
                storePassword project.property('MYAPP_RELEASE_STORE_PASSWORD')
                keyAlias project.property('MYAPP_RELEASE_KEY_ALIAS')
                keyPassword project.property('MYAPP_RELEASE_KEY_PASSWORD')
            }
        }
    }
    buildTypes {
        // Modify only this line, keep the rest
        release {
            signingConfig signingConfigs.release
        }
    }
}
```

#### Build APK
```sh
cd android
./gradlew assembleRelease
```

#### Enjoy
APK file is in
```sh
android/app/build/outputs/apk/release/app-release.apk
```

# Authentication
## Auth0
### Setup on Auth0 dashboard
Create a new application (SPA for web, Native for android)  
Make one by environmnent
### Setup on Expo
Put infos of this tenant in .env.local (ex: .env.blank)

# Packages
## Expo

React native framework and its library

```sh
npx expo install expo-auth-session expo-crypto
npx expo install expo-network
npx expo install expo-secure-store
```

## React native Auth0
Authentification with Auth0

```sh
npm install react-native-auth0 --save
```

### Config
Add and complete in .env.local ->
```sh
# Authentification
EXPO_PUBLIC_AUTH_DOMAIN=
EXPO_PUBLIC_AUTH_CLIENT_WEB=
EXPO_PUBLIC_AUTH_CLIENT_ANDROID=
EXPO_PUBLIC_AUTH_SCOPE=openid profile email offline_access
```

## React native logs
Log services interface

```sh
npm install react-native-logs
```

### Config
Docker compose [compose.yml](/infrastructure/logger/compose.yml)
```sh
docker compose up
```

Add and complete in .env.local ->
```sh
# Logger
EXPO_PUBLIC_LOKI_URL=
```

## React native paper

graphic component with Material3 theme

```sh
npm install react-native-paper react-native-paper-dropdown react-native-paper-dates
```

## Axios

API calls with interceptors

```sh
npm install axios
```

## react-native-event-listeners

Create an event listener

```sh
npm install react-native-event-listeners
```

## Inversify

Dependency injection with @injectable / @inject
Babel is needed to use decorator in typescript and expo project

```sh
npm install inversify reflect-metadata
npm install --save-dev @babel/plugin-proposal-decorators babel-plugin-transform-typescript-metadata
```

### Config
Add in tsconfig.json ->

```json
"compilerOptions": {
    "experimentalDecorators": true,
    "emitDecoratorMetadata": true,
}
```

## Zod

Create validator schema

```sh
npm install zod
```

## React hook form

Prérequis : Zod
Validate form from a Zod schema

```sh
npm install react-hook-form @hookform/resolvers
```

## Legend app & MMKV

Store data locally and update local data with API/notification when possible

```sh
npm install @legendapp/state@beta
npx expo install react-native-mmkv react-native-nitro-modules
```

## SignalR

Subscribe and receive notification

```sh
npm install @microsoft/signalr
```

## OpenAPI zod client & Zodios

Generate API url from back-end openAPI json

```sh
npm install --save-dev openapi-zod-client
npm install @zodios/core
```

If zodios have dependency errors  
```sh
npm uninstall axios
npm uninstall zod
npm install @zodios/core zod axios
```

Usage  
```sh
# To create an openAPI file from a swagger url
npm run openAPI:web
# To create an openAPI file from a swagger file
npm run openAPI:file
```

## Jest

Unit testing

Warning : @testing-library/react-native need react-test-renderer, and react-test-renderer need to be at the EXACT same version as react

```sh
npx expo install jest-expo jest @types/jest --dev
npm install --save-dev react-test-renderer@19.1.0
npx expo install @testing-library/react-native --dev
```

```json
Package.json :
"jest": {
    "preset": "jest-expo",
    "transformIgnorePatterns": [
        "node_modules/(?!((jest-)?react-native|@react-native(-community)?)|expo(nent)?|@expo(nent)?/.*|@expo-google-fonts/.*|react-navigation|@react-navigation/.*|@sentry/react-native|native-base|react-native-svg)",
        "eslint.config.js"
    ],
    "collectCoverage": true,
    "collectCoverageFrom": [
        "**/*.{ts,tsx,js,jsx}",
        "!**/coverage/**",
        "!**/node_modules/**",
        "!**/babel.config.js",
        "!**/expo-env.d.ts",
        "!**/.expo/**"
    ]
},
```
