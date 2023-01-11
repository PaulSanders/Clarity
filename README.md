# Clarity
This is the original library I published on CodePlex back in 2014. The original code can be found via http://archive.org/
Note that only basic WinForms support was achieved at the time.

![image](https://ibb.co/PwLYvYV)

## Introduction to Clarity

Clarity is a minimal library that was developed to provide a number of key features:
- **Comprehensive Model implementation including change tracking, validation and property change monitoring and delegation
- **Loose coupling via the built-in MessageBus
- **DesignTime View display, and automatic view resolution
- **Simple IOC/DI implementation
- **Base Disposable implementation used throughout
- **Built-in Model validation
- **UI Agnostic
- **Built-in change tracking and n-level undo
- **Abstracted bootstrap process

Clarity.Wpf
-- **Provides Wpf custom command bindings integrated into the normal microsoft namespaces to minimise code

##I love the power of using an internal MessageBus!
Below is a small extract from the CustomerBrowser sample project. This project allows the user to switch between a windowed and tabbed approach for displaying data

Define some application level events
![image](https://ibb.co/ZXXsvjq)

Subscribe to the OnAddCustomerMessage, OnEditCustomerMessage and return the relevant ViewModel
![image](https://ibb.co/QH5FcTR)


## Samples

This solution contains a number of samples for how to utilize the library.
These are all legacy and have not been tested since migration from CodePlex.
