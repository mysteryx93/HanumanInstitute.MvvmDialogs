global using System.ComponentModel;
global using System.Runtime.CompilerServices;
global using Avalonia;
global using Avalonia.Controls;
global using Avalonia.Headless;
global using Avalonia.Markup.Xaml;
global using Avalonia.Threading;
global using HanumanInstitute.MvvmDialogs;
global using HanumanInstitute.MvvmDialogs.Avalonia;
global using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;
global using HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;
global using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
global using Moq;
global using ReactiveUI;
global using Xunit;
using Xunit.Sdk;
using Xunit.v3;

// Headless sessions reset Avalonia's shared dispatcher and render loop.
[assembly: Parallelization(Mode = ParallelMode.None)]
