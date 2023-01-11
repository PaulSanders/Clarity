# Clarity
This is the original library I published on CodePlex back in 2014. The original code can be found via http://archive.org/. Specifically [here](http://https://web.archive.org/web/20180424030009/https://archive.codeplex.com/?p=clarity "Archive of Clarity on CodePlex")

Note that only basic WinForms support was achieved at the time.

![ViewModel](https://i.ibb.co/yVy2KN2/clarity.png "Example ViewModel implementation with Property Change handling")

## Introduction to Clarity

Clarity is a minimal library that was developed to provide a number of key features:
- Comprehensive Model implementation including change tracking, validation and property change monitoring and delegation
- Loose coupling via the built-in MessageBus
- DesignTime View display, and automatic view resolution
- Simple IOC/DI implementation
- Base Disposable implementation used throughout
- Built-in Model validation
- UI Agnostic
- Built-in change tracking and n-level undo
- Abstracted bootstrap process

UI specific libraries for Wpf and Winforms
Clarity.Wpf
-- Provides Wpf custom command bindings integrated into the normal microsoft namespaces to minimise code


## I love the power of using an internal MessageBus!
Below is a small extract from the CustomerBrowser sample project. This project allows the user to switch between a windowed and tabbed approach for displaying data

Define some application level messages. Yes, this could be simplified to having a base class with the required public Customer property
![Sample Messages](https://i.ibb.co/gVKDPfm/Example-Messages.png "Define some messages")

Subscribe to the OnAddCustomerMessage, OnEditCustomerMessage and return the relevant ViewModel
![Handle Messages](https://i.ibb.co/WgkxWdk/Message-Handler.png "Handle Adding a new Customer or editing an existing one")

## Samples

This solution contains a number of samples for how to utilize the library.
These are all legacy and have not been tested since migration from CodePlex.

- FileExplorer. Very basic sample with a single library holding filesystem objects. Separate WPF and Winforms implementation use that for binding
- CustomerBrowser. Sample demonstrates ability to switch between windows and tabbed approach to managing customers by making use of MessageBus
- SearchExample. Legacy application to get weather information about a particular country. The public service has been discontinued. TODO: Re-write to use https://open-meteo.com/
- SimpleEditor. Shows how to use change tracking to manage state change
- WinformsEarningsCalculator. Example showing Winforms binding.

