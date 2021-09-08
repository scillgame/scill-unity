# SCILL Unity SDK

SCILL gives you the tools to activate, retain and grow your user base in your app or game by bringing you features well known 
in the gaming industry: Gamification. We take care of the services and technology involved so you can focus on your game and content.

This SDK is built on top of the SCILL C# SDK and adds Components, Prefabs and fully functional implementations for Challenges, Battle Passes and Leaderboards.

## About SCILL

SCILL is a fully customizable toolkit that enables you to integrate tailored challenges, full-fledged Battle Passes and Leaderboards to your game, app and website. SCILL connects seamlessly to your user account and payment systems, adding new retention 
and monetization layers within minutes.

Learn more about SCILL here: [https://www.scillgame.com](https://www.scillgame.com).

Developer documentation can be found here: [https://developers.scillgame.com](https://developers.scillgame.com/sdks/csharp.html) 

## Getting Started

You can install the SDK using different methods:

### Package Manager: Adding from git URL

Use "add a package from git URL..." with this URL: https://github.com/crystal-mesh/scill-unity.git?path=/unity-package

### Package Manager: Adding from disk

Clone the repository to your local disk and use "add package from disk...". Navigate to "repository-path/unity-package" and select the package.json file.

### Unity Package

You can download the SCILL Unity SDK as a .unitypackage from our Github releases page at https://github.com/scillgame/scill-unity/releases/latest

Because Unity does not resolve dependencies automatically when importing Assets as a `.unitypackage`, you will need to ensure that Unity’s Newtonsoft-JSON package has been added to your project. To add the Newtonsoft package to your project, navigate to the Package Manager by using the `Window` -> `Package Manager` menu. In the Package Manager window, click on the + button, choose `Add package from git URL ...` and enter the package reference: `com.unity.nuget.newtonsoft-json`

## Documentation

Extensive documentation, getting started guides and example code can be found in our [SCILL Unity SDK documentation](https://developers.scillgame.com/sdks/unity.html).
