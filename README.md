## Introduction

openHAB Windows application is a native client for openHAB. It uses REST API of openHAB to render
sitemaps of your openHAB. 
The app is currently under heavy development and far from MVP status.

## Builds

[![CI Build status](https://github.com/openhab/openhab-windows/workflows/CI/badge.svg)](https://github.com/openhab/openhab-windows/actions?query=workflow%3A%22CI+Build%22)
[![Latest App Release Build](https://github.com/openhab/openhab-windows/workflows/App%20Release/badge.svg)](https://github.com/openhab/openhab-windows/actions?query=workflow%3A%22App+Release%22)


## Setting up development environment

If you want to contribute to the Windows application we are here to help you to set up
development environment. openHAB Windows app is developed using Visual Studio 2019 and later.

- Download and install [Visual Studio Community Edition](https://www.visualstudio.com/downloads/)
- During install, make sure to select UWP SDK 17134 and SDK 18362
- Check out the latest code from github
- Open the project in Visual Studio (File -> Open, Project/Solution)
- Rebuild the solution to fetch all missing NuGet packages

You are ready to contribute!

Before producing any amount of code please have a look at [contribution guidelines](https://github.com/openhab/openhab.windows/blob/master/CONTRIBUTING.md)

## Trademark Disclaimer

Product names, logos, brands and other trademarks referred to within the openHAB website are the
property of their respective trademark holders. These trademark holders are not affiliated with
openHAB or our website. They do not sponsor or endorse our materials.
