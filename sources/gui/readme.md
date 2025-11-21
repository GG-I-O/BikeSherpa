# Initialisation

## Prérequis 
- npm  
npm update :  
```sh
nvm install node --reinstall-packages-from=node
nvm use node

or

npm install -g npm@latest
```

## Install
```sh
npm install -g npm@latest
npx create-expo-app@latest Ggio.BikeSherpa.Frontend
cd Ggio.BikeSherpa.Frontend
npm run reset-project
npm update
```

## Launch

### Web
```sh
npx expo start --clear
```

### Mobile
```sh
npx expo prebuild --clean
npx expo start --clear
```

# Packages

## Expo
React native framework and its library

```sh
npx expo install expo-auth-session expo-crypto
npx expo install expo-network
npx expo install expo-secure-store
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

## Inversify
Dependency injection with @injectable / @inject

```sh
npm install inversify reflect-metadata
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

## OpenAPI zod client
Generate API url from back-end openAPI json

```sh
npm install --save-dev openapi-zod-client
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