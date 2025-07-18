# IndoorsSDK Unity3D Example

This repository demonstrates how to use **IndoorsSDK** in Unity to display indoor positioning on **iOS** and **Android** devices. The project shows that the SDK is functional: it loads indoor map data and, if nearby BLE beacons (iBeacons) are detected, the user's position inside the building is calculated and printed in the console.

---

## 📦 What's Included

- Sample Unity scene integrated with IndoorsSDK
- Native plugins for Android and iOS
- Indoor location data loading and position tracking example
- Sample indoor map data

---

## 🚀 Getting Started

### Unity Setup

- Recommended Unity version: **2022.3.6f1 LTS**
- Open the project in Unity Hub and make sure the correct platform modules (iOS/Android) are installed.
- Go to **File → Build Settings** and select your target platform (iOS or Android).
- Open and run the `MainScene` to test locally in the editor (position data won't work in the editor).

---

### Beacon Configuration Required

To start receiving indoor position data from the Indoors SDK, you need to configure at least one physical iBeacon with the following parameters:

- **UUID:** `EF12688D-38CF-36CB-8811-1F7C2F6B33E2`  
- **Major:** `10272`  
- **Minor:** `317`

Make sure the beacon is powered on and within range of your test device. The SDK will only provide positioning data if at least one correctly configured beacon is detected nearby.

---

## 📱 Building for iOS

### Requirements

- **Unity 2022.3.6f1**
- **Xcode**
- **CocoaPods** (Install via `sudo gem install cocoapods` if not already installed)
- Xcode Command Line Tools

### Steps

1. In Unity, build the project for iOS:  
   `File → Build Settings → iOS → Build`.
2. After the build completes, navigate to the output folder.
3. Check for the existence of `Unity-iPhone.xcworkspace`.

   - ✅ If it **exists**, open it in Xcode.
   - ❌ If it **does not exist**, but you see `Unity-iPhone.xcodeproj`, initialize CocoaPods manually:

     ```bash
     pod init
     ```

4. In the generated `Podfile`, make sure the target is set to `Unity-iPhone`, and include any necessary pods.
5. Then install the pods:

   ```bash
   pod install
   ```
---
   
# 🤖 Building for Android

## Requirements
- **Unity 2022.3.6f1**
- **Android Studio** (Latest stable version)
- **JDK 11** (Required for Unity 2022)
- **Gradle 7.x** (Recommended for compatibility)
- **Android SDK**:
  - Min SDK: 24 (Android 7.0 Nougat)
    
## Steps

1. **Build Android Project in Unity**  
   `File → Build Settings → Android → Switch Platform → Build`  
2. **After the build completes, navigate to the output folder.**
